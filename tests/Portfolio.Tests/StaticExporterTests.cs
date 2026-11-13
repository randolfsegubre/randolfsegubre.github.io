using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Web.Export;

namespace Portfolio.Tests;

/// <summary>
/// Tests for the static export that produces the GitHub Pages site (ADR-0001).
/// </summary>
public sealed class StaticExporterTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly string outputDirectory = Path.Combine(Path.GetTempPath(), "portfolio-export-" + Guid.NewGuid().ToString("N"));

    public void Dispose()
    {
        if (Directory.Exists(outputDirectory))
        {
            Directory.Delete(outputDirectory, recursive: true);
        }
    }

    [Fact]
    public async Task Export_writes_both_pages_the_assets_and_the_nojekyll_marker()
    {
        using var client = factory.CreateClient();
        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;

        await StaticExporter.ExportAsync(client, webRoot, outputDirectory);

        Assert.Contains("Randolf Segubre", await File.ReadAllTextAsync(Path.Combine(outputDirectory, "index.html")));
        Assert.Contains("That page does not exist", await File.ReadAllTextAsync(Path.Combine(outputDirectory, "404.html")));
        Assert.True(File.Exists(Path.Combine(outputDirectory, "css", "site.css")));
        Assert.True(File.Exists(Path.Combine(outputDirectory, "js", "theme.js")));
        Assert.True(File.Exists(Path.Combine(outputDirectory, ".nojekyll")));
    }

    [Fact]
    public async Task Export_removes_stale_files_from_a_previous_run()
    {
        Directory.CreateDirectory(outputDirectory);
        var stale = Path.Combine(outputDirectory, "stale.html");
        await File.WriteAllTextAsync(stale, "old");
        // The marker a previous export leaves behind: without it the exporter refuses to empty the folder.
        await File.WriteAllTextAsync(Path.Combine(outputDirectory, ".nojekyll"), string.Empty);
        using var client = factory.CreateClient();
        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;

        await StaticExporter.ExportAsync(client, webRoot, outputDirectory);

        Assert.False(File.Exists(stale));
    }

    [Fact]
    public async Task Export_fails_loudly_when_a_page_does_not_return_200()
    {
        using var broken = new HttpClient(new StatusHandler(HttpStatusCode.InternalServerError)) { BaseAddress = new Uri("http://localhost") };
        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;

        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => StaticExporter.ExportAsync(broken, webRoot, outputDirectory));

        Assert.Contains("500", error.Message);
    }

    [Fact]
    public async Task Export_refuses_a_folder_that_has_files_but_is_not_a_previous_export()
    {
        // A folder with someone's files and no .nojekyll marker is not the exporter's to empty.
        Directory.CreateDirectory(outputDirectory);
        var precious = Path.Combine(outputDirectory, "precious.txt");
        await File.WriteAllTextAsync(precious, "keep me");
        using var client = factory.CreateClient();
        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;

        var error = await Assert.ThrowsAsync<ArgumentException>(() => StaticExporter.ExportAsync(client, webRoot, outputDirectory));

        Assert.Contains("not a previous export", error.Message);
        Assert.True(File.Exists(precious));
    }

    [Fact]
    public async Task Export_accepts_a_folder_that_a_previous_export_left_behind()
    {
        using var client = factory.CreateClient();
        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;

        await StaticExporter.ExportAsync(client, webRoot, outputDirectory);
        await StaticExporter.ExportAsync(client, webRoot, outputDirectory);

        Assert.True(File.Exists(Path.Combine(outputDirectory, "index.html")));
    }

    [Fact]
    public void Export_refuses_the_web_root_a_folder_above_it_a_dotdot_path_and_a_drive_root()
    {
        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;
        var parentOfWebRoot = Path.GetDirectoryName(Path.TrimEndingDirectorySeparator(webRoot))!;
        var driveRoot = Path.GetPathRoot(webRoot)!;
        var dotDotPath = Path.Combine(webRoot, "..", "..");

        Assert.Throws<ArgumentException>(() => StaticExporter.EnsureSafeToEmpty(webRoot, webRoot));
        Assert.Throws<ArgumentException>(() => StaticExporter.EnsureSafeToEmpty(parentOfWebRoot, webRoot));
        Assert.Throws<ArgumentException>(() => StaticExporter.EnsureSafeToEmpty(driveRoot, webRoot));
        Assert.Throws<ArgumentException>(() => StaticExporter.EnsureSafeToEmpty(dotDotPath, webRoot));
    }

    [Fact]
    public async Task Export_includes_the_sitemap_robots_and_theme_script_files()
    {
        using var client = factory.CreateClient();
        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;

        await StaticExporter.ExportAsync(client, webRoot, outputDirectory);

        Assert.True(File.Exists(Path.Combine(outputDirectory, "sitemap.xml")));
        Assert.True(File.Exists(Path.Combine(outputDirectory, "robots.txt")));
        Assert.True(File.Exists(Path.Combine(outputDirectory, "js", "theme-init.js")));
    }

    /// <summary>A client handler that answers every request with one fixed status code.</summary>
    private sealed class StatusHandler(HttpStatusCode status) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(status));
    }
}
