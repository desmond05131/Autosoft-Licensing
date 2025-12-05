using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.Services;
using System.Text;

namespace Autosoft_Licensing.UI.Pages
{
    public partial class UserDetailsPage : PageBase
    {
        private ILicenseDatabaseService _db;
        private int? _userId;
        private User _loadedUser;

        // Navigation back event - host can subscribe (same pattern as ProductDetailsPage)
        public event EventHandler<NavigateBackEventArgs> NavigateBackRequested;

        public class NavigateBackEventArgs : EventArgs
        {
            public bool Saved { get; set; }
            public int? UserId { get; set; }
        }

        public UserDetailsPage()
        {
            InitializeComponent();

            try
            {
                if (!DesignMode)
                {
                    try { _db ??= ServiceRegistry.Database; } catch { }

                    btnSave.Click += btnSave_Click;
                    btnCancel.Click += btnCancel_Click;

                    // Styling consistent with other pages
                    pnlHeader.BackColor = Color.FromArgb(253, 243, 211);
                    this.BackColor = Color.White;

                    Color purple = Color.FromArgb(98, 75, 255);
                    foreach (var b in new[] { btnSave, btnCancel })
                    {
                        b.Appearance.BackColor = purple;
                        b.Appearance.ForeColor = Color.White;
                        b.Appearance.Options.UseBackColor = true;
                        b.Appearance.Options.UseForeColor = true;
                    }

                    foreach (var te in new[] { txtUsername, txtPassword, txtEmail, txtDisplayName, txtRole })
                    {
                        try
                        {
                            te.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
                        }
                        catch { /* ignore */ }
                    }

                    // default create mode if host forgets to call
                    Initialize(null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserDetailsPage ctor error: {ex}");
            }
        }

        public void Initialize(int? userId, ILicenseDatabaseService db = null)
        {
            try
            {
                _db = db ?? _db ?? ServiceRegistry.Database;
            }
            catch { /* ignore */ }

            _userId = userId;
            _loadedUser = null;

            // Reset UI
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtDisplayName.Text = string.Empty;
            txtRole.Text = string.Empty;

            chkIsActive.Checked = true; // default active
            chkGenerate.Checked = false;
            chkRecords.Checked = false;
            chkProduct.Checked = false;
            chkUsers.Checked = false;

            EnableAllInputs(true);

            if (_userId == null)
            {
                lblHeader.Text = "Create User";
            }
            else
            {
                lblHeader.Text = "Edit User";
                LoadUser(_userId.Value);
            }
        }

        public override void InitializeForRole(User user)
        {
            // Future role-aware enable/disable if needed.
        }

        private void LoadUser(int id)
        {
            try
            {
                if (_db == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                var u = _db.GetUserById(id);
                if (u == null)
                {
                    ShowError("User not found.");
                    return;
                }

                _loadedUser = u;

                txtUsername.Text = u.Username ?? string.Empty;
                txtDisplayName.Text = u.DisplayName ?? string.Empty;
                txtRole.Text = u.Role ?? string.Empty;
                txtEmail.Text = u.Email ?? string.Empty;
                txtPassword.Text = string.Empty; // do not show existing password

                chkIsActive.Checked = u.IsActive;
                chkGenerate.Checked = u.CanGenerateLicense;
                chkRecords.Checked = u.CanViewRecords;
                chkProduct.Checked = u.CanManageProduct;
                chkUsers.Checked = u.CanManageUsers;

                if (!string.IsNullOrWhiteSpace(u.Username) &&
                    u.Username.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    chkIsActive.Checked = true;

                    chkGenerate.Checked = true;
                    chkRecords.Checked = true;
                    chkProduct.Checked = true;
                    chkUsers.Checked = true;

                    txtUsername.Properties.ReadOnly = true;
                    chkIsActive.Enabled = false;
                    chkGenerate.Enabled = false;
                    chkRecords.Enabled = false;
                    chkProduct.Enabled = false;
                    chkUsers.Enabled = false;
                }
                else
                {
                    EnableAllInputs(true);
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to load user details.");
                System.Diagnostics.Debug.WriteLine($"UserDetailsPage.LoadUser error: {ex}");
            }
        }

        private void EnableAllInputs(bool enabled)
        {
            try
            {
                txtUsername.Properties.ReadOnly = !enabled;
                txtPassword.Properties.ReadOnly = !enabled;
                txtDisplayName.Properties.ReadOnly = !enabled;
                txtRole.Properties.ReadOnly = !enabled;
                txtEmail.Properties.ReadOnly = !enabled;

                chkIsActive.Enabled = enabled;
                chkGenerate.Enabled = enabled;
                chkRecords.Enabled = enabled;
                chkProduct.Enabled = enabled;
                chkUsers.Enabled = enabled;
            }
            catch { /* ignore */ }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (_db == null)
                {
                    ShowError("Database service not initialized.");
                    return;
                }

                var username = (txtUsername.Text ?? string.Empty).Trim();
                var displayName = (txtDisplayName.Text ?? string.Empty).Trim();
                var role = (txtRole.Text ?? string.Empty).Trim(); // "Admin" or "Support"
                var email = (txtEmail.Text ?? string.Empty).Trim();
                var password = txtPassword.Text ?? string.Empty;

                if (string.IsNullOrWhiteSpace(username))
                {
                    ShowError("Username is required.");
                    return;
                }

                string passwordHashToPersist = null;
                bool isCreate = _userId == null;

                if (isCreate)
                {
                    if (string.IsNullOrEmpty(password))
                    {
                        ShowError("Password is required for new users.");
                        return;
                    }
                    try
                    {
                        var bytes = Encoding.UTF8.GetBytes(password);
                        passwordHashToPersist = ServiceRegistry.Encryption?.ComputeSha256Hex(bytes);
                    }
                    catch
                    {
                        ShowError("Failed to hash password.");
                        return;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        try
                        {
                            var bytes = Encoding.UTF8.GetBytes(password);
                            passwordHashToPersist = ServiceRegistry.Encryption?.ComputeSha256Hex(bytes);
                        }
                        catch
                        {
                            ShowError("Failed to hash password.");
                            return;
                        }
                    }
                }

                bool isAdminUserName = username.Equals("admin", StringComparison.OrdinalIgnoreCase);

                var user = new User
                {
                    Id = _userId ?? 0,
                    Username = username,
                    DisplayName = string.IsNullOrWhiteSpace(displayName) ? username : displayName,
                    Role = string.IsNullOrWhiteSpace(role) ? "Support" : role,
                    Email = email,
                    PasswordHash = isCreate ? (passwordHashToPersist ?? string.Empty)
                                            : (_loadedUser?.PasswordHash ?? string.Empty),
                    CreatedUtc = isCreate ? DateTime.UtcNow : (_loadedUser?.CreatedUtc ?? DateTime.UtcNow),

                    IsActive = isAdminUserName ? true : chkIsActive.Checked,
                    CanGenerateLicense = isAdminUserName ? true : chkGenerate.Checked,
                    CanViewRecords = isAdminUserName ? true : chkRecords.Checked,
                    CanManageProduct = isAdminUserName ? true : chkProduct.Checked,
                    CanManageUsers = isAdminUserName ? true : chkUsers.Checked
                };

                if (!isCreate && !string.IsNullOrEmpty(passwordHashToPersist))
                {
                    user.PasswordHash = passwordHashToPersist;
                }

                if (isCreate)
                {
                    var newId = _db.InsertUser(user);
                    _userId = newId;
                    ShowInfo("User created.", "Success");
                    NavigateBack(true, newId);
                }
                else
                {
                    user.Id = _userId.Value;
                    _db.UpdateUser(user);
                    ShowInfo("User updated.", "Success");
                    NavigateBack(true, user.Id);
                }
            }
            catch (Exception ex)
            {
                ShowError("Failed to save user.");
                System.Diagnostics.Debug.WriteLine($"UserDetailsPage.btnSave_Click error: {ex}");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                NavigateBack(false, _userId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserDetailsPage.btnCancel_Click error: {ex}");
            }
        }

        private void NavigateBack(bool saved, int? id)
        {
            try
            {
                NavigateBackRequested?.Invoke(this, new NavigateBackEventArgs
                {
                    Saved = saved,
                    UserId = id
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UserDetailsPage.NavigateBack error: {ex}");
            }
        }
    }
}
