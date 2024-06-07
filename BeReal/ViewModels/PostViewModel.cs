using BeReal.Models;
using BeReal.Models.Comments;

namespace BeReal.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Author { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
        public DateTime publicationDate { get; set; }
        public string? Image { get; set; }
        public bool Approved { get; set; }
        public Document? Document { get; set; }
        public List<MainComment>? MainComments { get; set; }
    }
}
