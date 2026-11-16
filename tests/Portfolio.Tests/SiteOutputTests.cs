using AngleSharp.Html.Parser;

namespace Portfolio.Tests;

/// <summary>
/// Tests for what the Nuxt build puts in the published folder, beyond the page markup (ADR-0011).
/// </summary>
/// <remarks>
/// Nuxt is a large tool that adds its own tags, scripts and files by default. The site's Content Security
/// Policy allows only its own two small scripts and the theme script, so anything else it added would either
/// be blocked in the browser or weaken the policy if the policy were relaxed to let it run. These tests fail
/// when the build output grows something nobody chose, so the answer is never to loosen the policy.
/// </remarks>
public sealed class SiteOutputTests(SiteFactory factory) : IClassFixture<SiteFactory>
{
    private async Task<AngleSharp.Html.Dom.IHtmlDocument> PageAsync(string route)
    {
        using var client = factory.CreateClient();
        return await new HtmlParser().ParseDocumentAsync(await client.GetStringAsync(route));
    }

    [Theory]
    [InlineData("/")]
    [InlineData("/not-found")]
    public async Task Ships_only_the_three_same_origin_scripts_and_no_inline_script_or_handler(string route)
    {
        var page = await PageAsync(route);

        var scripts = page.QuerySelectorAll("script").ToList();
        Assert.All(scripts, script =>
        {
            Assert.True(script.HasAttribute("src"), "An inline script would be blocked by the Content Security Policy: " + script.OuterHtml[..Math.Min(80, script.OuterHtml.Length)]);
            Assert.StartsWith("/js/", script.GetAttribute("src"));
        });
        Assert.Equal(["theme-init.js", "theme.js", "site.js"], scripts.Select(s => Path.GetFileName(s.GetAttribute("src")!.Split('?')[0])));

        // No inline event handler anywhere (onclick, onload and the rest), and no javascript: address.
        Assert.All(page.All, element =>
        {
            Assert.DoesNotContain(element.Attributes, a => a.Name.StartsWith("on", StringComparison.OrdinalIgnoreCase));
            Assert.DoesNotContain(element.Attributes, a => a.Value.TrimStart().StartsWith("javascript:", StringComparison.OrdinalIgnoreCase));
        });
    }

    [Fact]
    public async Task Does_not_ship_Nuxts_own_hints_or_data_files()
    {
        var page = await PageAsync("/");

        // Nuxt's speculation rules are an inline script (blocked by the policy) and nothing here needs them.
        Assert.Empty(page.QuerySelectorAll("script[type='speculationrules']"));
        Assert.Empty(page.QuerySelectorAll("link[rel='prefetch'], link[rel='modulepreload'], link[rel='preload'][as='script']"));

        // With no Nuxt JavaScript in the browser there is nothing to read a payload, and a source map would hand out the source.
        var files = Directory.EnumerateFiles(factory.WebRootPath, "*", SearchOption.AllDirectories).Select(f => Path.GetFileName(f)).ToList();
        Assert.DoesNotContain(files, f => f.EndsWith(".map", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain("_payload.json", files);
    }

    [Fact]
    public void Publishes_the_two_pages_GitHub_Pages_uses_and_no_leftover_Nuxt_files()
    {
        var root = factory.WebRootPath;
        Assert.True(File.Exists(Path.Combine(root, "index.html")));
        Assert.True(File.Exists(Path.Combine(root, "404.html")));

        // Nuxt also prerenders an empty single-page-app shell to 200.html (GitHub Pages never uses it) and to 404.html.
        // That second write once raced with the real not-found page and could win, publishing a blank 404 page.
        Assert.False(File.Exists(Path.Combine(root, "200.html")), "200.html is Nuxt's empty fallback shell; nothing serves it.");
        Assert.Contains("That page does not exist", File.ReadAllText(Path.Combine(root, "404.html")));

        // Nuxt's own error-screen styles and build manifest are never loaded by the page (scripts/prune-output.mjs, nuxt.config.js).
        Assert.False(Directory.Exists(Path.Combine(root, "_nuxt")), "The _nuxt folder holds files no page loads.");
    }

    [Fact]
    public void Keeps_the_marker_that_stops_GitHub_Pages_running_Jekyll_over_the_output()
    {
        Assert.True(File.Exists(Path.Combine(factory.WebRootPath, ".nojekyll")));
    }

    [Fact]
    public async Task The_page_is_plain_HTML_with_every_section_present_even_though_no_framework_runs_in_the_browser()
    {
        var page = await PageAsync("/");

        foreach (var id in new[] { "top", "about", "live-sites", "projects", "experience", "contact", "main" })
        {
            Assert.NotNull(page.GetElementById(id));
        }

        Assert.Single(page.QuerySelectorAll("h1"));
    }
}
