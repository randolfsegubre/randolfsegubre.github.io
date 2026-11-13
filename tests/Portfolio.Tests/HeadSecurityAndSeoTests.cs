using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Web.Content;

namespace Portfolio.Tests;

/// <summary>
/// Tests for the page head: the Content Security Policy that stands in for response
/// headers on GitHub Pages (ADR-0009), and the search and sharing tags.
/// </summary>
/// <remarks>
/// The policy only protects the page while the page obeys it, so these tests also check
/// the page itself: no inline script, no inline event handler, nothing third-party.
/// </remarks>
public sealed class HeadSecurityAndSeoTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private async Task<IDocument> GetPageAsync(string path)
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync(path);
        response.EnsureSuccessStatusCode();
        return await new HtmlParser().ParseDocumentAsync(await response.Content.ReadAsStringAsync());
    }

    /// <summary>Reads the policy meta tag into a directive-name to value map.</summary>
    private static Dictionary<string, string> Directives(IDocument page)
    {
        var policy = page.QuerySelector("meta[http-equiv='Content-Security-Policy']")?.GetAttribute("content");
        Assert.False(string.IsNullOrWhiteSpace(policy), "The page must carry a Content Security Policy.");
        return policy!.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(directive => directive.Split(' ', 2, StringSplitOptions.TrimEntries))
            .ToDictionary(parts => parts[0], parts => parts.Length > 1 ? parts[1] : string.Empty);
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/not-found")]
    public async Task Every_page_has_a_strict_content_security_policy(string path)
    {
        var directives = Directives(await GetPageAsync(path));

        Assert.Equal("'none'", directives["default-src"]);
        Assert.Equal("'self'", directives["script-src"]);
        Assert.Equal("'self'", directives["style-src"]);
        Assert.Equal("'none'", directives["object-src"]);
        Assert.Equal("'none'", directives["connect-src"]);
        Assert.Equal("'self'", directives["base-uri"]);
        Assert.Equal("'none'", directives["form-action"]);
    }

    [Fact]
    public async Task Policy_never_allows_inline_or_eval_script_or_any_remote_source()
    {
        var directives = Directives(await GetPageAsync("/"));

        Assert.DoesNotContain("'unsafe-inline'", directives["script-src"]);
        Assert.DoesNotContain("'unsafe-eval'", directives["script-src"]);
        Assert.DoesNotContain("'unsafe-inline'", directives["style-src"]);
        Assert.All(directives.Values, value => Assert.DoesNotContain("http", value));
        Assert.All(directives.Values, value => Assert.DoesNotContain("*", value));
    }

    [Fact]
    public async Task Page_obeys_its_own_policy_with_no_inline_script_no_event_handlers_and_no_style_blocks()
    {
        var page = await GetPageAsync("/");

        Assert.All(page.QuerySelectorAll("script"), script => Assert.NotNull(script.GetAttribute("src")));
        Assert.Empty(page.QuerySelectorAll("style"));
        var handlers = page.All
            .SelectMany(element => element.Attributes.Select(attribute => attribute.Name))
            .Where(name => name.StartsWith("on", StringComparison.OrdinalIgnoreCase))
            .ToList();
        Assert.Empty(handlers);
        Assert.DoesNotContain("javascript:", page.DocumentElement.OuterHtml, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Policy_comes_before_any_script_or_stylesheet_so_it_applies_to_all_of_them()
    {
        var page = await GetPageAsync("/");

        var elements = page.Head!.Children.ToList();
        var policyIndex = elements.FindIndex(e => e.GetAttribute("http-equiv") == "Content-Security-Policy");
        var firstResourceIndex = elements.FindIndex(e => e.TagName == "SCRIPT" || (e.TagName == "LINK" && e.GetAttribute("rel") == "stylesheet"));

        Assert.True(policyIndex >= 0);
        Assert.True(policyIndex < firstResourceIndex, "A policy only governs what loads after it.");
    }

    [Fact]
    public async Task Home_page_has_a_canonical_link_to_the_site_and_is_indexable()
    {
        var page = await GetPageAsync("/");

        Assert.Equal(SiteInfo.Url + "/", page.QuerySelector("link[rel='canonical']")?.GetAttribute("href"));
        Assert.Null(page.QuerySelector("meta[name='robots']"));
        Assert.Equal(SiteInfo.Url + "/", page.QuerySelector("meta[property='og:url']")?.GetAttribute("content"));
    }

    [Fact]
    public async Task Not_found_page_is_marked_noindex_and_has_no_canonical_link()
    {
        var page = await GetPageAsync("/not-found");

        Assert.Equal("noindex", page.QuerySelector("meta[name='robots']")?.GetAttribute("content"));
        Assert.Null(page.QuerySelector("link[rel='canonical']"));
    }

    [Fact]
    public async Task Sharing_tags_are_complete_and_the_preview_image_is_an_absolute_address_with_its_size()
    {
        var page = await GetPageAsync("/");

        Assert.False(string.IsNullOrWhiteSpace(page.QuerySelector("meta[name='twitter:title']")?.GetAttribute("content")));
        Assert.False(string.IsNullOrWhiteSpace(page.QuerySelector("meta[name='twitter:description']")?.GetAttribute("content")));
        Assert.StartsWith(SiteInfo.Url + "/", page.QuerySelector("meta[property='og:image']")?.GetAttribute("content"));
        Assert.Equal("640", page.QuerySelector("meta[property='og:image:width']")?.GetAttribute("content"));
        Assert.Equal("640", page.QuerySelector("meta[property='og:image:height']")?.GetAttribute("content"));
    }

    [Fact]
    public async Task Sitemap_and_robots_point_at_the_one_site_address_and_the_theme_script_exists()
    {
        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;

        var sitemap = await File.ReadAllTextAsync(Path.Combine(webRoot, "sitemap.xml"));
        var robots = await File.ReadAllTextAsync(Path.Combine(webRoot, "robots.txt"));

        Assert.Contains($"<loc>{SiteInfo.Url}/</loc>", sitemap);
        Assert.Contains($"Sitemap: {SiteInfo.Url}/sitemap.xml", robots);
        Assert.True(File.Exists(Path.Combine(webRoot, "js", "theme-init.js")));
    }
}
