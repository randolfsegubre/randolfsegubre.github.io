# ADR-0001: A static site hosted on GitHub Pages, no backend

- **Status:** Accepted
- **Date:** 2026-09-24

## Context

The site is a personal portfolio: a few sections of text, project cards,
and contact links. It has no user accounts, no forms that need a server, and
no data that changes per visitor. Budget is zero, and the site must stay
online for years without babysitting.

## Decision

Build a fully static site and host it on GitHub Pages as a user site, from a
public repository named `randolfsegubre.github.io`, served from the root
path `/`. There is no backend, no database, and no server-side code. Contact
is a `mailto:` link and social links, not a form.

## Consequences

- Hosting is free, the connection is secured automatically, and there is no
  server to patch.
- No form handling: a contact form would need a third-party service, which
  adds a dependency and a privacy question for no real gain over an email
  link.
- The repository must be public (Pages on a private repository needs a paid
  plan). That is fine: the code is part of the portfolio, so nothing secret
  may ever be committed (see ADR-0006).
- A custom domain can be added later without changing the architecture.

## Alternatives rejected

- **Azure Static Web Apps or Vercel:** also free, but add an account and a
  second dashboard to maintain, and the GitHub-native option already covers
  the need.
- **A .NET backend (Blazor Server or Minimal Application Programming
  Interface (API)):** would show the primary stack, but a portfolio must
  load instantly and cost nothing to host, and both fail that bar.
