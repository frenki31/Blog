using BeReal.Models;
namespace BeReal.ViewModels
{
    public class HomeViewModel
    {
        public string? Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? ShortDescription { get; set; }
        public List<Post>? Posts { get; set; }

    }
}
