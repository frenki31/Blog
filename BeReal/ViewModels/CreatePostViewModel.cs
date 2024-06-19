using BeReal.Models;
using System.ComponentModel.DataAnnotations;

namespace BeReal.ViewModels
{
    public class CreatePostViewModel
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? ShortDescription { get; set; }
        public string? Author { get; set; }
        [Required]
        public string? Category { get; set; }
        public List<BR_Category>? Categories { get; set; }
        public string? Tags { get; set; }
        public string? Description { get; set; }
        public FileViewModel? Image { get; set; }
        public FileViewModel? File { get; set; }
        public bool Approved { get; set; }
    }
}
