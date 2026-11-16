<!--
  The not-found page. The static build saves it as 404.html, which GitHub Pages serves for any unknown
  address. It must never compete with the real page in search results, so it is marked noindex and has no
  canonical link.
-->
<script setup>
const meta = content.meta

useHead({
  title: meta.notFoundTitle,
  meta: [
    { name: 'description', content: meta.description },
    { property: 'og:type', content: 'website' },
    { property: 'og:title', content: meta.notFoundTitle },
    { property: 'og:description', content: meta.shareDescription },
    { property: 'og:url', content: meta.siteUrl + '/' },
    { name: 'robots', content: 'noindex' },
    { name: 'twitter:title', content: meta.notFoundTitle },
    { name: 'twitter:description', content: meta.shareDescription },
  ],
})
</script>

<template>
  <section class="section" aria-labelledby="notfound-heading">
    <div class="container">
      <h1 id="notfound-heading" class="section-title">That page does not exist</h1>
      <p class="section-intro">This site is a single page. <a href="/">Go to the portfolio</a>.</p>
    </div>
  </section>
</template>
