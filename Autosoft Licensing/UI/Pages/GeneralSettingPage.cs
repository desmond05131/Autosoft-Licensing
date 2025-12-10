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
                if (btnLogout != null) BindNavigationEvent(btnLogout, "Logout");
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
                int demo = SafeParseInt(_dbService.GetSetting("Duration_Demo_Months", "1"), 1);
                int sub = SafeParseInt(_dbService.GetSetting("Duration_Sub_Months", "12"), 12);
                int perm = SafeParseInt(_dbService.GetSetting("Duration_Perm_Years", "10"), 10);

                // Clamp within control limits
                if (demo < 1) demo = 1; if (demo > 12) demo = 12;
                if (sub < 1) sub = 1; if (sub > 120) sub = 120;
                if (perm < 1) perm = 1; if (perm > 99) perm = 99;

                spinDemo.Value = demo;
                spinSub.Value = sub;
                spinPerm.Value = perm;
            }
            catch
            {
                // Keep safe defaults if DB read fails
                spinDemo.Value = 1;
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

                _dbService.SaveSetting("Duration_Demo_Months", ((int)spinDemo.Value).ToString());
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
            }
            catch { /* ignore */ }
        }
    }
}