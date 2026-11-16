using Microsoft.Extensions.DependencyInjection;

namespace Portfolio.Data;

/// <summary>
/// Registers the data layer. The business layer calls this, so the presentation layer never has to know the data layer exists (ADR-0013).
/// </summary>
public static class DataServiceCollectionExtensions
{
    /// <summary>
    /// Adds the database context and the content repository.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">
    /// Finds the connection string when a context is first created, not when the app is built, so an app that never
    /// touches the database (a test with its own content source) never needs one. It comes from configuration and
    /// is never kept in the repository.
    /// </param>
    public static IServiceCollection AddPortfolioData(this IServiceCollection services, Func<IServiceProvider, string> connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddDbContext<PortfolioDbContext>((provider, options) => PortfolioDbContext.Configure(options, connectionString(provider)));
        services.AddScoped<IContentRepository, ContentRepository>();
        return services;
    }
}
