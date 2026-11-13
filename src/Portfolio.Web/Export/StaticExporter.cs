namespace Portfolio.Web.Export;

/// <summary>
/// One page to export: the address to request and the file to write.
/// </summary>
public sealed record ExportPage(string Route, string OutputFile);

/// <summary>
/// Renders the MVC site to plain static files that GitHub Pages can serve (ADR-0001, ADR-0007).
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
    /// <exception cref="ArgumentException">The output folder is not safe to empty (see <see cref="EnsureSafeToEmpty"/>).</exception>
    public static async Task ExportAsync(HttpClient client, string webRootPath, string outputDirectory, CancellationToken cancellationToken = default)
    {
        // STEP 1 of 4: refuse an unsafe folder, then start from an empty one so no stale file can be published.
        EnsureSafeToEmpty(outputDirectory, webRootPath);
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
        await File.WriteAllTextAsync(Path.Combine(outputDirectory, ExportMarkerFile), string.Empty, cancellationToken);
    }

    /// <summary>The file every export writes last; its presence proves a folder is a previous export of this site.</summary>
    private const string ExportMarkerFile = ".nojekyll";

    /// <summary>
    /// Stops an export from deleting a folder it did not create.
    /// </summary>
    /// <remarks>
    /// The exporter empties its output folder before writing, which is a recursive delete
    /// of whatever path it is given. A wrong argument (a drive root, the repository, the
    /// <c>wwwroot</c> folder it is about to copy from) would destroy real work, so it is
    /// refused unless the folder is missing, empty, or carries the marker file that a
    /// previous export left behind.
    /// </remarks>
    /// <exception cref="ArgumentException">The folder is a filesystem root, contains the web root, or holds files that are not a previous export.</exception>
    public static void EnsureSafeToEmpty(string outputDirectory, string webRootPath)
    {
        // STEP 1 of 3: compare full, normalised paths so "..", trailing slashes and case cannot hide a match.
        var output = Path.TrimEndingDirectorySeparator(Path.GetFullPath(outputDirectory));
        var webRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(webRootPath));

        // STEP 2 of 3: never a drive root, and never the web root or any folder above it (that would include the source code).
        if (Path.GetPathRoot(output) is { } root && string.Equals(Path.TrimEndingDirectorySeparator(root), output, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"Refusing to export into the filesystem root '{output}'.", nameof(outputDirectory));
        }

        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (webRoot.StartsWith(output + Path.DirectorySeparatorChar, comparison) || string.Equals(webRoot, output, comparison))
        {
            throw new ArgumentException($"Refusing to export into '{output}': it is, or contains, the site's own source folder '{webRoot}'.", nameof(outputDirectory));
        }

        // STEP 3 of 3: a folder that already has files in it must be an earlier export (marker present), or it is not ours to empty.
        if (Directory.Exists(output) && Directory.EnumerateFileSystemEntries(output).Any() && !File.Exists(Path.Combine(output, ExportMarkerFile)))
        {
            throw new ArgumentException($"Refusing to empty '{output}': it has files in it but is not a previous export of this site (no {ExportMarkerFile} marker). Choose an empty or new folder.", nameof(outputDirectory));
        }
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
