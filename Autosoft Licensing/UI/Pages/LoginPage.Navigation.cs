using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraEditors;

namespace Autosoft_Licensing.UI.Pages
{
    // Runtime-only logic for LoginPage. Kept small so designer can load safely.
    partial class LoginPage
    {
        private void LoginPage_Load(object sender, EventArgs e)
        {
            // Skip runtime-only initialization when hosted by the WinForms designer
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            try
            {
                // Ensure DevExpress global skin won't force override for these test runs.
                DevExpress.LookAndFeel.UserLookAndFeel.Default.UseDefaultLookAndFeel = false;
                DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = string.Empty;

                // Enforce colors and attach paint handlers that draw the exact colors
                // (DevExpress skins sometimes paint over BackColor; custom paint guarantees color).
                if (this.topStrip != null)
                {
                    this.topStrip.LookAndFeel.UseDefaultLookAndFeel = false;
                    this.topStrip.Paint -= TopStrip_Paint;
                    this.topStrip.Paint += TopStrip_Paint;
                    this.topStrip.Refresh();
                }

                if (this.topBanner != null)
                {
                    this.topBanner.LookAndFeel.UseDefaultLookAndFeel = false;
                    this.topBanner.Paint -= TopBanner_Paint;
                    this.topBanner.Paint += TopBanner_Paint;
                    this.topBanner.Refresh();
                }

                if (this.panelCenter != null)
                {
                    this.panelCenter.LookAndFeel.UseDefaultLookAndFeel = false;
                    this.panelCenter.Paint -= PanelCenter_Paint;
                    this.panelCenter.Paint += PanelCenter_Paint;
                    this.panelCenter.Refresh();
                }

                if (this.btnLogin != null)
                {
                    this.btnLogin.LookAndFeel.UseDefaultLookAndFeel = false;
                    this.btnLogin.Appearance.BackColor = Color.FromArgb(158, 173, 186);
                    this.btnLogin.Appearance.Options.UseBackColor = true;
                    this.btnLogin.Refresh();
                }

                // Re-center panel for the actual host size (keeps wireframe centering)
                if (this.panelCenter != null)
                {
                    var parentWidth = this.ClientSize.Width;
                    var parentHeight = this.ClientSize.Height;
                    this.panelCenter.Left = (parentWidth - this.panelCenter.Width) / 2;
                    this.panelCenter.Top = Math.Max(40, (parentHeight - this.panelCenter.Height) / 2);
                }
            }
            catch
            {
                // Best-effort; do not throw from UI init.
            }
        }

        // Paint handlers draw exact colors regardless of DevExpress skinning.
        private void TopStrip_Paint(object sender, PaintEventArgs e)
        {
            // thin bluish-gray strip - wireframe color #CADCE6 (202,220,230)
            using (var b = new SolidBrush(Color.FromArgb(202, 220, 230)))
            {
                e.Graphics.FillRectangle(b, ((Control)sender).ClientRectangle);
            }
        }

        private void TopBanner_Paint(object sender, PaintEventArgs e)
        {
            // pale yellow banner - wireframe color #FFF3D9 (255,243,217)
            using (var b = new SolidBrush(Color.FromArgb(255, 243, 217)))
            {
                e.Graphics.FillRectangle(b, ((Control)sender).ClientRectangle);
            }
        }

        private void PanelCenter_Paint(object sender, PaintEventArgs e)
        {
            // pure white inner panel
            using (var b = new SolidBrush(Color.White))
            {
                e.Graphics.FillRectangle(b, ((Control)sender).ClientRectangle);
            }

            // draw subtle border similar to BorderStyles.Simple, using a light gray stroke
            var rect = ((Control)sender).ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;
            using (var p = new Pen(Color.FromArgb(200, 200, 200)))
            {
                e.Graphics.DrawRectangle(p, rect);
            }
        }
    }
}
