using System;
using System.Windows.Forms;
using Autosoft_Licensing.Data;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.Utils;

namespace Autosoft_Licensing
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Accepts an optional "--smoke" argument to run the non-UI smoke test and exit.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // Composition root
            var connFactory = new SqlConnectionFactory("LicensingDb");
            ServiceRegistry.Database = new LicenseDatabaseService(connFactory);

            // --- Startup sanity checks ---

            // DB quick-check: verify we can query the Users table (admin)
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

            // Crypto quick-check: force reading and validation of configured key/IV
            try
            {
                var key = CryptoConstants.AesKey; // will throw if missing/invalid
                var iv = CryptoConstants.AesIV;   // will throw if missing/invalid

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

            // If invoked with --smoke run the smoke test harness (non-UI) and exit with result shown to user.
            if (args != null && Array.Exists(args, a => string.Equals(a, "--smoke", StringComparison.OrdinalIgnoreCase)))
            {
                var result = Tools.SmokeTestHarness.RunAll();
                var caption = "Smoke test result";
                MessageBoxIcon icon = result.Success ? MessageBoxIcon.Information : MessageBoxIcon.Error;
                var msg = result.Message;
                MessageBox.Show(msg, caption, MessageBoxButtons.OK, icon);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
