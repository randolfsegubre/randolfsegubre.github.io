using Portfolio.Business.Content;

namespace Portfolio.Tests;

/// <summary>
/// The site's content as the tests see it: read from the JSON data files, the source of truth (ADR-0012).
/// </summary>
/// <remarks>
/// The writing rules and the page checks read the same words the database is filled from, so a mistake is caught in the
/// file that holds it. No database is needed for these. The tests that do need SQL Server are in
/// <c>SqlContentTests</c>, and they prove the database returns exactly what these files say.
/// <para>
/// Set <c>PORTFOLIO_CONTENT_DIR</c> to read another folder. Otherwise the folder is found from the repository root.
/// </para>
/// </remarks>
internal static class TestContent
{
    /// <summary>The content folder.</summary>
    public static string Folder { get; } = FindFolder();

    /// <summary>The content, loaded once and shared, because it is immutable.</summary>
    public static ContentSnapshot Snapshot { get; } = Load(Folder);

    /// <summary>
    /// Reads the files on a worker thread and waits for them. The folder is passed in as an argument on purpose: the
    /// worker must not touch this class's own static members while the class is still being set up, because it would
    /// wait for the very initialization that is waiting for it, and the test run would hang forever.
    /// </summary>
    private static ContentSnapshot Load(string folder) => Task.Run(() => ContentFiles.LoadAsync(folder)).GetAwaiter().GetResult();

    private static string FindFolder()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("PORTFOLIO_CONTENT_DIR");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return Directory.Exists(fromEnvironment)
                ? fromEnvironment
                : throw new InvalidOperationException($"PORTFOLIO_CONTENT_DIR is '{fromEnvironment}', which does not exist.");
        }

        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Portfolio.slnx")))
            {
                return Path.Combine(directory.FullName, "content");
            }
        }

        throw new InvalidOperationException("Could not find the repository root (Portfolio.slnx) above the test folder.");
    }
}
