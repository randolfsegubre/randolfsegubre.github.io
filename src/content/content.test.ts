import { experience } from "./experience";
import { profile } from "./profile";
import { projects } from "./projects";
import { skills } from "./skills";

/**
 * Content integrity and writing-rule tests (ADR-0003, ADR-0006).
 *
 * These turn the owner's writing rules into a failing build instead of
 * something a reviewer has to remember. They only read data, so they run in
 * milliseconds and need no rendering.
 */

/** Product names that legitimately contain capital-letter runs in prose. */
const ALLOWED_CAPS = ["ASP.NET", ".NET", "SQL Server", "UK"]; // longest first: ".NET" would otherwise strand "ASP"

/** Every prose string on the site: text a visitor reads as a sentence. */
function proseStrings(): { where: string; text: string }[] {
  const prose: { where: string; text: string }[] = [
    { where: "profile.headline", text: profile.headline },
    { where: "profile.availability", text: profile.availability },
  ];
  for (const project of projects) {
    prose.push({ where: `${project.id}.tagline`, text: project.tagline });
    prose.push({ where: `${project.id}.summary`, text: project.summary });
    prose.push({ where: `${project.id}.role`, text: project.role });
    if (project.honestNote) prose.push({ where: `${project.id}.honestNote`, text: project.honestNote });
    project.highlights.forEach((text, i) => prose.push({ where: `${project.id}.highlights[${i}]`, text }));
  }
  for (const role of experience) {
    role.highlights.forEach((text, i) => prose.push({ where: `${role.id}.highlights[${i}]`, text }));
  }
  return prose;
}

/** Every string in the content, prose and chips alike. */
function allStrings(): string[] {
  return [
    ...Object.values(profile).flatMap((v) => (typeof v === "string" ? [v] : [])),
    ...proseStrings().map((p) => p.text),
    ...projects.flatMap((p) => [p.name, ...p.stack]),
    ...experience.flatMap((r) => [r.company, r.title, r.period, ...(r.client ? [r.client] : []), ...r.stack]),
    ...skills.flatMap((g) => [g.label, ...g.items]),
  ];
}

describe("content writing rules", () => {
  it("contains no em dashes anywhere", () => {
    const offenders = allStrings().filter((s) => s.includes("—"));
    expect(offenders).toEqual([]);
  });

  it("writes abbreviations out in full words in prose (Full Word Format)", () => {
    const offenders: string[] = [];
    for (const { where, text } of proseStrings()) {
      // STEP 1 of 3: drop allowed product names that contain capital runs.
      let cleaned = text;
      for (const name of ALLOWED_CAPS) cleaned = cleaned.split(name).join(" ");
      // STEP 2 of 3: drop a short form that directly follows its full words, e.g. "(CSP)".
      cleaned = cleaned.replace(/\([A-Z]{2,}\)/g, " ");
      // STEP 3 of 3: any remaining run of two or more capitals is a bare abbreviation.
      const bare = cleaned.match(/\b[A-Z]{2,}\b/g);
      if (bare) offenders.push(`${where}: ${bare.join(", ")}`);
    }
    expect(offenders).toEqual([]);
  });

  it("uses https for every outbound link", () => {
    const links = [...profile.links, ...projects.flatMap((p) => p.links)];
    for (const link of links) {
      expect(link.href.startsWith("https://"), link.href).toBe(true);
      expect(link.label.trim()).not.toBe("");
    }
  });
});

describe("content integrity", () => {
  it("gives every project and role a unique id", () => {
    const ids = [...projects.map((p) => p.id), ...experience.map((r) => r.id), ...skills.map((g) => g.id)];
    expect(new Set(ids).size).toBe(ids.length);
  });

  it("has no empty required fields", () => {
    for (const project of projects) {
      for (const value of [project.name, project.tagline, project.summary, project.role]) {
        expect(value.trim(), project.id).not.toBe("");
      }
      expect(project.highlights.length, project.id).toBeGreaterThan(0);
      expect(project.stack.length, project.id).toBeGreaterThan(0);
    }
    for (const role of experience) {
      expect(role.highlights.length, role.id).toBeGreaterThan(0);
    }
  });

  it("features at least one project and does not link a private repository", () => {
    expect(projects.some((p) => p.featured)).toBe(true);
    const galaxy = projects.find((p) => p.id === "galaxy-survivor");
    expect(galaxy?.links).toEqual([]);
  });
});
