using BeReal.Models;
using BeReal.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace BeReal.Data.Repository.Files
{
    public class FileManager : IFileManager
    {
        private readonly string _imagePath;
        private readonly ApplicationDbContext _context;
        public FileManager(IConfiguration config, ApplicationDbContext context)
        {
            _imagePath = config["Path:Images"]!;
            _context = context;
        }
        public List<int> Pages(int PageNumber, int PageCount)
        {
            List<int> pages = new List<int>();
            if (PageCount <= 5)
            {
                for (int i = 1; i <= PageCount; i++)
                    pages.Add(i);
            }
            else
            {
                int mid = PageNumber < 3 ? 3 : PageNumber > PageCount ? PageCount - 2 : PageNumber;
                for (int i = mid - 2; i <= mid + 2; i++)
                    pages.Add(i);
                if (pages[0] != 1)
                {
                    pages.Insert(0, 1);
                    if (pages[1] - pages[0] > 1)
                        pages.Insert(1, -1);
                }
                if (pages[pages.Count - 1] != PageCount)
                {
                    pages.Insert(pages.Count, PageCount);
                    if (pages[pages.Count - 1] - pages[pages.Count - 2] > 1)
                        pages.Insert(pages.Count - 1, -1);
                }
            }
            return pages;
        }
        public string GetImagePath(IFormFile formFile)
        {
            var folderPath = Path.Combine(_imagePath);
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            var suffix = formFile.FileName.Substring(formFile.FileName.LastIndexOf('.'));
            var uniqueFileName = $"img_{DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss")}{suffix}";
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = File.Create(filePath))
            {
                formFile.CopyToAsync(fileStream).GetAwaiter().GetResult();
            }
            return uniqueFileName;
        }
        public bool RemoveImage(string image)
        {
            try
            {
                var file = Path.Combine(_imagePath, image);
                if (File.Exists(file))
                    File.Delete(file);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public async Task<BR_Document> GetFileInfo(CreatePostViewModel vm)
        {
            List<string> suffixes = new List<string> { ".pdf", ".docx", ".xlsx", ".csv" };
            string suffix = vm.File!.UploadedFile!.FileName.Substring(vm.File.UploadedFile.FileName.LastIndexOf('.'));
            if (suffixes.Contains(suffix))
            {
                string fileName = Path.GetFileName(vm.File.UploadedFile.FileName);
                string contentType = vm.File.UploadedFile.ContentType;
                using (var memoryStream = new MemoryStream())
                {
                    await vm.File.UploadedFile.CopyToAsync(memoryStream);
                    return new BR_Document { IDBR_Document = vm.File!.Id, FileName = fileName, ContentType = contentType, Data = memoryStream.ToArray() };
                }
            }
            return new BR_Document { };
        }
        public void AddFile(BR_Document file) => _context.Files.Add(file);
        public async Task<BR_Document?> GetFileById(int? id) => await _context.Files.FirstOrDefaultAsync(f => f.IDBR_Document == id);
    }
}
