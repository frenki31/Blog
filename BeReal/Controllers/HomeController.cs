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
        public async Task<IActionResult> Index(int page, string category, string subcategory, string search, DateTime startDate, DateTime endDate) //main page with all posts and filters
        {
            if (page < 1)
                return RedirectToAction("Index", new { page = 1, search, category, subcategory, startDate, endDate });
            var home = await _pagesOperations.GetPage("home");
            var query = _postsOperations.GetFilteredPosts(category, subcategory, search, startDate, endDate);
            int pageSize = 5;
            int skip = pageSize * (page - 1);
            int postCount = query.Count();
            int pageCount = (int)Math.Ceiling((double)postCount / pageSize);
            var viewModel = new HomeViewModel()
            {
                Page = _pagesOperations.GetPageViewModel(home!),
                Category = category,
                SubCategory = subcategory,
                Search = search,
                StartDate = startDate,
                EndDate = endDate,
                PageNumber = page,
                NextPage = postCount > skip + pageSize,
                PageCount = pageCount,
                Categories = await _postsOperations.GetCategories(),
                Posts = await _postsOperations.GetPostsWithPagination(query, skip, pageSize),
                Pages = _fileManager.Pages(page, pageCount),
            };
            return View(viewModel);
        }
        public async Task<IActionResult> About()
        {
            var page = await _pagesOperations.GetPage("about");
            return View(_pagesOperations.GetPageViewModel(page!));
        }
        public async Task<IActionResult> Contact()
        {
            var page = await _pagesOperations.GetPage("contact");
            return View(_pagesOperations.GetPageViewModel(page!));
        }
        public async Task<IActionResult> Privacy()
        {
            var page = await _pagesOperations.GetPage("privacy");
            return View(_pagesOperations.GetPageViewModel(page!));
        }
        public async Task<IActionResult> Profile(string id) //display the profile of a user
        {
            var user = await _usersOperations.GetUserById(id);
            var userRole = await _usersOperations.GetUserRole(user);
            var posts = await _postsOperations.GetUserPosts(user!);
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
        public async Task<IActionResult> Download(int? id) //download a file or image
        {
            var (fileData, contentType, fileName) = await _fileManager.GetFile(id, _fileManager);
            return File(fileData, contentType, fileName);
        }
        public async Task<IActionResult> GetImage(int? id) //get the image to display
        {
            var (data, contentType, fileName) = await _fileManager.GetFile(id, _fileManager);
            return File(data, contentType, fileName);
        }
    }
}