namespace BeReal.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string? Message { get; set; }
        public DateTime? Created { get; set; }
        public int PostId { get; set; }
        public string? UserId { get; set; }
        public Post? Post { get; set; }
        public ApplicationUser? User { get; set; }
        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public List<Comment>? Replies { get; set; }
    }
}
