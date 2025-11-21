using System;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using Autosoft_Licensing.Data;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Accepts:
        /// - "--smoke" : non-UI smoke harness
        /// - "--smoke-ui" : programmatic non-visual UI smoke (existing)
        /// - "--smoke-ui-visual" : show the MainForm so you can manually inspect and click pages
        /// - "--smoke-ui-demo" : show MainForm and automatically step through pages (visual demo)
        ///   add "--smoke-ui-demo-admin" to run demo with admin user set (shows admin items)
        /// </summary>
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

            // Visual smoke: open the real MainForm so user can manually click navigation
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
                    catch
                    {
                        // ignore: visual smoke should still show even if admin read failed
                    }
                }

                // Show form and allow manual inspection/clicks. User closes when done.
                Application.Run(mf);
                return;
            }

            // Demo smoke: automatically step through each navigation element so you can watch pages render
            if (args != null && args.Any(a => string.Equals(a, "--smoke-ui-demo", StringComparison.OrdinalIgnoreCase)))
            {
                var mf = new MainForm();

                // Optionally set admin user to reveal admin items during demo
                if (args.Any(a => string.Equals(a, "--smoke-ui-demo-admin", StringComparison.OrdinalIgnoreCase)))
                {
                    try
                    {
                        var admin = ServiceRegistry.Database.GetUserByUsername("admin");
                        if (admin != null) mf.SetLoggedInUser(admin);
                    }
                    catch { }
                }

                // Use a timer to step through elements on the UI thread
                mf.Shown += (s, e) =>
                {
                    var elements = mf.GetNavigationElements().ToArray();
                    int idx = 0;
                    var timer = new Timer { Interval = 800 }; // ms
                    timer.Tick += (ts, te) =>
                    {
                        if (idx >= elements.Length)
                        {
                            timer.Stop();
                            // Keep window open after demo so user can inspect; close automatically if you prefer:
                            // mf.Close();
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

            // Normal interactive run
            Application.Run(new MainForm());
        }
    }
}
