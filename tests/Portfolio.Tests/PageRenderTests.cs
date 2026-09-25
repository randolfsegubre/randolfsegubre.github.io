using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Portfolio.Web.Content;

namespace Portfolio.Tests;

/// <summary>
/// Render tests: the page shows what the data says, and its accessibility and safety
/// details hold. The app is hosted in memory (real MVC pipeline and real Razor views)
/// and the returned HTML is parsed the way a browser would.
/// </summary>
public sealed class PageRenderTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly PortfolioContent Content = new();

    private async Task<IDocument> GetPageAsync(string path)
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync(path);
        response.EnsureSuccessStatusCode();
        var html = await response.Content.ReadAsStringAsync();
        return await new HtmlParser().ParseDocumentAsync(html);
    }

    [Fact]
    public async Task Home_page_has_one_h1_with_the_owners_name_and_the_main_landmarks()
    {
        var page = await GetPageAsync("/");

        var h1 = Assert.Single(page.QuerySelectorAll("h1"));
        Assert.Equal(Content.Profile.Name, h1.TextContent.Trim());
        Assert.NotNull(page.QuerySelector("header.site-header"));
        Assert.NotNull(page.QuerySelector("main#main"));
        Assert.NotNull(page.QuerySelector("footer"));
        Assert.Equal("#main", page.QuerySelector("a.skip-link")?.GetAttribute("href"));
    }

    [Fact]
    public async Task Home_page_renders_a_card_for_every_project_and_a_role_for_every_job()
    {
        var page = await GetPageAsync("/");

        foreach (var project in Content.Projects)
        {
            var heading = page.QuerySelector($"article#{project.Id} h3");
            Assert.Equal(project.Name, heading?.TextContent.Trim());
        }

        foreach (var role in Content.Experience)
        {
            Assert.Equal(role.Title, page.QuerySelector($"#{role.Id}-title")?.TextContent.Trim());
        }
    }

    [Fact]
    public async Task Known_gap_appears_only_where_the_data_names_one()
    {
        var page = await GetPageAsync("/");

        var expected = Content.Projects.Count(p => p.HonestNote is not null);
        Assert.Equal(expected, page.QuerySelectorAll("#projects p.honest-note").Length);
    }

    [Fact]
    public async Task Every_outbound_link_opens_safely_in_a_new_tab()
    {
        var page = await GetPageAsync("/");

        var outbound = page.QuerySelectorAll("a[href^='https://']").ToList();
        Assert.NotEmpty(outbound);
        Assert.All(outbound, link =>
        {
            Assert.Equal("_blank", link.GetAttribute("target"));
            Assert.Contains("noopener", link.GetAttribute("rel"));
        });
    }

    [Fact]
    public async Task Page_loads_no_third_party_resources()
    {
        var page = await GetPageAsync("/");

        var references = page.QuerySelectorAll("link[href], script[src]")
            .Select(e => e.GetAttribute("href") ?? e.GetAttribute("src"))
            .Where(r => r is not null)
            .ToList();

        Assert.NotEmpty(references);
        Assert.All(references, reference => Assert.StartsWith("/", reference));
    }

    [Fact]
    public async Task Resume_button_is_hidden_until_a_resume_file_is_configured()
    {
        var page = await GetPageAsync("/");

        Assert.Null(Content.Profile.ResumeUrl);
        Assert.DoesNotContain(page.QuerySelectorAll("a"), a => a.TextContent.Contains("Download resume"));
    }

    [Fact]
    public async Task Private_project_card_has_no_links()
    {
        var page = await GetPageAsync("/");

        Assert.Empty(page.QuerySelectorAll("article#galaxy-survivor a"));
    }

    [Fact]
    public async Task Theme_toggle_ships_hidden_so_it_never_shows_as_a_dead_button()
    {
        var page = await GetPageAsync("/");

        var toggle = page.QuerySelector("#theme-toggle");
        Assert.NotNull(toggle);
        Assert.True(toggle.HasAttribute("hidden"));
    }

    [Fact]
    public async Task Dark_is_the_default_so_the_toggle_offers_light()
    {
        var page = await GetPageAsync("/");

        // No theme attribute is written by the server, and the toggle describes the action it will take.
        Assert.False(page.DocumentElement.HasAttribute("data-theme"));
        Assert.Equal("Switch to light theme", page.QuerySelector("#theme-toggle")?.GetAttribute("aria-label"));
    }

    [Fact]
    public async Task Live_sites_section_lists_every_site_with_a_safe_public_link()
    {
        var page = await GetPageAsync("/");

        var section = page.QuerySelector("section#live-sites");
        Assert.NotNull(section);
        Assert.Equal("live-sites-heading", section.GetAttribute("aria-labelledby"));

        foreach (var site in Content.LiveSites)
        {
            var card = section.QuerySelector($"article#{site.Id}");
            Assert.NotNull(card);
            Assert.Equal(site.Name, card.QuerySelector("h3")?.TextContent.Trim());

            var link = card.QuerySelector($"a[href='{site.Href}']");
            Assert.NotNull(link);
            Assert.Equal("_blank", link.GetAttribute("target"));
            Assert.Contains("noopener", link.GetAttribute("rel"));
            Assert.NotNull(card.QuerySelector("p.honest-note"));
        }

        // The navigation reaches it.
        Assert.NotNull(page.QuerySelector(".nav-list a[href='/#live-sites']"));
    }

    [Fact]
    public async Task Contact_section_shows_every_phone_number_with_working_links()
    {
        var page = await GetPageAsync("/");

        var contact = page.QuerySelector("section#contact");
        Assert.NotNull(contact);

        foreach (var phone in Content.Profile.Phones)
        {
            var card = contact.QuerySelectorAll("li.contact-card").Single(li => li.QuerySelector(".contact-label")?.TextContent.Trim() == phone.Label);
            var main = card.QuerySelector($"a[href='{phone.Href}']");
            Assert.NotNull(main);
            Assert.Contains(phone.Display, main.TextContent);

            if (phone.ActionHref is not null)
            {
                Assert.Equal(phone.ActionLabel, card.QuerySelector($"a[href='{phone.ActionHref}']")?.TextContent.Trim());
            }
        }

        // The WhatsApp link leaves the site, so it must open safely in a new tab.
        var whatsapp = contact.QuerySelector("a[href^='https://wa.me/']");
        Assert.NotNull(whatsapp);
        Assert.Equal("_blank", whatsapp.GetAttribute("target"));
        Assert.Contains("noopener", whatsapp.GetAttribute("rel"));
    }

    [Fact]
    public async Task Whole_page_never_exposes_an_internal_hosting_address()
    {
        var page = await GetPageAsync("/");

        Assert.DoesNotContain("azurewebsites", page.DocumentElement.OuterHtml, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Stats_strip_shows_every_stat_from_the_data()
    {
        var page = await GetPageAsync("/");

        var rendered = page.QuerySelectorAll("li.stat").Select(li => li.QuerySelector(".stat-value")?.TextContent.Trim()).ToList();
        Assert.Equal(Content.Profile.Stats.Select(s => s.Value), rendered);
    }

    [Fact]
    public async Task Hero_code_card_is_rendered_from_the_profile_and_hidden_from_assistive_technology()
    {
        var page = await GetPageAsync("/");

        var card = page.QuerySelector(".code-card");
        Assert.NotNull(card);
        Assert.Equal("true", card.GetAttribute("aria-hidden"));
        Assert.Contains(Content.Profile.Name, card.TextContent);
        Assert.Contains(Content.Profile.Title, card.TextContent);
        Assert.All(Content.Profile.SignatureStack, item => Assert.Contains(item, card.TextContent));
    }

    [Fact]
    public async Task Code_backdrop_is_purely_decorative()
    {
        var page = await GetPageAsync("/");

        var backdrop = page.QuerySelector(".code-backdrop");
        Assert.NotNull(backdrop);
        Assert.Equal("true", backdrop.GetAttribute("aria-hidden"));

        // Every lane of scrolling code is present, each holding its code twice so the loop has no visible jump.
        var streams = backdrop.QuerySelectorAll(".code-streams--hero .code-stream").ToList();
        Assert.Equal(BackdropCode.Streams.Count, streams.Count);
        Assert.All(streams, stream =>
        {
            var text = stream.QuerySelector("pre")!.TextContent;
            var half = text.Length / 2;
            Assert.Equal(text[..half].Trim(), text[half..].Trim());
        });

        // Floating logos come from the profile data, one inline vector image each.
        var logos = backdrop.QuerySelectorAll(".tech-logos--hero .tech-logo").ToList();
        Assert.Equal(Content.Profile.BackdropTags, logos.Select(l => l.GetAttribute("data-tech")));
        Assert.All(logos, logo => Assert.NotNull(logo.QuerySelector("svg path")));
    }

    [Fact]
    public async Task Logos_are_inline_vector_images_so_the_page_makes_no_image_requests_for_them()
    {
        var page = await GetPageAsync("/");

        // Only the portrait is an external image file; every technology logo is inline markup.
        var imageFiles = page.QuerySelectorAll("img").Select(i => i.GetAttribute("src")).ToList();
        Assert.Equal([Content.Profile.PhotoUrl], imageFiles);
    }

    [Fact]
    public async Task Background_lanes_are_grid_columns_so_they_cannot_overlap()
    {
        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;
        var css = await File.ReadAllTextAsync(Path.Combine(webRoot, "css", "site.css"));

        // Lanes have no absolute positions of their own: the container lays them out as equal grid columns.
        Assert.Contains("grid-template-columns: repeat(var(--lanes), minmax(0, 1fr))", css);
        Assert.DoesNotContain("left: var(--x);\n  width: 13rem", css);
        var page = await GetPageAsync("/");
        Assert.All(page.QuerySelectorAll(".code-stream"), lane => Assert.DoesNotContain("--x", lane.GetAttribute("style") ?? string.Empty));
    }

    [Fact]
    public async Task Faint_page_wide_code_layer_is_decorative_and_sits_outside_the_content()
    {
        var page = await GetPageAsync("/");

        // The page-wide layer covers every section: the same lanes as the hero, plus logos, fixed behind the content.
        var layer = page.QuerySelector(".code-streams--page");
        Assert.NotNull(layer);
        Assert.Equal("true", layer.GetAttribute("aria-hidden"));
        Assert.Null(layer.Closest("main"));
        Assert.Equal(BackdropCode.Streams.Count, layer.QuerySelectorAll(".code-stream").Length);

        var logos = page.QuerySelector(".tech-logos--page");
        Assert.NotNull(logos);
        Assert.Equal("true", logos.GetAttribute("aria-hidden"));
        Assert.Null(logos.Closest("main"));
        Assert.NotEmpty(logos.QuerySelectorAll(".tech-logo"));
    }

    [Fact]
    public async Task Portrait_is_in_the_hero_with_alt_text_a_size_and_no_lazy_loading()
    {
        var page = await GetPageAsync("/");

        var avatar = page.QuerySelector(".hero img.avatar");
        Assert.NotNull(avatar);
        Assert.Equal(Content.Profile.PhotoUrl, avatar.GetAttribute("src"));
        Assert.False(string.IsNullOrWhiteSpace(avatar.GetAttribute("alt")));
        Assert.NotNull(avatar.GetAttribute("width"));
        Assert.NotNull(avatar.GetAttribute("height"));
        Assert.NotEqual("lazy", avatar.GetAttribute("loading"));
    }

    [Fact]
    public async Task Portrait_file_exists_and_link_previews_use_it_with_an_absolute_address()
    {
        var page = await GetPageAsync("/");

        var webRoot = factory.Services.GetRequiredService<IWebHostEnvironment>().WebRootPath;
        Assert.True(File.Exists(Path.Combine(webRoot, Content.Profile.PhotoUrl!.TrimStart('/'))));

        var preview = page.QuerySelector("meta[property='og:image']")?.GetAttribute("content");
        Assert.NotNull(preview);
        Assert.StartsWith("https://", preview);
        Assert.EndsWith(Content.Profile.PhotoUrl, preview);
    }

    [Fact]
    public async Task Page_wires_reveals_progress_bar_and_script_as_enhancements()
    {
        var page = await GetPageAsync("/");

        Assert.True(page.QuerySelectorAll("[data-reveal]").Length > 10);
        Assert.Equal("true", page.QuerySelector(".scroll-progress")?.GetAttribute("aria-hidden"));
        Assert.NotNull(page.QuerySelector("script[src^='/js/site.js']"));

        // The hero must never wait for a reveal: it is the first thing every visitor sees.
        Assert.Empty(page.QuerySelectorAll(".hero [data-reveal]"));
    }

    [Fact]
    public async Task Not_found_page_renders_with_its_own_heading()
    {
        var page = await GetPageAsync("/not-found");

        Assert.Equal("That page does not exist", page.QuerySelector("h1")?.TextContent.Trim());
    }
}
