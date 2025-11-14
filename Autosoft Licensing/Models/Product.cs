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
    }
}