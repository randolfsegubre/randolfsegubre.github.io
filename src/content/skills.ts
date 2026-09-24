import type { SkillGroup } from "../types";

/**
 * Skills grouped the way the experience and projects back them up.
 * Items are product or technique names. Anything listed here appears in a
 * project or role above or on the owner's public profiles (ADR-0006).
 */
export const skills: SkillGroup[] = [
  {
    id: "backend",
    label: "Backend",
    items: ["C#", "ASP.NET Core", ".NET Framework", "EF Core", "MediatR", "Clean Architecture", "SOAP and REST integration", "xUnit"],
  },
  {
    id: "frontend",
    label: "Front end",
    items: ["React", "Vue.js", "Blazor", "JavaScript", "TypeScript", "HTML and CSS", "SCSS", "Figma handoff"],
  },
  {
    id: "cms",
    label: "Content management",
    items: ["Umbraco", "Content-type modelling", "Models Builder", "Custom backoffice property editors"],
  },
  {
    id: "data",
    label: "Data",
    items: ["SQL Server", "T-SQL", "MySQL", "MongoDB", "SQLite", "Search engine query debugging"],
  },
  {
    id: "security",
    label: "Security",
    items: ["Content Security Policy", "Trusted Types", "Security headers", "OWASP ZAP", "JWT authentication", "Code scanning remediation"],
  },
  {
    id: "cloud",
    label: "Cloud and delivery",
    items: ["Azure diagnostics", "AWS", "Docker", "Jenkins", "Git and GitHub"],
  },
  {
    id: "ai",
    label: "AI-assisted engineering",
    items: ["GitHub Copilot", "Claude Code", "Model Context Protocol configuration", "Shared instruction files"],
  },
  {
    id: "practice",
    label: "Practice",
    items: ["Code review", "Legacy upgrades", "Production support triage", "Documentation-first delivery"],
  },
];
