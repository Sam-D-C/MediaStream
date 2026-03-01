namespace MediaStream
{
    public class Program
    {
        Playlist likes = new Playlist("Likes", new List<MP3> { });

        public static void Main()
        {
            MP3 music = new MP3("Shape of You", "cover.jpg", "Ed Sheeran", "A popular song by Ed Sheeran", "2017-01-06");
            MP3 music2 = new MP3("Blinding Lights", "cover2.jpg", "The Weeknd", "A hit song by The Weeknd", "2019-11-29");
            MP3 music3 = new MP3("Levitating", "cover3.jpg", "Dua Lipa", "A catchy song by Dua Lipa", "2020-03-27");

            Playlist playlist = new Playlist("Playlist1", new List<MP3> { music, music2, music3 });

            User sam = new User( "Sam",  "email",  "password");
            sam.AddToLikes(music);
            sam.likes.DisplayPlaylist();
            sam.AddToLikes(music);
            sam.likes.DisplayPlaylist();

        }
    }
}