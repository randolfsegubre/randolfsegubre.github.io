/**
 * Runs in the page head, before first paint (ADR-0004, ADR-0009).
 *
 * It lives in its own file, not inline in the layout, so the Content Security Policy can
 * forbid inline scripts entirely. It is deliberately not deferred: it must finish before
 * the page is drawn so a remembered theme never flashes the wrong colours.
 */
(function () {
  "use strict";

  // STEP 1 of 2: mark the page as scripted, so scroll-reveal styles only hide
  // content when the script that reveals it is actually going to run.
  document.documentElement.classList.add("js");

  // STEP 2 of 2: apply a remembered manual theme choice. With no saved choice, dark is the default.
  try {
    var saved = localStorage.getItem("theme");
    if (saved === "light" || saved === "dark") {
      document.documentElement.setAttribute("data-theme", saved);
    }
  } catch (e) {
    // Storage can be blocked; the default theme is fine.
  }
})();
