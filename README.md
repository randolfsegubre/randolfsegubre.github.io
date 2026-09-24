# randolfsegubre.github.io

Randolf Segubre's portfolio website: a single static page with selected
projects, experience, skills and contact details. Built with Vite, React and
TypeScript, and hosted free on GitHub Pages.

- **Working on the code?** Read [`CLAUDE.md`](./CLAUDE.md) first, then
  [`docs/build/01_CLAUDE.md`](./docs/build/01_CLAUDE.md).
- **Want to understand how it works?** Read [`WALKTHROUGH.md`](./WALKTHROUGH.md).
- **Setup and commands:** [`docs/DEVELOPER_HANDBOOK.md`](./docs/DEVELOPER_HANDBOOK.md).

## Run it

```bash
npm ci
npm run dev
```

Other commands: `npm test`, `npm run build`, `npm run preview`.

## What is in the repository

| Path | What it holds |
|---|---|
| `src/content/` | All the site's words as typed data: profile, projects, experience, skills |
| `src/components/` | Small components that render that data |
| `src/hooks/useTheme.ts` | The light and dark theme logic |
| `src/styles.css` | One stylesheet with design tokens |
| `docs/adr/` | Architecture Decision Records: why it is built this way |
| `docs/build/` | Build plan, operating manual, patterns guide, tasks, devlog |
| `docs/CONTENT_SOURCES.md` | Where each claim on the site comes from |
| `.github/workflows/deploy.yml` | Test, build and publish to GitHub Pages on every push to `main` |

## Design rules in one paragraph

Static site, no backend and no third-party requests (ADR-0001, ADR-0004).
Content lives in typed data and a test enforces the writing rules (ADR-0003).
Every claim has a checkable source and gaps are stated plainly (ADR-0006).

## Status

See [`docs/build/04_TASKS.md`](./docs/build/04_TASKS.md).
