namespace BeReal.Models
{
    public class Document
    {
        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? Data { get; set; }
    }
}
