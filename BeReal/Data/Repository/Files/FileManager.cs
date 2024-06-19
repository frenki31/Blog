using BeReal.Models;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
namespace BeReal.Data.Repository.Files
{
    public class FileManager : IFileManager
    {
        private readonly ApplicationDbContext _context;
        public FileManager(IConfiguration config, ApplicationDbContext context)
        {
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
        public async Task<BR_Document> GetFileInfo(IFormFile file, int id, List<string> suffixes)
        {
            string suffix = file.FileName.Substring(file.FileName.LastIndexOf('.'));
            if (suffixes.Contains(suffix))
            {
                string fileName = Path.GetFileName(file.FileName);
                string contentType = file.ContentType;
                byte[] data;
                if (contentType.StartsWith("image/"))
                {
                    using var image = Image.Load(file.OpenReadStream());
                    image.Mutate(x => x.Resize(800, 600)); 
                    var quality = 75;
                    var encoder = new JpegEncoder { Quality = quality };
                    using var memoryStream = new MemoryStream();
                    image.Save(memoryStream, encoder);
                    data = memoryStream.ToArray();
                }
                else
                {
                    using var memoryStream = new MemoryStream();
                    await file.CopyToAsync(memoryStream);
                    data = memoryStream.ToArray();
                }

                return new BR_Document { IDBR_Document = id, FileName = fileName, ContentType = contentType, Data = data };
            }
            return new BR_Document { };
        }
        public void AddFile(BR_Document file) => _context.BR_Files.Add(file);
        public async Task<BR_Document?> GetFileById(int? id) => await _context.BR_Files.FirstOrDefaultAsync(f => f.IDBR_Document == id);
        public async Task<(byte[], string, string)> GetFile(int? id, IFileManager _fileManager)
        {
            var file = await _fileManager.GetFileById(id);
            return (file!.Data!, file.ContentType!, file.FileName!);
        }
    }
}
