using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace MiView
{
    public partial class ServerManagementWindow : Window
    {
        private readonly ObservableCollection<string> _instances;
        private readonly Dictionary<string, string> _instanceTokens;
        private readonly ObservableCollection<ServerInfo> _serverList = new();
        private bool _hasChanges = false;
        private const string SETTINGS_FILE = "settings.json";

        public ServerManagementWindow(ObservableCollection<string> instances, Dictionary<string, string> instanceTokens)
        {
            InitializeComponent();
            _instances = instances;
            _instanceTokens = instanceTokens;
            
            serverListBox.ItemsSource = _serverList;
            LoadServerList();
        }

        private void LoadServerList()
        {
            _serverList.Clear();
            foreach (var instance in _instances)
            {
                _serverList.Add(new ServerInfo
                {
                    InstanceName = instance,
                    HasApiKey = _instanceTokens.ContainsKey(instance) && !string.IsNullOrEmpty(_instanceTokens[instance])
                });
            }
        }

        private async void DeleteServer(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ServerInfo serverInfo)
            {
                var result = await ShowConfirmDialog($"サーバー '{serverInfo.InstanceName}' を削除しますか？");
                if (result)
                {
                    // インスタンスリストから削除
                    _instances.Remove(serverInfo.InstanceName);
                    
                    // トークンも削除
                    if (_instanceTokens.ContainsKey(serverInfo.InstanceName))
                    {
                        _instanceTokens.Remove(serverInfo.InstanceName);
                    }
                    
                    // 設定ファイルから削除
                    RemoveFromSettings(serverInfo.InstanceName);
                    
                    // UI更新
                    _serverList.Remove(serverInfo);
                    _hasChanges = true;
                    
                    // 即座に設定を保存
                    SaveSettings();
                    Console.WriteLine($"Server deleted: {serverInfo.InstanceName}, total instances: {_instances.Count}");
                }
            }
        }

        private async void EditServer(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ServerInfo serverInfo)
            {
                // 編集ダイアログを表示（簡易版）
                var editWindow = new ServerEditWindow(serverInfo.InstanceName, 
                    _instanceTokens.ContainsKey(serverInfo.InstanceName) ? _instanceTokens[serverInfo.InstanceName] : "");
                
                var result = await editWindow.ShowDialog<ServerEditResult?>(this);
                if (result?.Success == true && result.ApiKey != null)
                {
                    _instanceTokens[serverInfo.InstanceName] = result.ApiKey;
                    SaveSettings();
                    LoadServerList();
                    _hasChanges = true;
                    Console.WriteLine($"Server edited: {serverInfo.InstanceName}, API key updated");
                }
            }
        }

        private async void AddServer(object? sender, RoutedEventArgs e)
        {
            var addWindow = new ServerEditWindow("", "")
            {
                Title = "サーバー追加"
            };
            
            var result = await addWindow.ShowDialog<ServerEditResult?>(this);
            if (result?.Success == true && !string.IsNullOrEmpty(result.InstanceName))
            {
                var normalizedUrl = NormalizeInstanceUrl(result.InstanceName);
                if (!_instances.Contains(normalizedUrl))
                {
                    _instances.Add(normalizedUrl);
                    if (!string.IsNullOrEmpty(result.ApiKey))
                    {
                        _instanceTokens[normalizedUrl] = result.ApiKey;
                    }
                    SaveSettings();
                    LoadServerList();
                    _hasChanges = true;
                    Console.WriteLine($"Server added: {normalizedUrl}, total instances: {_instances.Count}");
                }
            }
        }

        private void CloseWindow(object? sender, RoutedEventArgs e)
        {
            Close(_hasChanges);
        }

        private void RemoveFromSettings(string instanceName)
        {
            try
            {
                if (File.Exists(SETTINGS_FILE))
                {
                    var json = File.ReadAllText(SETTINGS_FILE);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    
                    if (settings != null)
                    {
                        // インスタンスリストから削除
                        settings.Instances.Remove(instanceName);
                        
                        // APIキーからも削除
                        if (settings.InstanceTokens.ContainsKey(instanceName))
                        {
                            settings.InstanceTokens.Remove(instanceName);
                        }
                        
                        // 保存
                        var updatedJson = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(SETTINGS_FILE, updatedJson);
                        Console.WriteLine($"Removed {instanceName} from settings");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error removing from settings: {ex.Message}");
            }
        }

        private void SaveSettings()
        {
            try
            {
                var settings = new AppSettings
                {
                    Instances = _instances.ToList(),
                    InstanceTokens = _instanceTokens
                };
                
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SETTINGS_FILE, json);
                Console.WriteLine($"Settings saved successfully: {_instances.Count} instances - {string.Join(", ", _instances)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving settings: {ex.Message}");
            }
        }

        private async Task<bool> ShowConfirmDialog(string message)
        {
            var dialog = new Window
            {
                Title = "確認",
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Background = Avalonia.Media.Brush.Parse("#F0F0F0")
            };

            var panel = new StackPanel { Margin = new Avalonia.Thickness(20), Background = Avalonia.Media.Brushes.Transparent };
            panel.Children.Add(new TextBlock { 
                Text = message, 
                Margin = new Avalonia.Thickness(0, 0, 0, 20), 
                Foreground = Avalonia.Media.Brush.Parse("#000000"),
                Background = Avalonia.Media.Brushes.Transparent
            });

            var buttonPanel = new StackPanel { 
                Orientation = Avalonia.Layout.Orientation.Horizontal, 
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Background = Avalonia.Media.Brushes.Transparent
            };
            
            var yesButton = new Button { 
                Content = "はい", 
                Margin = new Avalonia.Thickness(0, 0, 10, 0), 
                Background = Avalonia.Media.Brush.Parse("#FFFFFF"), 
                Foreground = Avalonia.Media.Brush.Parse("#000000"),
                BorderBrush = Avalonia.Media.Brush.Parse("#8C8C8C"),
                BorderThickness = new Avalonia.Thickness(1),
                Padding = new Avalonia.Thickness(15, 5)
            };
            var noButton = new Button { 
                Content = "いいえ", 
                Background = Avalonia.Media.Brush.Parse("#FFFFFF"), 
                Foreground = Avalonia.Media.Brush.Parse("#000000"),
                BorderBrush = Avalonia.Media.Brush.Parse("#8C8C8C"),
                BorderThickness = new Avalonia.Thickness(1),
                Padding = new Avalonia.Thickness(15, 5)
            };

            bool result = false;
            yesButton.Click += (s, e) => { result = true; dialog.Close(); };
            noButton.Click += (s, e) => { result = false; dialog.Close(); };

            buttonPanel.Children.Add(yesButton);
            buttonPanel.Children.Add(noButton);
            panel.Children.Add(buttonPanel);

            dialog.Content = panel;
            await dialog.ShowDialog(this);
            return result;
        }
        
        private string NormalizeInstanceUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;
                
            var normalized = url.Trim();
            
            // プロトコルを除去
            if (normalized.StartsWith("http://"))
            {
                normalized = normalized.Substring(7);
            }
            else if (normalized.StartsWith("https://"))
            {
                normalized = normalized.Substring(8);
            }
            
            // 末尾のスラッシュを削除
            normalized = normalized.TrimEnd('/');
            
            return normalized;
        }
    }

    public class ServerInfo
    {
        public string InstanceName { get; set; } = "";
        public bool HasApiKey { get; set; }
        public string ApiKeyStatus => HasApiKey ? "APIキー: 設定済み" : "APIキー: 未設定";
    }

    public class ServerEditResult
    {
        public bool Success { get; set; }
        public string? InstanceName { get; set; }
        public string? ApiKey { get; set; }
    }
}