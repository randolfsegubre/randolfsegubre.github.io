<!--
  The hero's signature piece: the owner's profile written as a C# record, drawn from the same profile data as
  the rest of the page so it can never disagree with it (ADR-0003, ADR-0008). It repeats information already
  in the hero, so it is hidden from assistive technology to avoid reading everything twice.

  The code sits inside <pre>, where every space and line break is kept as written.
-->
<script setup>
defineProps({
  profile: { type: Object, required: true },
})
</script>

<template>
  <div class="code-card" aria-hidden="true">
    <div class="code-card-bar">
      <span class="code-dot code-dot-gold"></span>
      <span class="code-dot code-dot-orchid"></span>
      <span class="code-dot code-dot-violet"></span>
      <span class="code-card-file">Developer.cs</span>
    </div>
<pre class="code-card-body"><code><span class="tok-k">public sealed record</span> <span class="tok-t">Developer</span>
{
    <span class="tok-k">public string</span> Name  <span class="tok-o">=&gt;</span> <span class="tok-s">"{{ profile.name }}"</span>;
    <span class="tok-k">public string</span> Title <span class="tok-o">=&gt;</span> <span class="tok-s">"{{ profile.title }}"</span>;
    <span class="tok-k">public string</span> Based <span class="tok-o">=&gt;</span> <span class="tok-s">"{{ profile.location }}"</span>;

    <span class="tok-k">public string</span>[] Stack <span class="tok-o">=&gt;</span> [<template v-for="(item, i) in profile.signatureStack" :key="item"><span class="tok-s">"{{ item }}"</span>{{ i &lt; profile.signatureStack.length - 1 ? ', ' : '' }}</template>];

    <span class="tok-c">// {{ profile.availability }}</span>
}</code></pre>
  </div>
</template>
