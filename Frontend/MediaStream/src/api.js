const BASE = '/api'

async function apiFetch(path) {
  const res = await fetch(BASE + path)
  if (!res.ok) throw new Error(`API fout: ${res.status}`)
  return res.json()
}

export const api = {
  getSongs:       ()    => apiFetch('/songs'),
  getVideos:      ()    => apiFetch('/videos'),

  songStreamUrl:  (id)  => `${BASE}/songs/${id}/stream`,
  songCoverUrl:   (id)  => `${BASE}/songs/${id}/cover`,
  videoStreamUrl: (id)  => `${BASE}/videos/${id}/stream`,

  refresh: () => fetch(`${BASE}/songs/refresh`, { method: 'POST' }),

  // Playlists
  getPlaylists:      ()           => apiFetch('/playlists'),
  getPlaylist:       (name)       => apiFetch(`/playlists/${encodeURIComponent(name)}`),
  createPlaylist:    (name)       => fetch(`${BASE}/playlists`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ name })
  }).then(r => r.json()),
  deletePlaylist:    (name)       => fetch(`${BASE}/playlists/${encodeURIComponent(name)}`, {
    method: 'DELETE'
  }),
  addToPlaylist:     (name, songId) => fetch(`${BASE}/playlists/${encodeURIComponent(name)}/songs`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ songId })
  }).then(r => r.json()),
  removeFromPlaylist: (name, songId) => fetch(`${BASE}/playlists/${encodeURIComponent(name)}/songs/${encodeURIComponent(songId)}`, {
    method: 'DELETE'
  }),

  // Instellingen
  getSettings: () => apiFetch('/settings'),
  saveSettings: (musicPath, videosPath) => fetch(`${BASE}/settings`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ musicPath, videosPath })
  }).then(r => r.json()),
}