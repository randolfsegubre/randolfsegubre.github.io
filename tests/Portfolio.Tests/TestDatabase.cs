using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Business;
using Portfolio.Business.Services;
using Portfolio.Data;

namespace Portfolio.Tests;

/// <summary>
/// A real, throwaway SQL Server database for one test or one test class, created with the app's own migrations and
/// deleted afterwards (ADR-0012).
/// </summary>
/// <remarks>
/// The server comes from the environment variable <c>PORTFOLIO_TEST_SQL</c>, a connection string without a database
/// name (the deploy workflow starts a SQL Server container and sets it). When it is not set, the local SQL Server on
/// this machine is used with Windows authentication. If no server can be reached the tests fail with the connection
/// error; they are never skipped, because a skipped test is a silent hole in exactly the part that needs proving.
/// Each database has a random name, so tests never touch the real <c>PortfolioSite</c> database or anyone else's.
/// </remarks>
internal sealed class TestDatabase : IAsyncDisposable
{
    private const string LocalServer = "Server=localhost;Integrated Security=true;TrustServerCertificate=true";

    private readonly string connectionString;

    private TestDatabase(string connectionString, ServiceProvider services)
    {
        this.connectionString = connectionString;
        Services = services;
    }

    /// <summary>The app's own services, wired to this database, exactly as the real app wires them.</summary>
    public ServiceProvider Services { get; }

    /// <summary>Creates a new empty database and applies the migrations to it.</summary>
    public static async Task<TestDatabase> CreateAsync()
    {
        var server = Environment.GetEnvironmentVariable("PORTFOLIO_TEST_SQL");
        var connection = $"{(string.IsNullOrWhiteSpace(server) ? LocalServer : server).TrimEnd(';')};Database=PortfolioSite_Test_{Guid.NewGuid():N}";

        var services = new ServiceCollection().AddPortfolioContent(_ => connection).BuildServiceProvider();
        var database = new TestDatabase(connection, services);

        await database.InScopeAsync(provider => provider.GetRequiredService<IContentMaintenance>().MigrateAsync());
        return database;
    }

    /// <summary>Runs some work in its own scope, as one request or one build command would.</summary>
    public async Task<T> InScopeAsync<T>(Func<IServiceProvider, Task<T>> work)
    {
        await using var scope = Services.CreateAsyncScope();
        return await work(scope.ServiceProvider);
    }

    /// <summary>Runs some work in its own scope, as one request or one build command would.</summary>
    public async Task InScopeAsync(Func<IServiceProvider, Task> work)
    {
        await using var scope = Services.CreateAsyncScope();
        await work(scope.ServiceProvider);
    }

    public async ValueTask DisposeAsync()
    {
        await Services.DisposeAsync();

        // Connections are pooled, and a pooled connection keeps the database in use, so let go of them before dropping it.
        SqlConnection.ClearAllPools();
        var options = new DbContextOptionsBuilder<PortfolioDbContext>();
        PortfolioDbContext.Configure(options, connectionString);
        await using var context = new PortfolioDbContext(options.Options);
        await context.Database.EnsureDeletedAsync();
    }
}
