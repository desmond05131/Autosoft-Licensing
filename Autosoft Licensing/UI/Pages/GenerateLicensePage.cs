/*
PAGE: GenerateLicensePage.cs
ROLE: Dealer Admin (Dealer EXE)
PURPOSE:
  Primary license generation screen. Load a customer's .ARL file, display parsed fields, allow admin to adjust allowed fields
  (expiry, modules, license type where allowed), generate the license key/payload, preview the payload, and export a .ASL file.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Utils;
using Autosoft_Licensing.Models.Enums; // <-- Added to resolve LicenseType references
using Autosoft_Licensing.Tools; // for InMemoryLicenseDatabaseService fallback

namespace Autosoft_Licensing.UI.Pages
{
    public partial class GenerateLicensePage : PageBase
    {
        // Services (injected via Initialize)
        private IArlReaderService _arlReader;
        private IAslGeneratorService _aslService;
        private IProductService _productService;
        private ILicenseDatabaseService _dbService;
        private IUserService _userService;

        // Current state
        private ArlRequest _currentRequest;
        private AslPayload _currentPayload;

        // Internal mutable row used as the grid datasource so the Enabled checkbox is editable.
        private class ModuleRow
        {
            public bool Enabled { get; set; }
            public string ModuleName { get; set; }
            public string ModuleCode { get; set; }
        }

        public GenerateLicensePage()
        {
            InitializeComponent();
            try
            {
                // Navigation buttons -> FireNavigate
                // REPLACED: use BindNavigationEvent to ensure clicks on child label/icon also trigger navigation.
                if (btnNav_GenerateLicense != null) BindNavigationEvent(btnNav_GenerateLicense, "GenerateLicensePage");
                if (btnNav_LicenseRecords != null) BindNavigationEvent(btnNav_LicenseRecords, "LicenseRecordsPage");
                if (btnNav_ManageProduct != null) BindNavigationEvent(btnNav_ManageProduct, "ManageProductPage");
                if (btnNav_ManageUser != null) BindNavigationEvent(btnNav_ManageUser, "ManageUserPage");
                if (btnNavLogoutText != null) BindNavigationEvent(btnNavLogoutText, "Logout");
                if (pnlNavLogout != null) BindNavigationEvent(pnlNavLogout, "Logout");
            }
            catch { /* best-effort, avoid exceptions in ctor */ }

            try
            {
                if (!DesignMode)
                {
                    // Dates
                    dtIssueDate.DateTime = DateTime.UtcNow.Date;
                    dtExpireDate.DateTime = dtIssueDate.DateTime;

                    // Radio items
                    rgLicenseType.Properties.Items.Clear();
                    rgLicenseType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[]
                    {
                        new DevExpress.XtraEditors.Controls.RadioGroupItem("Demo", "Demo"),
                        new DevExpress.XtraEditors.Controls.RadioGroupItem("Subscription", "Subscription"),
                        new DevExpress.XtraEditors.Controls.RadioGroupItem("Permanent", "Permanent")
                    });
                    rgLicenseType.SelectedIndex = 1; // Subscription default

                    // Numeric
                    numSubscriptionMonths.Properties.IsFloatValue = false;
                    numSubscriptionMonths.Properties.MinValue = 1;
                    numSubscriptionMonths.Properties.MaxValue = 1200;
                    numSubscriptionMonths.Value = 12;

                    // Attach event handlers
                    btnUploadArl.Click += btnUploadArl_Click;
                    rgLicenseType.SelectedIndexChanged += rgLicenseType_SelectedIndexChanged;
                    numSubscriptionMonths.ValueChanged += numSubscriptionMonths_ValueChanged;
                    btnGenerateKey.Click += btnGenerateKey_Click;
                    btnPreview.Click += btnPreview_Click;
                    btnDownload.Click += btnDownload_Click;

                    // Default states
                    btnGenerateKey.Enabled = false;
                    btnPreview.Enabled = false;
                    btnDownload.Enabled = false;

                    // Best-effort ServiceRegistry wiring (omitted here for brevity; unchanged)
                    try { if (_arlReader == null) _arlReader = ServiceRegistry.ArlReader; } catch { }
                    try { if (_aslService == null) _aslService = ServiceRegistry.AslGenerator; } catch { }
                    try { if (_productService == null) _productService = ServiceRegistry.Product; } catch { }
                    try
                    {
                        if (_dbService == null) _dbService = ServiceRegistry.Database;
                    }
                    catch
                    {
                        try
                        {
                            var memDb = new Autosoft_Licensing.Tools.InMemoryLicenseDatabaseService();
                            ServiceRegistry.Database = memDb;
                            _dbService = memDb;
                        }
                        catch
                        {
                            _dbService = null;
                        }
                    }
                    try { if (_userService == null) _userService = ServiceRegistry.User; } catch { }

                    // Ensure modules grid is editable
                    try
                    {
                        var gv = grdModules.MainView as GridView;
                        if (gv != null)
                        {
                            gv.OptionsBehavior.Editable = true;
                            gv.OptionsView.ShowAutoFilterRow = false;
                            gv.OptionsView.ShowGroupPanel = false;
                        }
                        if (colEnabled != null)
                        {
                            colEnabled.OptionsColumn.AllowEdit = true;
                            try
                            {
                                var chk = new RepositoryItemCheckEdit();
                                chk.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
                                grdModules.RepositoryItems.Add(chk);
                                colEnabled.ColumnEdit = chk;
                            }
                            catch { }
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                try { System.Diagnostics.Debug.WriteLine($"GenerateLicensePage ctor suppressed exception: {ex}"); } catch { }
            }
        }

        /// <summary>
        /// Inject runtime services (host should call this).
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
            _user_service_check(userService);
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        // small helper to avoid altering the original method signature semantics (keeps ctor stable)
        private void _user_service_check(IUserService _dummy) { /* no-op to satisfy static analysis placement */ }

        #region Event handlers (existing)

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

                ArlRequest arl;
                try
                {
                    if (_arlReader == null) throw new InvalidOperationException("ARL reader not initialized.");
                    arl = _arlReader.ParseArl(ofd.FileName);
                }
                catch (ValidationException)
                {
                    ShowError("Invalid license request file.");
                    return;
                }
                catch (Exception)
                {
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

                // Populate fields
                _currentRequest = arl;
                txtCompanyName.Text = arl.CompanyName;
                txtProductId.Text = arl.ProductID;

                var productName = string.IsNullOrWhiteSpace(arl.ProductName)
                    ? (_productService != null ? _product_service_getname_safe(arl.ProductID) : string.Empty)
                    : arl.ProductName;
                txtProductName.Text = productName ?? string.Empty;

                // NEW: Currency mapping from ARL
                try
                {
                    txtCurrency.Text = arl.CurrencyCode ?? string.Empty;
                }
                catch { txtCurrency.Text = string.Empty; }

                // Bind modules (admin selects manually)
                try
                {
                    var modules = (_productService != null)
                        ? (_product_service_getmodules_safe(arl.ProductID) ?? Enumerable.Empty<ModuleDto>())
                        : Enumerable.Empty<ModuleDto>();
                    BindModules(modules, Enumerable.Empty<string>());
                }
                catch
                {
                    BindModules(Enumerable.Empty<ModuleDto>(), Enumerable.Empty<string>());
                }

                dtIssueDate.DateTime = DateTime.UtcNow.Date; // display local date, stored using ToUtc when saving

                // Respect the requested months but enforce Demo -> 1 month representation in UI.
                int months = Math.Max(1, arl.RequestedPeriodMonths);
                if (string.Equals(arl?.LicenseType, "Demo", StringComparison.Ordinal))
                    months = 1;

                numSubscriptionMonths.Value = months;
                dtExpireDate.DateTime = dtIssueDate.DateTime.AddMonths(months);

                // Set LicenseType radio selection visually: Demo -> Demo, otherwise treat Paid as Subscription
                if (string.Equals(arl.LicenseType, "Demo", StringComparison.Ordinal))
                    rgLicenseType.SelectedIndex = 0; // Demo
                else
                    rgLicenseType.SelectedIndex = 1; // Subscription (treat Paid as subscription by default)

                btnGenerateKey.Enabled = true;
                btnPreview.Enabled = false;
                btnDownload.Enabled = false;
            }
            catch (Exception)
            {
                ShowError("Operation failed. Contact admin.");
            }
        }

        private string _product_service_getname_safe(string productId)
        {
            try { return _productService.GetProductName(productId); } catch { return string.Empty; }
        }
        private IEnumerable<ModuleDto> _product_service_getmodules_safe(string productId)
        {
            try { return _productService.GetModulesByProductId(productId); } catch { return Enumerable.Empty<ModuleDto>(); }
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
                else
                {
                    numSubscriptionMonths.Enabled = false;
                    dtExpireDate.DateTime = DateTime.SpecifyKind(DateTime.MaxValue.Date, DateTimeKind.Utc).ToLocalTime();
                }
            }
            catch
            {
                // ignore UI update failures
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
                if (_currentRequest == null || string.IsNullOrWhiteSpace(txtCompanyName.Text) || string.IsNullOrWhiteSpace(txtProductId.Text))
                {
                    ShowError("Invalid license request file.");
                    return;
                }

                var company = txtCompanyName.Text.Trim();
                var product = txtProductId.Text.Trim();
                var issueLocal = dtIssueDate.DateTime.Date;
                var expireLocal = dtExpireDate.DateTime.Date;

                if (expireLocal < issueLocal)
                {
                    ShowError("Operation failed. Contact admin.");
                    return;
                }

                var issueUtc = ToUtc(issueLocal);
                var expireUtc = ToUtc(expireLocal);

                if (_dbService != null && _dbService.ExistsDuplicateLicense(company, product, issueUtc, expireUtc))
                {
                    ShowError("Duplicate license exists for same Company, Product, IssueDate and ExpiryDate.");
                    return;
                }

                try
                {
                    if (_aslService == null) throw new InvalidOperationException("ASL generator service not initialized.");

                    _currentPayload = new AslPayload
                    {
                        LicenseKey = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 32),
                        CompanyName = txtCompanyName.Text.Trim(),
                        ProductID = txtProductId.Text.Trim(),
                        DealerCode = _currentRequest?.DealerCode,
                        ValidFromUtc = ToUtc(dtIssueDate.DateTime.Date),
                        ValidToUtc = ToUtc(dtExpireDate.DateTime.Date),
                        ModuleCodes = GetSelectedModuleCodes(),
                        LicenseType = rgLicenseType.Properties.Items[rgLicenseType.SelectedIndex].Value?.ToString()
                    };

                    txtLicenseKey.Text = _currentPayload.LicenseKey ?? string.Empty;

                    btnPreview.Enabled = true;
                    btnDownload.Enabled = true;
                }
                catch (ValidationException vx)
                {
                    ShowError(vx.Message);
                    return;
                }
                catch
                {
                    ShowError("Operation failed. Contact admin.");
                    return;
                }
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
                    ShowError("Operation failed. Contact admin.");
                    return;
                }

                try { _currentPayload.ModuleCodes = GetSelectedModuleCodes(); } catch { }

                var data = new LicenseData
                {
                    CompanyName = _currentPayload.CompanyName,
                    ProductID = _currentPayload.ProductID,
                    DealerCode = _currentPayload.DealerCode,
                    ValidFromUtc = _currentPayload.ValidFromUtc,
                    ValidToUtc = _currentPayload.ValidToUtc,
                    LicenseKey = _currentPayload.LicenseKey ?? string.Empty,
                    ModuleCodes = _currentPayload.ModuleCodes?.ToList() ?? new List<string>(),
                    CurrencyCode = txtCurrency.Text ?? null
                };

                if (!string.IsNullOrWhiteSpace(_currentPayload.LicenseType) && Enum.TryParse<LicenseType>(_currentPayload.LicenseType, true, out var parsed))
                    data.LicenseType = parsed;
                else
                    data.LicenseType = LicenseType.Subscription;

                using (var preview = new PreviewLicenseForm())
                {
                    preview.Initialize(data);
                    preview.ShowDialog();
                }
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
                    ShowError("Operation failed. Contact admin.");
                    return;
                }

                try { _currentPayload.ModuleCodes = GetSelectedModuleCodes(); } catch { }

                var company = txtCompanyName.Text?.Trim();
                var product = txtProductId.Text?.Trim();
                var issueUtc = ToUtc(dtIssueDate.DateTime.Date);
                var expireUtc = ToUtc(dtExpireDate.DateTime.Date);

                if (_dbService != null && _dbService.ExistsDuplicateLicense(company, product, issueUtc, expireUtc))
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

                string base64Asl = null;

                try
                {
                    if (_asl_service_check() == null) throw new InvalidOperationException("ASL generator not initialized.");

                    // Map payload to LicenseData
                    var licenseData = new LicenseData
                    {
                        CompanyName = _currentPayload.CompanyName,
                        ProductID = _currentPayload.ProductID,
                        DealerCode = _currentPayload.DealerCode,
                        ValidFromUtc = _currentPayload.ValidFromUtc,
                        ValidToUtc = _currentPayload.ValidToUtc,
                        LicenseKey = _currentPayload.LicenseKey ?? string.Empty,
                        ModuleCodes = _currentPayload.ModuleCodes?.ToList() ?? new List<string>(),
                        CurrencyCode = string.IsNullOrWhiteSpace(txtCurrency.Text) ? null : txtCurrency.Text
                    };

                    if (!string.IsNullOrWhiteSpace(_currentPayload.LicenseType) && Enum.TryParse<LicenseType>(_currentPayload.LicenseType, true, out var lt2))
                        licenseData.LicenseType = lt2;
                    else
                        licenseData.LicenseType = LicenseType.Subscription;

                    // CRITICAL FIX (already implemented): same string used for disk and DB
                    base64Asl = _aslService.CreateAsl(licenseData, CryptoConstants.AesKey, CryptoConstants.AesIV, ensureLicenseKey: true);

                    // Write to disk
                    try
                    {
                        ServiceRegistry.File?.WriteFileBase64(sfd.FileName, base64Asl);
                    }
                    catch
                    {
                        System.IO.File.WriteAllText(sfd.FileName, base64Asl ?? string.Empty, new System.Text.UTF8Encoding(false));
                    }
                }
                catch (ArgumentNullException) { throw; }
                catch (ValidationException vx)
                {
                    ShowError(vx.Message);
                    return;
                }
                catch
                {
                    ShowError("Operation failed. Contact admin.");
                    return;
                }

                // Insert metadata to DB (include the EXACT same Base64 string)
                try
                {
                    if (_dbService != null)
                    {
                        var meta = new LicenseMetadata
                        {
                            CompanyName = _currentPayload.CompanyName,
                            ProductID = _current_payload_productid_safe(),
                            DealerCode = _currentPayload.DealerCode,
                            LicenseKey = _currentPayload.LicenseKey,
                            ValidFromUtc = _currentPayload.ValidFromUtc,
                            ValidToUtc = _currentPayload.ValidToUtc,
                            LicenseType = Enum.TryParse<LicenseType>(_currentPayload.LicenseType, true, out var lt3) ? lt3 : LicenseType.Subscription,
                            ImportedOnUtc = ServiceRegistry.Clock.UtcNow,
                            RawAslBase64 = base64Asl, // EXACTLY the same Base64 string saved to disk
                            Remarks = memRemark?.Text,
                            ModuleCodes = _currentPayload.ModuleCodes?.ToList() ?? new List<string>(),
                            CurrencyCode = string.IsNullOrWhiteSpace(txtCurrency.Text) ? null : txtCurrency.Text
                        };
                        _dbService.InsertLicense(meta);
                    }
                }
                catch
                {
                    // non-blocking
                }

                ShowInfo("License generated successfully.", "Success");

                // NEW: UI LOCKING — prevent accidental re-download/generation without starting fresh.
                try
                {
                    btnGenerateKey.Enabled = false;
                    btnDownload.Enabled = false;
                    // Keep Preview enabled so user can still inspect current payload if needed
                    // Optionally also lock preview: uncomment next line to disable
                    // btnPreview.Enabled = false;
                }
                catch { /* ignore UI state failures */ }
            }
            catch (Exception)
            {
                ShowError("Operation failed. Contact admin.");
            }
        }

        private IAslGeneratorService _asl_service_check() { return _aslService; }
        private string _current_payload_productid_safe()
        {
            try { return _currentPayload.ProductID ?? string.Empty; } catch { return string.Empty; }
        }

        #endregion

        #region Helpers

        private void BindModules(IEnumerable<ModuleDto> productModules, IEnumerable<string> enabledModuleCodes)
        {
            try
            {
                var list = productModules?
                    .Select(m => new ModuleRow
                    {
                        Enabled = enabledModuleCodes?.Contains(m.ModuleCode) ?? false,
                        ModuleName = m.ModuleName ?? m.ModuleCode,
                        ModuleCode = m.ModuleCode
                    })
                    .ToList() ?? new List<ModuleRow>();

                var binding = new BindingList<ModuleRow>(list);
                grdModules.DataSource = binding;

                colEnabled.FieldName = "Enabled";
                colModuleName.FieldName = "ModuleName";
                colModuleName.OptionsColumn.AllowEdit = false;

                try
                {
                    var gv = grdModules.MainView as GridView;
                    if (gv != null)
                    {
                        gv.OptionsBehavior.Editable = true;
                        gv.OptionsCustomization.AllowColumnMoving = false;
                        gv.OptionsView.ShowIndicator = false;

                        var enabledCol = gv.Columns.ColumnByFieldName("Enabled");
                        if (enabledCol != null)
                        {
                            enabledCol.OptionsColumn.AllowEdit = true;
                            try
                            {
                                var chk = new RepositoryItemCheckEdit();
                                chk.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
                                grdModules.RepositoryItems.Add(chk);
                                enabledCol.ColumnEdit = chk;
                            }
                            catch { }
                        }

                        var nameCol = gv.Columns.ColumnByFieldName("ModuleName");
                        if (nameCol != null)
                        {
                            nameCol.OptionsColumn.AllowEdit = false;
                            nameCol.Caption = "Module";
                        }

                        gv.RefreshData();
                    }
                }
                catch { }
            }
            catch
            {
                grdModules.DataSource = null;
            }
        }

        private IEnumerable<string> GetSelectedModuleCodes()
        {
            var list = new List<string>();
            try
            {
                var view = grdModules.MainView as GridView;
                if (view == null) return list;
                for (int i = 0; i < view.DataRowCount; i++)
                {
                    var row = view.GetRow(i);
                    if (row == null) continue;

                    if (row is ModuleRow mr)
                    {
                        if (mr.Enabled && !string.IsNullOrEmpty(mr.ModuleCode))
                            list.Add(mr.ModuleCode);
                        continue;
                    }

                    var propEnabled = row.GetType().GetProperty("Enabled");
                    var propCode = row.GetType().GetProperty("ModuleCode");
                    if (propEnabled != null && propCode != null)
                    {
                        var enabled = (bool)(propEnabled.GetValue(row) ?? false);
                        if (enabled)
                        {
                            var code = propCode.GetValue(row)?.ToString();
                            if (!string.IsNullOrEmpty(code)) list.Add(code);
                        }
                    }
                }
            }
            catch { }
            return list;
        }

        #endregion

        private void grpModules_Paint(object sender, PaintEventArgs e)
        {

        }

        public override void InitializeForRole(User user)
        {
            try
            {
                if (user == null) return;

                // Role-based visibility (requested replacement)
                if (btnNav_GenerateLicense != null) btnNav_GenerateLicense.Visible = user.CanGenerateLicense;
                if (btnNav_LicenseRecords != null) btnNav_LicenseRecords.Visible = user.CanViewRecords;
                if (btnNav_ManageProduct != null) btnNav_ManageProduct.Visible = user.CanManageProduct;
                if (btnNav_ManageUser != null) btnNav_ManageUser.Visible = user.CanManageUsers;
                if (btnNavLogoutText != null) btnNavLogoutText.Visible = true;

                // Optionally keep action buttons enabled based on page rules (no change requested here)
                // e.g., btnGenerateKey.Enabled = user.CanGenerateLicense;
            }
            catch { /* ignore */ }
        }
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

