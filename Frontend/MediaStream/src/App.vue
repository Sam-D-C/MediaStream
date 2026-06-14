<template>
  <div class="app">
<aside class="sidebar">
  <div class="logo">
    <span class="logo-icon">▶</span>
    <span class="logo-text">MediaStream</span>
  </div>

  <nav>
    <RouterLink to="/" class="nav-link" active-class="active">
      ♪ Muziek
    </RouterLink>
    <RouterLink to="/videos" class="nav-link" active-class="active">
      ▶ Video's
    </RouterLink>

    <div class="nav-divider" />

    <!-- Playlists -->
    <button class="nav-link nav-btn" @click="playlistsOpen = !playlistsOpen">
      <span>≡</span>
      <span>Playlists</span>
      <span class="nav-chevron">{{ playlistsOpen ? '▾' : '▸' }}</span>
    </button>

    <div v-if="playlistsOpen" class="nav-sub">
      <div v-if="playlists.length === 0" class="nav-sub-empty">Geen playlists</div>
      <RouterLink
        v-for="pl in playlists"
        :key="pl.name"
        :to="`/playlist/${encodeURIComponent(pl.name)}`"
        class="nav-sub-link"
        active-class="active"
      >
        {{ pl.name }}
      </RouterLink>
    </div>
    <RouterLink to="/settings" class="nav-link" active-class="active">
  ⚙ Instellingen
</RouterLink>
  </nav>

  <div class="sidebar-footer">
    <button class="nav-link nav-btn queue-btn" @click="$router.push('/queue')">
      <span>⏭</span>
      <span>Wachtrij</span>
      <span class="queue-badge" v-if="queue.length">{{ queue.length }}</span>
    </button>
    <button class="refresh-btn" @click="refresh" :disabled="refreshing">
      {{ refreshing ? 'Laden...' : '↺ Vernieuwen' }}
    </button>
  </div>
</aside>

    <main class="content">
      <RouterView />
    </main>

    <footer class="player" v-if="currentSong">
      <div class="player-cover">
        <img v-if="currentSong.hasCover" :src="coverUrl" :alt="currentSong.name" />
        <div v-else class="cover-placeholder">♪</div>
      </div>

      <div class="player-info">
        <div class="player-title">{{ currentSong.name }}</div>
        <div class="player-artist">{{ currentSong.artist }}</div>
      </div>

      <div class="player-controls">
        <button class="ctrl-btn" @click="togglePlay">
          {{ isPlaying ? '⏸' : '▶' }}
        </button>

        <div class="progress-wrap">
          <span class="time">{{ formatTime(currentTime) }}</span>
          <input type="range" class="progress"
            :max="duration" :value="currentTime" @input="seek" />
          <span class="time">{{ formatTime(duration) }}</span>
        </div>

        <div class="volume-wrap">
          <span>🔊</span>
          <input type="range" class="volume" min="0" max="1" step="0.05"
            v-model="volume" @input="setVolume" />
        </div>
      </div>

      <audio
        ref="audioEl"
        :src="streamUrl"
        @timeupdate="onTimeUpdate"
        @loadedmetadata="onLoaded"
        @ended="playNext"
        @play="isPlaying = true"
        @pause="isPlaying = false"
      />
    </footer>
  </div>
</template>

<script setup>
import { ref, computed, provide, onMounted, onUnmounted } from 'vue'
import { api } from './api.js'

const contextMenu = ref(null)
const queue     = ref([])   // De wachtrij
const queueIndex = ref(0)   // Welk nummer speelt nu

const currentSong = ref(null)
const audioEl     = ref(null)
const isPlaying   = ref(false)
const currentTime = ref(0)
const duration    = ref(0)
const volume      = ref(0.8)
const refreshing  = ref(false)

const playlistsOpen = ref(false)
const playlists = ref([])

async function loadPlaylists() {
  playlists.value = await api.getPlaylists()
}

onMounted(() => {
  loadPlaylists()
  window.addEventListener('playlists-changed', loadPlaylists)
})

onUnmounted(() => {
  window.removeEventListener('playlists-changed', loadPlaylists)
})

const streamUrl = computed(() =>
  currentSong.value ? api.songStreamUrl(currentSong.value.id) : ''
)
const coverUrl = computed(() =>
  currentSong.value ? api.songCoverUrl(currentSong.value.id) : ''
)

provide('playSong', playSong)
provide('queue', queue)
provide('queueIndex', queueIndex)

function togglePlay() {
  if (isPlaying.value) audioEl.value?.pause()
  else audioEl.value?.play()
}

function seek(e) {
  if (audioEl.value) audioEl.value.currentTime = e.target.value
}

function setVolume() {
  if (audioEl.value) audioEl.value.volume = volume.value
}

function onTimeUpdate() {
  currentTime.value = audioEl.value?.currentTime ?? 0
}

function onLoaded() {
  duration.value = audioEl.value?.duration ?? 0
  audioEl.value.volume = volume.value
}

function formatTime(secs) {
  const m = Math.floor(secs / 60)
  const s = Math.floor(secs % 60).toString().padStart(2, '0')
  return `${m}:${s}`
}

async function refresh() {
  refreshing.value = true
  await api.refresh()
  window.dispatchEvent(new Event('media-refresh'))
  refreshing.value = false
}

function playSong(song) {
  currentSong.value = song
  setTimeout(() => audioEl.value?.play(), 50)
}

function playQueue(songs, startIndex = 0) {
  queue.value = songs
  queueIndex.value = startIndex
  playSong(songs[startIndex])
}

function addToQueue(song) {
  queue.value.push(song)
  // Als er niets speelt, begin meteen
  if (!currentSong.value) {
    queueIndex.value = 0
    playSong(queue.value[0])
  }
}

function playNext() {
  if (queue.value.length > 0) {
    // Speel volgende uit wachtrij
    const nextIndex = queueIndex.value + 1
    if (nextIndex < queue.value.length) {
      queueIndex.value = nextIndex
      playSong(queue.value[nextIndex])
    }
  } else {
    // Geen wachtrij — stuur event naar MusicView
    window.dispatchEvent(new CustomEvent('play-next', { detail: currentSong.value?.id }))
  }
}

// Maak functies beschikbaar voor child components
provide('playSong', playSong)
provide('playQueue', playQueue)
provide('addToQueue', addToQueue)
</script>