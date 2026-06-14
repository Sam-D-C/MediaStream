<template>
  <div>
    <header class="page-header">
      <div class="playlist-header-row">
        <h1>{{ name }}</h1>
        <button class="delete-playlist-btn" @click="deletePlaylist">
          🗑 Playlist verwijderen
        </button>
      </div>
      <p class="subtitle" v-if="songs.length">{{ songs.length }} nummers</p>
    </header>

    <div v-if="loading" class="state-msg">Laden...</div>
    <div v-else-if="songs.length === 0" class="state-msg">
      Playlist is leeg — rechtsklik op een nummer om toe te voegen.
    </div>

    <div v-else>
      <button class="play-all-btn" @click="playAll">
        ▶ Alles afspelen
      </button>

      <div class="song-list">
        <div
          v-for="song in songs"
          :key="song.id"
          class="song-row"
          :class="{ active: currentId === song.id }"
          @click="play(song)"
        >
          <div class="song-cover">
            <img
              v-if="song.hasCover"
              :src="api.songCoverUrl(song.id)"
              :alt="song.name"
              loading="lazy"
            />
            <div v-else class="cover-ph">♪</div>
            <div class="play-overlay">▶</div>
          </div>

          <div class="song-meta">
            <div class="song-name">{{ song.name }}</div>
            <div class="song-artist">{{ song.artist !== 'Unknown' ? song.artist : '' }}</div>
          </div>

          <div class="song-album">{{ song.album !== 'Unknown' ? song.album : '' }}</div>
          <div class="song-dur">{{ formatDur(song.durationSeconds) }}</div>

          <button class="remove-btn" @click.stop="removeSong(song.id)">✕</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, inject, onMounted, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { api } from '../api.js'

const route    = useRoute()
const router   = useRouter()
const name     = ref(decodeURIComponent(route.params.name))
const songs    = ref([])
const loading  = ref(true)
const currentId = ref(null)

const playSong  = inject('playSong')
const playQueue = inject('playQueue')

async function load() {
  loading.value = true
  const data = await api.getPlaylist(name.value)
  songs.value = data.songs ?? []
  loading.value = false
}

function play(song) {
  currentId.value = song.id
  playSong(song)
}

function playAll() {
  if (songs.value.length === 0) return
  playQueue(songs.value, 0)
  currentId.value = songs.value[0].id
}

async function removeSong(songId) {
  await api.removeFromPlaylist(name.value, songId)
  await load()
}

async function deletePlaylist() {
  if (!confirm(`Playlist '${name.value}' verwijderen?`)) return
  await api.deletePlaylist(name.value)
  window.dispatchEvent(new Event('playlists-changed'))
  router.push('/')
}

function formatDur(secs) {
  if (!secs) return '--:--'
  const m = Math.floor(secs / 60)
  const s = (secs % 60).toString().padStart(2, '0')
  return `${m}:${s}`
}

watch(() => route.params.name, (val) => {
  name.value = decodeURIComponent(val)
  load()
})

onMounted(load)
</script>