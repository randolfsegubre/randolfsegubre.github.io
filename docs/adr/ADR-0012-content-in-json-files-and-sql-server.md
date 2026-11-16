# ADR-0012: Content in JSON data files, stored in SQL Server

- **Status:** Accepted. Amends ADR-0003 (typed content modules) and ADR-0001 (no backend in production).
- **Date:** 2026-11-17

## Context

Until now every word and drawing on the site was written in C# classes (`ProfileData`, `ProjectData` and ten more),
compiled into the app. The owner pointed out that hard-coding data is not best practice and asked for it to live in
a file type made for data and in a Microsoft SQL Server database (not SQLite).

The constraints did not change: GitHub Pages serves static files only (ADR-0001), so no database can exist in
production; every claim needs a source (ADR-0006); the Content Security Policy stays strict (ADR-0009); and the
published page must come out identical. GitHub's build server cannot reach a developer's own SQL Server, so the
build has to bring its own.

## Decision

- **The content is 12 JSON files in `content/`**, one per kind of content (`content/README.md` lists them and the
  rules that come with each). They are reviewable, versioned and checked by the tests, and they are the source of
  truth. Nothing is written in code any more.
- **The content is stored in SQL Server**, in a dedicated `content` schema designed table by table
  (`src/Portfolio.Data/PortfolioDbContext.cs`, migrations by Entity Framework Core 10). A `Seed` step makes the
  database say exactly what the files say, replacing everything inside one transaction. It is safe to run on every
  build, and a failed seed leaves the previous content in place.
- **The app reads only from SQL Server**, once, when it starts, into an immutable in-memory snapshot. Requests are
  served from memory, so a database outage cannot take a running API down and the database is never touched at
  request time.
- **Every build exercises the database.** `npm run generate` runs one backend command that migrates the database,
  seeds it from the files and reads the content back out into `web/data/content.json`. The Nuxt build starts from
  that. The GitHub workflow starts a throwaway SQL Server container for the length of the build (`mcr.microsoft.com/mssql/server`,
  2025, pinned by digest, listening on the runner's loopback address only, with a password generated for the run and
  never stored). Locally, the SQL Server on the developer's machine is used with Windows authentication.
- **Schema rules live in the database**: keys and foreign keys, one-row tables enforced by a check constraint, the
  allowed project statuses, a unique filtered index that permits at most one centre star, one-of-two rules, length
  limits. Ordered lists of plain strings that are only read with their parent (a project's highlights, a role's
  technologies) are one JSON text column on the parent row; anything with an identity or that other rows point at is
  its own table.
- **Reading the files is strict**: an unknown field, a null where a value is required, a missing file or invalid JSON
  stops the build and names the file.
- **The production site is unchanged: static files.** No database or server runs there.

## Proof that nothing changed

The document built through SQL Server was compared with the document built from the old C# classes: identical, apart
from line endings inside the decorative code lanes, which the C# raw strings had taken from the file (so Windows and
Linux builds used to differ). The data files use LF, so every machine now builds the same bytes. The generated site was
then compared file by file with the deployed one: identical.

## Security

- No connection string or password is in the repository. The app requires `ConnectionStrings__Portfolio` from the
  environment (or user secrets). Local development uses Windows authentication, so there is no password at all.
- All database access goes through Entity Framework Core (parameterised); there is no hand-built SQL.
- The workflow's container: loopback only, a fresh random password masked in the log, destroyed at the end of the run,
  image pinned by digest. `TrustServerCertificate=True` is used because the container's certificate is self-signed;
  that is acceptable only for a database on the same machine and must never be copied to a remote server.
- A manual run of the workflow from a branch builds and tests but never deploys (`deploy` runs only on `main`).
- New package: Entity Framework Core SQL Server provider 10.0.15 (and its Design tools, build-time only). Both
  vulnerability gates stay in the workflow.

## Consequences

- Changing content is editing JSON, then `npm run generate`. A database is now needed to build the site (SQL Server
  locally; the workflow provides one).
- The database is a derived store, not an independent source of truth: edits made directly in the database are
  overwritten by the next seed. To keep a change, edit the file.
- Tests: the writing rules and page checks read the JSON files; `SqlContentTests` runs against a real, throwaway SQL
  Server database and proves the round trip, idempotent seeding, and 13 ways the schema must refuse nonsense.
- The Dependency Inversion in ADR-0003 pays off: nothing above the data layer changed when the source did.

## Alternatives rejected

- **SQLite.** Ruled out by the owner; the owner's stack is SQL Server.
- **The local database as the source of truth, with an exported snapshot committed for the build.** Easier to edit
  rows, but the snapshot can silently go stale and the pipeline would never exercise the database.
- **Azure SQL or another hosted database.** It costs money and adds a secret and a network dependency to a free static site.
- **JSON files only, no database.** Simpler, but it was not what was asked, and it would not show the SQL Server work.
- **Child tables for every string list.** About ten extra tables with no behaviour of their own; JSON text columns on the
  parent row carry the same data with the same ordering, and the lists are never queried on their own.
- **Storing the skills cloud's computed positions.** They are a pure function of the tree, so storing them would only
  create a second copy that can disagree.
