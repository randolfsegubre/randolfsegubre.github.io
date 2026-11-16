# Best-practice checklist

The single checklist for "are we following best practice?" (ADR-0009). It is a
standing gate, not a one-off cleanup: run it before any change is called done,
and again when updating dependencies or the workflow. The `best-practice-check`
skill in `.claude/skills/` runs it. Where this file and general advice differ,
the ADRs decide; where a rule below is broken on purpose, there must be an
ADR or a comment that says why.

Best practice here means: the project's own rules first (the operating manual
and ADRs), then current ASP.NET Core and web practice. When two conflict, name
the conflict and ask the owner; do not pick silently.

## 1. Build and code

- [ ] `dotnet build Portfolio.slnx -warnaserror` is clean: zero warnings, zero
      errors. Nullable and analyzers (`latest-recommended`) stay on; do not
      silence a rule without a comment that names why.
- [ ] `npm run generate` (in `web/`, with SQL Server running) succeeds, then `dotnet test Portfolio.slnx` passes, and new behaviour has a test.
- [ ] The conventions in the operating manual hold: components draw, the API stays
      thin, content in `content/` (JSON), `<summary>` blocks and numbered `STEP N of M` comments.
- [ ] The layers hold (ADR-0013): `LayeringTests` pass; presentation talks only to business,
      business to data, nothing reaches past its neighbour. A table or query is in
      `Portfolio.Data`, a content rule in `Portfolio.Business`, HTTP and commands in `Portfolio.Api`.
- [ ] A schema change is a new migration (never an edited old one), the schema rule is
      tested in `SqlContentTests`, and `ContentMapper` and its round-trip test still agree.
- [ ] Every ADR cited in a code comment is the current one (ADR-0007 for the
      Nuxt and API shape, ADR-0011, not the superseded ADR-0002).
- [ ] No dead code, no unused files, no secrets, no machine paths.

## 2. Security

- [ ] The Content Security Policy is intact: no inline `<script>`, no inline
      event handlers, no third-party script, style, font or image, no
      `eval`. A new origin needs a new ADR (ADR-0009).
- [ ] Outbound links go through the `ExternalLink` component (`rel="noopener noreferrer"`).
- [ ] `v-html` is used only on constants the site owns (logo markup), never on input.
- [ ] Nothing was loosened to make a tool work: no weaker policy, no approved install
      script, no `--force` on npm, no skipped or deleted test. `SiteOutputTests` and
      `HeadSecurityAndSeoTests` pass (policy first in `<head>`, only the three same-origin
      scripts, no Nuxt hints).
- [ ] `npm audit --audit-level=moderate` reports nothing; the accepted low finding in
      ADR-0011 is rechecked.
- [ ] `dotnet list Portfolio.slnx package --vulnerable --include-transitive`
      reports nothing.
- [ ] The API still sends its security headers and no cross-origin policy
      (`ContentApiTests`), and the export still refuses a relative path.
- [ ] No connection string, password or token is in the repository, the workflow log or a test.
      The app needs `ConnectionStrings__Portfolio` from the environment; local development uses
      Windows authentication. All database access goes through Entity Framework Core (no
      hand-built SQL), and `TrustServerCertificate=True` is only ever used for a database on the
      same machine (ADR-0012).

## 3. Workflow and supply chain

- [ ] Workflow permissions are per job and minimal; top level is `{}`.
- [ ] The SQL Server container image is pinned by digest, bound to the runner's loopback
      address, with a fresh masked password per run, and `deploy` runs only on `main`.
- [ ] Every third-party action is pinned to a full commit hash with its tag in
      a comment. Resolve with
      `gh api repos/<owner>/<action>/git/ref/tags/<tag>`.
- [ ] `global.json` pins the SDK feature band; the workflow reads it. npm packages are
      pinned exactly and installed with `npm ci` from `web/package-lock.json`.
- [ ] Tests gate the deploy; a failed export publishes nothing.

## 4. Accessibility and quality of the page

- [ ] One `<h1>`, landmarks, skip link, visible focus, alt text, sufficient
      contrast in both themes, keyboard operability.
- [ ] `prefers-reduced-motion` is respected for any new motion. A video never
      autoplays under reduced motion or data saving, is muted, decorative and
      under 1 MB per file, and is not fetched on phones (ADR-0008).
- [ ] Checked in a real browser at phone and desktop width, light and dark,
      with the console empty (no policy violations).

## 5. Search and sharing

- [ ] Canonical link on indexable pages; `noindex` on the not-found page.
- [ ] Title, description, Open Graph and Twitter tags are present and say what
      the page says; the preview image is an absolute address with its size.
- [ ] `sitemap.xml` and `robots.txt` use the address in `content/site.json`.

## 6. Documentation and honesty

- [ ] Every new claim has a row in `docs/CONTENT_SOURCES.md` (ADR-0006).
- [ ] Writing style holds: first person, no em dashes, Full Word Format.
- [ ] `WALKTHROUGH.md`, the handbook and `README.md` match the code.
- [ ] A change to a design, layout, data shape or behaviour that an ADR describes
      updates that ADR in the same turn: an amendment that says what changed and
      why, and a fix to any earlier sentence it makes false (an amendment never
      leaves a contradiction behind). A new decision gets a new ADR and a row in
      the table in `docs/build/01_CLAUDE.md`.
- [ ] A new or changed image, icon or logo has its source and licence in
      `docs/THIRD_PARTY_NOTICES.md`, and says so plainly when it is an
      approximation and not the official artwork.
- [ ] The skills cloud shows only technologies the owner has used himself (ADR-0010,
      fifth amendment). Adding a sphere needs a caption, a row in
      `docs/CONTENT_SOURCES.md` and the owner's confirmation that he used it; a
      practice or technique is not a sphere. Check his memory and repositories first, and
      remember that a package in a project built with AI help is not his experience.
- [ ] `04_TASKS.md` updated and a dated entry appended to `05_DEVLOG.md`.
- [ ] All of the above happen in the same turn as the code change, never as a
      later "docs pass".
- [ ] Nothing is committed, pushed or published without the owner's explicit
      yes (operating manual, "Do not publish without approval").

## Dependencies

Review `dotnet list Portfolio.slnx package --outdated` when touching packages.
Take minor and patch updates that keep the tests green. A major version that
targets a newer .NET than the project (for example a package built for .NET
11 while the site is on .NET 10) is not an update, it is a migration; leave it
until the project moves.
