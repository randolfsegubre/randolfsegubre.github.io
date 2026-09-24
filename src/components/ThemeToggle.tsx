import { useTheme } from "../hooks/useTheme";

/**
 * Button that switches between the light and dark theme.
 *
 * It is a real `<button>` (keyboard and screen reader friendly by default)
 * whose label names the action it will take, and `aria-pressed` reports the
 * current state. All the state logic lives in `useTheme`.
 */
export function ThemeToggle() {
  const { theme, toggle } = useTheme();
  const isDark = theme === "dark";
  return (
    <button type="button" className="theme-toggle" onClick={toggle} aria-pressed={isDark} aria-label={isDark ? "Switch to light theme" : "Switch to dark theme"}>
      <span aria-hidden="true">{isDark ? "Light" : "Dark"}</span>
    </button>
  );
}
