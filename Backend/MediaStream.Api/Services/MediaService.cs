using MediaStream.Api.Models;
using TagLib;

namespace MediaStream.Api.Services
{
    public class MediaService
    {
        private string _musicPath;
        private string _videoPath;
        private List<Song> _songs = new();
        private List<Video> _videos = new();

        public MediaService(SettingsService settings)
        {
            var s = settings.Get();
            _musicPath = s.MusicPath;
            _videoPath = s.VideosPath;
            Refresh();
        }

        public void UpdatePaths(string musicPath, string videoPath)
        {
            _musicPath = musicPath;
            _videoPath = videoPath;
            Refresh();
        }

        private void Refresh()
        {
            LoadSongs();
            LoadVideos();
        }

        private void LoadSongs()
        {
            _songs.Clear();

            if (!Directory.Exists(_musicPath))
            {
                Directory.CreateDirectory(_musicPath);
                return;
            }

            var files = Directory.GetFiles(_musicPath, "*.mp3");

            foreach (var file in files)
            {
                try
                {
                    var tagFile = TagLib.File.Create(file);
                    var fileName = Path.GetFileNameWithoutExtension(file);

                    _songs.Add(new Song
                    {
                        ID = fileName,
                        Name = tagFile.Tag.Title ?? fileName,
                        Artist = tagFile.Tag.FirstPerformer ?? "Unknown",
                        Album = tagFile.Tag.Album ?? "Unknown",
                        ReleaseDate = tagFile.Tag.Year > 0 ? tagFile.Tag.Year.ToString() : "Unknown",
                        FilePath = file,
                        HasCover = tagFile.Tag.Pictures.Length > 0,
                        DurationSeconds = (long)tagFile.Properties.Duration.TotalSeconds
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[MediaService] Fout bij laden: {file} — {ex.Message}");
                }
            }
        }

        private void LoadVideos()
        {
            _videos.Clear();

            if (!Directory.Exists(_videoPath))
            {
                Directory.CreateDirectory(_videoPath);
                return;
            }

            var extensions = new[] { "*.mp4", "*.mkv", "*.webm" };
            var files = extensions.SelectMany(ext => Directory.GetFiles(_videoPath, ext));

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var info = new FileInfo(file);

                _videos.Add(new Video
                {
                    ID = fileName,
                    Name = fileName.Replace("_", " ").Replace("-", " "),
                    FilePath = file,
                    FileSizeBytes = info.Length
                });
            }
        }

        public IEnumerable<object> GetAllSongs()
        {
            return _songs.Select(s => new {
                s.ID,
                s.Name,
                s.Artist,
                s.Album,
                s.ReleaseDate,
                s.HasCover,
                s.DurationSeconds
            });
        }

        public Song? GetSongById(string id) =>
            _songs.FirstOrDefault(s => s.ID == id);

        public IEnumerable<object> GetAllVideos()
        {
            return _videos.Select(v => new { v.ID, v.Name, v.FileSizeBytes });
        }

        public Video? GetVideoById(string id) =>
            _videos.FirstOrDefault(v => v.ID == id);
        public byte[]? GetCoverArt(string id)
        {
            var song = GetSongById(id);
            if (song == null) return null;

            var tagFile = TagLib.File.Create(song.FilePath);
            return tagFile.Tag.Pictures.FirstOrDefault()?.Data.Data;
        }

        public void RefreshLibrary()
        {
            Refresh();
        }
    }
}