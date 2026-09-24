# ADR-0005: Deploy with the official GitHub Actions Pages workflow

- **Status:** Accepted
- **Date:** 2026-09-24

## Context

Vite produces a `dist/` folder that must reach GitHub Pages. Two mechanisms
exist: committing built files to a `gh-pages` branch, or letting GitHub
Actions build and publish an artifact.

## Decision

Use the official Pages actions in `.github/workflows/deploy.yml`. On every
push to `main` the workflow installs dependencies with `npm ci`, runs the
tests, builds, uploads `dist/` as the Pages artifact, and deploys it. A
failing test blocks the deploy. The repository setting for Pages source is
"GitHub Actions".

## Consequences

- No built files in git history, and the deployed site always matches a
  tested commit.
- The first deploy needs the repository to exist, the Pages source set once,
  and a push. That is a public, outward-facing step and is held for the
  owner's approval (see `docs/build/04_TASKS.md`).

## Alternatives rejected

- **`gh-pages` branch with built output:** pollutes history with generated
  files and allows deploying something that was never tested.
