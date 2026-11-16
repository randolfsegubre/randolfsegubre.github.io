using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Portfolio.Api.Content;
using Portfolio.Api.Export;

namespace Portfolio.Tests;

/// <summary>
/// Tests for the .NET backend that feeds the Nuxt front end (ADR-0011): the content document, the file export
/// the static build uses, and the security of the API.
/// </summary>
public sealed class ContentApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly PortfolioContent Content = new();

    [Fact]
    public async Task The_API_returns_the_content_document_as_JSON()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/content");

        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(Content.Profile.Name, document.RootElement.GetProperty("profile").GetProperty("name").GetString());
        Assert.Equal(Content.Universe.Stars.Count, document.RootElement.GetProperty("universe").GetProperty("stars").GetArrayLength());
    }

    [Fact]
    public async Task The_API_sends_security_headers_and_hides_the_server_software()
    {
        using var client = factory.CreateClient();

        using var response = await client.GetAsync("/api/content");

        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("no-referrer", response.Headers.GetValues("Referrer-Policy").Single());
        Assert.Equal("default-src 'none'; frame-ancestors 'none'", response.Headers.GetValues("Content-Security-Policy").Single());
        Assert.Equal("same-origin", response.Headers.GetValues("Cross-Origin-Resource-Policy").Single());
        Assert.False(response.Headers.Contains("Server"), "The Server header names the software, which helps only an attacker.");
    }

    [Fact]
    public async Task The_API_allows_no_other_site_to_read_it_and_offers_no_other_method()
    {
        using var client = factory.CreateClient();

        using var crossOrigin = new HttpRequestMessage(HttpMethod.Get, "/api/content");
        crossOrigin.Headers.Add("Origin", "https://evil.example");
        using var response = await client.SendAsync(crossOrigin);

        // No CORS policy is registered, so the browser is never told another site may read the response.
        Assert.False(response.Headers.Contains("Access-Control-Allow-Origin"));
        using var post = await client.PostAsync("/api/content", new StringContent("{}"));
        Assert.Equal(System.Net.HttpStatusCode.MethodNotAllowed, post.StatusCode);
        using var unknown = await client.GetAsync("/api/anything-else");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, unknown.StatusCode);
    }

    [Fact]
    public async Task The_export_writes_the_same_document_the_API_serves()
    {
        var file = Path.Combine(Path.GetTempPath(), "portfolio-content-" + Guid.NewGuid().ToString("N"), "nested", "content.json");
        try
        {
            await ContentExporter.WriteAsync(Content, file, year: 2026);

            Assert.Equal(SiteContentBuilder.ToJson(SiteContentBuilder.Build(Content, 2026)), await File.ReadAllTextAsync(file));
        }
        finally
        {
            Directory.Delete(Path.GetDirectoryName(Path.GetDirectoryName(file))!, recursive: true);
        }
    }

    [Fact]
    public async Task The_export_refuses_a_relative_path_so_it_never_writes_in_a_surprising_folder()
    {
        await Assert.ThrowsAsync<ArgumentException>(() => ContentExporter.WriteAsync(Content, "data/content.json", year: 2026));
    }

    [Fact]
    public void The_document_resolves_every_icon_and_places_the_floating_logos_once_each()
    {
        var site = SiteContentBuilder.Build(Content, 2026);

        // Every sphere carries the symbol it draws, and every symbol is defined in one of the two sprites.
        var symbols = site.Logos.Select(l => l.SymbolId).Concat(site.SkillIcons.Select(i => i.SymbolId)).ToHashSet();
        Assert.All(site.Universe.Stars, star => Assert.Contains(star.IconSymbolId, symbols));

        // Hero and page layers share the technologies, so no logo appears in both.
        var hero = site.HeroLogos.Select(l => l.Key).ToList();
        var page = site.PageLogos.Select(l => l.Key).ToList();
        Assert.Empty(hero.Intersect(page));
        Assert.All(site.HeroLogos.Concat(site.PageLogos), slot => Assert.Contains(slot.SymbolId, symbols));

        // Project cards carry their wording, and the profile links carry GitHub's and LinkedIn's icons.
        Assert.All(site.Projects, card => Assert.False(string.IsNullOrWhiteSpace(card.StatusLabel)));
        Assert.All(site.ProfileLinks, link => Assert.NotNull(link.IconMarkup));
        Assert.Equal(2026, site.Meta.Year);
    }

    [Fact]
    public void The_JSON_names_enums_as_words_and_uses_camel_case()
    {
        var json = SiteContentBuilder.ToJson(SiteContentBuilder.Build(Content, 2026));

        Assert.Contains("\"statusLabel\"", json);
        Assert.Contains("\"iconSymbolId\"", json);
        Assert.Contains("\"inDevelopment\"", json);
        Assert.DoesNotContain("\"StatusLabel\"", json);
    }
}
