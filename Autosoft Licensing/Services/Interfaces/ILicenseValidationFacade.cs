using System;

namespace Autosoft_Licensing.Services
{
    public interface ILicenseValidationFacade
    {
        // Returns true if license is valid. Provides a user-facing message and whether the plugin tab should be hidden.
        bool IsLicenseValid(string productId, string companyName, out string message, out bool hidePluginTab);
    }
}