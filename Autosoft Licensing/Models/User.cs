using System;
using System.ComponentModel.DataAnnotations;

namespace Autosoft_Licensing.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Username { get; set; }

        [Required, MaxLength(200)]
        public string DisplayName { get; set; }

        [Required, MaxLength(20)]
        public string Role { get; set; } // Admin | Support

        [MaxLength(200)]
        public string Email { get; set; }

        [Required, MaxLength(512)]
        public string PasswordHash { get; set; }

        public DateTime CreatedUtc { get; set; }

        // New granular permissions and status
        public bool IsActive { get; set; }
        public bool CanGenerateLicense { get; set; }
        public bool CanViewRecords { get; set; }
        public bool CanManageProduct { get; set; }
        public bool CanManageUsers { get; set; }
    }
}