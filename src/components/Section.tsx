import type { ReactNode } from "react";

interface SectionProps {
  id: string;
  title: string;
  intro?: string;
  children: ReactNode;
}

/**
 * The shared wrapper for every content section.
 *
 * It exists so heading structure, spacing and the accessible name are decided
 * in one place: the `<section>` is labelled by its own heading, which lets
 * screen reader users list the page's regions by name.
 */
export function Section({ id, title, intro, children }: SectionProps) {
  const headingId = `${id}-heading`;
  return (
    <section id={id} aria-labelledby={headingId} className="section">
      <div className="container">
        <h2 id={headingId} className="section-title">
          {title}
        </h2>
        {intro ? <p className="section-intro">{intro}</p> : null}
        {children}
      </div>
    </section>
  );
}
