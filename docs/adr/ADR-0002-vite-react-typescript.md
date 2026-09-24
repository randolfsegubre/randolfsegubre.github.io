# ADR-0002: Vite, React and TypeScript for the front end

- **Status:** Accepted
- **Date:** 2026-09-24

## Context

The site is static (ADR-0001), so the choice is which tool builds it. The
owner's professional strength is C# and .NET; his front-end experience is
Vue.js and React. Many target job postings ask for React, and the site
itself is a small, honest example of current React and TypeScript practice.

## Decision

Use Vite as the build tool and development server, React for components, and
TypeScript in strict mode. Testing uses Vitest with Testing Library. No
routing library: the site is one page with anchor links.

## Consequences

- Fast builds and a plain static `dist/` folder that GitHub Pages serves
  as is.
- Building and documenting the site in React and TypeScript is real practice
  in areas the owner wants to strengthen, and every file carries the in-code
  documentation standard so it doubles as a learning aid.
- Slightly more tooling than plain HyperText Markup Language (HTML). Accepted:
  typed content (ADR-0003) is worth it.

## Alternatives rejected

- **Plain HTML, Cascading Style Sheets (CSS) and JavaScript:** smallest and
  fastest, but gives up typed content and component reuse.
- **Astro:** excellent for content sites and ships almost no JavaScript, but
  is another framework to learn for a page this small.
- **Next.js static export:** heavier than the problem needs.
- **Blazor WebAssembly:** the owner's home stack, but the first load is
  several megabytes, which is the wrong first impression for a recruiter
  opening a link.
