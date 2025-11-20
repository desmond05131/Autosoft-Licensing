using System;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;

namespace Autosoft_Licensing.UI.Pages
{
    /// <summary>
    /// Lightweight placeholder page used by the navigation shell.
    /// Replace with real pages that inherit PageBase.
    /// </summary>
    public class GenericPage : PageBase
    {
        private readonly LabelControl _label;

        public string Title { get; }

        public GenericPage(string title)
        {
            Title = title ?? "Page";
            _label = new LabelControl
            {
                Text = Title,
                Dock = System.Windows.Forms.DockStyle.Fill,
                Appearance = { Font = new System.Drawing.Font("Segoe UI", 14F) },
                AutoSizeMode = LabelAutoSizeMode.Vertical
            };
            this.Controls.Add(_label);
        }

        public override void InitializeForRole(User user)
        {
            // Default placeholder behaviour: do nothing.
            // Real pages can enable/disable controls based on user.Role here.
            if (user == null)
            {
                // Example: show limited info when no user is set
                _label.Text = Title + " (no user)";
            }
            else
            {
                _label.Text = $"{Title} (role: {user.Role})";
            }
        }
    }
}
