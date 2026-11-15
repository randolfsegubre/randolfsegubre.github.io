namespace Portfolio.Web.Models;

/// <summary>
/// A labelled outbound link. The href must be https (enforced by a content test).
/// </summary>
public sealed record Link(string Label, string Href);

/// <summary>
/// One way to reach the owner by phone or a messaging app.
/// </summary>
/// <param name="Label">What the channel is, for example "Mobile and Viber".</param>
/// <param name="Display">The number as a person reads it, for example "+63 917 1022 203".</param>
/// <param name="Href">The main link: a <c>tel:</c> link, or an https link that opens a messaging app.</param>
/// <param name="ActionLabel">Text of an optional second link, for example "Open in Viber".</param>
/// <param name="ActionHref">Address of the optional second link. Its scheme may be an app link such as <c>viber:</c>.</param>
public sealed record ContactChannel(string Label, string Display, string Href, string? ActionLabel, string? ActionHref);

/// <summary>
/// One fact shown in the stats strip under the hero, for example "9+" and "years of professional C# and .NET".
/// </summary>
/// <param name="Value">The short figure. Must be a checkable fact (ADR-0006).</param>
/// <param name="Label">What the figure counts. Prose, so it follows the writing rules.</param>
public sealed record Stat(string Value, string Label);

/// <summary>
/// Top-level facts about the owner, used by the hero, header and contact section.
/// </summary>
/// <param name="Headline">One-paragraph pitch shown in the hero. Prose, so it follows the writing rules.</param>
/// <param name="Availability">Short line about what he is looking for.</param>
/// <param name="Phones">Phone and messaging-app numbers, exactly as the owner supplied them. Public on the site by his choice.</param>
/// <param name="SignatureStack">The few product names shown in the hero's code card. Exempt from the abbreviation rule.</param>
/// <param name="Stats">Facts for the stats strip. Replaces skill percentage bars, which cannot be verified.</param>
/// <param name="BackdropTags">Technologies shown as floating logos. Each must appear in the projects, roles or skills (ADR-0006) and have a logo in <c>TechLogoData</c>.</param>
/// <param name="PhotoUrl">Site-relative path to the portrait under wwwroot, or null for no photo.</param>
/// <param name="PhotoAlt">Alternative text describing the portrait for screen reader users.</param>
/// <param name="PhotoWebpUrl">Optional smaller WebP copy of the portrait for the page itself (320 by 320, twice its display size). <see cref="PhotoUrl"/> stays the fallback and the link-preview image, because LinkedIn and some other sharing crawlers do not reliably show WebP.</param>
/// <param name="Greeting">The short line above the name in the hero, so the page introduces him in his own voice. Prose, so it follows the writing rules.</param>
/// <param name="About">The paragraphs of the "About me" section, in order. Prose, so they follow the writing rules; each claim needs a row in <c>docs/CONTENT_SOURCES.md</c>.</param>
/// <param name="ResumeUrl">Site-relative path to the downloadable resume under wwwroot (a PDF). The hero button and the contact card are hidden while this is null, so the site never shows a dead download link.</param>
public sealed record Profile(
    string Name,
    string Title,
    string Location,
    string Headline,
    string Availability,
    string Email,
    IReadOnlyList<Link> Links,
    IReadOnlyList<ContactChannel> Phones,
    IReadOnlyList<string> SignatureStack,
    IReadOnlyList<Stat> Stats,
    IReadOnlyList<string> BackdropTags,
    string? PhotoUrl,
    string? PhotoAlt,
    string? ResumeUrl,
    string? PhotoWebpUrl = null,
    string Greeting = "Hi, I'm",
    IReadOnlyList<string>? About = null);

/// <summary>
/// Where a project stands. Shown on its card so nothing is oversold.
/// </summary>
public enum ProjectStatus
{
    InDevelopment,
    Reference,
}

/// <summary>
/// One portfolio project card.
/// </summary>
/// <param name="Id">Stable unique identifier, also used as the element id.</param>
/// <param name="Role">His actual role on it, stated plainly.</param>
/// <param name="Featured">Featured projects render larger and first.</param>
/// <param name="Highlights">Specific, checkable points. Prose, so they follow the writing rules.</param>
/// <param name="HonestNote">A known gap stated plainly, or null when there is none worth naming.</param>
/// <param name="Stack">Product names shown as chips. Exempt from the abbreviation rule.</param>
/// <param name="Links">Public links only. Empty for a private repository.</param>
public sealed record Project(
    string Id,
    string Name,
    string Tagline,
    string Summary,
    string Role,
    ProjectStatus Status,
    bool Featured,
    IReadOnlyList<string> Highlights,
    string? HonestNote,
    IReadOnlyList<string> Stack,
    IReadOnlyList<Link> Links);

