<!--
  How to reach the owner, as a row of large link cards: email, then phone and messaging apps, then profiles.
  Email is a plain mailto: link, not a form, because a form would need a third-party service (ADR-0001). The
  numbers are shown publicly at the owner's request; each is a real tap-to-call or open-in-app link.
-->
<script setup>
const profile = content.profile

// The profile's label (GitHub, LinkedIn) picks the icon; the shown text is the address without "https://".
const linkCards = content.profileLinks.map((link) => ({
  label: link.label,
  iconMarkup: link.iconMarkup,
  link: { label: link.href.replace('https://', ''), href: link.href },
}))
</script>

<template>
  <section id="contact" class="section section-contact" aria-labelledby="contact-heading">
    <div class="container">
      <h2 id="contact-heading" class="section-title" data-reveal>Let's talk</h2>
      <p class="section-intro" data-reveal>{{ profile.availability }}</p>
      <ul class="contact-list">
        <li class="contact-card" data-reveal>
          <span class="contact-label">Email</span>
          <a class="contact-link" :href="'mailto:' + profile.email">{{ profile.email }}</a>
        </li>
        <li v-if="profile.resumeUrl" class="contact-card" data-reveal>
          <span class="contact-label">Resume</span>
          <a class="contact-link" :href="profile.resumeUrl" download>Download a copy (PDF)</a>
        </li>
        <li v-for="phone in profile.phones" :key="phone.label" class="contact-card" data-reveal>
          <span class="contact-label">{{ phone.label }}</span>
          <!-- An https link that opens a messaging app goes through the shared outbound-link component. -->
          <ExternalLink v-if="phone.href.startsWith('https://')" :link="{ label: phone.display, href: phone.href }" css-class="contact-link" />
          <a v-else class="contact-link" :href="phone.href">{{ phone.display }}</a>
          <a v-if="phone.actionLabel && phone.actionHref" class="contact-action" :href="phone.actionHref">{{ phone.actionLabel }}</a>
        </li>
        <li v-for="card in linkCards" :key="card.label" class="contact-card" data-reveal>
          <span class="contact-label">{{ card.label }}</span>
          <ExternalLink :link="card.link" css-class="contact-link" :icon-markup="card.iconMarkup" />
        </li>
        <li class="contact-card" data-reveal>
          <span class="contact-label">Location</span>
          <span class="contact-value">{{ profile.location }}</span>
        </li>
      </ul>
    </div>
  </section>
</template>
