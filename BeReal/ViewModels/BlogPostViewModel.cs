using BeReal.Models;
namespace BeReal.ViewModels
{
    public class BlogPostViewModel
    {
        public BR_Post? Post { get; set; }
        public BR_Comment? Comment { get; set; }
        public string? ReturnUrl { get; set; }
    }
}
