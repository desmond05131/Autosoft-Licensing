using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.XtraEditors;

namespace Autosoft_Licensing.UI
{
    /// <summary>
    /// Small factory used by AutoCount plugin hosts to create the licensing UI window
    /// without starting a separate message loop.
    /// </summary>
    public static class HostFactory
    {
        public static XtraForm CreateMainForm()
        {
            // MainForm must be a DevExpress.XtraEditors.XtraForm in your project.
            return new MainForm();
        }
    }
}
