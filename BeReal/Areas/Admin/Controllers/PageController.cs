using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data;
using BeReal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public INotyfService _notification { get; }
        public PageController(ApplicationDbContext context,
            IWebHostEnvironment webHostEnvironment,
            INotyfService notification)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;   
            _notification = notification;
        }
        [HttpGet]
        public async Task<IActionResult> About()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "about");
            var pageVm = new PageViewModel()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(pageVm);
        }
        [HttpPost]
        public async Task<IActionResult> About(PageViewModel vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "about");
            if (page == null)
            {
                _notification.Error("Page not found");
                return View();
            }
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            if (vm.Image != null)
            {
                page.ImageUrl = GetImagePath(vm.Image);
            }
            await _context.SaveChangesAsync();
            _notification.Success("About Page Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }
        [HttpGet]
        public async Task<IActionResult> Contact()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "contact");
            var pageVm = new PageViewModel()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(pageVm);
        }
        [HttpPost]
        public async Task<IActionResult> Contact(PageViewModel vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "contact");
            if (page == null)
            {
                _notification.Error("Page not found");
                return View();
            }
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            if (vm.Image != null)
            {
                page.ImageUrl = GetImagePath(vm.Image);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Contact Page Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }
        [HttpGet]
        public async Task<IActionResult> Privacy()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "privacy");
            var pageVm = new PageViewModel()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ImageUrl = page.ImageUrl,
            };
            return View(pageVm);
        }
        [HttpPost]
        public async Task<IActionResult> Privacy(PageViewModel vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "privacy");
            if (page == null)
            {
                _notification.Error("Page not found");
                return View();
            }
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            if (vm.Image != null)
            {
                page.ImageUrl = GetImagePath(vm.Image);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Privacy Page Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }
        private string GetImagePath(IFormFile formFile)
        {
            string uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                formFile.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}
