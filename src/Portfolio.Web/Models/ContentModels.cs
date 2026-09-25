namespace Portfolio.Web.Models;

/// <summary>
/// A labelled outbound link. The href must be https (enforced by a content test).
/// </summary>
public sealed record Link(string Label, string Href);

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
/// <param name="SignatureStack">The few product names shown in the hero's code card. Exempt from the abbreviation rule.</param>
/// <param name="Stats">Facts for the stats strip. Replaces skill percentage bars, which cannot be verified.</param>
/// <param name="BackdropTags">Technologies shown as floating logos. Each must appear in the projects, roles or skills (ADR-0006) and have a logo in <c>TechLogoData</c>.</param>
/// <param name="PhotoUrl">Site-relative path to the portrait under wwwroot, or null for no photo.</param>
/// <param name="PhotoAlt">Alternative text describing the portrait for screen reader users.</param>
/// <param name="ResumeUrl">Relative path to a resume file under wwwroot. The button is hidden while this is null.</param>
public sealed record Profile(
    string Name,
    string Title,
    string Location,
    string Headline,
    string Availability,
    string Email,
    IReadOnlyList<Link> Links,
    IReadOnlyList<string> SignatureStack,
    IReadOnlyList<Stat> Stats,
    IReadOnlyList<string> BackdropTags,
    string? PhotoUrl,
    string? PhotoAlt,
    string? ResumeUrl);

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
/// Everything the home page needs, passed from the controller to the view as one model.
/// </summary>
public sealed record PortfolioViewModel(
    Profile Profile,
    IReadOnlyList<Project> Projects,
    IReadOnlyList<Role> Experience,
    IReadOnlyList<SkillGroup> Skills);

/// <summary>
/// Input for the floating-logos partial: which technologies to show and which layer they float in.
/// </summary>
/// <param name="Variant">"hero" for the logos inside the hero, "page" for the fainter layer fixed behind the whole page.</param>
public sealed record TechLogosModel(IReadOnlyList<string> Techs, string Variant);

/// <summary>
/// Input for the shared tag-list partial: the chips and the accessible name of the list.
/// </summary>
public sealed record TagListModel(IReadOnlyList<string> Items, string Label);
