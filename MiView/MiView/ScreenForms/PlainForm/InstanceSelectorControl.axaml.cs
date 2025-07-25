using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiView.ScreenForms.PlainForm
{
    public partial class InstanceSelectorControl : UserControl
    {
        public event EventHandler<RoutedEventArgs>? ConnectButtonClicked;
        public event EventHandler<RoutedEventArgs>? AddInstanceButtonClicked;
        public event EventHandler<string>? ServerTabSelected;

        private List<string> _servers = new();
        private string? _selectedServer;
        private Dictionary<string, Button> _serverButtons = new();

        public InstanceSelectorControl()
        {
            InitializeComponent();
            UpdateScrollButtons();
        }

        public Button ConnectButton => cmdConnect;
        public Button AddInstanceButton => new Button(); // 仮の実装（後で削除予定）

        public string? SelectedServer => _selectedServer;

        public List<string> Servers
        {
            get => _servers;
            set
            {
                _servers = value ?? new List<string>();
                UpdateServerTabs();
            }
        }

        private void OnConnectButtonClick(object? sender, RoutedEventArgs e)
        {
            ConnectButtonClicked?.Invoke(this, e);
        }

        private void OnAddInstanceButtonClick(object? sender, RoutedEventArgs e)
        {
            AddInstanceButtonClicked?.Invoke(this, e);
        }

        private void OnScrollLeftClick(object? sender, RoutedEventArgs e)
        {
            scrollViewer.LineLeft();
            UpdateScrollButtons();
        }

        private void OnScrollRightClick(object? sender, RoutedEventArgs e)
        {
            scrollViewer.LineRight();
            UpdateScrollButtons();
        }

        private void OnServerTabClick(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string serverName)
            {
                SelectServer(serverName);
            }
        }

        private void UpdateServerTabs()
        {
            // 既存のボタンをクリア
            serverTabsPanel.Children.Clear();
            _serverButtons.Clear();

            foreach (var server in _servers)
            {
                var button = new Button
                {
                    Content = server,
                    Tag = server,
                    Classes = { "server-tab" }
                };
                button.Click += OnServerTabClick;
                
                serverTabsPanel.Children.Add(button);
                _serverButtons[server] = button;
            }

            // 最初のサーバーを選択
            if (_servers.Count > 0 && _selectedServer == null)
            {
                SelectServer(_servers[0]);
            }
            else if (_selectedServer != null && _servers.Contains(_selectedServer))
            {
                SelectServer(_selectedServer);
            }
        }

        public void SelectServer(string serverName)
        {
            if (!_servers.Contains(serverName))
                return;

            // 前の選択をクリア
            if (_selectedServer != null && _serverButtons.ContainsKey(_selectedServer))
            {
                _serverButtons[_selectedServer].Classes.Remove("selected");
            }

            // 新しい選択を設定
            _selectedServer = serverName;
            if (_serverButtons.ContainsKey(serverName))
            {
                _serverButtons[serverName].Classes.Add("selected");
            }

            ServerTabSelected?.Invoke(this, serverName);
        }

        private void UpdateScrollButtons()
        {
            // スクロール可能かどうかをチェック
            btnScrollLeft.IsEnabled = scrollViewer.Offset.X > 0;
            btnScrollRight.IsEnabled = scrollViewer.Offset.X < scrollViewer.Extent.Width - scrollViewer.Viewport.Width;
        }
        
        /// <summary>
        /// 接続ボタンのテキストを設定し、幅を調整
        /// </summary>
        public void SetConnectButtonText(string text)
        {
            cmdConnect.Content = text;
            
            // テキストの長さに応じてボタンの幅を調整
            var textLength = text.Length;
            if (textLength <= 4) // "接続" など短いテキスト
            {
                cmdConnect.MinWidth = 60;
                cmdConnect.MaxWidth = 80;
            }
            else if (textLength <= 8) // "切断 (2接続中)" など
            {
                cmdConnect.MinWidth = 80;
                cmdConnect.MaxWidth = 120;
            }
            else // より長いテキスト
            {
                cmdConnect.MinWidth = 100;
                cmdConnect.MaxWidth = 150;
            }
        }
        
        /// <summary>
        /// 接続状態に応じてボタンのテキストを更新
        /// </summary>
        public void UpdateConnectButtonState(string instanceUrl, bool isConnected)
        {
            if (isConnected)
            {
                SetConnectButtonText("切断");
            }
            else
            {
                SetConnectButtonText("接続");
            }
        }
    }
} 