using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindesMusic
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int IsArtist { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public List<Playlist> Playlists { get; set; } = new List<Playlist>();
        public List<Song> Songs { get; set; } = new List<Song>();
    }
}
