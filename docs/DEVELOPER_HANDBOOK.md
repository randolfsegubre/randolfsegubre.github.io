# Developer Handbook

Everything needed to run and change the site with no assistant.

## Prerequisites

- Node.js 24 or newer (the project was built on 24.18) and npm 11.
- Git.

## First run

```bash
npm ci
npm run dev
```

The development server prints a local address (normally
`http://localhost:5173`). Edits reload automatically.

## Everyday commands

| Command | What it does |
|---|---|
| `npm run dev` | Development server with hot reload |
| `npm test` | Runs the content integrity and render tests once |
| `npm run typecheck` | Type checks without building |
| `npm run build` | Type checks, then builds `dist/` |
| `npm run preview` | Serves the built `dist/` locally, like production |

## Changing content

Edit the typed data in `src/content/`:

- `profile.ts`: name, headline, availability, contact links, resume link.
- `projects.ts`: project cards. Add an entry; no component changes needed.
- `experience.ts`: roles, newest first.
- `skills.ts`: grouped skills.

Then update `docs/CONTENT_SOURCES.md` with where the new claim comes from,
run `npm test` (it enforces the writing rules), and check the page.

## Writing rules the test enforces

No em dashes; abbreviations written out in full words on first use with the
short form in brackets only if reused; https-only links; unique project
identifiers; no empty fields.

## Deployment

Pushing to `main` runs `.github/workflows/deploy.yml`: install, test, build,
publish to GitHub Pages. The repository must be named
`randolfsegubre.github.io` and Pages must be set to the "GitHub Actions"
source (Settings, Pages, Build and deployment).

## Troubleshooting

- **Blank page after deploy:** check the Actions run for a failed test or
  build; confirm `vite.config.ts` still has `base: "/"`.
- **Wrong theme flash:** the inline script in `index.html` must stay before
  the module script.
- **Content test fails on a bare abbreviation:** write the full words, or add
  the short form in brackets right after them.
