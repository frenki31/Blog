using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreatePost()
        {
            return View(new CreatePostViewModel());
        }
        /*
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult CreatePost(CreatePostViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }
        }*/
    }
}
