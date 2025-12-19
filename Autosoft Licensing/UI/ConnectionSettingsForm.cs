using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using Autosoft_Licensing.Properties; // Ensure this matches your namespace

namespace Autosoft_Licensing.UI
{
    public partial class ConnectionSettingsForm : Form
    {
        public ConnectionSettingsForm()
        {
            InitializeComponent();
            LoadCurrentSettings();
        }

        private void LoadCurrentSettings()
        {
            txtServer.Text = Settings.Default.DbServer;
            txtDatabase.Text = Settings.Default.DbName; // Default is "AutosoftLicensing"
            txtUsername.Text = Settings.Default.DbUser;
            // For security, you might not want to autofill the password, but for UX:
            txtPassword.Text = Settings.Default.DbPassword;
        }

        private string BuildConnectionString()
        {
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = txtServer.Text.Trim();
            builder.InitialCatalog = txtDatabase.Text.Trim();
            builder.UserID = txtUsername.Text.Trim();
            builder.Password = txtPassword.Text.Trim();

            // CRITICAL SETTINGS FOR REMOTE CONNECTIONS
            builder.IntegratedSecurity = false;
            builder.ConnectTimeout = 5;

            // Bypass SSL Certificate validation (Self-signed certs)
            builder.TrustServerCertificate = true;
            builder.Encrypt = false; // Optional: Try false if true fails

            return builder.ConnectionString;
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                string connStr = BuildConnectionString();

                // --- ADD THIS DEBUG BLOCK ---
                var builder = new SqlConnectionStringBuilder(connStr);
                DialogResult result = MessageBox.Show(
                    $"Attempting to connect to:\n\nServer: {builder.DataSource}\nDatabase: {builder.InitialCatalog}\nUser: {builder.UserID}\n\nIs this the correct Remote IP?",
                    "Verify Connection Details",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.No) return;
                // -----------------------------

                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    MessageBox.Show("Connection Successful!", "Success");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection Failed:\n{ex.Message}", "Error");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtServer.Text) || string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please fill in Server and Username fields.");
                return;
            }

            try
            {
                // Verify before saving
                using (var conn = new SqlConnection(BuildConnectionString()))
                {
                    conn.Open();
                }

                // Save to User Settings
                Settings.Default.DbServer = txtServer.Text.Trim();
                Settings.Default.DbName = txtDatabase.Text.Trim();
                Settings.Default.DbUser = txtUsername.Text.Trim();
                Settings.Default.DbPassword = txtPassword.Text.Trim(); // Note: In high security apps, encrypt this.
                Settings.Default.Save();

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot connect with these settings:\n{ex.Message}", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}