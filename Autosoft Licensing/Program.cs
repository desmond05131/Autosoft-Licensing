using System;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using Autosoft_Licensing.Services;
using Autosoft_Licensing.UI.Pages;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;

namespace Autosoft_Licensing
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ---------------------------------------------------------
            // 1. SELF-HEALING DATABASE LOGIC
            // ---------------------------------------------------------
            try
            {
                DbBootstrapper.EnsureDatabaseAndSchema();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Critical Error during Database Setup:\n" + ex.Message +
                    "\n\nPlease ensure SQL Server Express/LocalDB is installed and the SQL files are in the application folder.",
                    "Deployment Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Stop app if DB fails
            }

            // ---------------------------------------------------------
            // 2. NORMAL STARTUP
            // ---------------------------------------------------------
            ServiceRegistry.InitializeDatabase("LicensingDb");

            // Verify Admin exists (Double check)
            try
            {
                var admin = ServiceRegistry.Database.GetUserByUsername("admin");
                if (admin == null)
                {
                    MessageBox.Show("Database tables exist but 'admin' user is missing. Please check Seed.sql.", "Warning");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to query database after setup: " + ex.Message, "Startup Error");
                return;
            }

            // ... (Rest of your existing startup logic for styles/forms) ...

            // Launch Logic
            var launchPage = ConfigurationManager.AppSettings["LaunchPage"] ?? "Login";
            var launchAsAdmin = string.Equals(ConfigurationManager.AppSettings["LaunchAsAdmin"], "true", StringComparison.OrdinalIgnoreCase);

            var mainForm = new MainForm();
            if (launchAsAdmin)
            {
                try
                {
                    var admin = ServiceRegistry.Database.GetUserByUsername("admin");
                    if (admin != null) mainForm.SetLoggedInUser(admin);
                }
                catch { }
            }

            if (launchAsAdmin && !string.Equals(launchPage, "Login", StringComparison.OrdinalIgnoreCase))
            {
                mainForm.NavigateToPage(launchPage);
            }

            Application.Run(mainForm);
        }
    }

    /// <summary>
    /// Handles creation of Database, Tables, and Seed Data if missing.
    /// </summary>
    public static class DbBootstrapper
    {
        public static void EnsureDatabaseAndSchema()
        {
            var targetConnString = ConfigurationManager.ConnectionStrings["LicensingDb"].ConnectionString;
            var builder = new SqlConnectionStringBuilder(targetConnString);
            string targetDbName = builder.InitialCatalog;
            string serverConnString = targetConnString.Replace($"Initial Catalog={targetDbName}", "Initial Catalog=master");

            // 1. Create Database if it doesn't exist
            using (var conn = new SqlConnection(serverConnString))
            {
                conn.Open();
                var cmd = new SqlCommand($"SELECT database_id FROM sys.databases WHERE Name = '{targetDbName}'", conn);
                if (cmd.ExecuteScalar() == null)
                {
                    using (var createCmd = new SqlCommand($"CREATE DATABASE [{targetDbName}]", conn))
                    {
                        createCmd.ExecuteNonQuery();
                    }
                }
            }

            // 2. Connect to the App Database to check Tables & Seed
            using (var conn = new SqlConnection(targetConnString))
            {
                conn.Open();

                // Check if 'Users' table exists
                bool tablesExist = false;
                using (var checkCmd = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'Users'", conn))
                {
                    var count = (int)checkCmd.ExecuteScalar();
                    tablesExist = (count > 0);
                }

                if (!tablesExist)
                {
                    RunScript(conn, "Schema.sql");
                }

                // 3. Check if Seed data exists (e.g. Admin user)
                bool adminExists = false;
                if (tablesExist || true) // We just ran schema, so tables should exist now
                {
                    try
                    {
                        using (var userCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = 'admin'", conn))
                        {
                            var userCount = (int)userCmd.ExecuteScalar();
                            adminExists = (userCount > 0);
                        }
                    }
                    catch { /* If table creation failed, this fails safely */ }
                }

                if (!adminExists)
                {
                    RunScript(conn, "Seed.sql");
                }
            }
        }

        private static void RunScript(SqlConnection conn, string fileName)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Database", fileName);
            if (!File.Exists(path))
            {
                // Fallback to checking root directory
                path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
            }

            if (!File.Exists(path))
                throw new FileNotFoundException($"Could not find database script: {path}");

            string script = File.ReadAllText(path);

            // Split by "GO" statements (case insensitive, robust regex)
            var commands = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            foreach (var commandText in commands)
            {
                if (!string.IsNullOrWhiteSpace(commandText))
                {
                    using (var cmd = new SqlCommand(commandText, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}