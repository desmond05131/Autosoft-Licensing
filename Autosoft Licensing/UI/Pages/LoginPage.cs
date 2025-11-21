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
            InitializeComponent();

            // Initial state
            lblError.Visible = false;

            // Wire events (designer already sets btnLogin.Click to btnLogin_Click,
            // but keep this here to be explicit in the skeleton)
            btnLogin.Click += btnLogin_Click;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            lblError.Visible = false;

            var username = (txtUsername.Text ?? string.Empty).Trim();
            var password = (txtPassword.Text ?? string.Empty);

            // Local validation
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
                // TODO: Prefer constructor injection for IUserService instead of using ServiceRegistry directly.
                var userService = ServiceRegistry.User;

                // Call into the IUserService to validate credentials.
                // IUserService.ValidateCredentials should perform hashing checks and return bool.
                var ok = userService.ValidateCredentials(username, password);

                if (!ok)
                {
                    lblError.Text = "Invalid username or password.";
                    lblError.Visible = true;
                    return;
                }

                // On success, load user record from DB and set app-level context
                // TODO: Replace with DI/context management (e.g., CurrentUserContext.Set(user))
                var user = ServiceRegistry.Database.GetUserByUsername(username);

                // If this control is hosted inside MainForm, propagate the logged-in user and navigate.
                var parentForm = this.FindForm() as MainForm;
                if (parentForm != null)
                {
                    try
                    {
                        parentForm.SetLoggedInUser(user);
                        // TODO: Replace with DI-based navigation call when available
                        parentForm.NavigateToDefaultPage();
                    }
                    catch
                    {
                        // swallow navigation exceptions and show friendly message
                        lblError.Text = "Login failed, contact admin.";
                        lblError.Visible = true;
                    }
                }
                else
                {
                    // Host not found; surface info and leave TODO for host integration.
                    lblError.Text = "Login succeeded but application host not found.";
                    lblError.Visible = true;
                }
            }
            catch (Exception)
            {
                // Do not reveal internal details to the user.
                lblError.Text = "Login failed, contact admin.";
                lblError.Visible = true;

                // TODO: Consider logging exception details to an application log (not shown in UI).
            }
        }
    }
}
