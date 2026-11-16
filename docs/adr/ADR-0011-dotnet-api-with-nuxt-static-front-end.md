# ADR-0011: A .NET API feeding a Nuxt front end that is generated to static files

- **Status:** Accepted. Supersedes the front-end half of ADR-0007 (Razor views); the .NET half stays.
- **Date:** 2026-11-16

## Context

ADR-0007 made the whole site an ASP.NET Core MVC application, exported to static files. The owner
asked whether a portfolio can use React or Vue.js for the front end with .NET as the Web API behind it,
and whether that would still work on github.io. Vue.js is his main front end and his signature stack, so
the answer chosen was Nuxt (Vue.js).

The constraints did not change: GitHub Pages serves static files only (ADR-0001), the page must stay online
for free for years, the Content Security Policy must stay strict (ADR-0009), every claim needs a source
(ADR-0006), and the output page must look and behave exactly as it did.

A tag, `pre-nuxt-rewrite`, marks the last version of the MVC site that was deployed, so this change can be
reverted by checking out that tag (or reverting the merge) and pushing.

## Decision

- **The .NET app becomes a content API** (`src/Portfolio.Api`, renamed from `Portfolio.Web` because it no
  longer serves web pages). It keeps all the content classes and the skills-cloud layout. It builds one JSON
  document (`SiteContent`, by `SiteContentBuilder`) and exposes it two ways with the same builder and
  serializer settings: `GET /api/content`, and `--export-content <file>` which writes it to disk with no server.
- **The front end is Nuxt** (`web/`): Vue.js components (`web/app/components/`) that mirror the old Razor
  views one to one, drawing the content document. `nuxt generate` writes `index.html` and `404.html`.
  The stylesheet and the three page scripts (`theme-init.js`, `theme.js`, `site.js`) are unchanged, and moved
  to `web/public/`.
- **No Nuxt JavaScript reaches the browser** (`features.noScripts`). Nuxt is a build-time template engine
  here. The published page has no hydration, no framework bundle, and the same three scripts as before, so the
  Content Security Policy (`script-src 'self'`) did not change.
- **The deployed site is still only static files.** The .NET API is not deployed. It runs at build time, or
  locally for the live-API endpoint.
- **The tests run against the generated output** (`SiteFactory` serves `web/.output/public`), so what is
  tested is what is published. The 173 earlier tests were kept and now cover the Nuxt output, which is the
  proof that the page is the same. New tests cover the API (`ContentApiTests`) and what Nuxt adds to the output
  (`SiteOutputTests`).
- **One intended visible change:** the footer sentence says "Built with Nuxt and ASP.NET Core on .NET 10"
  instead of "ASP.NET Core MVC", because the old sentence would now be untrue, and the ASP.NET MVC sphere caption
  no longer says the portfolio is built on it.

## Security

Nothing was loosened, and the checks are tested:

- The Content Security Policy string is byte for byte the one from ADR-0009. Nuxt put it after the theme script
  and the stylesheet at first; a meta policy only governs what follows it, and an existing test caught this. It is
  now first in `<head>` (a head priority in `nuxt.config.js`).
- Nuxt adds an inline `speculationrules` script when its JavaScript is off. The policy would block it, and the
  wrong response is to relax the policy, so a small Nitro plugin removes the tag. `SiteOutputTests` fails if it
  or any other inline script, inline handler, preload hint, source map or payload file appears.
- Nuxt also prerenders an empty single-page-app shell to `/404.html` and `/200.html`. The first raced with the real
  not-found page for the same file, and whichever finished last won, so a build could publish a blank 404 page (found
  on 2026-11-16 when four not-found tests failed; earlier green runs were luck). `nitro.prerender.ignore` now skips both
  fallbacks, `appManifest` is off, and `scripts/prune-output.mjs` deletes the `_nuxt` folder (only Nuxt's own error-screen
  styles) after checking no page references it. `SiteOutputTests` fails if `200.html`, `_nuxt` or a wrong `404.html`
  comes back.
- The API sends `X-Content-Type-Options: nosniff`, `Referrer-Policy: no-referrer`,
  `Content-Security-Policy: default-src 'none'; frame-ancestors 'none'`, `Cross-Origin-Resource-Policy:
  same-origin`, hides the `Server` header, registers no cross-origin policy (another site cannot read it), allows
  only GET, and outside development uses HTTPS redirection and HSTS (`ContentApiTests`).
- Supply chain: dependencies are pinned exactly and locked, installed with `npm ci`; npm install scripts are not
  run (the new npm `allowScripts` rule is left on, not approved); every GitHub Action, including the new
  `setup-node`, is pinned to a commit hash; the workflow fails on a known .NET package vulnerability and on a
  moderate or worse npm one.
- The local test server (`web/scripts/serve.mjs`) listens on localhost only, allows GET and HEAD only, refuses
  path traversal and dotfiles, and sends `nosniff`.

## Accepted risk

`npm audit` reports one low-severity finding: `deepmerge` 4.3.1 (prototype poisoning in `mergeObject`), a
build-time helper inside Nuxt that merges this repository's own trusted configuration. No patched release
exists upstream (4.3.1 is the latest). It cannot be reached by a visitor or by untrusted input, and it is
rechecked at every Nuxt update. A moderate `minimatch` advisory is fixed with an `overrides` entry in
`web/package.json` that moves the nested copy to a patched version; remove it when Nuxt's dependencies catch up.

## Consequences

- A second toolchain (Node 24 and npm) joins .NET in the build and in CI, with its own supply chain to keep
  current. Dependabot or a monthly manual update is the upkeep.
- Components are Vue.js, the owner's main front end, so changing the page is now Vue.js work; changing a claim is
  still C# in `Content/`, with its source in `CONTENT_SOURCES.md`.
- The Razor views, controller, and the old static exporter are gone. `StaticExporter` and its tests were replaced
  by `ContentExporter` and the API tests.
- Visual Studio keeps the .NET solution; the front end is edited in `web/`.
- Rollback: check out the `pre-nuxt-rewrite` tag, or revert the merge, and push to `main`.

## Alternatives rejected

- **React with a live .NET API.** The front end could sit on GitHub Pages, but the API needs a second host, a
  cross-origin allowance in the Content Security Policy (`connect-src`), and it can be down or asleep, leaving a
  blank page for a recruiter. Rejected for the same reasons as ADR-0001.
- **Vue.js without Nuxt.** A single-page app that renders in the browser would be weaker for search engines and
  link previews, and would put framework JavaScript in the browser.
- **Nuxt with its normal browser bundle (hydration).** It would rewrite the DOM that `site.js` also manages, add
  scripts the policy must allow, and change the behaviour of a page that had to stay the same.
- **Relaxing the policy to let Nuxt's defaults run** (for example `inline-speculation-rules`). Never.
