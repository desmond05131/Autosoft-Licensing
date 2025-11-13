using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;

namespace Autosoft_Licensing
{
    public partial class MainForm : XtraForm
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private AccordionControlElement MakeItem(string text)
        {
            return new AccordionControlElement
            {
                Text = text,
                Style = ElementStyle.Item,
                Name = "ace" + text.Replace(" ", "").Replace("/", "")
            };
        }
    }
}