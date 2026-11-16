# ADR-0013: A three-layer architecture for the .NET backend

- **Status:** Accepted. Refines ADR-0011 (the backend is no longer one project).
- **Date:** 2026-11-17

## Context

With a database in the picture (ADR-0012), one project that held the web endpoint, the content rules and the
database code would mix three kinds of work. The owner asked for the backend to use a three-layer architecture.

## Decision

The backend is three projects, and every dependency points one way: **presentation, then business, then data**.

| Layer | Project | Holds | Must not know |
|---|---|---|---|
| Presentation | `src/Portfolio.Api` | The web endpoint, its security headers, the build commands (`--migrate`, `--seed`, `--export-content`), the file exporter | The data layer, the database, Entity Framework |
| Business | `src/Portfolio.Business` | The content models, the document builder, the skills-cloud layout, the JSON file reader, the content service, the mapper between rows and records | The presentation layer, the database itself |
| Data access | `src/Portfolio.Data` | The SQL Server schema, the migrations, the repository that reads and writes rows | Our other two layers, the web |

- **Presentation talks only to business.** Its one registration call is `AddPortfolioContent`, which also wires the
  data layer behind it, so it never names a repository, a context or a connection. It asks for content
  (`IContentSource`) or for maintenance (`IContentMaintenance`) and never sees a row.
- **Business uses the data layer through an interface.** `ContentService` depends on `IContentRepository`, so a test can
  replace the database with an in-memory repository, and the data layer can change without touching business rules.
  The business layer owns the translation (`ContentMapper`): the schema can change without touching the content
  records, and the records without touching the schema.
- **Data speaks in rows and nothing else.** It does not know what a content record or a page is.
- **The rule is enforced, not hoped for.** Project references make most violations fail to compile, and
  `LayeringTests` inspect the compiled assemblies, so a quiet leak (the endpoint reaching the database through a
  transitive package) fails the build too.
- Shared build settings (strict warnings, analyzers) are in `src/Directory.Build.props`. Migrations are generated code
  and exempt from the analyzers.

## Layers are not tiers

A **layer** is a logical separation of code inside a solution. A **tier** is a physical separation: a part that runs on
its own machine or process and is reached over a network. Three layers can be deployed as one tier (everything in one
process) or as three, so "three-layer" and "N-tier" are related but not the same.

This solution is three layers. At build time it runs as two processes plus a database server (the app, which holds
the presentation and business layers and the data code, and SQL Server). In production nothing runs at all, because
the site is static files. Becoming truly N-tier would mean a hosted API that the browser calls at run time, plus a
shared contracts project for the data crossing the network. ADR-0011 rejected that: GitHub Pages cannot host an API,
it needs a second host (a cost, and an API that can be asleep when a recruiter visits), a cross-origin allowance and a
looser Content Security Policy (`connect-src`).

## Consequences

- More projects and one more registration call, in exchange for rules a reader (or a test) can check.
- Adding a content kind means touching the layers in order: a row and a table (data), a record and a mapping
  (business), and nothing in presentation unless the document changes.
- Visual Studio reloads the solution to show the two new projects.

## Alternatives rejected

- **Clean Architecture (the data layer depending on the business layer's interfaces).** Better in a large system and
  the owner's pattern in other projects, but the owner asked for three layers, and with the repository interface in the
  data layer the business layer is already testable without a database.
- **One project with folders.** Cheaper, but nothing stops a controller reaching the database, and the compiler cannot
  help.
- **A fourth layer (domain or shared contracts).** There is no second consumer of the models, so it would be a project
  with one reference.
- **N-tier deployment.** See above.
