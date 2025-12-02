using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Autosoft_Licensing.Models
{
    // Plaintext JSON (.ARL) — simplified to the new schema (no ModuleCodes).
    public class LicenseRequest
    {
        [Required, MinLength(1)]
        public string CompanyName { get; set; }

        [Required, Range(1, 1200)]
        public int RequestedPeriodMonths { get; set; }

        [Required, MinLength(1)]
        public string DealerCode { get; set; }

        [Required, MinLength(1)]
        public string ProductID { get; set; }

        // Now a string with allowed values "Demo" or "Paid" (case-sensitive)
        [Required, MinLength(1)]
        public string LicenseType { get; set; }

        [Required, MinLength(1)]
        public string LicenseKey { get; set; }

        public string CurrencyCode { get; set; }

        [Required]
        public DateTime RequestDateUtc { get; set; }

        public string ToJson() =>
            JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}