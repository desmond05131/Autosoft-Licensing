using System;
using DevExpress.XtraEditors;
using DevExpress.XtraBars.Navigation;

namespace Autosoft_Licensing
{
    public partial class MainForm : XtraForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void navList_SelectedIndexChanged(object sender, EventArgs e)
        {
            // TODO: swap content based on selected item.
            // This stub resolves the missing handler compile error.
        }

        // Placeholder if you later want to handle accordion clicks:
        // private void OnAccordionElementClick(object sender, ElementClickEventArgs e)
        // {
        //     // TODO: swap user controls into contentPanel based on e.Element.Text
        // }
    }
}