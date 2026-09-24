import type { Role } from "../types";

/**
 * Employment history, newest first (ADR-0003, ADR-0006).
 *
 * Employer and end-client names are public on the owner's own profiles. No
 * internal system names, tickets or code appear, and wording matches the real
 * role ("contributed to" where the work was shared).
 */
export const experience: Role[] = [
  {
    id: "cloud-employee-hotelplan",
    company: "Cloud Employee",
    client: "Hotelplan UK",
    title: "Senior Full-Stack .NET Developer",
    period: "June 2024 to August 2026",
    location: "Remote",
    highlights: [
      "Contributed full-stack development to a live, high-traffic Umbraco booking platform and its sister sites, with more than 660 commits across eight actively worked repositories.",
      "Led a Content Security Policy hardening effort across two customer-facing production sites: removed unsafe-inline and unsafe-eval, added per-request nonces, and adopted Trusted Types.",
      "Diagnosed a layered Azure hosting stack (gateway with its web application firewall, content delivery rules, blob storage and app service configuration) to find which layer produced a given response header.",
      "Built custom Umbraco backoffice property editors in AngularJS for editors across two content platforms.",
      "Configured Model Context Protocol server integrations and shared instruction files so GitHub Copilot and Claude Code follow one set of team conventions.",
    ],
    stack: ["C#", "ASP.NET Core", "Umbraco", "Vue.js", "AngularJS", "SCSS", "Azure", "Docker", "Jenkins"],
  },
  {
    id: "accenture",
    company: "Accenture",
    client: "Centene, then UnitedHealth Group",
    title: "Package App Senior Analyst",
    period: "January 2022 to June 2024",
    location: null,
    highlights: [
      "Remediated GitHub code-scanning findings and reviewed colleagues' code for the whole tenure, flagging redundant logic and proposing shared helpers instead of duplicated code.",
      "On the UnitedHealth Group claims system: improved query-based pagination performance and fixed data-consistency and duplicate-record problems across two core applications.",
      "Took part in upgrading legacy ASP.NET Core 2.x systems to ASP.NET Core 6, one closely monitored dependency step at a time.",
      "Used GitHub Copilot from 2022, when the firm began encouraging its adoption to evaluate reliability and speed gains during its generative artificial intelligence rollout.",
    ],
    stack: ["C#", "ASP.NET Core", "Razor Pages", "SQL Server", "GitHub"],
  },
  {
    id: "collabera-centene",
    company: "Collabera",
    client: "Centene (through Accenture)",
    title: "ASP.NET Developer",
    period: "May 2020 to January 2022",
    location: null,
    highlights: [
      "Led the supplemental-file processing for the 834 healthcare enrolment standard (add, change, term, void, reinstatement) with minimal guidance and close quality assurance coordination, on a case-management platform spanning 19 health plans.",
      "Migrated more than 500,000 member records into a repository of over one million records, and built the legacy project framework from scratch for a newly acquired state health plan.",
      "Took part in upgrading the platform from .NET Framework 4.6 to 4.8, and maintained its legacy Windows Presentation Foundation desktop companion app.",
      "Diagnosed ServiceNow-escalated production issues for inbound 834 processing as data or code root cause, resolving them with peer-reviewed data-correction scripts or direct code fixes.",
    ],
    stack: ["C#", ".NET Framework", "React", "Redux", "MongoDB", "WPF"],
  },
  {
    id: "bcs-scoot",
    company: "BCS Technology International",
    client: "Scoot Airlines",
    title: ".NET Developer",
    period: "September 2018 to March 2020",
    location: null,
    highlights: [
      "Built backend services for customer-facing airline booking tools, deployed on Amazon Web Services.",
      "Integrated the Navitaire Dot-Rez reservation system over its Simple Object Access Protocol interface for flight-transfer management.",
      "Designed the schema and Entity Framework Core Code First models against MySQL, with 90 percent backend test coverage in xUnit.",
    ],
    stack: ["C#", "ASP.NET", "Vue.js", "EF Core", "MySQL", "xUnit", "AWS"],
  },
  {
    id: "gurrea",
    company: "F. Gurrea Construction, Inc.",
    client: null,
    title: "Computer Programmer",
    period: "June 2017 to July 2018",
    location: null,
    highlights: [
      "First professional role: an in-house Windows desktop application that generates employment and attendance certificates, with Dapper data access against SQL Server and an editable print preview.",
    ],
    stack: ["C#", "WinForms", "Dapper", "SQL Server"],
  },
];
