using BeReal.Models;
namespace BeReal.ViewModels
{
    public class HomeViewModel
    {
        public string? Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? ShortDescription { get; set; }
        public IEnumerable<Post>? Posts { get; set; }
        public string? Search {  get; set; }
        public string? Category { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public bool NextPage { get; set; }
        public Document? Document { get; set; }
        public List<int>? Pages { get; set; }
    }
}
