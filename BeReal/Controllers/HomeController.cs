using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data.Repository;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BeReal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository _repo;
        private readonly IFileManager _fileManager;
        public INotyfService _notification { get; }
        public HomeController(INotyfService notification, IRepository repo, IFileManager fileManager)
        {
            _repo = repo;
            _notification = notification;
            _fileManager = fileManager; 
        }
        public async Task<IActionResult> Index(int page, string category, string search, DateTime startDate, DateTime endDate)
        {
            if (page < 1)
                return RedirectToAction("Index", new { page = 1, search, category, startDate, endDate });
            
            var home = await _repo.getPage("home");
            var query = _repo.getFilteredPosts(category, search, startDate, endDate);

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
                Pages = _fileManager.Pages(page, pageCount),
            };
            return View(viewModel);
        }
        public async Task<IActionResult> About()
        {
            var page = await _repo.getPage("about");
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
            var page = await _repo.getPage("contact");
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
            var page = await _repo.getPage("privacy");
            var vm = new PageViewModel()
            {
                Title = page!.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Profile(string id)
        {
            var user = await _repo.getUserById(id);
            var userRole = await _repo.getUserRole(user!);
            var posts = await _repo.getUserPosts(user!);
            var postCount = posts.Count();
            if (user == null)
            {
                _notification.Error("User does not exist");
                return View();
            }
            var userVM = new UserViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Role = userRole[0],
                NumberPosts = postCount,
                Posts = posts,
            };
            return View(userVM);
        }
        [HttpPost]
        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var file = await _fileManager.GetFileById(id);
            if (file == null)
            {
                return NotFound();
            }
            return File(file.Data!, file.ContentType!, file.FileName);
        }
    }
}
