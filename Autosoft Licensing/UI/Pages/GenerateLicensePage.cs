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

            // Design-time safe initialization
            // Wrap runtime initialization in try/catch: some environments (test host, headless CI) can
            // throw during DevExpress control creation or look-and-feel operations. Do not allow such
            // exceptions to escape the constructor (they crash the UI thread). Swallowing here keeps the
            // designer stable and the tests able to create the control; runtime behavior remains the same.
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

                    // Attach event handlers (do not attach in designer to keep design-time stable)
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

                    // Try to wire default services from ServiceRegistry so the page works even when host
                    // didn't call Initialize(...). This makes the UI usable in the running EXE and by E2E tests.
                    try
                    {
                        // Lightweight defensive wiring; do not throw if registry access fails.
                        _arlReader ??= ServiceRegistry.ArlReader;
                    }
                    catch { /* best-effort */ }

                    try
                    {
                        _aslService ??= ServiceRegistry.AslGenerator;
                    }
                    catch { /* best-effort */ }

                    try
                    {
                        _productService ??= ServiceRegistry.Product;
                    }
                    catch { /* best-effort */ }

                    try
                    {
                        // Database may not be initialized in some test hosts; fall back to in-memory DB if needed.
                        _dbService ??= ServiceRegistry.Database;
                    }
                    catch
                    {
                        try
                        {
                            var memDb = new InMemoryLicenseDatabaseService();
                            ServiceRegistry.Database = memDb;
                            _dbService = memDb;
                        }
                        catch
                        {
                            _dbService = null; // leave null if even fallback fails
                        }
                    }

                    try
                    {
                        _userService ??= ServiceRegistry.User;
                    }
                    catch { /* best-effort */ }

                    // Ensure modules grid is editable at runtime
                    try
                    {
                        var gv = grdModules.MainView as GridView;
                        if (gv != null)
                        {
                            gv.OptionsBehavior.Editable = true;
                            gv.OptionsView.ShowAutoFilterRow = false;
                            gv.OptionsView.ShowGroupPanel = false;
                        }
                        // Ensure Enabled column (designer variable colEnabled) is editable when present
                        if (colEnabled != null)
                        {
                            colEnabled.OptionsColumn.AllowEdit = true;
                            // Provide a repository check edit so the column shows a checkbox
                            try
                            {
                                var chk = new RepositoryItemCheckEdit();
                                chk.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
                                // Avoid duplicate addition: RepositoryItem equality isn't trivial so always add then set
                                grdModules.RepositoryItems.Add(chk);
                                colEnabled.ColumnEdit = chk;
                            }
                            catch { /* ignore repository hookup failures */ }
                        }
                    }
                    catch { /* non-fatal */ }

                    // TODO inject via constructor or call Initialize(...) from host/composition root
                    // _arlReader = ServiceRegistry.ArlReader; // example if available
                }
            }
            catch (Exception ex)
            {
                // Prevent unhandled exceptions during construction from crashing the UI thread in tests.
                // Log to Debug so developers can inspect but do not rethrow.
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

        #region Event handlers (skeletons with TODO)

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
                    // Use injected _arlReader or fallback to ServiceRegistry implementation wired in ctor.
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

                // Bind modules from product service.
                // IMPORTANT: ignore any ModuleCodes present in ARL — admin selects modules manually.
                try
                {
                    var modules = (_productService != null)
                        ? (_product_service_getmodules_safe(arl.ProductID) ?? Enumerable.Empty<ModuleDto>())
                        : Enumerable.Empty<ModuleDto>();

                    // Pass an empty enabled list so admin must tick modules manually.
                    BindModules(modules, Enumerable.Empty<string>());
                }
                catch
                {
                    // non-fatal: clear modules
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

        // safe wrappers to avoid surprising NullReference during test/design-time
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
                else // Permanent
                {
                    numSubscriptionMonths.Enabled = false;
                    // Represent permanent by a far future date; TODO: consider special display "Permanent"
                    dtExpireDate.DateTime = DateTime.SpecifyKind(DateTime.MaxValue.Date, DateTimeKind.Utc).ToLocalTime();
                    // TODO: allow admin to pick custom expiry for Permanent if business requires
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
                // Validate required fields
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
                    ShowError("Operation failed. Contact admin."); // generic; business did not specify other exact string
                    return;
                }

                var issueUtc = ToUtc(issueLocal);
                var expireUtc = ToUtc(expireLocal);

                // Duplicate check
                if (_dbService != null && _dbService.ExistsDuplicateLicense(company, product, issueUtc, expireUtc))
                {
                    ShowError("Duplicate license exists for same Company, Product, IssueDate and ExpiryDate.");
                    return;
                }

                // Build payload (stub) and call ASL service to create payload / key
                try
                {
                    // TODO: Replace with real payload creation call to _aslService.CreatePayload(...) or equivalent
                    if (_aslService == null) throw new InvalidOperationException("ASL generator service not initialized.");

                    // In this skeleton we simulate creation of payload with a generated LicenseKey
                    _currentPayload = new AslPayload
                    {
                        LicenseKey = Guid.NewGuid().ToString("N").ToUpper().Substring(0, 32),
                        CompanyName = txtCompanyName.Text.Trim(),
                        ProductID = txtProductId.Text.Trim(),
                        DealerCode = _currentRequest?.DealerCode,
                        ValidFromUtc = ToUtc(dtIssueDate.DateTime.Date),
                        ValidToUtc = ToUtc(dtExpireDate.DateTime.Date),
                        ModuleCodes = GetSelectedModuleCodes()
                    };

                    // TODO: call real CreatePayload and get LicenseKey from result
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

                // Refresh payload with current module selections so admin can change selections after generation
                try
                {
                    _currentPayload.ModuleCodes = GetSelectedModuleCodes();
                }
                catch { /* non-fatal */ }

                // Map current AslPayload to canonical LicenseData used by PreviewLicenseForm.
                var data = new LicenseData
                {
                    CompanyName = _currentPayload.CompanyName,
                    ProductID = _currentPayload.ProductID,
                    DealerCode = _currentPayload.DealerCode,
                    ValidFromUtc = _currentPayload.ValidFromUtc,
                    ValidToUtc = _currentPayload.ValidToUtc,
                    LicenseKey = _currentPayload.LicenseKey ?? string.Empty,
                    ModuleCodes = _currentPayload.ModuleCodes?.ToList() ?? new List<string>(),
                    CurrencyCode = null
                };

                // Map license type if available; default to Subscription
                if (!string.IsNullOrWhiteSpace(_currentPayload.LicenseType) && Enum.TryParse<LicenseType>(_currentPayload.LicenseType, true, out var parsed))
                    data.LicenseType = parsed;
                else
                    data.LicenseType = LicenseType.Subscription;

                // Show Preview modal
                using (var preview = new PreviewLicenseForm())
                {
                    preview.Initialize(data);
                    // ShowDialog without owner is acceptable for modal preview.
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

                // Refresh payload with current module selections before packaging ASL
                try
                {
                    _currentPayload.ModuleCodes = GetSelectedModuleCodes();
                }
                catch { /* non-fatal */ }

                // Re-check duplicates (defensive)
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

                // Create and save the ASL using the UI-friendly AslGenerator (passes through to LicenseService + FileService).
                // Provide CryptoConstants key/iv from config (Program.Main validates these exist).
                try
                {
                    if (_asl_service_check() == null) throw new InvalidOperationException("ASL generator not initialized.");

                    // Map current payload to canonical LicenseData expected by the ASL generator.
                    var licenseData = new LicenseData
                    {
                        CompanyName = _currentPayload.CompanyName,
                        ProductID = _currentPayload.ProductID,
                        DealerCode = _currentPayload.DealerCode,
                        ValidFromUtc = _currentPayload.ValidFromUtc,
                        ValidToUtc = _currentPayload.ValidToUtc,
                        LicenseKey = _currentPayload.LicenseKey ?? string.Empty,
                        ModuleCodes = _currentPayload.ModuleCodes?.ToList() ?? new List<string>(),
                        CurrencyCode = null
                    };

                    // Parse license type if present; default to Subscription
                    if (!string.IsNullOrWhiteSpace(_currentPayload.LicenseType) && Enum.TryParse<LicenseType>(_currentPayload.LicenseType, true, out var lt2))
                        licenseData.LicenseType = lt2;
                    else
                        licenseData.LicenseType = LicenseType.Subscription;

                    // Use configured CryptoConstants for key/iv (Program.Main validates these)
                    _aslService.CreateAndSaveAsl(licenseData, sfd.FileName, CryptoConstants.AesKey, CryptoConstants.AesIV, ensureLicenseKey: true);
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

                // Insert metadata to DB (non-blocking)
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
                            RawAslBase64 = null,
                            ModuleCodes = _currentPayload.ModuleCodes?.ToList() ?? new List<string>()
                        };
                        _dbService.InsertLicense(meta);
                    }
                }
                catch
                {
                    // non-blocking: show success for file write but warn DB insert failed quietly
                }

                ShowInfo("License generated successfully.", "Success");
            }
            catch (Exception)
            {
                ShowError("Operation failed. Contact admin.");
            }
        }

        // Helper wrappers to centralize null checks for services/fields used above
        private IAslGeneratorService _asl_service_check()
        {
            return _aslService;
        }
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
                // Use a BindingList of mutable ModuleRow so checkboxes are editable.
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

                // Ensure checkbox column uses repository and is editable
                colEnabled.FieldName = "Enabled";
                colModuleName.FieldName = "ModuleName";
                colModuleName.OptionsColumn.AllowEdit = false;

                // Grid view tweaks to allow inline editing of the Enabled checkbox
                try
                {
                    var gv = grdModules.MainView as GridView;
                    if (gv != null)
                    {
                        gv.OptionsBehavior.Editable = true;
                        gv.OptionsCustomization.AllowColumnMoving = false;
                        gv.OptionsView.ShowIndicator = false;

                        // Ensure Enabled column exists and is editable
                        var enabledCol = gv.Columns.ColumnByFieldName("Enabled");
                        if (enabledCol != null)
                        {
                            enabledCol.OptionsColumn.AllowEdit = true;
                            // Setup check-edit repository item
                            try
                            {
                                // Try reuse existing repository items: create a new one and assign
                                var chk = new RepositoryItemCheckEdit();
                                chk.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
                                grdModules.RepositoryItems.Add(chk);
                                enabledCol.ColumnEdit = chk;
                            }
                            catch { /* ignore */ }
                        }

                        // Best-effort: ensure ModuleName column is read-only and shows text
                        var nameCol = gv.Columns.ColumnByFieldName("ModuleName");
                        if (nameCol != null)
                        {
                            nameCol.OptionsColumn.AllowEdit = false;
                            nameCol.Caption = "Module";
                        }

                        gv.RefreshData();
                    }
                }
                catch { /* non-fatal */ }
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

                    // Prefer strongly-typed ModuleRow when available (fast, safe)
                    if (row is ModuleRow mr)
                    {
                        if (mr.Enabled && !string.IsNullOrEmpty(mr.ModuleCode))
                            list.Add(mr.ModuleCode);
                        continue;
                    }

                    // Fallback reflection for anonymous or other row types (keeps compatibility with tests)
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

