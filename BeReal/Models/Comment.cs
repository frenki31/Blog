namespace BeReal.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? Message { get; set; }
        public DateTime Created { get; set; }
        public Post? Post { get; set; }
        public ApplicationUser? User { get; set; }
        public Comment? ParentComment { get; set; }
        public List<Comment>? Replies { get; set; }
    }
}
