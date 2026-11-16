# Architecture and Patterns Guide

This guide explains why the code is shaped the way it is, so someone opening
it cold (including the owner, months from now, with no assistant) can follow
the reasoning. Each pattern states its rejected alternative. The current shape
is ADR-0011; the earlier Razor-views shape (ADR-0007) is kept as history.

## Shape of the system

```
Build time (CI or your machine)                 Run time (visitor)
--------------------------------                ------------------
npm run generate   (in web/)                    GitHub Pages serves
   |                                            web/.output/public:
   |  1. export-content.mjs runs                  index.html, 404.html,
   |     dotnet run --project src/Portfolio.Api    css/site.css, js/*.js,
   |     -- --export-content <abs path>            images, media, files
   v
Program.cs -> ContentExporter -> web/data/content.json
   |         (the same SiteContentBuilder also serves GET /api/content)
   v
  2. nuxt generate: Vue components in web/app/ draw content.json
     into index.html and 404.html (no Nuxt JavaScript is shipped)
```

Data flows one way: `Content/*.cs` records, through `IPortfolioContent` and
`SiteContentBuilder`, into one JSON document, into Vue components, into HTML.
Nothing in the page writes back to the data, and nothing from the .NET side
runs in production.

## Pattern catalog

| Pattern | Where | Why | Rejected alternative |
|---|---|---|---|
| **API plus static front end** | `src/Portfolio.Api`, `web/` | The owner's home stacks (.NET and Vue.js) each do what they are best at; the output is still plain files (ADR-0001, ADR-0011) | A live API on a second host: another thing to keep running and to open to cross-origin requests |
| **Dependency injection and Dependency Inversion** | `IPortfolioContent` injected into `SiteContentBuilder`, `Program.cs` and `ContentExporter` | They depend on an interface, so the content source could change (Markdown, JSON) without touching them, and tests can substitute it | `static` access: simpler, but couples everything to one data source |
| **One content document** (a Data Transfer Object, DTO) | `Models/SiteContent.cs`, `SiteContentBuilder` | The front end draws; the backend decides everything that is not markup (titles, status wording, icon choice, logo placement) | Letting each component compute its own: the same rule in two languages that drift |
| **Data-driven presentation** (content separated from view) | `Content/*.cs`, all components | Content changes often, layout rarely; the compiler guards the data (ADR-0003) | Text inside components: every edit is a markup edit and duplicates hide |
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
  content classes supply data, `ContentExporter` writes the file, each
  component shows one thing.
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

No database, no authentication, no framework JavaScript in the browser, no CSS
framework, no analytics, no web fonts, no runtime network requests, and no
server at request time. Each would add weight or cost for no visitor benefit.
Nuxt and the .NET API exist only at build time.
