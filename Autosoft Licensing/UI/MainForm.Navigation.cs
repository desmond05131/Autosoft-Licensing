using System;
using System.Drawing;
using System.Windows.Forms;
using Autosoft_Licensing.UI.Pages;

namespace Autosoft_Licensing.UI
{
    // This file is now purely for internal navigation logic, 
    // effectively stripping out the UI event handlers for the deleted navbar.
    public partial class MainForm
    {
        // Internal method to switch pages programmatically
        public void NavigateToPage(string pageKey)
        {
            switch (pageKey)
            {
                case "GenerateLicense":
                    LoadPage(new GenerateLicensePage());
                    break;
                case "LicenseRecords":
                    LoadPage(new LicenseRecordsPage());
                    break;
                case "ManageUser":
                    LoadPage(new ManageUserPage());
                    break;
                case "ManageProduct":
                    LoadPage(new ManageProductPage());
                    break;
                case "GeneralSetting":
                    LoadPage(new GeneralSettingPage());
                    break;
                case "Login":
                    LoadPage(new LoginPage());
                    break;
                default:
                    LoadPage(new LicenseRecordsPage());
                    break;
            }
        }
    }
}