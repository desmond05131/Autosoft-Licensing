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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing.UI.Pages
{
    public partial class LoginPage : PageBase
    {
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
                // TODO: Replace ServiceRegistry usage with constructor-injected IUserService.
                var userService = ServiceRegistry.User;

                // Call into IUserService. This method is expected to perform proper hashing, salting, etc.
                var authOk = userService.ValidateCredentials(username, password);

                if (!authOk)
                {
                    // Exact string required by UI guide for invalid credentials (local inline).
                    lblError.Text = "Invalid username or password.";
                    lblError.Visible = true;
                    return;
                }

                // Load full user record and propagate to host (main form / context).
                var user = ServiceRegistry.Database.GetUserByUsername(username);
                if (user == null)
                {
                    // Unexpected: user validated but record not found. Surface generic failure.
                    lblError.Text = "Login failed, contact admin.";
                    lblError.Visible = true;
                    return;
                }

                // Propagate authenticated user to the host MainForm and navigate to default page.
                var parentForm = this.FindForm() as MainForm;
                if (parentForm != null)
                {
                    try
                    {
                        parentForm.SetLoggedInUser(user);

                        // Use the MainForm.NavigateToDefaultPage implemented in MainForm.cs
                        parentForm.NavigateToDefaultPage();
                    }
                    catch
                    {
                        // Surface a safe, generic message to the end user.
                        lblError.Text = "Login failed, contact admin.";
                        lblError.Visible = true;
                    }
                }
                else
                {
                    // Host not found (e.g., when hosted differently). Show friendly info and leave TODO.
                    lblError.Text = "Login succeeded but application host not found.";
                    lblError.Visible = true;
                }
            }
            catch (Exception)
            {
                // Do not reveal internal details; show a safe, support-directed message.
                lblError.Text = "Login failed, contact admin.";
                lblError.Visible = true;

                // TODO: log exception details using application logging (not shown in UI).
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
