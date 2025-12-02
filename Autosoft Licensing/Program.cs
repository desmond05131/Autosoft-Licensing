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
        /// <summary>
        /// The main entry point for the application.
        /// Accepts:
        /// - "--smoke" : non-UI smoke harness
        /// - "--smoke-ui" : programmatic non-visual UI smoke (existing)
        /// - "--smoke-ui-visual" : show the MainForm so you can manually inspect and click pages
        /// - "--smoke-ui-demo" : show MainForm and automatically step through pages (visual demo)
        ///   add "--smoke-ui-demo-admin" to run demo with admin user set (shows admin items)
        /// - "--show-login" : show only the LoginPage in a small host so you can verify runtime colors
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

            // If running the visual test, ensure global DevExpress look-and-feel is disabled so
            // per-control BackColor values render exactly as set.
            if (args != null && args.Any(a => string.Equals(a, "--show-login", StringComparison.OrdinalIgnoreCase)))
            {
                UserLookAndFeel.Default.UseDefaultLookAndFeel = false;
                UserLookAndFeel.Default.SkinName = string.Empty;

                Application.Run(CreateLoginTestForm());
                return;
            }

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

#if DEBUG
            // Debug convenience: when launching the EXE from Visual Studio in Debug without args
            // open a lightweight form hosting the GenerateLicensePage so you can test it directly.
            // This block runs only in DEBUG builds and only when no command-line args are present.
            if (args == null || args.Length == 0)
            {
                try
                {
                    var host = new XtraForm
                    {
                        Text = "Generate License Visual Test",
                        StartPosition = FormStartPosition.CenterScreen,
                        Width = 1100,
                        Height = 820
                    };

                    // Prefer explicit look/feel disabling to match other visual test behavior
                    host.LookAndFeel.UseDefaultLookAndFeel = false;
                    host.BackColor = System.Drawing.Color.White;

                    var page = new GenerateLicensePage();
                    page.Dock = DockStyle.Fill;

                    // Best-effort: inject runtime services from ServiceRegistry so page behaves normally in the test host.
                    try
                    {
                        page.Initialize(
                            ServiceRegistry.ArlReader,
                            ServiceRegistry.AslGenerator,
                            ServiceRegistry.Product,
                            ServiceRegistry.Database,
                            ServiceRegistry.User);
                    }
                    catch
                    {
                        // ignore injection errors; page ctor already tries to wire defaults
                    }

                    host.Controls.Add(page);
                    Application.Run(host);
                    return;
                }
                catch
                {
                    // fall back to normal interactive run if anything goes wrong here
                }
            }
#endif

            // Normal interactive run
            Application.Run(new MainForm());
        }

        /// <summary>
        /// Build a small DevExpress XtraForm that hosts the LoginPage control for visual testing.
        /// This method is only used by the "--show-login" test flag.
        /// </summary>
        private static XtraForm CreateLoginTestForm()
        {
            var form = new XtraForm
            {
                Text = "Login Visual Test",
                StartPosition = FormStartPosition.CenterScreen,
                Width = 1000,
                Height = 760
            };

            // Create the login page and add to form
            var login = new LoginPage();
            login.Dock = DockStyle.Fill;

            // Ensure the form has white background similar to wireframe
            form.LookAndFeel.UseDefaultLookAndFeel = false;
            form.BackColor = System.Drawing.Color.White;

            form.Controls.Add(login);

            return form;
        }
    }
}
