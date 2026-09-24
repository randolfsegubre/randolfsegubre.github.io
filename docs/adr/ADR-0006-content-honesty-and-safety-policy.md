# ADR-0006: Content honesty and safety policy

- **Status:** Accepted
- **Date:** 2026-09-24

## Context

A portfolio is read by people deciding whether to trust the owner. One
inflated claim undermines everything true around it. The repository is also
public, so anything committed is published.

## Decision

1. **Every claim must be backed by something checkable:** a public
   repository, a verified run of the project, git history, or a document the
   owner can produce. `docs/CONTENT_SOURCES.md` records the source of each
   claim on the site.
2. **Status is stated plainly.** A project that is in development says so,
   and known gaps are named (for example, "checkout is not built yet")
   instead of talked around.
3. **Wording matches the real role.** Say "contributed to" where the work
   was shared, and "led" only where that is the documented truth.
4. **No confidential or proprietary material.** Employer and client
   company names are fine. Code, internal system names, ticket numbers and
   anything covered by a confidentiality agreement are not.
5. **Only public work is linked.** A private repository gets no link and its
   card says so.
6. **No secrets in the repository.** The site has no environment secrets by
   design; `.env` files are ignored.
7. **Writing conventions:** first person, evidence-backed, no em dashes, and
   abbreviations written out in full words on first use (the tag chips that
   list product names are exempt). The content test enforces the mechanical
   parts.

## Consequences

- Cards are shorter and more specific than marketing copy would be.
- Adding a project means adding its source to `docs/CONTENT_SOURCES.md`.
- Some real work is deliberately left off the site because it cannot be
  shown publicly.

## Alternatives rejected

- **Showcase everything, polish later:** invites exactly the overclaim this
  policy exists to prevent.
