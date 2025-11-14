using System.ComponentModel.DataAnnotations;

namespace Autosoft_Licensing.Models
{
    public class Module
    {
        public int Id { get; set; }
        public int ProductId { get; set; }

        [Required, MaxLength(100)]
        public string ModuleCode { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(4000)]
        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}