<!--
  The whole page, top to bottom. This page only decides the order of sections and sets the head; each
  component draws just its slice of the content, and none holds a claim or a piece of copy (ADR-0003).
-->
<script setup>
const meta = content.meta

// Sharing tags (link previews in email, LinkedIn and chat apps show the portrait; the address must be absolute).
const share = meta.shareImageUrl
  ? [
      { property: 'og:image', content: meta.shareImageUrl },
      { property: 'og:image:width', content: '640' },
      { property: 'og:image:height', content: '640' },
      { property: 'og:image:alt', content: meta.shareImageAlt },
      { name: 'twitter:card', content: 'summary' },
    ]
  : []

useHead({
  title: meta.homeTitle,
  meta: [
    { name: 'description', content: meta.description },
    { property: 'og:type', content: 'website' },
    { property: 'og:title', content: meta.homeTitle },
    { property: 'og:description', content: meta.shareDescription },
    { property: 'og:url', content: meta.siteUrl + '/' },
    { name: 'twitter:title', content: meta.homeTitle },
    { name: 'twitter:description', content: meta.shareDescription },
    ...share,
  ],
  link: [{ rel: 'canonical', href: meta.siteUrl + '/' }],
})
</script>

<template>
  <HeroSection />
  <StatsStrip />
  <AboutSection />
  <LiveSitesSection />
  <ProjectsSection />
  <ExperienceSection />
  <ContactSection />
</template>
