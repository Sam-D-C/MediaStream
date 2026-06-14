<template>
  <div>
    <header class="page-header">
      <h1>Video's</h1>
      <p class="subtitle" v-if="videos.length">{{ videos.length }} video's</p>
    </header>

    <div v-if="loading" class="state-msg">Video's laden...</div>
    <div v-else-if="error" class="state-msg error">{{ error }}</div>
    <div v-else-if="videos.length === 0" class="state-msg">Geen video's gevonden.</div>

    <div v-if="activeVideo" class="player-wrap">
      <video
        :src="api.videoStreamUrl(activeVideo.id)"
        controls
        autoplay
        class="video-player"
      />
      <div class="video-title">{{ activeVideo.name }}</div>
      <button class="close-btn" @click="activeVideo = null">✕ Sluiten</button>
    </div>

    <div v-else class="video-grid">
      <div
        v-for="video in videos"
        :key="video.id"
        class="video-card"
        @click="play(video)"
      >
        <div class="video-thumb">
          <span class="play-icon">▶</span>
        </div>
        <div class="video-info">
          <div class="video-name">{{ video.name }}</div>
          <div class="video-size">{{ formatSize(video.fileSizeBytes) }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue'
import { api } from '../api.js'

const videos      = ref([])
const loading     = ref(true)
const error       = ref(null)
const activeVideo = ref(null)

function play(video) {
  activeVideo.value = video
}

function formatSize(bytes) {
  if (bytes > 1e9) return (bytes / 1e9).toFixed(1) + ' GB'
  return (bytes / 1e6).toFixed(0) + ' MB'
}

async function load() {
  loading.value = true
  error.value   = null
  try {
    videos.value = await api.getVideos()
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  load()
  window.addEventListener('media-refresh', load)
})

onUnmounted(() => {
  window.removeEventListener('media-refresh', load)
})
</script>