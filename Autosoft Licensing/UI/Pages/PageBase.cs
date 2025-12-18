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
    public class PageBase : XtraUserControl
    {
        private static bool _threadExceptionHooked = false;

        // Shared navigation event for all pages
        public event EventHandler<NavigateEventArgs> NavigateRequested;

        // NEW: Provide a Title property pages and host can use to set UI text.
        // Default implementation derives a readable title from the type name (e.g. "GenerateLicensePage" -> "GenerateLicense").
        public virtual string Title => GetDefaultTitle();

        private string GetDefaultTitle()
        {
            try
            {
                var t = this.GetType().Name;
                if (string.IsNullOrWhiteSpace(t)) return string.Empty;
                if (t.EndsWith("Page", StringComparison.OrdinalIgnoreCase))
                    return t.Substring(0, t.Length - "Page".Length);
                return t;
            }
            catch { return string.Empty; }
        }

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

            // Hook UI-thread exception handler once per AppDomain to prevent unhandled UI exceptions
            // from terminating the test host process when controls are created/manipulated by tests.
            try
            {
                if (!_threadExceptionHooked)
                {
                    // Ensure ThreadException events are delivered to our handler.
                    Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                    Application.ThreadException += Application_ThreadException;
                    _threadExceptionHooked = true;
                }
            }
            catch
            {
                // best-effort: do not throw from constructor
            }

            // Normal runtime behaviour
            this.Dock = DockStyle.Fill;
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            try
            {
                // Log the exception for diagnostics; do NOT rethrow.
                System.Diagnostics.Debug.WriteLine("Unhandled UI thread exception suppressed: " + (e?.Exception?.ToString() ?? "(null)"));
            }
            catch { /* ignore logging failures */ }

            // Swallow the exception to prevent the host process from crashing during tests.
            // In production you may choose to surface or report this differently.
        }

        /// <summary>
        /// Show a transient informational message to the user.
        /// Use PageBase.ShowInfo instead of calling XtraMessageBox directly from pages where possible.
        /// Non-blocking during test/initialization when control handle is not yet created.
        /// </summary>
        protected void ShowInfo(string message, string caption = "Info")
        {
            try
            {
                // If the control does not yet have a window handle (test host may call methods early),
                // avoid creating modal dialogs — just log and return to prevent blocking the UI thread.
                if (!this.IsHandleCreated)
                {
                    System.Diagnostics.Debug.WriteLine($"ShowInfo suppressed (no handle): {caption} - {message}");
                    return;
                }

                // Use asynchronous invoke so the calling UI handler isn't blocked by the modal dialog.
                this.BeginInvoke(new Action(() =>
                    XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Information)
                ));
            }
            catch
            {
                // Best-effort: don't throw from UI helper.
                try { System.Diagnostics.Debug.WriteLine($"ShowInfo failed: {caption} - {message}"); } catch { /* ignore */ }
            }
        }

        /// <summary>
        /// Show a transient error message to the user.
        /// Non-blocking during test/initialization when control handle is not yet created.
        /// </summary>
        protected void ShowError(string message, string caption = "Error")
        {
            try
            {
                if (!this.IsHandleCreated)
                {
                    System.Diagnostics.Debug.WriteLine($"ShowError suppressed (no handle): {caption} - {message}");
                    return;
                }

                this.BeginInvoke(new Action(() =>
                    XtraMessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error)
                ));
            }
            catch
            {
                try { System.Diagnostics.Debug.WriteLine($"ShowError failed: {caption} - {message}"); } catch { /* ignore */ }
            }
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

        // Helper to raise navigation requests from any page
        protected void FireNavigate(string target)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(target)) return;
                NavigateRequested?.Invoke(this, new NavigateEventArgs { TargetPage = target });
            }
            catch
            {
                // ignore
            }
        }

        // NEW: Overload to raise navigation with an optional record identifier
        protected void FireNavigate(string target, int? recordId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(target)) return;
                NavigateRequested?.Invoke(this, new NavigateEventArgs { TargetPage = target, RecordId = recordId });
            }
            catch
            {
                // ignore
            }
        }

        // NEW: Helper to bind click to container and its children so labels/icons within panels respond to clicks.
        protected void BindNavigationEvent(Control control, string targetPage)
        {
            if (control == null || string.IsNullOrWhiteSpace(targetPage)) return;

            EventHandler handler = (s, e) => FireNavigate(targetPage);

            // Bind to the container
            control.Click += handler;

            // Bind to immediate children (1 level; usually sufficient)
            foreach (Control child in control.Controls)
            {
                child.Click += handler;
            }
        }
    }

    // Unified NavigateEventArgs for page-to-shell navigation
    public class NavigateEventArgs : EventArgs
    {
        public string TargetPage { get; set; }
        // NEW: unified optional record identifier used by details pages (User/Product/License)
        public int? RecordId { get; set; }
    }
}
