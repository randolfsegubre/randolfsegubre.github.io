/**
 * Builds the hero background video: renders scene.html frame by frame in headless Chrome, then encodes
 * WebM (VP9), MP4 (H.264) and a still poster (WebP) into web/public/media.
 *
 * Usage (from this folder):  npm install   then   npm run build
 * Chrome is found through the CHROME_PATH environment variable, or the usual Windows location.
 *
 * STEP 1 of 3: render 288 frames (12 seconds at 24 per second) to a temporary folder.
 * STEP 2 of 3: encode the frames to WebM and MP4 with the bundled ffmpeg.
 * STEP 3 of 3: save one frame as the still poster, then delete the temporary frames.
 */
const fs = require('node:fs');
const path = require('node:path');
const { execFileSync } = require('node:child_process');
const puppeteer = require('puppeteer-core');
const ffmpeg = require('ffmpeg-static');

const FPS = 24;
const LOOP_SECONDS = 12; // must match LOOP in scene.html, or the video will not loop seamlessly
const POSTER_FRAME = 144; // the frame at 6 seconds
const chrome = process.env.CHROME_PATH || 'C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe';
const work = path.join(__dirname, '.work');
const media = path.resolve(__dirname, '..', '..', 'web', 'public', 'media');

async function renderFrames() {
  fs.rmSync(work, { recursive: true, force: true });
  fs.mkdirSync(work, { recursive: true });
  const browser = await puppeteer.launch({ executablePath: chrome, headless: 'new', args: ['--no-sandbox', '--force-color-profile=srgb'] });
  try {
    const page = await browser.newPage();
    await page.setViewport({ width: 1280, height: 720, deviceScaleFactor: 1 });
    await page.goto('file:///' + path.join(__dirname, 'scene.html').replace(/\\/g, '/'));
    await page.evaluate('document.fonts.ready');
    for (let i = 0; i < FPS * LOOP_SECONDS; i++) {
      // The scene is a pure function of time, so frame i is exactly the same on every run.
      const dataUrl = await page.evaluate((t) => { window.draw(t); return document.getElementById('c').toDataURL('image/png'); }, i / FPS);
      fs.writeFileSync(path.join(work, `f${String(i).padStart(4, '0')}.png`), Buffer.from(dataUrl.split(',')[1], 'base64'));
    }
  } finally {
    await browser.close();
  }
}

function encode() {
  fs.mkdirSync(media, { recursive: true });
  const input = ['-hide_banner', '-loglevel', 'error', '-y', '-framerate', String(FPS), '-i', path.join(work, 'f%04d.png')];
  execFileSync(ffmpeg, [...input, '-c:v', 'libvpx-vp9', '-b:v', '0', '-crf', '42', '-row-mt', '1', '-deadline', 'good', '-cpu-used', '2', '-pix_fmt', 'yuv420p', '-an', path.join(media, 'hero.webm')]);
  execFileSync(ffmpeg, [...input, '-c:v', 'libx264', '-profile:v', 'high', '-crf', '31', '-preset', 'slower', '-pix_fmt', 'yuv420p', '-movflags', '+faststart', '-an', path.join(media, 'hero.mp4')]);
}

function poster() {
  const frame = path.join(work, `f${String(POSTER_FRAME).padStart(4, '0')}.png`);
  execFileSync(ffmpeg, ['-hide_banner', '-loglevel', 'error', '-y', '-i', frame, '-c:v', 'libwebp', '-quality', '72', '-compression_level', '6', path.join(media, 'hero-poster.webp')]);
  fs.rmSync(work, { recursive: true, force: true });
}

(async () => {
  await renderFrames();
  encode();
  poster();
  for (const file of ['hero.webm', 'hero.mp4', 'hero-poster.webp']) {
    console.log(file, fs.statSync(path.join(media, file)).size, 'bytes');
  }
})().catch((error) => { console.error(error); process.exit(1); });
