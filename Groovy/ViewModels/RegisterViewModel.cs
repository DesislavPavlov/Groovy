namespace Groovy.ViewModels
{
    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Password_Hash { get; set; }
        public string AvatarUrl { get; set; }

        public Dictionary<string, string> AvatarBlobUrls { get; set; } = new Dictionary<string, string>();
    }
}
