using Portfolio.Business.Content;
using Portfolio.Business.Models;
using Portfolio.Data;

namespace Portfolio.Business.Services;

/// <summary>
/// Translates between the data layer's rows and the business layer's content records (ADR-0013).
/// </summary>
/// <remarks>
/// This is the one place that knows both shapes, which is why the schema can change without touching the content
/// records and the records can change without touching the schema. It is a pair of pure functions, so a test proves
/// that a trip to rows and back loses nothing without needing a database.
/// </remarks>
public static class ContentMapper
{
    /// <summary>
    /// Builds the in-memory content from rows. The skills map is laid out in space here, from the stored tree.
    /// </summary>
    /// <param name="rows">Every row, with each list in display order.</param>
    public static ContentSnapshot ToSnapshot(ContentRows rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        var profile = rows.Profile;
        return new ContentSnapshot(
            Site: new SiteSettings(rows.Site.Url, rows.Site.Description, rows.Site.ShareDescription, rows.Site.HeroLogoCount),
            Profile: new Profile(
                Name: profile.Name,
                Title: profile.Title,
                Location: profile.Location,
                Headline: profile.Headline,
                Availability: profile.Availability,
                Email: profile.Email,
                Links: [.. rows.ProfileLinks.Select(l => new Link(l.Label, l.Href))],
                Phones: [.. rows.ContactChannels.Select(p => new ContactChannel(p.Label, p.Display, p.Href, p.ActionLabel, p.ActionHref))],
                SignatureStack: profile.SignatureStack,
                Stats: [.. rows.Stats.Select(s => new Stat(s.Value, s.Label))],
                BackdropTags: profile.BackdropTags,
                PhotoUrl: profile.PhotoUrl,
                PhotoAlt: profile.PhotoAlt,
                ResumeUrl: profile.ResumeUrl,
                PhotoWebpUrl: profile.PhotoWebpUrl,
                Greeting: profile.Greeting,
                About: profile.About),
            LiveSites: [.. rows.LiveSites.Select(s => new LiveSite(s.Id, s.Name, s.Address, s.Href, s.Summary, s.Role, s.Highlights, s.HonestNote, s.Stack))],
            Projects: [.. rows.Projects.Select(p => new Project(
                p.Id, p.Name, p.Tagline, p.Summary, p.Role, StatusWords.FromWord(p.Status), p.Featured, p.Highlights, p.HonestNote, p.Stack,
                [.. p.Links.Select(l => new Link(l.Label, l.Href))]))],
            Experience: [.. rows.Roles.Select(r => new Role(r.Id, r.Company, r.Client, r.Title, r.Period, r.Location, r.Highlights, r.Stack))],
            Skills: [.. rows.SkillGroups.Select(g => new SkillGroup(g.Id, g.Label, g.Items))],
            Universe: SkillUniverseLayout.Build(
                rows.SkillUniverse.Intro,
                [.. rows.SkillStars.Select(s => new SkillStar(s.Id, s.Label, s.ParentId, s.Caption, s.Icon, s.Main))],
                [.. rows.SkillLinks.Select(l => new SkillLink(l.FromId, l.ToId))],
                [.. rows.JourneySteps.Select(j => j.StarId)]),
            HeroTags: [.. rows.HeroTags.Select(t => t.Text)],
            CodeStreams: [.. rows.CodeStreams.Select(c => new CodeStream(c.Code, c.Seconds, c.Reverse))],
            TechLogos: [.. rows.TechLogos.Select(l => new TechLogo(l.Key, l.Hex, l.Markup))],
            SkillIcons: [.. rows.SkillIcons.Select(i => new SkillIcon(i.Key, i.Markup, i.Fill, i.OnDark))],
            SocialIcons: [.. rows.SocialIcons.Select(i => new SocialIcon(i.Label, i.LogoKey, i.Markup))]);
    }

