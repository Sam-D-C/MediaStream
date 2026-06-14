namespace MediaStream.Api.Models
{
    public class Song
    {
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Album { get; set; } = "";
        public string ReleaseDate { get; set; } = "";
        public string FilePath { get; set; } = "";
        public bool HasCover { get; set; } = false;
        public long DurationSeconds { get; set; } = 0;
    }

    public class Video
    {
        public string ID { get; set; } = "";
        public string Name { get; set; } = "";
        public string FilePath { get; set; } = "";
        public long FileSizeBytes { get; set; } = 0;
    }

    public class Playlist
    {
        public string Name { get; set; } = "";
        public List<string> SongIds { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
