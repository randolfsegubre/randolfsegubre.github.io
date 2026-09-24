/**
 * Shapes of every piece of content on the site (ADR-0003).
 *
 * Components depend on these types, never on where the data comes from, so
 * the content could later move to Markdown or a loader without touching any
 * component. The content integrity test (`content.test.ts`) also reads data
 * through these types.
 */

/** A labelled outbound link. `href` must be https (enforced by a test). */
export interface Link {
  label: string;
  href: string;
}

/** Top-level facts about the owner, used by the hero, header and contact. */
export interface Profile {
  name: string;
  title: string;
  location: string;
  /** One-paragraph pitch shown in the hero. Prose: follows the writing rules. */
  headline: string;
  /** Short line about what he is looking for. */
  availability: string;
  email: string;
  links: Link[];
  /** Relative path to a resume file under `public/`. Button hidden while null. */
  resumeUrl: string | null;
}

/** Where a project stands. Shown on the card so nothing is oversold. */
export type ProjectStatus = "in-development" | "reference";

/** One portfolio project card. */
export interface Project {
  /** Stable unique identifier, also used as the element id. */
  id: string;
  name: string;
  /** One line under the name. */
  tagline: string;
  /** What it is and why it exists, in prose. */
  summary: string;
  /** His actual role on it, stated plainly. */
  role: string;
  status: ProjectStatus;
  /** Featured projects render larger and first. */
  featured: boolean;
  /** Specific, checkable points. Prose: follows the writing rules. */
  highlights: string[];
  /** A known gap, stated plainly, or null when there is none worth naming. */
  honestNote: string | null;
  /** Product names shown as chips. Exempt from the abbreviation rule. */
  stack: string[];
  /** Public links only (ADR-0006). Empty for a private repository. */
  links: Link[];
}

/** One employment role. */
export interface Role {
  id: string;
  company: string;
  /** End client, when the work was done through a staffing or consulting firm. */
  client: string | null;
  title: string;
  /** Human-readable period, for example "June 2024 to August 2026". */
  period: string;
  location: string | null;
  highlights: string[];
  /** Product names shown as chips. Exempt from the abbreviation rule. */
  stack: string[];
}

/** A named group of skills. Items are product or technique names. */
export interface SkillGroup {
  id: string;
  label: string;
  items: string[];
}
