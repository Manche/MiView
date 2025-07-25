using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Avalonia.Controls.Selection;
using System;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using MiView.Common.TimeLine;
using MiView.Common.AnalyzeData.Format;
using MiView.Common.Connection.WebSocket.Misskey.v2025;
using MiView.Common.AnalyzeData;
using MiView.Common.Fonts;
using MiView.Common.Fonts.Material;
using MiView.Common.Connection;

namespace MiView
{
    public partial class MainWindow : Avalonia.Controls.Window
    {
        private ClientWebSocket? _webSocket;
        private CancellationTokenSource? _cancellationTokenSource;
        private ObservableCollection<string> _instances = new();
        private ObservableCollection<TimeLineContainer> _timelineData = new();
        private Dictionary<string, List<string>> _serverTabs = new();
        private Dictionary<string, string> _instanceTokens = new();
        // 定数
        private const string SETTINGS_FILE = "settings.json";
        private const int MAX_CACHED_ITEMS = 500; // 内部キャッシュ
        private const string DEFAULT_INSTANCE = "misskey.io";
        private const string DEFAULT_SOFTWARE = "Misskey";

        private const int MAX_UI_ITEMS = 500;      // UI表示
        // 状態管理
        private int _selectedTabIndex = 0;
        
        // **PERFORMANCE FIX: Scroll virtualization - dynamic based on UI size**
        private const int BUFFER_ITEM_COUNT = 5;   // 上下のバッファ（少なく調整）
        private const double ESTIMATED_ITEM_HEIGHT = 80.0; // 推定アイテム高さ
        private int _currentScrollIndex = 0;        // 現在のスクロール位置
        
        // データ管理
        private List<TimeLineContainer> _timelineItems = new();
        private Dictionary<string, List<TimeLineContainer>> _timelineCache = new();
        private Dictionary<string, Dictionary<string, List<TimeLineContainer>>> _timelineCacheByType = new(); // インスタンス別 → タイムライン種別 → データ
        
        // サービス
        private WebSocketTimeLineCommon? _webSocketTimeLine;
        private FontLoader _fontLoader = new();
        private TimeLineCreator _timelineCreator = new();
        private WebSocketConnectionManager _connectionManager;
        // private bool _isConnected = false; // 未使用のためコメントアウト
        private HashSet<string> _connectedInstances = new();
        // 統合TL用の接続管理（WebSocketConnectionManagerに統一）
        // インスタンスごとの持続接続（例: サーバー名→タイムライン種別→WebSocketTimeLineCommon）
        private Dictionary<string, Dictionary<string, WebSocketTimeLineCommon>> _persistentConnections = new();
        // JobQueue用フィールド
        private readonly Queue<(string InstanceName, string TimelineType, TimeLineContainer Container)> _timelineJobQueue = new();
        private bool _isJobQueueRunning = false;
        private const int JOBQUEUE_BATCH_SIZE = 10; // 一度に処理する最大件数
        private const int JOBQUEUE_INTERVAL_MS = 50; // 処理間隔（ms）
        
        // **MEMORY LEAK FIX: Add size limits for job queue**
        private const int MAX_JOBQUEUE_SIZE = 500; // 最大キュー容量
        
        // キャッシュクリーンアップ用
        private Timer? _cacheCleanupTimer;
        private const int CACHE_CLEANUP_INTERVAL_MINUTES = 3; // 3分間隔でチェック（より頻繁に）
        private const int CACHE_CLEANUP_AGE_MINUTES = 30; // 30分以上前のデータを削除
        private const int CACHE_CLEANUP_MIN_ITEMS = 50; // 50件を上回っている場合のみ実行
        
        // タブリスト定数（メモリ最適化）
        private static readonly string[] DEFAULT_TABS = { "統合TL", "ローカルTL", "ソーシャルTL", "グローバルTL", "ホームTL" };
        
        // 定数string（メモリ最適化）
        private const string LOADING_MESSAGE = "読み込み中...";
        private const string CONNECTED_MESSAGE = "接続済み";
        private const string DISCONNECTED_MESSAGE = "切断しました";
        private const string DEBUG_PREFIX = "[DEBUG]";
        private const string ERROR_PREFIX = "[ERROR]";

        /// <summary>
        /// タイムライン表示用の列定義
        /// </summary>
        private static readonly TimeLineCreator.TIMELINE_ELEMENT[] TimelineDisplayElements = new[]
        {
            TimeLineCreator.TIMELINE_ELEMENT.USERNAME,      // name
            TimeLineCreator.TIMELINE_ELEMENT.USERID,        // username
            TimeLineCreator.TIMELINE_ELEMENT.ICON,          // 公開範囲
            TimeLineCreator.TIMELINE_ELEMENT.RENOTED_DISP,  // RN(していなければなし)
            TimeLineCreator.TIMELINE_ELEMENT.ISLOCAL_DISP,  // 連合(してる場合緑,していなければ赤)
            TimeLineCreator.TIMELINE_ELEMENT.DETAIL,        // 本文
            TimeLineCreator.TIMELINE_ELEMENT.UPDATEDAT,     // 時間
            TimeLineCreator.TIMELINE_ELEMENT.SOURCE         // 受信したサーバー名
        };

        /// <summary>
        /// タイムライン表示用の列幅定義
        /// </summary>
        private static readonly Avalonia.Controls.GridLength[] TimelineColumnWidths = new[]
        {
            new Avalonia.Controls.GridLength(150), // USERNAME (name)
            new Avalonia.Controls.GridLength(100), // USERID (username)
            new Avalonia.Controls.GridLength(30),  // ICON (公開範囲)
            new Avalonia.Controls.GridLength(30),  // RENOTED_DISP (RN)
            new Avalonia.Controls.GridLength(30),  // ISLOCAL_DISP (連合)
            new Avalonia.Controls.GridLength(1, Avalonia.Controls.GridUnitType.Star), // DETAIL (本文)
            new Avalonia.Controls.GridLength(150), // UPDATEDAT (時間)
            new Avalonia.Controls.GridLength(120)  // SOURCE (受信したサーバー名)
        };

        /// <summary>
        /// UI色の定数
        /// </summary>
        private static class UIColors
        {
            public const string Public = "#4CAF50";      // 緑色（公開）
            public const string Home = "#FF9800";        // オレンジ色（ホーム）
            public const string Follower = "#F44336";    // 赤色（フォロワー）
            public const string Direct = "#9C27B0";      // 紫色（ダイレクト）
            public const string Renote = "#2E7D32";      // 深緑色（リノート）
            public const string Warning = "#FF9800";     // オレンジ色（警告）
            public const string Reply = "#2196F3";       // 青色（リプライ）
            public const string Help = "#9E9E9E";        // グレー色（ヘルプ）
            public const string Local = "#F44336";       // 赤色（ローカル）
            public const string Remote = "#4CAF50";      // 緑色（リモート）
            public const string TextPrimary = "#000000"; // 黒色（主要テキスト）
            public const string TextSecondary = "#666666"; // グレー色（補助テキスト）
            public const string Border = "#8C8C8C";      // グレー色（ボーダー）
            public const string BackgroundWhite = "#FFFFFF"; // 白色（背景）
            public const string BackgroundGray = "#F5F5F5"; // グレー色（背景）
            public const string BackgroundRenote = "#F0F8F0"; // 薄緑色（リノート背景）
            public const string HoverBlue = "#E8F4FD";   // 青色（ホバー）
            public const string HoverRenote = "#E0F0E0"; // 緑色（リノートホバー）
        }

        /// <summary>
        /// タイムラインアイテムの背景色を取得
        /// </summary>
        private Avalonia.Media.IBrush GetTimelineItemBackground(TimeLineContainer timelineItem)
        {
            if (timelineItem.RENOTED)
            {
                return Avalonia.Media.Brush.Parse(UIColors.BackgroundRenote);
            }
            
            var isEvenRow = (_timelineItems.Count % 2 == 0);
            return isEvenRow ? Avalonia.Media.Brush.Parse(UIColors.BackgroundWhite) : Avalonia.Media.Brush.Parse(UIColors.BackgroundGray);
        }

        /// <summary>
        /// タイムラインアイテムのホバー背景色を取得
        /// </summary>
        private Avalonia.Media.IBrush GetTimelineItemHoverBackground(TimeLineContainer timelineItem)
        {
            return timelineItem.RENOTED 
                ? Avalonia.Media.Brush.Parse(UIColors.HoverRenote)
                : Avalonia.Media.Brush.Parse(UIColors.HoverBlue);
        }

        /// <summary>
        /// 可視性アイコンを設定
        /// </summary>
        private void ConfigureVisibilityIcon(TextBlock textBlock, TimeLineContainer.PROTECTED_STATUS protectedStatus, Avalonia.Media.FontFamily materialIconFont)
        {
            switch (protectedStatus)
            {
                case TimeLineContainer.PROTECTED_STATUS.Public:
                    textBlock.Text = MaterialIcons.Public;
                    textBlock.Foreground = Avalonia.Media.Brush.Parse(UIColors.Public);
                    break;
                case TimeLineContainer.PROTECTED_STATUS.Home:
                    textBlock.Text = MaterialIcons.Home;
                    textBlock.Foreground = Avalonia.Media.Brush.Parse(UIColors.Home);
                    break;
                case TimeLineContainer.PROTECTED_STATUS.Follower:
                    textBlock.Text = MaterialIcons.Lock;
                    textBlock.Foreground = Avalonia.Media.Brush.Parse(UIColors.Follower);
                    break;
                case TimeLineContainer.PROTECTED_STATUS.Direct:
                    textBlock.Text = MaterialIcons.Mail;
                    textBlock.Foreground = Avalonia.Media.Brush.Parse(UIColors.Direct);
                    break;
                default:
                    textBlock.Text = MaterialIcons.Public;
                    textBlock.Foreground = Avalonia.Media.Brush.Parse(UIColors.Public);
                    break;
            }
            
            textBlock.FontFamily = materialIconFont;
            textBlock.FontSize = 14;
            textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        }
        // 持続接続用のデータ受信イベントハンドラ（ダミー実装）
        private void OnPersistentTimeLineDataReceived(object? sender, TimeLineContainer container) { /* 必要に応じて実装 */ }

        public MainWindow()
        {
            // **MEMORY OPTIMIZATION: Pre-allocate memory and optimize startup**
            PreAllocateMemory();
            
            InitializeComponent();
            _connectionManager = new WebSocketConnectionManager();
            _connectionManager.TimeLineDataReceived += OnConnectionManagerTimeLineDataReceived;
            _connectionManager.ConnectionStatusChanged += OnConnectionStatusChanged;
            InitializeUI();
            StartCacheCleanupTimer();
            
            // **PERFORMANCE FIX: Setup scroll virtualization**
            timelineControl.TimelineScrollChanged += OnTimelineScrollChanged;
            for (int i = 0; i < MAX_UI_ITEMS; i++)
            {
                var emptyItem = new TimeLineContainer
                {
                    IDENTIFIED = $"empty_{i}",
                    USERNAME = string.Empty,
                    USERID = string.Empty,
                    DETAIL = string.Empty,
                    UPDATEDAT = string.Empty,
                    SOURCE = string.Empty,
                    SOFTWARE = string.Empty,
                    RENOTED = false,
                    ISLOCAL = false,
                    PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
                    ORIGINAL = null
                };
                _timelineData.Add(emptyItem);
                AddTimelineItem(emptyItem);
            }
        }

