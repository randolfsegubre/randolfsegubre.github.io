using Portfolio.Api.Models;

namespace Portfolio.Api.Content;

/// <summary>
/// The single source of the site's words (ADR-0003).
/// </summary>
/// <remarks>
/// The content builder, the API and the exporter depend on this interface, not on where the
/// data lives. Today it is C# data compiled into the app; it could later read a database or
/// JSON without touching any of them (Dependency Inversion, see the patterns guide).
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

    /// <summary>The full skills list, grouped. Kept under the star map so nothing is lost.</summary>
    IReadOnlyList<SkillGroup> Skills { get; }

    /// <summary>The skills star map: .NET at the centre, laid out and ready to draw (ADR-0010).</summary>
    UniverseMap Universe { get; }
}
