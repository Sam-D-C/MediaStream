namespace MediaStream
{
    class Song
    {
        public string Name;
        public string Cover;
        public string Artist;
        public string Description;
        public string ReleaseDate;

        // Constructor
        public Song(string name, string cover, string artist, string description, string releaseDate)
        {
            Name = name;
            Cover = cover;
            Artist = artist;
            Description = description;
            ReleaseDate = releaseDate;
        }
    }
}
