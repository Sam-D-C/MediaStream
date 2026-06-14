using System.Text.Json;
using MediaStream.Api.Models;

namespace MediaStream.Api.Services
{
    public class PlaylistService
    {
        private readonly string _filePath;
        private List<Playlist> _playlists = new();

        public PlaylistService(IConfiguration config)
        {
            var dir = config["MediaPaths:Music"] ?? "./Music";
            _filePath = Path.Combine(Path.GetDirectoryName(dir)!, "playlists.json");
            Load();
        }

        private void Load()
        {
            if (!File.Exists(_filePath))
            {
                _playlists = new();
                return;
            }

            var json = File.ReadAllText(_filePath);
            _playlists = JsonSerializer.Deserialize<List<Playlist>>(json) ?? new();
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(_playlists, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(_filePath, json);
        }

        public IEnumerable<Playlist> GetAll() => _playlists;

        public Playlist? GetByName(string name) =>
            _playlists.FirstOrDefault(p => p.Name == name);

        public Playlist Create(string name)
        {
            if (_playlists.Any(p => p.Name == name))
                throw new InvalidOperationException($"Playlist '{name}' bestaat al.");

            var playlist = new Playlist { Name = name };
            _playlists.Add(playlist);
            Save();
            return playlist;
        }

        public bool Delete(string name)
        {
            var playlist = GetByName(name);
            if (playlist == null) return false;
            _playlists.Remove(playlist);
            Save();
            return true;
        }

        public bool AddSong(string playlistName, string songId)
        {
            var playlist = GetByName(playlistName);
            if (playlist == null) return false;
            if (!playlist.SongIds.Contains(songId))
            {
                playlist.SongIds.Add(songId);
                Save();
            }
            return true;
        }

        public bool RemoveSong(string playlistName, string songId)
        {
            var playlist = GetByName(playlistName);
            if (playlist == null) return false;
            playlist.SongIds.Remove(songId);
            Save();
            return true;
        }
    }
}