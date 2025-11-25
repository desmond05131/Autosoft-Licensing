/*
PAGE: GenerateLicensePage.cs
ROLE: Dealer Admin (Dealer EXE)
PURPOSE:
  Primary license generation screen. Load a customer's .ARL file, display parsed fields, allow admin to adjust allowed fields
  (expiry, modules, license type where allowed), generate the license key/payload, preview the license payload, and export a .ASL file.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Utils; // for CryptoConstants

namespace Autosoft_Licensing.UI.Pages
{
    public partial class GenerateLicensePage : PageBase
    {
        // Service dependencies (injected via Initialize to keep design-time safe)
        private IArlReaderService _arlReader;
        private IAslGeneratorService _aslService;
        private IProductService _productService;
        private ILicenseDatabaseService _dbService;
        private IUserService _userService;

        // Current loaded request / in-memory license data
        private ArlRequest _currentRequest;
        private LicenseData _currentPayload;

        public GenerateLicensePage()
        {
            InitializeComponent();

            // Build navigation visuals (icons, bar) at runtime
            if (!DesignMode)
            {
                try
                {
                    InitializeNavigation();
                }
                catch
                {
                    // Non-fatal: continue with default simple buttons if navigation initialization fails.
                }
            }

            // Move DevExpress property-rich initialization and event hookup here so the designer parser doesn't evaluate them.
            if (!DesignMode)
            {
                // Basic UI defaults
                dtIssueDate.DateTime = DateTime.Today;
                numSubscriptionMonths.Properties.IsFloatValue = false;
                numSubscriptionMonths.Properties.MinValue = 1;
                numSubscriptionMonths.Properties.MaxValue = 1200;
                numSubscriptionMonths.Value = 12;

                // DateEdit behaviours (safe to set at runtime)
                dtIssueDate.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
                dtIssueDate.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.False;
                dtExpireDate.Properties.VistaDisplayMode = DevExpress.Utils.DefaultBoolean.True;
                dtExpireDate.Properties.VistaEditTime = DevExpress.Utils.DefaultBoolean.False;

                // Radio items (do not call into Properties in the designer file)
                rgLicenseType.Properties.Items.Clear();
                rgLicenseType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[]
                {
                    new DevExpress.XtraEditors.Controls.RadioGroupItem("Demo", "Demo"),
                    new DevExpress.XtraEditors.Controls.RadioGroupItem("Subscription", "Subscription"),
                    new DevExpress.XtraEditors.Controls.RadioGroupItem("Permanent", "Permanent")
                });
                rgLicenseType.SelectedIndex = 1; // default to Subscription

                // Wire events here (designer will not attempt to evaluate these handlers)
                btnUploadArl.Click += btnUploadArl_Click;
                rgLicenseType.SelectedIndexChanged += rgLicenseType_SelectedIndexChanged;
                numSubscriptionMonths.ValueChanged += numSubscriptionMonths_ValueChanged;
                btnGenerateKey.Click += btnGenerateKey_Click;
                btnPreview.Click += btnPreview_Click;
                btnDownload.Click += btnDownload_Click;

                // DEFAULT service assignments (use ServiceRegistry defaults so pages work without DI)
                // These can be overridden by calling Initialize(...) from composition root or tests.
                _arlReader ??= ServiceRegistry.ArlReader;
                _aslService ??= ServiceRegistry.AslGenerator;
                _productService ??= ServiceRegistry.Product;
                _dbService ??= ServiceRegistry.Database;
                _userService ??= ServiceRegistry.User;

                // Initial button states
                btnGenerateKey.Enabled = false;
                btnPreview.Enabled = false;
                btnDownload.Enabled = false;
            }
        }

        /// <summary>
        /// Inject runtime services (tests or composition root should call this).
        /// Keeps control friendly for unit tests by avoiding hard DI in ctor.
        /// Call this from host if you want to override the ServiceRegistry defaults.
        /// </summary>
        public void Initialize(
            IArlReaderService arlReader,
            IAslGeneratorService aslService,
            IProductService productService,
            ILicenseDatabaseService dbService,
            IUserService userService)
        {
            _arlReader = arlReader ?? throw new ArgumentNullException(nameof(arlReader));
            _aslService = aslService ?? throw new ArgumentNullException(nameof(aslService));
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        #region Event handlers (stubs)

        private void btnUploadArl_Click(object sender, EventArgs e)
        {
            try
            {
                using var ofd = new OpenFileDialog
                {
                    Filter = "Autosoft Request (*.arl)|*.arl|All files (*.*)|*.*",
                    Title = "Open ARL request file"
                };

                if (ofd.ShowDialog() != DialogResult.OK) return;

                // Parse .ARL using provided service
                ArlRequest arl;
                try
                {
                    arl = _arlReader.ParseArl(ofd.FileName);
                }
                catch (Exception)
                {
                    // Must show exact business string on parse/validation failure
                    ShowError("Invalid license request file.");
                    return;
                }

                if (arl == null
                    || string.IsNullOrWhiteSpace(arl.CompanyName)
                    || string.IsNullOrWhiteSpace(arl.ProductID)
                    || arl.RequestDateUtc == default)
                {
                    ShowError("Invalid license request file.");
                    return;
                }

                _currentRequest = arl;
                txtCompanyName.Text = arl.CompanyName;
                txtProductId.Text = arl.ProductID;

                var productName = string.IsNullOrWhiteSpace(arl.ProductName)
                    ? _productService.GetProductName(arl.ProductID)
                    : arl.ProductName;
                txtProductName.Text = productName ?? string.Empty;

                var modules = _productService.GetModulesByProductId(arl.ProductID)?.ToList() ?? new List<ModuleDto>();
                BindModules(modules, arl.ModuleCodes ?? new List<string>());

                dtIssueDate.DateTime = DateTime.Today;
                int months = Math.Max(1, arl.RequestedPeriodMonths);
                numSubscriptionMonths.Value = months;
                dtExpireDate.DateTime = dtIssueDate.DateTime.AddMonths(months);

                btnGenerateKey.Enabled = true;
                btnPreview.Enabled = false;
                btnDownload.Enabled = false;
            }
            catch (Exception)
            {
                ShowError("Operation failed. Contact admin.");
            }
        }

        private void rgLicenseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var selected = rgLicenseType.Properties.Items[rgLicenseType.SelectedIndex].Value?.ToString();
                if (string.Equals(selected, "Demo", StringComparison.OrdinalIgnoreCase))
                {
                    numSubscriptionMonths.Value = 1;
                    numSubscriptionMonths.Enabled = false;
                    dtExpireDate.DateTime = dtIssueDate.DateTime.AddMonths(1);
                }
                else if (string.Equals(selected, "Subscription", StringComparison.OrdinalIgnoreCase))
                {
                    numSubscriptionMonths.Enabled = true;
                    if (numSubscriptionMonths.Value <= 1) numSubscriptionMonths.Value = 12;
                    dtExpireDate.DateTime = dtIssueDate.DateTime.AddMonths((int)numSubscriptionMonths.Value);
                }
                else // Permanent
                {
                    numSubscriptionMonths.Enabled = false;
                    dtExpireDate.DateTime = dtIssueDate.DateTime.AddYears(100); // placeholder
                }
            }
            catch
            {
                // ignore
            }
        }

        private void numSubscriptionMonths_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                var selected = rgLicenseType.Properties.Items[rgLicenseType.SelectedIndex].Value?.ToString();
                if (string.Equals(selected, "Demo", StringComparison.OrdinalIgnoreCase))
                {
                    numSubscriptionMonths.Value = 1;
                    dtExpireDate.DateTime = dtIssueDate.DateTime.AddMonths(1);
                    return;
                }

                int months = Math.Max(1, (int)numSubscriptionMonths.Value);
                dtExpireDate.DateTime = dtIssueDate.DateTime.AddMonths(months);
            }
            catch { }
        }

        private void btnGenerateKey_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentRequest == null)
                {
                    ShowError("Invalid license request file.");
                    return;
                }

                var company = txtCompanyName.Text?.Trim();
                var product = txtProductId.Text?.Trim();
                var issueLocal = dtIssueDate.DateTime.Date;
                var expireLocal = dtExpireDate.DateTime.Date;

                if (expireLocal < issueLocal)
                {
                    ShowError("ExpireDate must be on or after IssueDate.");
                    return;
                }

                var issueUtc = ToUtc(issueLocal);
                var expireUtc = ToUtc(expireLocal);

                if (_dbService.ExistsDuplicateLicense(company, product, issueUtc, expireUtc))
                {
                    ShowError("Duplicate license exists for same Company, Product, IssueDate and ExpiryDate.");
                    return;
                }

                var selectedModules = GetSelectedModuleCodes();
                var licenseTypeValue = rgLicenseType.Properties.Items[rgLicenseType.SelectedIndex].Value?.ToString();

                // Build canonical LicenseData (use the project's LicenseData model)
                var licenseData = new LicenseData
                {
                    CompanyName = company,
                    ProductID = product,
                    DealerCode = _currentRequest.DealerCode,
                    ValidFromUtc = issueUtc,
                    ValidToUtc = expireUtc,
                    LicenseType = EnumTryParseLicenseType(licenseTypeValue),
                    CurrencyCode = _currentRequest.CurrencyCode,
                    ModuleCodes = selectedModules?.ToList() ?? new List<string>()
                };

                // Ensure a license key exists. Use ServiceRegistry.KeyGenerator for consistent keys.
                try
                {
                    if (string.IsNullOrWhiteSpace(licenseData.LicenseKey))
                    {
                        // KeyGenerator may throw on repeated collisions; bubble as user-safe message.
                        licenseData.LicenseKey = ServiceRegistry.KeyGenerator.GenerateKey(company, product);
                    }
                }
                catch
                {
                    ShowError("Operation failed. Contact admin.");
                    return;
                }

                // Save to in-memory payload for later download
                _currentPayload = licenseData;

                txtLicenseKey.Text = _currentPayload.LicenseKey ?? string.Empty;

                btnPreview.Enabled = true;
                btnDownload.Enabled = true;
            }
            catch (Exception)
            {
                ShowError("Operation failed. Contact admin.");
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentPayload == null)
                {
                    ShowError("No payload available to preview. Generate a license key first.");
                    return;
                }

                ShowInfo("Preview is not implemented in this skeleton. TODO: Show PreviewLicense modal.");
            }
            catch (Exception)
            {
                ShowError("Operation failed. Contact admin.");
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentPayload == null)
                {
                    ShowError("No payload available to download. Generate a license key first.");
                    return;
                }

                var company = _currentPayload.CompanyName;
                var product = _currentPayload.ProductID;
                var issueUtc = _currentPayload.ValidFromUtc;
                var expireUtc = _currentPayload.ValidToUtc;

                if (_dbService.ExistsDuplicateLicense(company, product, issueUtc, expireUtc))
                {
                    ShowError("Duplicate license exists for same Company, Product, IssueDate and ExpiryDate.");
                    return;
                }

                using var sfd = new SaveFileDialog
                {
                    Filter = "Autosoft License (*.asl)|*.asl|All files (*.*)|*.*",
                    FileName = $"{company}_{product}_{issueUtc:yyyyMMdd}.asl",
                    Title = "Save license file"
                };

                if (sfd.ShowDialog() != DialogResult.OK) return;

                // Create and save the ASL using the UI-friendly AslGenerator (passes through to LicenseService + FileService).
                // Provide CryptoConstants key/iv from config (Program.Main validates these exist).
                try
                {
                    _aslService.CreateAndSaveAsl(_currentPayload, sfd.FileName, CryptoConstants.AesKey, CryptoConstants.AesIV, ensureLicenseKey: true);
                }
                catch (ArgumentNullException) { throw; } // let higher catch handle generic message
                catch (ValidationException vx)
                {
                    // Validation exceptions should surface exact messages (e.g., license data invalid)
                    ShowError(vx.Message);
                    return;
                }
                catch
                {
                    ShowError("Operation failed. Contact admin.");
                    return;
                }

                var meta = new LicenseMetadata
                {
                    CompanyName = _currentPayload.CompanyName,
                    ProductID = _currentPayload.ProductID,
                    DealerCode = _currentPayload.DealerCode,
                    LicenseKey = _currentPayload.LicenseKey,
                    ValidFromUtc = _currentPayload.ValidFromUtc,
                    ValidToUtc = _currentPayload.ValidToUtc,
                    LicenseType = _currentPayload.LicenseType,
                    ImportedOnUtc = ServiceRegistry.Clock.UtcNow,
                    RawAslBase64 = null,
                    ModuleCodes = _currentPayload.ModuleCodes?.ToList() ?? new List<string>()
                };

                try
                {
                    _dbService.InsertLicense(meta);
                }
                catch
                {
                    ShowInfo("License saved to disk. Failed to insert metadata to DB (non-blocking).");
                    return;
                }

                ShowInfo("License generated successfully.", "Success");
            }
            catch (Exception)
            {
                ShowError("Operation failed. Contact admin.");
            }
        }

        #endregion

        #region Helper methods

        private void BindModules(IEnumerable<ModuleDto> productModules, IEnumerable<string> enabledModuleCodes)
        {
            // Build simple list and populate CheckedListBoxControl.
            try
            {
                chkModules.Items.Clear();

                foreach (var m in productModules ?? Enumerable.Empty<ModuleDto>())
                {
                    var item = new CheckedListBoxItem(m.ModuleCode, m.ModuleName ?? m.ModuleCode, enabledModuleCodes?.Contains(m.ModuleCode) ?? false);
                    chkModules.Items.Add(item);
                }
            }
            catch
            {
                // Fail-safe: ensure UI remains stable
                chkModules.Items.Clear();
            }
        }

        private IEnumerable<string> GetSelectedModuleCodes()
        {
            var list = new List<string>();
            try
            {
                foreach (CheckedListBoxItem it in chkModules.CheckedItems)
                {
                    if (it.Value != null)
                        list.Add(it.Value.ToString());
                }
            }
            catch
            {
                // fall back to empty list
            }
            return list;
        }

        private Models.Enums.LicenseType EnumTryParseLicenseType(string value)
        {
            if (Enum.TryParse<Models.Enums.LicenseType>(value, true, out var lt)) return lt;
            return Models.Enums.LicenseType.Subscription;
        }

        #endregion
    }
}



/// ?? Copilot Instructions — Fix UI layout for GenerateLicensePage
/// 
/// You must modify ONLY the UI/layout portions of GenerateLicensePage.cs,
/// GenerateLicensePage.Designer.cs, and GenerateLicensePage.Navigation.cs.
/// Use *DevExpress controls only*. WinForms controls are not allowed.
///
/// ?? GOAL — Make the page UI look **identical** to the wireframe:
/// (Displayed by the user in screenshot at path: /mnt/data/Screenshot 2025-11-24 170655.png)
///
/// ?????????????????????????????????????????????????????????????????
/// EXACT REQUIREMENTS YOU MUST FOLLOW:
///
/// 1. TOP NAVIGATION BAR (DevExpress)
///    • Four tabs/buttons with icons:
///         - Generate License
///         - License Records
///         - Manage Product
///         - Manage User
///    • Use DevExpress BarManager / Ribbon / or NavigationBar (NOT standard WinForms MenuStrip).
///    • Highlight “Generate License” as the active tab.
///    • Icons MUST be shown. If no icons exist, use temporary placeholders:
///         Example icons (embedded as 16x16 PNG):
///         • generate.png
///         • records.png
///         • product.png
///         • user.png
///    • Create an /Assets/ folder and load icons with:
///         ImageOptions.Image = Image.FromFile("Assets/generate.png");
///
/// 2. “Upload License File” button
///    • Align left, wide, same style as wireframe.
///    • Use DevExpress SimpleButton with consistent styling.
///
/// 3. INFO PANEL
///    • DevExpress GroupControl with title “Info”.
///    • Inside it place three **read-only** DevExpress TextEdit fields:
///         - Company Name
///         - Product ID
///         - Product Name
///    • Use a 2-column table layout identical to the wireframe.
///
/// 4. TYPES PANEL
///    • DevExpress GroupControl with title “Types”.
///    • Use RadioGroup (Demo / Subscription / Permanent).
///    • When “Subscription” is selected, show a numeric SpinEdit: “Months: <value>”.
///    • Make layout identical to wireframe (radio group left, Months field right).
///
/// 5. MODULES PANEL
///    • DevExpress GroupControl with title “Modules”.
///    • Use a vertical CheckListBoxControl (DevExpress)
///    • Must be scrollable.
///    • Height and width must match wireframe exactly.
///
/// 6. ISSUE DATE + EXPIRE DATE
///    • Use DevExpress DateEdit controls.
///    • Must auto-fill from backend values.
///    • Align exactly like wireframe:
///         Issue Date left, Expire Date under it.
///
/// 7. REMARK FIELD
///    • DevExpress MemoEdit (multi-line).
///    • Full width as in wireframe.
///    • Margin and indentation exactly same.
///
/// 8. LICENSE KEY FIELD + BUTTONS
///    • DevExpress TextEdit (read-only)
///    • Two buttons:
///         - “Generate License Key”
///         - “Download License”
///    • Buttons aligned horizontally on the right, identical to wireframe.
///
/// 9. SPACING, PADDING, ALIGNMENT MUST MATCH
///    • No WinForms default spacing.
///    • Use TableLayoutPanel or DevExpress LayoutControl.
///    • Ensure the page looks pixel-accurate to the wireframe.
///
/// 10. IMPORTANT RULES
///     • DO NOT use Windows.Forms controls.
///     • DO NOT change backend logic.
///     • DO NOT rename methods.
///     • You may rearrange Designer code safely.
///
/// ?????????????????????????????????????????????????????????????????
/// Use the wireframe as the authoritative source. Adjust positions,
/// widths, margins, paddings, and anchors until it visually matches.
/// 
/// Begin implementing now.

