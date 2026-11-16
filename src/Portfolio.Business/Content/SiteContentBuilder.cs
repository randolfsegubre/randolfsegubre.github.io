using System.Text.Json;
using System.Text.Json.Serialization;
using Portfolio.Business.Models;

namespace Portfolio.Business.Content;

/// <summary>
/// Builds the one JSON document the Nuxt front end reads, from the content classes (ADR-0011).
/// </summary>
/// <remarks>
/// This class works out what the front end should not decide for itself: the page titles, the project status
/// wording, each sphere's icon and where the floating logos go. It is a pure function of the content (which
/// holds every word and drawing, ADR-0012), so a test can check it without a browser or a database, and the same
/// JSON is served by <c>GET /api/content</c> and written by <c>--export-content</c> for the static build.
/// </remarks>
public static class SiteContentBuilder
{
    /// <summary>The serializer settings both the API and the export use, so they can never differ.</summary>
    public static JsonSerializerOptions JsonOptions { get; } = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    // Where the floating logos go is a presentation decision tied to the stylesheet, so it stays in code; which
    // technologies float, and how many go in the hero, are content. x = left; either y (from the top) or b (from the
    // bottom); edge = keep on phones.
    // Hero slots sit in the hero's top padding (a fixed 1rem from its top edge), which is about 4rem tall and
    // never holds content at any screen size, so these logos can never land on the portrait, name or text.
    private static readonly (string X, string? Y, string? B, bool Edge, int Seconds, int Delay)[] HeroSlots =
    [
        ("6%", "0.8rem", null, true, 15, -2),
        ("46%", "0.8rem", null, false, 17, -6),
        ("86%", "0.8rem", null, true, 16, -4),
    ];

    private static readonly (string X, string? Y, string? B, bool Edge, int Seconds, int Delay)[] PageSlots =
    [
        ("6%", "14%", null, true, 16, -2),
        ("90%", "10%", null, true, 18, -6),
        ("3%", "52%", null, false, 15, -4),
        ("95%", "48%", null, false, 17, -1),
        ("12%", null, "10%", true, 19, -9),
        ("48%", null, "6%", false, 16, -7),
        ("88%", null, "12%", false, 18, -3),
        ("30%", "34%", null, false, 20, -5),
        ("70%", "26%", null, false, 17, -8),
        ("56%", "62%", null, false, 19, -2),
    ];

    /// <summary>
    /// Builds the whole document.
    /// </summary>
    /// <param name="content">The content service.</param>
    /// <param name="year">The year to show in the footer; the caller passes it so the build is repeatable.</param>
    public static SiteContent Build(IPortfolioContent content, int year)
    {
        var profile = content.Profile;
        var site = content.Site;

        // STEP 1 of 5: the head of each page.
        var meta = new SiteMeta(
            SiteUrl: site.Url,
            HomeTitle: $"{profile.Name} | {profile.Title}",
            NotFoundTitle: $"Page not found | {profile.Name}",
            Description: site.Description,
            ShareDescription: site.ShareDescription,
            ShareImageUrl: profile.PhotoUrl is null ? null : site.Url + profile.PhotoUrl,
            ShareImageAlt: profile.PhotoAlt,
            Year: year);

        // STEP 2 of 5: profile links with their icons, and project cards with their status wording.
        var links = profile.Links.Select(l => new ProfileLink(l.Label, l.Href, content.FindSocialIconMarkup(l.Label))).ToList();
        var projects = content.Projects.Select(p => p.Status switch
        {
            ProjectStatus.InDevelopment => new ProjectCard(p, "In development", "badge-in-development"),
            _ => new ProjectCard(p, "Working reference", "badge-reference"),
        }).ToList();

        // STEP 3 of 5: the skills cloud, each star carrying the icon it draws.
        var stars = content.Universe.Stars.Select(placed =>
        {
            var icon = content.ResolveSkillIcon(placed.Star.Icon)
                ?? throw new InvalidOperationException($"Star '{placed.Star.Id}' has no icon '{placed.Star.Icon}'.");
            return new StarView(
                placed.Star.Id, placed.Star.Label, placed.Star.Caption, placed.Star.Main, placed.Depth,
                placed.Azimuth, placed.Elevation, placed.Radius, icon.SymbolId, icon.Fill, icon.OnDark);
        }).ToList();
        var universe = new UniverseView(content.Universe.Intro, content.Universe.Journey, content.Universe.Links, stars);

        // STEP 4 of 5: the sprites, and where the floating logos go. The hero takes the first few tags and the
        // page layer the rest, so each logo appears once.
        var logos = content.TechLogos.Select(l => new LogoSymbol(l.Key, l.SymbolId, l.Markup)).ToList();
        var skillIcons = content.SkillIcons.Select(i => new LogoSymbol(i.Key, i.SymbolId, i.Markup)).ToList();
        var heroLogos = PlaceLogos(content, profile.BackdropTags.Take(site.HeroLogoCount), HeroSlots);
        var pageLogos = PlaceLogos(content, profile.BackdropTags.Skip(site.HeroLogoCount), PageSlots);

        // STEP 5 of 5: assemble.
        return new SiteContent(
            meta, profile, links, content.LiveSites, projects, content.Experience, content.Skills, universe,
            content.HeroTags, logos, skillIcons, heroLogos, pageLogos,
            content.CodeStreams.Select(s => new CodeLane(s.Code, s.Seconds, s.Reverse)).ToList());
    }

    /// <summary>The JSON text for the document, with the settings every consumer shares.</summary>
    public static string ToJson(SiteContent content) => JsonSerializer.Serialize(content, JsonOptions);

    /// <summary>
    /// Gives each technology that has a logo the next free slot, skipping technologies with no logo.
    /// </summary>
    private static List<LogoSlot> PlaceLogos(IPortfolioContent content, IEnumerable<string> technologies, (string X, string? Y, string? B, bool Edge, int Seconds, int Delay)[] slots)
    {
        var placed = new List<LogoSlot>();
        foreach (var tech in technologies)
        {
            var logo = content.FindTechLogo(tech);
            if (logo is null || placed.Count >= slots.Length)
            {
                continue;
            }

            var slot = slots[placed.Count];
            var style = $"--x:{slot.X};" + (slot.Y is not null ? $"--y:{slot.Y};" : $"--b:{slot.B};") + $"--d:{slot.Seconds}s;--delay:{slot.Delay}s";
            placed.Add(new LogoSlot(logo.Key, logo.SymbolId, slot.Edge, style));
        }

        return placed;
    }
}
