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

- [x] Best-practice review and fixes (2026-11-13, ADR-0009): Content Security
      Policy, exporter safety guard, per-job workflow permissions and pinned
      actions, canonical link, sitemap and sharing tags, `global.json`,
      `.editorconfig`, stricter analyzers, stale ADR references corrected; the
      standing checklist is `06_BEST_PRACTICE_CHECKLIST.md`.
- [x] Header link renamed "Home"; hero background replaced by a looping video of
      his stack (`tools/hero-video`, ADR-0008 fifth amendment, ADR-0009
      `media-src`), so the hero and the page no longer look like one surface
      (2026-11-14).
- [x] Local test of the Home link and hero video in a real browser (2026-11-14):
      desktop, tablet, phone, reduced motion and the theme toggle verified; one
      bug found and fixed (the "Live site" badge dot kept pulsing under reduced
      motion). Not verified: Safari playback of the MP4, and the live site.
- [x] Portrait converted to WebP for the page, JPEG kept for fallback and link
      previews (2026-11-14).
- [x] Lighthouse pass and phone-width check (2026-11-13): layout shift, small
      decorative text and a stale navigation highlight fixed and pushed; decided
      to keep `connect-src 'none'` (see devlog).
- [x] Résumé download replaced with the 2026-11-13 version (corrected summary),
      same file name so the link never changes.
- [x] Removed "senior" from the title, availability line and page
      descriptions (2026-11-13, at the owner's request).

## Next up (in order)

- [x] Pushed `main`; the workflow succeeded and the live address loads
      (checked over HTTPS and in a real browser, 2026-09-25).
- [ ] Owner reviews the wording in `src/Portfolio.Web/Content/`.
- [ ] Add a link to the live site in the GitHub profile README (a separate
      repository; its stale claims also need fixing, see the devlog).
- [ ] Phase 3: project screenshots, live demo links, resume download, a
      Lighthouse pass.

## Explicitly deferred (tracked so they are never silently forgotten)

- **Pause control for decorative motion.** The page has continuous decorative
  motion (the page code layer, and the hero video on wide screens). Reduced
  motion is respected, but the Web Content Accessibility Guidelines (WCAG)
  success criterion 2.2.2 asks for a way to pause it. A small "pause
  animations" toggle in the header would meet it; not built, owner's call.

- **Resume file refresh (done 2026-11-13, kept as the how-to).** The download (hero button and contact card) serves
  `wwwroot/files/Randolf_Segubre_Resume.pdf`, a copy of his Final résumé from
  2026-10-11. When he corrects the résumé (two lines are flagged in the devlog,
  2026-11-02), replace that one file and push; no code change is needed.
- **Custom domain.** Optional; costs money; not needed to launch.
- **Per-project detail pages.** Cards are enough for now.
- **Galaxy Survivor link.** Its repository is private, so the card carries no
  link by design.
- **A live MVC server.** Not needed; the app runs unchanged with `dotnet run`
  if ever wanted (ADR-0007).

## Open questions

- Resolved 2026-11-13: availability reads "Open to .NET and full-stack roles. Remote preferred, open to hybrid."
- Which two or three projects get real screenshots first (BudgetPH has
  verified ones in its repository).

## How to update this file

At the end of a session: move finished items to Done, add new discoveries to
Next up or Open questions, and append a same-day entry to
`docs/build/05_DEVLOG.md`. Keep this file short; narrative belongs in the
devlog.
