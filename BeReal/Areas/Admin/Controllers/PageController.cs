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
        private readonly string _imagePath;
        public INotyfService _notification { get; }
        public PageController(ApplicationDbContext context,
            IConfiguration config,
            INotyfService notification)
        {
            _context = context;
            _imagePath = config["Path:Images"]!;   
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
                if (page.ImageUrl != null)
                    RemoveImage(page.ImageUrl);
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
                if (page.ImageUrl != null)
                    RemoveImage(page.ImageUrl);
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
                if (page.ImageUrl != null) 
                    RemoveImage(page.ImageUrl);
                page.ImageUrl = GetImagePath(vm.Image);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Privacy Page Updated Successfully");
            return RedirectToAction("Index", "Post", new { area = "Admin" });

        }
        private string GetImagePath(IFormFile formFile)
        {
            var folderPath = Path.Combine(_imagePath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            var suffix = formFile.FileName.Substring(formFile.FileName.LastIndexOf('.'));
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
