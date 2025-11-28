using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Drawing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Utils;
using System.Windows.Forms; // used for SaveFileDialog & Clipboard
using System.ComponentModel.DataAnnotations;

namespace Autosoft_Licensing.UI
{
    public partial class PreviewLicenseForm : DevExpress.XtraEditors.XtraForm
    {
        // Constructor
        public PreviewLicenseForm()
        {
            InitializeComponent();

            // Wire up code-behind event handlers that the designer expects to exist.
            // Designer also wires these; adding again is safe and defensive.
            this.btnCopyJson.Click += btnCopyJson_Click;
            this.btnExportJson.Click += btnExportJson_Click;
            this.btnCopyChecksum.Click += btnCopyChecksum_Click;
            this.btnVerifyChecksum.Click += btnVerifyChecksum_Click;
            this.btnExportAsl.Click += btnExportAsl_Click;
            this.btnClose.Click += btnClose_Click;
        }

        /// <summary>
        /// Populate the preview UI with the provided payload. Defensive against nulls & missing services.
        /// </summary>
        public void Initialize(LicenseData payload)
        {
            try
            {
                if (payload == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
                    return;
                }

                // Build canonical JSON using service if available, otherwise fallback to local deterministic implementation.
                string canonicalJson = null;
                try
                {
                    canonicalJson = ServiceRegistry.Validation?.BuildCanonicalJson(payload);
                }
                catch
                {
                    // ignore and fallback
                }

                if (string.IsNullOrWhiteSpace(canonicalJson))
                {
                    try
                    {
                        canonicalJson = CanonicalizeJson(payload);
                    }
                    catch
                    {
                        canonicalJson = string.Empty;
                    }
                }

                memCanonicalJson.Text = canonicalJson ?? string.Empty;

                // Compute checksum using service or local helper
                string checksum = string.Empty;
                try
                {
                    if (!string.IsNullOrEmpty(canonicalJson) && ServiceRegistry.Encryption != null)
                    {
                        checksum = ServiceRegistry.Encryption.ComputeSha256Hex(Encoding.UTF8.GetBytes(canonicalJson));
                    }
                    else
                    {
                        checksum = ComputeSha256HexLocal(Encoding.UTF8.GetBytes(canonicalJson ?? string.Empty));
                    }
                }
                catch
                {
                    checksum = ComputeSha256HexLocal(Encoding.UTF8.GetBytes(canonicalJson ?? string.Empty));
                }

                lblChecksum.Text = checksum ?? string.Empty;
                lblVerifyResult.Text = string.Empty;

                // Populate summary labels
                lblCompany.Text = payload.CompanyName ?? string.Empty;
                lblProduct.Text = $"{payload.ProductID ?? string.Empty} { (payload is null ? string.Empty : string.Empty)}"; // ProductName may not be on LicenseData; keep compact
                lblLicenseType.Text = payload.LicenseType.ToString();
                try
                {
                    lblValidFromUtc.Text = payload.ValidFromUtc == default ? string.Empty : payload.ValidFromUtc.ToString("u");
                    lblValidToUtc.Text = payload.ValidToUtc == default ? string.Empty : payload.ValidToUtc.ToString("u");
                    lblValidFromLocal.Text = payload.ValidFromUtc == default ? string.Empty : payload.ValidFromUtc.ToLocalTime().ToString("u");
                    lblValidToLocal.Text = payload.ValidToUtc == default ? string.Empty : payload.ValidToUtc.ToLocalTime().ToString("u");
                }
                catch
                {
                    lblValidFromUtc.Text = string.Empty;
                    lblValidToUtc.Text = string.Empty;
                    lblValidFromLocal.Text = string.Empty;
                    lblValidToLocal.Text = string.Empty;
                }

                // License key
                txtLicenseKeySummary.Text = payload.LicenseKey ?? string.Empty;

                // Bind modules safely
                var modules = new List<object>();
                try
                {
                    var codes = payload.ModuleCodes ?? new List<string>();
                    foreach (var code in codes)
                    {
                        modules.Add(new { ModuleName = code, Enabled = true });
                    }
                }
                catch
                {
                    modules = new List<object>();
                }

                grdModulesSummary.DataSource = modules;
                try
                {
                    viewModulesSummary.BestFitColumns();
                }
                catch { }

                // Enable/disable Export ASL depending on AslGenerator availability
                btnExportAsl.Enabled = ServiceRegistry.AslGenerator != null;
            }
            catch (Exception)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
            }
        }

        #region Event handlers

