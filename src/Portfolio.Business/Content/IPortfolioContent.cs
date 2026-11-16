using Portfolio.Business.Models;

namespace Portfolio.Business.Content;

/// <summary>
/// The single source of the site's words and drawings (ADR-0003, ADR-0012).
/// </summary>
/// <remarks>
/// The content builder, the API and the exporter depend on this interface, not on where the data lives. It is
/// written down in the JSON files under <c>content/</c>, stored in SQL Server, and read from there when the app
/// starts (Dependency Inversion, see the patterns guide). The data is immutable once loaded, so one shared
/// instance is safe for every request and for the export.
/// </remarks>
public interface IPortfolioContent
{
    /// <summary>Facts about the deployed site itself: its address and its descriptions.</summary>
    SiteSettings Site { get; }

    Profile Profile { get; }

    /// <summary>Live public websites the owner contributed to (client work).</summary>
    IReadOnlyList<LiveSite> LiveSites { get; }

    /// <summary>Projects in display order: featured ones first.</summary>
    IReadOnlyList<Project> Projects { get; }

    /// <summary>Employment history, newest first.</summary>
    IReadOnlyList<Role> Experience { get; }

    /// <summary>The full skills list, grouped. Kept under the star map so nothing is lost.</summary>
    IReadOnlyList<SkillGroup> Skills { get; }

    /// <summary>The skills star map: .NET at the centre, laid out and ready to draw (ADR-0010).</summary>
    UniverseMap Universe { get; }

    /// <summary>
    /// The faint code tags in the hero's side margins (ADR-0010). The stylesheet positions them by their order
    /// (<c>hero-tag-1</c> to <c>hero-tag-6</c>), so there must be exactly six; a test enforces it.
    /// </summary>
    IReadOnlyList<string> HeroTags { get; }

    /// <summary>The lanes of scrolling code behind the page.</summary>
    IReadOnlyList<CodeStream> CodeStreams { get; }

    /// <summary>The technology logos for the animated background and the skills sphere.</summary>
    IReadOnlyList<TechLogo> TechLogos { get; }

    /// <summary>The extra icons for the skills spheres: one more logo and some generic line glyphs.</summary>
    IReadOnlyList<SkillIcon> SkillIcons { get; }

    /// <summary>The icons beside the profile links.</summary>
    IReadOnlyList<SocialIcon> SocialIcons { get; }
}
