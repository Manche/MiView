using Avalonia.Controls;

namespace MiView.ScreenForms.PlainForm
{
    public partial class StatusBarControl : UserControl
    {
        public StatusBarControl()
        {
            InitializeComponent();
        }

        public TextBlock MainLabel => tsLabelMain;
        public TextBlock MemoryInfoLabel => tsLabelMemoryInfo;
        public TextBlock MemoryUsageLabel => tsLabelMemoryUsage;
    }
} 