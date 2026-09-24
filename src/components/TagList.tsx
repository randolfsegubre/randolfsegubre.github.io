interface TagListProps {
  items: string[];
  label: string;
}

/**
 * A row of product-name chips (for example "React", "Umbraco").
 *
 * Rendered as a real list with an accessible name so a screen reader announces
 * "Stack, list, 10 items" instead of a run of unrelated words. Chips list
 * product names only, which is why they are exempt from the abbreviation rule
 * (ADR-0006).
 */
export function TagList({ items, label }: TagListProps) {
  return (
    <ul className="tags" aria-label={label}>
      {items.map((item) => (
        <li key={item} className="tag">
          {item}
        </li>
      ))}
    </ul>
  );
}
