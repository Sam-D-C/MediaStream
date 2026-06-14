<template>
  <div>
    <header class="page-header">
      <h1>Wachtrij</h1>
      <p class="subtitle" v-if="queue.length">{{ queue.length }} nummers</p>
    </header>

    <div v-if="queue.length === 0" class="state-msg">
      Wachtrij is leeg — rechtsklik op een nummer om toe te voegen.
    </div>

    <div v-else class="queue-list">
      <div
        v-for="(song, index) in queue"
        :key="song.id + index"
        class="queue-row"
        :class="{ active: index === queueIndex }"
      >
        <div class="queue-num">{{ index + 1 }}</div>

        <div class="song-cover">
          <img
            v-if="song.hasCover"
            :src="api.songCoverUrl(song.id)"
            :alt="song.name"
            loading="lazy"
          />
          <div v-else class="cover-ph">♪</div>
        </div>

        <div class="song-meta">
          <div class="song-name">{{ song.name }}</div>
          <div class="song-artist">{{ song.artist !== 'Unknown' ? song.artist : '' }}</div>
        </div>

        <div class="queue-actions">
          <button
            class="q-btn"
            @click="moveUp(index)"
            :disabled="index === 0"
            title="Omhoog"
          >▲</button>
          <button
            class="q-btn"
            @click="moveDown(index)"
            :disabled="index === queue.length - 1"
            title="Omlaag"
          >▼</button>
          <button
            class="q-btn q-btn-remove"
            @click="remove(index)"
            title="Verwijderen"
          >✕</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { inject } from 'vue'
import { api } from '../api.js'

const queue      = inject('queue')
const queueIndex = inject('queueIndex')

function moveUp(index) {
  if (index === 0) return
  const arr = [...queue.value]
  ;[arr[index - 1], arr[index]] = [arr[index], arr[index - 1]]
  queue.value = arr
  if (queueIndex.value === index) queueIndex.value--
  else if (queueIndex.value === index - 1) queueIndex.value++
}

function moveDown(index) {
  if (index === queue.value.length - 1) return
  const arr = [...queue.value]
  ;[arr[index + 1], arr[index]] = [arr[index], arr[index + 1]]
  queue.value = arr
  if (queueIndex.value === index) queueIndex.value++
  else if (queueIndex.value === index + 1) queueIndex.value--
}

function remove(index) {
  queue.value.splice(index, 1)
  if (queueIndex.value >= queue.value.length) {
    queueIndex.value = Math.max(0, queue.value.length - 1)
  }
}
</script>