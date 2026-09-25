using Portfolio.Web.Models;

namespace Portfolio.Web.Content;

/// <summary>
/// Owner facts for the hero, header and contact sections.
/// </summary>
/// <remarks>
/// Sources for each claim are listed in <c>docs/CONTENT_SOURCES.md</c>. The
/// location is deliberately the metro area, not a city, because his public
/// profiles name different cities within it.
/// </remarks>
public static class ProfileData
{
    public static Profile Profile { get; } = new(
        Name: "Randolf Segubre",
        Title: "Senior Full-Stack .NET Developer",
        Location: "Metro Manila, Philippines",
        Headline: "I have spent more than nine years building and maintaining web platforms in C# and .NET across travel, healthcare and construction. I work on the backend, the Umbraco content management system, and the front end that sits on top of them, and I like the parts that usually go wrong in production: security headers, legacy upgrades, and data that has to be right.",
        Availability: "Open to senior full-stack .NET roles. Remote preferred.",
        Email: "rsegubre@gmail.com",
        Links:
        [
            new Link("GitHub", "https://github.com/randolfsegubre"),
            new Link("LinkedIn", "https://www.linkedin.com/in/rsegubre"),
        ],
        SignatureStack: ["C#", "ASP.NET Core", "Umbraco", "Vue.js", "SQL Server"],
        Stats:
        [
            new Stat("9+", "years of professional C# and .NET"),
            new Stat("3", "industries: travel, healthcare and construction"),
            new Stat("5", "employers since 2017"),
            new Stat("660+", "commits across eight repositories on one client platform"),
        ],
        BackdropTags:
        [
            "C#", ".NET 10", "ASP.NET Core", "Umbraco", "Vue.js", "React",
            "SQL Server", "EF Core", "MediatR", "Azure", "Docker", "xUnit",
        ],
        PhotoUrl: "/images/randolf.webp",
        PhotoAlt: "Portrait of Randolf Segubre",
        ResumeUrl: null);
}
