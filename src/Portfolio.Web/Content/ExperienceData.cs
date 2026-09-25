using Portfolio.Web.Models;

namespace Portfolio.Web.Content;

/// <summary>
/// Employment history, newest first (ADR-0003, ADR-0006).
/// </summary>
/// <remarks>
/// Employer and end-client names are public on the owner's own profiles. No
/// internal system names, tickets or code appear, and wording matches the real
/// role ("contributed to" where the work was shared).
/// </remarks>
public static class ExperienceData
{
    public static IReadOnlyList<Role> Roles { get; } =
    [
        new Role(
            Id: "cloud-employee-hotelplan",
            Company: "Cloud Employee",
            Client: "Hotelplan UK",
            Title: "Senior Full-Stack .NET Developer",
            Period: "June 2024 to August 2026",
            Location: "Remote",
            Highlights:
            [
                "Contributed full-stack development to a live, high-traffic Umbraco booking platform and its sister sites, with more than 660 commits across eight actively worked repositories.",
                "Led a Content Security Policy hardening effort across two customer-facing production sites: removed unsafe-inline and unsafe-eval, added per-request nonces, and adopted Trusted Types.",
                "Diagnosed a layered Azure hosting stack (gateway with its web application firewall, content delivery rules, blob storage and app service configuration) to find which layer produced a given response header.",
                "Built custom Umbraco backoffice property editors in AngularJS for editors across two content platforms.",
                "Configured Model Context Protocol server integrations and shared instruction files so GitHub Copilot and Claude Code follow one set of team conventions.",
            ],
            Stack: ["C#", "ASP.NET Core", "Umbraco", "Vue.js", "AngularJS", "SCSS", "Azure", "Docker", "Jenkins"]),

        new Role(
            Id: "accenture",
            Company: "Accenture",
            Client: "Centene, then UnitedHealth Group",
            Title: "Package App Senior Analyst",
            Period: "January 2022 to June 2024",
            Location: null,
            Highlights:
            [
                "Remediated GitHub code-scanning findings and reviewed colleagues' code for the whole tenure, flagging redundant logic and proposing shared helpers instead of duplicated code.",
                "On the UnitedHealth Group claims system: improved query-based pagination performance and fixed data-consistency and duplicate-record problems across two core applications.",
                "Took part in upgrading legacy ASP.NET Core 2.x systems to ASP.NET Core 6, one closely monitored dependency step at a time.",
                "Used GitHub Copilot from 2022, when the firm began encouraging its adoption to evaluate reliability and speed gains during its generative artificial intelligence rollout.",
            ],
            Stack: ["C#", "ASP.NET Core", "Razor Pages", "SQL Server", "GitHub"]),

        new Role(
            Id: "collabera-centene",
            Company: "Collabera",
            Client: "Centene (through Accenture)",
            Title: "ASP.NET Developer",
            Period: "May 2020 to January 2022",
            Location: null,
            Highlights:
            [
                "Led the supplemental-file processing for the 834 healthcare enrolment standard (add, change, term, void, reinstatement) with minimal guidance and close quality assurance coordination, on a case-management platform spanning 19 health plans.",
                "Migrated more than 500,000 member records into a repository of over one million records, and built the legacy project framework from scratch for a newly acquired state health plan.",
                "Took part in upgrading the platform from .NET Framework 4.6 to 4.8, and maintained its legacy Windows Presentation Foundation desktop companion app.",
                "Diagnosed ServiceNow-escalated production issues for inbound 834 processing as data or code root cause, resolving them with peer-reviewed data-correction scripts or direct code fixes.",
            ],
            Stack: ["C#", ".NET Framework", "React", "Redux", "MongoDB", "WPF"]),

        new Role(
            Id: "bcs-scoot",
            Company: "BCS Technology International",
            Client: "Scoot Airlines",
            Title: ".NET Developer",
            Period: "September 2018 to March 2020",
            Location: null,
            Highlights:
            [
                "Built backend services for customer-facing airline booking tools, deployed on Amazon Web Services.",
                "Integrated the Navitaire Dot-Rez reservation system over its Simple Object Access Protocol interface for flight-transfer management.",
                "Designed the schema and Entity Framework Core Code First models against MySQL, with 90 percent backend test coverage in xUnit.",
            ],
            Stack: ["C#", "ASP.NET", "Vue.js", "EF Core", "MySQL", "xUnit", "AWS"]),

        new Role(
            Id: "gurrea",
            Company: "F. Gurrea Construction, Inc.",
            Client: null,
            Title: "Computer Programmer",
            Period: "June 2017 to July 2018",
            Location: null,
            Highlights:
            [
                "First professional role: an in-house Windows desktop application that generates employment and attendance certificates, with Dapper data access against SQL Server and an editable print preview.",
            ],
            Stack: ["C#", "WinForms", "Dapper", "SQL Server"]),
    ];
}
