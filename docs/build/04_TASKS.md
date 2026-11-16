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
- [x] New design built locally, NOT deployed (2026-11-14, ADR-0010): skills star map
      with .NET at the centre (data, layout function and tests), RS monogram
      home link, "Hi, I'm" greeting, margin code tags, letter-by-letter
      headings and the page-change veil with a footer off switch. Background,
      hero video and portrait unchanged. Tests 167 passing; checked in a real
      browser at desktop and phone width, light and dark, and with reduced motion.
      Not verified: real touch devices, Safari, the live site.
- [x] Skills merged into a new "About me" section and the map rebuilt as a floating,
      rotatable 3D solar system (2026-11-14, ADR-0010 amendment): drag, arrow
      keys, Turn and Pause buttons, spin paused on hover and focus, reduced
      motion respected. Tests 170 passing; checked in a real browser (desktop,
      phone width, reduced motion, drag, click-to-front, pause). Local only.
- [x] A .NET API feeding a Nuxt front end (2026-11-16, ADR-0011), on branch `nuxt-rewrite`, NOT merged or
      deployed: the .NET app renamed `Portfolio.Api` and reduced to the content API (`GET /api/content`,
      `--export-content`, security headers); a Nuxt app in `web/` mirrors the Razor views as Vue components, with no
      Nuxt JavaScript in the browser; assets moved to `web/public`; tests (179) now run against the generated
      output; workflow updated (setup-node pinned, `npm ci`, both audits). The page matches the old one element
      for element except the footer sentence. Rollback: tag `pre-nuxt-rewrite`. Open: owner to test locally, then
      say whether to commit, merge and deploy.
- [x] JavaScript joined to React and AngularJS, Nuxt.js added (2026-11-16, ADR-0010 seventh
      amendment): two cross links, one sphere (45 now) with a generic caption added at his instruction.
      Tests 173 passing; real browser. Local only.
- [x] Main stack drawn larger (2026-11-15, ADR-0010 sixth amendment): the five signature-stack
      spheres about 79 pixels, every other about 41 to 54, .NET 102; kept in step with the profile by
      a test. Tests 173 passing; real browser, desktop. Local only.
- [x] Only what he has used, a less crowded cloud, "About Me" and a crowned monogram
      (2026-11-15, ADR-0010 fifth amendment): 44 spheres checked against his memory, the
      dotfiles and GitHub; Azure Service Bus, TanStack Query, TypeScript, practices and other
      non-experience removed (a test names them); BudgetPH MediatR chip and the Scoot
      "designed" and "achieved 90 percent" wording corrected; Azure DevOps added; header link
      and section heading read "About Me"; RS carries a gold crown with the cross cut out.
      Tests 172 passing; real browser desktop and phone. Local only. Same day, at his yes:
      GraphQL, Vite, Semantic Kernel and SQLite removed from the project cards (SQLite also from the
      grouped list). Open: Nuxt.js (see the devlog).
- [x] Every skill and stack entry on the map, a real AngularJS logo, a documentation gate
      (2026-11-15, ADR-0010 fourth amendment): 68 spheres on three shells, no flat
      fallback (the caption list instead), far names fade, wider layout, 18 more CC0
      logos and 17 glyphs; the checklist and operating manual now say documentation is
      done in the same turn. Tests 171 passing; checked in a real browser (desktop and
      phone width). Local only. Open: owner to review the new captions in
      `CONTENT_SOURCES.md` (several marked "generic") and say whether 68 spheres is too
      dense on a phone.
- [x] Bigger spheres, an even arrangement and a purple "About me" band (2026-11-14,
      ADR-0010 third amendment): stars spread evenly over two spheres, the cloud a
      700-pixel square beside the text on a hero-purple band, the grouped skills list in
      its own section. Tests 172 passing; checked in a real browser (desktop and phone
      width). Local only.
- [x] Skills map rebuilt as a 3D cloud of logo spheres beside the About text, after
      the reference animation (2026-11-14, ADR-0010 second amendment): glossy
      spheres with logos and names, .NET as a large golden centre, no pills, lines
      only for the chosen sphere, square canvas that fits phones, touch swipe to
      turn. Tests 172 passing; checked in a real browser at desktop and phone width.
      Local only.
- [x] Hero button grid fixed (2026-11-14): full-width primary, pairs below, one
      column under 34 rem; Docker box in the hero video moved clear of the buttons.
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
- [ ] Owner reviews the wording in `src/Portfolio.Api/Content/`, including the new star-map captions (`SkillUniverseData.cs`).
- [ ] Owner decides whether to deploy the new design, and whether to add the optional left navigation rail (ADR-0010, alternatives).
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
  `web/public/files/Randolf_Segubre_Resume.pdf`, a copy of his Final résumé from
  2026-10-11. When he corrects the résumé (two lines are flagged in the devlog,
  2026-11-02), replace that one file and push; no code change is needed.
- **Custom domain.** Optional; costs money; not needed to launch.
- **Per-project detail pages.** Cards are enough for now.
- **Galaxy Survivor link.** Its repository is private, so the card carries no
  link by design.
- **A live API server.** Not needed; the .NET API runs unchanged with
  `dotnet run --project src/Portfolio.Api` if ever wanted (ADR-0011).

## Open questions

- Resolved 2026-11-13: availability reads "Open to .NET and full-stack roles. Remote preferred, open to hybrid."
- Which two or three projects get real screenshots first (BudgetPH has
  verified ones in its repository).

## How to update this file

At the end of a session: move finished items to Done, add new discoveries to
Next up or Open questions, and append a same-day entry to
`docs/build/05_DEVLOG.md`. Keep this file short; narrative belongs in the
devlog.
