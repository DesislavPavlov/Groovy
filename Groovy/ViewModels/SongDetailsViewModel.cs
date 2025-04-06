using Groovy.Models;

namespace Groovy.ViewModels
{
    public class SongDetailsViewModel
    {
        public int UserId { get; set; }
        public Song CurrentSong { get; set; }
        public List<Artist> Artists { get; set; }
        public List<Genre> Genres { get; set; }
        public List<Song> RelatedSongs { get; set; }
        public List<Song> RecommendedSongs { get; set; }
        public List<int> FavouriteSongIds { get; set; }
    }
}
