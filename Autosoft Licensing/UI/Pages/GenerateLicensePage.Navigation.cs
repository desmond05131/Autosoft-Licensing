using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;

namespace Autosoft_Licensing.UI.Pages
{
    partial class GenerateLicensePage
    {
        // Navigation controls created at runtime to avoid designer issues
        private BarManager _barManager;
        private Bar _barTopNav;
        private BarButtonItem _barBtnGenerate;
        private BarButtonItem _barBtnRecords;
        private BarButtonItem _barBtnProduct;
        private BarButtonItem _barBtnUser;

        /// <summary>
        /// Initialize a lightweight DevExpress Bar top-navigation with icons.
        /// Run only at runtime (not design-time).
        /// </summary>
        private void InitializeNavigation()
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            try
            {
                _barManager = new BarManager();
                _barTopNav = new Bar(_barManager, "TopNavigation");
                _barTopNav.DockStyle = BarDockStyle.Top;

                _barBtnGenerate = new BarButtonItem(_barManager, "Generate License");
                _barBtnRecords = new BarButtonItem(_barManager, "License Records");
                _barBtnProduct = new BarButtonItem(_barManager, "Manage Product");
                _barBtnUser = new BarButtonItem(_barManager, "Manage User");

                // Load icons from Assets folder if present. Use try/catch to avoid breaking if files missing.
                try
                {
                    _barBtnGenerate.ImageOptions.Image = Image.FromFile("Assets/generate.png");
                }
                catch { /* ignore - icon optional at runtime */ }
                try
                {
                    _barBtnRecords.ImageOptions.Image = Image.FromFile("Assets/records.png");
                }
                catch { }
                try
                {
                    _barBtnProduct.ImageOptions.Image = Image.FromFile("Assets/product.png");
                }
                catch { }
                try
                {
                    _barBtnUser.ImageOptions.Image = Image.FromFile("Assets/user.png");
                }
                catch { }

                // Add items to bar
                _barTopNav.AddItem(_barBtnGenerate);
                _barTopNav.AddItem(_barBtnRecords);
                _barTopNav.AddItem(_barBtnProduct);
                _barTopNav.AddItem(_barBtnUser);

                // Add bar manager's dock controls to this UserControl to ensure it displays
                var dockTop = new DevExpress.XtraBars.BarDockControl();
                var dockBottom = new DevExpress.XtraBars.BarDockControl();
                var dockLeft = new DevExpress.XtraBars.BarDockControl();
                var dockRight = new DevExpress.XtraBars.BarDockControl();

                dockTop.Manager = _barManager;
                dockBottom.Manager = _barManager;
                dockLeft.Manager = _barManager;
                dockRight.Manager = _barManager;

                dockTop.Dock = System.Windows.Forms.DockStyle.Top;
                dockBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
                dockLeft.Dock = System.Windows.Forms.DockStyle.Left;
                dockRight.Dock = System.Windows.Forms.DockStyle.Right;

                _barManager.Bars.Add(_barTopNav);
                _barManager.Form = this.FindForm(); // best-effort; if null, bar still works visually within control

                // Add dock controls to this control so the Bar can be visible
                this.Controls.Add(dockTop);
                this.Controls.Add(dockBottom);
                this.Controls.Add(dockLeft);
                this.Controls.Add(dockRight);

                // Visual: mark Generate as active by changing its paint style (simple approach)
                _barBtnGenerate.ItemAppearance.Normal.BackColor = Color.FromArgb(255, 243, 217);
                _barBtnGenerate.ItemAppearance.Normal.Options.UseBackColor = true;

                // Wire click handlers to mimic original simple top buttons (optional)
                _barBtnGenerate.ItemClick += (s, e) => { /* already on page; visual only */ };
                _barBtnRecords.ItemClick += (s, e) => { /* host should navigate; keep no-op here */ };
                _barBtnProduct.ItemClick += (s, e) => { /* host should navigate */ };
                _barBtnUser.ItemClick += (s, e) => { /* host should navigate */ };
            }
            catch
            {
                // If navigation build fails, fail silently and keep original simple buttons present in designer.
            }
        }
    }
}
