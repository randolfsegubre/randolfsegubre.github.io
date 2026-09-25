using Portfolio.Web.Models;

namespace Portfolio.Web.Content;

/// <summary>
/// The default <see cref="IPortfolioContent"/>: composes the four data classes.
/// </summary>
/// <remarks>
/// Registered as a singleton in <c>Program.cs</c>. The data is immutable, so one
/// shared instance is safe for every request and for the static export.
/// </remarks>
public sealed class PortfolioContent : IPortfolioContent
{
    public Profile Profile => ProfileData.Profile;

    public IReadOnlyList<LiveSite> LiveSites => LiveSiteData.Sites;

    public IReadOnlyList<Project> Projects => ProjectData.Projects;

    public IReadOnlyList<Role> Experience => ExperienceData.Roles;

    public IReadOnlyList<SkillGroup> Skills => SkillData.Groups;
}