        private void btnCopyJson_Click(object sender, EventArgs e)
        {
            try
            {
                var text = memCanonicalJson.Text ?? string.Empty;
                if (string.IsNullOrEmpty(text))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
                    return;
                }
                Clipboard.SetText(text);
                // Optionally show info: use PageBase.ShowInfo from pages; here use messagebox minimal.
            }
            catch
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
            }
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            try
            {
                var json = memCanonicalJson.Text ?? string.Empty;
                using (var sfd = new SaveFileDialog
                {
                    Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                    FileName = "license.json",
                    Title = "Export canonical JSON"
                })
                {
                    if (sfd.ShowDialog() != DialogResult.OK) return;
                    // Ensure UTF-8
                    File.WriteAllText(sfd.FileName, json, Encoding.UTF8);
                }
            }
            catch
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
            }
        }

        private void btnCopyChecksum_Click(object sender, EventArgs e)
        {
            try
            {
                var cs = lblChecksum.Text ?? string.Empty;
                if (string.IsNullOrEmpty(cs))
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
                    return;
                }
                Clipboard.SetText(cs);
            }
            catch
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
            }
        }

        private void btnVerifyChecksum_Click(object sender, EventArgs e)
        {
            try
            {
                var canonical = memCanonicalJson.Text ?? string.Empty;
                var recomputed = ComputeSha256HexLocal(Encoding.UTF8.GetBytes(canonical));
                var existing = lblChecksum.Text ?? string.Empty;

                if (string.Equals(recomputed, existing, StringComparison.OrdinalIgnoreCase))
                {
                    lblVerifyResult.Text = "OK";
                    lblVerifyResult.Appearance.ForeColor = Color.Green;
                }
                else
                {
                    lblVerifyResult.Text = "Checksum mismatch.";
                    lblVerifyResult.Appearance.ForeColor = Color.Red;

                    // If this verification is part of import flow, UI may require showing fatal message:
                    // DevExpress.XtraEditors.XtraMessageBox.Show("Invalid or tampered license file.");
                }
            }
            catch
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
            }
        }

        private void btnExportAsl_Click(object sender, EventArgs e)
        {
            try
            {
                if (ServiceRegistry.AslGenerator == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
                    return;
                }

                // Start with displayed values but prefer canonical JSON for authoritative fields (ProductID, DealerCode)
                var data = new LicenseData
                {
                    CompanyName = lblCompany.Text ?? string.Empty,
                    ProductID = lblProduct.Text?.Trim() ?? string.Empty,
                    DealerCode = string.Empty,
                    LicenseKey = txtLicenseKeySummary.Text ?? string.Empty,
                    ModuleCodes = (grdModulesSummary.DataSource as IEnumerable<object>)?.Cast<dynamic>().Select(x => (string)x.ModuleName).ToList() ?? new List<string>(),
                    ValidFromUtc = DateTime.TryParse(lblValidFromUtc.Text, out var vf) ? vf : default,
                    ValidToUtc = DateTime.TryParse(lblValidToUtc.Text, out var vt) ? vt : default
                };

                // Try to read authoritative ProductID / DealerCode from the canonical JSON in the memo.
                try
                {
                    var json = memCanonicalJson.Text ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var token = JToken.Parse(json);
                        var pid = token["ProductID"]?.ToString();
                        var dcode = token["DealerCode"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(pid)) data.ProductID = pid;
                        if (!string.IsNullOrWhiteSpace(dcode)) data.DealerCode = dcode;
                    }
                }
                catch
                {
                    // ignore parse errors and continue using displayed values
                }

                // If DealerCode still empty try to fallback to any visible label (best-effort)
                if (string.IsNullOrWhiteSpace(data.DealerCode))
                {
                    // Attempt to extract DealerCode from Product label if it was displayed there (unlikely), otherwise leave empty
                    // The LicenseData.DealerCode is required, so this will surface a sensible validation message rather than null ref.
                    data.DealerCode = string.Empty;
                }

                // Map license type displayed in UI to enum (best-effort)
                try
                {
                    if (!string.IsNullOrWhiteSpace(lblLicenseType.Text) &&
                        Enum.TryParse<Models.Enums.LicenseType>(lblLicenseType.Text, true, out var parsedLt))
                    {
                        data.LicenseType = parsedLt;
                    }
                    else
                    {
                        data.LicenseType = Models.Enums.LicenseType.Subscription;
                    }
                }
                catch
                {
                    data.LicenseType = Models.Enums.LicenseType.Subscription;
                }

                using (var sfd = new SaveFileDialog
                {
                    Filter = "Autosoft License (*.asl)|*.asl|All files (*.*)|*.*",
                    FileName = $"{data.CompanyName}_{data.ProductID}.asl",
                    Title = "Export ASL"
                })
                {
                    if (sfd.ShowDialog() != DialogResult.OK) return;

                    try
                    {
                        // Use same CryptoConstants used by Generate page
                        var key = CryptoConstants.AesKey;
                        var iv = CryptoConstants.AesIV;

                        ServiceRegistry.AslGenerator.CreateAndSaveAsl(data, sfd.FileName, key, iv);
                        DevExpress.XtraEditors.XtraMessageBox.Show("License generated successfully.");
                    }
                    catch (ValidationException vx)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show(vx.Message);
                    }
                    catch (Exception)
                    {
                        DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
                    }
                }
            }
            catch
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
            }
            catch
            {
                // ignore
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Local deterministic canonicalization fallback.
        /// Builds a JObject from payload, sorts properties lexicographically and returns indented JSON.
        /// </summary>
        private string CanonicalizeJson(LicenseData payload)
        {
            if (payload == null) return string.Empty;
            // Use Json.NET to build JObject and reuse JsonHelper.Canonicalize for stable ordering.
            var json = JsonConvert.SerializeObject(payload, Formatting.None);
            var withoutChecksum = JsonHelper.RemoveProperty(json, "ChecksumSHA256");
            var canon = JsonHelper.Canonicalize(withoutChecksum);
            // Return formatted (indented) for display
            var token = JToken.Parse(canon);
            return token.ToString(Formatting.Indented);
        }

        /// <summary>
        /// Local SHA-256 hex helper (lowercase hex).
        /// </summary>
        private string ComputeSha256HexLocal(byte[] bytes)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(bytes ?? Array.Empty<byte>());
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash) sb.AppendFormat("{0:x2}", b);
            return sb.ToString();
        }

        #endregion
    }
}
