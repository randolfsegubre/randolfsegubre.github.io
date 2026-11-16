# Architecture and Patterns Guide

This guide explains why the code is shaped the way it is, so someone opening
it cold (including the owner, months from now, with no assistant) can follow
the reasoning. Each pattern states its rejected alternative. The current shape
is ADR-0011; the earlier Razor-views shape (ADR-0007) is kept as history.

## Shape of the system

```
Build time (CI or your machine)                      Run time (visitor)
--------------------------------                     ------------------
content/*.json  (source of truth)                    GitHub Pages serves
   |                                                 web/.output/public:
npm run generate   (in web/)                           index.html, 404.html,
   |  dotnet run --project src/Portfolio.Api           css/site.css, js/*.js,
   |  -- --migrate --seed <folder>                     images, media, files
   |     --export-content <file>
   v
 PRESENTATION  Portfolio.Api       Program.cs, ContentExporter (HTTP, commands)
   |   asks for content or maintenance, never sees a row
   v
 BUSINESS      Portfolio.Business  ContentService, ContentMapper, ContentFiles,
   |   models, SiteContentBuilder, SkillUniverseLayout
   |   speaks in rows to the layer below
   v
 DATA ACCESS   Portfolio.Data      ContentRepository, PortfolioDbContext, migrations
   |
   v
 SQL Server (schema "content")  --> read back --> web/data/content.json
   |
   v
 nuxt generate: Vue components in web/app/ draw content.json
 into index.html and 404.html (no Nuxt JavaScript is shipped)
```

Data flows one way: `content/*.json`, through the business layer into SQL Server, back out through
`IPortfolioContent` and `SiteContentBuilder`, into one JSON document, into Vue components, into
HTML. Nothing in the page writes back to the data, and nothing from the .NET side or the database
runs in production.

## Pattern catalog

