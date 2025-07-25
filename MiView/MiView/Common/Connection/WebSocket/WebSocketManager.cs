using MiView.Common.AnalyzeData;
using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiView.Common.Connection.WebSocket
{
    public class WebSocketManager : IDisposable
    {
        /// <summary>
        /// Host
        /// </summary>
        protected string _HostUrl
        {
            get; set;
        }
        = string.Empty;

        /// <summary>
        /// Host(original)
        /// </summary>
        protected string _HostDefinition
        {
            get; set;
        }
        = string.Empty;

        /// <summary>
        /// APIKey
        /// </summary>
        protected string? _APIKey
        {
            get; set;
        }
        = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        private WebSocketState _State
        {
            get; set;
        }
        = WebSocketState.None;

        /// <summary>
        /// Status/Command
        /// </summary>
        private WebSocketState _State_Command
        {
            get; set;
        }
        = WebSocketState.None;

        /// <summary>
        /// CloseConnection
        /// </summary>
        private bool _ConnectionClose
        {
            get; set;
        }
        = false;

        /// <summary>
        /// ユーザー操作による切断フラグ
        /// </summary>
        private bool _UserInitiatedDisconnect
        {
            get; set;
        }
        = false;

        /// <summary>
        /// KeepAliveタイマー (接続維持用)
        /// </summary>
        private Timer? _PingTimer;
        
        /// <summary>
        /// 最後のデータ受信時刻
        /// </summary>
        private DateTime _LastPongReceived = DateTime.Now;
        
        /// <summary>
        /// KeepAlive送信間隔（秒）
        /// </summary>
        private const int PING_INTERVAL_SECONDS = 15;
        
        /// <summary>
        /// データ受信タイムアウト（秒）- 5分
        /// </summary>
        private const int PONG_TIMEOUT_SECONDS = 300;

        protected object? _MainForm
        {
            get; set;
        }

        /// <summary>
        /// 紐づいているタイムラインオブジェクト
        /// </summary>
        protected object[]? _TimeLineObject
        {
            get; set;
        }
        = new object[0];
        

        /// <summary>
        /// Set TimeLineControl
        /// </summary>
        /// <param name="timeLine"></param>
        public void SetDataGridTimeLine(object timeLine)
        {
            if (this._TimeLineObject == null)
            {
                this._TimeLineObject = new object[0];
            }
            
            this._TimeLineObject = this._TimeLineObject.Concat(new object[] { timeLine }).ToArray();
        }

        /// <summary>
        /// WebSocket
        /// </summary>
        private ClientWebSocket _WebSocket
        {
            get; set;
        }
        = new ClientWebSocket();

        public event EventHandler<EventArgs>? ConnectionClosed;

        /// <summary>
        /// Constructor
        /// </summary>
        public WebSocketManager()
        {
            this.ConnectionLost += OnConnectionLost;
            this.DataReceived += OnDataReceived;
        }

        /// <summary>
        /// ConstructorWithOpen
        /// </summary>
        /// <param name="HostUrl"></param>
        public WebSocketManager(string HostUrl)
        {
            this.ConnectionLost += OnConnectionLost;
            this.DataReceived += OnDataReceived;

            this._HostUrl = HostUrl;

            Task.Run(async () =>
            {
                await Watcher();
            });
        }

        /// <summary>
        /// socket open and start
        /// </summary>
        /// <param name="HostUrl"></param>
        /// <returns></returns>
        protected WebSocketManager Start(string HostUrl)
        {
            this._HostUrl = HostUrl;

            _ = Task.Run(async () =>
            {
                await Watcher();
            });

            return this;
        }

        /// <summary>
        /// Prepare for Socket Close
        /// </summary>
        protected void ConnectionAbort()
        {
            this._ConnectionClose = true;
        }

        /// <summary>
        /// ユーザー操作による切断を設定
        /// </summary>
        public void SetUserInitiatedDisconnect(bool isUserInitiated)
        {
            this._UserInitiatedDisconnect = isUserInitiated;
        }

        /// <summary>
        /// ユーザー操作による切断かどうかを取得
        /// </summary>
        public bool IsUserInitiatedDisconnect()
        {
            return this._UserInitiatedDisconnect;
        }

        /// <summary>
        /// Get Socket
        /// </summary>
        /// <returns></returns>
        public ClientWebSocket GetSocketClient()
        {
            return this._WebSocket;
        }

        /// <summary>
        /// Set WebSocket Status
        /// </summary>
        /// <param name="State"></param>
        public void SetSocketState(WebSocketState State)
        {
            this._State = State;
        }

        /// <summary>
        /// Get WebSocket Status
        /// </summary>
        /// <returns></returns>
        public WebSocketState? GetSocketState()
        {
            return this._WebSocket.State;
        }

        /// <summary>
        /// Standby WebSocket Open
        /// </summary>
        /// <returns></returns>
        public bool IsStandBySocketOpen()
        {
            return this.GetSocketState() == WebSocketState.None;
        }

        /// <summary>
        /// SocketWatcher
        /// </summary>
        private async Task Watcher()
        {
            if (_WebSocket == null)
            {
                _WebSocket = new ClientWebSocket();
            }

            int retryCount = 0;
            const int maxRetries = 5;
            const int retryDelaySeconds = 5;
            
            while (!this._ConnectionClose)
            {
                try
                {
                    if (_WebSocket.State != WebSocketState.Open)
                    {
                        // ユーザー操作による切断でない場合のみ再接続
                        if (!this._UserInitiatedDisconnect)
                        {
                            System.Diagnostics.Debug.WriteLine($"WebSocket not open, attempting to connect. State: {_WebSocket.State}, Retry: {retryCount + 1}/{maxRetries}");
                            
                            await CreateAndOpen(this._HostUrl);
                            retryCount = 0; // 成功したらリトライカウントをリセット
                        }
                        else
                        {
                            // ユーザー操作による切断の場合は終了
                            System.Diagnostics.Debug.WriteLine("User initiated disconnect, stopping watcher");
                            break;
                        }
                    }
                    else
                    {
                        // 接続が正常な場合はリトライカウントをリセット
                        retryCount = 0;
                    }
                    
                    // 接続状態をチェック
                    await Task.Delay(TimeSpan.FromSeconds(10)); // 10秒間隔でチェック
                }
                catch (Exception ex)
                {
                    retryCount++;
                    System.Diagnostics.Debug.WriteLine($"WebSocket watcher error (retry {retryCount}/{maxRetries}): {ex.Message}");
                    
                    if (retryCount >= maxRetries)
                    {
                        System.Diagnostics.Debug.WriteLine($"Max retries reached, stopping watcher for {this._HostUrl}");
                        break;
                    }
                    
                    // 指数バックオフで待機
                    var delaySeconds = Math.Min(retryDelaySeconds * (int)Math.Pow(2, retryCount - 1), 60);
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }
        }

        /// <summary>
        /// SocketOpen
        /// </summary>
        /// <param name="HostUrl"></param>
        /// <exception cref="InvalidOperationException"></exception>
        protected async Task CreateAndOpen(string HostUrl)
        {
            _HostUrl = HostUrl;

            if ((this._State == WebSocketState.Open))
            {
                System.Diagnostics.Debug.WriteLine("Socket is already opened, skipping connection");
                return;
            }

            ClientWebSocket? WS = null;
            try
            {
                // 既存のWebSocketをクリーンアップ
                if (_WebSocket != null && _WebSocket.State != WebSocketState.Closed)
                {
                    try
                    {
                        await _WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Reconnecting", CancellationToken.None);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error closing existing WebSocket: {ex.Message}");
                    }
                }

                WS = new ClientWebSocket();
                
                // WebSocketオプションを設定
                WS.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
                WS.Options.SetRequestHeader("User-Agent", "MiView/1.0");
                
                // 接続タイムアウトを10分に延長
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
                
                System.Diagnostics.Debug.WriteLine($"Attempting WebSocket connection to: {this._HostUrl}");
                await WS.ConnectAsync(new Uri(this._HostUrl), cts.Token);

                // 接続状態を確認
                await Task.Delay(1000); // 1秒待機して状態を安定させる

                if (WS.State != WebSocketState.Open)
                {
                    System.Diagnostics.Debug.WriteLine($"WebSocket connection failed. State: {WS.State}");
                    throw new InvalidOperationException($"WebSocket connection failed. State: {WS.State}");
                }

                System.Diagnostics.Debug.WriteLine($"WebSocket connection successful to: {this._HostUrl}");
                this._WebSocket = WS;
                this._State = WS.State;
                
                // **アイドルタイムアウト対策: KeepAliveタイマーを開始**
                StartPingTimer();
#if DEBUG
                Console.WriteLine($"🏓 KEEPALIVE TIMER STARTED: {this._HostDefinition} - Interval: {PING_INTERVAL_SECONDS}s");
#endif
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine($"WebSocket connection timeout for {this._HostUrl}");
                throw;
            }
            catch (WebSocketException wsEx)
            {
                System.Diagnostics.Debug.WriteLine($"WebSocket connection error for {this._HostUrl}: {wsEx.Message}");
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebSocket operation error for {this._HostUrl}: {ex.Message}");
                throw;
            }
        }

        protected async Task Close(string HostUrl)
        {
            _HostUrl = HostUrl;

            if ((this._State == WebSocketState.Closed))
            {
                throw new InvalidOperationException("Socket is already opened");
            }

            try
            {

                await this._WebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, null, CancellationToken.None);
                while (this._WebSocket.State != WebSocketState.Closed &&
                       this._WebSocket.State != WebSocketState.Aborted)
                {
                }

                this._WebSocket = this._WebSocket;
                this._State = this._WebSocket.State;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebSocket operation error: {ex.Message}");
            }
        }

        /// <summary>
        /// Set WebSocket URL
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="APIKey"></param>
        /// <returns></returns>
        protected string GetWSURL(string InstanceURL, string? APIKey)
        {
            this._HostDefinition = InstanceURL;
            this._APIKey = APIKey;

            return APIKey != null ? string.Format("wss://{0}/streaming?i={1}", InstanceURL, APIKey) : string.Format("wss://{0}/streaming", InstanceURL);
        }

        public event EventHandler<EventArgs> ConnectionLost;
        protected virtual void OnConnectionLost(object? sender, EventArgs e)
        {
        }
        protected void CallConnectionLost()
        {
            ConnectionLost(this, new EventArgs());
        }

        public event EventHandler<ConnectDataReceivedEventArgs> DataReceived;
        protected virtual void OnDataReceived(object? sender, ConnectDataReceivedEventArgs e)
        {
        }
        protected void CallDataReceived(string ResponseMessage)
        {
            // **アイドルタイムアウト対策: データ受信時刻を更新**
            _LastPongReceived = DateTime.Now;
            
            DataReceived(this, new ConnectDataReceivedEventArgs() { MessageRaw = ResponseMessage });
        }

        /// <summary>
        /// **アイドルタイムアウト対策: KeepAliveタイマーを開始**
        /// </summary>
        private void StartPingTimer()
        {
            StopPingTimer(); // 既存のタイマーがあれば停止
            
            _LastPongReceived = DateTime.Now;
            _PingTimer = new Timer(SendPingFrame, null, 
                TimeSpan.FromSeconds(PING_INTERVAL_SECONDS), 
                TimeSpan.FromSeconds(PING_INTERVAL_SECONDS));
                
#if DEBUG
            Console.WriteLine($"🏓 KEEPALIVE TIMER: Started for {_HostDefinition}");
#endif
        }

        /// <summary>
        /// **アイドルタイムアウト対策: KeepAliveタイマーを停止**
        /// </summary>
        private void StopPingTimer()
        {
            if (_PingTimer != null)
            {
#if DEBUG
                Console.WriteLine($"🏓 KEEPALIVE TIMER: Stopped for {_HostDefinition}");
#endif
                _PingTimer.Dispose();
                _PingTimer = null;
            }
        }

        /// <summary>
        /// **アイドルタイムアウト対策: ダミーデータを送信して接続維持**
        /// </summary>
        private async void SendPingFrame(object? state)
        {
            try
            {
                if (_WebSocket?.State != WebSocketState.Open)
                {
#if DEBUG
                    Console.WriteLine($"🏓 KEEPALIVE SKIPPED: Connection not open for {_HostDefinition} (State: {_WebSocket?.State})");
#endif
                    return;
                }

                // 最後のデータ受信からの経過時間をチェック
                var timeSinceLastData = DateTime.Now - _LastPongReceived;
                if (timeSinceLastData.TotalSeconds > PONG_TIMEOUT_SECONDS)
                {
#if DEBUG
                    Console.WriteLine($"🏓 DATA TIMEOUT: {_HostDefinition} - {timeSinceLastData.TotalSeconds}s since last data");
#endif
                    // タイムアウトした場合は接続切断を報告
                    CallConnectionLost();
                    return;
                }

                // Misskeyサーバー用のダミーデータ（小さなJSONメッセージ）を送信
                var keepAliveMessage = "{\"type\":\"ping\"}";
                var messageBuffer = Encoding.UTF8.GetBytes(keepAliveMessage);
                await _WebSocket.SendAsync(new ArraySegment<byte>(messageBuffer), 
                    WebSocketMessageType.Text, true, CancellationToken.None);

#if DEBUG
                Console.WriteLine($"🏓 KEEPALIVE SENT: {_HostDefinition} - Message: {keepAliveMessage}");
#endif
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"🏓 KEEPALIVE ERROR: {_HostDefinition} - {ex.Message}");
#endif
                // KeepAlive送信エラーの場合も接続切断を報告
                CallConnectionLost();
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                try
                {
                    _ConnectionClose = true;
                    
                    // **アイドルタイムアウト対策: KeepAliveタイマーを停止**
                    StopPingTimer();
                    
                    this.ConnectionLost -= OnConnectionLost;
                    this.DataReceived -= OnDataReceived;
                    
                    if (_WebSocket != null && _WebSocket.State == WebSocketState.Open)
                    {
                        try
                        {
                            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10)); // 10分タイムアウト
                            _WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Dispose", cts.Token)
                                .ConfigureAwait(false).GetAwaiter().GetResult();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error closing WebSocket: {ex.Message}");
                        }
                    }
                    
                    _WebSocket?.Dispose();
                    _WebSocket = null;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error during WebSocketManager dispose: {ex.Message}");
                }
            }
        }
    }
}
