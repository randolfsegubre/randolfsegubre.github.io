import { useCallback, useEffect, useState } from "react";

export type Theme = "light" | "dark";

const STORAGE_KEY = "theme";

/** Reads the visitor's remembered choice. Storage can throw (private windows, blocked data), so failure means "no choice". */
function readStoredTheme(): Theme | null {
  try {
    const saved = window.localStorage.getItem(STORAGE_KEY);
    return saved === "light" || saved === "dark" ? saved : null;
  } catch {
    return null;
  }
}

/** True when the operating system prefers dark. `matchMedia` is missing in some test environments, so guard it. */
function systemPrefersDark(): boolean {
  return typeof window.matchMedia === "function" && window.matchMedia("(prefers-color-scheme: dark)").matches;
}

/**
 * Custom hook that owns the light and dark theme (ADR-0004).
 *
 * It exists so the theme's stateful logic (storage, system preference, the
 * `data-theme` attribute CSS reads) stays out of the toggle component. The
 * inline script in `index.html` has already applied a remembered choice
 * before first paint; this hook keeps React's state and the attribute in step
 * afterwards.
 *
 * @returns the active theme and a function that flips it and remembers it.
 */
export function useTheme(): { theme: Theme; toggle: () => void } {
  // STEP 1 of 3: start from the remembered choice, else the system setting.
  const [theme, setTheme] = useState<Theme>(() => readStoredTheme() ?? (systemPrefersDark() ? "dark" : "light"));

  // STEP 2 of 3: mirror the state onto <html data-theme> so the CSS tokens switch.
  useEffect(() => {
    document.documentElement.setAttribute("data-theme", theme);
  }, [theme]);

  // STEP 3 of 3: flipping remembers the choice, best effort (storage may be blocked).
  const toggle = useCallback(() => {
    setTheme((current) => {
      const next: Theme = current === "dark" ? "light" : "dark";
      try {
        window.localStorage.setItem(STORAGE_KEY, next);
      } catch {
        // Not remembering the choice is acceptable; the theme still changes.
      }
      return next;
    });
  }, []);

  return { theme, toggle };
}
