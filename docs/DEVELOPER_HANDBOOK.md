# Developer Handbook

Everything needed to run and change the site with no assistant.

## What runs where

| Part | What it is | Where it runs |
|---|---|---|
| Backend (`src/Portfolio.Api`) | .NET 10 app that owns the content and serves it as JSON | On your machine only: for the build, and when you start it to try the API. It is **not** deployed. |
| Front end (`web/`) | Nuxt (Vue.js) app that is generated to static files | At build time only. What is published is plain files. |
| Local site (`npm run serve`) | A small static server for `web/.output/public` | `http://localhost:5099/`, this machine only |

Only the generated files in `web/.output/public` reach GitHub Pages. No server software runs
in production, so there is nothing there to patch or attack.

## Prerequisites

- .NET SDK 10 (see `global.json`). It brings C# 14.
- Node.js 24 (the front end requires `^22.22.3 || ^24.15.0`) and npm.
- Git.

## First run

```bash
cd web
npm ci
npm run generate
npm run serve
```

Open <http://localhost:5099/>. `npm run generate` asks the backend for the content
(`web/data/content.json`, regenerated every time, not committed), then builds the site into
`web/.output/public`.

## Everyday commands

| Command (from `web/` unless stated) | What it does |
|---|---|
| `npm run content` | Asks the .NET backend to write `data/content.json` |
| `npm run generate` | Writes the content, builds the static site into `.output/public`, then prunes files no page loads |
| `npm run serve` | Serves `.output/public` at `http://localhost:5099/` (localhost only; `PORT=` changes it) |
| `npm run dev` | Writes the content, then runs Nuxt's development server with reload on edits |
| `dotnet run --project src/Portfolio.Api` (repo root) | Runs the live API: `GET /api/content` |
| `dotnet test Portfolio.slnx` (repo root) | Runs the content, API and generated-site tests (generate first) |
| `dotnet build Portfolio.slnx` (repo root) | Builds the backend and tests (warnings are errors) |
| `npm audit --audit-level=moderate` | Checks the front-end packages for known vulnerabilities |
| `dotnet list Portfolio.slnx package --vulnerable --include-transitive` (repo root) | Checks the .NET packages |

To stop the local site, press `Ctrl+C` in its terminal. If it was started in the background,
stop only that one process, for example on Windows
`Get-NetTCPConnection -LocalPort 5099 | Select OwningProcess`, then stop that id.

## Changing content

Edit the C# records in `src/Portfolio.Api/Content/`:

- `ProfileData.cs`: name, headline, availability, contact links, resume link.
- `ProjectData.cs`: project cards. Add a `Project`; no component changes needed.
- `ExperienceData.cs`: roles, newest first.
- `SkillData.cs`: grouped skills.
- `SkillUniverseData.cs`: the skills cloud. Only technologies he has used himself (ADR-0010).

Then update `docs/CONTENT_SOURCES.md` with where the new claim comes from, run `npm run generate`
and `dotnet test Portfolio.slnx` (it enforces the writing rules), and check the page.

## Changing the look

Components are in `web/app/components/`, the stylesheet is `web/public/css/site.css`, and the
behaviour scripts are in `web/public/js/`. The page's own scripts are plain JavaScript, loaded
as external files.

## Writing rules the tests enforce

No em dashes; abbreviations written out in full words on first use with the short form in
brackets only if reused; https-only links; unique identifiers; no empty fields.

## Packages and supply chain

- Versions are pinned exactly in `web/package.json` and recorded in `web/package-lock.json`;
  CI installs with `npm ci`, so only locked versions are used.
- npm install scripts are not run unless approved (npm's `allowScripts` rule). Do not approve one
  without a reason written in an ADR. `esbuild` currently shows a notice; the build does not need
  its install script.
- `overrides` in `package.json` lifts one transitive package past a known advisory (`minimatch`).
  Remove an override when the upstream package has moved past it.
- Known and accepted: `deepmerge` (low severity, prototype poisoning in a build-time helper that
  only merges this repository's own configuration; no patched release exists upstream). Recheck at
  every update (ADR-0011).
- GitHub Actions are pinned to full commit hashes (ADR-0009).

## Deployment

Pushing to `main` runs `.github/workflows/deploy.yml`: set up .NET and Node, check both package
sets for known vulnerabilities, `npm ci`, generate the site, test it, publish `web/.output/public`
to GitHub Pages. The repository must be named `randolfsegubre.github.io` and Pages must be set to
the "GitHub Actions" source (Settings, Pages, Build and deployment).

## Troubleshooting

- **A test says the site has not been generated:** in `web/` run `npm ci && npm run generate`, or
  set `PORTFOLIO_SITE_DIR` to a generated folder.
- **`--export-content needs an absolute file path`:** run `npm run content` (it passes an absolute
  path), not `dotnet run` by hand with a relative one.
- **The content file is missing:** `npm run content`. The .NET SDK must be installed.
- **Wrong theme flash:** `theme-init.js` must stay in the head, before the stylesheet, and must not
  be `defer`red.
- **A script or style is blocked in the browser:** the page has a Content Security Policy (ADR-0009).
  Put scripts in external files under `web/public/js/`, never inline or in `onclick=` attributes, and
  do not load anything from another site without a new ADR. Never relax the policy to make something
  work; the tests in `HeadSecurityAndSeoTests` and `SiteOutputTests` exist to stop that.
- **`404.html` is tiny or blank, or `Not pruning: ... reference /_nuxt/` appears:** Nuxt wrote its own fallback over the
  real not-found page, or started to ship files the page needs. Read the "Security" section of ADR-0011, then
  `nitro.prerender` in `web/nuxt.config.js` and `web/scripts/prune-output.mjs`. Do not delete the guard.
- **The generated page has a tag or file nobody chose:** Nuxt adds things by default. Read
  `SiteOutputTests`, then switch the feature off in `web/nuxt.config.js` or remove the tag in
  `web/server/plugins/`.
- **The hero video does not play:** it plays only on screens wider than 42rem, with reduced motion and
  data saving off (then a still frame shows instead), and never on phones. A blocked or silent video
  usually means the `media-src 'self'` directive was lost (ADR-0009) or a file is not under
  `web/public/media`. To change the picture, edit `tools/hero-video/scene.html` and run `npm run build`
  there (see its README); keep each file under 1 MB.
- **Before calling a change done:** run the checklist in `docs/build/06_BEST_PRACTICE_CHECKLIST.md`.
- **A test fails on writing rules:** it names the string and the rule. Write the full words, or add
  the short form in brackets right after them.
- **Blank page after deploy:** check the failed step in the Actions run.
