using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public interface ILicenseRequestService
    {
        string SerializeToArl(LicenseRequest request);
        void SaveArl(string path, LicenseRequest request);
    }
}