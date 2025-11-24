using System;
using System.Collections.Generic;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.Services
{
    public interface ILicenseDatabaseService
    {
        // Users
        User GetUserByUsername(string username);
        User GetUserById(int id);

        // Admin CRUD for users
        IEnumerable<User> GetUsers();
        int InsertUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);

        // License Requests
        int InsertLicenseRequest(LicenseRequestRecord record);
        LicenseRequestRecord GetLicenseRequestById(int id);
        IEnumerable<LicenseRequestRecord> GetLicenseRequests(string productId = null);
        void SetRequestModules(int requestId, IEnumerable<string> moduleCodes);

        // Licenses
        int InsertLicense(LicenseMetadata meta);
        LicenseMetadata GetLicenseById(int id);
        LicenseMetadata GetActiveLicense(string productId, string companyName);
        IEnumerable<LicenseMetadata> GetLicenses(string productId = null);
        void SetLicenseModules(int licenseId, IEnumerable<string> moduleCodes);

        // Products (admin CRUD)
        IEnumerable<Product> GetProducts();
        Product GetProductById(int id);
        Product GetProductByProductId(string productId);
        int InsertProduct(Product product);
        void UpdateProduct(Product product);
        void DeleteProduct(int id);

        // Dealers (admin CRUD)
        IEnumerable<Dealer> GetDealers();
        Dealer GetDealerById(int id);
        Dealer GetDealerByCode(string dealerCode);
        int InsertDealer(Dealer dealer);
        void UpdateDealer(Dealer dealer);
        void DeleteDealer(int id);

        // Misc
        bool LicenseKeyExists(string licenseKey);
        void UpdateLicenseStatus(int licenseId, string status);

        // Summary for validation
        bool TryGetLatestLicenseSummary(string productId, string companyName,
            out string licenseType, out DateTime validFromUtc, out DateTime validToUtc, out string status);

        // New Methods
        IEnumerable<ModuleDto> GetModulesForProduct(string productId);

        // Duplicate check: exact-match on CompanyName + ProductID + ValidFromUtc + ValidToUtc
        bool ExistsDuplicateLicense(string companyName, string productId, DateTime validFromUtc, DateTime validToUtc);
    }
}