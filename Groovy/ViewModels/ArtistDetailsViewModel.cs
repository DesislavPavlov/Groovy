using Groovy.Models;

namespace Groovy.ViewModels
{
    public class ArtistDetailsViewModel
    {
        public Artist Artist { get; set; }
        public List<Artist> RelatedArtists { get; set;}
        public List<Song> Songs { get; set;}
    }
}
