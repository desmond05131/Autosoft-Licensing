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

                    // 1. DIAGNOSTIC POPUP (Add this)
                    // This will reveal the "Real" connection string the app is using.
                    var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connString);
                    MessageBox.Show($"Debug Info:\n\nTarget Server: {builder.DataSource}\nDatabase: {builder.InitialCatalog}\nUser: {builder.UserID}", 
                                    "Verifying Connection Target");

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
                    // Note: Ensure ServiceRegistry or LicenseDatabaseService is updated to use SqlConnectionFactory.GetConnectionString()
                    // or allows the connection string to be injected implicitly.
                    ServiceRegistry.InitializeDatabase("LicensingDb");

                    connectionValid = true;
                }
                catch (Exception ex)
                {
                    // Provide SQL-specific diagnostic info when available so the user (or support) can identify the failure reason.
                    string extraInfo = "";
                    if (ex is System.Data.SqlClient.SqlException sqlEx)
                    {
                        extraInfo = $"\n\nSQL Error Number: {sqlEx.Number}\nSQL State: {sqlEx.State}";

                        // Helpful states (non-exhaustive)
                        // State 18 = Password expired
                        // State 38 = Database not found / Valid login but wrong DB
                        // State 5  = Invalid user id / password
                    }

                    // Show detailed error to assist troubleshooting before opening Settings
                    MessageBox.Show($"Connection Failed:\n{ex.Message}{extraInfo}", "Diagnostic Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Connection failed or not configured. Show Settings Form.
                    // This requires you to have created ConnectionSettingsForm.cs in UI folder
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
            // We removed DbBootstrapper.EnsureDatabaseAndSchema() because
            // clients connect to an existing Central Database.

            var mainForm = new MainForm();

            // Force navigation to Login page for security in Client-Server mode
            mainForm.NavigateToPage("Login");

            Application.Run(mainForm);
        }
    }
}