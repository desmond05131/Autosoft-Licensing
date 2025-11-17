using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Models.Enums;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing.Tools
{
    /// <summary>
    /// Small non-UI smoke test harness to exercise core flows.
    /// Run by launching the application with the "--smoke" argument.
    /// </summary>
    internal static class SmokeTestHarness
    {
        public struct Result
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public static Result RunAll()
        {
            var sb = new StringBuilder();
            TryAppend(sb, "Starting smoke test...");

            User admin = null;

            // 1) Read admin user via Database
            try
            {
                admin = ServiceRegistry.Database.GetUserByUsername("admin");
                if (admin == null)
                {
                    return Failure("Admin user not found (ServiceRegistry.Database.GetUserByUsername returned null). Ensure seed data exists.");
                }
                TryAppend(sb, $"Admin user found: Id={admin.Id}, Username='{admin.Username}', DisplayName='{admin.DisplayName}'");
            }
            catch (Exception ex)
            {
                return Failure("Database query for admin user failed: " + ex.Message);
            }

            // 2) Validate admin credentials using User.ValidateCredentials("admin", "admin")
            try
            {
                var ok = ServiceRegistry.User.ValidateCredentials("admin", "admin");
                TryAppend(sb, $"ValidateCredentials('admin','admin') returned: {ok}");
                if (!ok)
                    TryAppend(sb, "Note: seeded admin password is SHA256(\"admin\") per Seed.sql. If password differs, update seed or DB.");
            }
            catch (Exception ex)
            {
                return Failure("User credential validation failed: " + ex.Message);
            }

            // 3) Create LicenseRequest and call SerializeToArl(...) to confirm validation
            try
            {
                var req = new LicenseRequest
                {
                    CompanyName = "SmokeTest Co",
                    ProductID = "SAMPLE-PRODUCT",
                    DealerCode = "DEALER-001",
                    RequestedPeriodMonths = 1,
                    LicenseType = LicenseType.Demo,
                    LicenseKey = "SMOKETEST-KEY-001",
                    CurrencyCode = "USD",
                    RequestDateUtc = DateTime.UtcNow,
                    ModuleCodes = new List<string> { "MODULE-001" }
                };

                string arl = ServiceRegistry.LicenseRequest.SerializeToArl(req);
                TryAppend(sb, "LicenseRequest.SerializeToArl succeeded; length=" + (arl?.Length ?? 0));
            }
            catch (ValidationException vex)
            {
                return Failure("LicenseRequest validation failed: " + vex.Message);
            }
            catch (Exception ex)
            {
                return Failure("LicenseRequest.SerializeToArl failed: " + ex.Message);
            }

            // 4) Create LicenseData and round-trip with GenerateAsl, ImportAslBase64 and Activate (persist to DB)
            try
            {
                var now = DateTime.UtcNow;
                var data = new LicenseData
                {
                    CompanyName = "SmokeTest Co",
                    ProductID = "SAMPLE-PRODUCT",
                    DealerCode = "DEALER-001",
                    LicenseType = LicenseType.Demo,
                    ValidFromUtc = now.Date,
                    ValidToUtc = now.Date.AddMonths(1),
                    LicenseKey = "SMOKETEST-KEY-001",
                    CurrencyCode = "USD",
                    ModuleCodes = new List<string> { "MODULE-001" }
                };

                // Build ASL (encrypted base64)
                string base64Asl;
                try
                {
                    base64Asl = ServiceRegistry.License.GenerateAsl(data, CryptoConstants.AesKey, CryptoConstants.AesIV);
                    TryAppend(sb, "GenerateAsl succeeded; length=" + (base64Asl?.Length ?? 0));
                }
                catch (ValidationException vx)
                {
                    return Failure("LicenseData validation failed during GenerateAsl: " + vx.Message);
                }

                // Import ASL (decrypt & validate)
                LicenseData imported;
                try
                {
                    imported = ServiceRegistry.License.ImportAslBase64(base64Asl, CryptoConstants.AesKey, CryptoConstants.AesIV);
                    if (imported == null)
                        return Failure("ImportAslBase64 returned null.");
                    TryAppend(sb, $"ImportAslBase64 succeeded; LicenseKey='{imported.LicenseKey}', ProductID='{imported.ProductID}'");
                }
                catch (ValidationException vx)
                {
                    return Failure("ImportAslBase64 validation failed: " + vx.Message);
                }

                // Activate -> persist license and modules to DB using admin user id
                LicenseMetadata persistedMeta;
                try
                {
                    persistedMeta = ServiceRegistry.License.Activate(imported, admin?.Id);
                    if (persistedMeta == null)
                        return Failure("Activate returned null (unexpected).");
                    TryAppend(sb, $"Activate succeeded; new License Id = {persistedMeta.Id}");
                }
                catch (Exception ex)
                {
                    return Failure("Activate failed: " + ex.Message);
                }

                // Verify DB record and modules were saved
                try
                {
                    var dbMeta = ServiceRegistry.Database.GetLicenseById(persistedMeta.Id);
                    if (dbMeta == null)
                        return Failure($"License record not found after activate (Id={persistedMeta.Id}).");

                    TryAppend(sb, $"DB license readback OK: Id={dbMeta.Id}, LicenseKey={dbMeta.LicenseKey}, ProductID={dbMeta.ProductID}, CompanyName={dbMeta.CompanyName}");
                    TryAppend(sb, $"Module count stored: {dbMeta.ModuleCodes?.Count ?? 0}");
                    if (dbMeta.ModuleCodes == null || dbMeta.ModuleCodes.Count == 0)
                        return Failure("No modules were stored for the activated license (expected at least one).");
                }
                catch (Exception ex)
                {
                    return Failure("Verification of persisted license failed: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                return Failure("License ASL round-trip + activate failed: " + ex.Message);
            }

            TryAppend(sb, "Smoke test completed successfully.");
            return new Result { Success = true, Message = sb.ToString() };
        }

        private static Result Failure(string msg)
            => new Result { Success = false, Message = "Smoke test failed: " + msg };

        private static void TryAppend(StringBuilder sb, string s)
        {
            try { sb.AppendLine(s); } catch { /* ignore */ }
        }
    }
}
