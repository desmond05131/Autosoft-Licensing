using System;
using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Autosoft_Licensing.Models.Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    namespace Autosoft_Licensing.Models
    {
        // Plaintext JSON (.ARL)
        public class LicenseRequest
    {
        [Required, MinLength(1)]
        public string CompanyName { get; set; }

        [Required, MinLength(1)]
        public string ProductID { get; set; }

        [Required, MinLength(1)]
        public string DealerCode { get; set; }

        [Required, Range(1, 1200)]
        public int RequestedPeriodMonths { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        public LicenseType LicenseType { get; set; }

        [Required, MinLength(1)]
        public string LicenseKey { get; set; }

        public string CurrencyCode { get; set; }

        [Required]
        public DateTime RequestDateUtc { get; set; }

        // Modules requested (by ModuleCode for portability)
        public List<string> ModuleCodes { get; set; } = new List<string>();

        public string ToJson() =>
            JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}