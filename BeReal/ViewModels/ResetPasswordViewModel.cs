using System.ComponentModel.DataAnnotations;
namespace BeReal.ViewModels
{
    public class ResetPasswordViewModel
    {
        public string? Id { get; set; }
        public string? Username { get; set; }
        [Required]
        public string? NewPassword { get; set; }
        [Required]
        [Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; set; }
        
    }
}
