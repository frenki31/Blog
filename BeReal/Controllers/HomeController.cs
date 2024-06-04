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
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        public HomeController(INotyfService notification,ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
            _notification = notification;
        }

        public async Task<IActionResult> Index(int? page, string category)
        {
            var viewModel = new HomeViewModel();
            var setting = _context.Settings.ToList();
            viewModel.Title = setting[0].Title;
            viewModel.ShortDescription = setting[0].Description;
            viewModel.ImageUrl = setting[0].ImageUrl;
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            viewModel.Posts = string.IsNullOrEmpty(category) ? 
                await _context.Posts.OrderByDescending(x => x.publicationDate)
                                    .ToPagedListAsync(pageNumber, pageSize) 
                : await _context.Posts.Where(post => post.Category!.ToLower().Equals(category.ToLower()))
                                      .OrderByDescending(x => x.publicationDate)
                                      .ToPagedListAsync(pageNumber, pageSize);
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

        [HttpGet("[controller]/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            if (slug == "")
            {
                _notification.Error("Post not found");
                return View();
            }
            var post = await _context.Posts.Include(p => p.MainComments)!
                                           .ThenInclude(mc => mc.SubComments)
                                           .FirstOrDefaultAsync(x => x.Slug == slug);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            var vm = new BlogPostViewModel()
            {
                Id = post.Id,
                Author = post.Author,
                Description = post.Description,
                PublicationDate = post.publicationDate,
                ShortDescription = post.ShortDescription,
                Title = post.Title,
                ImageUrl = post.Image,
                Tags = post.Tags,
                Category = post.Category,
                MainComments = post.MainComments,
            };
            return View(vm);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            if (!ModelState.IsValid) { return RedirectToAction("Index", "Home"); }
            var post = await _context.Posts
                                     .Include(p => p.MainComments)!
                                     .ThenInclude(mc => mc.SubComments)
                                     .FirstOrDefaultAsync(x => x.Id == vm.PostId);
            if (vm.MainCommentId == 0)
            {
                post!.MainComments = post.MainComments ?? new List<MainComment>();
                post.MainComments.Add(new MainComment
                {
                    Message = vm.Message,
                    Created = DateTime.Now,
                });
                _context.Posts.Update(post);
            }
            else
            {
                var comment = new SubComment()
                {
                    MainCommentId = vm.MainCommentId,
                    Message = vm.Message,
                    Created = DateTime.Now,
                };
                _context.SubComments.Add(comment);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Post", new { slug = post!.Slug });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
