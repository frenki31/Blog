using BeReal.Models;
namespace BeReal.ViewModels
{
    public class HomeViewModel
    {
        public PageViewModel? Page { get; set; }
        public IEnumerable<BR_Post>? Posts { get; set; }
        public string? Search {  get; set; }
        public string? Category { get; set; }
        public string? SubCategory { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public bool NextPage { get; set; }
        public BR_Document? Document { get; set; }
        public List<BR_Category>? Categories { get; set; }
        public List<int>? Pages { get; set; }
    }
}
