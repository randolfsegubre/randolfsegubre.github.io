import { skills } from "../content/skills";
import { Section } from "./Section";
import { TagList } from "./TagList";

/**
 * Skills, grouped. Every group is a labelled list of product and technique
 * names that the projects and roles above back up (ADR-0006).
 */
export function SkillsSection() {
  return (
    <Section id="skills" title="Skills">
      <div className="grid">
        {skills.map((group) => (
          <div key={group.id} className="skill-group">
            <h3 className="skill-title">{group.label}</h3>
            <TagList items={group.items} label={group.label} />
          </div>
        ))}
      </div>
    </Section>
  );
}
