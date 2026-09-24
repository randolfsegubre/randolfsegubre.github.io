# Architecture and Patterns Guide

This guide explains why the code is shaped the way it is, so someone opening
it cold (including the owner, months from now, with no assistant) can follow
the reasoning. Each pattern states its rejected alternative.

## Shape of the system

```
index.html  ->  src/main.tsx  ->  <App />
                                   |- Header (nav + ThemeToggle)
                                   |- Hero            <- content/profile
                                   |- ProjectsSection <- content/projects
                                   |- ExperienceSection <- content/experience
                                   |- SkillsSection   <- content/skills
                                   |- ContactSection  <- content/profile
                                   '- Footer
```

Data flows one way: content modules, then props, then markup. Nothing in the
page writes back to the data.

## Pattern catalog

| Pattern | Where | Why | Rejected alternative |
|---|---|---|---|
| **Data-driven presentation** (content separated from view) | `src/content/*`, all sections | Content changes often, layout rarely; the type checker guards the data (ADR-0003) | Text inside JSX: every edit is a code edit and duplicates hide |
| **Composition over configuration** | `Section` wraps every section; `ExternalLink` wraps every outbound link | One place enforces the heading structure, spacing and link safety (`rel`, new-tab hint) | Repeating raw `<a target="_blank">` tags and forgetting `rel="noopener noreferrer"` |
| **Custom hook for a cross-cutting concern** | `useTheme` | Theme logic (storage, system preference, DOM attribute) is stateful and must not leak into components | Inline effects in the toggle component, which would tangle rendering with storage |
| **Progressive enhancement** | inline theme script in `index.html`, plus CSS `prefers-color-scheme` | The right theme paints before React loads; the page still reads without JavaScript in the head script | Applying the theme only in a React effect, which flashes the wrong theme |
| **Design tokens** | `:root` custom properties in `src/styles.css` | One block controls color, spacing and type in both themes (ADR-0004) | Hard-coded colors scattered through rules |
| **Content lint as a test** | `content.test.ts` | Turns the writing rules (ADR-0006) into a failing build instead of a memory | Relying on review to notice a bare abbreviation or a stray em dash |

## Object-oriented and SOLID notes for a functional front end

React function components map onto the same principles:

- **Single Responsibility:** each component shows one thing; `useTheme` only
  manages the theme.
- **Open/Closed:** adding a project or a role means adding data, not editing
  a component.
- **Dependency Inversion:** components depend on the typed shapes in
  `src/types.ts`, not on where the data comes from, so the source could move
  to Markdown or a file loader without touching them.
- **Interface Segregation:** each section receives only the slice it needs.

## Accessibility architecture

Landmarks (`header`, `main`, `footer`, `nav`) and one `h1`; a skip link to
`#main`; every section labelled by its heading; focus styles never removed;
external links announce that they open a new tab; motion is disabled under
`prefers-reduced-motion`.

## What is deliberately absent

No router, no state library, no CSS framework, no analytics, no web fonts, no
runtime network requests. Each would add weight for no visitor benefit.
