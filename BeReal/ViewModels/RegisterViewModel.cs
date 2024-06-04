using System.ComponentModel.DataAnnotations;

namespace BeReal.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string? ConfirmPassword { get; set; }
        public bool isAdmin { get; set; }
    }
}
