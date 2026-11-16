<!--
  The hero's animated background (ADR-0008, amended 2026-11-14): a looping video of the owner's own stack at
  work, with a few quiet technology logos over it. It is deliberately a different surface from the faint
  code-and-logo layer fixed behind the rest of the page (see app.vue), so the hero reads as its own banner.
  Purely decorative, so it is hidden from assistive technology and never takes pointer events.

  The video: the page script plays it only on wide screens, with motion allowed and data saving off;
  otherwise a still frame of the same scene stays (hero-poster.webp, set in the stylesheet so phones, which
  hide the video, never fetch it). There is no autoplay attribute and preload is "none", so no video bytes are
  fetched until the script decides. Files: web/public/media; how they are made: tools/hero-video/README.md.
-->
<template>
  <div class="code-backdrop" aria-hidden="true">
    <video class="hero-video" muted loop playsinline preload="none" width="1280" height="720" disablepictureinpicture disableremoteplayback tabindex="-1">
      <source src="/media/hero.webm" type="video/webm" />
      <source src="/media/hero.mp4" type="video/mp4" />
    </video>
    <TechLogos variant="hero" :slots="content.heroLogos" />
  </div>
</template>
