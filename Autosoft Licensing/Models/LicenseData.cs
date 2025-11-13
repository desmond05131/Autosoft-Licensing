using System;
using System.ComponentModel.DataAnnotations;

namespace Autosoft_Licensing.Models
{
    // Decrypted content of .ASL
    public class LicenseData
    {
        [Required, MinLength(1)]
        public string CompanyName { get; set; }
        [Required, MinLength(1)]
        public string ProductID { get; set; }
        [Required, MinLength(1)]
        public string DealerCode { get; set; }
        public string CurrencyCode { get; set; }
        [Required, RegularExpression("Demo|Paid")]
        public string LicenseType { get; set; }
        [Required]
        public DateTime ValidFromUtc { get; set; }
        [Required]
        public DateTime ValidToUtc { get; set; }
        [Required, MinLength(1)]
        public string LicenseKey { get; set; }
        [Required, MinLength(1)]
        public string ChecksumSHA256 { get; set; }
    }
}
