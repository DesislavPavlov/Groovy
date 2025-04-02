using Groovy.Models;

namespace Groovy.ViewModels
{
    public class SongsViewModel
    {
        public int UserId { get; set; }
        public List<Song> Songs { get; set; } = new List<Song>();
        public List<int> FavouriteSongIds { get; set; } = new List<int>();
        public string SearchTerm { get; set; }
    }
}
