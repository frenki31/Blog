using System.ComponentModel.DataAnnotations;

namespace BeReal.Models
{
    public class BR_Page
    {
        [Key]
        public int IDBR_Page { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? ImageUrl { get; set; }
    }
}
