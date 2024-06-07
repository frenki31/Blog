using BeReal.Models;
namespace BeReal.ViewModels
{
    public class BlogPostViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string ShortDescription { get; set; } = "";
        public string? Category { get; set; }
        public string? Tags { get; set; }
        public string? Author { get; set; }
        public DateTime PublicationDate { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
