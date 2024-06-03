using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeReal.Controllers
{ 
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification { get; }
        public BlogController(ApplicationDbContext context, INotyfService notification) 
        { 
            _context = context; 
            _notification = notification;
        }
        [HttpGet]
        public async Task<IActionResult> Post(string slug)
        {
            if (slug == "")
            {
                _notification.Error("Post not found");
                return View();
            }
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Slug == slug);
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
                ImageUrl = post.Image
            };
            return View(vm);
        }
    }
}
