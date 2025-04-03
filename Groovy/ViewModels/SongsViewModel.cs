using Groovy.Models;

namespace Groovy.ViewModels
{
    public class SongsViewModel
    {
        public int UserId { get; set; }
        public List<Song> Songs { get; set; } = new List<Song>();
        public List<int> FavouriteSongIds { get; set; } = new List<int>();
        public string SearchTerm { get; set; }
        public List<Artist> Artists { get; set; }
        public List<Genre> Genres { get; set; }
        public Dictionary<int, List<int>> SongArtistRelations { get; set; }
        public Dictionary<int, List<int>> SongGenreRelations { get; set; }

    }
}
