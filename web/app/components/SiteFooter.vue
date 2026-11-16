<!--
  Page footer: copyright and a pointer to this site's own source, itself a small, public example of the
  owner's work. The year is fixed when the site is built. The page-transition switch is rendered hidden and
  revealed by site.js, so it never shows as a dead button (ADR-0010).
-->
<script setup>
const source = { label: 'Source of this site', href: 'https://github.com/randolfsegubre/randolfsegubre.github.io' }
</script>

<template>
  <footer class="site-footer">
    <div class="container">
      <p>
        &copy; {{ content.meta.year }} {{ content.profile.name }}. Built with Nuxt and ASP.NET Core on .NET 10 and exported as a static site.
        <ExternalLink :link="source" />
      </p>
      <p class="footer-controls">
        <button type="button" class="footer-toggle" id="transition-toggle" aria-pressed="true" hidden>Page transitions: on</button>
      </p>
      <p class="footer-note">Technology logos are from the Simple Icons set (public domain), apart from the C# mark, which is drawn for this site. All trademarks belong to their owners.</p>
    </div>
  </footer>
</template>
