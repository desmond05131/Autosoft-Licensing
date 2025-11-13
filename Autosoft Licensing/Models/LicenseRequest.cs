using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Autosoft_Licensing.Models
{
    public class LicenseRequest
    {
        [Required, MinLength(1)]
        public string CompanyName { get; set; }
        [Required, MinLength(1)]
        public string ProductID { get; set; }
        [Required, MinLength(1)]
        public string DealerCode { get; set; }
        [Required]
        [Range(1, 1200)]
        public int RequestedPeriodMonths { get; set; }
        [Required]
        [RegularExpression("Demo|Paid")]
        public string LicenseType { get; set; }
        [Required, MinLength(1)]
        public string LicenseKey { get; set; }
        public string CurrencyCode { get; set; }
        [Required]
        public DateTime RequestDateUtc { get; set; }

        public string ToJson() =>
            Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
    }
}