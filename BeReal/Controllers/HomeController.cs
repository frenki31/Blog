using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data.Repository.Files;
using BeReal.Data.Repository.Pages;
using BeReal.Data.Repository.Posts;
using BeReal.Data.Repository.Users;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BeReal.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUsersOperations _usersOperations;
        private readonly IPagesOperations _pagesOperations;
        private readonly IPostsOperations _postsOperations;
        private readonly IFileManager _fileManager;
        public INotyfService _notification { get; }
        public HomeController(INotyfService notification, IUsersOperations usersOperations, IPagesOperations pagesOperations, IPostsOperations postsOperations, IFileManager fileManager)
        {
            _pagesOperations = pagesOperations;
            _usersOperations = usersOperations;
            _postsOperations = postsOperations;
            _notification = notification;
            _fileManager = fileManager; 
        }
        public async Task<IActionResult> Index(int page, string category, string search, DateTime startDate, DateTime endDate)
        {
            if (page < 1)
                return RedirectToAction("Index", new { page = 1, search, category, startDate, endDate });
            var home = await _pagesOperations.getPage("home");
            var query = _postsOperations.getFilteredPosts(category, search, startDate, endDate);
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
            var page = await _pagesOperations.getPage("about");
            return View(_pagesOperations.GetPageViewModel(page!));
        }
        public async Task<IActionResult> Contact()
        {
            var page = await _pagesOperations.getPage("contact");
            return View(_pagesOperations.GetPageViewModel(page!));
        }
        public async Task<IActionResult> Privacy()
        {
            var page = await _pagesOperations.getPage("privacy");
            return View(_pagesOperations.GetPageViewModel(page!));
        }
        public async Task<IActionResult> Profile(string id)
        {
            var user = await _usersOperations.getUserById(id);
            var userRole = await _usersOperations.getUserRole(user);
            var posts = await _postsOperations.getUserPosts(user!);
            var postCount = posts.Count();
            if (user == null)
                return View();
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
            if (id == null) return NotFound();
            var file = await _fileManager.GetFileById(id);
            if (file == null) return NotFound();
            return File(file.Data!, file.ContentType!, file.FileName);
        }
    }
}