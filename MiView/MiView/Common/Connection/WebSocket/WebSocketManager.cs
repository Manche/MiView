using MiView.Common.AnalyzeData;
using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiView.Common.Connection.WebSocket
{
    internal class WebSocketManager
    {
        // ------------------------------
        // 既存フィールドを保持
        // ------------------------------
        protected string _HostUrl { get; set; } = string.Empty;
        protected string _HostDefinition { get; set; } = string.Empty;
        protected string? _APIKey { get; set; } = string.Empty;

        private WebSocketState _State { get; set; } = WebSocketState.None;
        private WebSocketState _State_Command { get; set; } = WebSocketState.None;
        private bool _ConnectionClose { get; set; } = false;
        protected MainForm _MainForm { get; set; } = new MainForm();
        protected DataGridTimeLine[]? _TimeLineObject { get; set; } = new DataGridTimeLine[0];

        private ClientWebSocket _WebSocket { get; set; } = new ClientWebSocket();

        // ------------------------------
        // 新規内部管理用
        // ------------------------------
        private CancellationTokenSource? _watcherCts;
        private SemaphoreSlim _reconnectLock = new SemaphoreSlim(1, 1);
        private System.Threading.Timer? _keepAliveTimer;
        private bool _Accepted = false;

        // ------------------------------
        // 外部イベント
        // ------------------------------
        public event EventHandler<EventArgs>? ConnectionClosed;
        public event EventHandler<EventArgs> ConnectionLost;
        public event EventHandler<ConnectDataReceivedEventArgs> DataReceived;
        public event EventHandler<DataContainerEventArgs>? DataAccepted;
        public event EventHandler<DataContainerEventArgs>? DataRejected;

        // ------------------------------
        // コンストラクタ
        // ------------------------------
        public WebSocketManager()
        {
            ConnectionLost += OnConnectionLost;
            DataReceived += OnDataReceived;
        }

        public WebSocketManager(string HostUrl) : this()
        {
            _HostUrl = HostUrl;
            Task.Run(async () => await Watcher());
        }

        // ------------------------------
        // 既存 public メソッド保持
        // ------------------------------
        public WebSocketManager Start(string HostUrl)
        {
            _HostUrl = HostUrl;
            Task.Run(async () => await Watcher());
            return this;
        }

        public ClientWebSocket GetSocketClient() => _WebSocket;
        public void SetSocketState(WebSocketState State) => _State = State;
        public WebSocketState? GetSocketState() => _WebSocket?.State;
        public bool IsStandBySocketOpen() => GetSocketState() == WebSocketState.None;

        public void SetDataGridTimeLine(DataGridTimeLine timeLine)
        {
            _TimeLineObject = _TimeLineObject?.Concat(new DataGridTimeLine[] { timeLine }).ToArray() ?? new DataGridTimeLine[] { timeLine };
        }

        // ------------------------------
        // 既存イベント呼び出し保持
        // ------------------------------

        protected virtual void OnConnectionLost(object? sender, EventArgs e) { }
        protected void CallConnectionLost() => ConnectionLost?.Invoke(this, new EventArgs());

        protected virtual void OnDataReceived(object? sender, ConnectDataReceivedEventArgs e)
        {
        }
        protected void CallDataReceived(string ResponseMessage) => DataReceived?.Invoke(this, new ConnectDataReceivedEventArgs() { MessageRaw = ResponseMessage });
        protected virtual void OnDataAccepted(object? sender, DataContainerEventArgs Container)
        {
            this._MainForm.CallDataAccepted(Container.Container);
        }
        protected void CallDataAccepted(TimeLineContainer Container) => DataAccepted?.Invoke(this, new DataContainerEventArgs());
        protected virtual void OnDataRejected(object? sender, DataContainerEventArgs Container)
        {
            this._MainForm.CallDataRejected(Container.Container);
        }
        protected void CallDataRejected(TimeLineContainer Container) => DataRejected?.Invoke(this, new DataContainerEventArgs());

        // ------------------------------
        // 内部接続管理
        // ------------------------------
        protected void ConnectionAbort() => _ConnectionClose = true;

        protected string GetWSURL(string InstanceURL, string? APIKey)
        {
            _HostDefinition = InstanceURL;
            _APIKey = APIKey;
            return APIKey != null ? $"wss://{InstanceURL}/streaming?i={APIKey}" : $"wss://{InstanceURL}/streaming";
        }

        // ------------------------------
        // Watcher + 再接続 + KeepAlive
        // ------------------------------
        private async Task Watcher()
        {
            if (_WebSocket == null) _WebSocket = new ClientWebSocket();
            if (_watcherCts != null) return;

            _watcherCts = new CancellationTokenSource();
            var token = _watcherCts.Token;

            // KeepAlive: 30秒ごとに Ping
            _keepAliveTimer = new System.Threading.Timer(async _ =>
            {
                if (_WebSocket.State == WebSocketState.Open)
                {
                    try
                    {
                        var ping = Encoding.UTF8.GetBytes("ping");
                        await _WebSocket.SendAsync(ping, WebSocketMessageType.Text, true, token);
                    }
                    catch { }
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

            // 監視ループ
            while (!token.IsCancellationRequested && !_ConnectionClose)
            {
                if (_WebSocket.State != WebSocketState.Open)
                {
                    await TryReconnect();
                }
                await Task.Delay(5000, token);
            }
        }

        private async Task TryReconnect()
        {
            await _reconnectLock.WaitAsync();
            try
            {
                if (_WebSocket.State == WebSocketState.Open) return;

                _WebSocket.Dispose();
                _WebSocket = new ClientWebSocket();
                await _WebSocket.ConnectAsync(new Uri(GetWSURL(_HostDefinition, _APIKey)), CancellationToken.None);
                _State = _WebSocket.State;
                _Accepted = (_WebSocket.State == WebSocketState.Open);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("再接続失敗: " + ex.Message);
            }
            finally
            {
                _reconnectLock.Release();
            }
        }
    }
}
