using Groovy.Models;

namespace Groovy.ViewModels
{
    public class GenreDetailsViewModel
    {
        public Genre Genre { get; set; }
        public List<Song> Songs { get; set; }
        public List<Artist> Artists { get; set; }
    }
}
