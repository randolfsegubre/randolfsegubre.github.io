using Portfolio.Business.Models;

namespace Portfolio.Business.Content;

/// <summary>
/// All of the site's content, in memory, as one immutable value (ADR-0012).
/// </summary>
/// <remarks>
/// Two loaders produce one: <see cref="ContentFiles"/> reads the JSON files under <c>content/</c>, and the SQL
/// Server loader reads the database. The seeder writes a snapshot into the database, so the two always describe
/// the same site, and a test proves it by comparing the document each one produces.
/// </remarks>
public sealed record ContentSnapshot(
    SiteSettings Site,
    Profile Profile,
    IReadOnlyList<LiveSite> LiveSites,
    IReadOnlyList<Project> Projects,
    IReadOnlyList<Role> Experience,
    IReadOnlyList<SkillGroup> Skills,
    UniverseMap Universe,
    IReadOnlyList<string> HeroTags,
    IReadOnlyList<CodeStream> CodeStreams,
    IReadOnlyList<TechLogo> TechLogos,
    IReadOnlyList<SkillIcon> SkillIcons,
    IReadOnlyList<SocialIcon> SocialIcons) : IPortfolioContent;
