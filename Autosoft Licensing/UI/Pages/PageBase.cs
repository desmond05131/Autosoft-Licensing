using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.UI.Pages
{
    /// <summary>
    /// Base class for all page UserControls. Provides:
    /// - transient message helpers (ShowInfo, ShowError)
    /// - access to ServiceRegistry via Services property
    /// - UTC / Local conversion helpers
    /// - abstract InitializeForRole(User user) to enable/disable controls after load
    /// </summary>
    public abstract class PageBase : UserControl
    {
        protected PageBase()
        {
            this.Dock = DockStyle.Fill;
        }

        // Actually use the static ServiceRegistry class directly where needed:
        // e.g. ServiceRegistry.License, ServiceRegistry.Database, etc.

        /// <summary>
        /// Show a transient informational message to the user.
        /// </summary>
        protected void ShowInfo(string message, string caption = "Info")
        {
            XtraMessageBox.Show(message, caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
        }

        /// <summary>
        /// Show a transient error message to the user.
        /// </summary>
        protected void ShowError(string message, string caption = "Error")
        {
            XtraMessageBox.Show(message, caption, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
        /// </summary>
        public abstract void InitializeForRole(User user);
    }
}
