using Portfolio.Business.Content;
using Portfolio.Data;

namespace Portfolio.Business.Services;

/// <summary>
/// The business layer's service: reads the site from the content database and seeds the database from the data files (ADR-0013).
/// </summary>
/// <remarks>
/// It sits between the layers. Above it, the presentation layer asks for content or for maintenance and never sees a
/// row or a table. Below it, the repository stores and returns rows and never sees a content record. This class
/// does the translating (<see cref="ContentMapper"/>) and the judging (an empty database is an error with a clear
/// message, not a blank page).
/// </remarks>
public sealed class ContentService(IContentRepository repository) : IContentSource, IContentMaintenance
{
    /// <summary>
    /// Reads the whole site once. The content never changes while the app runs, so the caller keeps the result.
    /// </summary>
    /// <exception cref="InvalidOperationException">The database has no content yet, so it has not been seeded.</exception>
    public async Task<IPortfolioContent> LoadAsync(CancellationToken cancellationToken = default)
    {
        var rows = await repository.ReadAsync(cancellationToken)
            ?? throw new InvalidOperationException("The content database is empty. Seed it first: dotnet run --project src/Portfolio.Api -- --migrate --seed <absolute content folder>");

        return ContentMapper.ToSnapshot(rows);
    }

    public Task MigrateAsync(CancellationToken cancellationToken = default) => repository.MigrateAsync(cancellationToken);

    public async Task SeedFromFilesAsync(string folder, CancellationToken cancellationToken = default)
    {
        // STEP 1 of 2: read and check the files first, so a mistake in them is found before the database is touched.
        var content = await ContentFiles.LoadAsync(folder, cancellationToken);

        // STEP 2 of 2: translate to rows and store them, replacing what was there.
        await repository.ReplaceAsync(ContentMapper.ToRows(content), cancellationToken);
    }
}
