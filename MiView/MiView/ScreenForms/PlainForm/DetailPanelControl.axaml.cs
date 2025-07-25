using Avalonia.Controls;

namespace MiView.ScreenForms.PlainForm
{
    public partial class DetailPanelControl : UserControl
    {
        public DetailPanelControl()
        {
            InitializeComponent();
        }

        public TextBlock UserLabel => lblUser;
        public TextBlock TLFromLabel => lblTLFrom;
        public TextBlock SoftwareLabel => lblSoftware;
        public TextBlock UpdatedAtLabel => lblUpdatedAt;
        public TextBox DetailTextBox => txtDetail;
    }
} 