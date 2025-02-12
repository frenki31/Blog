using BeReal.Models;
using System.ComponentModel.DataAnnotations;

namespace BeReal.ViewModels
{
    public class CreatePostViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string? Title { get; set; }
        [Required(ErrorMessage = "Short Description is required")]
        public string? ShortDescription { get; set; }
        public string? Author { get; set; }
        [Required(ErrorMessage = "Category is required")]
        public string? Category { get; set; }
        public List<BR_Category>? Categories { get; set; }
        public string? Tags { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }
        public FileViewModel? Image { get; set; }
        public FileViewModel? File { get; set; }
        public bool Approved { get; set; }
    }
}
