﻿using BeReal.Models;
namespace BeReal.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int? NumberPosts { get; set; }
        public int? NumberComments { get; set; }
        public List<BR_Post>? Posts { get; set; }
    }
}
