namespace Portfolio.Business.Content;

/// <summary>
/// Where the app gets its content from when it starts (ADR-0012).
/// </summary>
/// <remarks>
/// The real source reads SQL Server. The interface exists so the API tests can start the app without a database,
/// by giving it content they already hold in memory, while the real database path is proven separately by tests
/// that run against a real SQL Server.
/// </remarks>
public interface IContentSource
{
    /// <summary>Loads the whole site's content once.</summary>
    /// <param name="cancellationToken">Cancels the loading.</param>
    Task<IPortfolioContent> LoadAsync(CancellationToken cancellationToken = default);
}
