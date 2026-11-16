# Walkthrough: how the site is built, from a claim to a visitor's screen

This traces what actually happens, in order, from the .NET backend to the page in a
visitor's browser. It explains vocabulary as it goes and points at the Architecture
Decision Records (ADRs) instead of repeating their reasoning. The shape (ADR-0011): a
**.NET backend** owns the content and serves it as JSON; a **Nuxt front end** (Vue.js
components) turns that JSON into plain static files; **GitHub Pages** serves the files.

## Vocabulary

- **Application Programming Interface (API):** here, one read-only address,
  `GET /api/content`, that returns the whole site's content as JSON.
- **Content document:** that JSON (`SiteContent`, built by `SiteContentBuilder`). It holds
  every claim plus the few values the front end must not decide for itself.
- **Nuxt:** a framework built on Vue.js. Here it is used at build time only, to turn
  components into HyperText Markup Language (HTML) files (static generation).
- **Vue component:** a `.vue` file with a template (the HTML), and sometimes a small
  script. Components only draw the content document; none holds a claim.
- **Static generation:** at build time Nuxt renders every page and saves it as a file,
  because GitHub Pages can only serve files (ADR-0001).
- **No Nuxt JavaScript in the browser:** `features.noScripts` is on, so the published
  page is plain HTML plus the page's own three small scripts. The Content Security
  Policy (`script-src 'self'`) is therefore unchanged (ADR-0009, ADR-0011).
- **Dependency injection:** the framework builds objects and passes them in through
  constructors, so classes ask for an interface (`IPortfolioContent`) instead of
  creating their own dependencies.

## The flow, step by step

**At build time (what CI and `npm run generate` do):**

1. **`web/scripts/export-content.mjs`** runs the backend in export mode:
   `dotnet run --project src/Portfolio.Api -- --export-content <absolute path>`.
2. **`Program.cs` (STEP 1 of 4)** sees `--export-content` and calls
   `ContentExporter.WriteAsync`, then exits without starting a web server. (In normal
   mode it serves the same document at `GET /api/content`, with security headers.)
3. **`SiteContentBuilder.Build`** gathers the profile, projects, experience, skills and the
   skills cloud from the `Content/*.cs` classes, and does what the old Razor views did
   that was not markup: page titles, project status wording, each sphere's icon and where
   the floating logos go. **`ContentExporter`** writes it to `web/data/content.json`
   (git-ignored, regenerated every build).
4. **`nuxt generate`** loads the components in `web/app/`, which read that file through
   `app/utils/content.js`, renders `/` and `/404`, and writes `index.html` and
   `404.html` into `web/.output/public`, together with everything in `web/public`
   (stylesheet, scripts, images, media, `robots.txt`, `sitemap.xml`, `.nojekyll`).
5. **Small Nuxt settings keep the output safe and the same as before**
   (`web/nuxt.config.js`, `web/server/plugins/`): the Content Security Policy meta tag
   is put first in `<head>` (a meta policy only protects what follows it); the inline
   speculation-rules script Nuxt adds is removed (the policy would block it); preload
   and payload files are off; the cache-busting `?v=` on the stylesheet and scripts is
   a hash of each file. Nuxt's own empty fallback pages (`200.html` and a second `404.html`) are skipped, and
   `scripts/prune-output.mjs` then deletes the `_nuxt` folder (only Nuxt's error-screen styles, which no page
   loads), refusing to do so if any page references it.
6. **`dotnet test`** then checks that generated folder (`SiteFactory` serves it to the
   tests), so what is tested is what is published.

**Inside the page (the components, in `web/app/`):**

7. **`app.vue`** is the shell: the code-lane background, the logo sprite, the floating
   logos, the skip link, `SiteHeader`, `<main>` with the page, and `SiteFooter`.
8. **`pages/index.vue`** sets the title and sharing tags and lists the sections in
   order; each section component draws just its slice of the content.
9. **Shared components keep details consistent:** `ExternalLink` adds
   `rel="noopener noreferrer"` and a screen-reader hint to every outbound link;
   `TagList` renders chips as a real list; `ProjectCard` shows the "Known gap" line
   only if the data names one (ADR-0006).

**In the visitor's browser:**

10. GitHub Pages returns the saved `index.html`. The browser enforces the policy in its
    meta tag; `theme-init.js` applies a remembered theme before anything paints, and
    `site.css` reads its color tokens (ADR-0004). Dark royal purple is the default;
    only an explicit "light" choice changes it.
