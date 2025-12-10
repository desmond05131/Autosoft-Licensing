using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Autosoft_Licensing.Data;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Models.Enums;

namespace Autosoft_Licensing.Services
{
    public class LicenseDatabaseService : ILicenseDatabaseService
    {
        private readonly SqlConnectionFactory _factory;

        public LicenseDatabaseService(SqlConnectionFactory factory)
        {
            _factory = factory;
        }

        // ---------- Users ----------
        public User GetUserByUsername(string username)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    Id, Username, DisplayName, Role, Email, PasswordHash, CreatedUtc,
    IsActive, CanGenerateLicense, CanViewRecords, CanManageProduct, CanManageUsers
FROM dbo.Users WHERE Username = @u;", conn);
            cmd.Parameters.AddWithValue("@u", username ?? (object)DBNull.Value);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return ReadUser(r);
        }

        public User GetUserById(int id)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT TOP(1)
    Id, Username, DisplayName, Role, Email, PasswordHash, CreatedUtc,
    IsActive, CanGenerateLicense, CanViewRecords, CanManageProduct, CanManageUsers
FROM dbo.Users WHERE Id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return ReadUser(r);
        }

        // Admin: list users
        public IEnumerable<User> GetUsers()
        {
            var list = new List<User>();
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT
    Id, Username, DisplayName, Role, Email, PasswordHash, CreatedUtc,
    IsActive, CanGenerateLicense, CanViewRecords, CanManageProduct, CanManageUsers
FROM dbo.Users
ORDER BY Username;", conn);
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(ReadUser(r));
            return list;
        }

        // Admin: insert user (returns new Id)
        public int InsertUser(User user)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.Users
(Username, DisplayName, Role, Email, PasswordHash, CreatedUtc,
 IsActive, CanGenerateLicense, CanViewRecords, CanManageProduct, CanManageUsers)
VALUES
(@Username, @DisplayName, @Role, @Email, @PasswordHash, @CreatedUtc,
 @IsActive, @CanGenerateLicense, @CanViewRecords, @CanManageProduct, @CanManageUsers);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@DisplayName", user.DisplayName);
            cmd.Parameters.AddWithValue("@Role", user.Role);
            cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@CreatedUtc", user.CreatedUtc == default ? DateTime.UtcNow : user.CreatedUtc);

            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            cmd.Parameters.AddWithValue("@CanGenerateLicense", user.CanGenerateLicense);
            cmd.Parameters.AddWithValue("@CanViewRecords", user.CanViewRecords);
            cmd.Parameters.AddWithValue("@CanManageProduct", user.CanManageProduct);
            cmd.Parameters.AddWithValue("@CanManageUsers", user.CanManageUsers);

            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        // Admin: update user
        public void UpdateUser(User user)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Users
SET Username = @Username,
    DisplayName = @DisplayName,
    Role = @Role,
    Email = @Email,
    PasswordHash = @PasswordHash,
    IsActive = @IsActive,
    CanGenerateLicense = @CanGenerateLicense,
    CanViewRecords = @CanViewRecords,
    CanManageProduct = @CanManageProduct,
    CanManageUsers = @CanManageUsers
WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", user.Id);
            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@DisplayName", user.DisplayName);
            cmd.Parameters.AddWithValue("@Role", user.Role);
            cmd.Parameters.AddWithValue("@Email", (object)user.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            cmd.Parameters.AddWithValue("@CanGenerateLicense", user.CanGenerateLicense);
            cmd.Parameters.AddWithValue("@CanViewRecords", user.CanViewRecords);
            cmd.Parameters.AddWithValue("@CanManageProduct", user.CanManageProduct);
            cmd.Parameters.AddWithValue("@CanManageUsers", user.CanManageUsers);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Admin: delete user
        public void DeleteUser(int id)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand("DELETE FROM dbo.Users WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        private static User ReadUser(IDataRecord r) => new User
        {
            Id = r.GetInt32(0),
            Username = r.GetString(1),
            DisplayName = r.GetString(2),
            Role = r.GetString(3),
            Email = r.IsDBNull(4) ? null : r.GetString(4),
            PasswordHash = r.GetString(5),
            CreatedUtc = r.GetDateTime(6),
            IsActive = !r.IsDBNull(7) && r.GetBoolean(7),
            CanGenerateLicense = !r.IsDBNull(8) && r.GetBoolean(8),
            CanViewRecords = !r.IsDBNull(9) && r.GetBoolean(9),
            CanManageProduct = !r.IsDBNull(10) && r.GetBoolean(10),
            CanManageUsers = !r.IsDBNull(11) && r.GetBoolean(11)
        };

        // ---------- Products ----------
        public IEnumerable<Product> GetProducts()
        {
            var list = new List<Product>();
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT Id, ProductID, Name, Description, ReleaseNotes, CreatedBy, CreatedUtc, LastModifiedUtc, IsDeleted
FROM dbo.Products
ORDER BY ProductID;", conn); // REMOVED: WHERE IsDeleted = 0
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(ReadProduct(r));
            return list;
        }

        public Product GetProductById(int id)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) Id, ProductID, Name, Description, ReleaseNotes, CreatedBy, CreatedUtc, LastModifiedUtc, IsDeleted
FROM dbo.Products WHERE Id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return ReadProduct(r);
        }

        public Product GetProductByProductId(string productId)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) Id, ProductID, Name, Description, ReleaseNotes, CreatedBy, CreatedUtc, LastModifiedUtc, IsDeleted
