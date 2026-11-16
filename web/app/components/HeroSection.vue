<!--
  The first screen: portrait, name, title, the pitch, availability and the main calls to action, beside a code
  card that draws the same profile as a C# record, over an animated background of a looping video and
  floating technology tags (ADR-0008). It holds the page's only <h1>. The resume button renders only when
  there is a resume, so the site never shows a dead download link, and the portrait only when there is a
  portrait.
-->
<script setup>
const profile = content.profile
const iconFor = (label) => content.profileLinks.find((l) => l.label === label)?.iconMarkup
</script>

<template>
  <section id="top" class="hero" aria-labelledby="hero-heading">
    <CodeBackdrop />
    <HeroTags />
    <div class="container hero-grid">
      <div class="hero-copy">
        <div class="hero-identity">
          <!-- The portrait is above the fold, so it is neither lazy-loaded nor left without a size (avoids layout shift). A small WebP for browsers that support it; the 640 px JPEG is the fallback (and the link-preview image). -->
          <picture v-if="profile.photoUrl">
            <source v-if="profile.photoWebpUrl" :srcset="profile.photoWebpUrl" type="image/webp" />
            <img class="avatar" :src="profile.photoUrl" :alt="profile.photoAlt" width="160" height="160" decoding="async" fetchpriority="high" />
          </picture>
          <div class="hero-identity-text">
            <p class="hero-hi">{{ profile.greeting }}</p>
            <h1 id="hero-heading" class="hero-title">{{ profile.name }}</h1>
            <p class="eyebrow hero-role">{{ profile.title }}</p>
          </div>
        </div>
        <hr class="hero-rule" />
        <p class="hero-lead">{{ profile.headline }}</p>
        <p class="hero-meta">
          <span>{{ profile.location }}</span>
          <span aria-hidden="true"> &middot; </span>
          <span>{{ profile.availability }}</span>
        </p>
        <div class="hero-actions">
          <a class="button button-primary" href="#projects">See selected projects</a>
          <a class="button" :href="'mailto:' + profile.email">Email me</a>
          <a v-if="profile.resumeUrl" class="button button-resume" :href="profile.resumeUrl" download>Download resume (PDF)</a>
          <ExternalLink v-for="link in profile.links" :key="link.label" :link="link" css-class="button" :icon-markup="iconFor(link.label)" />
        </div>
      </div>
      <DeveloperRecord :profile="profile" />
    </div>
  </section>
</template>
