using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Portfolio.Data;

/// <summary>
/// Lets Entity Framework's command-line tools (<c>dotnet ef migrations add</c>) build a context without starting the app.
/// </summary>
/// <remarks>
/// It is used only by the tools, never by the running app. Adding a migration only reads the model and does not
/// connect, so the fallback below is never actually opened; it is a local development address with Windows
/// authentication, so there is no secret in it. The app itself requires the connection string from configuration.
/// </remarks>
internal sealed class PortfolioDbContextFactory : IDesignTimeDbContextFactory<PortfolioDbContext>
{
    private const string LocalDevelopmentConnection = "Server=localhost;Database=PortfolioSite;Integrated Security=true;TrustServerCertificate=true";

    public PortfolioDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Portfolio") ?? LocalDevelopmentConnection;
        var options = new DbContextOptionsBuilder<PortfolioDbContext>();
        PortfolioDbContext.Configure(options, connectionString);
        return new PortfolioDbContext(options.Options);
    }
}
