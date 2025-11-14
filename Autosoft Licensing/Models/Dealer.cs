using System;
using System.ComponentModel.DataAnnotations;

namespace Autosoft_Licensing.Models
{
    public class Dealer
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string DealerCode { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        public DateTime CreatedUtc { get; set; }
    }
}