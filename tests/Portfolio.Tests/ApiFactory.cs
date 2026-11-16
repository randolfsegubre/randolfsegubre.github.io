using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Portfolio.Business.Content;

namespace Portfolio.Tests;

/// <summary>
/// Starts the real API in memory with the content taken from the data files instead of SQL Server (ADR-0012).
/// </summary>
/// <remarks>
/// The API tests are about the web layer: its endpoint, its headers, what it refuses. Swapping the content source keeps
/// them fast and database-free, while everything else is the real app. That the real source returns exactly the same
/// content is proved separately, against a real SQL Server, in <c>SqlContentTests</c>.
/// </remarks>
public sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IContentSource>();
            services.AddSingleton<IContentSource>(new InMemoryContentSource(TestContent.Snapshot));
        });
    }
}

/// <summary>A content source that hands back content the test already holds, so no database is involved.</summary>
internal sealed class InMemoryContentSource(IPortfolioContent content) : IContentSource
{
    public Task<IPortfolioContent> LoadAsync(CancellationToken cancellationToken = default) => Task.FromResult(content);
}
