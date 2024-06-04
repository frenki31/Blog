using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BeReal.Models;
using Microsoft.EntityFrameworkCore;
using BeReal.Utilities;
using X.PagedList;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _imagePath;
        private readonly UserManager<ApplicationUser> _userManager;
        public INotyfService _notification { get; }
        public PostController(ApplicationDbContext context, 
            IConfiguration config,
            INotyfService notification,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _notification = notification;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
            _imagePath = config["Path:Images"]!;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page)
        {
            var posts = new List<Post>();
            var loggedUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedUserRole = await _userManager.GetRolesAsync(loggedUser!);
            if (loggedUserRole[0] == Roles.Admin)
            {
                posts = await _context.Posts.Include(x => x.User).ToListAsync();
            }
            else
            {
                posts = await _context.Posts.Include(x => x.User).Where(x => x.User!.Id == loggedUser!.Id).ToListAsync();
            }
            var pageSize = 4;
            var pageNumber = (page ?? 1);
            var postVms = posts.Select(x => new PostViewModel()
            {
                Id = x.Id,
                Title = x.Title,
                ShortDescription = x.ShortDescription,
                Author = x.Author,
                publicationDate = x.publicationDate,
                Image = x.Image,
                Category = x.Category,
                Tags = x.Tags,
            }).ToList();
            return View(await postVms.OrderByDescending(x => x.publicationDate).ToPagedListAsync(pageNumber, pageSize));
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePostViewModel());
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreatePostViewModel model)
        {
            if (!ModelState.IsValid) { return View(model); }
            var loggedUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var post = new Post
            {
                Title = model.Title,
                ShortDescription = model.ShortDescription,
                Description = model.Description,
                Category = model.Category,
                Tags = model.Tags,
                User = loggedUser,
                Author = loggedUser!.FirstName + " " + loggedUser.LastName,
            };
            if (post.Title != null)
            {
                string slug = model.Title!.Trim();
                slug = slug.Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid();
            }
            if (model.Image != null)
            {
                post.Image = GetImagePath(model.Image);
            }
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            _notification.Success("Post Create Successfully");
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            var loggedUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedUserRole = await _userManager.GetRolesAsync(loggedUser!);
            if (loggedUserRole[0] != Roles.Admin && loggedUser!.Id != post.User!.Id)
            {
                _notification.Error("You are not authorized");
                return RedirectToAction("Index");
            }
            var edit = new CreatePostViewModel() { 
                Id = post.Id,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                Description = post.Description,
                ImageUrl = post.Image,
                Category = post.Category,
                Tags = post.Tags,
            };
            return View(edit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostViewModel vm) 
        { 
            if (!ModelState.IsValid) {  return View(vm); }
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (post == null)
            {
                _notification.Error("Post not found");
                return View();
            }
            post.Title = vm.Title;
            post.ShortDescription = vm.ShortDescription;
            post.Description = vm.Description;
            post.Category = vm.Category;
            post.Tags = vm.Tags;
            if (vm.Image != null)
            {
                if (post.Image != null) 
                    RemoveImage(post.Image);
                post.Image = GetImagePath(vm.Image);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Post Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == id);
            var loggedUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedUserRole = await _userManager.GetRolesAsync(loggedUser!);
            if (loggedUserRole[0] == Roles.Admin || loggedUser!.Id == post!.User!.Id)
            {
                _context.Posts.Remove(post!);
                await _context.SaveChangesAsync();
                _notification.Success("Post Deleted Successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }
            return View();
        }

        private string GetImagePath(IFormFile formFile)
        {
            var folderPath = Path.Combine(_imagePath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var suffix = formFile.FileName.Substring(formFile.FileName.LastIndexOf("."));
            var uniqueFileName = $"img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{suffix}";
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                formFile.CopyToAsync(fileStream).GetAwaiter().GetResult();
            }
            return uniqueFileName;
        }
        private bool RemoveImage(string image)
        {
            try
            {
                var file = Path.Combine(_imagePath, image);
                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
