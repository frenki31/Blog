using System.ComponentModel.DataAnnotations;

namespace BeReal.ViewModels
{
    public class CommentViewModel
    {
        [Required]
        public int PostId { get; set; }
        [Required]
        public int MainCommentId { get; set; }
        [Required]
        public string? Message { get; set; }
    }
}
