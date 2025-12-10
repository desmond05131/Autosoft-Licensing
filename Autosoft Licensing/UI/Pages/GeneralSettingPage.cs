using System;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.UI.Pages
{
    public partial class GeneralSettingPage : PageBase
    {
        private ILicenseDatabaseService _dbService;

        public GeneralSettingPage()
        {
            InitializeComponent();

            try
            {
                // Nav wiring using PageBase helper
                if (btnNav_GenerateLicense != null) BindNavigationEvent(btnNav_GenerateLicense, "GenerateLicensePage");
                if (btnNav_LicenseRecords != null) BindNavigationEvent(btnNav_LicenseRecords, "LicenseRecordsPage");
                if (btnNav_ManageProduct != null) BindNavigationEvent(btnNav_ManageProduct, "ManageProductPage");
                if (btnNav_ManageUser != null) BindNavigationEvent(btnNav_ManageUser, "ManageUserPage");
                if (btnNav_GeneralSetting != null) BindNavigationEvent(btnNav_GeneralSetting, "GeneralSettingPage");
                // NEW: Logout nav bindings (panel + inner controls)
                if (btnNav_Logout != null) BindNavigationEvent(btnNav_Logout, "Logout");
                if (lblNav_Logout != null) BindNavigationEvent(lblNav_Logout, "Logout");
                if (picNav_Logout != null) BindNavigationEvent(picNav_Logout, "Logout");
            }
            catch { /* best-effort */ }

            try
            {
                if (!DesignMode)
                {
                    btnSave.Click += btnSave_Click;

                    // Best-effort ServiceRegistry wiring
                    try { if (_dbService == null) _dbService = ServiceRegistry.Database; } catch { }

                    LoadSettings();
                }
            }
            catch (Exception ex)
            {
                try { System.Diagnostics.Debug.WriteLine($"GeneralSettingPage ctor suppressed: {ex}"); } catch { }
            }
        }

        public void Initialize(ILicenseDatabaseService dbService)
        {
            _dbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                if (_dbService == null) return;

                // Defaults if not present
                int demoDays = SafeParseInt(_dbService.GetSetting("Duration_Demo_Days", "30"), 30);
                int subMonths = SafeParseInt(_dbService.GetSetting("Duration_Sub_Months", "12"), 12);
                int permYears = SafeParseInt(_dbService.GetSetting("Duration_Perm_Years", "10"), 10);

                // Clamp within control limits
                if (demoDays < 1) demoDays = 1; if (demoDays > 365) demoDays = 365;
                if (subMonths < 1) subMonths = 1; if (subMonths > 120) subMonths = 120;
                if (permYears < 1) permYears = 1; if (permYears > 999) permYears = 999;

                spinDemo.Value = demoDays;
                spinSub.Value = subMonths;
                spinPerm.Value = permYears;
            }
            catch
            {
                // Keep safe defaults if DB read fails
                spinDemo.Value = 30;
                spinSub.Value = 12;
                spinPerm.Value = 10;
            }
        }

        private int SafeParseInt(string s, int fallback)
        {
            if (int.TryParse(s, out var val)) return val;
            return fallback;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_dbService == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                _dbService.SaveSetting("Duration_Demo_Days", ((int)spinDemo.Value).ToString());
                _dbService.SaveSetting("Duration_Sub_Months", ((int)spinSub.Value).ToString());
                _dbService.SaveSetting("Duration_Perm_Years", ((int)spinPerm.Value).ToString());

                ShowInfo("Settings saved successfully.", "Success");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GeneralSettingPage.Save error: {ex}");
                ShowError("Operation failed. Contact admin.");
            }
        }

        public override void InitializeForRole(User user)
        {
            try
            {
                if (user == null) return;

                // Visibility of top nav according to user permissions
                if (btnNav_GenerateLicense != null) btnNav_GenerateLicense.Visible = user.CanGenerateLicense;
                if (btnNav_LicenseRecords != null) btnNav_LicenseRecords.Visible = user.CanViewRecords;
                if (btnNav_ManageProduct != null) btnNav_ManageProduct.Visible = user.CanManageProduct;
                if (btnNav_ManageUser != null) btnNav_ManageUser.Visible = user.CanManageUsers;

                // Settings page: typically Admin-only; if not Admin, keep visible only if they have ManageUser or ManageProduct (optional)
                bool isAdmin = string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase);
                if (btnNav_GeneralSetting != null) btnNav_GeneralSetting.Visible = isAdmin;

                // Save button only for Admin
                if (btnSave != null) btnSave.Enabled = isAdmin;

                // NEW: Logout always visible
                if (btnNav_Logout != null) btnNav_Logout.Visible = true;
            }
            catch { /* ignore */ }
        }
    }
}