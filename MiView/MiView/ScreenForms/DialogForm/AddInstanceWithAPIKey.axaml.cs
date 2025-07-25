using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Net.Http;
using System.Threading.Tasks;

namespace MiView.ScreenForms.DialogForm
{
    public partial class AddInstanceWithAPIKey : Window
    {
        public delegate void AddInstanceHandler(string instanceUrl, string tabName, string apiKey, string tlKind);
        public event AddInstanceHandler? InstanceAdded;

        public AddInstanceWithAPIKey()
        {
            InitializeComponent();
        }

        private async void cmdApply_Click(object? sender, RoutedEventArgs e)
        {
            var url = txtInstanceURL.Text?.Trim() ?? "";
            var apiKey = txtAPIKey.Text?.Trim() ?? "";
            var tabName = txtTabName.Text?.Trim() ?? "";
            var tlKind = (cmbTLKind.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";

            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(tabName))
            {
                await MessageBox("インスタンスURLもしくはAPIキー、タブ名称が入力されていません。", "エラー");
                return;
            }
            if (string.IsNullOrEmpty(tlKind))
            {
                await MessageBox("TLの種類を選択してください。", "エラー");
                return;
            }

            // URL存在チェック
            try
            {
                using var clt = new HttpClient();
                var httpResult = await clt.GetAsync($"http://{url}/");
                if (httpResult.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    await MessageBox("存在しないURLです。", "エラー");
                    return;
                }
            }
            catch
            {
                await MessageBox("URLの確認に失敗しました。", "エラー");
                return;
            }

            InstanceAdded?.Invoke(url, tabName, apiKey, tlKind);
            Close();
        }

        private async Task MessageBox(string message, string title)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Children =
                    {
                        new TextBlock { Text = message, Margin = new Avalonia.Thickness(0,0,0,20) },
                        new Button { Content = "OK", HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right, IsDefault = true }
                    }
                }
            };
            var btn = ((dialog.Content as StackPanel)?.Children[1]) as Button;
            btn!.Click += (_, __) => dialog.Close();
            await dialog.ShowDialog(this);
        }
    }
} 