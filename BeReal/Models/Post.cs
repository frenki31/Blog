namespace BeReal.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        //relation
        public string? Author { get; set; }
        public ApplicationUser? User { get; set; }
        public DateTime publicationDate { get; set; } = DateTime.Now;
        public string? Description { get; set; }
        public string? Slug { get; set; }
        public string? Image { get; set; }
    }
}
