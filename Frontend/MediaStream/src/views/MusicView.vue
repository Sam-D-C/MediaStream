<template>
    <ContextMenu ref="contextMenu" />
  <div>
    <header class="page-header">
      <h1>Muziekbibliotheek</h1>
      <p class="subtitle" v-if="songs.length">{{ songs.length }} nummers</p>
    </header>

    <div class="search-wrap">
      <input
        v-model="search"
        type="text"
        placeholder="Zoek op titel of artiest..."
        class="search-input"
      />
    </div>

    <div v-if="loading" class="state-msg">Bibliotheek laden...</div>
    <div v-else-if="error" class="state-msg error">{{ error }}</div>
    <div v-else-if="songs.length === 0" class="state-msg">Geen nummers gevonden.</div>

    <div v-else class="song-list">
<div
  v-for="song in filteredSongs"
  :key="song.id"
  class="song-row"
  :class="{ active: currentId === song.id }"
  @click="play(song)"
  @contextmenu="(e) => contextMenu.open(e, song)"
>
  <div class="song-cover">
    <img
      v-if="song.hasCover"
      :src="api.songCoverUrl(song.id)"
      :alt="song.name"
      loading="lazy"
      @error="() => song.hasCover = false"
    />
    <div v-else class="cover-ph">♪</div>
    <div class="play-overlay">{{ currentId === song.id ? '⏸' : '▶' }}</div>
  </div>

  <div class="song-meta">
    <div class="song-name">{{ song.name }}</div>
    <div class="song-artist">{{ song.artist !== 'Unknown' ? song.artist : '' }}</div>
  </div>

  <div class="song-dur">{{ formatDur(song.durationSeconds) }}</div>

  <div class="song-btns" @click.stop>
    <button class="song-action-btn" title="Toevoegen aan wachtrij" @click="onAddToQueue(song)">⏭</button>
    <button class="song-action-btn" title="Toevoegen aan playlist" @click="contextMenu.openPlaylistOnly($event, song)">≡</button>
  </div>
</div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, inject, onMounted, onUnmounted } from 'vue'
import { api } from '../api.js'
import ContextMenu from '../components/ContextMenu.vue'
const contextMenu = ref(null)

const songs     = ref([])
const loading   = ref(true)
const error     = ref(null)
const search    = ref('')
const currentId = ref(null)

const playSong = inject('playSong')
const addToQueue = inject('addToQueue')

function onAddToQueue(song) {
  addToQueue(song)
}

const filteredSongs = computed(() => {
  const q = search.value.toLowerCase()
  if (!q) return songs.value
  return songs.value.filter(s =>
    s.name.toLowerCase().includes(q) ||
    s.artist.toLowerCase().includes(q)
  )
})

function play(song) {
  currentId.value = song.id
  playSong(song)
}

function formatDur(secs) {
  if (!secs) return '--:--'
  const m = Math.floor(secs / 60)
  const s = (secs % 60).toString().padStart(2, '0')
  return `${m}:${s}`
}

async function load() {
  loading.value = true
  error.value   = null
  try {
    songs.value = await api.getSongs()
  } catch (e) {
    error.value = e.message
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  load()
  window.addEventListener('media-refresh', load)
  window.addEventListener('play-next', (e) => {
  const currentId = e.detail
  const index = filteredSongs.value.findIndex(s => s.id === currentId)
  const next = filteredSongs.value[index + 1]
  if (next) play(next)
})
})

onUnmounted(() => {
  window.removeEventListener('media-refresh', load)
  window.removeEventListener('play-next', () => {})
})
</script>