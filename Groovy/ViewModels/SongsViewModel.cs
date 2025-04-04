using Groovy.Models;

namespace Groovy.ViewModels
{
    public class SongsViewModel
    {
        public List<Song> Songs { get; set; } = new List<Song>();
        public string SearchTerm { get; set; }

    }
}
