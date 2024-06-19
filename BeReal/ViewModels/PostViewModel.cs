using BeReal.Models;

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
        public BR_Document? Image { get; set; }
        public bool Approved { get; set; }
        public BR_Document? Document { get; set; }
        public List<BR_Comment>? Comments { get; set; }
    }
}
