# Architecture and Patterns Guide

This guide explains why the code is shaped the way it is, so someone opening
it cold (including the owner, months from now, with no assistant) can follow
the reasoning. Each pattern states its rejected alternative.

## Shape of the system

```
Build time (CI or your machine)                 Run time (visitor)
--------------------------------                ------------------
dotnet run -- --export dist                     GitHub Pages serves dist/
   |                                               index.html, 404.html,
   |  starts the MVC app on a free port            css/site.css, js/theme.js
   v
Program.cs -> Kestrel -> MVC pipeline
   GET /           -> HomeController.Index        -> Views/Home/Index.cshtml
   GET /not-found  -> HomeController.NotFoundPage -> Views/Home/NotFound.cshtml
   |
   v
StaticExporter writes each response as a file, copies wwwroot/
```

Data flows one way: `Content/*.cs` records, through `IPortfolioContent`, into
a view model, into Razor markup. Nothing in the page writes back to the data.

## Pattern catalog

| Pattern | Where | Why | Rejected alternative |
|---|---|---|---|
| **Model-View-Controller (MVC)** | `HomeController`, `Views/`, `Models/` | The owner's home stack, and it separates HTTP handling, data shape and markup (ADR-0007) | Minimal API returning string HTML: no view separation, no partials |
| **Dependency injection and Dependency Inversion** | `IPortfolioContent` injected into the controller and into two partials with `@inject` | The controller depends on an interface, so the content source could change (Markdown, JSON) without touching it, and tests can substitute it | `static` access from the controller: simpler, but couples it to one data source |
| **Data-driven presentation** (content separated from view) | `Content/*.cs`, all partials | Content changes often, layout rarely; the compiler guards the data (ADR-0003) | Text inside Razor: every edit is a markup edit and duplicates hide |
| **Immutable records** | `Models/ContentModels.cs` | Content never changes after startup, so a shared singleton is safe for every request and for the export | Mutable classes: invites accidental changes |
| **Partial views as small components** | `_ProjectCard`, `_TagList`, `_ExternalLink`, one per section | One place enforces heading structure, link safety and chip markup | Repeating raw `<a target="_blank">` and forgetting `rel="noopener noreferrer"` |
| **Build-time static export** (a form of pre-rendering) | `StaticExporter`, the `--export` mode in `Program.cs` | GitHub Pages cannot run a server; the export gives free hosting and an instant first load (ADR-0001, ADR-0007) | A live server on a free tier: cold starts on a recruiter's first click |
| **Progressive enhancement** | inline theme script in `_Layout.cshtml`, hidden toggle revealed by `theme.js` | The right theme paints before scripts load; the toggle never shows as a dead button without scripting | Rendering the toggle always: a useless control when scripting is off |
| **Generated view of the same data** | `_DeveloperRecord.cshtml` renders the profile as a C# record | The code card can never disagree with the rest of the page (ADR-0003, ADR-0008) | Hand-typed code text: drifts from the data the first time the profile changes |
| **Decorative layer isolated from content** | `_CodeBackdrop.cshtml`, `aria-hidden`, `pointer-events: none` | Ambient motion adds identity without adding noise for assistive technology or blocking clicks | Putting fragments in the content flow: read aloud and in the tab order |
| **Enhancement-only behavior** | `wwwroot/js/site.js`: reveal, active-section highlight, progress bar | Content is hidden for a reveal only when the `js` class is set, and reduced motion shows everything at once | Hiding content in CSS unconditionally: blank sections if the script fails |
| **Design tokens** | `:root` custom properties in `site.css` | One block controls color, spacing and type in both themes (ADR-0004) | Hard-coded colors scattered through rules |
| **Content lint as a test** | `ContentTests.cs` | Turns the writing rules (ADR-0006) into a failing build instead of a memory | Relying on review to notice a bare abbreviation or a stray em dash |
| **Fail loudly** | `StaticExporter` throws on any non-200 page; `Program.cs` rejects relative export paths | A broken or misplaced export must never be published silently | Writing whatever came back |

## SOLID notes

- **Single Responsibility:** the controller shapes a response, the content
  service supplies data, the exporter writes files, each partial shows one
  thing.
- **Open/Closed:** adding a project or role means adding a record, not
  editing a view or controller.
- **Liskov:** any `IPortfolioContent` can replace the default one.
- **Interface Segregation:** partials receive only the slice of the model
  they render (a `Profile`, a list of `Project`), not the whole view model.
- **Dependency Inversion:** see the injection row above.

## Accessibility architecture

Landmarks (`header`, `main`, `footer`, `nav`) and one `h1` per page; a skip
link to `#main`; every section labelled by its heading; focus styles never
removed; external links announce that they open a new tab; motion is disabled
under `prefers-reduced-motion`.

## What is deliberately absent

No database, no authentication, no front-end framework, no CSS framework, no
analytics, no web fonts, no runtime network requests, and no server at request
time. Each would add weight or cost for no visitor benefit.