/// <summary>
/// A live, public website the owner contributed to as a developer (client work).
/// </summary>
/// <remarks>
/// Only what any visitor can see is described here. No internal repository names, ticket
/// numbers, hosting addresses or configuration detail, ever; a test enforces this
/// (ADR-0006).
/// </remarks>
/// <param name="Address">The public web address shown on the card, for example "inghams.co.uk".</param>
/// <param name="Href">The public https link, on the site's own public domain only.</param>
/// <param name="Role">His real part, stated plainly ("contributed", never "built").</param>
/// <param name="Stack">Product names shown as chips. Each must appear in a role stack or the skills.</param>
public sealed record LiveSite(
    string Id,
    string Name,
    string Address,
    string Href,
    string Summary,
    string Role,
    IReadOnlyList<string> Highlights,
    string HonestNote,
    IReadOnlyList<string> Stack);

/// <summary>
/// One employment role.
/// </summary>
/// <param name="Client">End client, when the work was done through a staffing or consulting firm.</param>
/// <param name="Period">Human-readable period, for example "June 2024 to August 2026".</param>
/// <param name="Stack">Product names shown as chips. Exempt from the abbreviation rule.</param>
public sealed record Role(
    string Id,
    string Company,
    string? Client,
    string Title,
    string Period,
    string? Location,
    IReadOnlyList<string> Highlights,
    IReadOnlyList<string> Stack);

/// <summary>
/// A named group of skills. Items are product or technique names.
/// </summary>
public sealed record SkillGroup(string Id, string Label, IReadOnlyList<string> Items);

/// <summary>
/// One star on the skills map: a technology, where it hangs from, and one honest line about where it was used.
/// </summary>
/// <param name="Id">Stable unique identifier, also used for the element ids and the walk order.</param>
/// <param name="Label">The product name shown on the star. Must appear in the roles, projects or skills (a test enforces this).</param>
/// <param name="ParentId">The star this one grows out of, or null for the single centre. The parent decides the ring and the direction.</param>
/// <param name="Caption">One sentence about where the technology was used. Prose, so it follows the writing rules (ADR-0006).</param>
/// <param name="Icon">The key of the icon drawn inside the sphere: a technology logo or a generic glyph (<c>SkillIconData</c>). Every star needs one.</param>
/// <param name="Main">True for the owner's main stack (the profile's signature stack), which is drawn as a larger sphere than the rest. A test keeps this in step with <c>Profile.SignatureStack</c>.</param>
public sealed record SkillStar(string Id, string Label, string? ParentId, string Caption, string Icon, bool Main = false);

/// <summary>
/// A line between two stars on the map. Every parent link is drawn automatically; these are the extra cross links.
/// </summary>
public sealed record SkillLink(string FromId, string ToId);

/// <summary>
/// A star with its place on the map.
/// </summary>
/// <param name="Depth">0 for the centre, 1 for the first step out, and so on outward.</param>
/// <param name="Azimuth">Where the star sits round the 3D cloud, in degrees (0 to 360). Zero for the centre.</param>
/// <param name="Elevation">How far above (positive) or below (negative) the middle of the 3D cloud the star sits, in degrees. Zero for the centre.</param>
/// <param name="Radius">The star's distance from the centre of the 3D cloud in world units; stars one step out share the inner sphere, two steps the middle one and the rest the outer one. Zero for the centre.</param>
public sealed record PlacedStar(SkillStar Star, int Depth, double Azimuth, double Elevation, double Radius);

/// <summary>
/// The whole skills map, laid out and ready to draw (ADR-0010).
/// </summary>
/// <param name="Intro">A short line above the map. Prose, so it follows the writing rules.</param>
/// <param name="Stars">Every star with its position.</param>
/// <param name="Links">Every line to draw: parent links first, then the cross links.</param>
/// <param name="Journey">The order the "walk my journey" button visits the stars; each star appears exactly once.</param>
public sealed record UniverseMap(
    string Intro,
    IReadOnlyList<PlacedStar> Stars,
    IReadOnlyList<SkillLink> Links,
    IReadOnlyList<string> Journey);

/// <summary>
/// What the "About me" section needs: his own paragraphs, then the skills map, with the full grouped list kept underneath it.
/// </summary>
public sealed record AboutSectionModel(IReadOnlyList<string> Paragraphs, UniverseMap Map, IReadOnlyList<SkillGroup> Groups);

/// <summary>
/// Everything the home page needs, passed from the controller to the view as one model.
/// </summary>
public sealed record PortfolioViewModel(
    Profile Profile,
    IReadOnlyList<LiveSite> LiveSites,
    IReadOnlyList<Project> Projects,
    IReadOnlyList<Role> Experience,
    IReadOnlyList<SkillGroup> Skills,
    UniverseMap Universe);

/// <summary>
/// Input for the shared tag-list partial: the chips and the accessible name of the list.
/// </summary>
public sealed record TagListModel(IReadOnlyList<string> Items, string Label);
