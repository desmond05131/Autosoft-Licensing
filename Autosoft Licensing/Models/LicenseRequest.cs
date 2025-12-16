using System;
using Newtonsoft.Json;

namespace Autosoft_Licensing.Models
{
    public class LicenseRequest
    {
        public string CompanyName { get; set; }
        public string DealerCode { get; set; }
        public string ProductID { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime RequestDateUtc { get; set; }
        public string LicenseKey { get; set; }

        // --- FIX IS HERE ---

        // Fix 1: Ensure LicenseType handles nulls (explicitly marked optional)
        [JsonProperty(Required = Required.Default)]
        public string LicenseType { get; set; }

        // FIX: Change 'int' to 'int?' so it can accept 'null'
        [JsonProperty(Required = Required.Default)]
        public int? RequestedPeriodMonths { get; set; }

        // Optional: Support the alias if your plugin sends "RequestedPeriod" instead
        [JsonProperty("RequestedPeriod")]
        private int? _requestedPeriodCompat
        {
            set { RequestedPeriodMonths = value; }
        }
    }
}
