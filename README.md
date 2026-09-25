# randolfsegubre.github.io

Randolf Segubre's portfolio website: a single page with selected projects,
experience, skills and contact details. Written as an **ASP.NET Core MVC**
application on **.NET 10 (C# 14)** and exported to static files at build time,
so it is hosted free on GitHub Pages and loads instantly.

Live at <https://randolfsegubre.github.io>.

- **Working on the code?** Read [`CLAUDE.md`](./CLAUDE.md) first, then
  [`docs/build/01_CLAUDE.md`](./docs/build/01_CLAUDE.md).
- **Want to understand how it works?** Read [`WALKTHROUGH.md`](./WALKTHROUGH.md).
- **Setup and commands:** [`docs/DEVELOPER_HANDBOOK.md`](./docs/DEVELOPER_HANDBOOK.md).

## Run it

```bash
dotnet run --project src/Portfolio.Web
dotnet test Portfolio.slnx
```

## What is in the repository

| Path | What it holds |
|---|---|
| `src/Portfolio.Web/Content/` | All the site's words as C# records: profile, projects, experience, skills |
| `src/Portfolio.Web/Controllers/` | The single MVC controller |
| `src/Portfolio.Web/Views/` | Razor layout, page and partial views |
| `src/Portfolio.Web/Export/` | The static exporter that produces the deployable files |
| `src/Portfolio.Web/wwwroot/` | Stylesheet, theme script, favicon, robots.txt |
| `tests/Portfolio.Tests/` | xUnit tests: content rules, rendered HTML, exporter |
| `docs/adr/` | Architecture Decision Records: why it is built this way |
| `docs/build/` | Build plan, operating manual, patterns guide, tasks, devlog |
| `docs/CONTENT_SOURCES.md` | Where each claim on the site comes from |
| `.github/workflows/deploy.yml` | Test, export and publish to GitHub Pages on every push to `main` |

## Design rules in one paragraph

ASP.NET Core MVC exported to a static site, with no live server and no
third-party requests (ADR-0001, ADR-0007). Content lives in C# records and a
test enforces the writing rules (ADR-0003). Every claim has a checkable source
and gaps are stated plainly (ADR-0006).

## Status

See [`docs/build/04_TASKS.md`](./docs/build/04_TASKS.md).
