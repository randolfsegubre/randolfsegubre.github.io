import { experience } from "../content/experience";
import { Section } from "./Section";
import { TagList } from "./TagList";

/**
 * Employment history, newest first, as a list of articles.
 *
 * Roles done through a staffing or consulting firm show the firm as the
 * employer and the end client on its own line, which matches how the work was
 * actually organised. The client line is omitted when there is none.
 */
export function ExperienceSection() {
  return (
    <Section id="experience" title="Experience" intro="Nine-plus years of professional C# and .NET, newest first.">
      <ol className="timeline">
        {experience.map((role) => (
          <li key={role.id}>
            <article className="role" aria-labelledby={`${role.id}-title`}>
              <header>
                <h3 id={`${role.id}-title`} className="role-title">
                  {role.title}
                </h3>
                <p className="role-company">
                  {role.company}
                  {role.client ? <span className="role-client">, client {role.client}</span> : null}
                </p>
                <p className="role-period">
                  {role.period}
                  {role.location ? ` · ${role.location}` : ""}
                </p>
              </header>
              <ul className="highlights">
                {role.highlights.map((highlight) => (
                  <li key={highlight}>{highlight}</li>
                ))}
              </ul>
              <TagList items={role.stack} label={`${role.company} technologies`} />
            </article>
          </li>
        ))}
      </ol>
    </Section>
  );
}
