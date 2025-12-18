using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Autosoft_Licensing.Models;
using Autosoft_Licensing.UI.Pages;

namespace Autosoft_Licensing.UI
{
    public partial class MainForm : XtraForm
    {
        private User _currentUser;
        private PageBase _currentPage;

        public User CurrentUser => _currentUser;

        // BACKWARD COMPATIBILITY: some pages reference LoggedInUser on the host.
        // Provide LoggedInUser as an alias to CurrentUser to avoid changing all callers.
        public User LoggedInUser => _currentUser;

        public MainForm()
        {
            InitializeComponent();
            // Navbar customization calls removed
        }

        public void SetLoggedInUser(User user)
        {
            _currentUser = user;

            // Logic to direct user based on permissions
            if (_currentUser.CanGenerateLicense)
            {
                NavigateToPage("GenerateLicense");
            }
            else if (_currentUser.CanViewRecords)
            {
                NavigateToPage("LicenseRecords");
            }
            else if (_currentUser.CanManageUsers)
            {
                NavigateToPage("ManageUser");
            }
            else
            {
                // Fallback for users with no specific rights
                NavigateToPage("Login");
                MessageBox.Show("User has no access rights.", "Access Denied");
            }
        }

        public void LoadPage(UserControl page)
        {
            try
            {
                // Dispose previous page if any
                if (_currentPage != null)
                {
                    try { _currentPage.Dispose(); } catch { /* ignore dispose errors */ }
                    _currentPage = null;
                }

                // Clear existing controls from the desktop container
                panelDesktop.Controls.Clear();

                // CRITICAL: Force the page to fill the entire container
                page.Dock = DockStyle.Fill;

                // Add the new page
                panelDesktop.Controls.Add(page);

                // Track current page for disposal/navigation purposes
                _currentPage = page as PageBase;

                // Ensure the container repaints so the page is visible immediately
                panelDesktop.Refresh();

                // Bring the new page to front
                page.BringToFront();

                // Update Form Title if the page exposes a Title
                if (page is PageBase pb)
                {
                    try { lblTitle.Text = pb.Title; } catch { /* ignore if no title control */ }
                    this.Text = $"Autosoft Licensing - {pb.Title}";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadPage error: {ex}");
                // Best-effort: avoid throwing from UI navigation helper
            }
        }

        // Helper property for backward compatibility if pages try to access it
        private Label lblTitle => null;
    }
}