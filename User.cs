using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaStream
{
    class User
    {
        public string username;
        string email;
        string password;
        public Playlist likes = new Playlist("Likes", new List<Song> { });

        public User(string username, string email, string password)
        {
            this.username = username;
            this.email = email;
            this.password = password;
        }

        public void AddToLikes(Song song)
        {
            if (!likes.Songs.Contains(song))
            {
                likes.AddSong(song);
            }
            else
            {
                likes.DeleteSong(song);
            }
        }
    }
}
