# ADR-0005: Deploy with the official GitHub Actions Pages workflow

- **Status:** Accepted
- **Date:** 2026-09-24

## Context

The static export (ADR-0007) produces a `dist/` folder that must reach GitHub
Pages. Two mechanisms exist: committing built files to a `gh-pages` branch, or
letting GitHub Actions build and publish an artifact.

## Decision

Use the official Pages actions in `.github/workflows/deploy.yml`. On every
push to `main` the workflow sets up .NET 10, runs `dotnet test`, runs the
export (`dotnet run --project src/Portfolio.Web -- --export "$GITHUB_WORKSPACE/dist"`),
uploads `dist/` as the Pages artifact, and deploys it. A failing test blocks
the deploy. The repository setting for Pages source is "GitHub Actions".

## Consequences

- No built files in git history, and the deployed site always matches a
  tested commit.
- The first deploy needs the repository to exist, the Pages source set once,
  and a push. That is a public, outward-facing step and needs the owner's
  approval; it was given on 2026-09-25 (see `docs/build/05_DEVLOG.md`).

## Alternatives rejected

- **`gh-pages` branch with built output:** pollutes history with generated
  files and allows deploying something that was never tested.
