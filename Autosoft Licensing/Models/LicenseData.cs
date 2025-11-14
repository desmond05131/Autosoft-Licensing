using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Autosoft_Licensing.Models.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Autosoft_Licensing.Models
{
    // Decrypted JSON inside .ASL
    public class LicenseData
    {
        [Required, MinLength(1)]
        public string CompanyName { get; set; }

        [Required, MinLength(1)]
        public string ProductID { get; set; }

        [Required, MinLength(1)]
        public string DealerCode { get; set; }

        public string CurrencyCode { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public LicenseType LicenseType { get; set; }

        [Required]
        public DateTime ValidFromUtc { get; set; }

        [Required]
        public DateTime ValidToUtc { get; set; }

        [Required, MinLength(1)]
        public string LicenseKey { get; set; }

        // Activated modules (ModuleCode list)
        public List<string> ModuleCodes { get; set; } = new List<string>();

        [Required, MinLength(1)]
        public string ChecksumSHA256 { get; set; }
    }
}
