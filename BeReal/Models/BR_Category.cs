using System.ComponentModel.DataAnnotations;

namespace BeReal.Models
{
    public class BR_Category
    {
        [Key]
        public int IDBR_Category { get; set; }
        public string? Name { get; set; }
        public BR_Category? ParentCategory { get; set; }
        public List<BR_Category>? Subcategories { get; set; }
    }
}
