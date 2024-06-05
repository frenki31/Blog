using BeReal.Models;
using X.PagedList;
namespace BeReal.ViewModels
{
    public class HomeViewModel
    {
        public string? Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? ShortDescription { get; set; }
        public IPagedList<Post>? Posts { get; set; }
        public string? Search {  get; set; }
        public string? Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
