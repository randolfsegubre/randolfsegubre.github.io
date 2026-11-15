/**
 * Page behaviors (ADR-0008, ADR-0010): scroll reveals, active-section highlight in the navigation, the hero video,
 * a scroll-progress bar, the interactive skills star map, letter-by-letter headings and the page-change veil. Plain vanilla script, no third-party library, and every
 * behavior is an enhancement: with scripting off (or reduced motion on) the page is
 * fully visible and fully usable.
 */
(function () {
  "use strict";

  var root = document.documentElement;
  var reducedMotion = typeof window.matchMedia === "function" && window.matchMedia("(prefers-reduced-motion: reduce)").matches;

  // STEP 1 of 7: reveal elements as they scroll into view. Without IntersectionObserver, or when
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

  // STEP 2 of 7: mark the navigation link of the section currently in view with aria-current,
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

  // STEP 3 of 7: update the gold progress bar at the top as the page scrolls, at most once per frame.
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

  // STEP 4 of 7: play the hero video only when it is appropriate. The markup has no autoplay and
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

  // STEP 5 of 7: the skills sphere cloud (ADR-0010). The markup is complete without script: every star is
  // in the page and every caption is listed (the map itself stays hidden until script runs). Here the markup becomes a
  // 3D cloud with no library: each glossy sphere (a logo with its name underneath) sits on one of three shells around .NET,
  // at the place the server worked out for it (data-azimuth, data-elevation, data-radius), the cloud turns slowly, every
  // sphere floats a little, near spheres are large and bright and far ones small and faint. The visitor can drag or swipe it,
  // use the arrow keys or the Turn buttons. A word lights its connections and shows its caption; choosing one
  // (or walking the journey) turns it to the front. Under reduced motion nothing moves by itself: no spin, no
  // floating, and the camera jumps instead of gliding, but the visitor can still turn it.
  var universe = document.querySelector("[data-universe]");
  if (universe) {
    var mapBox = universe.querySelector(".universe-map");
    var scroller = universe.querySelector(".universe-scroll");
    var drawing = universe.querySelector(".universe-lines");
    var stars = Array.prototype.slice.call(universe.querySelectorAll("[data-star]"));
    var lines = Array.prototype.slice.call(universe.querySelectorAll("[data-link-from]"));
    var captions = {};
    universe.querySelectorAll("[data-caption]").forEach(function (item) {
      captions[item.getAttribute("data-caption")] = item;
    });
    var journey = (universe.getAttribute("data-journey") || "").split(",").filter(Boolean);
    var walkButton = universe.querySelector("[data-universe-walk]");
    var spinButton = universe.querySelector("[data-universe-spin]");
    var turnButtons = Array.prototype.slice.call(universe.querySelectorAll("[data-universe-turn]"));

    var SVG_NS = "http://www.w3.org/2000/svg";
    // The cloud is drawn on a square canvas that fits its box, so it needs no sideways scrolling on a phone.
    var MAP_W = 1000;
    var MAP_H = 1000;
    drawing.setAttribute("viewBox", "0 0 " + MAP_W + " " + MAP_H);
    var FOCAL = 3200; // camera distance: near spheres are noticeably larger than far ones
    var ZOOM = 0.7; // scales the outer shell (600 world units) to fill the box
    var OUTER = 1; // the outer sphere's radius, read from the stars below

    // STEP 5a: one body per sphere. The server has already placed each star evenly on one of three spheres around
    // .NET (data-azimuth, data-elevation and data-radius), so the script only has to project it. Each body gets its own
    // floating phase so the spheres never bob in step.
    var bodies = stars.map(function (el, index) {
      return {
        id: el.getAttribute("data-star"),
        el: el,
        depth: Number(el.getAttribute("data-depth")),
        theta: (Number(el.getAttribute("data-azimuth")) * Math.PI) / 180,
        elevation: (Number(el.getAttribute("data-elevation")) * Math.PI) / 180,
        radius: Number(el.getAttribute("data-radius")),
        phase: index * 1.7,
        x: 0, y: 0, z: 0, scale: 1
      };
    });
    OUTER = Math.max.apply(null, bodies.map(function (body) { return body.radius; }));
    var bodyById = {};
    bodies.forEach(function (body) {
      bodyById[body.id] = body;
    });

    // STEP 5b: add a little drifting dust around the cloud so the rotation reads as depth.
    var dust = [];
    for (var d = 0; d < 40; d += 1) {
      var spin = Math.sin(d * 12.9898) * 43758.5453;
      var u = spin - Math.floor(spin);
      var v = Math.sin(d * 78.233) * 12345.678;
      v = v - Math.floor(v);
      var azimuth = u * Math.PI * 2;
      var height = v * 2 - 1;
      var shell = OUTER * (0.9 + ((d * 7) % 10) / 12);
      var dot = document.createElementNS(SVG_NS, "circle");
      dot.setAttribute("class", "universe-dust");
      drawing.insertBefore(dot, drawing.firstChild);
      dust.push({
        el: dot,
        x: Math.cos(azimuth) * Math.sqrt(1 - height * height) * shell,
        y: height * shell,
        z: Math.sin(azimuth) * Math.sqrt(1 - height * height) * shell,
        size: d % 4 === 0 ? 2.2 : 1.3
      });
    }

    // STEP 5c: the camera and the projection. Yaw turns the cloud about the vertical axis, pitch tips it toward
    // or away from the viewer, then a perspective divides by distance.
    var yaw = 0.4;
    var pitch = 0.65; // looking down at about 37 degrees, so the shells spread out around the centre instead of lining up behind it
    var PITCH_MIN = 0.15;
    var PITCH_MAX = 1.2;
    var targetYaw = null;
    var velocity = 0;
    var spinning = !reducedMotion;
    var hovering = false;
    var focusedInside = false;
    var dragging = false;
    var walkTimer = null;
    var walkIndex = 0;
    var activeId = null;
    var onScreen = true;

    var project = function (x, y, z) {
      var cosY = Math.cos(yaw);
      var sinY = Math.sin(yaw);
      var xr = x * cosY + z * sinY;
      var zr = -x * sinY + z * cosY;
      var cosP = Math.cos(pitch);
      var sinP = Math.sin(pitch);
      var up = y * cosP - zr * sinP;
      var depth = y * sinP + zr * cosP;
      var s = FOCAL / (FOCAL - depth);
      return { x: MAP_W / 2 + xr * s * ZOOM, y: MAP_H / 2 - up * s * ZOOM, z: depth, s: s };
    };
    var clamp = function (value, low, high) {
      return Math.min(high, Math.max(low, value));
    };
    var wrap = function (angle) {
      return Math.atan2(Math.sin(angle), Math.cos(angle));
    };

    // Sphere and name sizes follow the cloud box, so a phone gets smaller spheres rather than a crowd. Each level has its
    // own size, as in a tag cloud: the centre is largest, then the first ring, and so on outward.
    // The centre is largest, then the main stack (the stars the server marks star-main); every other technology is smaller,
    // and gets a little smaller again the further out it sits.
    var MAIN_ORB = 4.8;
    var MAIN_NAME = 0.92;
    var ORB_SIZES = [6.2, 3.3, 3.0, 2.7, 2.5];
    var NAME_SIZES = [1.05, 0.8, 0.76, 0.72, 0.7];
    var fitWords = function () {
      var width = mapBox.getBoundingClientRect().width || 900;
      var scale = clamp(width / 900, 0.6, 1.15);
      ZOOM = width < 480 ? 0.62 : 0.7; // a little less magnification on a phone, so the outer spheres keep clear of the edge
      bodies.forEach(function (body) {
        var isMain = body.el.classList.contains("star-main");
        body.el.style.setProperty("--orb", ((isMain ? MAIN_ORB : ORB_SIZES[body.depth]) * scale).toFixed(3) + "rem");
        // Names never go below a readable size, even on a phone.
        body.el.style.setProperty("--name", Math.max(0.64, (isMain ? MAIN_NAME : NAME_SIZES[body.depth]) * scale).toFixed(3) + "rem");
      });
    };
    fitWords();
    if ("ResizeObserver" in window) {
      new ResizeObserver(fitWords).observe(mapBox);
    }

    // STEP 5d: draw one frame. Positions are written as percentages of the map so they scale with it.
    var near = {};
    var draw = function (seconds) {
      var floating = !reducedMotion;
      bodies.forEach(function (body) {
        var bob = floating ? Math.sin(seconds * 0.9 + body.phase) * (body.depth === 0 ? 3 : 9) : 0;
        var driftX = floating ? Math.sin(seconds * 0.55 + body.phase * 1.3) * 6 : 0;
        var driftZ = floating ? Math.cos(seconds * 0.5 + body.phase) * 6 : 0;
        var flat = body.radius * Math.cos(body.elevation);
        var p = project(
          Math.cos(body.theta) * flat + driftX,
          body.radius * Math.sin(body.elevation) + bob,
          Math.sin(body.theta) * flat + driftZ
        );
        body.x = p.x;
        body.y = p.y;
        body.z = p.z;
        body.scale = clamp(p.s, 0.72, 1.3);
        var emphasis = body.id === activeId ? 1.12 : 1;
        var far = clamp((p.z + OUTER) / (2 * OUTER), 0, 1);
        var dimmed = activeId !== null && body.id !== activeId && !near[body.id];
        body.el.style.left = (p.x / MAP_W) * 100 + "%";
        body.el.style.top = (p.y / MAP_H) * 100 + "%";
        body.el.style.transform = "translate(-50%, -50%) scale(" + (body.scale * emphasis).toFixed(3) + ")";
        // Near words are bright, far words fade almost out, as in a tag cloud. The centre never fades.
        body.el.style.opacity = body.depth === 0 ? "1" : ((0.2 + 0.8 * far) * (dimmed ? 0.7 : 1)).toFixed(2);
        // With this many spheres the names of the far ones would pile up behind the near ones, so they fade out first;
        // the chosen sphere and the ones it connects to always keep their name.
        var nameVisible = body.depth === 0 || body.id === activeId || near[body.id] ? 1 : clamp((far - 0.4) / 0.25, 0, 1);
        body.el.style.setProperty("--name-opacity", nameVisible.toFixed(2));
        body.el.style.zIndex = String(Math.round(p.z + OUTER + 10) + (body.id === activeId ? 1000 : 0));
      });
      lines.forEach(function (line) {
        var from = bodyById[line.getAttribute("data-link-from")];
        var to = bodyById[line.getAttribute("data-link-to")];
        line.setAttribute("x1", from.x.toFixed(1));
        line.setAttribute("y1", from.y.toFixed(1));
        line.setAttribute("x2", to.x.toFixed(1));
        line.setAttribute("y2", to.y.toFixed(1));
      });
      dust.forEach(function (mote) {
        var p = project(mote.x, mote.y, mote.z);
        mote.el.setAttribute("cx", p.x.toFixed(1));
        mote.el.setAttribute("cy", p.y.toFixed(1));
        mote.el.setAttribute("r", (mote.size * clamp(p.s, 0.7, 1.3)).toFixed(2));
        mote.el.style.opacity = (0.1 + 0.4 * clamp((p.z + OUTER * 1.6) / (OUTER * 3.2), 0, 1)).toFixed(2);
      });
    };

    // STEP 5e: the animation loop. It runs only while the cloud is on screen and the tab is visible, so a
    // visitor reading elsewhere on the page costs no processor time. Under reduced motion it draws only
    // when something changed.
    var frameRequest = null;
    var lastStamp = null;
    var motionClock = 0; // seconds of floating so far; it stops advancing while the visitor has paused the motion
    var tick = function (stamp) {
      frameRequest = null;
      var step = lastStamp === null ? 0 : Math.min(0.05, (stamp - lastStamp) / 1000);
      lastStamp = stamp;
      if (!dragging) {
        if (targetYaw !== null) {
          var gap = wrap(targetYaw - yaw);
          yaw += gap * (1 - Math.exp(-step * 3.2));
          if (Math.abs(gap) < 0.002) {
            yaw = targetYaw;
            targetYaw = null;
          }
        } else if (spinning && !hovering && !focusedInside && walkTimer === null) {
          yaw += step * 0.09;
        }
        if (Math.abs(velocity) > 0.001) {
          yaw += velocity * step;
          velocity *= Math.exp(-step * 3);
        } else {
          velocity = 0;
        }
      }
      if (spinning && !reducedMotion) {
        motionClock += step;
      }
      draw(motionClock);
      var keepGoing = onScreen && !document.hidden && (!reducedMotion || targetYaw !== null || Math.abs(velocity) > 0.001);
      if (keepGoing) {
        frameRequest = window.requestAnimationFrame(tick);
      } else {
        lastStamp = null;
      }
    };
    var wake = function () {
      if (frameRequest === null) {
        frameRequest = window.requestAnimationFrame(tick);
      }
    };
    if ("IntersectionObserver" in window) {
      new IntersectionObserver(function (entries) {
        onScreen = entries[0].isIntersecting;
        if (onScreen) {
          wake();
        }
      }).observe(mapBox);
    }
    document.addEventListener("visibilitychange", function () {
      if (!document.hidden) {
        wake();
      }
    });

    // STEP 5f: choosing a word. It lights its own connections (lines appear only for the chosen word) and
    // shows its caption in a live region that replaces the caption list; the camera may also turn it to the front.
    var live = document.createElement("div");
    live.className = "universe-live";
    live.setAttribute("aria-live", "polite");
    universe.insertBefore(live, universe.querySelector(".universe-captions"));
    universe.classList.add("is-interactive", "is-3d");
    var starList = universe.querySelector(".universe-stars");
    if (starList) {
      starList.setAttribute("role", "group");
    }

    var activate = function (id) {
      activeId = id;
      near = {};
      lines.forEach(function (line) {
        var from = line.getAttribute("data-link-from");
        var to = line.getAttribute("data-link-to");
        var hot = from === id || to === id;
        line.classList.toggle("is-hot", hot);
        if (hot) {
          near[from] = true;
          near[to] = true;
        }
      });
      stars.forEach(function (star) {
        var starId = star.getAttribute("data-star");
        star.classList.toggle("is-active", starId === id);
        star.classList.toggle("is-near", starId !== id && near[starId] === true);
        star.setAttribute("aria-pressed", starId === id ? "true" : "false");
      });
      universe.classList.add("has-focus");
      var caption = captions[id];
      if (caption) {
        live.textContent = "";
        var title = document.createElement("strong");
        title.textContent = caption.querySelector("strong").textContent;
        live.appendChild(title);
        live.appendChild(document.createTextNode(caption.textContent.slice(title.textContent.length).trim()));
      }
      wake();
    };
    var turnTo = function (angle) {
      targetYaw = angle;
      if (reducedMotion) {
        yaw = angle;
        targetYaw = null;
      }
      wake();
    };
    var bringToFront = function (id) {
      var body = bodyById[id];
      if (body && body.depth > 0) {
        // A word faces the viewer when its angle, after turning, is a quarter turn: yaw = angle - 90 degrees.
        turnTo(yaw + wrap(body.theta - Math.PI / 2 - yaw));
      }
    };

    var stopWalk = function () {
      if (walkTimer !== null) {
        window.clearInterval(walkTimer);
        walkTimer = null;
      }
      if (walkButton) {
        walkButton.textContent = "Walk my journey";
      }
    };

    stars.forEach(function (star) {
      var id = star.getAttribute("data-star");
      star.setAttribute("role", "button");
      star.setAttribute("tabindex", "0");
      star.setAttribute("aria-pressed", "false");
      star.addEventListener("mouseenter", function () {
        stopWalk();
        activate(id);
      });
      star.addEventListener("focus", function () {
        activate(id);
        // Keyboard users cannot drag, so a word they tab to is turned to the front where it can be seen.
        if (star.matches(":focus-visible")) {
          bringToFront(id);
        }
      });
      star.addEventListener("click", function () {
        stopWalk();
        activate(id);
        bringToFront(id);
      });
      star.addEventListener("keydown", function (event) {
        if (event.key === "Enter" || event.key === " ") {
          event.preventDefault();
          stopWalk();
          activate(id);
          bringToFront(id);
        }
      });
    });

    if (walkButton) {
      walkButton.hidden = false;
      walkButton.addEventListener("click", function () {
        if (walkTimer !== null) {
          stopWalk();
          return;
        }
        walkIndex = 0;
        activate(journey[0]);
        live.setAttribute("aria-live", "off");
        walkButton.textContent = "Stop";
        walkTimer = window.setInterval(function () {
          walkIndex += 1;
          if (walkIndex >= journey.length) {
            stopWalk();
            live.setAttribute("aria-live", "polite");
            return;
          }
          activate(journey[walkIndex]);
          bringToFront(journey[walkIndex]);
        }, 1800);
      });
    }

    // STEP 5g: turning it by hand. A mouse, pen or finger drags the cloud (the stylesheet lets a vertical swipe
    // still scroll the page); arrow keys work anywhere in the cloud, and the Turn and Pause buttons give
    // everyone else the same control.
    var pressX = 0;
    var pressY = 0;
    var pressing = false;
    var moved = false;
    var lastMoveStamp = 0;
    mapBox.addEventListener("pointerdown", function (event) {
      if (event.button !== 0) {
        return;
      }
      pressing = true;
      moved = false;
      pressX = event.clientX;
      pressY = event.clientY;
      lastMoveStamp = event.timeStamp;
    });
    mapBox.addEventListener("pointermove", function (event) {
      if (!pressing) {
        return;
      }
      var dx = event.clientX - pressX;
      var dy = event.clientY - pressY;
      if (!dragging && Math.abs(dx) + Math.abs(dy) < 5) {
        return;
      }
      if (!dragging) {
        dragging = true;
        moved = true;
        targetYaw = null;
        mapBox.setPointerCapture(event.pointerId);
        universe.classList.add("is-dragging");
      }
      var span = Math.max(1, event.timeStamp - lastMoveStamp) / 1000;
      yaw += dx * 0.007;
      pitch = clamp(pitch + dy * 0.004, PITCH_MIN, PITCH_MAX);
      velocity = (dx * 0.007) / span;
      lastMoveStamp = event.timeStamp;
      pressX = event.clientX;
      pressY = event.clientY;
      wake();
    });
    var release = function () {
      pressing = false;
      if (dragging) {
        dragging = false;
        universe.classList.remove("is-dragging");
        velocity = reducedMotion ? 0 : clamp(velocity, -2.5, 2.5);
        // The browser sends a click right after the release; if none comes, the next real click must still count.
        window.setTimeout(function () {
          moved = false;
        }, 0);
        wake();
      }
    };
    mapBox.addEventListener("pointerup", release);
    mapBox.addEventListener("pointercancel", release);
    // A drag that ends on a word must not also count as choosing it.
    mapBox.addEventListener(
      "click",
      function (event) {
        if (moved) {
          event.stopPropagation();
          event.preventDefault();
          moved = false;
        }
      },
      true
    );
    mapBox.addEventListener("pointerenter", function (event) {
      hovering = event.pointerType !== "touch";
    });
    mapBox.addEventListener("pointerleave", function () {
      hovering = false;
      wake();
    });
    // Only a focused word holds the spin still (so its label stays put); a focused button does not.
    universe.addEventListener("focusin", function (event) {
      focusedInside = event.target.hasAttribute && event.target.hasAttribute("data-star");
    });
    universe.addEventListener("focusout", function () {
      focusedInside = false;
      wake();
    });
    universe.addEventListener("keydown", function (event) {
      var turn = { ArrowLeft: -0.14, ArrowRight: 0.14 }[event.key];
      var tilt = { ArrowUp: -0.08, ArrowDown: 0.08 }[event.key];
      if (turn === undefined && tilt === undefined) {
        return;
      }
      event.preventDefault();
      targetYaw = null;
      yaw += turn || 0;
      pitch = clamp(pitch + (tilt || 0), PITCH_MIN, PITCH_MAX);
      wake();
    });

    turnButtons.forEach(function (button) {
      button.hidden = false;
      button.addEventListener("click", function () {
        turnTo(yaw + Number(button.getAttribute("data-universe-turn")) * 0.6);
      });
    });
    if (spinButton) {
      spinButton.hidden = reducedMotion; // nothing spins by itself under reduced motion, so there is nothing to pause
      spinButton.addEventListener("click", function () {
        spinning = !spinning;
        spinButton.textContent = spinning ? "Pause rotation" : "Resume rotation";
        wake();
      });
    }

    // Start on the centre.
    activate("dotnet");
    draw(0);
    wake();
  }

  // STEP 6 of 7: letter-by-letter headings (ADR-0010). The hero name and each section title are split into
  // words and letters (words stay in one piece so a heading never breaks mid-word), then the letters glow
  // gold and settle one after another. Skipped for visitors who prefer reduced motion, and the full text is
  // kept as the heading's accessible name so a screen reader still reads it as one phrase.
  if (!reducedMotion) {
    var letterHeadings = Array.prototype.slice.call(document.querySelectorAll(".hero-title, .section-title"));
    letterHeadings.forEach(function (heading) {
      var text = heading.textContent.trim();
      var index = 0;
      heading.setAttribute("aria-label", text);
      heading.textContent = "";
      text.split(" ").forEach(function (wordText, wordIndex) {
        if (wordIndex > 0) {
          heading.appendChild(document.createTextNode(" "));
        }
        var word = document.createElement("span");
        word.className = "word";
        word.setAttribute("aria-hidden", "true");
        wordText.split("").forEach(function (letter) {
          var ch = document.createElement("span");
          ch.className = "ch";
          ch.style.setProperty("--i", String(index));
          ch.textContent = letter;
          word.appendChild(ch);
          index += 1;
        });
        heading.appendChild(word);
      });
      heading.classList.add("letters");
    });

    var settle = function (heading) {
      heading.classList.add("is-in");
    };
    // The hero name settles on load; each section title settles as it scrolls into view.
    window.requestAnimationFrame(function () {
      window.requestAnimationFrame(function () {
        letterHeadings.filter(function (h) { return h.classList.contains("hero-title"); }).forEach(settle);
      });
    });
    if ("IntersectionObserver" in window) {
      var letterObserver = new IntersectionObserver(
        function (entries) {
          entries.forEach(function (entry) {
            if (entry.isIntersecting) {
              settle(entry.target);
              letterObserver.unobserve(entry.target);
            }
          });
        },
        { rootMargin: "0px 0px -8% 0px", threshold: 0.2 }
      );
      letterHeadings.filter(function (h) { return !h.classList.contains("hero-title"); }).forEach(function (h) {
        letterObserver.observe(h);
      });
    } else {
      letterHeadings.forEach(settle);
    }
  }

  // STEP 7 of 7: the page-change veil (ADR-0010). Following a link to another section fades the page to the
  // RS monogram and a thin loading line for under a second, jumps while the page is covered, then fades back.
  // It never runs for visitors who prefer reduced motion or have asked to save data, a footer switch turns it
  // off for everyone else (and is remembered), and a link to a place already on screen, a skip link, or a
  // click with a modifier key (open in a new tab) is left alone. Without script a link just scrolls.
  var transitionSwitch = document.getElementById("transition-toggle");
  var transitionsAllowed = !reducedMotion && !saveData;
  var transitionsOn = transitionsAllowed;
  try {
    if (window.localStorage.getItem("transitions") === "off") {
      transitionsOn = false;
    }
  } catch (storageError) {
    // Storage can be blocked; the default stays.
  }

  var renderSwitch = function () {
    if (transitionSwitch) {
      transitionSwitch.setAttribute("aria-pressed", String(transitionsOn));
      transitionSwitch.textContent = "Page transitions: " + (transitionsOn ? "on" : "off");
    }
  };
  if (transitionSwitch && transitionsAllowed) {
    transitionSwitch.hidden = false;
    renderSwitch();
    transitionSwitch.addEventListener("click", function () {
      transitionsOn = !transitionsOn;
      try {
        window.localStorage.setItem("transitions", transitionsOn ? "on" : "off");
      } catch (storageError) {
        // Not remembering the choice is acceptable.
      }
      renderSwitch();
    });
  }

  var veil = null;
  var veilBusy = false;
  var buildVeil = function () {
    veil = document.createElement("div");
    veil.className = "veil";
    veil.setAttribute("aria-hidden", "true");
    var mark = document.createElement("span");
    mark.className = "veil-mark";
    mark.textContent = "RS";
    var line = document.createElement("span");
    line.className = "veil-line";
    veil.appendChild(mark);
    veil.appendChild(line);
    document.body.appendChild(veil);
  };

  document.addEventListener("click", function (event) {
    if (!transitionsAllowed || !transitionsOn || veilBusy || event.defaultPrevented) {
      return;
    }
    if (event.button !== 0 || event.metaKey || event.ctrlKey || event.shiftKey || event.altKey) {
      return;
    }
    var link = event.target.closest ? event.target.closest("a[href^='#']") : null;
    if (!link || link.classList.contains("skip-link")) {
      return;
    }
    var id = link.getAttribute("href").slice(1);
    var target = id ? document.getElementById(id) : null;
    if (!target || Math.abs(target.getBoundingClientRect().top) < 120) {
      return;
    }

    event.preventDefault();
    veilBusy = true;
    if (veil === null) {
      buildVeil();
    }
    veil.classList.add("is-on");
    window.setTimeout(function () {
      // Jump while the page is covered: no smooth scroll, then keep the address and the keyboard focus in step.
      root.style.scrollBehavior = "auto";
      target.scrollIntoView();
      root.style.scrollBehavior = "";
      try {
        window.history.pushState(null, "", "#" + id);
      } catch (historyError) {
        // The address bar simply stays as it was.
      }
      if (!target.hasAttribute("tabindex")) {
        target.setAttribute("tabindex", "-1");
      }
      target.focus({ preventScroll: true });
      veil.classList.remove("is-on");
      window.setTimeout(function () {
        veilBusy = false;
      }, 300);
    }, 550);
  });
})();
