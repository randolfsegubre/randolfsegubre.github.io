import type { Project, ProjectStatus } from "../types";
import { ExternalLink } from "./ExternalLink";
import { TagList } from "./TagList";

/** Card wording for each status, so a project is never oversold (ADR-0006). */
const STATUS_LABEL: Record<ProjectStatus, string> = {
  "in-development": "In development",
  reference: "Working reference",
};

interface ProjectCardProps {
  project: Project;
}

/**
 * One project, rendered as an `<article>`.
 *
 * Featured projects show the full set of highlights; the compact ones show
 * only the first two so the page stays scannable. The "known gap" line is
 * rendered only when the data names one, which is the mechanism behind the
 * site's honesty policy: gaps are data, not something a template can forget.
 */
export function ProjectCard({ project }: ProjectCardProps) {
  // STEP 1 of 2: decide how many highlights this card earns.
  const highlights = project.featured ? project.highlights : project.highlights.slice(0, 2);

  // STEP 2 of 2: render the card; the heading id ties the article to its name.
  const headingId = `${project.id}-name`;
  return (
    <article id={project.id} className={project.featured ? "card card-featured" : "card"} aria-labelledby={headingId}>
      <header className="card-header">
        <h3 id={headingId} className="card-title">
          {project.name}
        </h3>
        <span className={`badge badge-${project.status}`}>{STATUS_LABEL[project.status]}</span>
      </header>
      <p className="card-tagline">{project.tagline}</p>
      <p>{project.summary}</p>
      <p className="card-role">
        <strong>My role:</strong> {project.role}
      </p>
      <ul className="highlights">
        {highlights.map((highlight) => (
          <li key={highlight}>{highlight}</li>
        ))}
      </ul>
      {project.honestNote ? (
        <p className="honest-note">
          <strong>Known gap:</strong> {project.honestNote}
        </p>
      ) : null}
      <TagList items={project.stack} label={`${project.name} stack`} />
      {project.links.length > 0 ? (
        <p className="card-links">
          {project.links.map((link) => (
            <ExternalLink key={link.href} href={link.href}>
              {link.label}
            </ExternalLink>
          ))}
        </p>
      ) : null}
    </article>
  );
}
