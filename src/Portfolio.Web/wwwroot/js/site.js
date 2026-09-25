/**
 * Page behaviors (ADR-0008): scroll reveals, active-section highlight in the navigation,
 * and a scroll-progress bar. Plain vanilla script, no third-party library, and every
 * behavior is an enhancement: with scripting off (or reduced motion on) the page is
 * fully visible and fully usable.
 */
(function () {
  "use strict";

  var root = document.documentElement;
  var reducedMotion = typeof window.matchMedia === "function" && window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // STEP 1 of 3: reveal elements as they scroll into view. Without IntersectionObserver, or when
  // the visitor prefers reduced motion, show everything immediately instead of animating.
  var revealItems = document.querySelectorAll("[data-reveal]");
  if (!("IntersectionObserver" in window) || reducedMotion) {
    revealItems.forEach(function (item) {
      item.classList.add("is-visible");
    });
  } else {
    var revealObserver = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (entry.isIntersecting) {
            entry.target.classList.add("is-visible");
            revealObserver.unobserve(entry.target);
          }
        });
      },
      { rootMargin: "0px 0px -8% 0px", threshold: 0.05 }
    );
    revealItems.forEach(function (item) {
      revealObserver.observe(item);
    });
  }

  // STEP 2 of 3: mark the navigation link of the section currently in view with aria-current,
  // which the stylesheet turns into a gold underline. Sections without a nav link are ignored.
  var navLinks = Array.prototype.slice.call(document.querySelectorAll(".nav-list a[href*='#']"));
  var sections = navLinks
    .map(function (link) {
      return document.getElementById(link.getAttribute("href").split("#")[1]);
    })
    .filter(Boolean);

  if ("IntersectionObserver" in window && sections.length > 0) {
    var spy = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (!entry.isIntersecting) {
            return;
          }
          navLinks.forEach(function (link) {
            var isCurrent = link.getAttribute("href").split("#")[1] === entry.target.id;
            if (isCurrent) {
              link.setAttribute("aria-current", "true");
            } else {
              link.removeAttribute("aria-current");
            }
          });
        });
      },
      // A thin band near the top of the viewport decides which section is "current".
      { rootMargin: "-30% 0px -60% 0px", threshold: 0 }
    );
    sections.forEach(function (section) {
      spy.observe(section);
    });
  }

  // STEP 3 of 3: update the gold progress bar at the top as the page scrolls, at most once per frame.
  var ticking = false;
  function updateProgress() {
    var scrollable = root.scrollHeight - window.innerHeight;
    var progress = scrollable > 0 ? Math.min(1, Math.max(0, window.scrollY / scrollable)) : 0;
    root.style.setProperty("--progress", String(progress));
    ticking = false;
  }
  window.addEventListener(
    "scroll",
    function () {
      if (!ticking) {
        ticking = true;
        window.requestAnimationFrame(updateProgress);
      }
    },
    { passive: true }
  );
  updateProgress();
})();
