using Groovy.Models;

namespace Groovy.ViewModels
{
    public class UserDetailsViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Artist> FavouriteArtists { get; set; }
        public List<Genre> FavouriteGenres { get; set; }
        public List<Song> FavouriteSongs { get; set; }
    }
}