FROM dbo.Products WHERE ProductID = @pid;", conn);
            cmd.Parameters.AddWithValue("@pid", productId ?? (object)DBNull.Value);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return ReadProduct(r);
        }

        public Product GetProductByName(string name)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) Id, ProductID, Name, Description, ReleaseNotes, CreatedBy, CreatedUtc, LastModifiedUtc, IsDeleted
FROM dbo.Products
WHERE Name = @name AND IsDeleted = 0;", conn);
            cmd.Parameters.AddWithValue("@name", (object)name ?? DBNull.Value);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return ReadProduct(r);
        }

        public int InsertProduct(Product product)
        {
            using var conn = _factory.Create();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                int newId;
                using (var cmd = new SqlCommand(@"
INSERT INTO dbo.Products (ProductID, Name, Description, ReleaseNotes, CreatedBy, CreatedUtc, LastModifiedUtc)
VALUES (@ProductID, @Name, @Description, @ReleaseNotes, @CreatedBy, @CreatedUtc, @LastModifiedUtc);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                    cmd.Parameters.AddWithValue("@Name", (object)product.Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReleaseNotes", (object)product.ReleaseNotes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", (object)product.CreatedBy ?? DBNull.Value);
                    var createdUtc = product.CreatedUtc == default ? DateTime.UtcNow : product.CreatedUtc;
                    var modifiedUtc = product.LastModifiedUtc == default ? createdUtc : product.LastModifiedUtc;
                    cmd.Parameters.AddWithValue("@CreatedUtc", createdUtc);
                    cmd.Parameters.AddWithValue("@LastModifiedUtc", modifiedUtc);
                    newId = (int)cmd.ExecuteScalar();
                }

                // Insert modules for the new product
                foreach (var m in product.Modules ?? new List<Module>())
                {
                    using var ins = new SqlCommand(@"
INSERT INTO dbo.Modules (ProductId, ModuleCode, Name, Description, IsActive)
VALUES (@ProductId, @ModuleCode, @Name, @Description, @IsActive);", conn, tx);
                    ins.Parameters.AddWithValue("@ProductId", newId);
                    ins.Parameters.AddWithValue("@ModuleCode", m.ModuleCode);
                    ins.Parameters.AddWithValue("@Name", (object)m.Name ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@Description", (object)m.Description ?? DBNull.Value);
                    ins.Parameters.AddWithValue("@IsActive", m.IsActive);
                    ins.ExecuteNonQuery();
                }

                tx.Commit();
                return newId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void UpdateProduct(Product product)
        {
            using var conn = _factory.Create();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                // 1. Update Product Details
                using (var cmd = new SqlCommand(@"
UPDATE dbo.Products
SET ProductID = @ProductID,
    Name = @Name,
    Description = @Description,
    ReleaseNotes = @ReleaseNotes,
    LastModifiedUtc = @LastModifiedUtc
WHERE Id = @Id;", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@Id", product.Id);
                    cmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                    cmd.Parameters.AddWithValue("@Name", (object)product.Name ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", (object)product.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ReleaseNotes", (object)product.ReleaseNotes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@LastModifiedUtc", product.LastModifiedUtc == default ? DateTime.UtcNow : product.LastModifiedUtc);
                    cmd.ExecuteNonQuery();
                }

                // 2. Fetch existing modules to map Code -> Id
                var dbModules = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                using (var cmd = new SqlCommand("SELECT Id, ModuleCode FROM dbo.Modules WHERE ProductId = @pid;", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@pid", product.Id);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            var mid = r.GetInt32(0);
                            var mcode = r.GetString(1);
                            if (!dbModules.ContainsKey(mcode)) dbModules[mcode] = mid;
                        }
                    }
                }

                // 3. Upsert (Update or Insert)
                var incomingCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var m in product.Modules ?? new List<Module>())
                {
                    // Normalize nulls to avoid PK/UK violations on ModuleCode
                    var moduleCode = m.ModuleCode ?? string.Empty;
                    incomingCodes.Add(moduleCode);

                    if (dbModules.TryGetValue(moduleCode, out int existingId))
                    {
                        // UPDATE existing module by Id
                        using (var up = new SqlCommand(@"
UPDATE dbo.Modules 
SET Name = @Name, Description = @Description, IsActive = @IsActive 
WHERE Id = @Id;", conn, tx))
                        {
                            up.Parameters.AddWithValue("@Id", existingId);
                            up.Parameters.AddWithValue("@Name", (object)m.Name ?? DBNull.Value);
                            up.Parameters.AddWithValue("@Description", (object)m.Description ?? DBNull.Value);
                            up.Parameters.AddWithValue("@IsActive", m.IsActive);
                            up.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // INSERT new module
                        using (var ins = new SqlCommand(@"
INSERT INTO dbo.Modules (ProductId, ModuleCode, Name, Description, IsActive)
VALUES (@ProductId, @ModuleCode, @Name, @Description, @IsActive);", conn, tx))
                        {
                            ins.Parameters.AddWithValue("@ProductId", product.Id);
                            ins.Parameters.AddWithValue("@ModuleCode", moduleCode);
                            ins.Parameters.AddWithValue("@Name", (object)m.Name ?? DBNull.Value);
                            ins.Parameters.AddWithValue("@Description", (object)m.Description ?? DBNull.Value);
                            ins.Parameters.AddWithValue("@IsActive", m.IsActive);
                            ins.ExecuteNonQuery();
                        }
                    }
                }

                // 4. Delete removed modules (Clean up)
                foreach (var kvp in dbModules)
                {
                    if (!incomingCodes.Contains(kvp.Key))
                    {
                        try
                        {
                            // Try hard delete
                            using (var del = new SqlCommand("DELETE FROM dbo.Modules WHERE Id = @Id;", conn, tx))
                            {
                                del.Parameters.AddWithValue("@Id", kvp.Value);
                                del.ExecuteNonQuery();
                            }
                        }
                        catch (SqlException)
                        {
                            // FK Constraint (Module in use) -> Fallback to Soft Delete (Deactivate)
                            using (var soft = new SqlCommand("UPDATE dbo.Modules SET IsActive = 0 WHERE Id = @Id;", conn, tx))
                            {
                                soft.Parameters.AddWithValue("@Id", kvp.Value);
                                soft.ExecuteNonQuery();
                            }
                        }
                    }
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void DeleteProduct(int id)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand("UPDATE dbo.Products SET IsDeleted = 1 WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        private static Product ReadProduct(IDataRecord r) => new Product
        {
            Id = r.GetInt32(0),
            ProductID = r.GetString(1),
            Name = r.IsDBNull(2) ? null : r.GetString(2),
            Description = r.IsDBNull(3) ? null : r.GetString(3),
            ReleaseNotes = r.IsDBNull(4) ? null : r.GetString(4),
            CreatedBy = r.IsDBNull(5) ? null : r.GetString(5),
            CreatedUtc = r.IsDBNull(6) ? default : r.GetDateTime(6),
            LastModifiedUtc = r.IsDBNull(7) ? default : r.GetDateTime(7),
            IsDeleted = !r.IsDBNull(8) && r.GetBoolean(8),
            Modules = new List<Module>() // not populated here; UI can fetch separately if needed
        };

        // ---------- Dealers ----------
        public IEnumerable<Dealer> GetDealers()
        {
            var list = new List<Dealer>();
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT Id, DealerCode, Name
FROM dbo.Dealers
ORDER BY DealerCode;", conn);
            conn.Open();
            using var r = cmd.ExecuteReader();
            while (r.Read()) list.Add(ReadDealer(r));
            return list;
        }

        public Dealer GetDealerById(int id)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) Id, DealerCode, Name
FROM dbo.Dealers WHERE Id = @id;", conn);
            cmd.Parameters.AddWithValue("@id", id);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return ReadDealer(r);
        }

        public Dealer GetDealerByCode(string dealerCode)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) Id, DealerCode, Name
FROM dbo.Dealers WHERE DealerCode = @code;", conn);
            cmd.Parameters.AddWithValue("@code", dealerCode ?? (object)DBNull.Value);
            conn.Open();
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return ReadDealer(r);
        }

        public int InsertDealer(Dealer dealer)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.Dealers (DealerCode, Name)
VALUES (@DealerCode, @Name);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);
            cmd.Parameters.AddWithValue("@DealerCode", dealer.DealerCode);
            cmd.Parameters.AddWithValue("@Name", dealer.Name);
            conn.Open();
            return (int)cmd.ExecuteScalar();
        }

        public void UpdateDealer(Dealer dealer)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
UPDATE dbo.Dealers
SET DealerCode = @DealerCode,
    Name = @Name
WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", dealer.Id);
            cmd.Parameters.AddWithValue("@DealerCode", dealer.DealerCode);
            cmd.Parameters.AddWithValue("@Name", dealer.Name);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void DeleteDealer(int id)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand("DELETE FROM dbo.Dealers WHERE Id = @Id;", conn);
            cmd.Parameters.AddWithValue("@Id", id);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        private static Dealer ReadDealer(IDataRecord r) => new Dealer
        {
            Id = r.GetInt32(0),
            DealerCode = r.GetString(1),
            Name = r.IsDBNull(2) ? null : r.GetString(2)
        };

        // ---------- License Requests ----------
        public int InsertLicenseRequest(LicenseRequestRecord record)
        {
            using var conn = _factory.Create();
            conn.Open();
            using var cmd = new SqlCommand(@"
INSERT INTO dbo.LicenseRequests
(CompanyName, ProductID, DealerCode, LicenseType, RequestedPeriodMonths, LicenseKey, CurrencyCode, RequestDateUtc, RequestFileBase64, CreatedByUserId)
VALUES (@CompanyName, @ProductID, @DealerCode, @LicenseType, @RequestedPeriodMonths, @LicenseKey, @CurrencyCode, @RequestDateUtc, @RequestFileBase64, @CreatedByUserId);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn);

            cmd.Parameters.AddWithValue("@CompanyName", record.CompanyName);
            cmd.Parameters.AddWithValue("@ProductID", record.ProductID);
            cmd.Parameters.AddWithValue("@DealerCode", record.DealerCode);
            cmd.Parameters.AddWithValue("@LicenseType", record.LicenseType.ToString());
            cmd.Parameters.AddWithValue("@RequestedPeriodMonths", record.RequestedPeriodMonths);
            cmd.Parameters.AddWithValue("@LicenseKey", record.LicenseKey);
            cmd.Parameters.AddWithValue("@CurrencyCode", (object)record.CurrencyCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@RequestDateUtc", record.RequestDateUtc);
            cmd.Parameters.AddWithValue("@RequestFileBase64", (object)record.RequestFileBase64 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedByUserId", record.CreatedByUserId);

            return (int)cmd.ExecuteScalar();
        }

        public LicenseRequestRecord GetLicenseRequestById(int id)
        {
            using var conn = _factory.Create();
            conn.Open();
            LicenseRequestRecord rec = null;

            using (var cmd = new SqlCommand(@"
SELECT Id, CompanyName, ProductID, DealerCode, LicenseType, RequestedPeriodMonths, LicenseKey, CurrencyCode, RequestDateUtc, RequestFileBase64, CreatedByUserId
FROM dbo.LicenseRequests WHERE Id = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                {
                    rec = new LicenseRequestRecord
                    {
                        Id = r.GetInt32(0),
                        CompanyName = r.GetString(1),
                        ProductID = r.GetString(2),
                        DealerCode = r.GetString(3),
                        LicenseType = ParseEnum<LicenseType>(r.GetString(4)),
                        RequestedPeriodMonths = r.GetInt32(5),
                        LicenseKey = r.GetString(6),
                        CurrencyCode = r.IsDBNull(7) ? null : r.GetString(7),
                        RequestDateUtc = r.GetDateTime(8),
                        RequestFileBase64 = r.IsDBNull(9) ? null : r.GetString(9),
                        CreatedByUserId = r.GetInt32(10),
                        ModuleCodes = new System.Collections.Generic.List<string>()
                    };
                }
            }

            if (rec == null) return null;
            rec.ModuleCodes.AddRange(GetRequestModuleCodes(conn, rec.Id));
            return rec;
        }

        public IEnumerable<LicenseRequestRecord> GetLicenseRequests(string productId = null)
        {
            var list = new List<LicenseRequestRecord>();
            using var conn = _factory.Create();
            conn.Open();

            using (var cmd = new SqlCommand(@"
SELECT Id, CompanyName, ProductID, DealerCode, LicenseType, RequestedPeriodMonths, LicenseKey, CurrencyCode, RequestDateUtc, RequestFileBase64, CreatedByUserId
FROM dbo.LicenseRequests
WHERE (@pid IS NULL OR ProductID = @pid)
ORDER BY RequestDateUtc DESC, Id DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@pid", (object)productId ?? DBNull.Value);
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new LicenseRequestRecord
                    {
                        Id = r.GetInt32(0),
                        CompanyName = r.GetString(1),
                        ProductID = r.GetString(2),
                        DealerCode = r.GetString(3),
                        LicenseType = ParseEnum<LicenseType>(r.GetString(4)),
                        RequestedPeriodMonths = r.GetInt32(5),
                        LicenseKey = r.GetString(6),
                        CurrencyCode = r.IsDBNull(7) ? null : r.GetString(7),
                        RequestDateUtc = r.GetDateTime(8),
                        RequestFileBase64 = r.IsDBNull(9) ? null : r.GetString(9),
                        CreatedByUserId = r.GetInt32(10),
                        ModuleCodes = new List<string>()
                    });
                }
            }

            foreach (var rec in list)
                rec.ModuleCodes.AddRange(GetRequestModuleCodes(conn, rec.Id));

            return list;
        }

        // ---------- Licenses ----------
        public int InsertLicense(LicenseMetadata meta)
        {
            using var conn = _factory.Create();
            conn.Open();
            using var tx = conn.BeginTransaction();

            try
            {
                int id;
                using (var cmd = new SqlCommand(@"
INSERT INTO dbo.Licenses
(CompanyName, ProductID, DealerCode, LicenseKey, LicenseType, ValidFromUtc, ValidToUtc, CurrencyCode, Status, ImportedOnUtc, ImportedByUserId, RawAslBase64, Remarks)
VALUES
(@CompanyName, @ProductID, @DealerCode, @LicenseKey, @LicenseType, @ValidFromUtc, @ValidToUtc, @CurrencyCode, @Status, @ImportedOnUtc, @ImportedByUserId, @RawAslBase64, @Remarks);
SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@CompanyName", meta.CompanyName);
                    cmd.Parameters.AddWithValue("@ProductID", meta.ProductID);
                    cmd.Parameters.AddWithValue("@DealerCode", meta.DealerCode);
                    cmd.Parameters.AddWithValue("@LicenseKey", meta.LicenseKey);
                    cmd.Parameters.AddWithValue("@LicenseType", meta.LicenseType.ToString());
                    cmd.Parameters.AddWithValue("@ValidFromUtc", meta.ValidFromUtc);
                    cmd.Parameters.AddWithValue("@ValidToUtc", meta.ValidToUtc);
                    cmd.Parameters.AddWithValue("@CurrencyCode", (object)meta.CurrencyCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", meta.Status.ToString());
                    cmd.Parameters.AddWithValue("@ImportedOnUtc", meta.ImportedOnUtc);
                    cmd.Parameters.AddWithValue("@ImportedByUserId", (object)meta.ImportedByUserId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@RawAslBase64", (object)meta.RawAslBase64 ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Remarks", (object)meta.Remarks ?? DBNull.Value);
                    id = (int)cmd.ExecuteScalar();
                }

                SetLicenseModules(conn, tx, id, meta.ModuleCodes, meta.ProductID);
                tx.Commit();
                return id;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public LicenseMetadata GetLicenseById(int id)
        {
            using var conn = _factory.Create();
            conn.Open();

            LicenseMetadata meta = null;
            using (var cmd = new SqlCommand(@"
SELECT Id, CompanyName, ProductID, DealerCode, LicenseKey, LicenseType, ValidFromUtc, ValidToUtc, CurrencyCode, Status, ImportedOnUtc, ImportedByUserId, RawAslBase64, Remarks
FROM dbo.Licenses WHERE Id = @id;", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                    meta = ReadLicense(r);
            }

            if (meta == null) return null;
            meta.ModuleCodes.AddRange(GetLicenseModuleCodes(conn, meta.Id));
            return meta;
        }

        public LicenseMetadata GetActiveLicense(string productId, string companyName)
        {
            using var conn = _factory.Create();
            conn.Open();

            LicenseMetadata meta = null;
            using (var cmd = new SqlCommand(@"
SELECT TOP(1) Id, CompanyName, ProductID, DealerCode, LicenseKey, LicenseType, ValidFromUtc, ValidToUtc, CurrencyCode, Status, ImportedOnUtc, ImportedByUserId, RawAslBase64, Remarks
FROM dbo.Licenses
WHERE ProductID = @pid AND CompanyName = @c
ORDER BY ImportedOnUtc DESC, Id DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@pid", productId);
                cmd.Parameters.AddWithValue("@c", companyName);
                using var r = cmd.ExecuteReader();
                if (r.Read())
                    meta = ReadLicense(r);
            }

            if (meta == null) return null;
            meta.ModuleCodes.AddRange(GetLicenseModuleCodes(conn, meta.Id));
            return meta;
        }

        public IEnumerable<LicenseMetadata> GetLicenses(string productId = null)
        {
            var list = new List<LicenseMetadata>();
            using var conn = _factory.Create();
            conn.Open();
            using (var cmd = new SqlCommand(@"
SELECT Id, CompanyName, ProductID, DealerCode, LicenseKey, LicenseType, ValidFromUtc, ValidToUtc, CurrencyCode, Status, ImportedOnUtc, ImportedByUserId, RawAslBase64, Remarks
FROM dbo.Licenses
WHERE (@pid IS NULL OR PRODUCTID = @pid)
ORDER BY ImportedOnUtc DESC, Id DESC;", conn))
            {
                cmd.Parameters.AddWithValue("@pid", (object)productId ?? DBNull.Value);
                using var r = cmd.ExecuteReader();
                while (r.Read()) list.Add(ReadLicense(r));
            }

            foreach (var l in list)
                l.ModuleCodes.AddRange(GetLicenseModuleCodes(conn, l.Id));

            return list;
        }

        public void SetRequestModules(int requestId, IEnumerable<string> moduleCodes)
        {
            using var conn = _factory.Create();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                string productIdStr;
                using (var cmd = new SqlCommand("SELECT ProductID FROM dbo.LicenseRequests WHERE Id = @id;", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", requestId);
                    productIdStr = cmd.ExecuteScalar() as string;
                }
                if (string.IsNullOrEmpty(productIdStr))
                    throw new InvalidOperationException("License request not found for modules assignment.");

                var productDbId = ResolveProductDbId(conn, tx, productIdStr);

                using (var del = new SqlCommand("DELETE FROM dbo.LicenseRequestModules WHERE LicenseRequestId = @id;", conn, tx))
                {
                    del.Parameters.AddWithValue("@id", requestId);
                    del.ExecuteNonQuery();
                }

                foreach (var code in moduleCodes ?? Array.Empty<string>())
                {
                    var moduleId = ResolveModuleId(conn, tx, productDbId, code);
                    using var ins = new SqlCommand(@"
INSERT INTO dbo.LicenseRequestModules (LicenseRequestId, ModuleId)
VALUES (@rid, @mid);", conn, tx);
                    ins.Parameters.AddWithValue("@rid", requestId);
                    ins.Parameters.AddWithValue("@mid", moduleId);
                    ins.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void SetLicenseModules(int licenseId, IEnumerable<string> moduleCodes)
        {
            using var conn = _factory.Create();
            conn.Open();
            using var tx = conn.BeginTransaction();
            try
            {
                string productIdStr;
                using (var cmd = new SqlCommand("SELECT ProductID FROM dbo.Licenses WHERE Id = @id;", conn, tx))
                {
                    cmd.Parameters.AddWithValue("@id", licenseId);
                    productIdStr = cmd.ExecuteScalar() as string;
                }
                if (string.IsNullOrEmpty(productIdStr))
                    throw new InvalidOperationException("License not found for modules assignment.");

                var productDbId = ResolveProductDbId(conn, tx, productIdStr);

                using (var del = new SqlCommand("DELETE FROM dbo.LicenseModules WHERE LicenseId = @id;", conn, tx))
                {
                    del.Parameters.AddWithValue("@id", licenseId);
                    del.ExecuteNonQuery();
                }

                foreach (var code in moduleCodes ?? Array.Empty<string>())
                {
                    var moduleId = ResolveModuleId(conn, tx, productDbId, code);
                    using var ins = new SqlCommand(@"
INSERT INTO dbo.LicenseModules (LicenseId, ModuleId)
VALUES (@lid, @mid);", conn, tx);
                    ins.Parameters.AddWithValue("@lid", licenseId);
                    ins.Parameters.AddWithValue("@mid", moduleId);
                    ins.ExecuteNonQuery();
                }

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        // New methods required by ILicenseDatabaseService
        public IEnumerable<ModuleDto> GetModulesForProduct(string productId)
        {
            var list = new List<ModuleDto>();
            try
            {
                using var conn = _factory.Create();
                using var cmd = new SqlCommand(@"
SELECT m.ModuleCode, m.Name, m.Description
FROM dbo.Modules m
JOIN dbo.Products p ON p.Id = m.ProductId
WHERE p.ProductID = @pid
ORDER BY m.ModuleCode;", conn);
                cmd.Parameters.AddWithValue("@pid", (object)productId ?? DBNull.Value);
                conn.Open();
                using var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    list.Add(new ModuleDto
                    {
                        ModuleCode = r.GetString(0),
                        ModuleName = r.IsDBNull(1) ? null : r.GetString(1),
                        Description = r.IsDBNull(2) ? null : r.GetString(2)
                    });
                }
            }
            catch
            {
                // Fail safely: do not leak DB errors to UI. TODO: log exception.
                return new List<ModuleDto>();
            }
            return list;
        }

        public bool ExistsDuplicateLicense(string companyName, string productId, DateTime validFromUtc, DateTime validToUtc)
        {
            try
            {
                using var conn = _factory.Create();
                using var cmd = new SqlCommand(@"
SELECT TOP(1) 1
FROM dbo.Licenses
WHERE ProductID = @pid
  AND CompanyName = @cn
  AND ValidFromUtc = @vf
  AND ValidToUtc = @vt
  AND Status != 'Deleted';", conn);
                cmd.Parameters.AddWithValue("@pid", productId ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@cn", companyName ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@vf", validFromUtc);
                cmd.Parameters.AddWithValue("@vt", validToUtc);
                conn.Open();
                var val = cmd.ExecuteScalar();
                return val != null;
            }
            catch
            {
                // Fail safely: do not leak DB exceptions to UI. TODO: log exception.
                return false;
            }
        }

        public bool LicenseKeyExists(string licenseKey)
        {
            try
            {
                using var conn = _factory.Create();
                using var cmd = new SqlCommand("SELECT TOP(1) 1 FROM dbo.Licenses WHERE LicenseKey = @k;", conn);
                cmd.Parameters.AddWithValue("@k", (object)licenseKey ?? DBNull.Value);
                conn.Open();
                var val = cmd.ExecuteScalar();
                return val != null;
            }
            catch
            {
                // Fail safely: on DB error assume key does not exist to avoid blocking generation; TODO: log exception.
                return false;
            }
        }

        public void UpdateLicenseStatus(int licenseId, string status)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand("UPDATE dbo.Licenses SET Status=@s WHERE Id=@id;", conn);
            cmd.Parameters.AddWithValue("@s", status);
            cmd.Parameters.AddWithValue("@id", licenseId);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public bool TryGetLatestLicenseSummary(string productId, string companyName,
            out string licenseType, out DateTime validFromUtc, out DateTime validToUtc, out string status)
        {
            licenseType = string.Empty;
            validFromUtc = DateTime.MinValue;
            validToUtc = DateTime.MinValue;
            status = string.Empty;

            using var conn = _factory.Create();
            conn.Open();
            using var cmd = new SqlCommand(@"
SELECT TOP(1) LicenseType, ValidFromUtc, ValidToUtc, Status
FROM dbo.Licenses
WHERE ProductID=@pid AND CompanyName=@cn
ORDER BY ValidToUtc DESC, Id DESC;", conn);
            cmd.Parameters.AddWithValue("@pid", productId);
            cmd.Parameters.AddWithValue("@cn", companyName);

            using var rdr = cmd.ExecuteReader(CommandBehavior.SingleRow);
            if (!rdr.Read()) return false;

            licenseType = rdr.GetString(0);
            validFromUtc = rdr.GetDateTime(1);
            validToUtc = rdr.GetDateTime(2);
            status = rdr.GetString(3);
            return true;
        }

        private static LicenseMetadata ReadLicense(IDataRecord r) => new LicenseMetadata
        {
            Id = r.GetInt32(0),
            CompanyName = r.GetString(1),
            ProductID = r.GetString(2),
            DealerCode = r.GetString(3),
            LicenseKey = r.GetString(4),
            LicenseType = ParseEnum<LicenseType>(r.GetString(5)),
            ValidFromUtc = r.GetDateTime(6),
            ValidToUtc = r.GetDateTime(7),
            CurrencyCode = r.IsDBNull(8) ? null : r.GetString(8),
            Status = ParseEnum<LicenseStatus>(r.GetString(9)),
            ImportedOnUtc = r.GetDateTime(10),
            ImportedByUserId = r.IsDBNull(11) ? (int?)null : r.GetInt32(11),
            RawAslBase64 = r.IsDBNull(12) ? null : r.GetString(12),
            Remarks = r.IsDBNull(13) ? null : r.GetString(13),
            ModuleCodes = new List<string>()
        };

        private static IEnumerable<string> GetRequestModuleCodes(SqlConnection conn, int requestId)
        {
            using var cmd = new SqlCommand(@"
SELECT m.ModuleCode
FROM dbo.LicenseRequestModules lrm
JOIN dbo.Modules m ON m.Id = lrm.ModuleId
WHERE lrm.LicenseRequestId = @id
ORDER BY m.ModuleCode;", conn);
            cmd.Parameters.AddWithValue("@id", requestId);
            using var r = cmd.ExecuteReader();
            while (r.Read()) yield return r.GetString(0);
        }

        private static IEnumerable<string> GetLicenseModuleCodes(SqlConnection conn, int licenseId)
        {
            using var cmd = new SqlCommand(@"
SELECT m.ModuleCode
FROM dbo.LicenseModules lm
JOIN dbo.Modules m ON m.Id = lm.ModuleId
WHERE lm.LicenseId = @id
ORDER BY m.ModuleCode;", conn);
            cmd.Parameters.AddWithValue("@id", licenseId);
            using var r = cmd.ExecuteReader();
            while (r.Read()) yield return r.GetString(0);
        }

        private static int ResolveProductDbId(SqlConnection conn, SqlTransaction tx, string productIdStr)
        {
            using var cmd = new SqlCommand("SELECT TOP(1) Id FROM dbo.Products WHERE ProductID = @pid;", conn, tx);
            cmd.Parameters.AddWithValue("@pid", productIdStr);
            var obj = cmd.ExecuteScalar();
            if (obj == null)
                throw new InvalidOperationException($"Product '{productIdStr}' not found.");
            return Convert.ToInt32(obj);
        }

        private static int ResolveModuleId(SqlConnection conn, SqlTransaction tx, int productDbId, string moduleCode)
        {
            using var cmd = new SqlCommand("SELECT TOP(1) Id FROM dbo.Modules WHERE ProductId = @p AND ModuleCode = @c;", conn, tx);
            cmd.Parameters.AddWithValue("@p", productDbId);
            cmd.Parameters.AddWithValue("@c", moduleCode);
            var obj = cmd.ExecuteScalar();
            if (obj == null)
                throw new InvalidOperationException($"Module '{moduleCode}' not found for product id {productDbId}.");
            return Convert.ToInt32(obj);
        }

        private void SetLicenseModules(SqlConnection conn, SqlTransaction tx, int licenseId, IEnumerable<string> moduleCodes, string productIdStr)
        {
            var productDbId = ResolveProductDbId(conn, tx, productIdStr);

            using (var del = new SqlCommand("DELETE FROM dbo.LicenseModules WHERE LicenseId = @id;", conn, tx))
            {
                del.Parameters.AddWithValue("@id", licenseId);
                del.ExecuteNonQuery();
            }

            foreach (var code in moduleCodes ?? Array.Empty<string>())
            {
                var moduleId = ResolveModuleId(conn, tx, productDbId, code);
                using var ins = new SqlCommand(@"
INSERT INTO dbo.LicenseModules (LicenseId, ModuleId)
VALUES (@lid, @mid);", conn, tx);
                ins.Parameters.AddWithValue("@lid", licenseId);
                ins.Parameters.AddWithValue("@mid", moduleId);
                ins.ExecuteNonQuery();
            }
        }

        // ---------- App Settings ----------
        public string GetSetting(string key, string defaultValue)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand("SELECT TOP(1) [Value] FROM dbo.AppSettings WHERE [Key] = @k;", conn);
            cmd.Parameters.AddWithValue("@k", key ?? (object)DBNull.Value);
            conn.Open();
            var obj = cmd.ExecuteScalar();
            if (obj == null || obj == DBNull.Value) return defaultValue;
            return Convert.ToString(obj);
        }

        public void SaveSetting(string key, string value)
        {
            using var conn = _factory.Create();
            using var cmd = new SqlCommand(@"
MERGE dbo.AppSettings AS target
USING (SELECT @Key AS [Key]) AS source
ON (target.[Key] = source.[Key])
WHEN MATCHED THEN UPDATE SET [Value] = @Value
WHEN NOT MATCHED THEN INSERT ([Key], [Value]) VALUES (@Key, @Value);", conn);
            cmd.Parameters.AddWithValue("@Key", (object)key ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Value", (object)value ?? DBNull.Value);
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Helper to parse enums using the non-generic Enum.Parse and cast to the requested enum type.
        private static T ParseEnum<T>(string value, bool ignoreCase = true) where T : struct, Enum
        {
            if (value == null) throw new InvalidOperationException($"Cannot parse null into enum {typeof(T).Name}.");
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }
    }
}