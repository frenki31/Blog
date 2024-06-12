using System.ComponentModel.DataAnnotations;

namespace BeReal.Models
{
    public class BR_Comment
    {
        [Key]
        public int IDBR_Comment { get; set; }
        public string? Message { get; set; }
        public DateTime Created { get; set; }
        public BR_Post? Post { get; set; }
        public BR_ApplicationUser? ApplicationUser { get; set; }
        public BR_Comment? ParentComment { get; set; }
        public List<BR_Comment>? Replies { get; set; }
    }
}
