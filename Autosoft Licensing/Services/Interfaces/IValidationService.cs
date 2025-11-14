using System.ComponentModel.DataAnnotations;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public interface IValidationService
    {
        ValidationResult ValidateArl(LicenseRequest r);
        ValidationResult ValidateAslData(LicenseData d); // basic shape checks only
    }
}