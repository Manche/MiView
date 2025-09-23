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
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MiView.Common.Connection.WebSocket
{
    internal class WebSocketManager
    {
        /// <summary>
        /// Host
        /// </summary>
        protected string _HostUrl { get; set; } = string.Empty;

        /// <summary>
        /// Host(original)
        /// </summary>
        protected string _HostDefinition { get; set; } = string.Empty;

        /// <summary>
        /// APIKey
        /// </summary>
        protected string? _APIKey { get; set; } = string.Empty;

        /// <summary>
        /// Status
        /// </summary>
        private WebSocketState _State { get; set; } = WebSocketState.None;

        /// <summary>
        /// Status/Command
        /// </summary>
        private WebSocketState _State_Command { get; set; } = WebSocketState.None;

        /// <summary>
        /// CloseConnection
        /// </summary>
        private bool _ConnectionClose { get; set; } = false;

        protected MainForm _MainForm { get; set; } = new MainForm();

        /// <summary>
        /// 紐づいているタイムラインオブジェクト
        /// </summary>
        protected DataGridTimeLine[]? _TimeLineObject { get; set; } = new DataGridTimeLine[0];

        /// <summary>
        /// Set TimeLineControl
        /// </summary>
        /// <param name="timeLine"></param>
        public void SetDataGridTimeLine(DataGridTimeLine timeLine)
        {
            if (this._TimeLineObject == null)
            {
                this._TimeLineObject = new DataGridTimeLine[0];
            }
            this._TimeLineObject = this._TimeLineObject.Concat(new DataGridTimeLine[] { timeLine }).ToArray();
        }

        /// <summary>
        /// WebSocket
        /// </summary>
        private ClientWebSocket _WebSocket { get; set; } = new ClientWebSocket();

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

            // 旧実装互換：起動時に Watcher を走らせる
            Task.Run(async () =>
            {
                await Watcher().ConfigureAwait(false);
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
                await Watcher().ConfigureAwait(false);
            });

            return this;
        }

        /// <summary>
        /// Prepare for Socket Close
        /// </summary>
        protected void ConnectionAbort()
        {
            // 既存フラグを立てる（Watcher が見て停止する）
            this._ConnectionClose = true;
            // 保持しているソケット/CTS を確実に破棄
            DisposeCurrentWebSocket();
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
            try
            {
                return this._WebSocket?.State;
            }
            catch
            {
                return WebSocketState.None;
            }
        }

        /// <summary>
        /// Standby WebSocket Open
        /// </summary>
        /// <returns></returns>
        public bool IsStandBySocketOpen()
        {
            return this.GetSocketState() == WebSocketState.None;
        }

        // --- 再接続管理用（内部） ---
        private readonly object _sync = new object();
        private CancellationTokenSource? _lifecycleCts = null;
        private Task? _connectLoopTask = null;
        private bool _watcherRunning = false;
        private int _retryCount = 0;

        /// <summary>
        /// SocketWatcher
        /// 既存の Watcher と同名を保ったまま内部で安定した接続ループを回す。
        /// </summary>
        private async Task Watcher()
        {
            lock (_sync)
            {
                if (_watcherRunning) return;
                _watcherRunning = true;
            }

            try
            {
                // ラッパーとして ConnectLoop を走らせる（外からは Watcher 呼び出しのみで互換性確保）
                await ConnectLoopAsync().ConfigureAwait(false);
            }
            finally
            {
                lock (_sync)
                {
                    _watcherRunning = false;
                }
            }
        }

        /// <summary>
        /// 実際の接続ループ：接続 -> 受信 -> 切断 -> 再接続（バックオフ） を管理
        /// </summary>
        private async Task ConnectLoopAsync()
        {
            // ループ。_ConnectionClose が true または Cancel で抜ける
            while (!this._ConnectionClose)
            {
                try
                {
                    // 接続が開いていなければ作る/接続する
                    if (this._WebSocket == null || this._WebSocket.State != WebSocketState.Open)
                    {
                        await CreateAndOpen(this._HostUrl).ConfigureAwait(false);
                    }

                    // もし接続されたら受信ループを実行（CreateAndOpen が接続失敗なら例外で飛ぶ）
                    if (this._WebSocket != null && this._WebSocket.State == WebSocketState.Open)
                    {
                        _retryCount = 0; // 成功したのでリトライカウンタをリセット
                        await ReceiveLoopAsync(this._WebSocket).ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException)
                {
                    // 停止要求が来た
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WebSocketManager.ConnectLoopAsync exception: {ex.GetType().Name} {ex.Message}");
                    // 接続や受信で問題が起きたら接続は確実に破棄して再試行へ
                    CallConnectionLost();
                    DisposeCurrentWebSocket();
                }

                if (this._ConnectionClose) break;

                // バックオフ待ち（再接続）
                _retryCount++;
                var waitMs = ComputeBackoffMs(_retryCount);
                Debug.WriteLine($"WebSocketManager: reconnecting in {waitMs} ms (attempt {_retryCount})");
                try
                {
                    await Task.Delay(waitMs).ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            } // while

            // ループ終了時には確実にクローズイベントを呼ぶ
            try
            {
                ConnectionClosed?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        /// <summary>
        /// SocketOpen
        /// (protected の既存シグネチャを保つ)
        /// </summary>
        /// <param name="HostUrl"></param>
        /// <exception cref="InvalidOperationException"></exception>
        protected async Task CreateAndOpen(string HostUrl)
        {
            _HostUrl = HostUrl;

            // 既に開いているなら例外を投げる（既存の仕様を維持）
            if ((this._State == WebSocketState.Open) || (this._WebSocket != null && this._WebSocket.State == WebSocketState.Open))
            {
                throw new InvalidOperationException("Socket is already opened");
            }

            // 古いソケットを破棄してから新規作成
            DisposeCurrentWebSocket();

            ClientWebSocket? WS = null;
            try
            {
                WS = new ClientWebSocket();
                // KeepAliveInterval を既存実装と同様に設定（0なら OS による）
                WS.Options.KeepAliveInterval = TimeSpan.Zero;

                // 接続タイムアウトやキャンセル管理のために CTS を用意
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                // 保持しているライフサイクルトークンとして保存（必要なら Stop 停止でも利用）
                lock (_sync)
                {
                    _lifecycleCts?.Dispose();
                    _lifecycleCts = new CancellationTokenSource();
                }

                Debug.WriteLine($"WebSocketManager: Attempting ConnectAsync to {_HostUrl}");
                await WS.ConnectAsync(new Uri(this._HostUrl), cts.Token).ConfigureAwait(false);

                if (WS.State != WebSocketState.Open)
                {
                    Debug.WriteLine("WebSocketManager: Connect did not reach Open state.");
                    // 接続に失敗している扱いで Dispose して例外へ
                    WS.Abort();
                    WS.Dispose();
                    throw new InvalidOperationException("WebSocket connection failed to open.");
                }

                // 成功したら保持し、状態を更新
                this._WebSocket = WS;
                this._State = WS.State;
                Debug.WriteLine("WebSocketManager: Connected");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WebSocketManager.CreateAndOpen exception: {ex.GetType().Name} {ex.Message}");
                // 例外は呼び出し元に返さず（Watcher の再試行で扱う）クリーンアップする
                if (WS != null)
                {
                    try { WS.Abort(); } catch { }
                    try { WS.Dispose(); } catch { }
                }
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
                if (this._WebSocket != null && (this._WebSocket.State == WebSocketState.Open || this._WebSocket.State == WebSocketState.CloseReceived || this._WebSocket.State == WebSocketState.CloseSent))
                {
                    await this._WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"WebSocketManager.Close exception: {ex.GetType().Name} {ex.Message}");
            }
            finally
            {
                DisposeCurrentWebSocket();
                this._State = WebSocketState.Closed;
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
            // 外部購読者がある場合は呼ばれる（既存互換）
        }
        protected void CallConnectionLost()
        {
            try
            {
                ConnectionLost?.Invoke(this, new EventArgs());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ConnectionLost handler threw: {ex.GetType().Name} {ex.Message}");
            }
        }

        public event EventHandler<ConnectDataReceivedEventArgs> DataReceived;
        protected virtual void OnDataReceived(object? sender, ConnectDataReceivedEventArgs e)
        {
            // 外部購読者がある場合は呼ばれる（既存互換）
        }
        protected void CallDataReceived(string ResponseMessage)
        {
            try
            {
                DataReceived?.Invoke(this, new ConnectDataReceivedEventArgs() { MessageRaw = ResponseMessage });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DataReceived handler threw: {ex.GetType().Name} {ex.Message}");
            }
        }

        // --- 内部受信ループ ---
        private async Task ReceiveLoopAsync(ClientWebSocket ws)
        {
            var buffer = new byte[8192];

            while (ws != null && ws.State == WebSocketState.Open && !this._ConnectionClose)
            {
                WebSocketReceiveResult? result = null;
                try
                {
                    var seg = new ArraySegment<byte>(buffer);
                    result = await ws.ReceiveAsync(seg, CancellationToken.None).ConfigureAwait(false);
                }
                catch (WebSocketException wex)
                {
                    Debug.WriteLine($"WebSocketManager.ReceiveLoopAsync WebSocketException: {wex.Message}");
                    break;
                }
                catch (OperationCanceledException)
                {
                    // 停止要求時のキャンセル
                    break;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WebSocketManager.ReceiveLoopAsync Exception: {ex.GetType().Name} {ex.Message}");
                    break;
                }

                if (result == null)
                {
                    Debug.WriteLine("WebSocketManager.ReceiveLoopAsync: null result");
                    break;
                }

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Debug.WriteLine($"WebSocketManager: server requested close: {result.CloseStatus} {result.CloseStatusDescription}");
                    // 切断イベント通知
                    CallConnectionLost();
                    try
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client ack close", CancellationToken.None).ConfigureAwait(false);
                    }
                    catch { }
                    break;
                }

                // メッセージのチャンク結合
                using (var ms = new System.IO.MemoryStream())
                {
                    ms.Write(buffer, 0, result.Count);
                    while (!result.EndOfMessage)
                    {
                        try
                        {
                            result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"WebSocketManager.ReceiveLoopAsync chunk read failed: {ex.GetType().Name} {ex.Message}");
                            result = null;
                            break;
                        }
                        if (result == null) break;
                        ms.Write(buffer, 0, result.Count);
                    }
                    if (ms.Length > 0)
                    {
                        var msg = Encoding.UTF8.GetString(ms.ToArray());
                        // 既存の DataReceived イベント呼び出しを利用
                        CallDataReceived(msg);
                    }
                }
            } // while

            Debug.WriteLine("WebSocketManager.ReceiveLoopAsync exiting");
        }

        /// <summary>
        /// 古いソケット／CTS を確実にクリーンアップする内部ユーティリティ
        /// </summary>
        private void DisposeCurrentWebSocket()
        {
            lock (_sync)
            {
                try
                {
                    if (_WebSocket != null)
                    {
                        try { _WebSocket.Abort(); } catch { }
                        try { _WebSocket.Dispose(); } catch { }
                    }
                }
                finally
                {
                    _WebSocket = new ClientWebSocket(); // 保守的に未 null の状態を保つ（GetSocketClient 互換性のため）
                }

                try
                {
                    if (_lifecycleCts != null)
                    {
                        try { _lifecycleCts.Cancel(); } catch { }
                        try { _lifecycleCts.Dispose(); } catch { }
                    }
                }
                finally
                {
                    _lifecycleCts = null;
                }
            }
        }

        private static int ComputeBackoffMs(int retryCount)
        {
            var pow = Math.Min(retryCount, 6); // 2^6
            var ms = (int)(1000 * Math.Pow(2, Math.Max(0, pow - 1)));
            if (ms <= 0) ms = 1000;
            return Math.Min(ms, 30000);
        }
    }
}
