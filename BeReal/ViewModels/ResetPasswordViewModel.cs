using System.ComponentModel.DataAnnotations;
namespace BeReal.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$")]
        public string? NewPassword { get; set; }
        [Required]
        [Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; set; }
        
    }
}
