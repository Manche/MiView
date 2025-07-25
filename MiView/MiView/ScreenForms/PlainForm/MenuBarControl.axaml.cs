using Avalonia.Controls;
using Avalonia.Interactivity;
using System;

namespace MiView.ScreenForms.PlainForm
{
    public partial class MenuBarControl : UserControl
    {
        public event EventHandler<RoutedEventArgs>? ServerManagementRequested;
        public event EventHandler<RoutedEventArgs>? ServerAddRequested;
        public event EventHandler<RoutedEventArgs>? ExitRequested;
#if DEBUG
        public event EventHandler<RoutedEventArgs>? GenerateDummyDataRequested;
#endif

        public MenuBarControl()
        {
            InitializeComponent();
            
#if DEBUG
            // Debugビルド時のみデバッグメニューを追加
            AddDebugMenu();
#endif
        }

#if DEBUG
        private void AddDebugMenu()
        {
            var debugMenuItem = new MenuItem
            {
                Header = "デバッグ(_D)",
                Background = Avalonia.Media.Brush.Parse("#2D2D30"),
                Foreground = Avalonia.Media.Brush.Parse("#FFFFFF")
            };

            var generateDummyDataMenuItem = new MenuItem
            {
                Header = "ダミーデータ生成(_T)",
                Background = Avalonia.Media.Brush.Parse("#2D2D30"),
                Foreground = Avalonia.Media.Brush.Parse("#FFFFFF")
            };
            
            generateDummyDataMenuItem.Click += OnGenerateDummyDataClick;
            debugMenuItem.Items.Add(generateDummyDataMenuItem);
            
            MainMenu.Items.Add(debugMenuItem);
        }
#endif

        private void OnServerManagementClick(object? sender, RoutedEventArgs e)
        {
            ServerManagementRequested?.Invoke(this, e);
        }

        private void OnServerAddClick(object? sender, RoutedEventArgs e)
        {
            ServerAddRequested?.Invoke(this, e);
        }

        private void OnExitClick(object? sender, RoutedEventArgs e)
        {
            ExitRequested?.Invoke(this, e);
        }

#if DEBUG
        private void OnGenerateDummyDataClick(object? sender, RoutedEventArgs e)
        {
            GenerateDummyDataRequested?.Invoke(this, e);
        }
#endif
    }
} 