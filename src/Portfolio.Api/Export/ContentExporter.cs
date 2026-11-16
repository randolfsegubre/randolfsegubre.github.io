using Portfolio.Api.Content;

namespace Portfolio.Api.Export;

/// <summary>
/// Writes the site's content to a JSON file for the Nuxt build to read (ADR-0011).
/// </summary>
/// <remarks>
/// GitHub Pages cannot run a server, so the deployed site is plain files. The Nuxt build needs the
/// content at build time, and this is how it gets it: the same document <c>GET /api/content</c> serves,
/// written to disk without starting a web server. Using the same builder and serializer settings for both
/// means the static site and the live API can never disagree.
/// </remarks>
public static class ContentExporter
{
    /// <summary>
    /// Builds the content document and saves it.
    /// </summary>
    /// <param name="content">The content service.</param>
    /// <param name="outputFile">The absolute path of the JSON file to write. Its folder is created if missing.</param>
    /// <param name="year">The year to show in the footer.</param>
    /// <exception cref="ArgumentException">The path is not absolute, so the file could land in an unexpected folder.</exception>
    public static async Task WriteAsync(IPortfolioContent content, string outputFile, int year, CancellationToken cancellationToken = default)
    {
        // STEP 1 of 3: refuse a relative path. `dotnet run` starts in the project folder, so a relative path would land in the wrong place.
        if (!Path.IsPathRooted(outputFile))
        {
            throw new ArgumentException($"--export-content needs an absolute file path, but got '{outputFile}'.", nameof(outputFile));
        }

        // STEP 2 of 3: build the document.
        var json = SiteContentBuilder.ToJson(SiteContentBuilder.Build(content, year));

        // STEP 3 of 3: save it, creating the folder when needed.
        Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);
        await File.WriteAllTextAsync(outputFile, json, cancellationToken);
    }
}
