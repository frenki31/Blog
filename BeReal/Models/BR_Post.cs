using System.ComponentModel.DataAnnotations;

namespace BeReal.Models
{
    public class BR_Post
    {
        [Key]
        public int IDBR_Post { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Author { get; set; }
        public BR_ApplicationUser? ApplicationUser { get; set; }
        public DateTime PublicationDate { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? Image { get; set; }
        public BR_Document? Document { get; set; }
        public string? Category { get; set; }
        public string? Tags { get; set; }
        public bool Approved { get; set; }
        public List<BR_Comment>? Comments { get; set; }
    }
}
