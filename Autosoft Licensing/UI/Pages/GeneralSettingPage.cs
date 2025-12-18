using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using DevExpress.XtraEditors;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Autosoft_Licensing.UI.Pages
{
    public partial class GeneralSettingPage : PageBase
    {
        private ILicenseDatabaseService _dbService;

        public GeneralSettingPage()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            this.AutoSize = false; // Ensure AutoSize doesn't shrink it

            // Override Spinner Limits (Fix for Max Value Issues)
            // By default DevExpress SpinEdits might be limited to 100 or decimal bounds.
            if (spinDemo != null) spinDemo.Properties.MaxValue = 9999;
            if (spinSub != null) spinSub.Properties.MaxValue = 9999;
            if (spinPerm != null)
            {
                spinPerm.Properties.MaxValue = 9999;

                // Make permanent period uneditable (read-only) with grey visual cue
                spinPerm.Properties.ReadOnly = true;
                spinPerm.Properties.AppearanceReadOnly.BackColor = Color.LightGray;
                spinPerm.Properties.AppearanceReadOnly.ForeColor = Color.Black;
                spinPerm.Properties.AppearanceReadOnly.Options.UseBackColor = true;
                spinPerm.Properties.AppearanceReadOnly.Options.UseForeColor = true;

                // Optional: prevent focus border to reinforce non-editable look
                spinPerm.Properties.AllowFocused = false;
            }

            try
            {
                // Nav wiring
                if (btnNav_GenerateLicense != null) BindNavigationEvent(btnNav_GenerateLicense, "GenerateLicensePage");
                if (btnNav_LicenseRecords != null) BindNavigationEvent(btnNav_LicenseRecords, "LicenseRecordsPage");
                if (btnNav_ManageProduct != null) BindNavigationEvent(btnNav_ManageProduct, "ManageProductPage");
                if (btnNav_ManageUser != null) BindNavigationEvent(btnNav_ManageUser, "ManageUserPage");
                if (btnNav_GeneralSetting != null) BindNavigationEvent(btnNav_GeneralSetting, "GeneralSettingPage");

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
                int permYears = SafeParseInt(_dbService.GetSetting("Duration_Perm_Years", "9999"), 9999);

                // Relaxed clamping (Fix for limited Demo/Perm periods)
                // Demo days: 1 to 9999
                if (demoDays < 1) demoDays = 1;
                if (demoDays > 9999) demoDays = 9999;

                // Sub months: 1 to 9999
                if (subMonths < 1) subMonths = 1;
                if (subMonths > 9999) subMonths = 9999;

                // Perm years: 1 to 9999
                if (permYears < 1) permYears = 1;
                if (permYears > 9999) permYears = 9999;

                spinDemo.Value = demoDays;
                spinSub.Value = subMonths;
                spinPerm.Value = permYears;
            }
            catch
            {
                // Keep safe defaults if DB read fails
                spinDemo.Value = 30;
                spinSub.Value = 12;
                spinPerm.Value = 9999;
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

                // Permanent is read-only; still persist current value from DB or UI display
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

                if (btnNav_GenerateLicense != null) btnNav_GenerateLicense.Visible = user.CanGenerateLicense;
                if (btnNav_LicenseRecords != null) btnNav_LicenseRecords.Visible = user.CanViewRecords;
                if (btnNav_ManageProduct != null) btnNav_ManageProduct.Visible = user.CanManageProduct;
                if (btnNav_ManageUser != null) btnNav_ManageUser.Visible = user.CanManageUsers;

                bool isAdmin = string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase);
                if (btnNav_GeneralSetting != null) btnNav_GeneralSetting.Visible = isAdmin;

                if (btnSave != null) btnSave.Enabled = isAdmin;
                if (btnNav_Logout != null) btnNav_Logout.Visible = true;
            }
            catch { /* ignore */ }
        }
    }
}