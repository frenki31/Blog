namespace BeReal.ViewModels
{
    public class FileViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public IFormFile? UploadedFile { get; set; }
    }
}
