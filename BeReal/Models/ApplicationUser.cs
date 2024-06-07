﻿using Microsoft.AspNetCore.Identity;

namespace BeReal.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public List<Post>? Posts { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
