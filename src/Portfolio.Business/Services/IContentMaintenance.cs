namespace Portfolio.Business.Services;

/// <summary>
/// The jobs that look after the content database: creating it and filling it from the data files (ADR-0012, ADR-0013).
/// </summary>
/// <remarks>
/// The presentation layer calls these from its build commands and never touches the database itself. They are kept
/// apart from <see cref="Portfolio.Business.Content.IContentSource"/> because reading the site and changing the database are different
/// responsibilities, and almost nothing that reads should be able to change.
/// </remarks>
public interface IContentMaintenance
{
    /// <summary>Creates the database if it is missing and applies the schema's migrations.</summary>
    /// <param name="cancellationToken">Cancels the work.</param>
    Task MigrateAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Makes the database say exactly what the JSON files in the folder say, all or nothing.
    /// </summary>
    /// <param name="folder">The folder that holds the content files.</param>
    /// <param name="cancellationToken">Cancels the work.</param>
    Task SeedFromFilesAsync(string folder, CancellationToken cancellationToken = default);
}
