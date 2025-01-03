﻿using Domain.Models.Management;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models.Auth
{
    public class User : IdentityUser
    {
        public string? FullName {  get; set; }
        public string? Image {  get; set; }
        public DateOnly? BirthDate { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Relationship
        public ICollection<Category> Categories { get; set; } = new List<Category>();           
    }
}
