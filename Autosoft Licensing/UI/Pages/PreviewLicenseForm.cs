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
            this.btnCopyChecksum.Click += btnCopyChecksum_Click;
            this.btnVerifyChecksum.Click += btnVerifyChecksum_Click;
            this.btnClose.Click += btnClose_Click;
        }

        /// <summary>
        /// Populate the preview UI with the provided payload.
        /// Uses central canonical pipeline (CanonicalJsonSerializer + Utils.ChecksumHelper).
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

                // Build canonical JSON from payload WITHOUT ChecksumSHA256 property.
                string canonicalJson;
                byte[] canonicalBytes;
                try
                {
                    var j = JObject.FromObject(payload);
                    j.Property("ChecksumSHA256")?.Remove();
                    canonicalJson = CanonicalJsonSerializer.Serialize(j); // compact canonical string
                    canonicalBytes = CanonicalJsonSerializer.SerializeToUtf8Bytes(j);
                }
                catch
                {
                    canonicalJson = string.Empty;
                    canonicalBytes = new byte[0];
                }

                // Display canonical JSON exactly as used for checksum (compact)
                try
                {
                    memCanonicalJson.Text = canonicalJson ?? string.Empty;
                    memCanonicalJson.Font = new Font("Consolas", memCanonicalJson.Font.Size);
                }
                catch
                {
                    memCanonicalJson.Text = canonicalJson ?? string.Empty;
                }

                // Compute checksum using canonical pipeline (bytes)
                string checksum;
                try
                {
                    checksum = Autosoft_Licensing.Utils.ChecksumHelper.ComputeSha256HexLower(canonicalBytes ?? Array.Empty<byte>());
                }
                catch
                {
                    checksum = ComputeSha256HexLocal(canonicalBytes ?? Array.Empty<byte>());
                }

                // Show checksum in label (existing control)
                lblChecksum.Text = checksum ?? string.Empty;
                lblVerifyResult.Text = string.Empty;

                // Populate summary labels (unchanged)
                lblCompany.Text = payload.CompanyName ?? string.Empty;
                lblProduct.Text = $"{payload.ProductID ?? string.Empty} { (payload is null ? string.Empty : string.Empty)}";
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
            }
            catch
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
            }
        }

        // Single-shot save guard used by both export handlers.
        private bool _isSaving = false;

        private async void btnExportJson_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnExportJson_Click invoked");

            if (_isSaving) return;
            _isSaving = true;
            btnExportJson.Enabled = false;

            try
            {
                using (var dlg = new System.Windows.Forms.SaveFileDialog())
                {
                    dlg.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                    dlg.DefaultExt = "json";

                    // Suggest a sensible filename from displayed values; fall back to license.json.
                    string suggested = "license.json";
                    try
                    {
                        var company = (lblCompany?.Text ?? string.Empty).Trim();
                        var product = (lblProduct?.Text ?? string.Empty).Trim();
                        if (!string.IsNullOrWhiteSpace(company) || !string.IsNullOrWhiteSpace(product))
                        {
                            var safeCompany = string.IsNullOrWhiteSpace(company) ? string.Empty : company.Replace(' ', '_');
                            var safeProduct = string.IsNullOrWhiteSpace(product) ? string.Empty : product.Replace(' ', '_');
                            suggested = $"{(string.IsNullOrWhiteSpace(safeCompany) ? "license" : safeCompany)}_{(string.IsNullOrWhiteSpace(safeProduct) ? "license" : safeProduct)}.json";
                        }
                    }
                    catch { /* non-fatal */ }

                    dlg.FileName = suggested;

                    var result = dlg.ShowDialog(this);
                    if (result != System.Windows.Forms.DialogResult.OK) return;

                    var path = dlg.FileName;
                    var text = memCanonicalJson?.Text ?? string.Empty;

                    // Do the write once on a background thread to keep UI responsive.
                    await System.Threading.Tasks.Task.Run(() =>
                    {
                        System.IO.File.WriteAllText(path, text, new System.Text.UTF8Encoding(false));
                    });
                }
            }
            catch (Exception)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
            }
            finally
            {
                _isSaving = false;
                try { btnExportJson.Enabled = true; } catch { /* ignore */ }
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
                var bytes = Encoding.UTF8.GetBytes(canonical);
                var recomputed = Autosoft_Licensing.Utils.ChecksumHelper.ComputeSha256HexLower(bytes);
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
                }
            }
            catch
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
            }
        }

        private async void btnExportAsl_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnExportAsl_Click invoked");

            if (_isSaving) return;
            _isSaving = true;
            btnExportAsl.Enabled = false;

            try
            {
                if (ServiceRegistry.AslGenerator == null)
                {
                    DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
                    return;
                }

                // Build LicenseData from displayed fields (same mapping as original implementation).
                var data = new Autosoft_Licensing.Models.LicenseData
                {
                    CompanyName = lblCompany?.Text ?? string.Empty,
                    ProductID = (lblProduct?.Text ?? string.Empty).Trim(),
                    DealerCode = string.Empty,
                    LicenseKey = txtLicenseKeySummary?.Text ?? string.Empty,
                    ModuleCodes = (grdModulesSummary.DataSource as System.Collections.IEnumerable) != null
                        ? (grdModulesSummary.DataSource as System.Collections.IEnumerable)
                            .Cast<object>()
                            .Select(x => {
                                try { dynamic d = x; return (string)d.ModuleName; } catch { return string.Empty; }
                            })
                            .Where(s => !string.IsNullOrEmpty(s))
                            .ToList()
                        : new System.Collections.Generic.List<string>(),
                    ValidFromUtc = DateTime.TryParse(lblValidFromUtc?.Text, out var vf) ? vf : default,
                    ValidToUtc = DateTime.TryParse(lblValidToUtc?.Text, out var vt) ? vt : default
                };

                // Try to prefer authoritative values from canonical JSON in memo.
                try
                {
                    var json = memCanonicalJson?.Text ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var token = Newtonsoft.Json.Linq.JToken.Parse(json);
                        var pid = token["ProductID"]?.ToString();
                        var dcode = token["DealerCode"]?.ToString();
                        if (!string.IsNullOrWhiteSpace(pid)) data.ProductID = pid;
                        if (!string.IsNullOrWhiteSpace(dcode)) data.DealerCode = dcode;
                    }
                }
                catch { /* ignore parse errors */ }

                // Map license type defensively
                try
                {
                    if (!string.IsNullOrWhiteSpace(lblLicenseType?.Text) &&
                        Enum.TryParse<Autosoft_Licensing.Models.Enums.LicenseType>(lblLicenseType.Text, true, out var parsedLt))
                    {
                        data.LicenseType = parsedLt;
                    }
                    else
                    {
                        data.LicenseType = Autosoft_Licensing.Models.Enums.LicenseType.Subscription;
                    }
                }
                catch
                {
                    data.LicenseType = Autosoft_Licensing.Models.Enums.LicenseType.Subscription;
                }

                using (var sfd = new System.Windows.Forms.SaveFileDialog())
                {
                    sfd.Filter = "Autosoft License (*.asl)|*.asl|All files (*.*)|*.*";
                    var suggested = $"{(data.CompanyName ?? "license").Replace(' ', '_')}_{(data.ProductID ?? "product").Replace(' ', '_')}.asl";
                    sfd.FileName = suggested;
                    sfd.Title = "Export ASL";

                    var dlgRes = sfd.ShowDialog(this);
                    if (dlgRes != System.Windows.Forms.DialogResult.OK) return;

                    var path = sfd.FileName;

                    // Run the create-and-save on background thread; service will throw on validation which we catch below.
                    await System.Threading.Tasks.Task.Run(() =>
                    {
                        var key = CryptoConstants.AesKey;
                        var iv = CryptoConstants.AesIV;
                        ServiceRegistry.AslGenerator.CreateAndSaveAsl(data, path, key, iv);
                    });

                    DevExpress.XtraEditors.XtraMessageBox.Show("License generated successfully.");
                }
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException vx)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show(vx.Message);
            }
            catch (Exception)
            {
                DevExpress.XtraEditors.XtraMessageBox.Show("Operation failed. Contact admin.");
            }
            finally
            {
                _isSaving = false;
                try { btnExportAsl.Enabled = ServiceRegistry.AslGenerator != null; } catch { /* ignore */ }
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
        /// (Kept for backward compatibility but primary path is ServiceRegistry.Validation.BuildCanonicalJson)
        /// </summary>
        private string CanonicalizeJson(LicenseData payload)
        {
            if (payload == null) return string.Empty;
            var json = JsonConvert.SerializeObject(payload, Formatting.None);
            var withoutChecksum = JsonHelper.RemoveProperty(json, "ChecksumSHA256");
            var canon = JsonHelper.Canonicalize(withoutChecksum);
            return canon; // already compact from JsonHelper.Canonicalize
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
