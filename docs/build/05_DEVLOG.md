# Devlog

Reverse-chronological. One entry per session or phase boundary: what was
asked, what changed and why, how it was verified, what is next. Full
narrative belongs here; `docs/build/04_TASKS.md` stays a short current
snapshot.

---

## 2026-09-24: Architectural Design Phase, scaffold and Phase 1 built locally

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
