using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.UserSkins;
using System;
using System.Windows.Forms;
using Autosoft_Licensing.Data;
using Autosoft_Licensing.Services;

namespace Autosoft_Licensing
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Wire services that require runtime configuration
            ServiceRegistry.Database = new LicenseDatabaseService(new SqlConnectionFactory("LicensingDb"));

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
