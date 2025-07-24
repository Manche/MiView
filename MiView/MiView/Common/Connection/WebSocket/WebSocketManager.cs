﻿using MiView.Common.AnalyzeData;
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
    internal class WebSocketManager : IDisposable
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
                
                // 接続タイムアウトを90秒に延長
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(90));
                
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
            DataReceived(this, new ConnectDataReceivedEventArgs() { MessageRaw = ResponseMessage });
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
                    
                    // **MEMORY LEAK FIX: Unsubscribe event handlers to prevent circular references**
                    this.ConnectionLost -= OnConnectionLost;
                    this.DataReceived -= OnDataReceived;
                    
                    if (_WebSocket != null && _WebSocket.State == WebSocketState.Open)
                    {
                        try
                        {
                            // Use ConfigureAwait(false) to avoid deadlocks
                            _WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Dispose", CancellationToken.None)
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
