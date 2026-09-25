/**
 * Light and dark theme toggle (ADR-0004).
 *
 * The inline script in the layout has already applied a remembered choice before
 * first paint. This file only wires the button: it reveals it (the button is
 * rendered hidden so it never shows as dead without scripting), keeps its label
 * and pressed state in step with the theme, and remembers the visitor's choice.
 */
(function () {
  "use strict";

  var STORAGE_KEY = "theme";
  var root = document.documentElement;
  var button = document.getElementById("theme-toggle");
  if (!button) {
    return;
  }

  /** The theme currently showing: the explicit attribute, else the system setting. */
  function currentTheme() {
    var attribute = root.getAttribute("data-theme");
    if (attribute === "light" || attribute === "dark") {
      return attribute;
    }
    var prefersDark = typeof window.matchMedia === "function" && window.matchMedia("(prefers-color-scheme: dark)").matches;
    return prefersDark ? "dark" : "light";
  }

  /** Updates the button's label and pressed state to describe the action it will take. */
  function render(theme) {
    var isDark = theme === "dark";
    button.setAttribute("aria-pressed", String(isDark));
    button.setAttribute("aria-label", isDark ? "Switch to light theme" : "Switch to dark theme");
    button.firstElementChild.textContent = isDark ? "Light" : "Dark";
  }

  // STEP 1 of 3: reveal the button and show the current state.
  button.hidden = false;
  render(currentTheme());

  // STEP 2 of 3: on click, flip the theme, apply it, and remember it (best effort, storage may be blocked).
  button.addEventListener("click", function () {
    var next = currentTheme() === "dark" ? "light" : "dark";
    root.setAttribute("data-theme", next);
    try {
      window.localStorage.setItem(STORAGE_KEY, next);
    } catch (e) {
      // Not remembering the choice is acceptable; the theme still changes.
    }
    render(next);
  });

  // STEP 3 of 3: with no manual choice, follow the system setting if it changes while the page is open.
  if (typeof window.matchMedia === "function") {
    window.matchMedia("(prefers-color-scheme: dark)").addEventListener("change", function () {
      if (!root.getAttribute("data-theme")) {
        render(currentTheme());
      }
    });
  }
})();
