namespace Portfolio.Data;

// The database shape of the site's content (ADR-0012). The records in Models/ are the content shape the page is
// drawn from; these classes are what the tables hold. Keeping them apart lets the schema be designed as a schema
// (keys, foreign keys, check constraints, one table per kind of thing) without bending the content records, and
// the two meet in exactly two places: ContentSeeder (records to rows) and SqlContentLoader (rows back to records).
//
// Conventions that hold throughout:
//   - A table of things in display order has a SortOrder column: the primary key when the thing has no name of
//     its own, otherwise a unique column beside the name.
//   - A "singleton" table holds exactly one row (Id is always 1, enforced by a check constraint).
//   - An ordered list of plain strings that is only ever read together with its parent (a project's highlights, a
//     role's technologies) is one JSON column on the parent row. Anything with an identity or that other rows
//     point at (a project's links, a star on the skills map) is its own table with real foreign keys.

/// <summary>The site's address and descriptions. Exactly one row.</summary>
public sealed class SiteSettingRow
{
    public int Id { get; set; }

    public string Url { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ShareDescription { get; set; } = string.Empty;

    public int HeroLogoCount { get; set; }
}

/// <summary>The owner and the facts about him. Exactly one row.</summary>
public sealed class ProfileRow
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public string Headline { get; set; } = string.Empty;

    public string Availability { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhotoUrl { get; set; }

    public string? PhotoAlt { get; set; }

    public string? PhotoWebpUrl { get; set; }

    public string? ResumeUrl { get; set; }

    public string Greeting { get; set; } = string.Empty;

    /// <summary>The few product names shown in the hero's code card, in order (a JSON column).</summary>
    public List<string> SignatureStack { get; set; } = [];

    /// <summary>The technologies shown as floating logos, in order (a JSON column).</summary>
    public List<string> BackdropTags { get; set; } = [];

    /// <summary>The paragraphs of the About section, in order, or null when the profile has none (a JSON column).</summary>
    public List<string>? About { get; set; }
}

/// <summary>A profile link such as GitHub or LinkedIn.</summary>
public sealed class ProfileLinkRow
{
    public int SortOrder { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Href { get; set; } = string.Empty;
}

/// <summary>A way to reach the owner by phone or a messaging app.</summary>
public sealed class ContactChannelRow
{
    public int SortOrder { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Display { get; set; } = string.Empty;

    public string Href { get; set; } = string.Empty;

    public string? ActionLabel { get; set; }

    public string? ActionHref { get; set; }
}

/// <summary>One fact in the stats strip under the hero.</summary>
public sealed class StatRow
{
    public int SortOrder { get; set; }

    public string Value { get; set; } = string.Empty;

    public string Label { get; set; } = string.Empty;
}

/// <summary>A portfolio project card.</summary>
public sealed class ProjectRow
{
    public string Id { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Tagline { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    /// <summary>The status as a word ("inDevelopment" or "reference"), checked against the allowed words.</summary>
    public string Status { get; set; } = string.Empty;

    public bool Featured { get; set; }

    public string? HonestNote { get; set; }

    public List<string> Highlights { get; set; } = [];

    public List<string> Stack { get; set; } = [];

    public List<ProjectLinkRow> Links { get; set; } = [];
}

/// <summary>A public link on a project card.</summary>
public sealed class ProjectLinkRow
{
    public string ProjectId { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string Label { get; set; } = string.Empty;

    public string Href { get; set; } = string.Empty;
}

/// <summary>A live public website the owner contributed to.</summary>
public sealed class LiveSiteRow
{
    public string Id { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Href { get; set; } = string.Empty;

    public string Summary { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string HonestNote { get; set; } = string.Empty;

    public List<string> Highlights { get; set; } = [];

    public List<string> Stack { get; set; } = [];
}

/// <summary>One employment role.</summary>
public sealed class RoleRow
{
    public string Id { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string Company { get; set; } = string.Empty;

    public string? Client { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Period { get; set; } = string.Empty;

    public string? Location { get; set; }

    public List<string> Highlights { get; set; } = [];

    public List<string> Stack { get; set; } = [];
}

/// <summary>A named group of skills; the items are a JSON column.</summary>
public sealed class SkillGroupRow
{
    public string Id { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string Label { get; set; } = string.Empty;

    public List<string> Items { get; set; } = [];
}

/// <summary>The line above the skills map. Exactly one row.</summary>
public sealed class SkillUniverseRow
{
    public int Id { get; set; }

    public string Intro { get; set; } = string.Empty;
}

/// <summary>One star on the skills map. Its parent is another star, so the tree is held together by a foreign key.</summary>
public sealed class SkillStarRow
{
    public string Id { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string Label { get; set; } = string.Empty;

    public string? ParentId { get; set; }

    public string Caption { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;

    public bool Main { get; set; }
}

/// <summary>An extra line between two stars that are related but not parent and child.</summary>
public sealed class SkillLinkRow
{
    public int SortOrder { get; set; }

    public string FromId { get; set; } = string.Empty;

    public string ToId { get; set; } = string.Empty;
}

/// <summary>One stop on the "walk my journey" tour; each star can be visited once.</summary>
public sealed class JourneyStepRow
{
    public int SortOrder { get; set; }

    public string StarId { get; set; } = string.Empty;
}

/// <summary>A faint code tag in the hero's side margins.</summary>
public sealed class HeroTagRow
{
    public int SortOrder { get; set; }

    public string Text { get; set; } = string.Empty;
}

/// <summary>A lane of scrolling code in the page background.</summary>
public sealed class CodeStreamRow
{
    public int SortOrder { get; set; }

    public string Code { get; set; } = string.Empty;

    public int Seconds { get; set; }

    public bool Reverse { get; set; }
}

/// <summary>A technology logo, drawn as vector markup.</summary>
public sealed class TechLogoRow
{
    public string Key { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string Hex { get; set; } = string.Empty;

    public string Markup { get; set; } = string.Empty;
}

/// <summary>An icon for a skills sphere, drawn as vector markup.</summary>
public sealed class SkillIconRow
{
    public string Key { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string Markup { get; set; } = string.Empty;

    public string? Fill { get; set; }

    public bool OnDark { get; set; }
}

/// <summary>An icon beside a profile link: either reuses a technology logo (a foreign key) or carries its own markup.</summary>
public sealed class SocialIconRow
{
    public string Label { get; set; } = string.Empty;

    public int SortOrder { get; set; }

    public string? LogoKey { get; set; }

    public string? Markup { get; set; }
}
