using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Linq;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Utils;
using Newtonsoft.Json;

namespace Autosoft_Licensing.UI
{
    public partial class PreviewLicenseForm : DevExpress.XtraEditors.XtraForm
    {
        private readonly LicenseData _payload;
        private readonly string _expectedChecksum;
        private string _canonicalJson;
        private string _computedChecksum;

        // Parameterless ctor used by the WinForms designer. Keep it simple and design-time safe.
        public PreviewLicenseForm()
        {
            InitializeComponent();
            // Do not run runtime initialization at design-time.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;
        }

        public PreviewLicenseForm(LicenseData payload, string canonicalJson = null, string expectedChecksum = null)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            InitializeComponent();

            _payload = payload;
            _expectedChecksum = expectedChecksum;
            _canonicalJson = canonicalJson;

            try
            {
                InitializeContent();
            }
            catch (Exception)
            {
                // Per spec: surface a generic failure message on unexpected errors
                MessageBox.Show("Operation failed. Contact admin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // keep form usable but empty
            }
        }

        private void InitializeContent()
        {
            // 1) Canonical JSON: try ServiceRegistry.Validation.BuildCanonicalJson(...) first
            if (string.IsNullOrEmpty(_canonicalJson))
            {
                try
                {
                    // Preferred call - may not exist in current implementation
                    // TODO: If ServiceRegistry.Validation exposes BuildCanonicalJson, prefer it.
                    var validation = ServiceRegistry.Validation;
                    var mi = (validation == null) ? null : validation.GetType().GetMethod("BuildCanonicalJson");
                    if (mi != null)
                    {
                        _canonicalJson = (string)mi.Invoke(validation, new object[] { _payload });
                    }
                    else
                    {
                        // Fallback: local canonical serializer using JsonHelper (deterministic ordering)
                        _canonicalJson = CanonicalJsonSerializer.Serialize(_payload);
                        // TODO: replace fallback with shared serializer when available.
                    }
                }
                catch
                {
                    // fallback to local serializer on any error
                    _canonicalJson = CanonicalJsonSerializer.Serialize(_payload);
                }
            }

            // 2) Compute checksum (try ServiceRegistry.Encryption first)
            try
            {
                // ServiceRegistry is a static type. Null-conditional on a type is invalid (CS0119).
                // Check the static property directly.
                if (ServiceRegistry.Encryption != null)
                {
                    _computedChecksum = ServiceRegistry.Encryption.ComputeSha256Hex(System.Text.Encoding.UTF8.GetBytes(_canonicalJson));
                }
                else
                {
                    _computedChecksum = ComputeSha256HexLocal(_canonicalJson);
                }
            }
            catch
            {
                _computedChecksum = ComputeSha256HexLocal(_canonicalJson);
            }

            // 3) Populate UI controls
            memoCanonicalJson.Text = _canonicalJson ?? string.Empty;
            txtChecksum.Text = _computedChecksum ?? string.Empty;

            txtCompanyName.Text = _payload.CompanyName ?? string.Empty;
            txtProductId.Text = _payload.ProductID ?? string.Empty;
            txtProductName.Text = GetProductName(_payload.ProductID) ?? string.Empty;
            txtLicenseKey.Text = _payload.LicenseKey ?? string.Empty;
            txtLicenseType.Text = _payload.LicenseType.ToString();
            txtValidFrom.Text = _payload.ValidFromUtc.ToString("u");
            txtValidTo.Text = _payload.ValidToUtc.ToString("u");

            // 4) Modules grid: populate ModuleName/Enabled rows
            var modules = new List<ModuleRow>();
            if (_payload.ModuleCodes != null)
            {
                foreach (var m in _payload.ModuleCodes)
                    modules.Add(new ModuleRow { ModuleName = m, Enabled = true });
            }
            grdModules.DataSource = modules;

            // 5) Checksum status
            UpdateChecksumStatus(_computedChecksum, _expectedChecksum);

            // 6) Expiry / status badge
            UpdateStatusBadge();

            // 7) License info grid placeholder: populate some key/value pairs
            PopulateLicenseInfoGrid();
        }

        private void UpdateChecksumStatus(string computed, string expected)
        {
            if (!string.IsNullOrEmpty(expected) && !string.Equals(computed, expected, StringComparison.OrdinalIgnoreCase))
            {
                lblChecksumStatus.Text = "Invalid or tampered license file.";
                lblChecksumStatus.Appearance.ForeColor = System.Drawing.Color.Red;
            }
            else
            {
                lblChecksumStatus.Text = "Checksum OK";
                lblChecksumStatus.Appearance.ForeColor = System.Drawing.Color.Green;
            }
        }

        private void UpdateStatusBadge()
        {
            try
            {
                var nowUtc = DateTime.UtcNow;
                if (_payload == null)
                {
                    SetStatusBadge("Invalid or tampered license file.", System.Drawing.Color.Red);
                    return;
                }

                if (nowUtc > _payload.ValidToUtc)
                {
                    if (_payload.LicenseType == Models.Enums.LicenseType.Demo)
                    {
                        SetStatusBadge("Demo license expired.", System.Drawing.Color.Red);
                    }
                    else
                    {
                        SetStatusBadge("License expired.", System.Drawing.Color.Red);
                    }
                }
                else
                {
                    // If checksum mismatch to expected (if expected provided) show tampered
                    if (!string.IsNullOrEmpty(_expectedChecksum) && !string.Equals(_computedChecksum, _expectedChecksum, StringComparison.OrdinalIgnoreCase))
                    {
                        SetStatusBadge("Invalid or tampered license file.", System.Drawing.Color.Red);
                    }
                    else
                    {
                        SetStatusBadge("Valid", System.Drawing.Color.Green);
                    }
                }

                lblExpiryDate.Text = _payload.ValidToUtc.ToString("yyyy-MM-dd HH:mm 'UTC'");
            }
            catch
            {
                SetStatusBadge("Invalid or tampered license file.", System.Drawing.Color.Red);
            }
        }

