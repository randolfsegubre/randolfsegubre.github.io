# Walkthrough: how a request flows through the code

This traces what actually happens, in order, from build time (the export) to a
visitor's page on screen. It explains vocabulary as it goes and points at the
Architecture Decision Records (ADRs) instead of repeating their reasoning.

## Vocabulary

- **MVC (Model-View-Controller):** the controller receives the request, the
  model carries the data, and the view turns the data into HyperText Markup
  Language (HTML).
- **Razor view / partial:** a `.cshtml` file mixing HTML and C#. A partial is
  a reusable piece included by another view.
- **View model:** the one object the controller hands to a view
  (`PortfolioViewModel`).
- **Dependency injection:** the framework builds objects and passes them in
  through constructors or `@inject`, so classes ask for an interface
  (`IPortfolioContent`) instead of creating their own dependencies.
- **Static export:** at build time the app renders its own pages and saves the
  HTML as files, because GitHub Pages can only serve files (ADR-0001,
  ADR-0007).

## The flow, step by step

**At build time (what CI and `--export` do):**

1. **`dotnet run -- --export "$PWD/dist"`** starts `Program.cs`.
2. **`Program.cs` (STEP 1 of 4)** registers the services: MVC with views, and
   `PortfolioContent` as the implementation of `IPortfolioContent`.
3. **STEP 2 of 4** builds the app and sets up the pipeline: static files, then
   MVC routing.
4. **STEP 3 of 4** sees `--export`, checks the folder is absolute, starts the
   app on a free local port, and finds the address Kestrel chose.
5. **`StaticExporter.ExportAsync`** runs four steps: empty the output folder;
   copy `wwwroot` (stylesheet, script, favicon); request `/` and `/not-found`
   and save the HTML as `index.html` and `404.html`; write `.nojekyll`. Any
   response other than 200 throws, so a broken site is never published.

**For each page the exporter requests (a normal MVC request):**

6. **Routing** maps `/` to `HomeController.Index` and `/not-found` to
   `NotFoundPage` (an attribute route).
7. **`HomeController.Index`** asks `IPortfolioContent` for the profile,
   projects, experience and skills, packs them into a `PortfolioViewModel`,
   and returns `View(model)`.
8. **`Views/Home/Index.cshtml`** sets the page title and includes one partial
   per section, passing each only the slice it needs.
9. **`_Layout.cshtml` wraps it:** document head (with the inline theme script
   that runs before first paint), skip link, `_Header`, the `<main>` landmark
   holding the page, and `_Footer`.
10. **Shared partials keep details consistent:** `_ExternalLink` adds
    `rel="noopener noreferrer"` and a screen-reader hint to every outbound
    link; `_TagList` renders chips as a real list; `_ProjectCard` shows the
    "Known gap" line only if the data names one (ADR-0006).

**In the visitor's browser:**

11. GitHub Pages returns the saved `index.html`. The inline script applies a
    remembered theme before anything paints, and `site.css` reads its color
    tokens (ADR-0004). Dark royal purple is the default for everyone; only an
    explicit "light" choice changes it.
12. **`theme.js`** reveals the theme button (rendered hidden) and wires it: it
    flips `data-theme` on `<html>`, remembers the choice, and updates the
    label.
13. **`site.js`** adds three enhancements (ADR-0008): elements marked
    `data-reveal` fade in as they scroll into view; the navigation link of the
    section in view gets `aria-current`, which the stylesheet turns into a gold
    underline; and a gold bar at the top tracks scroll progress. With scripting
    off, or reduced motion on, everything is simply visible.

**What the hero is made of (ADR-0008):** `_Hero.cshtml` lays out the copy beside
`_DeveloperRecord.cshtml`, a code card that renders the same `Profile` as a C#
record (so it cannot disagree with the page), over `_CodeBackdrop.cshtml`, a
decorative layer of drifting C# fragments hidden from assistive technology.
`_StatsStrip.cshtml` then shows checkable facts under the hero, instead of
unverifiable skill percentage bars.

## Following one claim end to end

Take the BudgetPH "Known gap" sentence:

1. It is written in `Content/ProjectData.cs` as `HonestNote`.
2. `ContentTests` checks it against the writing rules.
3. `_ProjectsSection.cshtml` passes the project to `_ProjectCard.cshtml`.
4. `_ProjectCard.cshtml` renders `<p class="honest-note">` because `HonestNote`
   is not null.
5. `PageRenderTests` counts the rendered "honest-note" paragraphs against the
   number of projects that have one.
6. Its source is recorded in `docs/CONTENT_SOURCES.md`.

## Where to look when something breaks

| Symptom | Look at |
|---|---|
| A word or claim is wrong | `Content/*.cs`, then `docs/CONTENT_SOURCES.md` |
| Layout or color is off | `wwwroot/css/site.css` tokens at the top |
| Wrong theme on load | the inline script in `_Layout.cshtml`, then `wwwroot/js/theme.js` |
| The export fails | the message names the route and status; run the app and open that route |
| Blank page after deploy | the failed step in the GitHub Actions run |
| A test fails on writing rules | `ContentTests` says which string and why |
