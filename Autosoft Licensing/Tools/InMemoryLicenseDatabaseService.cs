using System;
using System.Collections.Generic;
using System.Linq;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.Tools
{
    /// <summary>
    /// Lightweight in-memory implementation of ILicenseDatabaseService used for tests/smoke runs
    /// when a real SQL Server instance is not available. Minimal, self-seeding behaviour to satisfy
    /// SmokeTestHarness and E2E fallback flows.
    /// </summary>
    internal class InMemoryLicenseDatabaseService : ILicenseDatabaseService
    {
        private readonly List<User> _users = new List<User>();
        private readonly List<Product> _products = new List<Product>();
        private readonly List<Module> _modules = new List<Module>();
        private readonly List<Dealer> _dealers = new List<Dealer>();
        private readonly List<LicenseRequestRecord> _requests = new List<LicenseRequestRecord>();
        private readonly List<LicenseMetadata> _licenses = new List<LicenseMetadata>();

        // In-memory AppSettings (global key/value)
        private readonly Dictionary<string, string> _settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private int _nextUserId = 1;
        private int _nextProductId = 1;
        private int _nextModuleId = 1;
        private int _nextDealerId = 1;
        private int _nextRequestId = 1;
        private int _nextLicenseId = 1;

        public InMemoryLicenseDatabaseService()
        {
            // seed an admin user (username: admin, password hash is SHA256("admin") as used in Seed.sql)
            _users.Add(new User
            {
                Id = _nextUserId++,
                Username = "admin",
                DisplayName = "System Administrator",
                Role = "Admin",
                Email = "admin@example.com",
                PasswordHash = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918",
                CreatedUtc = DateTime.UtcNow
            });

            // seed a dealer
            _dealers.Add(new Dealer { Id = _nextDealerId++, DealerCode = "DEALER-001", Name = "Default Dealer", CreatedUtc = DateTime.UtcNow });

            // seed a sample product and modules to satisfy any lookup needs
            var prod = new Product { Id = _nextProductId++, ProductID = "SAMPLE-PRODUCT", Name = "Sample Product", IsDeleted = false, Modules = new List<Module>() };
            _products.Add(prod);

            _modules.Add(new Module { Id = _nextModuleId++, ProductId = prod.Id, ModuleCode = "MODULE-001", Name = "Module 1", IsActive = true });
            _modules.Add(new Module { Id = _nextModuleId++, ProductId = prod.Id, ModuleCode = "MODULE-002", Name = "Module 2", IsActive = true });
            _modules.Add(new Module { Id = _nextModuleId++, ProductId = prod.Id, ModuleCode = "MODULE-003", Name = "Module 3", IsActive = true });

            // Seed default settings to match GeneralSettingPage expectations
            _settings["Duration_Demo_Days"] = "30";
            _settings["Duration_Sub_Months"] = "12";
            _settings["Duration_Perm_Years"] = "10";
        }

        // --- Users ---
        public User GetUserByUsername(string username) => _users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        public User GetUserById(int id) => _users.FirstOrDefault(u => u.Id == id);

        public IEnumerable<User> GetUsers() => _users.ToList();

        public int InsertUser(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            user.Id = _nextUserId++;
            user.CreatedUtc = user.CreatedUtc == default ? DateTime.UtcNow : user.CreatedUtc;
            _users.Add(user);
            return user.Id;
        }

        public void UpdateUser(User user)
        {
            var existing = GetUserById(user.Id);
            if (existing == null) throw new InvalidOperationException("User not found");
            existing.Username = user.Username;
            existing.DisplayName = user.DisplayName;
            existing.Role = user.Role;
            existing.Email = user.Email;
            existing.PasswordHash = user.PasswordHash;
        }

        public void DeleteUser(int id)
        {
            var u = GetUserById(id);
            if (u != null) _users.Remove(u);
        }

        // --- License Requests ---
        public int InsertLicenseRequest(LicenseRequestRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            record.Id = _nextRequestId++;
            _requests.Add(record);
            return record.Id;
        }

        public LicenseRequestRecord GetLicenseRequestById(int id)
        {
            return _requests.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<LicenseRequestRecord> GetLicenseRequests(string productId = null)
        {
            return _requests.Where(r => productId == null || r.ProductID == productId).ToList();
        }

        public void SetRequestModules(int requestId, IEnumerable<string> moduleCodes)
        {
            var rec = GetLicenseRequestById(requestId);
            if (rec == null) throw new InvalidOperationException("Request not found");
            rec.ModuleCodes = moduleCodes?.ToList() ?? new List<string>();
        }

        // --- Licenses ---
        public int InsertLicense(LicenseMetadata meta)
        {
            if (meta == null) throw new ArgumentNullException(nameof(meta));
            meta.Id = _nextLicenseId++;
            meta.ImportedOnUtc = meta.ImportedOnUtc == default ? DateTime.UtcNow : meta.ImportedOnUtc;
            _licenses.Add(meta);
            // persist module codes on object
            meta.ModuleCodes = meta.ModuleCodes ?? new List<string>();
            return meta.Id;
        }

        public LicenseMetadata GetLicenseById(int id)
        {
            var m = _licenses.FirstOrDefault(x => x.Id == id);
            if (m == null) return null;
            // return a shallow copy to mimic DB readback behaviour
            return new LicenseMetadata
            {
                Id = m.Id,
                CompanyName = m.CompanyName,
                ProductID = m.ProductID,
                DealerCode = m.DealerCode,
                LicenseKey = m.LicenseKey,
                LicenseType = m.LicenseType,
                ValidFromUtc = m.ValidFromUtc,
                ValidToUtc = m.ValidToUtc,
                CurrencyCode = m.CurrencyCode,
                Status = m.Status,
                ImportedOnUtc = m.ImportedOnUtc,
                ImportedByUserId = m.ImportedByUserId,
                RawAslBase64 = m.RawAslBase64,
                Remarks = m.Remarks,
                ModuleCodes = m.ModuleCodes == null ? new List<string>() : new List<string>(m.ModuleCodes)
            };
        }

        public LicenseMetadata GetActiveLicense(string productId, string companyName)
        {
            return _licenses
                .Where(l => l.ProductID == productId && l.CompanyName == companyName)
                .OrderByDescending(l => l.ImportedOnUtc)
                .ThenByDescending(l => l.Id)
                .Select(l => GetLicenseById(l.Id))
                .FirstOrDefault();
        }

        public IEnumerable<LicenseMetadata> GetLicenses(string productId = null)
        {
            var query = _licenses.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(productId))
                query = query.Where(l => l.ProductID == productId);
            // return copies
            return query.Select(l => GetLicenseById(l.Id)).ToList();
        }

        public void SetLicenseModules(int licenseId, IEnumerable<string> moduleCodes)
        {
            var meta = _licenses.FirstOrDefault(l => l.Id == licenseId);
            if (meta == null) throw new InvalidOperationException("License not found");
            meta.ModuleCodes = moduleCodes?.ToList() ?? new List<string>();
        }

        // --- Products ---
        public Product GetProductByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            return _products.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Product> GetProducts()
        {
            return _products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _products.FirstOrDefault(p => p.Id == id);
        }

        public Product GetProductByProductId(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId)) return null;
            return _products.FirstOrDefault(p => string.Equals(p.ProductID, productId, StringComparison.OrdinalIgnoreCase));
        }

        public int InsertProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            product.Id = _nextProductId++;
            // clone shallow to avoid external mutation
            _products.Add(new Product
            {
                Id = product.Id,
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                ReleaseNotes = product.ReleaseNotes,
                CreatedBy = product.CreatedBy,
                CreatedUtc = product.CreatedUtc,
                LastModifiedUtc = product.LastModifiedUtc,
                IsDeleted = product.IsDeleted,
                Modules = product.Modules != null ? product.Modules.Select(m => new Module
                {
                    ProductId = product.Id,
                    ModuleCode = m.ModuleCode,
                    Name = m.Name,
                    Description = m.Description,
                    IsActive = m.IsActive
                }).ToList() : new List<Module>()
            });
            return product.Id;
        }

        public void UpdateProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            var existing = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existing == null) return;

            existing.ProductID = product.ProductID;
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.ReleaseNotes = product.ReleaseNotes;
            existing.CreatedBy = product.CreatedBy;
            existing.CreatedUtc = product.CreatedUtc;
            existing.LastModifiedUtc = product.LastModifiedUtc;
            existing.IsDeleted = product.IsDeleted;

            // replace modules
            existing.Modules = product.Modules != null ? product.Modules.Select(m => new Module
            {
                ProductId = product.Id,
                ModuleCode = m.ModuleCode,
                Name = m.Name,
                Description = m.Description,
                IsActive = m.IsActive
            }).ToList() : new List<Module>();
        }

        public void DeleteProduct(int id)
        {
            // Soft-delete to mirror SQL implementation (IsDeleted = 1)
            var existing = _products.FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                existing.IsDeleted = true;
                existing.LastModifiedUtc = DateTime.UtcNow;
            }
        }

        public void RestoreProduct(int id)
        {
            // Restore soft-deleted product (IsDeleted = 0)
            var existing = _products.FirstOrDefault(p => p.Id == id);
            if (existing != null)
            {
                existing.IsDeleted = false;
                existing.LastModifiedUtc = DateTime.UtcNow;
            }
        }

        public IEnumerable<ModuleDto> GetModulesForProduct(string productId)
        {
            var product = GetProductByProductId(productId);
            if (product?.Modules == null) return Enumerable.Empty<ModuleDto>();
            return product.Modules
                .Where(m => m.IsActive)
                .Select(m => new ModuleDto
                {
                    ModuleCode = m.ModuleCode,
                    ModuleName = string.IsNullOrWhiteSpace(m.Name) ? m.ModuleCode : m.Name,
                    Description = m.Description
                })
                .ToList();
        }

        // --- Dealers ---
        public IEnumerable<Dealer> GetDealers() => _dealers.ToList();

        public Dealer GetDealerById(int id) => _dealers.FirstOrDefault(d => d.Id == id);

        public Dealer GetDealerByCode(string dealerCode) => _dealers.FirstOrDefault(d => d.DealerCode == dealerCode);

        public int InsertDealer(Dealer dealer)
        {
            dealer.Id = _nextDealerId++;
            dealer.CreatedUtc = dealer.CreatedUtc == default ? DateTime.UtcNow : dealer.CreatedUtc;
            _dealers.Add(dealer);
            return dealer.Id;
        }

        public void UpdateDealer(Dealer dealer)
        {
            var d = GetDealerById(dealer.Id);
            if (d == null) throw new InvalidOperationException("Dealer not found");
            d.DealerCode = dealer.DealerCode;
            d.Name = dealer.Name;
        }

        public void DeleteDealer(int id)
        {
            var d = GetDealerById(id);
            if (d != null) _dealers.Remove(d);
        }

        // --- Misc ---
        public bool ExistsDuplicateLicense(string companyName, string productId, DateTime validFromUtc, DateTime validToUtc)
        {
            return _licenses.Any(l => l.CompanyName == companyName && l.ProductID == productId
                && l.ValidFromUtc == validFromUtc && l.ValidToUtc == validToUtc);
        }

        public bool LicenseKeyExists(string licenseKey)
        {
            return _licenses.Any(l => l.LicenseKey == licenseKey);
        }

        public void UpdateLicenseStatus(int licenseId, string status)
        {
            var l = _licenses.FirstOrDefault(x => x.Id == licenseId);
            if (l != null)
            {
                if (Enum.TryParse(status, true, out Models.Enums.LicenseStatus parsedStatus))
                {
                    l.Status = parsedStatus;
                }
            }
        }

        public bool TryGetLatestLicenseSummary(string productId, string companyName, out string licenseType, out DateTime validFromUtc, out DateTime validToUtc, out string status)
        {
            licenseType = string.Empty;
            validFromUtc = DateTime.MinValue;
            validToUtc = DateTime.MinValue;
            status = string.Empty;

            var l = GetActiveLicense(productId, companyName);
            if (l == null) return false;
            licenseType = l.LicenseType.ToString();
            validFromUtc = l.ValidFromUtc;
            validToUtc = l.ValidToUtc;
            status = l.Status.ToString();
            return true;
        }

        // --- App Settings (global key/value) ---
        public string GetSetting(string key, string defaultValue)
        {
            if (key == null) return defaultValue;
            if (_settings.TryGetValue(key, out var val))
                return val ?? defaultValue;
            return defaultValue;
        }

        public void SaveSetting(string key, string value)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            _settings[key] = value;
        }
    }
}