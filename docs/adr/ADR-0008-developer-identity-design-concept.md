# ADR-0008: A "developer identity" design concept, inspired by a reference and made original

- **Status:** Accepted
- **Date:** 2026-09-25

## Context

The owner asked for a concept in the spirit of another developer's portfolio
(`rujealfon.github.io`) but unique to him: use it as a reference, not a copy.
The reference is a Vue.js developer's site with a navy and green palette. Its
concept, as observed on the live page, is:

- a full-screen hero with faint code snippets floating behind the name;
- animated scroll reveals (built with the GSAP animation library);
- an "about" area with a profile photo and animated percentage skill bars;
- featured project cards with a "Live" badge, role and date;
- an animated work-experience timeline;
- a "Let's Work Together" contact section.

Copying its text, code, colors, photo or animation code would be both
unoriginal and wrong. Its structure is a common portfolio pattern; its
execution belongs to its author.

## Decision

Keep the general idea (a developer's identity expressed through code and
motion) and give it an original execution:

| Reference idea | This site's original version |
|---|---|
| Faint code snippets floating in the hero | **C# and Razor** fragments from the owner's own stack, in purple and gold, kept to the edges and behind the card so the headline stays readable |
| (nothing comparable) | A **`Developer.cs` code card** that renders the owner's profile as a C# record from the same `Profile` data as the rest of the page, so it can never disagree with it |
| Animated percentage skill bars | **No percentage bars.** They cannot be verified, which breaks ADR-0006. A **stats strip** of checkable facts instead (years, industries, employers, commits) |
| GSAP scroll animations | About 100 lines of vanilla script: scroll reveals, an active-section highlight in the navigation, and a gold scroll-progress bar. No third-party library |
| Profile photo | A crown mark (royal purple and gold, ADR-0004) |
| Navy and green palette | Dark royal purple and gold, dark by default |
| "Let's Work Together" contact | "Let's talk" with large link cards for email, profiles and location |

Every motion is an enhancement: content is hidden for a reveal only when the
`js` class is present, reduced-motion visitors see everything at once, and the
whole page works with scripting off.

## Consequences

- The design has a recognisable idea that is the owner's own: his stack as the
  visual language.
- No third-party requests and no animation dependency to maintain.
- The stats and the code card are generated from data, so wording changes stay
  in `Content/ProfileData.cs`, and tests check them (percent signs are
  rejected, and the card must match the profile).
- The hero has more moving parts than a plain layout. Accepted: the backdrop is
  decorative, hidden from assistive technology, and switched off under reduced
  motion.

## Alternatives rejected

- **A close copy of the reference layout:** unoriginal and not the owner's
  request.
- **Skill percentage bars:** fashionable and unverifiable.
- **GSAP or another animation library:** a third-party dependency and network
  request for effects a small script can do.
