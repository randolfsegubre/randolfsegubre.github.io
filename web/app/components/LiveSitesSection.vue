<!--
  Live public websites the owner contributed to as a developer (client work). This is the strongest proof on
  the page, because any visitor can open the sites. Only public information is shown: names, public addresses
  and his plain-language part. No internal repository names, ticket numbers or hosting details (ADR-0006).
-->
<template>
  <section id="live-sites" class="section" aria-labelledby="live-sites-heading">
    <div class="container">
      <h2 id="live-sites-heading" class="section-title" data-reveal>Live sites I contributed to</h2>
      <p class="section-intro" data-reveal>Public websites I worked on as a developer at Hotelplan UK, client work through Cloud Employee. Anyone can open them. I describe only what a visitor can see, never code or internal detail.</p>
      <div class="grid grid-wide">
        <article v-for="site in content.liveSites" :id="site.id" :key="site.id" class="card card-featured" :aria-labelledby="site.id + '-name'" data-reveal>
          <header class="card-header">
            <h3 :id="site.id + '-name'" class="card-title">{{ site.name }}</h3>
            <span class="badge badge-live">Live site</span>
          </header>
          <p class="live-address"><ExternalLink :link="{ label: site.address, href: site.href }" /></p>
          <p>{{ site.summary }}</p>
          <p class="card-role"><strong>My part:</strong> {{ site.role }}</p>
          <ul class="highlights">
            <li v-for="highlight in site.highlights" :key="highlight">{{ highlight }}</li>
          </ul>
          <p class="honest-note"><strong>Worth knowing:</strong> {{ site.honestNote }}</p>
          <TagList :items="site.stack" :label="site.name + ' technologies'" />
        </article>
      </div>
    </div>
  </section>
</template>
