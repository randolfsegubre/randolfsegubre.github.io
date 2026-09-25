# Tasks: current status

Live source of truth for where the project stands. Trust it over memory of a
prior session; update it at the end of every session or phase boundary.

## Current phase

**Phase 2 (publish)** done on 2026-09-25: the site is live at
`https://randolfsegubre.github.io` (approved by the owner so the link can go in
an email to Ben). Next is his content review, then Phase 3.

## Done

- [x] Phase 0: Architectural Design Phase artifacts (ADR-0001 to ADR-0007,
      operating manual, build plan, patterns guide, this file, devlog,
      developer handbook).
- [x] Phase 1: content, five sections, theme toggle, tests, walkthrough,
      content sources. First built in React and TypeScript (2026-09-24), then
      rebuilt in ASP.NET Core MVC on .NET 10 with a static export
      (2026-09-25, ADR-0007) at the owner's request.
- [x] Public repository `randolfsegubre/randolfsegubre.github.io` created and
      Pages source set to "GitHub Actions" (2026-09-25).

## Next up (in order)

- [x] Pushed `main`; the workflow succeeded and the live address loads
      (checked over HTTPS and in a real browser, 2026-09-25).
- [ ] Owner reviews the wording in `src/Portfolio.Web/Content/`.
- [ ] Add a link to the live site in the GitHub profile README (a separate
      repository; its stale claims also need fixing, see the devlog).
- [ ] Phase 3: project screenshots, live demo links, resume download, a
      Lighthouse pass.

## Explicitly deferred (tracked so they are never silently forgotten)

- **Resume download button.** Hidden until the owner confirms the final
  resume file; set `ResumeUrl` in `Content/ProfileData.cs` and add the file
  under `wwwroot/`.
- **Custom domain.** Optional; costs money; not needed to launch.
- **Per-project detail pages.** Cards are enough for now.
- **Galaxy Survivor link.** Its repository is private, so the card carries no
  link by design.
- **A live MVC server.** Not needed; the app runs unchanged with `dotnet run`
  if ever wanted (ADR-0007).

## Open questions

- Should the availability line say "remote preferred"? Currently it does.
- Which two or three projects get real screenshots first (BudgetPH has
  verified ones in its repository).

## How to update this file

At the end of a session: move finished items to Done, add new discoveries to
Next up or Open questions, and append a same-day entry to
`docs/build/05_DEVLOG.md`. Keep this file short; narrative belongs in the
devlog.
