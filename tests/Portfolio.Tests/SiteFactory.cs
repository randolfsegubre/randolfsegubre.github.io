using System.Net;

namespace Portfolio.Tests;

/// <summary>
/// Serves the generated static site (the Nuxt output) to the tests, as if from a web server (ADR-0011).
/// </summary>
/// <remarks>
/// The site's tests check what a visitor really receives: the HTML, the stylesheet, the script and the
/// files. After the move to Nuxt that is the generated folder <c>web/.output/public</c>, so these tests run
/// against the real build output and not against a server-side approximation of it. A test that passed
/// against anything else could pass while the published site was broken.
/// <para>
/// Set <c>PORTFOLIO_SITE_DIR</c> to test another folder (the deploy workflow does). Otherwise the folder is found
/// from the repository root. If nothing has been built, the tests stop with the command to run, instead of
/// passing on nothing.
/// </para>
/// </remarks>
public sealed class SiteFactory
{
    /// <summary>The folder that holds the generated site.</summary>
    public string WebRootPath { get; } = FindSiteDirectory();

    /// <summary>A client that answers requests from the generated files, with the same addresses a browser would use.</summary>
    public HttpClient CreateClient() => new(new SiteFileHandler(WebRootPath)) { BaseAddress = new Uri("http://localhost") };

    private static string FindSiteDirectory()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable("PORTFOLIO_SITE_DIR");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return Directory.Exists(fromEnvironment)
                ? fromEnvironment
                : throw new InvalidOperationException($"PORTFOLIO_SITE_DIR is '{fromEnvironment}', which does not exist.");
        }

        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Portfolio.slnx")))
            {
                var site = Path.Combine(directory.FullName, "web", ".output", "public");
                return File.Exists(Path.Combine(site, "index.html"))
                    ? site
                    : throw new InvalidOperationException("The site has not been generated. In the web folder run: npm ci && npm run generate");
            }
        }

        throw new InvalidOperationException("Could not find the repository root (Portfolio.slnx) above the test folder.");
    }

    /// <summary>
    /// Answers GET requests from a folder of files: "/" is index.html, "/not-found" is 404.html (the page
    /// GitHub Pages serves for unknown addresses), anything else is the file at that path, and a missing file is a 404.
    /// </summary>
    private sealed class SiteFileHandler(string root) : HttpMessageHandler
    {
        private static readonly Dictionary<string, string> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".html"] = "text/html",
            [".css"] = "text/css",
            [".js"] = "text/javascript",
            [".svg"] = "image/svg+xml",
            [".jpg"] = "image/jpeg",
            [".webp"] = "image/webp",
            [".pdf"] = "application/pdf",
            [".mp4"] = "video/mp4",
            [".webm"] = "video/webm",
            [".txt"] = "text/plain",
            [".xml"] = "application/xml",
        };

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var path = Uri.UnescapeDataString(request.RequestUri!.AbsolutePath);
            var relative = path switch
            {
                "/" => "index.html",
                "/not-found" => "404.html",
                _ => path.TrimStart('/'),
            };

            // A path with ".." could read outside the site folder; refuse it the way a real server does.
            var full = Path.GetFullPath(Path.Combine(root, relative));
            if (!full.StartsWith(Path.GetFullPath(root), StringComparison.OrdinalIgnoreCase) || !File.Exists(full))
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound) { Content = new StringContent(string.Empty) };
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(await File.ReadAllBytesAsync(full, cancellationToken)) };
            response.Content.Headers.ContentType = new(ContentTypes.GetValueOrDefault(Path.GetExtension(full), "application/octet-stream"));
            return response;
        }
    }
}