        private void InitializeUI()
        {
            // インスタンス選択コントロールのボタンイベントハンドラーを設定
            instanceSelectorControl.ConnectButtonClicked += cmdConnect_Click;
            instanceSelectorControl.AddInstanceButtonClicked += ShowAddInstanceDialog;
            instanceSelectorControl.ServerTabSelected += OnServerTabSelected;
            
            // メニューバーのイベントハンドラーを設定
            menuBarControl.ServerManagementRequested += ShowServerManagement;
            menuBarControl.ServerAddRequested += ShowAddInstanceDialog;
            menuBarControl.ExitRequested += OnExitRequested;
#if DEBUG
            menuBarControl.GenerateDummyDataRequested += GenerateDummyData;
#endif
            
            // 初期メッセージ
            statusBarControl.MainLabel.Text = "MiView - 起動中...";
            
            // 設定を読み込み
            LoadSettings();
            
            // サーバーリストを横タブに設定
            UpdateServerTabs();
            
            // テスト用の投稿を追加
            AddTestTimelineItems();
            
            // 持続接続を開始（全インスタンス）
            StartAllPersistentConnections();
            
            // 起動時に接続状態を設定
            Task.Delay(3000).ContinueWith(_ => 
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    if (_instances.Count > 0)
                    {
                        instanceSelectorControl.SetConnectButtonText("切断");
                        statusBarControl.MainLabel.Text = $"準備完了";
                        
                        // 初期サーバーを選択してタブを生成
                        if (instanceSelectorControl.SelectedServer == null && _instances.Count > 0)
                        {
                            instanceSelectorControl.SelectServer(_instances[0]);
                            Console.WriteLine($"Auto-selected first instance: {_instances[0]}");
                        }
                    }
                });
            });
        }

        private void AddTestTimelineItems()
        {
            // ローディングメッセージを削除
            timelineControl.TimelineContainer.Children.Clear();
            
            // // テスト用のタイムライン項目を追加
            // var testItems = new[]
            // {
            //     ("テスト投稿 1: MiViewのテストです。LinuxでAvalonia UIが動作中。", "homeTimeline", "mi.ruruke.moe"),
            //     ("テスト投稿 2: 元のWindows FormsデザインをAvaloniaで再現。", "localTimeline", "mi.ruruke.moe"),
            //     ("テスト投稿 3: クロスプラットフォーム対応完了。", "socialTimeline", "mi.ruruke.moe")
            // };

            // for (int i = 0; i < testItems.Length; i++)
            // {
            //     var (content, channel, instance) = testItems[i];
                
            //     // TimeLineContainerを作成（GitHubベースの構造に合わせて）
            //     var timelineItem = new TimeLineContainer
            //     {
            //         IDENTIFIED = $"test_{i + 1}_{DateTime.Now.Ticks}",
            //         USERID = $"user{i + 1}",
            //         USERNAME = $"テストユーザー{i + 1}",
            //         TLFROM = channel,
            //         RENOTED = false,
            //         REPLAYED = false,
            //         PROTECTED = TimeLineContainer.PROTECTED_STATUS.Public,
            //         ORIGINAL = JsonNode.Parse($"{{\"text\":\"{content}\",\"createdAt\":\"{DateTime.Now.AddMinutes(-i):yyyy-MM-ddTHH:mm:ss.fffZ}\",\"user\":{{\"username\":\"{$"テストユーザー{i + 1}"}\"}}}}")!,
            //         DETAIL = content,
            //         CONTENT = content, // CONTENTプロパティも設定
            //         UPDATEDAT = DateTime.Now.AddMinutes(-i).ToString("yyyy-MM-dd HH:mm:ss.fff"),
            //         SOURCE = instance,
            //         SOFTWARE = "Misskey",
            //         ISLOCAL = true
            //     };
                
            //     // TimeLineCreatorにデータを追加
            //     _timelineCreator.AddTimeLineData(timelineItem);
                
            //     // UIに表示
            //     AddTimelineItem(timelineItem, instance);
                
            //     // サンプルデータもキャッシュに追加
            //     var cacheKey = GetCacheKey(instance, _selectedTabIndex);
            //     if (!_timelineCache.ContainsKey(cacheKey))
            //     {
            //         _timelineCache[cacheKey] = new List<TimeLineContainer>();
            //     }
            //     _timelineCache[cacheKey].Insert(0, timelineItem);
                
            //     // ObservableCollectionにも追加
            //     _timelineData.Insert(0, timelineItem);
            // }
            
        }

#if DEBUG
        /// <summary>
        /// デバッグ用：各状態のダミーデータを生成
        /// </summary>
        private void GenerateDummyData(object? sender, RoutedEventArgs e)
        {
            // 既存のタイムラインをクリア
            timelineControl.TimelineContainer.Children.Clear();
            _timelineItems.Clear();
            
            // 各状態の組み合わせでダミーデータを生成
            var dummyItems = new[]
            {
                // 公開 + ローカル + リノートなし
                CreateDummyItem("公開ローカル", "public_local", "public", true, false, "これは公開投稿（ローカル）です。"),
                
                // 公開 + リモート + リノートなし
                CreateDummyItem("公開リモート", "public_remote", "public", false, false, "これは公開投稿（リモート）です。"),
                
                // ホーム + ローカル + リノートなし
                CreateDummyItem("ホームローカル", "home_local", "home", true, false, "これはホーム投稿（ローカル）です。"),
                
                // ホーム + リモート + リノートなし
                CreateDummyItem("ホームリモート", "home_remote", "home", false, false, "これはホーム投稿（リモート）です。"),
                
                // フォロワー + ローカル + リノートなし
                CreateDummyItem("フォロワーローカル", "follower_local", "followers", true, false, "これはフォロワー投稿（ローカル）です。"),
                
                // フォロワー + リモート + リノートなし
                CreateDummyItem("フォロワーリモート", "follower_remote", "followers", false, false, "これはフォロワー投稿（リモート）です。"),
                
                // ダイレクト + ローカル + リノートなし
                CreateDummyItem("ダイレクトローカル", "direct_local", "specified", true, false, "これはダイレクト投稿（ローカル）です。"),
                
                // ダイレクト + リモート + リノートなし
                CreateDummyItem("ダイレクトリモート", "direct_remote", "specified", false, false, "これはダイレクト投稿（リモート）です。"),
                
                // 公開 + ローカル + リノートあり
                CreateDummyItem("公開ローカルRN", "public_local_rn", "public", true, true, "これは公開投稿（ローカル）のリノートです。"),
                
                // 公開 + リモート + リノートあり
                CreateDummyItem("公開リモートRN", "public_remote_rn", "public", false, true, "これは公開投稿（リモート）のリノートです。"),
                
                // ホーム + ローカル + リノートあり
                CreateDummyItem("ホームローカルRN", "home_local_rn", "home", true, true, "これはホーム投稿（ローカル）のリノートです。"),
                
                // ホーム + リモート + リノートあり
                CreateDummyItem("ホームリモートRN", "home_remote_rn", "home", false, true, "これはホーム投稿（リモート）のリノートです。"),
            };

            foreach (var item in dummyItems)
            {
                AddTimelineItem(item);
            }
            
        }

        /// <summary>
        /// ダミーアイテムを作成
        /// </summary>
        private TimeLineContainer CreateDummyItem(string username, string userid, string visibility, bool isLocal, bool isRenoted, string content)
        {
            return new TimeLineContainer
            {
                IDENTIFIED = Guid.NewGuid().ToString(),
                USERNAME = username,
                USERID = userid,
                DETAIL = content,
                UPDATEDAT = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                SOURCE = isLocal ? "mi.ruruke.moe" : "misskey.flowers",
                SOFTWARE = "Misskey",
                RENOTED = isRenoted,
                ISLOCAL = isLocal,
                PROTECTED = StringToProtectedStatus(visibility),
                ORIGINAL = JsonNode.Parse($"{{\"visibility\":\"{visibility}\"}}")!
            };
        }
