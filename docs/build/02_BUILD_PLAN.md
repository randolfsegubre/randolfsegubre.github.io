# Build Plan

Phased plan for the portfolio site. Do not skip ahead: each phase's exit
criteria must hold before the next starts.

| Phase | Name | Touches | Documentation produced |
|---|---|---|---|
| 0 | Architectural Design Phase and scaffold | whole repo | ADRs, operating manual, this plan, patterns guide, tasks, devlog, handbook |
| 1 | Content model and page sections | `src/` | `CONTENT_SOURCES.md`, `WALKTHROUGH.md` |
| 2 | Publish | GitHub repo, Pages settings | devlog entry with the live address |
| 3 | Proof and polish | `Content/`, `wwwroot/` | updated sources, devlog |
| 4 | Maintenance | content | devlog per change |

## Phase 0: Architectural Design Phase and scaffold

- **Entry:** owner approves building the site.
- **Work:** the six artifacts (ADRs, operating manual, patterns guide, tasks
  and devlog, handbook, plus this plan); `git init`; empty-but-buildable
  project (ASP.NET Core MVC after ADR-0007).
- **Exit:** docs exist and agree with each other; `dotnet build` succeeds.

## Phase 1: Content model and page sections

- **Entry:** Phase 0 exit met.
- **Work:** typed content (`Profile`, `Project`, `Role`, `SkillGroup`); the
  sections Hero, Projects, Experience, Skills, Contact; theme toggle; the
  content integrity test and a render test; `WALKTHROUGH.md`;
  `CONTENT_SOURCES.md`.
- **Exit:** `dotnet test` and the static export pass; checked in a real browser
  at desktop and phone width in light and dark; no console errors; keyboard
  navigation works.

## Phase 2: Publish (needs the owner's approval)

- **Entry:** Phase 1 exit met and the owner has reviewed the content.
- **Work:** create the public repository `randolfsegubre.github.io`; set the
  Pages source to "GitHub Actions"; push `main`; confirm the workflow
  succeeds.
- **Exit:** `https://randolfsegubre.github.io` loads and matches the local
  build; the profile README links to it.

## Phase 3: Proof and polish

- **Work:** real screenshots per featured project (the repositories already
  hold verified ones), live demo links where a project has one, the resume
  download once the owner confirms the file, a Lighthouse pass targeting
  performance and accessibility of 95 or more, optional custom domain.
- **Exit:** each featured card has visual proof; scores recorded in the
  devlog.

## Phase 4: Maintenance

- Update the site when a project ships or a job changes; review every claim
  against `CONTENT_SOURCES.md` about once a quarter.
