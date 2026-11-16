using Microsoft.Extensions.DependencyInjection;
using Portfolio.Business.Content;
using Portfolio.Business.Services;
using Portfolio.Data;

namespace Portfolio.Business;

/// <summary>
/// Registers the business layer, and the data layer beneath it, in one call (ADR-0013).
/// </summary>
/// <remarks>
/// The presentation layer calls only this. That is how it stays ignorant of the data layer: it never names a
/// repository, a context or a connection, only what the business layer offers.
/// </remarks>
public static class BusinessServiceCollectionExtensions
{
    /// <summary>
    /// Adds the content service, the content source and the maintenance jobs, backed by SQL Server.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">
    /// Finds the connection string when the database is first used, so an app that never uses it needs none. It comes
    /// from configuration and is never kept in the repository.
    /// </param>
    public static IServiceCollection AddPortfolioContent(this IServiceCollection services, Func<IServiceProvider, string> connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddPortfolioData(connectionString);
        services.AddScoped<ContentService>();
        services.AddScoped<IContentSource>(provider => provider.GetRequiredService<ContentService>());
        services.AddScoped<IContentMaintenance>(provider => provider.GetRequiredService<ContentService>());
        return services;
    }
}