#endif

        private void AddTimelineItem(TimeLineContainer timelineItem, string instance = "misskey.io")
        {
            // TimeLineContainerからNoteオブジェクトを作成
            var note = new Note { Node = timelineItem.ORIGINAL };
            
            // 背景色を決定（Renote、交互の行色）
            Avalonia.Media.IBrush backgroundColor = GetTimelineItemBackground(timelineItem);
            
            var timelineGrid = new Grid
            {
                Background = backgroundColor,
                Height = 18,
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand),
                ColumnDefinitions =
                {
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(150) }, // USERNAME (name)
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(100) }, // USERID (username)
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(30) },  // ICON (公開範囲)
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(30) },  // RENOTED_DISP (RN)
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(30) },  // ISLOCAL_DISP (連合)
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(1, Avalonia.Controls.GridUnitType.Star) }, // DETAIL (本文)
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(150) }, // UPDATEDAT (時間)
                    new Avalonia.Controls.ColumnDefinition { Width = new Avalonia.Controls.GridLength(120) }  // SOURCE (受信したサーバー名)
                }
            };

            // ホバー効果を追加
            timelineGrid.PointerEntered += (sender, e) =>
            {
                timelineGrid.Background = GetTimelineItemHoverBackground(timelineItem);
            };
            
            timelineGrid.PointerExited += (sender, e) =>
            {
                timelineGrid.Background = backgroundColor;
            };

            // TimeLineCreatorのTIMELINE_ELEMENTに基づいて表示する列を定義
            var displayElements = new[]
            {
                TimeLineCreator.TIMELINE_ELEMENT.USERNAME,      // name
                TimeLineCreator.TIMELINE_ELEMENT.USERID,        // username
                TimeLineCreator.TIMELINE_ELEMENT.ICON,          // 公開範囲
                TimeLineCreator.TIMELINE_ELEMENT.RENOTED_DISP,  // RN(していなければなし)
                TimeLineCreator.TIMELINE_ELEMENT.ISLOCAL_DISP,  // 連合(してる場合緑,していなければ赤)
                TimeLineCreator.TIMELINE_ELEMENT.DETAIL,        // 本文
                TimeLineCreator.TIMELINE_ELEMENT.UPDATEDAT,     // 時間
                TimeLineCreator.TIMELINE_ELEMENT.SOURCE         // 受信したサーバー名
            };

            // 各列にBorderとTextBlockを追加
            for (int i = 0; i < displayElements.Length; i++)
            {
                var element = displayElements[i];
                
                var border = new Border
                {
                    Background = Avalonia.Media.Brushes.Transparent,
                    BorderBrush = Avalonia.Media.Brush.Parse(UIColors.Border),
                    BorderThickness = new Avalonia.Thickness(0, 0, i < displayElements.Length - 1 ? 1 : 0, 1),
                    [Grid.ColumnProperty] = i
                };

                var textBlock = new TextBlock
                {
                    FontSize = 11,
                    Foreground = Avalonia.Media.Brush.Parse(UIColors.TextPrimary),
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                    Margin = new Avalonia.Thickness(2, 0),
                    TextTrimming = Avalonia.Media.TextTrimming.CharacterEllipsis
                };
                
                // MaterialIconsフォントを取得
                var materialIconFont = _fontLoader.LoadFontFamilyFromFile(FontLoader.FONT_SELECTOR.MATERIALICONS);

                switch (element)
                {
                    case TimeLineCreator.TIMELINE_ELEMENT.ICON:
                        // 投稿の可視性（ノート範囲）アイコンを表示
                        ConfigureVisibilityIcon(textBlock, timelineItem.PROTECTED, materialIconFont);
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.USERNAME:
                        textBlock.Text = timelineItem.USERNAME;
                        textBlock.FontWeight = Avalonia.Media.FontWeight.Bold;
                        textBlock.Foreground = Avalonia.Media.Brush.Parse("#000000");
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.USERID:
                        textBlock.Text = timelineItem.USERID;
                        textBlock.Foreground = Avalonia.Media.Brush.Parse("#666666");
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.REPLAYED_DISP:
                        if (timelineItem.REPLAYED)
                        {
                            textBlock.Text = MaterialIcons.Reply;
                            textBlock.FontFamily = materialIconFont;
                            textBlock.Foreground = Avalonia.Media.Brush.Parse("#2196F3");
                            textBlock.FontSize = 14;
                            textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                        }
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.PROTECTED_DISP:
                        switch (timelineItem.PROTECTED)
                        {
                            case TimeLineContainer.PROTECTED_STATUS.Public:
                                textBlock.Text = MaterialIcons.Public;
                                textBlock.Foreground = Avalonia.Media.Brush.Parse("#4CAF50");
                                break;
                            case TimeLineContainer.PROTECTED_STATUS.Home:
                                textBlock.Text = MaterialIcons.Home;
                                textBlock.Foreground = Avalonia.Media.Brush.Parse("#FF9800");
                                break;
                            case TimeLineContainer.PROTECTED_STATUS.Follower:
                                textBlock.Text = MaterialIcons.Lock;
                                textBlock.Foreground = Avalonia.Media.Brush.Parse("#F44336");
                                break;
                            default:
                                textBlock.Text = MaterialIcons.Help;
                                textBlock.Foreground = Avalonia.Media.Brush.Parse("#9E9E9E");
                                break;
                        }
                        textBlock.FontFamily = materialIconFont;
                        textBlock.FontSize = 14;
                        textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.RENOTED_DISP:
                        if (timelineItem.RENOTED)
                        {
                            textBlock.Text = MaterialIcons.Repeat;
                            textBlock.FontFamily = materialIconFont;
                            textBlock.Foreground = Avalonia.Media.Brush.Parse("#2E7D32");
                            textBlock.FontSize = 14;
                            textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                        }
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.CW_DISP:
                        if (timelineItem.CW)
                        {
                            textBlock.Text = MaterialIcons.Warning;
                            textBlock.FontFamily = materialIconFont;
                            textBlock.Foreground = Avalonia.Media.Brush.Parse("#FF9800");
                            textBlock.FontSize = 14;
                            textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                        }
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.ISLOCAL_DISP:
                        // 連合状態に応じた宇宙船アイコンを表示
                        if (timelineItem.ISLOCAL)
                        {
                            // ローカル（連合している）：赤の宇宙船アイコン
                            textBlock.Text = MaterialIcons.Rocket;
                            textBlock.FontFamily = materialIconFont;
                            textBlock.Foreground = Avalonia.Media.Brush.Parse("#F44336");
                        }
                        else
                        {
                            // リモート（連合していない）：緑の宇宙船アイコン
                            textBlock.Text = MaterialIcons.Rocket;
                            textBlock.FontFamily = materialIconFont;
                            textBlock.Foreground = Avalonia.Media.Brush.Parse("#4CAF50");
                        }
                        textBlock.FontSize = 14;
                        textBlock.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.DETAIL:
                        textBlock.Text = timelineItem.DETAIL;
                        textBlock.TextWrapping = Avalonia.Media.TextWrapping.Wrap;
                        
                        // CWとRenoteの表示を色分け
                        if (timelineItem.CW && timelineItem.DETAIL.StartsWith("CW: "))
                        {
                            // CWの場合はオレンジ色で表示（CW部分のみ）
                            // 本文部分は黒色で表示するため、複数のTextBlockを使用
                            var stackPanel = new StackPanel();
                            
                            // CW部分を分割して表示
                            var lines = timelineItem.DETAIL.Split('\n');
                            bool isFirstLine = true;
                            
                            foreach (var line in lines)
                            {
                                var lineTextBlock = new TextBlock
                                {
                                    Text = line,
                                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                                    Margin = new Avalonia.Thickness(0, isFirstLine ? 0 : 2, 0, 0)
                                };
                                
                                if (isFirstLine && line.StartsWith("CW: "))
                                {
                                    // 最初の行がCWの場合はオレンジ色
                                    lineTextBlock.Foreground = Avalonia.Media.Brush.Parse("#FF9800");
                                }
                                else
                                {
                                    // その他の行は黒色（本文）
                                    lineTextBlock.Foreground = Avalonia.Media.Brush.Parse("#000000");
                                }
                                
                                stackPanel.Children.Add(lineTextBlock);
                                isFirstLine = false;
                            }
                            
                            border.Child = stackPanel;
                            timelineGrid.Children.Add(border);
                            continue; // 既にborderを追加したので、後続の処理をスキップ
                        }
                        else if (timelineItem.RENOTED && timelineItem.DETAIL.Contains(" RN:"))
                        {
                            // Renoteの場合は控えめな緑色で表示
                            textBlock.Foreground = Avalonia.Media.Brush.Parse("#2E7D32");
                        }
                        else
                        {
                            // 通常のテキストは黒色
                            textBlock.Foreground = Avalonia.Media.Brush.Parse("#000000");
                        }
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.UPDATEDAT:
                        textBlock.Text = timelineItem.UPDATEDAT;
                        textBlock.FontSize = 10;
                        textBlock.Foreground = Avalonia.Media.Brush.Parse("#666666");
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.SOURCE:
                        textBlock.Text = timelineItem.SOURCE;
                        textBlock.FontSize = 10;
                        textBlock.Foreground = Avalonia.Media.Brush.Parse("#666666");
                        break;
                    case TimeLineCreator.TIMELINE_ELEMENT.SOFTWARE:
                        textBlock.Text = timelineItem.SOFTWARE;
                        textBlock.FontSize = 10;
                        textBlock.Foreground = Avalonia.Media.Brush.Parse("#666666");
                        break;
                }

                border.Child = textBlock;
                timelineGrid.Children.Add(border);
            }

            // クリックイベントを追加
            timelineGrid.PointerPressed += (sender, e) =>
            {
                SetTimelineDetails(timelineItem, note);
            };

            // **PERFORMANCE FIX: 軽量化 - 重い要素は遅延作成**
            // タイムラインの先頭に追加
            timelineControl.TimelineContainer.Children.Insert(0, timelineGrid);
            
            // TimelineContainerの件数制限
            if (timelineControl.TimelineContainer.Children.Count > MAX_UI_ITEMS)
            {
                // **PERFORMANCE FIX: 削除時にオブジェクトを完全にクリア**
                var removedItem = timelineControl.TimelineContainer.Children[timelineControl.TimelineContainer.Children.Count - 1];
                if (removedItem is Grid removedGrid)
                {
                    // 子要素も完全にクリア
                    removedGrid.Children.Clear();
                }
                timelineControl.TimelineContainer.Children.RemoveAt(timelineControl.TimelineContainer.Children.Count - 1);
                Console.WriteLine($"{DEBUG_PREFIX} TimelineContainer削除（軽量化済）: {timelineControl.TimelineContainer.Children.Count}件");
            }
            
            // リストに追加
            _timelineItems.Add(timelineItem);
            
            // _timelineItemsの件数制限
            if (_timelineItems.Count > MAX_UI_ITEMS)
            {
                Console.WriteLine($"{DEBUG_PREFIX} _timelineItems制限: {_timelineItems.Count}件 → {MAX_UI_ITEMS}件に削減");
                _timelineItems.RemoveAt(_timelineItems.Count - 1);
                Console.WriteLine($"{DEBUG_PREFIX} _timelineItems削除後: {_timelineItems.Count}件");
            }
            
            UpdateStatusBarInfo();
        }
        

        private Button CreateActionButton(string emoji, string action)
        {
            var button = new Button
            {
                Content = emoji,
                FontSize = 14,
                Background = Avalonia.Media.Brushes.Transparent,
                BorderThickness = new Avalonia.Thickness(0),
                Padding = new Avalonia.Thickness(8, 4),
                Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
            };
            
            Avalonia.Controls.ToolTip.SetTip(button, action);
            return button;
        }

        private string GetRelativeTime(DateTime timestamp)
        {
            var now = DateTime.Now;
            var diff = now - timestamp;

            if (diff.TotalMinutes < 1)
                return "now";
            if (diff.TotalMinutes < 60)
                return $"{(int)diff.TotalMinutes}m";
            if (diff.TotalHours < 24)
                return $"{(int)diff.TotalHours}h";
            if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays}d";
            
            return timestamp.ToString("MM/dd");
        }

        private async void cmdConnect_Click(object? sender, RoutedEventArgs e)
        {
#if DEBUG
            Console.WriteLine("Connect button clicked!");
#endif
            var instanceUrl = instanceSelectorControl.SelectedServer?.Trim();
#if DEBUG
            Console.WriteLine($"Selected instance: {instanceUrl}");
            Console.WriteLine($"Current connected instances: {string.Join(", ", _connectedInstances)}");
#endif
            
            if (string.IsNullOrEmpty(instanceUrl))
            {
#if DEBUG
                Console.WriteLine("No instance selected, showing add dialog");
#endif
                // 新しいインスタンスを追加するためのダイアログを表示
                await ShowAddInstanceDialog();
                return;
            }
            
            // 選択されたサーバーが既に接続されているかチェック
            if (_connectedInstances.Contains(instanceUrl))
            {
#if DEBUG
                Console.WriteLine($"Disconnecting instance: {instanceUrl}");
#endif
                // 切断処理：選択されたサーバーの接続のみ切断（ユーザー操作による切断）
                await DisconnectInstance(instanceUrl, true);
                statusBarControl.MainLabel.Text = $"切断しました: {instanceUrl} ({_connectedInstances.Count}接続中)";
#if DEBUG
                Console.WriteLine($"Disconnect completed. Remaining connections: {string.Join(", ", _connectedInstances)}");
#endif
                return;
            }
            
#if DEBUG
            Console.WriteLine($"Connecting to new instance: {instanceUrl}");
#endif
            // 新しいサーバーに接続（既存の接続は維持）
            var apiKey = _instanceTokens.ContainsKey(instanceUrl) ? _instanceTokens[instanceUrl] : null;
#if DEBUG
            Console.WriteLine($"Connecting to {instanceUrl} with API key: {(apiKey != null ? "Yes" : "No")}");
#endif
            await ConnectToTimeline(instanceUrl, apiKey);
        }

        private async void ShowAddInstanceDialog(object? sender, RoutedEventArgs e)
        {
            await ShowAddInstanceDialog();
        }
        
        private async Task ShowAddInstanceDialog()
        {
            var urlTextBox = new TextBox { 
                Name = "urlTextBox", 
                Watermark = "mi.ruruke.moe", 
                Margin = new Avalonia.Thickness(0, 0, 0, 10),
                Background = Avalonia.Media.Brush.Parse("#FFFFFF"),
                Foreground = Avalonia.Media.Brush.Parse("#000000"),
                BorderBrush = Avalonia.Media.Brush.Parse("#8C8C8C"),
                BorderThickness = new Avalonia.Thickness(1),
                SelectionBrush = Avalonia.Media.Brush.Parse("#86b300"),
                SelectionForegroundBrush = Avalonia.Media.Brushes.White
            };
            var apiKeyTextBox = new TextBox { 
                Name = "apiKeyTextBox", 
                Watermark = "APIキー（オプション）", 
                Margin = new Avalonia.Thickness(0, 0, 0, 10),
                Background = Avalonia.Media.Brush.Parse("#FFFFFF"),
                Foreground = Avalonia.Media.Brush.Parse("#000000"),
                BorderBrush = Avalonia.Media.Brush.Parse("#8C8C8C"),
                BorderThickness = new Avalonia.Thickness(1),
                SelectionBrush = Avalonia.Media.Brush.Parse("#86b300"),
                SelectionForegroundBrush = Avalonia.Media.Brushes.White
            };
            
            var cancelButton = new Button 
            { 
                Content = "キャンセル", 
                Margin = new Avalonia.Thickness(0, 0, 10, 0),
                Background = Avalonia.Media.Brush.Parse("#FFFFFF"),
                BorderBrush = Avalonia.Media.Brush.Parse("#8C8C8C"),
                BorderThickness = new Avalonia.Thickness(1),
                Foreground = Avalonia.Media.Brush.Parse("#000000"),
                Padding = new Avalonia.Thickness(15, 5)
            };
            var addButton = new Button 
            { 
                Content = "追加",
                Background = Avalonia.Media.Brush.Parse("#FFFFFF"),
                BorderBrush = Avalonia.Media.Brush.Parse("#8C8C8C"),
                BorderThickness = new Avalonia.Thickness(1),
                Foreground = Avalonia.Media.Brush.Parse("#000000"),
                Padding = new Avalonia.Thickness(15, 5)
            };
            
            var dialog = new Avalonia.Controls.Window
            {
                Title = "インスタンス追加",
                Width = 400,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                CanResize = false,
                ShowInTaskbar = false,
                Background = Avalonia.Media.Brush.Parse("#F0F0F0"),
                Content = new StackPanel
                {
                    Margin = new Avalonia.Thickness(20),
                    Background = Avalonia.Media.Brushes.Transparent,
                    Children =
                    {
                        new TextBlock { 
                            Text = "インスタンスURL:", 
                            Margin = new Avalonia.Thickness(0, 0, 0, 5), 
                            Foreground = Avalonia.Media.Brush.Parse("#000000"),
                            Background = Avalonia.Media.Brushes.Transparent
                        },
                        urlTextBox,
                        new TextBlock { 
                            Text = "APIキー（オプション）:", 
                            Margin = new Avalonia.Thickness(0, 0, 0, 5), 
                            Foreground = Avalonia.Media.Brush.Parse("#000000"),
                            Background = Avalonia.Media.Brushes.Transparent
                        },
                        apiKeyTextBox,
                        new StackPanel
                        {
                            Orientation = Avalonia.Layout.Orientation.Horizontal,
                            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                            Background = Avalonia.Media.Brushes.Transparent,
                            Children =
                            {
                                cancelButton,
                                addButton
                            }
                        }
                    }
                }
            };

            cancelButton.Click += (s, e) => dialog.Close();
            
            addButton.Click += async (s, e) =>
            {
                try
                {
                    var url = urlTextBox.Text?.Trim();
                    var apiKey = apiKeyTextBox.Text?.Trim();
                    
                    if (string.IsNullOrEmpty(url))
                    {
                        statusBarControl.MainLabel.Text = "インスタンスURLを入力してください";
                        return;
                    }
                    
                    dialog.Close();
                    await AddInstance(url, string.IsNullOrEmpty(apiKey) ? null : apiKey);
                }
                catch (Exception ex)
                {
                    statusBarControl.MainLabel.Text = $"エラー: {ex.Message}";
                    dialog.Close();
                }
            };

            await dialog.ShowDialog(this);
        }

        private async Task AddInstance(string instanceUrl, string? apiKey = null)
        {
            try
            {
                // URLを正規化
                var normalizedUrl = NormalizeInstanceUrl(instanceUrl);
                
                statusBarControl.MainLabel.Text = $"インスタンス {normalizedUrl} を追加中...";
                
                // 既に存在する場合は追加しない
                if (_instances.Contains(normalizedUrl))
                {
                    statusBarControl.MainLabel.Text = $"インスタンス {normalizedUrl} は既に追加されています";
                    instanceSelectorControl.SelectServer(normalizedUrl);
                    return;
                }
                
                // インスタンスをリストに追加
                _instances.Add(normalizedUrl);
                
                // APIキーを保存
                if (!string.IsNullOrEmpty(apiKey))
                {
                    _instanceTokens[normalizedUrl] = apiKey;
                }
                
                // サーバー用のタブを作成（統合TL + 標準的なタイムラインのみ、独自TLは除外）
                var serverTabs = DEFAULT_TABS.ToList();
                _serverTabs[normalizedUrl] = serverTabs;
                
                // 選択状態にする
                instanceSelectorControl.SelectServer(normalizedUrl);
                
                statusBarControl.MainLabel.Text = $"インスタンス {normalizedUrl} を追加しました";
                
                // タブを更新
                UpdateTabs(normalizedUrl);
                
                statusBarControl.MainLabel.Text = $"インスタンス {normalizedUrl} の接続を開始しています...";
                
                // WebSocket接続を開始（ConnectToTimeline内でConnectPersistentInstanceが呼ばれる）
                var urlWithProtocol = AddProtocolToUrl(normalizedUrl);
                await ConnectToTimeline(urlWithProtocol, apiKey);
            }
            catch (Exception ex)
            {
                // エラーが発生した場合、追加したインスタンスを削除
                var normalizedUrl = NormalizeInstanceUrl(instanceUrl);
                if (_instances.Contains(normalizedUrl))
                {
                    _instances.Remove(normalizedUrl);
                }
                if (_instanceTokens.ContainsKey(normalizedUrl))
                {
                    _instanceTokens.Remove(normalizedUrl);
                }
                if (_serverTabs.ContainsKey(normalizedUrl))
                {
                    _serverTabs.Remove(normalizedUrl);
                }
                
                statusBarControl.MainLabel.Text = $"エラー: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"AddInstance error: {ex}");
                
                // エラーが発生しても設定を保存（正常なインスタンスは保持）
                SaveSettings();
            }
        }

        private async Task ConnectToTimeline(string instanceUrl, string? apiKey = null)
        {
            try
            {
#if DEBUG
                Console.WriteLine($"ConnectToTimeline called for {instanceUrl}");
#endif
                statusBarControl.MainLabel.Text = $"接続中: {instanceUrl}...";
                
                // 既存の接続は切断せず、新しいサーバーに接続
                // WebSocketConnectionManagerを使用して持続接続を管理
                await _connectionManager.ConnectPersistentInstance(instanceUrl, apiKey);
                
                // 接続済みインスタンスリストに追加
                _connectedInstances.Add(instanceUrl);
                
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    statusBarControl.MainLabel.Text = $"接続成功: {instanceUrl}";
                    
                    // 接続ボタンのテキストを更新
                    if (_connectedInstances.Count == 1)
                    {
                        instanceSelectorControl.SetConnectButtonText("切断");
                    }
                    else
                    {
                        instanceSelectorControl.SetConnectButtonText($"切断 ({_connectedInstances.Count}接続中)");
                    }
                    
                    // 現在選択中のインスタンスの接続状態を更新
                    var currentInstance = GetCurrentInstanceUrl();
                    if (currentInstance == instanceUrl)
                    {
                        instanceSelectorControl.UpdateConnectButtonState(currentInstance, true);
                    }
                });
                
                // 接続成功後に設定を保存
                SaveSettings();
            }
            catch (Exception ex)
            {
                statusBarControl.MainLabel.Text = $"接続エラー: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"ConnectToTimeline error: {ex}");
            }
        }
        
        
        private async void OnTimeLineDataReceived(object? sender, TimeLineContainer container)
        {
            System.Diagnostics.Debug.WriteLine($"Timeline data received from {container.SOURCE}: {container.DETAIL}");
#if DEBUG
            Console.WriteLine($"Timeline data received from {container.SOURCE}: {container.DETAIL}");
#endif
            
            // UIスレッドで投稿を追加
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                // キャッシュキーを生成
                var instanceName = string.IsNullOrEmpty(container.SOURCE) ? GetCurrentInstanceUrl() : container.SOURCE;
                
                // 統合TL接続中の場合は、常にタブインデックス0として処理
                var effectiveTabIndex = _selectedTabIndex;
#if DEBUG
                Console.WriteLine($"Current _selectedTabIndex: {_selectedTabIndex}");
#endif
                if (_selectedTabIndex == 0) // 統合TL接続中
                {
                    effectiveTabIndex = 0;
#if DEBUG
                    Console.WriteLine($"Using effectiveTabIndex=0 for unified timeline");
#endif
                }
                
                // 統合TL用の特別なキャッシュキー
                var cacheKey = effectiveTabIndex == 0 
                    ? "unified_tl_0" 
                    : GetCacheKey(instanceName, effectiveTabIndex);
                
                // タイムライン種別ごとの保存を追加（統合TLの場合はソーシャルTLとして保存）
                SaveToTimelineCacheByType(instanceName, container, effectiveTabIndex);
                
                // ソーシャルTLデータの場合は統合TL用にも保存（統合TLで表示するため）
                if (effectiveTabIndex == 0) // 統合TL接続中のソーシャルTLデータ
                {
#if DEBUG
                    Console.WriteLine($"🔄 UNIFIED TL DATA: Received from {instanceName} - {container.DETAIL?.Substring(0, Math.Min(30, container.DETAIL?.Length ?? 0))}...");
#endif
                    // ソーシャルTLタブ（インデックス2）用にも保存（重複チェック付き）
                    SaveToTimelineCacheByTypeIfNotExists(instanceName, container, 2);
                }
                
                // キャッシュに追加
                if (!_timelineCache.ContainsKey(cacheKey))
                {
                    _timelineCache[cacheKey] = new List<TimeLineContainer>();
                }
                
                _timelineCache[cacheKey].Insert(0, container);
                
                // キャッシュサイズ制限
                if (_timelineCache[cacheKey].Count > MAX_CACHED_ITEMS)
                {
                    Console.WriteLine($"{DEBUG_PREFIX} _timelineCache[{cacheKey}]制限: {_timelineCache[cacheKey].Count}件 → {MAX_CACHED_ITEMS}件に削減");
                    _timelineCache[cacheKey].RemoveAt(_timelineCache[cacheKey].Count - 1);
                    Console.WriteLine($"{DEBUG_PREFIX} _timelineCache[{cacheKey}]削除後: {_timelineCache[cacheKey].Count}件");
                }
                
                // UI更新条件を修正：統合TL接続中はすべてのソーシャルTLデータを表示
                var shouldUpdateUI = false;
                if (_selectedTabIndex == 0) // 統合TL表示中
                {
                    shouldUpdateUI = true;
#if DEBUG
                    Console.WriteLine($"Unified TL: showing data from {instanceName}");
#endif
                }
                else
                {
                    // 通常のタブ表示：現在のタブと一致する場合のみ表示
                    var currentCacheKey = effectiveTabIndex == 0 
                        ? "unified_tl_0" 
                        : GetCacheKey(GetCurrentInstanceUrl(), _selectedTabIndex);
                    shouldUpdateUI = (cacheKey == currentCacheKey);
#if DEBUG
                    Console.WriteLine($"Normal tab: received='{cacheKey}', current='{currentCacheKey}', match={shouldUpdateUI}");
#endif
                }
                
                if (shouldUpdateUI)
                {
                    // 初回メッセージを削除（WebSocketで初めてデータを受信した場合）
                    var loadingMessage = timelineControl.TimelineContainer.Children.OfType<TextBlock>()
                        .FirstOrDefault(tb => tb.Text == "タイムラインを読み込み中...");
                    if (loadingMessage != null)
                    {
                        timelineControl.TimelineContainer.Children.Remove(loadingMessage);
                    }
                    
                    // ObservableCollectionにも追加
                    _timelineData.Insert(0, container);
                    // UI表示件数制限
                    if (_timelineData.Count > MAX_UI_ITEMS)
                    {
                        Console.WriteLine($"{DEBUG_PREFIX} _timelineData制限: {_timelineData.Count}件 → {MAX_UI_ITEMS}件に削減");
                        _timelineData.RemoveAt(_timelineData.Count - 1);
                        Console.WriteLine($"{DEBUG_PREFIX} _timelineData削除後: {_timelineData.Count}件");
                    }

                    // タイムラインの先頭に追加
                    AddTimelineItem(container, instanceName);
                    // UI表示件数制限
                    if (timelineControl.TimelineContainer.Children.Count > MAX_UI_ITEMS)
                    {
                        Console.WriteLine($"{DEBUG_PREFIX} OnConnectionManager - TimelineContainer制限: {timelineControl.TimelineContainer.Children.Count}件 → {MAX_UI_ITEMS}件に削減");
                        timelineControl.TimelineContainer.Children.RemoveAt(timelineControl.TimelineContainer.Children.Count - 1);
                        Console.WriteLine($"{DEBUG_PREFIX} OnConnectionManager - TimelineContainer削除後: {timelineControl.TimelineContainer.Children.Count}件");
                    }
                    // SOURCEが空の場合は現在のインスタンス名を設定
                    if (string.IsNullOrEmpty(container.SOURCE))
                    {
                        container.SOURCE = instanceName;
                    }
                    
                    AddTimelineItem(container, instanceName);
                    
                    // 詳細パネルに表示
                    var note = new Note { Node = container.ORIGINAL };
                    SetTimelineDetails(container, note);
                    
                    System.Diagnostics.Debug.WriteLine($"UI updated with data from {container.SOURCE}. Timeline children count: {timelineControl.TimelineContainer.Children.Count}");
#if DEBUG
                    Console.WriteLine($"UI updated with data from {container.SOURCE}. Timeline children count: {timelineControl.TimelineContainer.Children.Count}");
#endif
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Data cached but not displayed");
                }
            });
        }
        
        private void SaveToTimelineCacheByType(string instanceName, TimeLineContainer container, int tabIndex)
        {
            // インスタンス別キャッシュを初期化
            if (!_timelineCacheByType.ContainsKey(instanceName))
            {
                _timelineCacheByType[instanceName] = new Dictionary<string, List<TimeLineContainer>>();
            }
            
            // タイムライン種別を現在のタブから判定
            string timelineType = tabIndex switch
            {
                0 => "ソーシャルTL", // 統合TL（ソーシャルTLデータ）
                1 => "ローカルTL",
                2 => "ソーシャルTL",
                3 => "グローバルTL",
                4 => "ホームTL",
                _ => "その他"
            };
#if DEBUG
            Console.WriteLine($"SaveToTimelineCacheByType: tabIndex={tabIndex}, timelineType={timelineType}, instanceName={instanceName}");
#endif
            
            // タイムライン種別ごとのリストを初期化
            if (!_timelineCacheByType[instanceName].ContainsKey(timelineType))
            {
                _timelineCacheByType[instanceName][timelineType] = new List<TimeLineContainer>();
            }
            
            // データを保存
            _timelineCacheByType[instanceName][timelineType].Insert(0, container);
            
            // サーバーごとのサイズ制限（全タイムライン種別の合計）
            ApplyServerCacheLimit(instanceName);
        }
        
        private void SaveToTimelineCacheByTypeIfNotExists(string instanceName, TimeLineContainer container, int tabIndex)
        {
            // インスタンス別キャッシュを初期化
            if (!_timelineCacheByType.ContainsKey(instanceName))
            {
                _timelineCacheByType[instanceName] = new Dictionary<string, List<TimeLineContainer>>();
            }
            
            // タイムライン種別を現在のタブから判定
            string timelineType = tabIndex switch
            {
                0 => "ソーシャルTL", // 統合TL（ソーシャルTLデータ）
                1 => "ローカルTL",
                2 => "ソーシャルTL",
                3 => "グローバルTL",
                4 => "ホームTL",
                _ => "その他"
            };
            
            // タイムライン種別ごとのリストを初期化
            if (!_timelineCacheByType[instanceName].ContainsKey(timelineType))
            {
                _timelineCacheByType[instanceName][timelineType] = new List<TimeLineContainer>();
            }
            
            // 重複チェック：同じDETAILとUPDATEDATの組み合わせがあるかチェック
            var existingItems = _timelineCacheByType[instanceName][timelineType];
            bool isDuplicate = existingItems.Any(item => 
                item.DETAIL == container.DETAIL && 
                item.UPDATEDAT == container.UPDATEDAT &&
                item.USERNAME == container.USERNAME);
            
            if (!isDuplicate)
            {
                // データを保存
                _timelineCacheByType[instanceName][timelineType].Insert(0, container);
                
                // サーバーごとのサイズ制限（全タイムライン種別の合計）
                ApplyServerCacheLimit(instanceName);
                
#if DEBUG
                Console.WriteLine($"Added to cache (no duplicate): {timelineType} - {instanceName}");
#endif
            }
            else
            {
#if DEBUG
                Console.WriteLine($"Skipped duplicate entry: {timelineType} - {instanceName}");
#endif
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
                    var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _cancellationTokenSource!.Token);
                    
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
                        statusBarControl.MainLabel.Text = $"受信エラー: {ex.Message}";
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
                
                if (json?["type"]?.ToString() == "channel" && json?["body"]?["type"]?.ToString() == "note")
                {
                    var noteNode = json["body"]?["body"];
                    if (noteNode == null)
                    {
                        return;
                    }
                    var note = new Note { Node = noteNode };
                    var user = note.User;
                    
                    var username = user?.UserName?.ToString() ?? "unknown";
                    var displayName = user?.Name?.ToString() ?? username;
                    var channel = json["body"]?["id"]?.ToString() ?? "homeTimeline";
                    var selectedInstance = instanceSelectorControl.SelectedServer ?? "misskey.io";
                    
                    // 連合状態を判定（hostがnullならローカル、そうでなければリモート）
                    var isLocal = noteNode?["user"]?["host"] == null;
                    
                    // 公開範囲を判定
                    var visibility = noteNode?["visibility"]?.ToString() ?? "public";
                    
                    // TimeLineContainerを作成（新しいデータ構造に対応）
                    var timelineItem = new TimeLineContainer
                    {
                        IDENTIFIED = noteNode?["id"]?.ToString() ?? Guid.NewGuid().ToString(),
                        USERID = user?.Id?.ToString() ?? "",
                        USERNAME = displayName, // 表示名を使用
                        TLFROM = channel,
                        RENOTED = noteNode?["renoteId"] != null,
                        REPLAYED = noteNode?["replyId"] != null,
                        PROTECTED = StringToProtectedStatus(visibility),
                        ORIGINAL = noteNode ?? JsonNode.Parse("{}")!,
                        DETAIL = note.Text?.ToString() ?? "（内容なし）",
                        CONTENT = note.Text?.ToString() ?? "（内容なし）",
                        UPDATEDAT = noteNode?["createdAt"]?.ToString() ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                        SOURCE = selectedInstance,
                        SOFTWARE = "Misskey",
                        ISLOCAL = isLocal
                    };
                    
                    // UIスレッドで投稿を追加
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        // TimeLineCreatorにデータを追加
                        _timelineCreator.AddTimeLineData(timelineItem);
                        
                        AddTimelineItem(timelineItem, selectedInstance);
                        SetTimelineDetails(timelineItem, note);
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"メッセージ処理エラー: {ex.Message}");
            }
        }
        
        private TimeLineContainer.PROTECTED_STATUS StringToProtectedStatus(string visibility)
        {
            return visibility.ToLower() switch
            {
                "public" => TimeLineContainer.PROTECTED_STATUS.Public,
                "home" => TimeLineContainer.PROTECTED_STATUS.Home,
                "followers" => TimeLineContainer.PROTECTED_STATUS.Follower,
                "specified" => TimeLineContainer.PROTECTED_STATUS.Direct,
                _ => TimeLineContainer.PROTECTED_STATUS.Public
            };
        }

        private void SetTimelineDetails(TimeLineContainer timelineItem, Note note)
        {
            // ユーザー名を表示（username@hostの形式）
            var username = timelineItem.ORIGINAL?["user"]?["username"]?.ToString() ?? "";
            var host = timelineItem.ORIGINAL?["user"]?["host"]?.ToString();
            var userDisplay = string.IsNullOrEmpty(host) ? $"@{username}" : $"@{username}@{host}";
            
            detailPanelControl.UserLabel.Text = userDisplay;
            detailPanelControl.TLFromLabel.Text = $"source: {timelineItem.TLFROM}";
            detailPanelControl.SoftwareLabel.Text = timelineItem.SOFTWARE != "" ? timelineItem.SOFTWARE : "Misskey";
            
            if (DateTime.TryParse(timelineItem.UPDATEDAT, out var timestamp))
            {
                detailPanelControl.UpdatedAtLabel.Text = timestamp.ToString("yyyy/MM/dd HH:mm:ss");
            }
            else
            {
                detailPanelControl.UpdatedAtLabel.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            }
            
            // CWと本文を適切に表示
            if (timelineItem.CW && timelineItem.DETAIL.StartsWith("CW: "))
            {
                // CWの場合は色分けして表示
                var lines = timelineItem.DETAIL.Split('\n');
                var formattedText = new StringBuilder();
                
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i == 0 && lines[i].StartsWith("CW: "))
                    {
                        // CW行はオレンジ色で表示
                        formattedText.AppendLine($"[CW] {lines[i]}");
                    }
                    else
                    {
                        // 本文行は通常の色で表示
                        formattedText.AppendLine(lines[i]);
                    }
                }
                
                detailPanelControl.DetailTextBox.Text = formattedText.ToString().TrimEnd();
            }
            else
            {
                detailPanelControl.DetailTextBox.Text = timelineItem.DETAIL;
            }
        }

        private async void OnConnectionManagerTimeLineDataReceived(object? sender, 
            MiView.Common.Connection.TimeLineDataReceivedEventArgs e)
        {
            lock (_timelineJobQueue)
            {
                // **MEMORY LEAK FIX: Enforce queue size limit**
                if (_timelineJobQueue.Count >= MAX_JOBQUEUE_SIZE)
                {
                    // 古いジョブを削除してメモリ不足を防ぐ
                    var removedJob = _timelineJobQueue.Dequeue();
                    Console.WriteLine($"{DEBUG_PREFIX} JobQueue満杯のため古いジョブ削除: {removedJob.InstanceName}-{removedJob.TimelineType}");
                }
                
                _timelineJobQueue.Enqueue((e.InstanceName, e.TimelineType, e.Container));
            }
            if (!_isJobQueueRunning)
            {
                _isJobQueueRunning = true;
                _ = ProcessTimelineJobQueueAsync();
            }
        }

        private async Task ProcessTimelineJobQueueAsync()
        {
            while (true)
            {
                List<(string InstanceName, string TimelineType, TimeLineContainer Container)> batch = new();
                lock (_timelineJobQueue)
                {
                    while (_timelineJobQueue.Count > 0 && batch.Count < JOBQUEUE_BATCH_SIZE)
                    {
                        batch.Add(_timelineJobQueue.Dequeue());
                    }
                }
                if (batch.Count == 0)
                {
                    _isJobQueueRunning = false;
                    return;
                }
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    foreach (var (instanceName, timelineType, container) in batch)
                    {
                        try
                        {
                            // TimeLineCreatorにデータを追加
                            _timelineCreator.AddTimeLineData(container);
                            // タイムライン種別ごとのキャッシュに保存
                            SaveToTimelineCacheByType(instanceName, container, GetTabIndexFromTimelineType(timelineType));
                            // UI表示の判定
                            bool shouldUpdateUI = ShouldUpdateUI(instanceName, timelineType);
                            if (shouldUpdateUI)
                            {
                                if (string.IsNullOrEmpty(container.SOURCE))
                                {
                                    container.SOURCE = instanceName;
                                }
                                if (_selectedTabIndex == 0)
                                {
                                    // 統合TLの場合は特別処理
                                }
                                AddTimelineItem(container, instanceName);
                                var note = new Note { Node = container.ORIGINAL ?? System.Text.Json.Nodes.JsonNode.Parse("{}")! };
                                SetTimelineDetails(container, note);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{ERROR_PREFIX} ジョブ処理エラー [{instanceName}-{timelineType}]: {ex.Message}");
                        }
                    }
                    
                    // **MEMORY LEAK FIX: Clear batch to ensure garbage collection**
                    batch.Clear();
                });
                await Task.Delay(JOBQUEUE_INTERVAL_MS);
            }
        }

        private void OnConnectionStatusChanged(object? sender, 
            MiView.Common.Connection.ConnectionStatusChangedEventArgs e)
        {
            Console.WriteLine($"OnConnectionStatusChanged: {e.InstanceName} - {e.Status}");
            
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                if (e.Status == "Connected")
                {
                    // 接続済みインスタンスリストに追加（重複チェック付き）
                    if (!_connectedInstances.Contains(e.InstanceName))
                    {
                        _connectedInstances.Add(e.InstanceName);
                        Console.WriteLine($"Added {e.InstanceName} to connected instances. Total: {_connectedInstances.Count}");
                    }
                    else
                    {
                        Console.WriteLine($"{e.InstanceName} already in connected instances. Total: {_connectedInstances.Count}");
                    }
                    
                    // 接続ボタンのテキストを更新
                    if (_connectedInstances.Count == 1)
                    {
                        instanceSelectorControl.SetConnectButtonText("切断");
                    }
                    else
                    {
                        instanceSelectorControl.SetConnectButtonText($"切断 ({_connectedInstances.Count}接続中)");
                    }
                    
                    // 現在選択中のインスタンスの接続状態を更新
                    var currentInstance = GetCurrentInstanceUrl();
                    if (currentInstance == e.InstanceName)
                    {
                        instanceSelectorControl.UpdateConnectButtonState(currentInstance, true);
                    }
                    
                    statusBarControl.MainLabel.Text = $"接続成功: {e.InstanceName} ({_connectedInstances.Count}接続中)";
                    Console.WriteLine($"UI updated: {_connectedInstances.Count} connections");
                }
                else if (e.Status == "Disconnected")
                {
                    // 接続済みインスタンスリストから削除
                    _connectedInstances.Remove(e.InstanceName);
                    Console.WriteLine($"Removed {e.InstanceName} from connected instances. Total: {_connectedInstances.Count}");
                    
                    // 接続ボタンのテキストを更新
                    if (_connectedInstances.Count == 0)
                    {
                        instanceSelectorControl.SetConnectButtonText("接続");
                    }
                    else
                    {
                        instanceSelectorControl.SetConnectButtonText($"切断 ({_connectedInstances.Count}接続中)");
                    }
                    
                    // 現在選択中のインスタンスの接続状態を更新
                    var currentInstance = GetCurrentInstanceUrl();
                    if (currentInstance == e.InstanceName)
                    {
                        instanceSelectorControl.UpdateConnectButtonState(currentInstance, false);
                    }
                    
                    statusBarControl.MainLabel.Text = $"切断: {e.InstanceName} ({_connectedInstances.Count}接続中)";
                }
            });
        }

        private bool ShouldUpdateUI(string instanceName, string timelineType)
        {
            // 統合TL表示中の場合 - 全インスタンスのソーシャルTLデータのみを表示
            if (_selectedTabIndex == 0)
            {
                // 統合TLでは、接続中の全インスタンスのソーシャルTLデータのみを表示
                var shouldUpdate = timelineType == "ソーシャルTL" && _connectedInstances.Contains(instanceName);
#if DEBUG
                Console.WriteLine($"ShouldUpdateUI check: instance={instanceName}, timelineType={timelineType}, selectedTab={_selectedTabIndex}, shouldUpdate={shouldUpdate}");
                Console.WriteLine($"Connected instances: {string.Join(", ", _connectedInstances)}");
#endif
                return shouldUpdate;
            }
            
            // 個別タイムライン表示の場合 - 現在選択中のインスタンスの該当タイムラインのみ表示
            var currentInstance = GetCurrentInstanceUrl();
            var currentTimelineType = GetTimelineTypeFromTabIndex(_selectedTabIndex);
            
            var result = instanceName == currentInstance && timelineType == currentTimelineType;
#if DEBUG
            Console.WriteLine($"ShouldUpdateUI check: instance={instanceName}, timelineType={timelineType}, currentInstance={currentInstance}, currentTimelineType={currentTimelineType}, result={result}");
#endif
            return result;
        }

        private int GetTabIndexFromTimelineType(string timelineType)
        {
            return timelineType switch
            {
                "統合TL" => 0,
                "ローカルTL" => 1,
                "ソーシャルTL" => 2,
                "グローバルTL" => 3,
                "ホームTL" => 4,
                _ => 0
            };
        }

        private string GetTimelineTypeFromTabIndex(int tabIndex)
        {
            return tabIndex switch
            {
                0 => "統合TL",
                1 => "ローカルTL",
                2 => "ソーシャルTL",
                3 => "グローバルTL",
                4 => "ホームTL",
                _ => "統合TL"
            };
        }

        private void StartAllPersistentConnections()
        {
            // UIを先に表示し、接続は非同期で続行
            Console.WriteLine($"Starting persistent connections for {_instances.Count} instances: {string.Join(", ", _instances)}");
            
            Console.WriteLine($"Available API keys: {_instanceTokens.Count}");
            foreach (var kvp in _instanceTokens)
            {
                Console.WriteLine($"  {kvp.Key}: [HIDDEN]");
            }
            
            // 全てのインスタンスを同時に接続（並列度制限付き）
            var connectionTasks = new List<Task>();
            var semaphore = new SemaphoreSlim(3, 3); // 最大3つのインスタンスを同時接続
            
            foreach (var instanceName in _instances)
            {
                var task = Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        Console.WriteLine($"Starting connection to instance: {instanceName}");
                        
                        // ステータスバーに接続開始を表示
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            statusBarControl.MainLabel.Text = $"接続中: {instanceName}...";
                        });
                        
                        var apiKey = _instanceTokens.ContainsKey(instanceName) ? _instanceTokens[instanceName] : null;
                        Console.WriteLine($"API key for {instanceName}: {(apiKey != null ? "Found" : "Not found")}");
                        if (apiKey != null)
                        {
                            Console.WriteLine($"API key: [HIDDEN]");
                        }
                        await _connectionManager.ConnectPersistentInstance(instanceName, apiKey);
                        Console.WriteLine($"Connection attempt completed for: {instanceName}");
                        
                        // ステータスバーに接続完了を表示
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            statusBarControl.MainLabel.Text = $"接続完了: {instanceName}";
                            // 接続状態の更新はOnConnectionStatusChangedで行うため、ここでは行わない
                        });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Connection failed for {instanceName}: {ex.Message}");
                        
                        // ステータスバーにエラーを表示
                        await Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            statusBarControl.MainLabel.Text = $"接続失敗: {instanceName} - {ex.Message}";
                        });
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                connectionTasks.Add(task);
            }
            
            // 接続タスクを非同期で実行（UIを待たせない）
            Task.Run(async () =>
            {
                // 全ての接続タスクを同時実行
                await Task.WhenAll(connectionTasks);
                
                Console.WriteLine("All persistent connection attempts completed");
                
                // 接続状態をデバッグ出力
                _connectionManager.DebugConnectionStatus();
                
                // 最終的な接続状態をステータスバーに表示
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    var connectedCount = _connectedInstances.Count;
                    if (connectedCount > 0)
                    {
                        statusBarControl.MainLabel.Text = $"接続完了: {connectedCount}個のサーバーに接続中";
                    }
                    else
                    {
                        statusBarControl.MainLabel.Text = "接続に失敗しました";
                    }
                });
            });
        }

        private async Task DisconnectWebSocket(bool isUserInitiated = true)
        {
            // 統合TLの接続を切断（WebSocketConnectionManagerに統一管理）
#if DEBUG
            Console.WriteLine($"🔄 UNIFIED TL DISCONNECT: Using ConnectionManager for unified timeline disconnect");
#endif
            
            if (_webSocketTimeLine != null)
            {
                // ユーザー操作による切断かどうかを設定
                _webSocketTimeLine.SetUserInitiatedDisconnect(isUserInitiated);
                
                _webSocketTimeLine.TimeLineDataReceived -= OnTimeLineDataReceived;
                // ConnectionAbortはprotectedなので直接呼び出せない
                // 代わりに、WebSocketの状態を確認してクローズする
                var socket = _webSocketTimeLine.GetSocketClient();
                if (socket != null && socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnect", CancellationToken.None);
                }
                _webSocketTimeLine = null;
            }
            
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

        private async Task DisconnectInstance(string instanceUrl, bool isUserInitiated = true)
        {
#if DEBUG
            Console.WriteLine($"DisconnectInstance called for: {instanceUrl}, isUserInitiated: {isUserInitiated}");
            Console.WriteLine($"Before disconnect - Connected instances: {string.Join(", ", _connectedInstances)}");
#endif
            
            // WebSocketConnectionManagerを使用して切断
            await _connectionManager.DisconnectInstance(instanceUrl, isUserInitiated);
            
            // 接続済みインスタンスリストから削除
            _connectedInstances.Remove(instanceUrl);
            
#if DEBUG
            Console.WriteLine($"After disconnect - Connected instances: {string.Join(", ", _connectedInstances)}");
#endif
            
            // 接続ボタンのテキストを更新
            if (_connectedInstances.Count == 0)
            {
                instanceSelectorControl.ConnectButton.Content = "接続";
#if DEBUG
                Console.WriteLine("All connections disconnected, button set to '接続'");
#endif
            }
            else
            {
                instanceSelectorControl.ConnectButton.Content = $"切断 ({_connectedInstances.Count}接続中)";
#if DEBUG
                Console.WriteLine($"Some connections remain, button set to '切断 ({_connectedInstances.Count}接続中)'");
#endif
            }
        }

        private async Task DisconnectAllConnections(bool isUserInitiated = true)
        {
            // WebSocketConnectionManagerを使用して全ての接続を切断
            await _connectionManager.DisconnectAll(isUserInitiated);
            
            // 接続済みインスタンスリストをクリア
            _connectedInstances.Clear();
            
            // 接続ボタンのテキストを更新
            instanceSelectorControl.ConnectButton.Content = "接続";
        }

        private async void OnInstanceSelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            var selectedInstance = instanceSelectorControl.SelectedServer;
            Console.WriteLine($"OnInstanceSelectionChanged called with: {selectedInstance}");
            
            if (!string.IsNullOrEmpty(selectedInstance))
            {
                statusBarControl.MainLabel.Text = $"サーバー {selectedInstance} を選択しました。";
                Console.WriteLine($"Selected instance: {selectedInstance}");
                
                // 接続状態に応じてボタンのテキストを更新
                var isConnected = _connectedInstances.Contains(selectedInstance);
                instanceSelectorControl.UpdateConnectButtonState(selectedInstance, isConnected);
                
                // タブを更新
                UpdateTabs(selectedInstance);
                
                // 初期選択時は総合TL（インデックス0）を選択
                if (_selectedTabIndex == 0)
                {
                    // 総合TLが選択されていることを明示的に設定
                    _selectedTabIndex = 0;
                    UpdateTabSelection(0);
                    Console.WriteLine($"Set initial tab selection to 0");
                }
                
                // 現在のタブに応じてタイムラインを切り替え（WebSocketは切断しない）
                await SwitchTab(selectedInstance, _selectedTabIndex);
            }
            else
            {
                Console.WriteLine("Selected instance is null or empty");
            }
        }

        private void UpdateTabs(string instanceUrl)
        {
            Console.WriteLine($"UpdateTabs called for: {instanceUrl}");
            Console.WriteLine($"Available server tabs: {string.Join(", ", _serverTabs.Keys)}");
            
            if (_serverTabs.ContainsKey(instanceUrl))
            {
                Console.WriteLine($"Found tabs for {instanceUrl}: {string.Join(", ", _serverTabs[instanceUrl])}");
                
                // タブコンテナを取得（TimelineControlから直接取得）
                var tabContainer = timelineControl.TabContainer;
                var tabScrollViewer = timelineControl.TabScrollViewer;
                if (tabContainer == null)
                {
                    Console.WriteLine("Tab container is null from TimelineControl");
                    return;
                }
                else
                {
                    Console.WriteLine("Existing tab container found from TimelineControl");
                }
                
                if (tabContainer != null)
                {
                    tabContainer.Children.Clear();
                    Console.WriteLine("Cleared tab container children");
                    
                    var tabs = _serverTabs[instanceUrl];
                    Console.WriteLine($"Creating {tabs.Count} tabs: {string.Join(", ", tabs)}");
                    
                    for (int i = 0; i < tabs.Count; i++)
                    {
                        var tabName = tabs[i];
                        var isSelected = i == _selectedTabIndex; // 現在選択中のタブを選択状態に
                        
                        Console.WriteLine($"Creating tab {i}: {tabName} (selected: {isSelected})");
                        
                        var tabBorder = new Border
                        {
                            Background = isSelected ? Avalonia.Media.Brushes.White : Avalonia.Media.Brush.Parse("#F0F0F0"),
                            BorderBrush = Avalonia.Media.Brush.Parse("#8C8C8C"),
                            BorderThickness = new Avalonia.Thickness(1, 0, 1, 1),
                            Padding = new Avalonia.Thickness(8, 2),
                            Cursor = new Avalonia.Input.Cursor(Avalonia.Input.StandardCursorType.Hand)
                        };
                        
                        var tabText = new TextBlock
                        {
                            Text = tabName,
                            FontSize = 11,
                            Foreground = Avalonia.Media.Brushes.Black,
                            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                        };
                        
                        tabBorder.Child = tabText;
                        
                        // クリックイベントを追加
                        var tabIndex = i;
                        tabBorder.PointerPressed += async (sender, e) =>
                        {
                            // 同じタブがクリックされた場合は何もしない
                            if (tabIndex == _selectedTabIndex)
                                return;
                                
                            _selectedTabIndex = tabIndex;
                            await SwitchTab(instanceUrl, tabIndex);
                        };
                        
                        tabContainer.Children.Add(tabBorder);
                        Console.WriteLine($"Added tab {i}: {tabName}");
                        
                        // タブ間のスペース
                        if (i < tabs.Count - 1)
                        {
                            tabContainer.Children.Add(new Border { Background = Avalonia.Media.Brush.Parse("#F0F0F0"), Width = 2 });
                        }
                    }
                    
                    Console.WriteLine($"Total tab container children: {tabContainer.Children.Count}");
                }
                else
                {
                    Console.WriteLine("Tab container is null, cannot create tabs");
                }
            }
            else
            {
                Console.WriteLine($"No tabs found for instance: {instanceUrl}");
            }
        }
        
        private async Task SwitchTab(string instanceUrl, int tabIndex)
        {
            try
            {
                // 選択されたタブインデックスを更新（重要！）
                _selectedTabIndex = tabIndex;
                Console.WriteLine($"SwitchTab: Updated _selectedTabIndex to {tabIndex}");
                
                // タブの選択状態を即座に更新（UI応答性向上）
                UpdateTabSelection(tabIndex);
                
                var tabName = _serverTabs[instanceUrl][tabIndex];
                statusBarControl.MainLabel.Text = $"切り替え中: {instanceUrl} - {tabName}";
                
                // タイムラインをクリア（UIスレッドで実行）
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    timelineControl.TimelineContainer.Children.Clear();
                    _timelineItems.Clear();
                    _timelineData.Clear();
                            
                    // タイムライン種別ごとのキャッシュからデータを復元
                    var cacheTabName = tabIndex switch
                    {
                        0 => "ソーシャルTL", // 統合TLはソーシャルTLデータを使用
                        1 => "ローカルTL", 
                        2 => "ソーシャルTL",
                        3 => "グローバルTL",
                        4 => "ホームTL",
                        _ => "ソーシャルTL"
                    };
                    
                    // 統合TLの場合は特別処理
                    if (tabIndex == 0)
                    {
                        Console.WriteLine($"Loading unified timeline data from {_connectedInstances.Count} connected instances");
                        
                        // 接続中の全インスタンスのソーシャルTLデータを統合表示
                        var allSocialData = new List<TimeLineContainer>();
                        foreach (var instance in _connectedInstances)
                        {
                            Console.WriteLine($"Checking cache for instance: {instance}");
                            if (_timelineCacheByType.ContainsKey(instance) && 
                                _timelineCacheByType[instance].ContainsKey("ソーシャルTL"))
                            {
                                var socialData = _timelineCacheByType[instance]["ソーシャルTL"];
                                Console.WriteLine($"Found {socialData.Count} social timeline items for {instance}");
                                allSocialData.AddRange(socialData);
                            }
                            else
                            {
                                Console.WriteLine($"No social timeline cache found for {instance}");
                            }
                        }
                        
                        Console.WriteLine($"Total social timeline items found: {allSocialData.Count}");
                        
                        // 時系列でソート
                        allSocialData = allSocialData.OrderByDescending(x => x.UPDATEDAT).Take(MAX_CACHED_ITEMS).ToList();
                        
                        foreach (var item in allSocialData)
                        {
                            _timelineData.Add(item);
                        }
                        foreach (var item in allSocialData.AsEnumerable().Reverse())
                        {
                            AddTimelineItem(item, item.SOURCE);
                        }
                        
                        Console.WriteLine($"Unified timeline loaded with {allSocialData.Count} items");
                    }
                    else
                    {
                        // 個別タイムライン表示はWebSocketConnectionManagerで管理されている
                        
                        // 個別タイムライン表示
                        if (_timelineCacheByType.ContainsKey(instanceUrl) && 
                            _timelineCacheByType[instanceUrl].ContainsKey(cacheTabName))
                        {
                            var cachedItems = _timelineCacheByType[instanceUrl][cacheTabName];
                            foreach (var item in cachedItems)
                            {
                                _timelineData.Add(item);
                            }
                            foreach (var item in cachedItems.AsEnumerable().Reverse())
                            {
                                AddTimelineItem(item, item.SOURCE);
                            }
                        }
                        else
                        {
                            // キャッシュがない場合はサンプルデータを表示
                            AddTestTimelineItems();
                        }
                    }
                    
                    UpdateStatusBarInfo();
                    
                    // 統合TLの場合は特別なメッセージを表示
                    if (tabIndex == 0)
                    {
                        var connectedInstanceCount = _connectedInstances.Count;
                        statusBarControl.MainLabel.Text = $"統合TL表示中: {connectedInstanceCount}個のインスタンスから統合";
                    }
                    else
                    {
                        statusBarControl.MainLabel.Text = $"表示切替完了: {instanceUrl} - {tabName}";
                    }
                });
            }
            catch (Exception ex)
            {
                statusBarControl.MainLabel.Text = $"タブ切り替えエラー: {ex.Message}";
            }
        }
        
        private void UpdateTabSelection(int selectedIndex)
        {
            var tabContainer = timelineControl.TabContainer;
            if (tabContainer != null)
            {
                int tabCount = 0;
                foreach (var child in tabContainer.Children)
                {
                    if (child is Border border && border.Child is TextBlock)
                    {
                        border.Background = tabCount == selectedIndex ? Avalonia.Media.Brushes.White : Avalonia.Media.Brush.Parse("#F0F0F0");
                        tabCount++;
                    }
                }
                Console.WriteLine($"Updated tab selection: {selectedIndex} (total tabs: {tabCount})");
            }
            else
            {
                Console.WriteLine("Tab container is null in UpdateTabSelection");
            }
        }

        private void LoadSettings()
        {
            try
            {
                if (File.Exists(SETTINGS_FILE))
                {
                    var json = File.ReadAllText(SETTINGS_FILE);
                    Console.WriteLine($"Loading settings from: {SETTINGS_FILE}");
                    Console.WriteLine($"Settings JSON: {json}");
                    
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    
                    if (settings != null)
                    {
                        Console.WriteLine($"Settings loaded successfully");
                        Console.WriteLine($"Instances count: {settings.Instances?.Count ?? 0}");
                        Console.WriteLine($"InstanceTokens count: {settings.InstanceTokens?.Count ?? 0}");
                        
                        // インスタンスを復元
                        foreach (var instance in settings.Instances)
                        {
                            _instances.Add(instance);
                            Console.WriteLine($"Added instance: {instance}");
                        }
                        
                        // APIキーを復元
                        foreach (var token in settings.InstanceTokens)
                        {
                            _instanceTokens[token.Key] = token.Value;
                            Console.WriteLine($"Added API key for {token.Key}: [HIDDEN]");
                        }
                        
                        Console.WriteLine($"Total instances loaded: {_instances.Count}");
                        Console.WriteLine($"Total API keys loaded: {_instanceTokens.Count}");
                        
                        // サーバータブを復元（標準的なタイムラインのみ、独自TLは除外）
                        foreach (var instance in settings.Instances)
                        {
                            var serverTabs = DEFAULT_TABS.ToList();
                            _serverTabs[instance] = serverTabs;
                            Console.WriteLine($"Tabs initialized for {instance}: {string.Join(", ", serverTabs)}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Settings deserialization returned null");
                    }
                }
                else
                {
                    Console.WriteLine($"Settings file not found: {SETTINGS_FILE}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex.Message}");
                Console.WriteLine($"Exception details: {ex}");
                statusBarControl.MainLabel.Text = $"設定読み込みエラー: {ex.Message}";
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
            }
            catch (Exception ex)
            {
                statusBarControl.MainLabel.Text = $"設定保存エラー: {ex.Message}";
            }
        }

        private string GetCacheKey(string instanceUrl, int tabIndex)
        {
            return $"{instanceUrl}_{tabIndex}";
        }
        
        private string GetCurrentInstanceUrl()
        {
            return instanceSelectorControl.SelectedServer ?? "";
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
        
        private string AddProtocolToUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;
                
            // プロトコルがない場合はhttps://を追加
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                return "https://" + url;
            }
            
            return url;
        }

        private void StartCacheCleanupTimer()
        {
            _cacheCleanupTimer = new Timer(CleanupOldCacheData, null, 
                TimeSpan.FromMinutes(CACHE_CLEANUP_INTERVAL_MINUTES), 
                TimeSpan.FromMinutes(CACHE_CLEANUP_INTERVAL_MINUTES));
        }

        private void CleanupOldCacheData(object? state)
        {
            try
            {
                Console.WriteLine($"{DEBUG_PREFIX} キャッシュクリーンアップ開始");
                var cutoffTime = DateTime.Now.AddMinutes(-CACHE_CLEANUP_AGE_MINUTES);
                
                // _timelineCache のクリーンアップ（統合TLデータは保護）
                foreach (var cacheKey in _timelineCache.Keys)
                {
                    // **統合TL絶対保護: 統合TLキャッシュは絶対にクリーンアップしない**
                    if (cacheKey.StartsWith("unified_tl"))
                    {
#if DEBUG
                        Console.WriteLine($"🛡️ UNIFIED TL PROTECTION: Skipping cleanup for unified TL cache: {cacheKey}");
#endif
                        continue; // 統合TLキャッシュのクリーンアップをスキップ
                    }
                    
                    var cache = _timelineCache[cacheKey];
                    if (cache.Count > CACHE_CLEANUP_MIN_ITEMS)
                    {
                        var beforeCount = cache.Count;
                        var removedCount = 0;
                        
                        for (int i = cache.Count - 1; i >= 0; i--)
                        {
                            if (DateTime.TryParse(cache[i].UPDATEDAT, out var updateTime))
                            {
                                if (updateTime < cutoffTime)
                                {
                                    cache.RemoveAt(i);
                                    removedCount++;
                                }
                            }
                        }
                        
                        if (removedCount > 0)
                        {
                            Console.WriteLine($"{DEBUG_PREFIX} クリーンアップ[{cacheKey}]: {beforeCount}件 → {cache.Count}件 ({removedCount}件削除)");
                        }
                    }
                }
                
                // _timelineCacheByType のクリーンアップ（統合TL用データは保護）
                foreach (var instanceName in _timelineCacheByType.Keys)
                {
                    foreach (var timelineType in _timelineCacheByType[instanceName].Keys)
                    {
                        // **統合TL絶対保護: ソーシャルTLデータは絶対にクリーンアップしない**
                        if (timelineType == "ソーシャルTL")
                        {
#if DEBUG
                            Console.WriteLine($"🛡️ UNIFIED TL PROTECTION: Skipping cleanup for {instanceName} - {timelineType} cache");
#endif
                            continue; // ソーシャルTLデータのクリーンアップをスキップ
                        }
                        
                        var cache = _timelineCacheByType[instanceName][timelineType];
                        if (cache.Count > CACHE_CLEANUP_MIN_ITEMS)
                        {
                            var beforeCount = cache.Count;
                            var removedCount = 0;
                            
                            for (int i = cache.Count - 1; i >= 0; i--)
                            {
                                if (DateTime.TryParse(cache[i].UPDATEDAT, out var updateTime))
                                {
                                    if (updateTime < cutoffTime)
                                    {
                                        cache.RemoveAt(i);
                                        removedCount++;
                                    }
                                }
                            }
                            
                            if (removedCount > 0)
                            {
                                Console.WriteLine($"{DEBUG_PREFIX} クリーンアップ[{instanceName}-{timelineType}]: {beforeCount}件 → {cache.Count}件 ({removedCount}件削除)");
                            }
                        }
                    }
                }
                
                // _timelineData のクリーンアップ（最重要）
                if (_timelineData.Count > CACHE_CLEANUP_MIN_ITEMS)
                {
                    var beforeCount = _timelineData.Count;
                    var removedCount = 0;
                    
                    for (int i = _timelineData.Count - 1; i >= 0; i--)
                    {
                        if (DateTime.TryParse(_timelineData[i].UPDATEDAT, out var updateTime))
                        {
                            if (updateTime < cutoffTime)
                            {
                                _timelineData.RemoveAt(i);
                                removedCount++;
                            }
                        }
                    }
                    
                    if (removedCount > 0)
                    {
                        Console.WriteLine($"{DEBUG_PREFIX} クリーンアップ[_timelineData]: {beforeCount}件 → {_timelineData.Count}件 ({removedCount}件削除)");
                        
                        // UIからも古いアイテムを削除
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            while (_timelineItems.Count > _timelineData.Count)
                            {
                                if (_timelineItems.Count > 0)
                                {
                                    timelineControl.TimelineContainer.Children.RemoveAt(timelineControl.TimelineContainer.Children.Count - 1);
                                    _timelineItems.RemoveAt(_timelineItems.Count - 1);
                                }
                            }
                        });
                    }
                }
                
                // **MEMORY LEAK FIX: Cleanup other components (WebSocket connections PROTECTED)**
                try
                {
                    // **PRIORITY 1: WebSocket接続の状況確認（切断は絶対にしない）**
                    
                    // **PRIORITY 2: WebSocket保護のため他のメモリ使用量を積極的に削減**
                    
                    // JobQueueの積極的クリーンアップ
                    lock (_timelineJobQueue)
                    {
                        if (_timelineJobQueue.Count > MAX_JOBQUEUE_SIZE / 4) // 25%で早期警告
                        {
                            Console.WriteLine($"{DEBUG_PREFIX} WebSocket保護のためJobQueue早期クリーンアップ: {_timelineJobQueue.Count}/{MAX_JOBQUEUE_SIZE}");
                            
                            // WebSocket保護のため、JobQueueを積極的に削減（統合TLジョブは保護）
                            var tempQueue = new Queue<(string InstanceName, string TimelineType, TimeLineContainer Container)>();
                            
                            // **統合TL絶対保護: ソーシャルTLジョブを優先保持**
                            while (_timelineJobQueue.Count > 0)
                            {
                                var job = _timelineJobQueue.Dequeue();
                                
                                // ソーシャルTLジョブまたは新しいジョブは保持
                                if (job.TimelineType == "ソーシャルTL" || tempQueue.Count < MAX_JOBQUEUE_SIZE / 2)
                                {
                                    tempQueue.Enqueue(job);
                                }
                                else
                                {
#if DEBUG
                                    Console.WriteLine($"🛡️ UNIFIED TL PROTECTION: Keeping ソーシャルTL job from {job.InstanceName}");
#endif
                                }
                            }
                            
                            // JobQueueを再構築
                            while (tempQueue.Count > 0)
                            {
                                _timelineJobQueue.Enqueue(tempQueue.Dequeue());
                            }
                            Console.WriteLine($"{DEBUG_PREFIX} JobQueue削減完了: {_timelineJobQueue.Count}件に削減");
                        }
                    }
                    
                    // **PRIORITY 3: UI要素の積極的削減でWebSocket保護**
                    if (_timelineItems.Count > MAX_UI_ITEMS / 2)
                    {
                        Console.WriteLine($"{DEBUG_PREFIX} WebSocket保護のためUI要素削減: {_timelineItems.Count}→{MAX_UI_ITEMS/2}");
                        var itemsToRemove = _timelineItems.Count - MAX_UI_ITEMS / 2;
                        for (int i = 0; i < itemsToRemove && _timelineItems.Count > 0; i++)
                        {
                            _timelineItems.RemoveAt(_timelineItems.Count - 1);
                        }
                    }
                    
                    // **強化されたガベージコレクション（WebSocket保護のため）**
                    GC.Collect(2, GCCollectionMode.Aggressive); // より積極的なGC
                    GC.WaitForPendingFinalizers();
                    GC.Collect(2, GCCollectionMode.Aggressive);
                    
                    Console.WriteLine($"{DEBUG_PREFIX} WebSocket保護のためのメモリ最適化完了");
                }
                catch (Exception cleanupEx)
                {
                    Console.WriteLine($"{ERROR_PREFIX} 追加クリーンアップエラー: {cleanupEx.Message}");
                    Console.WriteLine($"{DEBUG_PREFIX} エラーが発生してもWebSocket接続は保護されています");
                }
                
                Console.WriteLine($"{DEBUG_PREFIX} キャッシュクリーンアップ完了");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ERROR_PREFIX} キャッシュクリーンアップエラー: {ex.Message}");
            }
        }

        private void ApplyServerCacheLimit(string instanceName)
        {
            if (!_timelineCacheByType.ContainsKey(instanceName))
                return;

            // サーバー全体のキャッシュ件数を計算
            var totalCount = _timelineCacheByType[instanceName].Values.Sum(list => list.Count);
            
            if (totalCount > MAX_CACHED_ITEMS)
            {
                Console.WriteLine($"{DEBUG_PREFIX} サーバー[{instanceName}]キャッシュ制限: {totalCount}件 → {MAX_CACHED_ITEMS}件に削減");
                
                // 古いアイテムから削除（各タイムラインから最古のものを順次削除）
                while (_timelineCacheByType[instanceName].Values.Sum(list => list.Count) > MAX_CACHED_ITEMS)
                {
                    // 最も多くのアイテムを持つタイムラインから削除
                    var maxTimeline = _timelineCacheByType[instanceName]
                        .Where(kvp => kvp.Value.Count > 0)
                        .OrderByDescending(kvp => kvp.Value.Count)
                        .FirstOrDefault();
                    
                    if (maxTimeline.Value != null && maxTimeline.Value.Count > 0)
                    {
                        maxTimeline.Value.RemoveAt(maxTimeline.Value.Count - 1);
                    }
                    else
                    {
                        break; // 削除するアイテムがない場合は終了
                    }
                }
                
                var newTotal = _timelineCacheByType[instanceName].Values.Sum(list => list.Count);
                Console.WriteLine($"{DEBUG_PREFIX} サーバー[{instanceName}]キャッシュ削除後: {newTotal}件");
            }
        }

        private void UpdateStatusBarInfo()
        {
#if DEBUG
            // 各キャッシュの詳細情報を収集
            var cacheDetails = new List<string>();
            
            // UI表示件数
            cacheDetails.Add($"UI:{_timelineItems.Count}");
            
            // ObservableCollection件数
            cacheDetails.Add($"Data:{_timelineData.Count}");
            
            // TimelineContainer RAW件数（初期ダミー500件を除く）
            var rawCount = Math.Max(0, timelineControl.TimelineContainer.Children.Count - MAX_UI_ITEMS);
            cacheDetails.Add($"RAW:{rawCount}");
            
            // _timelineCache の詳細
            foreach (var kvp in _timelineCache)
            {
                cacheDetails.Add($"{kvp.Key}:{kvp.Value.Count}");
            }
            
            // _timelineCacheByType の詳細（インスタンスごとに合計）
            foreach (var instanceKvp in _timelineCacheByType)
            {
                var instanceTotal = instanceKvp.Value.Values.Sum(list => list.Count);
                var shortInstance = instanceKvp.Key.Length > 12 ? instanceKvp.Key.Substring(0, 12) + "..." : instanceKvp.Key;
                cacheDetails.Add($"{shortInstance}:{instanceTotal}");
            }
            
            statusBarControl.MemoryInfoLabel.Text = $"キャッシュ: {string.Join(",", cacheDetails)}";
            
            // メモリ使用量を表示
            var memoryMB = GC.GetTotalMemory(false) / (1024 * 1024);
            statusBarControl.MemoryUsageLabel.Text = $"メモリ: {memoryMB}MB";
#else
            // リリースビルドでは基本情報のみ
            statusBarControl.MemoryInfoLabel.Text = $"キャッシュ: {_timelineItems.Count + _timelineData.Count}件";
            statusBarControl.MemoryUsageLabel.Text = "";
#endif
        }

        protected override void OnClosed(EventArgs e)
        {
            // UIを先に閉じる
            Console.WriteLine("Application closing - UI will close first");
            
            // **MEMORY LEAK FIX: Unsubscribe all event handlers**
            if (_connectionManager != null)
            {
                _connectionManager.TimeLineDataReceived -= OnConnectionManagerTimeLineDataReceived;
                _connectionManager.ConnectionStatusChanged -= OnConnectionStatusChanged;
            }
            
            if (instanceSelectorControl != null)
            {
                instanceSelectorControl.ConnectButtonClicked -= cmdConnect_Click;
                instanceSelectorControl.AddInstanceButtonClicked -= ShowAddInstanceDialog;
                instanceSelectorControl.ServerTabSelected -= OnServerTabSelected;
            }
            
            if (menuBarControl != null)
            {
                menuBarControl.ServerManagementRequested -= ShowServerManagement;
                menuBarControl.ServerAddRequested -= ShowAddInstanceDialog;
                menuBarControl.ExitRequested -= OnExitRequested;
#if DEBUG
                menuBarControl.GenerateDummyDataRequested -= GenerateDummyData;
#endif
            }
            
            // **MEMORY LEAK FIX: Cancel and dispose CancellationTokenSource**
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
            
            // **MEMORY LEAK FIX: Dispose WebSocket**
            _webSocket?.Dispose();
            _webSocket = null;
            
            // キャッシュクリーンアップタイマーを停止
            _cacheCleanupTimer?.Dispose();
            
            // **MEMORY LEAK FIX: Clear job queue on exit**
            lock (_timelineJobQueue)
            {
                _timelineJobQueue.Clear();
                Console.WriteLine($"{DEBUG_PREFIX} JobQueue cleared on exit");
            }
            
            // 設定を保存
            SaveSettings();
            
            // WebSocket接続は非同期で後から閉じる（タイムアウト付き）
            Task.Run(async () =>
            {
                try
                {
                    Console.WriteLine("Starting async WebSocket cleanup...");
                    
                    // タイムアウト付きでWebSocket切断を実行
                    var cleanupTask = Task.Run(async () =>
                    {
                        // 既存のWebSocket接続を切断
                        await DisconnectWebSocket(true);
                        
                        // WebSocketConnectionManagerの接続を切断
                        await _connectionManager?.DisconnectAll(true);
                    });
                    
                    var timeoutTask = Task.Delay(TimeSpan.FromMinutes(10)); // 10分タイムアウト
                    var completedTask = await Task.WhenAny(cleanupTask, timeoutTask);
                    
                    if (completedTask == timeoutTask)
                    {
                        Console.WriteLine("WebSocket cleanup timed out, forcing cleanup");
                    }
                    else
                    {
                        await cleanupTask; // 例外があれば再スロー
                    }
                    
                    Console.WriteLine("WebSocket cleanup completed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during WebSocket cleanup: {ex.Message}");
                }
                finally
                {
                    _connectionManager?.Dispose();
                    Console.WriteLine("Connection manager disposed");
                }
            });
            
            base.OnClosed(e);
        }

        /// <summary>
        /// **MEMORY LEAK FIX: Named method to replace lambda expression**
        /// </summary>
        private void OnExitRequested(object? sender, RoutedEventArgs e)
        {
            Close();
        }
        
        /// <summary>
        /// **MEMORY OPTIMIZATION: Pre-allocate memory for better performance**
        /// </summary>
        private void PreAllocateMemory()
        {
            try
            {
                Console.WriteLine($"{DEBUG_PREFIX} メモリプリアロケーション開始");
                
                // **コレクション容量の事前確保**
                // Note: 既存のフィールド初期化は変更せず、容量のみ確保
                
                // **辞書の初期容量確保（既存初期化を保持）**
                if (_timelineCache.Count == 0)
                {
                    // 容量確保のため一時的なエントリを追加・削除
                    for (int i = 0; i < 20; i++)
                    {
                        _timelineCache[$"temp_{i}"] = new List<TimeLineContainer>();
                    }
                    _timelineCache.Clear();
                }
                
                // **文字列プールの事前作成（頻用文字列を事前生成）**
                var commonStrings = new[]
                {
                    "統合TL", "ホームTL", "ローカルTL", "ソーシャルTL", "グローバルTL",
                    "Misskey", "Mastodon", "Pleroma", 
                    "接続中", "切断", "エラー", "読み込み中"
                };
                foreach (var str in commonStrings)
                {
                    string.Intern(str); // 文字列プールに追加
                }
                
                // **ガベージコレクションの最適化設定**
                // Note: GCSettings is not available in all .NET versions, skip for compatibility
                
                // **初期メモリ確保（スタートアップ遅延回避）**
                GC.Collect(0, GCCollectionMode.Optimized);
                GC.WaitForPendingFinalizers();
                
                Console.WriteLine($"{DEBUG_PREFIX} メモリプリアロケーション完了");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ERROR_PREFIX} メモリプリアロケーションエラー: {ex.Message}");
            }
        }
        
        /// <summary>
        /// **PERFORMANCE FIX: Handle timeline scroll for virtualization (UI size aware)**
        /// </summary>
        private void OnTimelineScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            try
            {
                var scrollViewer = timelineControl.TimelineScrollViewer;
                if (scrollViewer == null || _timelineItems.Count == 0) return;
                
                // **画面サイズに基づく動的計算**
                var scrollTop = scrollViewer.Offset.Y;
                var viewportHeight = scrollViewer.Viewport.Height;
                
                // 実際の画面に表示できるアイテム数を計算
                var visibleItemsInViewport = Math.Ceiling(viewportHeight / ESTIMATED_ITEM_HEIGHT);
                
                // スクロール位置から表示すべきアイテムインデックスを計算
                var firstVisibleIndex = Math.Max(0, (int)(scrollTop / ESTIMATED_ITEM_HEIGHT));
                var lastVisibleIndex = Math.Min(_timelineItems.Count - 1, 
                    (int)(firstVisibleIndex + visibleItemsInViewport));
                
                // バッファを含めた範囲
                var startIndex = Math.Max(0, firstVisibleIndex - BUFFER_ITEM_COUNT);
                var endIndex = Math.Min(_timelineItems.Count - 1, lastVisibleIndex + BUFFER_ITEM_COUNT);
                
                var totalVirtualizedItems = endIndex - startIndex + 1;
                var actualVisibleItems = lastVisibleIndex - firstVisibleIndex + 1;
                
                // **デバッグ情報（画面サイズ対応）**
                Console.WriteLine($"{DEBUG_PREFIX} 仮想化[画面{viewportHeight:F0}px]: " +
                    $"表示{actualVisibleItems}件 + バッファ{totalVirtualizedItems-actualVisibleItems}件 " +
                    $"= 計{totalVirtualizedItems}件/{_timelineItems.Count}件");
                
                // 将来: 画面外要素の軽量化処理
                // OptimizeOffScreenItems(startIndex, endIndex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ERROR_PREFIX} スクロール仮想化エラー: {ex.Message}");
            }
        }

        private async void ShowServerManagement(object? sender, RoutedEventArgs e)
        {
            var serverManagementWindow = new ServerManagementWindow(_instances, _instanceTokens);
            
            var result = await serverManagementWindow.ShowDialog<bool?>(this);
            
            if (result == true)
            {
                // サーバー管理ウィンドウで変更があった場合
                Console.WriteLine("Server management changes detected, refreshing settings...");
                
                // 設定を再読み込み
                LoadSettings();
                
                // インスタンスリストを更新
                RefreshInstanceList();
                
                // UIを更新
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    // サーバータブを更新
                    if (_instances.Count > 0 && instanceSelectorControl.SelectedServer == null)
                    {
                        instanceSelectorControl.SelectServer(_instances[0]);
                    }
                    
                    statusBarControl.MainLabel.Text = "サーバー設定を更新しました";
                });
                
                Console.WriteLine($"Settings refreshed: {_instances.Count} instances loaded");
            }
        }
        
        private void RefreshInstanceList()
        {
            _instances.Clear();
            if (File.Exists(SETTINGS_FILE))
            {
                try
                {
                    var json = File.ReadAllText(SETTINGS_FILE);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json);
                    
                    if (settings != null)
                    {
                        // インスタンスを復元
                        foreach (var instance in settings.Instances)
                        {
                            _instances.Add(instance);
                        }
                        
                        // APIキーを復元
                        foreach (var token in settings.InstanceTokens)
                        {
                            _instanceTokens[token.Key] = token.Value;
                        }
                        
                        // サーバータブを復元（標準的なタイムラインのみ、独自TLは除外）
                        foreach (var instance in settings.Instances)
                        {
                            var serverTabs = DEFAULT_TABS.ToList();
                            _serverTabs[instance] = serverTabs;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading instances: {ex.Message}");
                }
            }
            
            // サーバータブも更新
            UpdateServerTabs();
        }

        private void UpdateServerTabs()
        {
            // サーバーリストを横タブに設定
            instanceSelectorControl.Servers = _instances.ToList();
        }

        private void OnServerTabSelected(object? sender, string serverName)
        {
            Console.WriteLine($"Server tab selected: {serverName}");
            
            // 選択されたサーバーのタブを更新
            UpdateTabs(serverName);
            
            // 接続状態を更新
            var isConnected = _connectedInstances.Contains(serverName);
            instanceSelectorControl.UpdateConnectButtonState(serverName, isConnected);
        }
        
    }
    
    public class AppSettings
    {
        public List<string> Instances { get; set; } = new List<string>();
        public Dictionary<string, string> InstanceTokens { get; set; } = new Dictionary<string, string>();
    }
}