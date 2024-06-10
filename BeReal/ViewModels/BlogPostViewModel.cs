using BeReal.Models;
namespace BeReal.ViewModels
{
    public class BlogPostViewModel
    {
        public Post? Post { get; set; }
        public Comment? Comment { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
