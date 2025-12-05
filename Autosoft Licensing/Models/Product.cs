using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Autosoft_Licensing.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string ProductID { get; set; } // Business key

        [MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }
        public string ReleaseNotes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime LastModifiedUtc { get; set; }

        // NEW: Soft delete flag
        public bool IsDeleted { get; set; }

        public List<Module> Modules { get; set; } = new List<Module>();
    }
}