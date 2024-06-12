using System.ComponentModel.DataAnnotations;

namespace BeReal.Models
{
    public class BR_Document
    {
        [Key]
        public int IDBR_Document { get; set; }
        public string? FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? Data { get; set; }
    }
}
