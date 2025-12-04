using System;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using Autosoft_Licensing.Data;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Utils;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel;
using Autosoft_Licensing.UI.Pages;
using System.Diagnostics;

namespace Autosoft_Licensing
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Composition root
            ServiceRegistry.InitializeDatabase("LicensingDb");

            try
            {
                ServiceRegistry.Database.GetUserByUsername("admin");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Failed to initialize or query the licensing database.\n\n" +
                    "Details: " + ex.Message + "\n\n" +
                    "Please check the connection string named 'LicensingDb' in App.config and ensure the database is reachable.",
                    "Startup error - Database",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            // Crypto quick-check
            try
            {
                var key = CryptoConstants.AesKey;
                var iv = CryptoConstants.AesIV;

                if (key == null || key.Length == 0 || iv == null || iv.Length == 0)
                {
                    MessageBox.Show(
                        "Cryptographic configuration appears empty. Check appSettings 'Crypto:AesKey' and 'Crypto:AesIV' in App.config.",
                        "Startup error - Crypto",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Invalid cryptographic configuration detected.\n\n" +
                    "Details: " + ex.Message + "\n\n" +
                    "Ensure appSettings 'Crypto:AesKey' and 'Crypto:AesIV' are present and Base64-encoded (32 bytes key, 16 bytes IV).",
                    "Startup error - Crypto",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // If running the visual login test, disable global look-and-feel and host LoginPage
            if (args != null && args.Any(a => string.Equals(a, "--show-login", StringComparison.OrdinalIgnoreCase)))
            {
                UserLookAndFeel.Default.UseDefaultLookAndFeel = false;
                UserLookAndFeel.Default.SkinName = string.Empty;

                Application.Run(CreateLoginTestForm());
                return;
            }

            // Visual smoke: open the real MainForm for manual inspection/clicks
            if (args != null && args.Any(a => string.Equals(a, "--smoke-ui-visual", StringComparison.OrdinalIgnoreCase)))
            {
                var mf = new MainForm();

                // If the caller requested admin visibility for demo, set logged-in admin before showing.
                if (args.Any(a => string.Equals(a, "--smoke-ui-visual-admin", StringComparison.OrdinalIgnoreCase)
                               || string.Equals(a, "--smoke-ui-demo-admin", StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        var admin = ServiceRegistry.Database.GetUserByUsername("admin");
                        if (admin != null) mf.SetLoggedInUser(admin);
                    }
                    catch { /* ignore */ }
                }

                Application.Run(mf);
                return;
            }

            // Demo smoke: automatically step through each navigation element so you can watch pages render
            if (args != null && args.Any(a => string.Equals(a, "--smoke-ui-demo", StringComparison.OrdinalIgnoreCase)))
            {
                var mf = new MainForm();

                if (args.Any(a => string.Equals(a, "--smoke-ui-demo-admin", StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        var admin = ServiceRegistry.Database.GetUserByUsername("admin");
                        if (admin != null) mf.SetLoggedInUser(admin);
                    }
                    catch { }
                }

                mf.Shown += (s, e) =>
                {
                    var elements = mf.GetNavigationElements().ToArray();
                    int idx = 0;
                    var timer = new Timer { Interval = 800 };
                    timer.Tick += (ts, te) =>
                    {
                        if (idx >= elements.Length)
                        {
                            timer.Stop();
                            return;
                        }

                        var name = elements[idx].Name;
                        mf.NavigateToElement(name);
                        idx++;
                    };
                    timer.Start();
                };

                Application.Run(mf);
                return;
            }

            // Existing programmatic non-visual UI smoke
            if (args != null && args.Any(a => string.Equals(a, "--smoke-ui", StringComparison.OrdinalIgnoreCase)))
            {
                var mf = new MainForm();
                var result = mf.RunUiSmokeTest();
                var caption = "UI Smoke test result";
                MessageBoxIcon icon = result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error;
                MessageBox.Show(result.Message, caption, MessageBoxButtons.OK, icon);
                return;
            }

            // Non-UI smoke harness
            if (args != null && args.Any(a => string.Equals(a, "--smoke", StringComparison.OrdinalIgnoreCase)))
            {
                var result = Tools.SmokeTestHarness.RunAll();
                var caption = "Smoke test result";
                MessageBoxIcon icon = result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error;
                var msg = result.Message;
                MessageBox.Show(msg, caption, MessageBoxButtons.OK, icon);
                return;
            }

            // NORMAL INTERACTIVE RUN (args empty): honor App.config appSettings
            // Use LaunchPage to pick initial page and LaunchAsAdmin to reveal admin-only UI.
            var launchPage = ConfigurationManager.AppSettings["LaunchPage"] ?? "LicenseRecords";
            var launchAsAdmin = string.Equals(ConfigurationManager.AppSettings["LaunchAsAdmin"], "true", StringComparison.OrdinalIgnoreCase);

            var mainForm = new MainForm();

            if (launchAsAdmin)
            {
                try
                {
                    var admin = ServiceRegistry.Database.GetUserByUsername("admin");
                    if (admin != null) mainForm.SetLoggedInUser(admin);
                }
                catch { /* continue without admin if lookup fails */ }
            }

            // Navigate to the requested initial page
            switch (launchPage.Trim())
            {
                case "GenerateLicense":
                case "GenerateLicensePage":
                case "aceGenerateRequest":
                case "aceGenerateLicense":
                case "btnNav_GenerateLicense":
                    mainForm.NavigateToElement("GenerateLicensePage");
                    break;

                default:
                    // Default to License Records
                    mainForm.NavigateToElement("LicenseRecordsPage");
                    break;
            }

            Application.Run(mainForm);
        }

        private static XtraForm CreateLoginTestForm()
        {
            var form = new XtraForm
            {
                Text = "Login Visual Test",
                StartPosition = FormStartPosition.CenterScreen,
                Width = 1000,
                Height = 760
            };

            var login = new LoginPage();
            login.Dock = DockStyle.Fill;

            form.LookAndFeel.UseDefaultLookAndFeel = false;
            form.BackColor = System.Drawing.Color.White;

            form.Controls.Add(login);

            return form;
        }
    }
}
