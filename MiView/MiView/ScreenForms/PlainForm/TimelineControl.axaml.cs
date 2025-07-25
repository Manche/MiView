using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MiView.ScreenForms.PlainForm
{
    public partial class TimelineControl : UserControl
    {
        public TimelineControl()
        {
            InitializeComponent();
        }

        public StackPanel TimelineContainer => timelineContainer;
        public StackPanel TabContainer => tabContainer;
        public Border TabBorder => tabBorder;
        public ScrollViewer TabScrollViewer => tabScrollViewer;
        public ScrollViewer TimelineScrollViewer => timelineScrollViewer;
        
        /// <summary>
        /// **PERFORMANCE FIX: Handle scroll events for virtualization**
        /// </summary>
        private void OnTimelineScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            // MainWindowに通知してビューポート更新
            TimelineScrollChanged?.Invoke(this, e);
        }
        
        public event EventHandler<ScrollChangedEventArgs>? TimelineScrollChanged;
        
        private void ScrollLeft_Click(object? sender, RoutedEventArgs e)
        {
            if (tabScrollViewer != null)
            {
                tabScrollViewer.Offset = new Avalonia.Vector(
                    System.Math.Max(0, tabScrollViewer.Offset.X - 100), 
                    tabScrollViewer.Offset.Y);
            }
        }
        
        private void ScrollRight_Click(object? sender, RoutedEventArgs e)
        {
            if (tabScrollViewer != null)
            {
                tabScrollViewer.Offset = new Avalonia.Vector(
                    tabScrollViewer.Offset.X + 100, 
                    tabScrollViewer.Offset.Y);
            }
        }
    }
}