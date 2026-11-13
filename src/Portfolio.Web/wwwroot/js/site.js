/**
 * Page behaviors (ADR-0008): scroll reveals, active-section highlight in the navigation, the hero video,
 * and a scroll-progress bar. Plain vanilla script, no third-party library, and every
 * behavior is an enhancement: with scripting off (or reduced motion on) the page is
 * fully visible and fully usable.
 */
(function () {
  "use strict";

  var root = document.documentElement;
  var reducedMotion = typeof window.matchMedia === "function" && window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // STEP 1 of 4: reveal elements as they scroll into view. Without IntersectionObserver, or when
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

  // STEP 2 of 4: mark the navigation link of the section currently in view with aria-current,
  // which the stylesheet turns into a gold underline. Sections without a nav link are ignored.
  var navLinks = Array.prototype.slice.call(document.querySelectorAll(".nav-list a[href*='#']"));
  var sections = navLinks
    .map(function (link) {
      return document.getElementById(link.getAttribute("href").split("#")[1]);
    })
    .filter(Boolean);

  var navList = document.querySelector(".nav-list");

  /**
   * On phones the navigation is one swipeable row, so keep the current link centered in it. This scrolls
   * only the row itself (never the page), and only when the row actually overflows.
   */
  function centerInNav(link) {
    if (!navList || navList.scrollWidth <= navList.clientWidth + 1) {
      return;
    }
    var left = link.offsetLeft - (navList.clientWidth - link.offsetWidth) / 2;
    navList.scrollTo({ left: left, behavior: reducedMotion ? "auto" : "smooth" });
  }

  if ("IntersectionObserver" in window && sections.length > 0) {
    var spy = new IntersectionObserver(
      function (entries) {
        entries.forEach(function (entry) {
          if (!entry.isIntersecting) {
            // A section that leaves the band stops being current. Without this, scrolling back up to the
            // hero (which has no navigation link) left the last link highlighted. A section that is
            // entering in the same batch has already replaced it, so only a still-marked link is cleared.
            navLinks.forEach(function (link) {
              if (link.getAttribute("href").split("#")[1] === entry.target.id) {
                link.removeAttribute("aria-current");
              }
            });
            return;
          }
          navLinks.forEach(function (link) {
            var isCurrent = link.getAttribute("href").split("#")[1] === entry.target.id;
            if (isCurrent) {
              link.setAttribute("aria-current", "true");
              centerInNav(link);
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

  // STEP 3 of 4: update the gold progress bar at the top as the page scrolls, at most once per frame.
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

  // STEP 4 of 4: play the hero video only when it is appropriate. The markup has no autoplay and
  // preload="none", so until this runs the browser has fetched only the poster. We skip playing when
  // the visitor prefers reduced motion, has asked to save data, or is on a phone-sized screen (where
  // the stylesheet hides the video anyway). It also pauses while the hero is out of view, so it uses
  // no processor time the rest of the page needs.
  var heroVideo = document.querySelector(".hero-video");
  var saveData = navigator.connection && navigator.connection.saveData === true;
  var wideScreen = typeof window.matchMedia === "function" && window.matchMedia("(min-width: 42rem)").matches;
  if (heroVideo && !reducedMotion && !saveData && wideScreen) {
    var playHero = function () {
      var attempt = heroVideo.play();
      if (attempt && typeof attempt.catch === "function") {
        attempt.catch(function () {
          // Autoplay can be refused; the poster simply stays, which is a fine result.
        });
      }
    };
    if ("IntersectionObserver" in window) {
      new IntersectionObserver(function (entries) {
        if (entries[0].isIntersecting) {
          playHero();
        } else {
          heroVideo.pause();
        }
      }).observe(heroVideo);
    } else {
      playHero();
    }
  }
})();
