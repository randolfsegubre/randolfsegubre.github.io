# ADR-0003: Content lives in typed data modules, not in components

- **Status:** Accepted
- **Date:** 2026-09-24

## Context

The portfolio's value is its content: projects, experience, skills. That
content changes far more often than the layout (new project, new job, a
corrected claim). If the words are buried in component markup, every content
edit becomes a code edit and it is easy to miss a duplicate.

## Decision

All copy lives in `src/Portfolio.Web/Content/*.cs` as immutable C# records
(`Profile`, `Project`, `Role`, `SkillGroup`, declared in
`Models/ContentModels.cs`) behind the `IPortfolioContent` interface. The
controller and Razor views only receive data and render it; they contain no
hard-coded claims.

An automated xUnit test (`tests/Portfolio.Tests/ContentTests.cs`) enforces the
writing rules on that data: no em dashes, no bare abbreviations in prose,
https-only links, unique identifiers, no empty fields.

*(The first build, ADR-0002, held the same data as TypeScript objects. The
rule is unchanged; only the language moved to C# with ADR-0007.)*

## Consequences

- Updating the site after a new project or job means editing one data file.
- The compiler catches a project card missing a field.
- The writing rules are enforced by a test instead of relying on memory.
- Slightly more indirection than inline text. Accepted.

## Alternatives rejected

- **Markdown files per project with a loader:** nicer for long prose, but
  adds a build step and a parsing dependency for a handful of short cards.
  Revisit if a project grows a full write-up page.
- **A headless Content Management System (CMS):** a service and an account
  to maintain for content that changes a few times a year.
