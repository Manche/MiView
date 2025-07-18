using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace MiView
{
    public partial class MainWindow : Avalonia.Controls.Window
    {
        private ClientWebSocket? _webSocket;
        private CancellationTokenSource? _cancellationTokenSource;
        private ObservableCollection<string> _instances = new();
        private int _noteCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeUI();
        }

        private void InitializeUI()
        {
            // インスタンス選択コンボボックスの初期化
            cmbInstanceSelect.ItemsSource = _instances;
            
            // 初期メッセージ
            tsLabelMain.Text = "タブ件数：";
            tsLabelNoteCount.Text = "0/0";
            
            // テスト用の投稿を追加
            AddTestTimelineItems();
        }

        private void AddTestTimelineItems()
        {
            // ローディングメッセージを削除
            timelineContainer.Children.Clear();
            
            // テスト用のタイムライン項目を追加
            var testItems = new[]
            {
                ("テスト投稿 1: MiViewのテストです。LinuxでAvalonia UIが動作中。", "homeTimeline", "misskey.io"),
                ("テスト投稿 2: 元のWindows FormsデザインをAvaloniaで再現。", "localTimeline", "misskey.dev"),
                ("テスト投稿 3: クロスプラットフォーム対応完了。", "socialTimeline", "misskey.shinan...")
            };

            for (int i = 0; i < testItems.Length; i++)
            {
                var (content, channel, instance) = testItems[i];
                AddTimelineItem($"テストユーザー{i + 1}", content, DateTime.Now.AddMinutes(-i), "note", channel, instance);
            }
        }

        private void AddTimelineItem(string username, string content, DateTime timestamp, string type = "note", string channel = "homeTimeline", string instance = "misskey.io")
        {
            // 交互の行色を決定
            var isEvenRow = (_noteCount % 2 == 0);
            var backgroundColor = isEvenRow ? Avalonia.Media.Brushes.White : Avalonia.Media.Brush.Parse("#F5F5F5");
            
            var timelineItem = new Grid
            {
                Background = backgroundColor,
                Height = 22,
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
                ColumnDefinitions =
                {
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(60) },
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(100) },
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(160) },
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(1, Avalonia.Controls.GridUnitType.Star) },
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(120) }
                }
            };

            // 各列にBorderとTextBlockを追加
            for (int i = 0; i < 5; i++)
            {
                var border = new Border
                {
                    BorderBrush = Avalonia.Media.Brushes.LightGray,
                    BorderThickness = new Avalonia.Thickness(0, 0, i < 4 ? 1 : 0, 1),
                    [Grid.ColumnProperty] = i
                };

                var textBlock = new TextBlock
                {
                    FontSize = 11,
                    Foreground = Avalonia.Media.Brushes.Black,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    Margin = new Avalonia.Thickness(3, 0),
                    TextTrimming = Avalonia.Media.TextTrimming.CharacterEllipsis
                };

                switch (i)
                {
                    case 0:
                        textBlock.Text = type;
                        break;
                    case 1:
                        textBlock.Text = channel;
                        break;
                    case 2:
                        textBlock.Text = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
                        break;
                    case 3:
                        textBlock.Text = $"@{username}: {content}";
                        break;
                    case 4:
                        textBlock.Text = instance;
                        break;
                }

                border.Child = textBlock;
                timelineItem.Children.Add(border);
            }

            // クリックイベントを追加
            timelineItem.PointerPressed += (sender, e) =>
            {
                SetTimelineDetails(username, content, timestamp);
            };

            // タイムラインの先頭に追加
            timelineContainer.Children.Insert(0, timelineItem);
            
            // 投稿数をカウント
            _noteCount++;
            tsLabelNoteCount.Text = $"{_noteCount}/9999";
        }

        private async void cmdAddInstance_Click(object? sender, RoutedEventArgs e)
        {
            // インスタンス追加ダイアログ
            var dialog = new Avalonia.Controls.Window
            {
                Title = "インスタンス追加",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Children =
                    {
                        new TextBlock { Text = "インスタンスURL:", Margin = new Avalonia.Thickness(0, 0, 0, 5) },
                        new TextBox { Name = "urlTextBox", Watermark = "https://misskey.io", Margin = new Avalonia.Thickness(0, 0, 0, 10) },
                        new TextBlock { Text = "APIキー (オプション):", Margin = new Avalonia.Thickness(0, 0, 0, 5) },
                        new TextBox { Name = "apiKeyTextBox", Watermark = "APIキーを入力", Margin = new Avalonia.Thickness(0, 0, 0, 10) },
                        new StackPanel
                        {
                            Orientation = Avalonia.Layout.Orientation.Horizontal,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                            Children =
                            {
                                new Button { Content = "キャンセル", Margin = new Avalonia.Thickness(0, 0, 10, 0) },
                                new Button { Content = "追加" }
                            }
                        }
                    }
                }
            };

            await dialog.ShowDialog(this);
        }

        private async Task AddInstance(string instanceUrl, string? apiKey = null)
        {
            try
            {
                // インスタンスをリストに追加
                _instances.Add(instanceUrl);
                
                // 選択状態にする
                cmbInstanceSelect.SelectedItem = instanceUrl;
                
                tsLabelMain.Text = $"インスタンス {instanceUrl} を追加しました";
                
                // WebSocket接続を開始
                await ConnectToTimeline(instanceUrl, apiKey);
            }
            catch (Exception ex)
            {
                tsLabelMain.Text = $"エラー: {ex.Message}";
            }
        }

        private async Task ConnectToTimeline(string instanceUrl, string? apiKey = null)
        {
            try
            {
                // 既存の接続を切断
                await DisconnectWebSocket();
                
                // WebSocket接続を開始
                _cancellationTokenSource = new CancellationTokenSource();
                _webSocket = new ClientWebSocket();
                
                var wsUrl = string.IsNullOrEmpty(apiKey) 
                    ? $"wss://{instanceUrl}/streaming"
                    : $"wss://{instanceUrl}/streaming?i={apiKey}";
                
                await _webSocket.ConnectAsync(new Uri(wsUrl), _cancellationTokenSource.Token);
                
                if (_webSocket.State == WebSocketState.Open)
                {
                    tsLabelMain.Text = $"接続成功: {instanceUrl}";
                    
                    // タイムラインに接続
                    await SubscribeToTimeline("homeTimeline");
                    
                    // メッセージの受信を開始
                    _ = Task.Run(async () => await ReceiveMessages());
                }
                else
                {
                    tsLabelMain.Text = $"接続失敗: {instanceUrl}";
                }
            }
            catch (Exception ex)
            {
                tsLabelMain.Text = $"接続エラー: {ex.Message}";
            }
        }

        private async Task SubscribeToTimeline(string channel)
        {
            if (_webSocket?.State == WebSocketState.Open)
            {
                var subscribeMessage = JsonSerializer.Serialize(new
                {
                    type = "connect",
                    body = new
                    {
                        channel = channel,
                        id = "timeline"
                    }
                });
                
                var buffer = Encoding.UTF8.GetBytes(subscribeMessage);
                await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        private async Task ReceiveMessages()
        {
            var buffer = new byte[4096];
            
            while (_webSocket?.State == WebSocketState.Open && !_cancellationTokenSource?.Token.IsCancellationRequested == true)
            {
                try
                {
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource.Token);
                    
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        await ProcessTimelineMessage(message);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        tsLabelMain.Text = $"受信エラー: {ex.Message}";
                    });
                    break;
                }
            }
        }

        private async Task ProcessTimelineMessage(string message)
        {
            try
            {
                var json = JsonNode.Parse(message);
                
                if (json?["type"]?.ToString() == "channel" && json["body"]?["type"]?.ToString() == "note")
                {
                    var note = json["body"]["body"];
                    var username = note?["user"]?["username"]?.ToString() ?? "unknown";
                    var content = note?["text"]?.ToString() ?? "（内容なし）";
                    var createdAt = note?["createdAt"]?.ToString();
                    var channel = json["body"]?["id"]?.ToString() ?? "homeTimeline";
                    var selectedInstance = cmbInstanceSelect.SelectedItem?.ToString() ?? "misskey.io";
                    
                    DateTime timestamp = DateTime.Now;
                    if (DateTime.TryParse(createdAt, out var parsed))
                    {
                        timestamp = parsed;
                    }
                    
                    // UIスレッドで投稿を追加
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        AddTimelineItem(username, content, timestamp, "note", channel, selectedInstance);
                        SetTimelineDetails(username, content, timestamp);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"メッセージ処理エラー: {ex.Message}");
            }
        }

        private void SetTimelineDetails(string username, string content, DateTime timestamp)
        {
            lblUser.Text = $"@{username}";
            lblTLFrom.Text = "source: timeline";
            lblSoftware.Text = "Misskey";
            lblUpdatedAt.Text = timestamp.ToString("yyyy/MM/dd HH:mm:ss");
            txtDetail.Text = content;
        }

        private async Task DisconnectWebSocket()
        {
            if (_webSocket != null)
            {
                _cancellationTokenSource?.Cancel();
                
                if (_webSocket.State == WebSocketState.Open)
                {
                    await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect", CancellationToken.None);
                }
                
                _webSocket.Dispose();
                _webSocket = null;
            }
            
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        protected override void OnClosed(EventArgs e)
        {
            DisconnectWebSocket().Wait();
            base.OnClosed(e);
        }
    }
}