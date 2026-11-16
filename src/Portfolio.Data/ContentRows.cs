namespace Portfolio.Data;

/// <summary>
/// Every row of the content database, as one value: what the repository reads and what it writes (ADR-0012, ADR-0013).
/// </summary>
/// <remarks>
/// It lets the business layer hand over, or receive, the whole site in one piece without knowing how many tables
/// that takes. A project's links travel inside the project row, as they do in the database.
/// </remarks>
public sealed class ContentRows
{
    public SiteSettingRow Site { get; set; } = new();

    public ProfileRow Profile { get; set; } = new();

    public List<ProfileLinkRow> ProfileLinks { get; set; } = [];

    public List<ContactChannelRow> ContactChannels { get; set; } = [];

    public List<StatRow> Stats { get; set; } = [];

    public List<ProjectRow> Projects { get; set; } = [];

    public List<LiveSiteRow> LiveSites { get; set; } = [];

    public List<RoleRow> Roles { get; set; } = [];

    public List<SkillGroupRow> SkillGroups { get; set; } = [];

    public SkillUniverseRow SkillUniverse { get; set; } = new();

    public List<SkillStarRow> SkillStars { get; set; } = [];

    public List<SkillLinkRow> SkillLinks { get; set; } = [];

    public List<JourneyStepRow> JourneySteps { get; set; } = [];

    public List<HeroTagRow> HeroTags { get; set; } = [];

    public List<CodeStreamRow> CodeStreams { get; set; } = [];

    public List<TechLogoRow> TechLogos { get; set; } = [];

    public List<SkillIconRow> SkillIcons { get; set; } = [];

    public List<SocialIconRow> SocialIcons { get; set; } = [];

    /// <summary>Every row, flattened, for inserting in one go.</summary>
    internal IEnumerable<object> AllRows()
    {
        yield return Site;
        yield return Profile;
        yield return SkillUniverse;

        foreach (var list in new IEnumerable<object>[]
        {
            ProfileLinks, ContactChannels, Stats, Projects, LiveSites, Roles, SkillGroups, SkillStars, SkillLinks,
            JourneySteps, HeroTags, CodeStreams, TechLogos, SkillIcons, SocialIcons,
        })
        {
            foreach (var row in list)
            {
                yield return row;
            }
        }
    }
}
