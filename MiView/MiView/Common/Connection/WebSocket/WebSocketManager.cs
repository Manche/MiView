using MiView.Common.AnalyzeData;
using MiView.Common.Connection.REST.Misskey.v2025.API.Notes;
using MiView.Common.Connection.VersionInfo;
using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.Connection.WebSocket.Misskey.v2025;
using MiView.Common.Connection.WebSocket.Structures;
using MiView.Common.TimeLine;
using MiView.Common.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace MiView.Common.Connection.WebSocket
{
    public class WebSocketManager
    {
        public string _HostUrl { get; set; } = string.Empty;
        public string _HostDefinition { get; set; } = string.Empty;
        protected string? _APIKey { get; set; } = string.Empty;
        public string? APIKey { get { return _APIKey; } }
        public void SetAPIKey(string APIKey) { _APIKey = APIKey; }
        public string _Host { get { return _OHost; } }
        protected string _OHost { get; set; } = string.Empty;
        public DateTime _LastDataReceived { get; set; }

        private WebSocketState _State { get; set; } = WebSocketState.None;
        private WebSocketState _State_Command { get; set; } = WebSocketState.None;
        private bool _ConnectionClose { get; set; } = false;
        public bool _ConnectionClosed { get { return _ConnectionClose; } }

        protected MainForm _MainForm { get; set; } = new MainForm();
        protected DataGridTimeLine[]? _TimeLineObject { get; set; } = new DataGridTimeLine[0];
        public DataGridTimeLine[]? TimeLineObject { get { return this._TimeLineObject; } }
        public void SetTimeLineObject(DataGridTimeLine[] Grid) {  this._TimeLineObject = Grid; }

        private ClientWebSocket _WebSocket { get; set; } = new ClientWebSocket();
        public ClientWebSocket WebSocket { get { return _WebSocket; } }
        public void SetWebSocket(ClientWebSocket value) {  this._WebSocket = value; }
        private CancellationTokenSource _Cancellation = new CancellationTokenSource();

        public CSoftwareVersionInfo SoftwareVersion { get; set; }

        public event EventHandler<EventArgs>? ConnectionClosed;
        public event EventHandler<EventArgs> ConnectionLost;
        public event EventHandler<ConnectDataReceivedEventArgs> DataReceived;
        public event EventHandler<DataContainerEventArgs>? DataAccepted;
        public event EventHandler<DataContainerEventArgs>? DataRejected;

        public WebSocketManager()
        {
            this.ConnectionLost += OnConnectionLost;
            this.DataReceived += OnDataReceived;
            this.DataAccepted += OnDataAccepted;
            this.DataRejected += OnDataRejected;
        }

        public WebSocketManager(string HostUrl) : this()
        {
            if (this._HostUrl == null || this._HostUrl == string.Empty)
            {
                this._HostUrl = HostUrl;
            }
            _ = Task.Run(async () => await Watcher());
        }

        public void SetDataGridTimeLine(DataGridTimeLine timeLine)
        {
            if (this._TimeLineObject == null) this._TimeLineObject = new DataGridTimeLine[0];
            if (this._TimeLineObject.ToList().FindAll(r => { return r._Definition == timeLine._Definition; }).Count > 0)
            {
                return;
            }
            this._TimeLineObject = this._TimeLineObject.Concat(new DataGridTimeLine[] { timeLine }).ToArray();
        }
        public bool IncludedDataGridTimeLine(Func<DataGridTimeLine, bool>[]? Expression = null)
        {
            if (this._TimeLineObject == null)
            {
                return false;
            }
            var TLObj = this._TimeLineObject.ToList();
            var index = TLObj.ToList()
                            .FindAll(r => {
                                if (Expression != null)
                                {
                                    return Expression.Length == Expression.ToList()
                                                                            .FindAll(e => {
                                                                                return e(r);
                                                                            })
                                                                            .Count;
                                }
                                else
                                {
                                    return true;
                                }
                            })
                            .Select(r =>
                            {
                                return TLObj.IndexOf(r);
                            })
                            .ToList();
            return index.Count > 0;
        }
        public bool DetachDataGridTimeLine(Func<DataGridTimeLine, bool>[]? Expression = null, bool DeleteAll = false)
        {
            List<int> RemoveIndex = new List<int>();
            if (this._TimeLineObject == null)
            {
                return true;
            }
            var TLObj = this._TimeLineObject.ToList();
            var index = TLObj.ToList()
                            .FindAll(r => {
                                if (Expression != null)
                                {
                                    return Expression.Length == Expression.ToList()
                                                                            .FindAll(e => {
                                                                                return e(r);
                                                                            })
                                                                            .Count;
                                }
                                else
                                {
                                    return true;
                                }
                            })
                            .Select(r =>
                            {
                                return TLObj.IndexOf(r);
                            })
                            .ToList();
            if (index.Count == 0 && (!DeleteAll ? index.Count == TLObj.Count : false))
            {
                return false;
            }
            return DetachDataGridTimeLine(index);
        }
        public bool DetachDataGridTimeLine(List<int> RemoveIndex)
        {
            var Inx = RemoveIndex.ToArray();
            Array.Reverse(Inx);

            if (this._TimeLineObject == null)
            {
                return false;
            }
            var TLObj = this._TimeLineObject.ToList();

            foreach (int index in Inx)
            {
                TLObj.RemoveAt(index);
            }
            this._TimeLineObject = TLObj.ToArray();

            return true;
        }

        public ClientWebSocket GetSocketClient() => this._WebSocket;
        public WebSocketState? GetSocketState() => this._WebSocket?.State;

        public void SetSocketState(WebSocketState State) => this._State = State;
        public bool IsStandBySocketOpen() => GetSocketState() == WebSocketState.None;
        public void ConnectionAbort() => this._ConnectionClose = true;
        public bool _IsOpenTimeLine = false;

        protected WebSocketManager Start(string HostUrl)
        {
            if (this._HostUrl == null || this._HostUrl == string.Empty)
            {
                this._HostUrl = HostUrl;
            }
            _ = Task.Run(async () => await Watcher());
            return this;
        }

        protected virtual string GetWSURL(string InstanceURL, string? APIKey)
        {
            throw new NotImplementedException("継承元クラスです");
        }

        protected virtual void OnConnectionLost(object? sender, EventArgs e) { }
        public void CallConnectionLost() => ConnectionLost?.Invoke(this, new EventArgs());

        protected virtual void OnDataReceived(object? sender, ConnectDataReceivedEventArgs e)
        {
        }
        public void CallDataReceived(string ResponseMessage)
        {
            this._LastDataReceived = DateTime.Now;
            DataReceived?.Invoke(this, new ConnectDataReceivedEventArgs() { MessageRaw = ResponseMessage });
        }
        protected virtual void OnDataAccepted(object? sender, DataContainerEventArgs Container)
        {
            this._MainForm.CallDataAccepted(Container.Container);
        }
        public void CallDataAccepted(TimeLineContainer Container) => DataAccepted?.Invoke(this, new DataContainerEventArgs());
        protected virtual void OnDataRejected(object? sender, DataContainerEventArgs Container)
        {
            this._MainForm.CallDataRejected(Container.Container);
        }
        public void CallDataRejected(TimeLineContainer Container) => DataRejected?.Invoke(this, new DataContainerEventArgs());

        private async Task Watcher()
        {
            if (_WebSocket == null) _WebSocket = new ClientWebSocket();

            while (!_ConnectionClose)
            {
                try
                {
                    if (_WebSocket == null ||
                        _WebSocket.State == WebSocketState.Closed ||
                        _WebSocket.State == WebSocketState.Aborted)
                    {
                        // 古いソケットを破棄して新規作成
                        _WebSocket?.Dispose();
                        _WebSocket = new ClientWebSocket();
                    }
                    if (_WebSocket.State != WebSocketState.Open)
                    {
                        await CreateAndOpen(_HostUrl);

                        if (_WebSocket.State == WebSocketState.Open)
                        {
                            _ = Task.Run(() => ReceiveLoop(_Cancellation.Token));
                        }
                    }
                }
                catch (Exception ex)
                {
                    CallError(ex);
                }

                await Task.Delay(2000, _Cancellation.Token); // CPU暴走防止
            }
        }

        protected async Task CreateAndOpen(string HostUrl)
        {
            if (this._HostUrl == null || this._HostUrl == string.Empty)
            {
                this._HostUrl = HostUrl;
            }

            await this._CreateAndOpen(_HostUrl);
        }

        /// <summary>
        /// ソケットロックオブジェクト
        /// </summary>
        private readonly object _socketLock = new();
        private bool _isReconnecting = false;
        private readonly object _reconnectLock = new();

        /// <summary>
        /// WebSocketを安全に再接続
        /// </summary>
        private async Task ReconnectAsync()
        {
            lock (_reconnectLock)
            {
                if (_isReconnecting)
                {
                    LogOutput.Write(LogOutput.LOG_LEVEL.INFO,
                        $"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [WebSocketManager] Reconnect already in progress — skipping {_HostDefinition}");
                    return;
                }
                _isReconnecting = true;
            }

            try
            {
                LogOutput.Write(LogOutput.LOG_LEVEL.INFO,
                    $"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [WebSocketManager] Reconnect start {_HostDefinition}");

                _WebSocket?.Dispose();
                _WebSocket = new ClientWebSocket();
                _WebSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);

                await _WebSocket.ConnectAsync(new Uri(_HostUrl), CancellationToken.None);

                LogOutput.Write(LogOutput.LOG_LEVEL.INFO,
                    $"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [WebSocketManager] Reconnected successfully. Restarting ReceiveLoop... {_HostDefinition}");

                await Task.Delay(1000); // Misskey側の再購読安定化用ディレイ

                await ResubscribeChannelAsync();

                // 新しいキャンセルトークンでループを再開
                var tokenSource = new CancellationTokenSource();
                _ = Task.Run(() => ReceiveLoop(tokenSource.Token));
                _ = Task.Run(() => PingLoop(tokenSource.Token));
            }
            catch (Exception ex)
            {
                LogOutput.Write(LogOutput.LOG_LEVEL.ERROR,
                    $"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [WebSocketManager] Reconnect failed — {ex.Message} {_HostDefinition}");
            }
            finally
            {
                lock (_reconnectLock)
                {
                    _isReconnecting = false;
                }
            }
        }


        private async Task ResubscribeChannelAsync()
        {
            try
            {
                var body = new
                {
                    type = "connect",
                    body = new { channel = "main", id = Guid.NewGuid().ToString() }
                };
                var json = JsonSerializer.Serialize(body);
                var bytes = Encoding.UTF8.GetBytes(json);

                await _WebSocket.SendAsync(
                    new ArraySegment<byte>(bytes),
                    WebSocketMessageType.Text,
                    true,
                    CancellationToken.None);

                LogOutput.Write(LogOutput.LOG_LEVEL.INFO,
                    $"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [WebSocketManager] Sent reconnect channel message: {json} {_HostDefinition}");
            }
            catch (Exception ex)
            {
                LogOutput.Write(LogOutput.LOG_LEVEL.ERROR,
                    $"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [WebSocketManager] ResubscribeChannelAsync failed: {ex.Message} {_HostDefinition}");
            }
        }

        /// <summary>
        /// WebSocket受信ループ
        /// </summary>
        private async Task StartReceiveLoop()
        {
            var buffer = new byte[4096 * 4];

            while (true)
            {
                try
                {
                    if (_WebSocket == null || _WebSocket.State != WebSocketState.Open)
                    {
                        Debug.WriteLine($"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [ReadLoop] socket not open (), requesting reconnect {_HostDefinition}");
                        LogOutput.Write(LogOutput.LOG_LEVEL.INFO, $"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [ReadLoop] socket not open (), requesting reconnect {_HostDefinition}");
                        await Task.Delay(2000);
                        await ReconnectAsync();
                        continue;
                    }

                    var result = await _WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _Cancellation?.Token ?? CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Debug.WriteLine($"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [ReadLoop] server requested close -> reconnect {_HostDefinition}");
                        LogOutput.Write(LogOutput.LOG_LEVEL.INFO, $"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [ReadLoop] server requested close -> reconnect {_HostDefinition}");
                        await ReconnectAsync();
                        continue;
                    }

                    string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    Debug.WriteLine($"[{DateTime.Now:yyyyMMddHHmmss}] [DEBUG] [ReadLoop] received {msg.Length} bytes from {_HostDefinition}");
                    LogOutput.Write(LogOutput.LOG_LEVEL.INFO, $"[{DateTime.Now:yyyyMMddHHmmss}] [DEBUG] [ReadLoop] received {msg.Length} bytes from {_HostDefinition}");
                }
                catch (WebSocketException ex)
                {
                    Debug.WriteLine($"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [ReadLoop] WebSocketException: {ex.Message} {_HostDefinition}");
                    LogOutput.Write(LogOutput.LOG_LEVEL.INFO, $"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [ReadLoop] WebSocketException: {ex.Message} {_HostDefinition}");
                    await Task.Delay(2000);
                    await ReconnectAsync();
                }
                catch (OperationCanceledException)
                {
                    Debug.WriteLine($"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [ReadLoop] OperationCanceledException -> reconnect {_HostDefinition}");
                    LogOutput.Write(LogOutput.LOG_LEVEL.INFO, $"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [ReadLoop] OperationCanceledException -> reconnect {_HostDefinition}");
                    await Task.Delay(2000);
                    await ReconnectAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [ReadLoop] General receive error2:{_HostDefinition}\n{ex}");
                    LogOutput.Write(LogOutput.LOG_LEVEL.INFO, $"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [ReadLoop] General receive error2:{_HostDefinition}\n{ex}");
                    await Task.Delay(3000);
                    await ReconnectAsync();
                }

                await Task.Delay(1000);
            }
        }

        /// <summary>
        /// 再接続
        /// </summary>
        public void CreateAndReOpen()
        {
            _ = Task.Run(async () => await ReconnectAsync());
        }

        private async Task _CreateAndOpen(string HostUrl)
        {
            if (_State == WebSocketState.Open &&
                this._WebSocket.State == WebSocketState.Open &&
                this._IsOpenTimeLine == true)
            {
                this._LastDataReceived = DateTime.Now;
                return;
            }

            try
            {
                var WS = new ClientWebSocket();
                WS.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
                await WS.ConnectAsync(new Uri(_HostUrl), CancellationToken.None);

                _WebSocket = WS;
                _State = WS.State;
            }
            catch (Exception ex)
            {
                CallError(ex);
            }
        }

        protected async Task Close(string HostUrl)
        {
            if (this._HostUrl == null || this._HostUrl == string.Empty)
            {
                this._HostUrl = HostUrl;
            }

            if (_State == WebSocketState.Closed)
                throw new InvalidOperationException("Socket is already closed");

            try
            {
                await _WebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, null, CancellationToken.None);
                while (_WebSocket.State != WebSocketState.Closed && _WebSocket.State != WebSocketState.Aborted)
                {
                    await Task.Delay(50);
                }
                _State = _WebSocket.State;
                _IsOpenTimeLine = false;
            }
            catch (Exception ex)
            {
                CallError(ex);
            }
        }

        private async Task ReceiveLoop(CancellationToken token)
        {
            var buffer = new byte[8192];
            var sb = new StringBuilder();

            try
            {
                while (!token.IsCancellationRequested)
                {
                    if (_WebSocket == null || _WebSocket.State != WebSocketState.Open)
                    {
                        LogOutput.Write(LogOutput.LOG_LEVEL.INFO,
                            $"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [ReadLoop] socket not open ({_WebSocket?.State}), requesting reconnect {_HostDefinition}");
                        await ReconnectAsync();
                        await Task.Delay(2000);
                        continue;
                    }

                    sb.Clear();
                    WebSocketReceiveResult result;

                    try
                    {
                        result = await _WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                        // サーバ側からclose指示
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            LogOutput.Write(LogOutput.LOG_LEVEL.INFO,
                                $"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [ReadLoop] server requested close -> reconnect {_HostDefinition}");
                            await ReconnectAsync();
                            continue;
                        }

                        // メッセージ受信処理
                        sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        if (result.EndOfMessage)
                        {
                            var msg = sb.ToString();
                            if (!string.IsNullOrEmpty(msg))
                                CallDataReceived(msg);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        LogOutput.Write(LogOutput.LOG_LEVEL.ERROR,
                            $"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [ReadLoop] OperationCanceledException -> reconnect {_HostDefinition}");
                        await ReconnectAsync();
                    }
                    catch (WebSocketException ex)
                    {
                        LogOutput.Write(LogOutput.LOG_LEVEL.ERROR,
                            $"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [ReadLoop] WebSocketException: {ex.Message} {_HostDefinition}");
                        await ReconnectAsync();
                    }
                    catch (Exception ex)
                    {
                        LogOutput.Write(LogOutput.LOG_LEVEL.ERROR,
                            $"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [ReadLoop] General receive error: {ex.Message} {_HostDefinition}");
                        await Task.Delay(1000);
                    }

                    await Task.Delay(500);
                }
            }
            finally
            {
                await Task.Delay(1000);
            }
        }

        private void CallError(Exception ex)
        {
            Debug.WriteLine($"WebSocketManager Error: {ex}");
        }

        private async Task PingLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_WebSocket != null && _WebSocket.State == WebSocketState.Open)
                    {
                        var pingJson = JsonSerializer.Serialize(new { type = "ping" });
                        var pingBytes = Encoding.UTF8.GetBytes(pingJson);

                        await _WebSocket.SendAsync(new ArraySegment<byte>(pingBytes),
                            WebSocketMessageType.Text, true, token);

                        LogOutput.Write(LogOutput.LOG_LEVEL.INFO,
                            $"[{DateTime.Now:yyyyMMddHHmmss}] [INFO] [PingLoop] Sent ping {_HostDefinition}");
                    }
                }
                catch (Exception ex)
                {
                    LogOutput.Write(LogOutput.LOG_LEVEL.ERROR,
                        $"[{DateTime.Now:yyyyMMddHHmmss}] [ERROR] [PingLoop] Error sending ping: {ex.Message} {_HostDefinition}");
                }

                await Task.Delay(30000, token);
            }
        }


        #region タイムライン操作
        /// <summary>
        /// 接続識別子
        /// </summary>
        protected virtual ConnectMainBody? _WebSocketConnectionObj { get; }
        protected virtual TimeLineBasic.ConnectTimeLineKind _TLKind
        {
            set; get;
        } = TimeLineBasic.ConnectTimeLineKind.None;
        public TimeLineBasic.ConnectTimeLineKind TLKind { get { return _TLKind; } }

        /// <summary>
        /// タイムライン展開
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual WebSocketManager OpenTimeLine(string InstanceURL, string? ApiKey)
        {
            throw new NotImplementedException("タイムラインを開けません。");
        }

        /// <summary>
        /// タイムライン展開(持続的)
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual WebSocketManager OpenTimeLineDynamic(string InstanceURL, string ApiKey)
        {
            throw new NotImplementedException("dynamicがありません。");
        }

        /// <summary>
        /// タイムライン取得
        /// </summary>
        /// <param name="WSTimeLine"></param>
        public virtual void ReadTimeLineContinuous(WebSocketManager WSTimeLine)
        {
            throw new NotImplementedException("受信TLがありません。");
        }
        #endregion
    }
}
