using System.ComponentModel.DataAnnotations;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public interface ILicenseRequestService
    {
        ValidationResult ValidateRequest(LicenseRequest r);
        string ToArlJson(LicenseRequest r);             // returns JSON text
        void SaveArl(string path, LicenseRequest r);     // writes .ARL
    }
}