        private void SetStatusBadge(string text, System.Drawing.Color color)
        {
            lblStatusBadge.Text = text;
            lblStatusBadge.Appearance.ForeColor = color;
        }

        private void PopulateLicenseInfoGrid()
        {
            var dt = new DataTable();
            dt.Columns.Add("Key");
            dt.Columns.Add("Value");

            AddRow(dt, "CompanyName", _payload.CompanyName);
            AddRow(dt, "ProductID", _payload.ProductID);
            AddRow(dt, "DealerCode", _payload.DealerCode);
            AddRow(dt, "LicenseKey", _payload.LicenseKey);
            AddRow(dt, "LicenseType", _payload.LicenseType.ToString());
            AddRow(dt, "ValidFromUtc", _payload.ValidFromUtc.ToString("u"));
            AddRow(dt, "ValidToUtc", _payload.ValidToUtc.ToString("u"));
            AddRow(dt, "CurrencyCode", _payload.CurrencyCode);
            AddRow(dt, "ChecksumSHA256", _payload.ChecksumSHA256);

            grdLicenseInfo.DataSource = dt;
        }

        // Event handlers

        private void btnCopyJson_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(memoCanonicalJson.Text ?? string.Empty);
                // Transient feedback using a MessageBox (simple). Could use toast.
                MessageBox.Show("Copied canonical JSON to clipboard.", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show("Operation failed. Contact admin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportJson_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dlg = new SaveFileDialog())
                {
                    dlg.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";

                    // Designer-friendly filename construction (avoid null-conditional + interpolation)
                    string company = "license";
                    string licenseKey = "payload";
                    if (_payload != null)
                    {
                        if (!string.IsNullOrEmpty(_payload.CompanyName))
                            company = _payload.CompanyName;
                        if (!string.IsNullOrEmpty(_payload.LicenseKey))
                            licenseKey = _payload.LicenseKey;
                    }

                    dlg.FileName = company + "-" + licenseKey + ".json";

                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        System.IO.File.WriteAllText(dlg.FileName, memoCanonicalJson.Text ?? string.Empty, Encoding.UTF8);
                    }
                }
            }
            catch
            {
                MessageBox.Show("Operation failed. Contact admin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnValidateChecksum_Click(object sender, EventArgs e)
        {
            try
            {
                var text = memoCanonicalJson.Text ?? string.Empty;
                string recomputed;
                try
                {
                    // ServiceRegistry is a static type; don't use null-conditional on the type name.
                    if (ServiceRegistry.Encryption != null)
                    {
                        recomputed = ServiceRegistry.Encryption.ComputeSha256Hex(Encoding.UTF8.GetBytes(text));
                    }
                    else
                    {
                        recomputed = ComputeSha256HexLocal(text);
                    }
                }
                catch
                {
                    recomputed = ComputeSha256HexLocal(text);
                }

                txtChecksum.Text = recomputed;
                // If expected checksum present, compare; otherwise compare payload.ChecksumSHA256 if present.
                var expected = _expectedChecksum;
                if (string.IsNullOrEmpty(expected) && _payload != null)
                    expected = _payload.ChecksumSHA256;

                UpdateChecksumStatus(recomputed, expected);
            }
            catch
            {
                MessageBox.Show("Operation failed. Contact admin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBackToGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Retry;
                this.Close();
            }
            catch
            {
                MessageBox.Show("Operation failed. Contact admin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            try
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            catch
            {
                MessageBox.Show("Operation failed. Contact admin.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Utility helpers

        private static string ComputeSha256HexLocal(string text)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text ?? string.Empty);
                var hash = sha.ComputeHash(bytes);
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash) sb.AppendFormat("{0:x2}", b);
                return sb.ToString();
            }
        }

        private static string GetProductName(string productId)
        {
            // Attempt to resolve a product name via ServiceRegistry.Product if available
            try
            {
                var svc = ServiceRegistry.Product;
                if (svc != null)
                {
                    // Try to call GetProductByProductId if exists
                    var mi = svc.GetType().GetMethod("GetProductByProductId");
                    if (mi != null)
                    {
                        var product = mi.Invoke(svc, new object[] { productId });
                        if (product != null)
                        {
                            var prop = product.GetType().GetProperty("Name");
                            if (prop != null) return prop.GetValue(product) as string;
                        }
                    }
                }
            }
            catch
            {
                // ignore and fallback
            }
            return null;
        }

        // Fallback canonical serializer local implementation; should match the generator's canonicalization rules.
        private static class CanonicalJsonSerializer
        {
            public static string Serialize(object obj)
            {
                try
                {
                    // Use JsonConvert to produce JSON and JsonHelper to canonicalize property ordering
                    var raw = JsonConvert.SerializeObject(obj, Formatting.None);
                    // Remove ChecksumSHA256 property if present so canonical form equals payload used to compute checksum
                    var without = JsonHelper.RemoveProperty(raw, "ChecksumSHA256");
                    var canon = JsonHelper.Canonicalize(without);
                    return canon;
                }
                catch
                {
                    // Fallback coarse serialization
                    return JsonConvert.SerializeObject(obj, Formatting.None);
                }
            }
        }

        // Simple DTO for module grid rows
        private class ModuleRow
        {
            public string ModuleName { get; set; }
            public bool Enabled { get; set; }
        }

        // Designer-friendly helper instead of a local function
        private static void AddRow(DataTable dt, string key, string value)
        {
            dt.Rows.Add(key, value ?? string.Empty);
        }
    }
}
