﻿using System.ComponentModel.DataAnnotations;

namespace SimbirSoft_Web_API.Models
{
    public partial class Account
    {
        public Account()
        {
            Animals = new HashSet<Animal>();
        }

        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = "USER";

        public virtual ICollection<Animal> Animals { get; set; }
    }
}
