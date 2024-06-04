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
        private readonly string _imagePath;
        public INotyfService _notification { get; }
        public SettingController(ApplicationDbContext context, 
            IConfiguration config,
            INotyfService notification)
        {
            _context = context;
            _imagePath = config["Path:Images"]!;   
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
                if (setting.ImageUrl != null) 
                    RemoveImage(setting.ImageUrl);
                setting.ImageUrl = GetImagePath(vm.Image);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Setting updated successfully");
            return RedirectToAction("Index", "Post", new {area = "Admin"});
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
