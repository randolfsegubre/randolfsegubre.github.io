# Developer Handbook

Everything needed to run and change the site with no assistant.

## What runs where

| Part | What it is | Where it runs |
|---|---|---|
| Content (`content/*.json`) | The site's words and drawings as JSON data files: the source of truth | In the repository. Read by the backend; never served directly. |
| Database (SQL Server, schema `content`) | Holds the content for the length of a build and for local runs | Your local SQL Server (database `PortfolioSite`), or a throwaway container in the deploy workflow. **Not** in production. |
| Backend, three layers (`src/Portfolio.Api`, `Portfolio.Business`, `Portfolio.Data`) | .NET 10 app: presentation (endpoint, build commands), business (models, rules, document builder, services), data access (schema, migrations, repository) | On your machine only: for the build, and when you start it to try the API. It is **not** deployed. |
| Front end (`web/`) | Nuxt (Vue.js) app that is generated to static files | At build time only. What is published is plain files. |
| Local site (`npm run serve`) | A small static server for `web/.output/public` | `http://localhost:5099/`, this machine only |

Only the generated files in `web/.output/public` reach GitHub Pages. No server software runs
in production, so there is nothing there to patch or attack.

## Prerequisites

- .NET SDK 10 (see `global.json`). It brings C# 14. For migrations, the Entity Framework tool: `dotnet tool install -g dotnet-ef`.
- Node.js 24 (the front end requires `^22.22.3 || ^24.15.0`) and npm.
- SQL Server (the Developer edition is enough), running on `localhost`. The build signs in with Windows
  authentication, so no password is involved, and it creates the database `PortfolioSite` itself. The login
  needs permission to create a database.
- Git.

## First run

```bash
cd web
npm ci
npm run generate
npm run serve
```

Open <http://localhost:5099/>. `npm run generate` runs one backend command that creates or updates the
database, makes it say what `content/*.json` say, and reads the content back out into
`web/data/content.json` (regenerated every time, not committed). Then Nuxt builds the site into
`web/.output/public`.

## Everyday commands

| Command (from `web/` unless stated) | What it does |
|---|---|
| `npm run content` | Migrates and seeds the database from `content/*.json`, then writes `data/content.json` from it |
| `npm run generate` | Writes the content, builds the static site into `.output/public`, then prunes files no page loads |
| `npm run serve` | Serves `.output/public` at `http://localhost:5099/` (localhost only; `PORT=` changes it) |
| `npm run dev` | Writes the content, then runs Nuxt's development server with reload on edits |
| `dotnet run --project src/Portfolio.Api` (repo root) | Runs the live API: `GET /api/content`. Needs `ConnectionStrings__Portfolio` set and the database seeded (it stops with a clear message if empty). |
| `dotnet run --project src/Portfolio.Api --no-launch-profile -- --migrate --seed <absolute folder> --export-content <absolute file>` | The build commands, which can be combined and always run in that order |
| `dotnet ef migrations add <Name> --project src/Portfolio.Data --output-dir Migrations` (repo root) | A schema change: creates a new migration. Never edit an old one. |
| `dotnet test Portfolio.slnx` (repo root) | Runs the content, layering, API, real-SQL-Server and generated-site tests (generate first) |
| `dotnet build Portfolio.slnx` (repo root) | Builds the backend and tests (warnings are errors) |
| `npm audit --audit-level=moderate` | Checks the front-end packages for known vulnerabilities |
| `dotnet list Portfolio.slnx package --vulnerable --include-transitive` (repo root) | Checks the .NET packages |

To stop the local site, press `Ctrl+C` in its terminal. If it was started in the background,
stop only that one process, for example on Windows
`Get-NetTCPConnection -LocalPort 5099 | Select OwningProcess`, then stop that id.

## Changing content

Edit the JSON files in `content/` (`content/README.md` explains each one and the rules that come with it):

- `profile.json`: name, headline, availability, contact links, resume link.
- `projects.json`: project cards. Add an object; no component changes needed.
- `experience.json`: roles, newest first.
- `skills.json`: grouped skills.
- `skill-universe.json`: the skills cloud. Only technologies he has used himself (ADR-0010).
- `site.json`, `live-sites.json`, `hero-tags.json`, `code-streams.json`, `tech-logos.json`, `skill-icons.json`,
  `social-icons.json`: the rest.

Then update `docs/CONTENT_SOURCES.md` with where the new claim comes from, run `npm run generate`
(it reloads the database from the files) and `dotnet test Portfolio.slnx` (it enforces the writing rules),
and check the page. The files are the source of truth: a row edited directly in the database is overwritten
by the next build (ADR-0012).

## Changing the database

Only when the shape of the data changes (a new kind of content, a new column). Edit the row classes in
`src/Portfolio.Data/Rows.cs` and the schema in `PortfolioDbContext.cs`, add a migration, then teach
`ContentMapper` in the business layer to translate it. `SqlContentTests` and the mapper tests will tell you
if anything was missed. The layers are in ADR-0013: a table or a query goes in the data layer, a content rule
in the business layer, and the presentation layer never sees either.

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
- GitHub Actions are pinned to full commit hashes, and the SQL Server container image to a digest (ADR-0009,
  ADR-0012).
- .NET packages are pinned to exact versions. Entity Framework Core is on the 10.0.x line to match the .NET 10 runtime;
  the 11.x packages target net11 and do not install here. `dotnet list package --vulnerable` gates the build.

## Deployment

Pushing to `main` runs `.github/workflows/deploy.yml`: set up .NET and Node, check both package
sets for known vulnerabilities, `npm ci`, start a throwaway SQL Server container (loopback only, a fresh
random password, image pinned by digest), generate the site through it, test the database path and the
site, publish `web/.output/public` to GitHub Pages. A manual run of the workflow from another branch
builds and tests but never deploys. The repository must be named `randolfsegubre.github.io` and Pages must be set to
the "GitHub Actions" source (Settings, Pages, Build and deployment).

## Troubleshooting

- **A test says the site has not been generated:** in `web/` run `npm ci && npm run generate`, or
  set `PORTFOLIO_SITE_DIR` to a generated folder.
- **`--export-content needs an absolute file path`:** run `npm run content` (it passes an absolute
  path), not `dotnet run` by hand with a relative one.
- **The content file is missing:** `npm run content`. The .NET SDK must be installed and SQL Server running.
- **`Cannot open server` or a login failure:** SQL Server is not running, or your Windows login may not create
  databases. Start the service `MSSQLSERVER`. On another machine, set `ConnectionStrings__Portfolio`
  (the connection string is never kept in the repository).
- **`The content database is empty`:** it was never seeded. Run `npm run content`.
- **`The content file 'x.json' is not valid`:** the message names the file and the problem. Reading is strict:
  a misspelt field, a `null` where a value is required, or invalid JSON stops the build.
- **A database test fails to connect:** the real-SQL-Server tests use your local server with Windows authentication,
  or the server in `PORTFOLIO_TEST_SQL` (a connection string without a database name). They are never skipped.
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
