using System;
using System.Collections.Generic;
using Autosoft_Licensing.Models.Enums;

namespace Autosoft_Licensing.Models
{
    public class LicenseRequestRecord
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ProductID { get; set; }
        public string DealerCode { get; set; }
        public LicenseType LicenseType { get; set; }
        public int RequestedPeriodMonths { get; set; }
        public string LicenseKey { get; set; }
        public string CurrencyCode { get; set; }
        public DateTime RequestDateUtc { get; set; }
        public int CreatedByUserId { get; set; }
        public string RequestFileBase64 { get; set; }
        public List<string> ModuleCodes { get; set; } = new List<string>();
    }
}