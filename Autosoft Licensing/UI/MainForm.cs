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
            if (_currentPage != null)
            {
                _currentPage.Dispose();
            }

            // CRITICAL: Ensure the page fills the desktop panel
            page.Dock = DockStyle.Fill;

            _currentPage = page as PageBase;
            panelDesktop.Controls.Clear();
            panelDesktop.Controls.Add(page);
            panelDesktop.Tag = page;
            page.BringToFront();

            // Update Form Title
            if (page is PageBase pb)
            {
                lblTitle.Text = pb.Title; // Note: lblTitle might have been in the navbar. 
                                          // If lblTitle was deleted, remove this line or add a new title label to panelDesktop.
                this.Text = $"Autosoft Licensing - {pb.Title}"; // Update Window Title instead
            }
        }

        // Helper property for backward compatibility if pages try to access it
        private Label lblTitle => null;
    }
}