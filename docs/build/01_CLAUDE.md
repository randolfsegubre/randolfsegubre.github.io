# Portfolio Site: AI Operating Manual

This is the constitution for any Artificial Intelligence (AI) agent or human
working in this repository. Read it fully before changing code or content.
The root `CLAUDE.md` auto-loads and points here.

## What this project is

Randolf Segubre's personal portfolio website: a single static page (hero,
selected projects, experience, skills, contact) built with Vite, React and
TypeScript, hosted free on GitHub Pages at
`https://randolfsegubre.github.io`. Its job is to give a recruiter or hiring
manager, within sixty seconds, an honest picture of what he has built and
what he can do. It supports his job search but is not a substitute for the
resume.

The sentence that overrides every design temptation: **the site says only
what can be checked** (ADR-0006). A modest true claim beats an impressive
doubtful one, every time.

## Document map: read in this order for a new session

1. `README.md`: what the site is and how to run it.
2. **This file.**
3. `docs/build/04_TASKS.md`: current phase and status. Trust it over any
   memory of a prior session.
4. Top of `docs/build/05_DEVLOG.md`: recent history and why.
5. `docs/build/02_BUILD_PLAN.md`: phases with entry and exit criteria.
6. `docs/build/03_ARCHITECTURE_AND_PATTERNS_GUIDE.md`: the patterns in use.
7. `docs/adr/`: one file per structural decision. Check before changing
   structure.
8. `docs/CONTENT_SOURCES.md`: where each claim on the site comes from.
9. `docs/DEVELOPER_HANDBOOK.md`: setup and everyday commands.
10. `WALKTHROUGH.md`: how a page load flows through the code.

## First-session checklist

- [ ] Read this file, `04_TASKS.md`, and the last two `05_DEVLOG.md` entries.
- [ ] Run `git status` and `git log --oneline -10`; confirm the tree matches
      what the docs claim before trusting either.
- [ ] Run `npm ci`, `npm test`, `npm run build`; all must pass before you
      change anything.
- [ ] Check `docs/adr/` before any structural decision.

## Load-bearing decisions (pointers, not repeats)

| Decision | ADR |
|---|---|
| Static site on GitHub Pages, no backend | [ADR-0001](../adr/ADR-0001-static-site-on-github-pages.md) |
| Vite, React, TypeScript (strict) | [ADR-0002](../adr/ADR-0002-vite-react-typescript.md) |
| Content in typed data modules, enforced by a test | [ADR-0003](../adr/ADR-0003-typed-content-modules.md) |
| Plain CSS design tokens, light and dark | [ADR-0004](../adr/ADR-0004-plain-css-design-tokens.md) |
| Deploy with the official GitHub Actions Pages workflow | [ADR-0005](../adr/ADR-0005-deploy-with-github-actions.md) |
| Content honesty and safety policy | [ADR-0006](../adr/ADR-0006-content-honesty-and-safety-policy.md) |

A new significant decision (a new dependency, a reversal) gets a new
`ADR-000N` the moment it is made, and a row in this table.

## Non-negotiable rules

- **Content rules (ADR-0006).** Every claim needs an entry in
  `docs/CONTENT_SOURCES.md`. State project status plainly and name known
  gaps. Say "contributed to" unless "led" is documented truth. Never add
  confidential material, internal system names or ticket numbers. Never link
  a private repository. Never include confidential client work.
- **No secrets, ever.** The site needs none. If a change seems to need one,
  stop and ask.
- **No AI-attribution trailers.** Do not add `Co-Authored-By` lines or
  "Generated with" footers to commits or pull requests. The owner wants the
  history to read as his own reviewed work.
- **Writing style.** First person where the site speaks as him, no em
  dashes, and Full Word Format (FWF): abbreviations are written out in full
  words on first use, for example Content Security Policy. The tag chips that
  list product names are exempt. `content.test.ts` enforces this on the
  content.
- **Do not publish without approval.** Creating the public repository,
  pushing, and turning on Pages are outward-facing steps that need the
  owner's explicit yes each time.
- **Accessibility is a requirement, not polish.** Semantic landmarks, a skip
  link, visible focus, sufficient contrast in both themes, keyboard
  operability, and `prefers-reduced-motion` respected.

## Engineering conventions

- **TypeScript strict mode.** No `any`.
- **In-code documentation is required.** This is the owner's standing
  preference for all his personal projects and deliberately overrides the
  general "no comments" default:
  1. A TSDoc `/** ... */` summary block above every component, hook and
     non-trivial function: what it is, the role or pattern it plays, and
     why it exists.
  2. Numbered `// STEP N of M: ...` comments inside any multi-step function
     body, in the order the steps execute.
  3. `WALKTHROUGH.md` at the repo root, kept current whenever the page-load
     flow changes.
  Trivial one-line helpers do not need a block.
- **Components render, data lives in `src/content`.** No claim text in JSX.
- **Small components, one job each**, named for what they show.
- **Tests match the phase.** Content integrity and render tests exist now.
  Do not add heavy infrastructure ahead of need.

## Working with Randolf on this project

- Update `04_TASKS.md` and append to `05_DEVLOG.md` at the end of every
  session or phase boundary.
- Surface any request that would reverse an ADR before following it.
- Check `gh pr list` before branching; add commits to an open relevant pull
  request instead of opening a parallel one. Trunk-based, small changes.
- Prefer to verify in a real browser (desktop and phone width, light and
  dark) rather than reasoning about the layout.