    /// <summary>
    /// Turns the content into rows. A position in a list becomes its SortOrder, counted from 1.
    /// </summary>
    /// <param name="content">The content to store.</param>
    public static ContentRows ToRows(IPortfolioContent content)
    {
        ArgumentNullException.ThrowIfNull(content);

        var profile = content.Profile;
        var universe = content.Universe;

        // The skills map is stored as it was written: the tree, the extra lines and the walk order. Where each star
        // sits in space is not stored, because it is worked out from these when the content is loaded. The layout
        // lists one line from every star to its parent first, then the extra cross links: only the cross links are
        // data, so the parent lines are skipped.
        var parentLineCount = universe.Stars.Count(p => p.Star.ParentId is not null);

        return new ContentRows
        {
            Site = new SiteSettingRow
            {
                Id = 1,
                Url = content.Site.Url,
                Description = content.Site.Description,
                ShareDescription = content.Site.ShareDescription,
                HeroLogoCount = content.Site.HeroLogoCount,
            },
            Profile = new ProfileRow
            {
                Id = 1,
                Name = profile.Name,
                Title = profile.Title,
                Location = profile.Location,
                Headline = profile.Headline,
                Availability = profile.Availability,
                Email = profile.Email,
                PhotoUrl = profile.PhotoUrl,
                PhotoAlt = profile.PhotoAlt,
                PhotoWebpUrl = profile.PhotoWebpUrl,
                ResumeUrl = profile.ResumeUrl,
                Greeting = profile.Greeting,
                SignatureStack = [.. profile.SignatureStack],
                BackdropTags = [.. profile.BackdropTags],
                About = profile.About is null ? null : [.. profile.About],
            },
            ProfileLinks = [.. profile.Links.Select((l, i) => new ProfileLinkRow { SortOrder = i + 1, Label = l.Label, Href = l.Href })],
            ContactChannels = [.. profile.Phones.Select((p, i) => new ContactChannelRow
            {
                SortOrder = i + 1,
                Label = p.Label,
                Display = p.Display,
                Href = p.Href,
                ActionLabel = p.ActionLabel,
                ActionHref = p.ActionHref,
            })],
            Stats = [.. profile.Stats.Select((s, i) => new StatRow { SortOrder = i + 1, Value = s.Value, Label = s.Label })],
            Projects = [.. content.Projects.Select((p, i) => new ProjectRow
            {
                Id = p.Id,
                SortOrder = i + 1,
                Name = p.Name,
                Tagline = p.Tagline,
                Summary = p.Summary,
                Role = p.Role,
                Status = StatusWords.ToWord(p.Status),
                Featured = p.Featured,
                HonestNote = p.HonestNote,
                Highlights = [.. p.Highlights],
                Stack = [.. p.Stack],
                Links = [.. p.Links.Select((l, n) => new ProjectLinkRow { ProjectId = p.Id, SortOrder = n + 1, Label = l.Label, Href = l.Href })],
            })],
            LiveSites = [.. content.LiveSites.Select((s, i) => new LiveSiteRow
            {
                Id = s.Id,
                SortOrder = i + 1,
                Name = s.Name,
                Address = s.Address,
                Href = s.Href,
                Summary = s.Summary,
                Role = s.Role,
                HonestNote = s.HonestNote,
                Highlights = [.. s.Highlights],
                Stack = [.. s.Stack],
            })],
            Roles = [.. content.Experience.Select((r, i) => new RoleRow
            {
                Id = r.Id,
                SortOrder = i + 1,
                Company = r.Company,
                Client = r.Client,
                Title = r.Title,
                Period = r.Period,
                Location = r.Location,
                Highlights = [.. r.Highlights],
                Stack = [.. r.Stack],
            })],
            SkillGroups = [.. content.Skills.Select((g, i) => new SkillGroupRow { Id = g.Id, SortOrder = i + 1, Label = g.Label, Items = [.. g.Items] })],
            SkillUniverse = new SkillUniverseRow { Id = 1, Intro = universe.Intro },
            SkillStars = [.. universe.Stars.Select((placed, i) => new SkillStarRow
            {
                Id = placed.Star.Id,
                SortOrder = i + 1,
                Label = placed.Star.Label,
                ParentId = placed.Star.ParentId,
                Caption = placed.Star.Caption,
                Icon = placed.Star.Icon,
                Main = placed.Star.Main,
            })],
            SkillLinks = [.. universe.Links.Skip(parentLineCount).Select((l, i) => new SkillLinkRow { SortOrder = i + 1, FromId = l.FromId, ToId = l.ToId })],
            JourneySteps = [.. universe.Journey.Select((id, i) => new JourneyStepRow { SortOrder = i + 1, StarId = id })],
            HeroTags = [.. content.HeroTags.Select((text, i) => new HeroTagRow { SortOrder = i + 1, Text = text })],
            CodeStreams = [.. content.CodeStreams.Select((c, i) => new CodeStreamRow { SortOrder = i + 1, Code = c.Code, Seconds = c.Seconds, Reverse = c.Reverse })],
            TechLogos = [.. content.TechLogos.Select((l, i) => new TechLogoRow { Key = l.Key, SortOrder = i + 1, Hex = l.Hex, Markup = l.Markup })],
            SkillIcons = [.. content.SkillIcons.Select((icon, i) => new SkillIconRow { Key = icon.Key, SortOrder = i + 1, Markup = icon.Markup, Fill = icon.Fill, OnDark = icon.OnDark })],
            SocialIcons = [.. content.SocialIcons.Select((icon, i) => new SocialIconRow { Label = icon.Label, SortOrder = i + 1, LogoKey = icon.LogoKey, Markup = icon.Markup })],
        };
    }
}

/// <summary>
/// The words a project's status is stored as, matching the check constraint on the Project table and the way the JSON names them.
/// </summary>
internal static class StatusWords
{
    public static string ToWord(ProjectStatus status) => status switch
    {
        ProjectStatus.InDevelopment => "inDevelopment",
        ProjectStatus.Reference => "reference",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown project status."),
    };

    public static ProjectStatus FromWord(string word) => word switch
    {
        "inDevelopment" => ProjectStatus.InDevelopment,
        "reference" => ProjectStatus.Reference,
        _ => throw new InvalidOperationException($"The database holds an unknown project status '{word}'."),
    };
}
