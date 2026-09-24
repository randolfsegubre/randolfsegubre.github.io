# Tasks: current status

Live source of truth for where the project stands. Trust it over memory of a
prior session; update it at the end of every session or phase boundary.

## Current phase

**Phase 1 (content model and page sections)** built and verified locally.
**Phase 2 (publish)** is waiting for the owner's approval.

## Done

- [x] Phase 0: Architectural Design Phase artifacts (ADR-0001 to ADR-0006,
      operating manual, build plan, patterns guide, this file, devlog,
      developer handbook) and the Vite scaffold.
- [x] Phase 1: typed content, five sections, theme toggle, content integrity
      test, render test, `WALKTHROUGH.md`, `CONTENT_SOURCES.md`.

## Next up (in order)

- [ ] Owner reviews the content in `src/content/` (wording, project choice,
      contact details, availability line).
- [ ] Phase 2: create the public repository `randolfsegubre.github.io`, set
      Pages source to "GitHub Actions", push `main`, confirm the deploy.
- [ ] Add a link to the live site in the GitHub profile README.
- [ ] Phase 3: project screenshots, live demo links, resume download, a
      Lighthouse pass.

## Explicitly deferred (tracked so they are never silently forgotten)

- **Resume download button.** Hidden until the owner confirms the final
  resume file; set `resumeUrl` in `src/content/profile.ts` and add the file
  under `public/`.
- **Custom domain.** Optional; costs money; not needed to launch.
- **Per-project detail pages.** Cards are enough for now (ADR-0003 notes when
  to revisit).
- **Galaxy Survivor link.** Its repository is private, so the card carries no
  link by design.

## Open questions

- Should the availability line say "remote preferred"? Currently it does.
- Which two or three projects get real screenshots first (BudgetPH has
  verified ones in its repository).

## How to update this file

At the end of a session: move finished items to Done, add new discoveries to
Next up or Open questions, and append a same-day entry to
`docs/build/05_DEVLOG.md`. Keep this file short; narrative belongs in the
devlog.