11. **`theme.js`** would reveal the theme button (rendered hidden) and wire it, but the owner asked on
    2026-11-16 to hide theme switching for now: while `TOGGLE_ENABLED` is `false` at the top of the file it
    removes the button and clears any saved "light" choice, so the site is always the default dark theme.
    Setting it to `true` brings the toggle back; the light styles and the tests are untouched.
12. **`site.js`** adds the enhancements (ADR-0008, ADR-0010): elements marked
    `data-reveal` fade in; the navigation link of the section in view gets
    `aria-current`; a gold bar tracks scroll; the skills cloud turns into a 3D scene;
    headings reveal letter by letter; links change page behind a veil; and the "More projects" strip
    (a plain scroll-snap row that already scrolls without script) gains previous and next buttons, a
    "Showing 1 to 2 of 4" count, and mouse dragging with a flick to the nearest card (step 8). With
    scripting off, or reduced motion on, everything is simply visible.

**What the hero is made of (ADR-0008):** `HeroSection.vue` lays out the copy beside
`DeveloperRecord.vue`, a code card that draws the same profile as a C# record (so it
cannot disagree with the page), over `CodeBackdrop.vue`, a decorative layer hidden from
assistive technology: a looping video of a request travelling through the owner's
stack (`web/public/media`, built from `tools/hero-video`), plus a few quiet logos.
`site.js` plays the video only on wide screens when motion and data saving allow it;
otherwise the stylesheet shows a still frame. `HeroTags.vue` adds the faint code tags in
the margins. The header's first link shows the "RS" monogram with its crown (one
even-odd path, so the cross is a real hole), accessible name "RS, home". `StatsStrip.vue`
shows checkable facts under the hero, instead of unverifiable percentage bars.

**About Me and the skills cloud (ADR-0010):** `SkillUniverseData` holds the tree of
technologies he has used himself (45, not every skill listed elsewhere) with one honest
caption each. `SkillUniverseLayout.Build` spreads them evenly over three spheres around
.NET (a golden-angle spiral, stars in tree order) once at start-up. The content document
carries each star's place and icon; `AboutSection.vue` draws the paragraphs, every star
(a sphere holding its logo with its name below, with `data-depth`, `data-azimuth`,
`data-elevation` and `data-radius`), the lines and the caption list, all without script
(the cloud stays hidden until script runs). `site.js` step 5 then projects each star to
the screen every frame, floats and slowly spins it, fades the names of the far spheres,
and lets the visitor drag, key or button it around. The five stars of his main stack are
drawn larger.

**The technology logos (ADR-0008):** `LogoSprite.vue` draws each logo once as an SVG
`<symbol>` in a hidden sprite at the top of `<body>` (`SkillIconSprite.vue` does the same
for the cloud's own icons). `TechLogos.vue` then draws faint, slowly drifting silhouettes
in two layers that split the technologies between them, so each logo appears once; the
backend decides which logo goes in which slot (`SiteContentBuilder`).

## Following one claim end to end

Take the BudgetPH "Known gap" sentence:

1. It is written in `src/Portfolio.Api/Content/ProjectData.cs` as `HonestNote`.
2. `ContentTests` checks it against the writing rules.
3. `SiteContentBuilder` puts the project, with its status wording, in the content document.
4. `ProjectsSection.vue` passes it to `ProjectCard.vue`, which draws
   `<p class="honest-note">` because the note is not empty.
5. `PageRenderTests` counts the rendered "honest-note" paragraphs in the generated page
   against the number of projects that have one.
6. Its source is recorded in `docs/CONTENT_SOURCES.md`.

## Where to look when something breaks

| Symptom | Look at |
|---|---|
| A word or claim is wrong | `src/Portfolio.Api/Content/*.cs`, then `docs/CONTENT_SOURCES.md` |
| The page layout or markup is off | the component in `web/app/components/` |
| Layout or color is off | `web/public/css/site.css` tokens at the top |
| Wrong theme on load | `web/public/js/theme-init.js` (runs before paint), then `theme.js` |
| A script, style or image silently does not load | the Content Security Policy in `web/nuxt.config.js` (ADR-0009); the browser console names the blocked resource |
| `content.json` is missing or stale | run `npm run content` in `web/` (the .NET SDK must be installed) |
| The API returns something unexpected | `SiteContentBuilder`, then `ContentApiTests` |
| Blank page after deploy | the failed step in the GitHub Actions run |
| A test fails on writing rules | `ContentTests` says which string and why |
| A test says the site has not been generated | in `web/` run `npm ci && npm run generate` |
