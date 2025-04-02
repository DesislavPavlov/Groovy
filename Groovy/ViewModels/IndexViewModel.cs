using Groovy.Models;

namespace Groovy.ViewModels
{
    public class IndexViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public List<Song> Songs { get; set; }
        public List<Artist> Artists { get; set; }
        public List<Genre> Genres { get; set; }
        public List<int> FavouriteSongIds { get; set; }
        public List<Song> TopSongs { get; set; }
        public Dictionary<int, List<int>> ArtistGenreRelations { get; set; }
        public Dictionary<int, List<int>> SongArtistRelations { get; set; }
        public Dictionary<int, List<int>> SongGenreRelations { get; set; }

        //public IndexViewModel()
        //{
        //    Songs = new List<Song>();
        //    Artists = new List<Artist>();
        //    Genres = new List<Genre>();
        //    TopSongs = new List<Song>();
        //    ArtistGenreRelations = new Dictionary<int, List<int>>();
        //    SongArtistRelations = new Dictionary<int, List<int>>();
        //    SongGenreRelations = new Dictionary<int, List<int>>();
        //}
    }
}
