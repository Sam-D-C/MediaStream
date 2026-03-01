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
        public Playlist likes = new Playlist("Likes", new List<MP3> { });

        public User(string username, string email, string password)
        {
            this.username = username;
            this.email = email;
            this.password = password;
        }

        public void AddToLikes(MP3 song)
        {
            if (!likes.Songs.Contains(song))
            {
                likes.AddSong(song);
                Console.WriteLine($"Added '{song.Name}' to {username}'s Likes playlist.");
            }
            else
            {
                Console.WriteLine($"Deleting '{song.Name}' from {username}'s Likes playlist.");
            }
        }
    }
}
