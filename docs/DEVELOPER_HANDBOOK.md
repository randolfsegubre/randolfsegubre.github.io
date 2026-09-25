# Developer Handbook

Everything needed to run and change the site with no assistant.

## Prerequisites

- .NET SDK 10 (built on 10.0.400). It brings C# 14.
- Git.

## First run

```bash
dotnet run --project src/Portfolio.Web
```

The console prints a local address (normally `http://localhost:5000`). Open
it. Razor views need a restart of `dotnet run` (or `dotnet watch`) to show
edits; use `dotnet watch --project src/Portfolio.Web` for reloading.

## Everyday commands

| Command | What it does |
|---|---|
| `dotnet run --project src/Portfolio.Web` | Runs the MVC site locally |
| `dotnet watch --project src/Portfolio.Web` | Same, with reload on edits |
| `dotnet test Portfolio.slnx` | Runs the content, render and exporter tests |
| `dotnet build Portfolio.slnx` | Builds everything (warnings are errors) |
| `dotnet run --project src/Portfolio.Web -c Release --no-launch-profile -- --export "$PWD/dist"` | Exports the static site to `dist/`, exactly what gets deployed |

Run the export from the repository root and use an **absolute** path (the
`"$PWD/dist"` part). `dotnet run` starts inside the project folder, so a
relative path would land in the wrong place; the tool refuses relative paths
with a clear message.

To look at the exported site, serve `dist/` with any static file server
(for example `npx http-server dist`) and open it.

## Changing content

Edit the C# records in `src/Portfolio.Web/Content/`:

- `ProfileData.cs`: name, headline, availability, contact links, resume link.
- `ProjectData.cs`: project cards. Add a `Project`; no view changes needed.
- `ExperienceData.cs`: roles, newest first.
- `SkillData.cs`: grouped skills.

Then update `docs/CONTENT_SOURCES.md` with where the new claim comes from, run
`dotnet test Portfolio.slnx` (it enforces the writing rules), and check the
page.

## Writing rules the tests enforce

No em dashes; abbreviations written out in full words on first use with the
short form in brackets only if reused; https-only links; unique identifiers; no
empty fields.

## Deployment

Pushing to `main` runs `.github/workflows/deploy.yml`: set up .NET 10, test,
export, publish to GitHub Pages. The repository must be named
`randolfsegubre.github.io` and Pages must be set to the "GitHub Actions"
source (Settings, Pages, Build and deployment).

## Troubleshooting

- **The export fails with a status code:** a route returned something other
  than 200. Run the app and open that route to see the error.
- **`--export needs an absolute folder path`:** pass `"$PWD/dist"`, not `dist`.
- **Wrong theme flash:** the inline script in `_Layout.cshtml` must stay in
  the head, before the stylesheet.
- **A test fails on writing rules:** it names the string and the rule. Write
  the full words, or add the short form in brackets right after them.
- **Blank page after deploy:** check the failed step in the Actions run.
