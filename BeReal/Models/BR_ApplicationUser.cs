using Microsoft.AspNetCore.Identity;

namespace BeReal.Models
{
    public class BR_ApplicationUser: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<BR_Post>? Posts { get; set; }
        public List<BR_Comment>? Comments { get; set; }
    }
}
