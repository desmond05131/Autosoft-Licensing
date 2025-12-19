using System;
using System.Windows.Forms;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.UI; // Needed for MainForm and ConnectionSettingsForm
using Autosoft_Licensing.Data; // Needed for SqlConnectionFactory

namespace Autosoft_Licensing
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool connectionValid = false;

            // ---------------------------------------------------------
            // 1. DYNAMIC CONNECTION LOGIC
            // ---------------------------------------------------------
            while (!connectionValid)
            {
                try
                {
                    // Get connection string from Factory (checks Settings first, then App.config fallback)
                    string connString = Autosoft_Licensing.Data.SqlConnectionFactory.GetConnectionString();

                    // If no settings exist yet (and no valid fallback), force error to open settings form
                    if (string.IsNullOrWhiteSpace(connString) || string.IsNullOrWhiteSpace(Properties.Settings.Default.DbServer))
                        throw new Exception("No connection settings configured.");

                    // Try to connect (Test Connection) to ensure Server is reachable
                    using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                    {
                        conn.Open(); // Will throw exception if fails (e.g., Firewall, Wrong Password)
                    }

                    // If we get here, connection is good.
                    // Initialize ServiceRegistry.
                    ServiceRegistry.InitializeDatabase("LicensingDb");

                    connectionValid = true;
                }
                catch (Exception)
                {
                    // Connection failed or not configured. 
                    // Automatically open Settings Form so the user can fix it.
                    using (var settingsForm = new ConnectionSettingsForm())
                    {
                        if (settingsForm.ShowDialog() == DialogResult.OK)
                        {
                            // User saved new settings, loop back and try connecting again
                            continue;
                        }
                        else
                        {
                            // User cancelled/closed the app
                            return;
                        }
                    }
                }
            }

            // ---------------------------------------------------------
            // 2. LAUNCH APPLICATION
            // ---------------------------------------------------------
            var mainForm = new MainForm();

            // NOTE: The MainForm constructor automatically calls ShowLogin().
            // We do NOT call NavigateToPage("Login") here because "Login" is not a mapped page 
            // and would result in a blank GenericPage.

            Application.Run(mainForm);
        }
    }
}