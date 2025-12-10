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
using Autosoft_Licensing.Models.Enums;
using Autosoft_Licensing.Tools;

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

        // AppSettings-driven defaults helper (updated: Demo in Days)
        private (int demoDays, int subscriptionMonths, int permanentYears) GetDefaultDurations()
        {
            // Safe defaults
            int defDemoDays = 30;
            int defSubMonths = 12;
            int defPermYears = 10;

            try
            {
                if (_dbService != null)
                {
                    // Read Demo in Days (new key). Fallback 30. Clamp to [1..365].
                    defDemoDays = SafeParseInt(_dbService.GetSetting("Duration_Demo_Days", "30"), 30);
                    if (defDemoDays < 1) defDemoDays = 1;
                    if (defDemoDays > 365) defDemoDays = 365;

                    // Subscription months (unchanged). Clamp to [1..120].
                    defSubMonths = SafeParseInt(_dbService.GetSetting("Duration_Sub_Months", "12"), 12);
                    if (defSubMonths < 1) defSubMonths = 1;
                    if (defSubMonths > 120) defSubMonths = 120;

                    // Permanent years (unchanged). Clamp to [1..999].
                    defPermYears = SafeParseInt(_dbService.GetSetting("Duration_Perm_Years", "10"), 10);
                    if (defPermYears < 1) defPermYears = 1;
                    if (defPermYears > 999) defPermYears = 999;
                }
            }
            catch
            {
                // keep safe defaults if settings fail
            }

            return (defDemoDays, defSubMonths, defPermYears);
        }

        // Helper since SafeParseInt used above (kept local to avoid dependency ripple)
        private static int SafeParseInt(string s, int fallback)
        {
            return int.TryParse(s, out var v) ? v : fallback;
        }

        public GenerateLicensePage()
        {
            InitializeComponent();

            // FORCE EVENT WIRING: Ensure buttons are connected to their handlers
            if (btnUploadArl != null) btnUploadArl.Click += btnUploadArl_Click;
            if (btnGenerateKey != null) btnGenerateKey.Click += btnGenerateKey_Click;
            if (btnPreview != null) btnPreview.Click += btnPreview_Click;
            if (btnDownload != null) btnDownload.Click += btnDownload_Click;

            try
            {
                if (!DesignMode)
                {
                    // Standardized navigation wiring
                    if (btnNav_GenerateLicense != null) BindNavigationEvent(btnNav_GenerateLicense, "GenerateLicensePage");
                    if (btnNav_LicenseRecords != null) BindNavigationEvent(btnNav_LicenseRecords, "LicenseRecordsPage");
                    if (btnNav_ManageProduct != null) BindNavigationEvent(btnNav_ManageProduct, "ManageProductPage");
                    if (btnNav_ManageUser != null) BindNavigationEvent(btnNav_ManageUser, "ManageUserPage");
                    if (btnNav_GeneralSetting != null) BindNavigationEvent(btnNav_GeneralSetting, "GeneralSettingPage");

                    // Logout (panel + inner label + picture)
                    if (btnNav_Logout != null) BindNavigationEvent(btnNav_Logout, "Logout");
                    if (lblNav_Logout != null) BindNavigationEvent(lblNav_Logout, "Logout");
                    if (picNav_Logout != null) BindNavigationEvent(picNav_Logout, "Logout");
                }
            }
            catch { /* best-effort */ }
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

        private void _user_service_check(IUserService _dummy) { /* no-op to satisfy static analysis placement */ }

        #region Event handlers

        private void btnUploadArl_Click(object sender, EventArgs e)
        {
            try
            {
                if (_arlReader == null)
                {
                    ShowError("Operation failed. Contact admin.");
                    return;
                }

                // FIX: Implement file dialog and parsing
                using (var ofd = new OpenFileDialog())
                {
                    ofd.Filter = "License Request Files (*.arl)|*.arl|All files (*.*)|*.*";
                    ofd.Title = "Select License Request";
                    if (ofd.ShowDialog() != DialogResult.OK) return;

                    try
                    {
                        // 1. Parse ARL
                        _currentRequest = _arlReader.ParseArl(ofd.FileName);
                        if (_currentRequest == null) throw new ValidationException("Invalid license request file.");

                        // 2. Populate Header Fields
                        txtCompanyName.Text = _currentRequest.CompanyName ?? string.Empty;
                        txtProductId.Text = _currentRequest.ProductID ?? string.Empty;

                        // Look up product name
                        string productName = string.Empty;
                        if (_productService != null && !string.IsNullOrEmpty(_currentRequest.ProductID))
                        {
                            productName = _productService.GetProductName(_currentRequest.ProductID);
                        }
                        txtProductName.Text = productName;

                        // Populate Currency if available in request (or leave/default)
                        if (txtCurrency != null)
                            txtCurrency.Text = _currentRequest.CurrencyCode ?? "MYR";

                        // 3. Load Product Modules
                        IEnumerable<ModuleDto> productModules = Enumerable.Empty<ModuleDto>();
                        if (_productService != null && !string.IsNullOrEmpty(_currentRequest.ProductID))
                        {
                            productModules = _productService.GetModulesByProductId(_currentRequest.ProductID);
                        }

                        // 4. Bind Grid (pre-select modules from request if any, otherwise default to all active or none)
                        // ARL V2 schema doesn't send modules, so usually we default to 'Select All' or 'Select None' based on business rule.
                        // Here we select all available modules by default for convenience.
                        var modulesToEnable = productModules.Select(m => m.ModuleCode).ToList();
                        BindModules(productModules, modulesToEnable);

                        // 5. Apply Defaults (Dates & License Type)
                        var (defDemoDays, defSubMonths, defPermYears) = GetDefaultDurations();
                        var issueLocal = DateTime.Now.Date;
                        dtIssueDate.DateTime = issueLocal;

                        // Determine License Type
                        LicenseType lt = LicenseType.Subscription;
                        if (!string.IsNullOrWhiteSpace(_currentRequest.LicenseType))
                        {
                            if (string.Equals(_currentRequest.LicenseType, "Demo", StringComparison.OrdinalIgnoreCase))
                                lt = LicenseType.Demo;
                            else if (string.Equals(_currentRequest.LicenseType, "Permanent", StringComparison.OrdinalIgnoreCase))
                                lt = LicenseType.Permanent;
                            else
                                lt = LicenseType.Subscription;
                        }

                        // Set Radio Group
                        if (rgLicenseType != null)
                        {
                            // Try setting by enum, if radio items are mapped to enum values
                            rgLicenseType.EditValue = lt;
                            // Fallback if mapped to strings
                            if (rgLicenseType.EditValue == null)
                                rgLicenseType.EditValue = lt.ToString();
                        }

                        // Set Expiry
                        if (lt == LicenseType.Demo)
                        {
                            dtExpireDate.DateTime = issueLocal.AddDays(defDemoDays);
                            if (numSubscriptionMonths != null) numSubscriptionMonths.Enabled = false;
                        }
                        else if (lt == LicenseType.Permanent)
                        {
                            dtExpireDate.DateTime = issueLocal.AddYears(defPermYears);
                            if (numSubscriptionMonths != null) numSubscriptionMonths.Enabled = false;
                        }
                        else // Subscription
                        {
                            var months = _currentRequest.RequestedPeriodMonths > 0 ? _currentRequest.RequestedPeriodMonths : defSubMonths;
                            dtExpireDate.DateTime = issueLocal.AddMonths(months);
                            if (numSubscriptionMonths != null)
                            {
                                numSubscriptionMonths.Enabled = true;
                                numSubscriptionMonths.Value = months;
                            }
                        }

                        // 6. Reset generation state
                        txtLicenseKey.Text = string.Empty;
                        btnGenerateKey.Enabled = true;
                        btnPreview.Enabled = false;
                        btnDownload.Enabled = false;

                        // Initialize payload container
                        _currentPayload = new AslPayload
                        {
                            CompanyName = _currentRequest.CompanyName,
                            ProductID = _currentRequest.ProductID,
                            DealerCode = _currentRequest.DealerCode,
                            LicenseKey = null
                        };
                    }
                    catch (ValidationException vx)
                    {
                        ShowError(vx.Message);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error parsing ARL: {ex}");
                        ShowError("Invalid license request file.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnUploadArl_Click error: " + ex);
                ShowError("Operation failed. Contact admin.");
            }
        }

        private void rgLicenseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                var (defDemoDays, defSubMonths, defPermYears) = GetDefaultDurations();

                var issueLocal = dtIssueDate?.DateTime.Date ?? DateTime.Now.Date;
                // Determine selected LicenseType from the radio group
                LicenseType selected = LicenseType.Subscription;
                try
                {
                    // Typical mapping: rgLicenseType.EditValue stores enum or string. Handle both.
                    var val = rgLicenseType?.EditValue;
                    if (val is LicenseType ltEnum)
                        selected = ltEnum;
                    else if (val is string s)
                    {
                        if (Enum.TryParse<LicenseType>(s, true, out var parsed)) selected = parsed;
                    }
                    // Map index if value mapping fails (0=Demo, 1=Sub, 2=Perm)
                    else if (rgLicenseType != null && rgLicenseType.SelectedIndex >= 0)
                    {
                        selected = (LicenseType)rgLicenseType.SelectedIndex;
                    }
                }
                catch { }

                if (selected == LicenseType.Demo)
                {
                    dtExpireDate.DateTime = issueLocal.AddDays(defDemoDays);
                    if (numSubscriptionMonths != null) numSubscriptionMonths.Enabled = false;
                }
                else if (selected == LicenseType.Permanent)
                {
                    dtExpireDate.DateTime = issueLocal.AddYears(defPermYears);
                    if (numSubscriptionMonths != null) numSubscriptionMonths.Enabled = false;
                }
                else // Subscription
                {
                    // If switching to sub, prefer current spinner value if valid, else default
                    int months = defSubMonths;
                    if (numSubscriptionMonths != null)
                    {
                        numSubscriptionMonths.Enabled = true;
                        if (numSubscriptionMonths.Value > 0) months = (int)numSubscriptionMonths.Value;
                        else numSubscriptionMonths.Value = defSubMonths;
                    }
                    dtExpireDate.DateTime = issueLocal.AddMonths(months);
                }

                // Update current payload type reflectively
                if (_currentPayload != null) _currentPayload.LicenseType = selected.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("rgLicenseType_SelectedIndexChanged error: " + ex);
            }
        }

        // Months spinner changed (guard against disabled states)
        private void numSubscriptionMonths_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Prevent overwriting calculations when not applicable (Demo/Permanent disable this)
                if (numSubscriptionMonths == null || !numSubscriptionMonths.Enabled) return;

                var issueLocal = dtIssueDate?.DateTime.Date ?? DateTime.Now.Date;
                var months = Math.Max(1, (int)numSubscriptionMonths.Value);
                dtExpireDate.DateTime = issueLocal.AddMonths(months);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("numSubscriptionMonths_ValueChanged error: " + ex);
            }
        }

        private void btnGenerateKey_Click(object sender, EventArgs e)
        {
            GenerateKey();
        }

        private void GenerateKey()
        {
            try
            {
                // Validate basic fields
                var company = txtCompanyName?.Text?.Trim();
                var productId = txtProductId?.Text?.Trim();
                if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(productId))
                {
                    ShowError("Please upload a valid request or fill in Company and Product ID.");
                    return;
                }

                // Read dates (local -> utc)
                var issueLocal = dtIssueDate?.DateTime ?? DateTime.Now.Date;
                var expireLocal = dtExpireDate?.DateTime ?? issueLocal.AddMonths(1);
                var validFromUtc = ToUtc(issueLocal);
                var validToUtc = ToUtc(expireLocal);

                // Duplicate check (must ignore deleted — handled in Database service)
                if (_dbService != null && _dbService.ExistsDuplicateLicense(company, productId, validFromUtc, validToUtc))
                {
                    // prevent generation when an active/valid duplicate exists
                    txtLicenseKey.Text = string.Empty;
                    ShowError("A license with the same Company, Product and dates already exists.");
                    // Keep Preview/Download disabled
                    btnPreview.Enabled = false;
                    btnDownload.Enabled = false;
                    return;
                }

                // Generate a 32-char key
                string key = null;
                try
                {
                    if (ServiceRegistry.KeyGenerator != null)
                        key = ServiceRegistry.KeyGenerator.GenerateKey(company, productId);
                }
                catch { }

                if (string.IsNullOrWhiteSpace(key))
                {
                    // Fallback
                    key = Guid.NewGuid().ToString("N").ToUpper();
                }

                txtLicenseKey.Text = key;

                // Update payload object
                if (_currentPayload == null) _currentPayload = new AslPayload();
                _currentPayload.CompanyName = company;
                _currentPayload.ProductID = productId;
                _currentPayload.LicenseKey = key;
                _currentPayload.ValidFromUtc = validFromUtc;
                _currentPayload.ValidToUtc = validToUtc;

                // Ensure DealerCode is set (fallback if manually entered)
                if (string.IsNullOrWhiteSpace(_currentPayload.DealerCode)) _currentPayload.DealerCode = "DEALER-001";

                // Ensure LicenseType is synced
                if (rgLicenseType != null && rgLicenseType.EditValue != null)
                    _currentPayload.LicenseType = rgLicenseType.EditValue.ToString();
                else
                    _currentPayload.LicenseType = LicenseType.Subscription.ToString();

                // Enable preview & download immediately after generation
                btnPreview.Enabled = true;
                btnDownload.Enabled = true;
            }
            catch (Exception ex)
            {
                ShowError("Failed to generate license key.");
                System.Diagnostics.Debug.WriteLine($"GenerateKey error: {ex}");
                // Ensure fields/buttons are in a safe state
                txtLicenseKey.Text = string.Empty;
                btnPreview.Enabled = false;
                btnDownload.Enabled = false;
            }
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentPayload == null)
                {
                    ShowError("Operation failed. Please generate a key first.");
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
                    ShowError("Operation failed. Please generate a key first.");
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
                    if (_aslService == null) throw new InvalidOperationException("ASL generator not initialized.");

                    var licenseData = new LicenseData
                    {
                        CompanyName = _currentPayload.CompanyName,
                        ProductID = _currentPayload.ProductID ?? string.Empty,
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

                    base64Asl = _aslService.CreateAsl(licenseData, CryptoConstants.AesKey, CryptoConstants.AesIV, ensureLicenseKey: true);

                    // Write to disk
                    try
                    {
                        if (ServiceRegistry.File != null)
                            ServiceRegistry.File.WriteFileBase64(sfd.FileName, base64Asl);
                        else
                            System.IO.File.WriteAllText(sfd.FileName, base64Asl ?? string.Empty, new System.Text.UTF8Encoding(false));
                    }
                    catch
                    {
                        // Fallback
                        System.IO.File.WriteAllText(sfd.FileName, base64Asl ?? string.Empty, new System.Text.UTF8Encoding(false));
                    }
                }
                catch (ValidationException vx)
                {
                    ShowError(vx.Message);
                    return;
                }
                catch (Exception)
                {
                    ShowError("Operation failed. Contact admin.");
                    return;
                }

                // Insert metadata to DB
                try
                {
                    if (_dbService != null)
                    {
                        var meta = new LicenseMetadata
                        {
                            CompanyName = _currentPayload.CompanyName,
                            ProductID = _currentPayload.ProductID ?? string.Empty,
                            DealerCode = _currentPayload.DealerCode,
                            LicenseKey = _currentPayload.LicenseKey,
                            ValidFromUtc = _currentPayload.ValidFromUtc,
                            ValidToUtc = _currentPayload.ValidToUtc,
                            LicenseType = Enum.TryParse<LicenseType>(_currentPayload.LicenseType, true, out var lt3) ? lt3 : LicenseType.Subscription,
                            ImportedOnUtc = ServiceRegistry.Clock.UtcNow,
                            RawAslBase64 = base64Asl,
                            Remarks = memRemark?.Text,
                            ModuleCodes = _currentPayload.ModuleCodes?.ToList() ?? new List<string>(),
                            CurrencyCode = string.IsNullOrWhiteSpace(txtCurrency.Text) ? null : txtCurrency.Text,
                            Status = LicenseStatus.Valid
                        };
                        _dbService.InsertLicense(meta);
                    }
                }
                catch { /* non-blocking */ }

                ShowInfo("License generated successfully.", "Success");

                // Lock UI
                try
                {
                    btnGenerateKey.Enabled = false;
                    btnDownload.Enabled = false;
                }
                catch { }
            }
            catch (Exception)
            {
                ShowError("Operation failed. Contact admin.");
            }
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

        private void grpModules_Paint(object sender, PaintEventArgs e) { }

        public override void InitializeForRole(User user)
        {
            try
            {
                if (user == null) return;

                if (btnNav_GenerateLicense != null) btnNav_GenerateLicense.Visible = user.CanGenerateLicense;
                if (btnNav_LicenseRecords != null) btnNav_LicenseRecords.Visible = user.CanViewRecords;
                if (btnNav_ManageProduct != null) btnNav_ManageProduct.Visible = user.CanManageProduct;
                if (btnNav_ManageUser != null) btnNav_ManageUser.Visible = user.CanManageUsers;

                if (btnNav_GeneralSetting != null)
                    btnNav_GeneralSetting.Visible = string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase);

                if (btnNav_Logout != null) btnNav_Logout.Visible = true;
            }
            catch { /* ignore */ }
        }

        #endregion
    }
}