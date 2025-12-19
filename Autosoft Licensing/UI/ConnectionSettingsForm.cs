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
            builder.IntegratedSecurity = false; // We are forcing SQL Auth for Client-Server
            builder.ConnectTimeout = 5; // Fail fast during testing
            return builder.ConnectionString;
        }

        private void btnTestConnection_Click(object sender, EventArgs e)
        {
            try
            {
                using (var conn = new SqlConnection(BuildConnectionString()))
                {
                    conn.Open();
                    MessageBox.Show("Connection Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection Failed:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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