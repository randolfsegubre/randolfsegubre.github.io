namespace Portfolio.Web.Export;

/// <summary>
/// One page to export: the address to request and the file to write.
/// </summary>
public sealed record ExportPage(string Route, string OutputFile);

/// <summary>
/// Renders the MVC site to plain static files that GitHub Pages can serve (ADR-0001, ADR-0002).
/// </summary>
/// <remarks>
/// GitHub Pages cannot run a server, so the deployed site is the app's own output:
/// each route is requested from the running app and its HTML is saved, then the
/// <c>wwwroot</c> files are copied alongside. The caller supplies the
/// <see cref="HttpClient"/>, so production uses a real Kestrel server and tests use
/// the in-memory test server with the same code.
/// </remarks>
public static class StaticExporter
{
    /// <summary>Routes exported, and the file each becomes. The 404 file is what Pages serves for unknown addresses.</summary>
    public static IReadOnlyList<ExportPage> Pages { get; } =
    [
        new ExportPage("/", "index.html"),
        new ExportPage("/not-found", "404.html"),
    ];

    /// <summary>
    /// Writes the whole site into <paramref name="outputDirectory"/>.
    /// </summary>
    /// <param name="client">A client already pointed at the running app.</param>
    /// <param name="webRootPath">The app's <c>wwwroot</c> folder (styles, script, favicon).</param>
    /// <param name="outputDirectory">Where to write the site. It is emptied first.</param>
    /// <exception cref="InvalidOperationException">Any page did not return 200, so a broken site is never published.</exception>
    public static async Task ExportAsync(HttpClient client, string webRootPath, string outputDirectory, CancellationToken cancellationToken = default)
    {
        // STEP 1 of 4: start from an empty output folder so no stale file can be published.
        if (Directory.Exists(outputDirectory))
        {
            Directory.Delete(outputDirectory, recursive: true);
        }

        Directory.CreateDirectory(outputDirectory);

        // STEP 2 of 4: copy the static assets (stylesheet, script, favicon, robots.txt).
        CopyDirectory(webRootPath, outputDirectory);

        // STEP 3 of 4: request each page from the running app and save the HTML it renders.
        foreach (var page in Pages)
        {
            using var response = await client.GetAsync(page.Route, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"Export failed: {page.Route} returned {(int)response.StatusCode}.");
            }

            var html = await response.Content.ReadAsStringAsync(cancellationToken);
            await File.WriteAllTextAsync(Path.Combine(outputDirectory, page.OutputFile), html, cancellationToken);
        }

        // STEP 4 of 4: tell GitHub Pages not to run Jekyll over the output.
        await File.WriteAllTextAsync(Path.Combine(outputDirectory, ".nojekyll"), string.Empty, cancellationToken);
    }

    private static void CopyDirectory(string source, string destination)
    {
        foreach (var directory in Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(Path.Combine(destination, Path.GetRelativePath(source, directory)));
        }

        foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            File.Copy(file, Path.Combine(destination, Path.GetRelativePath(source, file)), overwrite: true);
        }
    }
}
