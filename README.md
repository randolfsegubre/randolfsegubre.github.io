# randolfsegubre.github.io

Randolf Segubre's portfolio website: a single page with selected projects,
experience, skills and contact details. The content is written as **JSON data
files** and stored in **SQL Server**; a **.NET 10 (C# 14) backend**, built as three
layers (presentation, business, data access), reads it from there and serves it as
JSON; a **Nuxt (Vue.js) front end** turns that JSON into plain static files at build
time, so the live site is hosted free on GitHub Pages and loads instantly.

Live at <https://randolfsegubre.github.io>.

- **Working on the code?** Read [`CLAUDE.md`](./CLAUDE.md) first, then
  [`docs/build/01_CLAUDE.md`](./docs/build/01_CLAUDE.md).
- **Want to understand how it works?** Read [`WALKTHROUGH.md`](./WALKTHROUGH.md).
- **Setup and commands:** [`docs/DEVELOPER_HANDBOOK.md`](./docs/DEVELOPER_HANDBOOK.md).

## Run it

You need the .NET SDK (see `global.json`), Node.js 24 and a SQL Server (the Developer edition on this
machine is enough; the build uses Windows authentication and creates its own `PortfolioSite` database).

```bash
cd web
npm ci
npm run generate   # loads content/*.json into SQL Server, reads it back, and Nuxt builds the site
npm run serve      # http://localhost:5099/ (localhost only)
```

```bash
dotnet test Portfolio.slnx   # run after "npm run generate": it tests the generated site and a real SQL Server
dotnet run --project src/Portfolio.Api   # the live API: GET /api/content (needs the database filled)
```

## What is in the repository

| Path | What it holds |
|---|---|
| `content/` | **The site's words and drawings as JSON data files**: profile, projects, experience, skills, the skills cloud, logos. The source of truth. See `content/README.md`. |
| `src/Portfolio.Api/` | **Presentation layer**: the API (`GET /api/content`, with security headers), the build commands (`--migrate`, `--seed`, `--export-content`) and the file exporter |
| `src/Portfolio.Business/` | **Business layer**: the content models, the document builder, the skills-cloud layout, the JSON file reader and the content service |
| `src/Portfolio.Data/` | **Data access layer**: the SQL Server schema, the Entity Framework Core migrations and the repository |
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
lives in JSON files and in SQL Server, never in code, and a test enforces the writing
rules (ADR-0003, ADR-0012). The backend is three layers whose dependencies point one
way, presentation to business to data, and a test enforces that (ADR-0013). Every claim
has a checkable source and gaps are stated plainly (ADR-0006).

## Status

See [`docs/build/04_TASKS.md`](./docs/build/04_TASKS.md).
