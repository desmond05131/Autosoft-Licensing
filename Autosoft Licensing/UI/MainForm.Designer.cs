using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Autosoft_Licensing
{
    partial class MainForm
    {
        // Fields declared for runtime-created controls (designer-safe)
        private AccordionControl accordion;
        private PanelControl contentPanel;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// This InitializeComponent wires the Load event only; runtime UI is created at runtime
        /// in MainForm.Navigation.cs to keep the designer stable.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(766, 600);
            this.Name = "MainForm";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);

        }

        // Add this method to fix CS1061
        private void MainForm_Load(object sender, EventArgs e)
        {
            // You can add initialization code here if needed
        }
    }
}