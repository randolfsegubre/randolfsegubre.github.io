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

    /// <summary>A client handler that answers every request with one fixed status code.</summary>
    private sealed class StatusHandler(HttpStatusCode status) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(status));
    }
}
