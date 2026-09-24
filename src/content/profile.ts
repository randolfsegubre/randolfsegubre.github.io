import type { Profile } from "../types";

/**
 * Owner facts for the hero, header and contact sections.
 *
 * Sources for each claim are listed in `docs/CONTENT_SOURCES.md`. The location
 * is deliberately the metro area, not a city, because his public profiles
 * name different cities within it.
 */
export const profile: Profile = {
  name: "Randolf Segubre",
  title: "Senior Full-Stack .NET Developer",
  location: "Metro Manila, Philippines",
  headline:
    "I have spent more than nine years building and maintaining web platforms in C# and .NET across travel, healthcare and construction. I work on the backend, the Umbraco content management system, and the front end that sits on top of them, and I like the parts that usually go wrong in production: security headers, legacy upgrades, and data that has to be right.",
  availability: "Open to senior full-stack .NET roles. Remote preferred.",
  email: "rsegubre@gmail.com",
  links: [
    { label: "GitHub", href: "https://github.com/randolfsegubre" },
    { label: "LinkedIn", href: "https://www.linkedin.com/in/rsegubre" },
  ],
  resumeUrl: null,
};
