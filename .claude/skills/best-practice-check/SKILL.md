---
name: best-practice-check
description: Run the portfolio site's best-practice checklist (build, tests, security, Content Security Policy, workflow pinning, accessibility, search tags, docs) before calling any change done. Use whenever code, content, dependencies, the deploy workflow or documentation in this repository changed, and before any commit or push proposal.
---

# Portfolio best-practice check

The standing gate from ADR-0009. The checklist itself lives in
`docs/build/06_BEST_PRACTICE_CHECKLIST.md`; this skill is how to run it. Do not
copy the checklist into other files.

## Steps

1. Read `docs/build/06_BEST_PRACTICE_CHECKLIST.md` in full, and the operating
   manual `docs/build/01_CLAUDE.md` if this is a new session.
2. Run the mechanical checks and read the real output, not a memory of it:
   - `dotnet build Portfolio.slnx -warnaserror`
   - in `web/`: `npm ci`, `npm audit --audit-level=moderate`, then `npm run generate`
   - `dotnet test Portfolio.slnx` (it tests the generated site)
   - `dotnet list Portfolio.slnx package --vulnerable --include-transitive`
   - `grep -rn "ADR-0002" src` must print nothing (that decision is superseded).
3. Check what a tool cannot: open the page in a real browser (`npm run serve` in `web/`,
   then `http://localhost:5099/`),
   confirm the console is empty (no Content Security Policy violations), the
   theme toggle works and survives a reload, and the layout holds at phone and
   desktop width. Stop only the server you started.
4. Walk each remaining checklist item against the diff (`git diff`), not
   against intentions. Mark what is verified, and say plainly what was not.
5. Fix what is wrong, or, if a rule is broken on purpose, record the reason in
   an ADR or a comment. If two rules conflict, name the conflict to the owner.
6. Update `docs/build/04_TASKS.md` and append a dated `05_DEVLOG.md` entry.
7. Report in two or three sentences: what passed, what was fixed, what is left.
   Never commit, push or publish without the owner's explicit yes.
