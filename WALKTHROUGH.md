# Walkthrough: how a page load flows through the code

This traces what actually happens, in order, from a visitor opening the site
to the page on screen. It explains vocabulary as it goes and points at the
Architecture Decision Records (ADRs) instead of repeating their reasoning.

## Vocabulary

- **Component:** a JavaScript function that returns a piece of the page
  (`Hero`, `ProjectCard`). React calls it and turns the result into
  HyperText Markup Language (HTML).
- **Props:** the inputs a component receives, like function arguments.
- **Hook:** a function starting with `use` that gives a component memory or
  side effects (`useTheme`).
- **Content module:** a plain TypeScript file in `src/content/` holding the
  site's words as typed data (ADR-0003).
- **Build:** `vite build` turns `src/` into the static `dist/` folder that
  GitHub Pages serves (ADR-0001, ADR-0005).

## The flow, step by step

1. **The browser requests `/`.** GitHub Pages returns `dist/index.html`. It
   is a nearly empty page: a `<div id="root">`, a small inline script, and
   links to the built JavaScript and Cascading Style Sheets (CSS) files.
2. **The inline theme script runs first** (in `index.html`, before anything
   paints). If the visitor previously chose a theme, it is read from
   `localStorage` and written to `<html data-theme="...">`. This is why the
   page never flashes the wrong theme (ADR-0004).
3. **The stylesheet applies.** `src/styles.css` reads its color tokens from
   `:root`. If the visitor has no saved choice and their system prefers dark,
   the `prefers-color-scheme` block redefines the tokens.
4. **`src/main.tsx` mounts React.** It finds `#root` (throwing a clear error
   if it is missing) and renders `<App />` inside `StrictMode`, a development
   aid that double-checks components for unsafe patterns.
5. **`App` (`src/App.tsx`) lays out the page.** It only decides order: skip
   link, `Header`, then inside `<main>` the five sections, then `Footer`. It
   holds no copy.
6. **`Header` renders the name and navigation,** and `ThemeToggle` calls the
   `useTheme` hook.
7. **`useTheme` (`src/hooks/useTheme.ts`) runs its three steps:** start from
   the remembered choice or the system setting; mirror the state onto
   `<html data-theme>`; on click, flip the theme and remember it.
8. **Each section reads its own content module.** `Hero` reads `profile`;
   `ProjectsSection` reads `projects`; `ExperienceSection` reads
   `experience`; `SkillsSection` reads `skills`; `ContactSection` reads
   `profile` again.
9. **Shared wrappers keep the details consistent.** `Section` gives every
   section a heading-labelled region. `ExternalLink` adds
   `rel="noopener noreferrer"` and a screen-reader hint to every outbound
   link. `TagList` renders chips as a real list.
10. **`ProjectCard` decides what a card shows.** Featured projects show all
    highlights, the others show two. The "Known gap" line appears only if the
    data names one, so an honest note can never be forgotten by a template
    (ADR-0006).

## Following one claim end to end

Take the BudgetPH "Known gap" sentence:

1. It is written in `src/content/projects.ts` as `honestNote`.
2. `content.test.ts` checks it against the writing rules (no em dashes, no
   bare abbreviations).
3. `ProjectsSection` passes the project to `ProjectCard`.
4. `ProjectCard` renders `<p class="honest-note">` because `honestNote` is
   not null.
5. `App.test.tsx` counts "Known gap:" labels against the number of projects
   that have one.
6. Its source is recorded in `docs/CONTENT_SOURCES.md`.

## Where to look when something breaks

| Symptom | Look at |
|---|---|
| A word or claim is wrong | `src/content/*.ts`, then `docs/CONTENT_SOURCES.md` |
| Layout or color is off | `src/styles.css` tokens at the top |
| Wrong theme on load | the inline script in `index.html`, then `useTheme` |
| Blank page after deploy | the failed step in the GitHub Actions run |
| A test fails on writing rules | `src/content/content.test.ts` says which string and why |
