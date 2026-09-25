# ADR-0007: ASP.NET Core MVC on .NET 10, exported to static files

- **Status:** Accepted. Supersedes ADR-0002.
- **Date:** 2026-09-25

## Context

ADR-0002 chose Vite, React and TypeScript. On 2026-09-25 the owner asked for
his main stack instead: C#, ASP.NET MVC, and the latest .NET and C# versions.
That is the right call for a portfolio whose main message is "senior .NET
developer": the site itself should be written in the stack it advertises.

The constraint from ADR-0001 still holds: GitHub Pages serves static files
only, so it cannot run an ASP.NET server. A live server would need a paid or
cold-starting host, and the site must be online instantly, for free, for years.

## Decision

- Build the site as an **ASP.NET Core MVC** application on **.NET 10 with
  C# 14**: one controller, Razor views and partials, C# records for content,
  dependency injection, xUnit tests.
- At build time, the app exports itself to plain HTML: `dotnet run -- --export
  <folder>` starts the app on a free local port, requests each route
  (`/` and `/not-found`), writes the HTML as `index.html` and `404.html`,
  copies `wwwroot`, and exits. Only that output is deployed to Pages.
- The visitor's browser receives static HTML, one stylesheet and one small
  script. No .NET runtime runs in the browser or on a server.

## Consequences

- The source shows real MVC (controller, view model, partials, dependency
  injection, interface-based content service) in the owner's home stack.
- First load is a few tens of kilobytes, with no runtime download.
- The live site is an export, not a running MVC server. Anything that needs a
  server at request time (forms, a database, server-side sessions) is out of
  scope, which ADR-0001 already accepted.
- The exporter is a small piece of custom code, covered by tests, that must
  fail loudly if any page does not return 200.
- If a server-hosted version is ever wanted (for example on a free web app
  tier), the same app runs unchanged with `dotnet run`.

## Alternatives rejected

- **Keep React (ADR-0002):** works, but the site would not show the owner's
  strongest stack.
- **Blazor WebAssembly:** C#, but visitors download the .NET runtime first,
  several megabytes before anything shows, which is the wrong first
  impression on a phone.
- **A live ASP.NET MVC server on a free tier:** real MVC at request time, but
  free tiers cold-start (a slow first click from a recruiter's email link) and
  a custom domain usually costs money. Static hosting has neither problem.
- **A third-party static site generator (Statiq and similar):** adds a
  dependency to learn for a two-page site the framework can already render.
