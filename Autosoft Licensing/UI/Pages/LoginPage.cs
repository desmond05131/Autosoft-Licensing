/*
PAGE: LoginPage.cs
ROLE: Dealer EXE user (Admin / Support)
PURPOSE:
  Authenticate dealer application users and, on success, load the main application UI (navigation + pages).
  This file is a skeleton: UI event handling and navigation stubs are implemented here; real authentication,
  dependency injection and user-context wiring must be integrated by the application composition root.

EXPECTED SERVICES / CALLS:
  - IUserService.ValidateCredentials(string username, string password)
  - ServiceRegistry.Database.GetUserByUsername(string username)  // to load full User object
  - CurrentUserContext.Set(User user) OR MainForm.SetLoggedInUser(user) to propagate authenticated user to UI
  - MainForm.NavigateToDefaultPage() to navigate to the primary landing page after successful login

ERROR STRINGS (exact):
  - Invalid credentials (local inline message): "Invalid username or password."
  - Internal failure (generic): "Login failed, contact admin."

NOTES / TODOs:
  - TODO: Inject IUserService (and any context/navigation service) via constructor or use a service locator.
  - TODO: Replace direct ServiceRegistry calls with your DI container resolution when wiring this page.
  - Do NOT implement password hashing here; call IUserService.ValidateCredentials which handles auth logic.
*/

using System;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.UI.Pages
{
    public partial class LoginPage : PageBase
    {
        private ILicenseDatabaseService _db;
        private IEncryptionService _crypto;

        // Raised when login succeeds; the MainForm should subscribe to transition to the app shell
        public event EventHandler<User> LoginSuccess;

        public LoginPage()
        {
            // Disable DevExpress default look-and-feel BEFORE control creation so skins cannot repaint over
            // the Appearance/BackColor values set in the designer or by our paint handlers.
            DevExpress.LookAndFeel.UserLookAndFeel.Default.UseDefaultLookAndFeel = false;
            DevExpress.LookAndFeel.UserLookAndFeel.Default.SkinName = string.Empty;

            InitializeComponent();

            // Initial state
            lblError.Visible = false;

            // Default username shown in wireframe
            if (txtUsername != null)
            {
                txtUsername.Text = "ADMIN";
            }

            // Wire events explicitly (designer wires Load and others)
            if (btnLogin != null)
                btnLogin.Click += btnLogin_Click;
        }

        // ACTION 1: Inject ILicenseDatabaseService and IEncryptionService via Initialize()
        public void Initialize(ILicenseDatabaseService db, IEncryptionService crypto)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _crypto = crypto ?? throw new ArgumentNullException(nameof(crypto));
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            var username = (txtUsername.Text ?? string.Empty).Trim();
            var password = (txtPassword.Text ?? string.Empty);

            // Local validation: show friendly inline messages for missing fields.
            if (string.IsNullOrEmpty(username))
            {
                lblError.Text = "Please enter username.";
                lblError.Visible = true;
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                lblError.Text = "Please enter password.";
                lblError.Visible = true;
                return;
            }

            try
            {
                if (_db == null || _crypto == null)
                {
                    lblError.Text = "Login failed, contact admin.";
                    lblError.Visible = true;
                    return;
                }

                // Fetch user and ensure it exists and is active
                var user = _db.GetUserByUsername(username);
                if (user == null || !user.IsActive)
                {
                    lblError.Text = "Invalid username or password.";
                    lblError.Visible = true;
                    return;
                }

                // Verify password using SHA256 hex of UTF8 password text
                var inputBytes = Encoding.UTF8.GetBytes(password);
                var inputHash = _crypto.ComputeSha256Hex(inputBytes);

                if (!string.Equals(inputHash, user.PasswordHash, StringComparison.OrdinalIgnoreCase))
                {
                    lblError.Text = "Invalid username or password.";
                    lblError.Visible = true;
                    return;
                }

                // Success: notify host shell
                try
                {
                    LoginSuccess?.Invoke(this, user);
                }
                catch
                {
                    // Surface a safe, generic message to the end user.
                    lblError.Text = "Login failed, contact admin.";
                    lblError.Visible = true;
                }
            }
            catch (Exception)
            {
                lblError.Text = "Login failed, contact admin.";
                lblError.Visible = true;
            }
        }

        public override void InitializeForRole(User user)
        {
            // Login page doesn't gate by role: ensure controls are usable and clear transient state.
            try
            {
                lblError.Visible = false;
                if (txtUsername != null) txtUsername.Enabled = true;
                if (txtPassword != null) txtPassword.Enabled = true;
                if (btnLogin != null) btnLogin.Enabled = true;
            }
            catch
            {
                // best-effort; do not throw from initialization
            }
        }
    }
}
