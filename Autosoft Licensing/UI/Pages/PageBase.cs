using System;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.UI.Pages
{
    /// <summary>
    /// Design-time safe base class for page UserControls.
    /// Ensures the WinForms designer can load derived controls without requiring runtime-only services.
    /// Keep this class minimal and avoid any static initialization or calls to ServiceRegistry in the constructor.
    /// </summary>
    [DesignerCategory("UserControl")]
    public class PageBase : XtraUserControl
    {
        public PageBase()
        {
            // Protect design-time: any code that might access runtime-only services should be avoided here.
            try
            {
                // When the designer instantiates this control, UsageMode will be Designtime.
                // Keep constructor minimal and side-effect free.
                if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                {
                    this.Dock = DockStyle.Fill;
                    return;
                }
            }
            catch
            {
                // swallow any design-time exception to keep the designer stable
                return;
            }

            // Normal runtime behaviour
            this.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Show a transient informational message to the user.
        /// Use PageBase.ShowInfo instead of calling XtraMessageBox directly from pages where possible.
        /// </summary>
        protected void ShowInfo(string message, string caption = "Info")
        {
            XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Show a transient error message to the user.
        /// </summary>
        protected void ShowError(string message, string caption = "Error")
        {
            XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Convert UTC to local DateTime for display.
        /// </summary>
        protected DateTime ToLocal(DateTime utc)
        {
            if (utc.Kind == DateTimeKind.Local) return utc;
            return DateTime.SpecifyKind(utc, DateTimeKind.Utc).ToLocalTime();
        }

        /// <summary>
        /// Convert local DateTime to UTC for storage.
        /// </summary>
        protected DateTime ToUtc(DateTime local)
        {
            if (local.Kind == DateTimeKind.Utc) return local;
            return DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();
        }

        /// <summary>
        /// Called by the host after the page is loaded/shown so the page can enable/disable controls by role.
        /// Default implementation is a no-op; override in derived pages as needed.
        /// </summary>
        public virtual void InitializeForRole(User user)
        {
            // no-op by default
        }
    }
}