| Pattern | Where | Why | Rejected alternative |
|---|---|---|---|
| **API plus static front end** | `src/Portfolio.Api`, `web/` | The owner's home stacks (.NET and Vue.js) each do what they are best at; the output is still plain files (ADR-0001, ADR-0011) | A live API on a second host: another thing to keep running and to open to cross-origin requests |
| **Three-layer architecture** | `Portfolio.Api` (presentation), `Portfolio.Business`, `Portfolio.Data`; `LayeringTests` | Each kind of work has one home, and a one-way dependency rule that the compiler and a test both enforce (ADR-0013) | One project with folders: nothing stops an endpoint reaching the database |
| **Repository** | `IContentRepository` and `ContentRepository` in the data layer | The business layer asks for rows and never builds a query; a test replaces the database with an in-memory fake | Using the DbContext from the business layer: couples every rule to Entity Framework |
| **Mapper between two shapes** | `ContentMapper` (rows to records and back) | The schema and the content records can change independently; a round-trip test proves nothing is lost without a database | Mapping the records straight onto tables: every record change becomes a schema change |
| **Replace-all seeding in one transaction** | `ContentRepository.ReplaceAsync` | The database always equals the files, a failed seed leaves the old content, and running it twice is harmless (ADR-0012) | Diffing and upserting: more code, and stale rows survive |
| **Rules in the schema, not only in code** | Foreign keys, check constraints and a filtered unique index in `PortfolioDbContext` | The database itself refuses a second centre star, a bad status or a journey step for a missing star; 13 cases are tested against a real SQL Server | Trusting application code alone: one bug away from bad data |
| **Strict data files** | `ContentFiles` (unknown fields, nulls and bad JSON are errors) | A typo in a data file stops the build and names the file | Lenient parsing: a misspelt field is silently ignored |
| **Dependency injection and Dependency Inversion** | `IContentSource`, `IContentRepository` and `IPortfolioContent` injected into `Program.cs`, `ContentService` and `SiteContentBuilder` | They depend on interfaces, so the source of content can change (it already did, from C# to SQL Server) without touching them, and tests can substitute it | `static` access: simpler, but couples everything to one data source |
| **One content document** (a Data Transfer Object, DTO) | `Models/SiteContent.cs`, `SiteContentBuilder` | The front end draws; the backend decides everything that is not markup (titles, status wording, icon choice, logo placement) | Letting each component compute its own: the same rule in two languages that drift |
| **Data-driven presentation** (content separated from view) | `content/*.json`, all components | Content changes often, layout rarely; strict reading and tests guard the data (ADR-0003, ADR-0012) | Text inside components or classes: every edit is a code edit and duplicates hide |
| **Immutable records** | `Models/ContentModels.cs`, `Models/SiteContent.cs` | Content never changes after startup, so a shared singleton is safe for every request and for the export | Mutable classes: invites accidental changes |
| **Small components** | `ProjectCard`, `TagList`, `ExternalLink`, one per section | One place enforces heading structure, link safety and chip markup | Repeating raw `<a target="_blank">` and forgetting `rel="noopener noreferrer"` |
| **Build-time static generation** (pre-rendering) | `nuxt generate`, `nitro.prerender` in `nuxt.config.js` | GitHub Pages cannot run a server; generation gives free hosting and an instant first load (ADR-0001, ADR-0011) | A live server on a free tier: cold starts on a recruiter's first click |
| **No framework JavaScript in the browser** | `features.noScripts` in `nuxt.config.js` | Hydration would rewrite the DOM that `site.js` manages and force new Content Security Policy allowances (ADR-0009, ADR-0011) | Nuxt's normal bundle: more scripts, a looser policy, a changed page |
| **Progressive enhancement** | `theme-init.js` in the head, toggle revealed by `theme.js` | The right theme paints before scripts load; the toggle never shows as a dead button without scripting | Rendering the toggle always: a useless control when scripting is off |
| **Generated view of the same data** | `DeveloperRecord.vue` renders the profile as a C# record | The code card can never disagree with the rest of the page (ADR-0003, ADR-0008) | Hand-typed code text: drifts from the data the first time the profile changes |
| **Decorative layer isolated from content** | `CodeBackdrop.vue`, `aria-hidden`, `pointer-events: none` | Ambient motion adds identity without adding noise for assistive technology or blocking clicks | Putting fragments in the content flow: read aloud and in the tab order |
| **Enhancement-only behavior** | `web/public/js/site.js`: reveal, active-section highlight, progress bar, skills cloud | Content is hidden for a reveal only when the `js` class is set, and reduced motion shows everything at once | Hiding content in CSS unconditionally: blank sections if the script fails |
| **Design tokens** | `:root` custom properties in `site.css` | One block controls color, spacing and type in both themes (ADR-0004) | Hard-coded colors scattered through rules |
| **Content lint as a test** | `ContentTests.cs` | Turns the writing rules (ADR-0006) into a failing build instead of a memory | Relying on review to notice a bare abbreviation or a stray em dash |
| **Test the published output** | `SiteFactory` serves `web/.output/public` | What is tested is what is deployed, so a framework default that weakens security fails a test (`SiteOutputTests`) | Testing only the .NET side: Nuxt could change the page unseen |
| **Fail loudly** | `Program.cs` and `ContentExporter` reject a relative export path; `serve.mjs` refuses traversal and dotfiles | A misplaced export must never be published silently | Writing whatever came back |

## SOLID notes

- **Single Responsibility:** `SiteContentBuilder` shapes the document, the
  repository talks to the database, `ContentMapper` translates, `ContentFiles`
  reads the data files, `ContentExporter` writes the file, each component shows
  one thing.
- **Open/Closed:** adding a project or role means adding a record, not
  editing a component or the builder.
- **Liskov:** any `IPortfolioContent` can replace the default one.
- **Interface Segregation:** components receive only the slice of the document
  they draw (a `profile`, a list of `projects`), not the whole of it.
- **Dependency Inversion:** see the injection row above.

## Accessibility architecture

Landmarks (`header`, `main`, `footer`, `nav`) and one `h1` per page; a skip
link to `#main`; every section labelled by its heading; focus styles never
removed; external links announce that they open a new tab; motion is disabled
under `prefers-reduced-motion`.

## What is deliberately absent

No database in production, no authentication, no framework JavaScript in the browser, no CSS
framework, no analytics, no web fonts, no runtime network requests, and no
server at request time. Each would add weight or cost for no visitor benefit.
Nuxt, the .NET backend and SQL Server exist only at build time (and on the developer's machine).
