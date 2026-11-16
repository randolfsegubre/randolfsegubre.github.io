# randolfsegubre.github.io

Randolf Segubre's portfolio website: a single page with selected projects,
experience, skills and contact details. A **.NET 10 (C# 14) backend** owns the
content and serves it as JSON; a **Nuxt (Vue.js) front end** turns that JSON into
plain static files at build time, so it is hosted free on GitHub Pages and loads
instantly.

Live at <https://randolfsegubre.github.io>.

- **Working on the code?** Read [`CLAUDE.md`](./CLAUDE.md) first, then
  [`docs/build/01_CLAUDE.md`](./docs/build/01_CLAUDE.md).
- **Want to understand how it works?** Read [`WALKTHROUGH.md`](./WALKTHROUGH.md).
- **Setup and commands:** [`docs/DEVELOPER_HANDBOOK.md`](./docs/DEVELOPER_HANDBOOK.md).

## Run it

You need the .NET SDK (see `global.json`) and Node.js 24.

```bash
cd web
npm ci
npm run generate   # the .NET backend writes the content, Nuxt builds the site
npm run serve      # http://localhost:5099/ (localhost only)
```

```bash
dotnet test Portfolio.slnx   # run after "npm run generate": it tests the generated site
dotnet run --project src/Portfolio.Api   # the live API: GET /api/content
```

## What is in the repository

| Path | What it holds |
|---|---|
| `src/Portfolio.Api/Content/` | All the site's words as C# records: profile, projects, experience, skills, the skills cloud |
| `src/Portfolio.Api/Models/` | The content records and the content document (`SiteContent`) the front end reads |
| `src/Portfolio.Api/Export/` | Writes the content document to a file for the build |
| `src/Portfolio.Api/Program.cs` | The API (`GET /api/content`, with security headers) and the export mode |
| `web/app/` | The Nuxt front end: the page shell, the page and the Vue components |
| `web/public/` | Stylesheet, scripts, favicon, `robots.txt`, `sitemap.xml`, résumé, portrait, hero video |
| `web/scripts/` | `export-content.mjs` (asks the backend for the content) and `serve.mjs` (local static server) |
| `web/nuxt.config.js` | Nuxt settings, including the Content Security Policy |
| `tools/hero-video/` | Source and rebuild script for the hero background video (not part of the site build) |
| `tests/Portfolio.Tests/` | xUnit tests: content rules, the API, and the generated HTML, CSS and files |
| `docs/adr/` | Architecture Decision Records: why it is built this way |
| `docs/build/` | Build plan, operating manual, patterns guide, tasks, devlog |
| `docs/CONTENT_SOURCES.md` | Where each claim on the site comes from |
| `.github/workflows/deploy.yml` | Audit, generate, test and publish to GitHub Pages on every push to `main` |

## Design rules in one paragraph

A .NET API feeds a Nuxt front end that is generated to a static site, with no live
server and no third-party requests (ADR-0001, ADR-0011). No Nuxt JavaScript runs in
the browser, so the strict Content Security Policy is unchanged (ADR-0009). Content
lives in C# records and a test enforces the writing rules (ADR-0003). Every claim has
a checkable source and gaps are stated plainly (ADR-0006).

## Status

See [`docs/build/04_TASKS.md`](./docs/build/04_TASKS.md).
