# Devlog

Reverse-chronological. One entry per session or phase boundary: what was
asked, what changed and why, how it was verified, what is next. Full
narrative belongs here; `docs/build/04_TASKS.md` stays a short current
snapshot.

---

## 2026-09-25: Rebuilt in ASP.NET Core MVC on .NET 10 and published

**Asked:** Randolf asked to publish the site so the link can go in an email to
Ben. Mid-publish he asked which stack it used and whether it could use his
main stack. He chose "C#, ASP.NET MVC, with the latest .NET and C# version".
Nothing had been pushed yet, so the switch cost no public history.

**What changed:**
- Replaced the React and TypeScript front end with an **ASP.NET Core MVC**
  application on **.NET 10 (C# 14)**: `HomeController`, Razor layout, page and
  partial views, C# record content behind `IPortfolioContent`, dependency
  injection, an xUnit test project. New decision record ADR-0007 supersedes
  ADR-0002; ADR-0001, 0003, 0004 and 0005 were updated to match.
- GitHub Pages cannot run a server, so the app exports itself at build time:
  `dotnet run -- --export "$PWD/dist"` starts Kestrel on a free port, saves
  `/` as `index.html` and `/not-found` as `404.html`, copies `wwwroot`, and
  exits (`StaticExporter`). The deployed site is that static output. I did
  not silently narrow "MVC" to "static": the source is real MVC and the live
  site is its export, stated in ADR-0007 with the rejected alternatives.
- Kept the content, wording, styles, and documentation standard unchanged;
  the writing-rule test was ported to xUnit and extended with rendered-HTML
  and exporter tests.
- Rewrote the deploy workflow for .NET (setup, test, export, upload, deploy).
- Created the public repository and set Pages to the "GitHub Actions" source
  before the first push, so the first workflow run could deploy.

**Verified:**
- `dotnet build` (warnings as errors) and `dotnet test`: 18 tests pass
  (content rules, rendered HTML through the real MVC pipeline, exporter).
- The export produces `index.html` (about 30 KB), `404.html`, stylesheet,
  script, favicon, `robots.txt` and `.nojekyll`; a relative export path is
  rejected with a clear message.
- Served the exported files like Pages would and checked in a real browser at
  375 pixels wide: all five sections, 12 cards and roles, no failed or
  third-party requests, no horizontal overflow, and a real click on the theme
  toggle switches the theme, stores the choice, and updates the label.

**Caught along the way:** Razor rejects nested quotes inside tag-helper
attributes (moved those expressions into code blocks); `dotnet run` starts in
the project folder so a relative export path landed in the wrong place (now
rejected, absolute path used in CI); my throwaway test server compared
forward-slash and backslash paths and served the 404 page for everything
(fixed the script, not the site).

**Published:** pushed `main`; the GitHub Actions run passed (test, export,
deploy) and `https://randolfsegubre.github.io` returned 200 for the page and
every asset, with a real 404 page for unknown addresses. Checked in a browser
against the live address: five sections, 12 cards and roles, theme toggle
revealed, no failed or third-party requests.

**Next:** the owner reviews wording; then link the site from the GitHub
profile README. See `docs/build/04_TASKS.md`.

---

## 2026-09-24: Architectural Design Phase, scaffold and Phase 1 built locally

*(First build, in React and TypeScript, replaced on 2026-09-25 by ADR-0007.
Kept as history.)*

**Asked:** Randolf asked whether a GitHub Pages portfolio needs payment (it
does not), then to scaffold the website portfolio and "do our standard
process", meaning the Architectural Design Phase.

**What changed:**
- Created the repository at `D:\_DEV\Personal_Projects\randolfsegubre.github.io`
  (single-repo shape, like Ophir and Grace at Work: root `CLAUDE.md` pointer,
  operating docs under `docs/build/`).
- Produced the six Architectural Design Phase artifacts before application
  code: ADR-0001 to ADR-0006, `docs/build/01_CLAUDE.md`, `02_BUILD_PLAN.md`,
  `03_ARCHITECTURE_AND_PATTERNS_GUIDE.md`, this file and `04_TASKS.md`, and
  `docs/DEVELOPER_HANDBOOK.md`.
- Built Phase 1: typed content modules, five sections, theme toggle, a
  content integrity and writing-rule test, a render test, `WALKTHROUGH.md`
  and `docs/CONTENT_SOURCES.md`, all following the in-code documentation
  standard (TSDoc summaries, numbered STEP comments, a walkthrough guide).
- Chose Vite, React and TypeScript (ADR-0002) after weighing plain HTML,
  Astro, Next.js static export and Blazor WebAssembly; the last would load
  several megabytes before a recruiter sees anything.
- Wrote the deploy workflow but did not publish: creating the public
  repository and pushing are outward-facing and wait for approval.

**Decisions on content (ADR-0006):** the site names only public repositories,
states status and known gaps plainly, uses "Solo personal project" for role
wording, and leaves off a private repository's link, confidential client work
and an e-commerce demo whose front end was never connected.

**Verified:**
- `npm run typecheck`, `npm test` (13 tests) and `npm run build` pass; the
  production bundle is about 76 kilobytes of JavaScript compressed.
- Checked in a real browser: no console errors, zero third-party requests,
  one `h1`, all outbound links carry `noopener`, no horizontal overflow at 375
  pixels wide, and the theme toggle works with a real click and persists.
- Measured text contrast for every color pair in both themes: all pass the
  4.5 to 1 minimum (lowest measured 4.80 to 1, for the focus ring).

**Caught and fixed along the way:**
- The content test found four prose lines with bare abbreviations (JSON, API,
  JWT, and ASP stranded by an ordering bug in the allow-list); fixed the
  copy and reordered the allow-list longest first.
- A test query for the page banner also matched card `<header>` elements;
  tightened the test.
- At phone width the sticky header wrapped to three rows; restructured it to
  two rows and made it non-sticky on very short viewports.

**Next:** Randolf reviews the content; on approval, create the public
repository, set Pages to GitHub Actions and push (Phase 2). See
`docs/build/04_TASKS.md`.
