import { projects } from "../content/projects";
import { ProjectCard } from "./ProjectCard";
import { Section } from "./Section";

/**
 * Selected projects: featured ones first as large cards, the rest as a
 * compact grid.
 *
 * The split is computed from each project's `featured` flag, so promoting or
 * demoting a project is a one-word data change (ADR-0003).
 */
export function ProjectsSection() {
  const featured = projects.filter((project) => project.featured);
  const others = projects.filter((project) => !project.featured);

  return (
    <Section
      id="projects"
      title="Selected projects"
      intro="Public work I can show. Each card says what it is, what my part was, and what is not finished."
    >
      <div className="stack">
        {featured.map((project) => (
          <ProjectCard key={project.id} project={project} />
        ))}
      </div>
      <h3 className="subheading">More projects</h3>
      <div className="grid">
        {others.map((project) => (
          <ProjectCard key={project.id} project={project} />
        ))}
      </div>
    </Section>
  );
}
