using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data;
using BeReal.Models;
using BeReal.Models.Comments;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

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
        public async Task<IActionResult> Index(int? page, string category, string search, DateTime startDate, DateTime endDate)
        {
            var viewModel = new HomeViewModel();
            var home = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "home");
            viewModel.Title = home!.Title;
            viewModel.ShortDescription = home.ShortDescription;
            viewModel.ImageUrl = home.ImageUrl;
            int pageSize = 4;
            int pageNumber = (page ?? 1);
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
            viewModel.Posts = query.ToPagedList(pageNumber, pageSize);
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
