using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeReal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        public HomeController(INotyfService notification, ApplicationDbContext context)
        {
            _context = context;
            _notification = notification;
        }
        public async Task<IActionResult> Index(int page, string category, string search, DateTime startDate, DateTime endDate)
        {
            if (page < 1)
                return RedirectToAction("Index", new { page = 1, search, category, startDate, endDate });
            
            var home = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "home");
            
            //create a query
            var query = _context.Posts.AsQueryable();
            //order all approved posts by date desc
            query = query.OrderByDescending(x => x.publicationDate).Where(x => x.Approved == true);
            //filter by category
            query = string.IsNullOrEmpty(category) ? query : query.Where(post => post.Category!.ToLower().Equals(category.ToLower()));
            //filter by searchword
            query = string.IsNullOrEmpty(search) ? query : query.Where(x => x.Title!.Contains(search) || x.Author!.Contains(search) ||
                                                                       x.ShortDescription!.Contains(search) || x.Description!.Contains(search));
            //filter by date
            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue && startDate < endDate)
            {
                query = query.Where(x => x.publicationDate >= startDate && x.publicationDate <= endDate);
            }
            else if (startDate != DateTime.MinValue && endDate == DateTime.MinValue)
            {
                query = query.Where(x => x.publicationDate >= startDate);
            }
            else if (endDate != DateTime.MinValue && startDate == DateTime.MinValue)
            {
                query = query.Where(x => x.publicationDate <= endDate);
            }
            else if (startDate == endDate && endDate != DateTime.MinValue && startDate != DateTime.MinValue)
            {
                query = query.Where(x => x.publicationDate == startDate);
            }
            else if (startDate > endDate)
            {
                _notification.Error("Start date must be earlier than end date");
            }
            else if (endDate == DateTime.MinValue && startDate == DateTime.MinValue)
            {
                _notification.Error("Choose the dates");
            }
            int postCount = query.Count();
            int pageSize = 5;
            int skip = pageSize * (page - 1);
            int pageCount = (int)Math.Ceiling((double)postCount / pageSize);
            var viewModel = new HomeViewModel()
            {
                Title = home!.Title,
                ShortDescription = home.ShortDescription,
                ImageUrl = home.ImageUrl,
                Category = category,
                Search = search,
                StartDate = startDate,
                EndDate = endDate,
                PageNumber = page,
                NextPage = postCount > skip + pageSize,
                PageCount = pageCount,
                Posts = query.Skip(skip).Take(pageSize).ToList(),
                Pages = Pages(page, pageCount),
            };
            return View(viewModel);
        }
        public async Task<IActionResult> About()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "about");
            var vm = new PageViewModel()
            {
                Title = page!.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(vm);
        }
        public async Task<IActionResult> Contact()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "contact");
            var vm = new PageViewModel()
            {
                Title = page!.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(vm);
        }
        public async Task<IActionResult> Privacy()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "privacy");
            var vm = new PageViewModel()
            {
                Title = page!.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(vm);
        }
        public List<int> Pages (int PageNumber, int PageCount)
        {
            List<int> pages = new List<int>();
            if (PageCount <= 5)
            {
                for (int i = 1; i <= PageCount; i++)
                    pages.Add(i);
            }
            else
            {
                int mid = PageNumber;
                if (mid < 3)
                    mid = 3;
                else if (mid > PageCount)
                    mid = PageCount - 2;

                for (int i = mid - 2; i <= mid + 2; i++)
                {
                    pages.Add(i);
                }
                if (pages[0] != 1)
                {
                    pages.Insert(0, 1);
                    if (pages[1] - pages[0] > 1)
                    {
                        pages.Insert(1, -1);
                    }
                }
                if (pages[pages.Count - 1] != PageCount)
                {
                    pages.Insert(pages.Count, PageCount);
                    if (pages[pages.Count - 1] - pages[pages.Count - 2] > 1)
                    {
                        pages.Insert(pages.Count - 1, -1);
                    }
                }
            }
            return pages;
        }
    }
}
