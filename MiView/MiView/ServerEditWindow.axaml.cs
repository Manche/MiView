using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MiView
{
    public partial class ServerEditWindow : Window
    {
        public ServerEditWindow(string instanceName, string apiKey)
        {
            InitializeComponent();
            instanceNameTextBox.Text = instanceName;
            apiKeyTextBox.Text = apiKey;
            
            if (!string.IsNullOrEmpty(instanceName))
            {
                // 編集モードの場合、インスタンス名は変更不可
                instanceNameTextBox.IsReadOnly = true;
                instanceNameTextBox.Background = Avalonia.Media.Brush.Parse("#F5F5F5");
            }
        }

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            var instanceName = instanceNameTextBox.Text?.Trim();
            var apiKey = apiKeyTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(instanceName))
            {
                // エラーメッセージを表示
                ShowErrorMessage("インスタンス名を入力してください。");
                return;
            }
            
            Close(new ServerEditResult
            {
                Success = true,
                InstanceName = instanceName,
                ApiKey = apiKey ?? ""
            });
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            Close(new ServerEditResult { Success = false });
        }

        private async void ShowErrorMessage(string message)
        {
            var dialog = new Window
            {
                Title = "エラー",
                Width = 250,
                Height = 120,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = Avalonia.Media.Brush.Parse("#F0F0F0")
            };

            var panel = new StackPanel { 
                Margin = new Avalonia.Thickness(20),
                Background = Avalonia.Media.Brushes.Transparent
            };
            panel.Children.Add(new TextBlock 
            { 
                Text = message, 
                Margin = new Avalonia.Thickness(0, 0, 0, 20), 
                Foreground = Avalonia.Media.Brush.Parse("#000000"),
                Background = Avalonia.Media.Brushes.Transparent,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap
            });

            var okButton = new Button 
            { 
                Content = "OK", 
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                Background = Avalonia.Media.Brush.Parse("#FFFFFF"), 
                Foreground = Avalonia.Media.Brush.Parse("#000000"),
                BorderBrush = Avalonia.Media.Brush.Parse("#8C8C8C"),
                BorderThickness = new Avalonia.Thickness(1),
                Padding = new Avalonia.Thickness(15, 5)
            };
            
            okButton.Click += (s, e) => dialog.Close();
            panel.Children.Add(okButton);

            dialog.Content = panel;
            await dialog.ShowDialog(this);
        }
    }
}