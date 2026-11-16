<!--
  The page shell shared by every page (ADR-0011, the successor of the old Razor _Layout):
  the animated background, the two hidden logo sprites, the skip link, the header and footer, and the
  single <main> landmark each page is drawn into. The head (title, description, sharing tags, the
  Content Security Policy) is set in nuxt.config.js for the parts every page shares and by each page for
  its own title.
-->
<script setup>
// The header's links stay in-page anchors on the home page, and point at the home page from the not-found page.
const route = useRoute()
const onHomePage = route.path === '/'
</script>

<template>
  <!-- Animated background fixed behind the whole page (code lanes and floating logos), so the motion covers every section as the visitor scrolls (ADR-0008). -->
  <CodeStreams variant="page" />
  <LogoSprite />
  <TechLogos variant="page" :slots="content.pageLogos" />
  <div class="scroll-progress" aria-hidden="true"><span></span></div>
  <a class="skip-link" href="#main">Skip to main content</a>
  <SiteHeader :home="onHomePage" />
  <main id="main">
    <NuxtPage />
  </main>
  <SiteFooter />
</template>
