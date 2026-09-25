using Portfolio.Web.Models;

namespace Portfolio.Web.Content;

/// <summary>
/// Skills grouped the way the experience and projects back them up.
/// </summary>
/// <remarks>
/// Items are product or technique names. Anything listed here appears in a
/// project or role above, or on the owner's public profiles (ADR-0006).
/// </remarks>
public static class SkillData
{
    public static IReadOnlyList<SkillGroup> Groups { get; } =
    [
        new SkillGroup("backend", "Backend", ["C#", "ASP.NET Core", "ASP.NET MVC", ".NET Framework", "EF Core", "MediatR", "Clean Architecture", "SOAP and REST integration", "xUnit"]),
        new SkillGroup("frontend", "Front end", ["React", "Vue.js", "Blazor", "JavaScript", "TypeScript", "HTML and CSS", "SCSS", "Figma handoff"]),
        new SkillGroup("cms", "Content management", ["Umbraco", "Content-type modelling", "Models Builder", "Custom backoffice property editors"]),
        new SkillGroup("data", "Data", ["SQL Server", "T-SQL", "MySQL", "MongoDB", "SQLite", "Search engine query debugging"]),
        new SkillGroup("security", "Security", ["Content Security Policy", "Trusted Types", "Security headers", "OWASP ZAP", "JWT authentication", "Code scanning remediation"]),
        new SkillGroup("cloud", "Cloud and delivery", ["Azure diagnostics", "AWS", "Docker", "Jenkins", "Git and GitHub"]),
        new SkillGroup("ai", "AI-assisted engineering", ["GitHub Copilot", "Claude Code", "Model Context Protocol configuration", "Shared instruction files"]),
        new SkillGroup("practice", "Practice", ["Code review", "Legacy upgrades", "Production support triage", "Documentation-first delivery"]),
    ];
}
