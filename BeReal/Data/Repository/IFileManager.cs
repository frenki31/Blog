using BeReal.Models;
using BeReal.ViewModels;

namespace BeReal.Data.Repository
{
    public interface IFileManager
    {
        List<int> Pages(int PageNumber, int PageCount);
        string GetImagePath(IFormFile formFile);
        bool RemoveImage(string image);
        void AddFile(Document file);
        Task<Document?> GetFileById(int? id);
        Task<Document> GetFileInfo(CreatePostViewModel vm);
    }
}
