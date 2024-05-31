namespace BeReal.Models
{
    public class ApplicationUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Post> Posts { get; set; }
    }
}
