using Portfolio.Web.Models;

namespace Portfolio.Web.Content;

/// <summary>
/// The single source of the site's words (ADR-0003).
/// </summary>
/// <remarks>
/// Controllers depend on this interface, not on where the data lives. Today it is
/// C# data compiled into the app; it could later read Markdown or JSON without
/// touching a controller or a view (Dependency Inversion, see the patterns guide).
/// </remarks>
public interface IPortfolioContent
{
    Profile Profile { get; }

    /// <summary>Live public websites the owner contributed to (client work).</summary>
    IReadOnlyList<LiveSite> LiveSites { get; }

    /// <summary>Projects in display order: featured ones first.</summary>
    IReadOnlyList<Project> Projects { get; }

    /// <summary>Employment history, newest first.</summary>
    IReadOnlyList<Role> Experience { get; }

    IReadOnlyList<SkillGroup> Skills { get; }
}
