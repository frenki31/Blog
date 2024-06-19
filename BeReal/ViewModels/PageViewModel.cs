using System.ComponentModel.DataAnnotations;

namespace BeReal.ViewModels
{
    public class PageViewModel
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? Description { get; set; }
        public string? Slug { get; set; }
    }
}
