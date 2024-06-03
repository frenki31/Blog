using AspNetCoreHero.ToastNotification.Abstractions;
using BeReal.Data;
using BeReal.ViewModels;
using BeReal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeReal.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles="Admin")]
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public INotyfService _notification { get; }
        public SettingController(ApplicationDbContext context, 
            IWebHostEnvironment webHostEnvironment,
            INotyfService notification)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;   
            _notification = notification;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = _context.Settings.ToList();
            if (settings.Count > 0)
            {
                var vm = new SettingViewModel()
                {
                    Id = settings[0].Id,
                    Name = settings[0].Name,
                    Title = settings[0].Title,
                    Description = settings[0].Description,
                    FacebookUrl = settings[0].FacebookUrl,
                    GithubUrl = settings[0].GithubUrl,
                    ImageUrl = settings[0].ImageUrl,
                    TwitterUrl = settings[0].TwitterUrl,
                };
                return View(vm);    
            }
            var setting = new Setting()
            {
                Name = "Demo Site",
            };
            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();
            var createdSettings = _context.Settings.ToList();
            var createdVm = new SettingViewModel()
            {
                Id = createdSettings[0].Id,
                Name = createdSettings[0].Name,
                Title = createdSettings[0].Title,
                Description = createdSettings[0].Description,
                FacebookUrl = createdSettings[0].FacebookUrl,
                GithubUrl = createdSettings[0].GithubUrl,
                ImageUrl = createdSettings[0].ImageUrl,
                TwitterUrl = createdSettings[0].TwitterUrl,
            };
            return View(createdVm);
        }
        [HttpPost]
        public async Task<IActionResult> Index(SettingViewModel vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var setting = await _context.Settings.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (setting == null) {
                _notification.Error("Something went wrong");
                return View(vm);
            }
            setting.Name = vm.Name;
            setting.Title = vm.Title;
            setting.Description = vm.Description;
            setting.FacebookUrl = vm.FacebookUrl;
            setting.GithubUrl = vm.GithubUrl;
            setting.TwitterUrl = vm.TwitterUrl;
            if (vm.Image != null)
            {
                setting.ImageUrl = GetImagePath(vm.Image);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Setting updated successfully");
            return RedirectToAction("Index", "Post", new {area = "Admin"});
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
