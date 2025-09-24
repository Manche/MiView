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

namespace MiView.Common.Connection.WebSocket
{
    internal class WebSocketManager
    {
        // ==== 保持していたプロパティ群（アクセス修飾は元のまま）====
        protected string _HostUrl { get; set; } = string.Empty;
        protected string _HostDefinition { get; set; } = string.Empty;
        protected string? _APIKey { get; set; } = string.Empty;

        private WebSocketState _State { get; set; } = WebSocketState.None;
        private WebSocketState _State_Command { get; set; } = WebSocketState.None;
        private bool _ConnectionClose { get; set; } = false;

        protected MainForm _MainForm { get; set; } = new MainForm();
        protected DataGridTimeLine[]? _TimeLineObject { get; set; } = new DataGridTimeLine[0];

        private ClientWebSocket _WebSocket { get; set; } = new ClientWebSocket();
        private CancellationTokenSource? _Cts;

        private bool _IsMisskey = false;   // Misskeyモードフラグ

        // ==== イベント ====
        public event EventHandler<EventArgs>? ConnectionClosed;
        public event EventHandler<EventArgs> ConnectionLost;
        public event EventHandler<ConnectDataReceivedEventArgs> DataReceived;

        // ==== コンストラクタ ====
        public WebSocketManager()
        {
            this.ConnectionLost += OnConnectionLost;
            this.DataReceived += OnDataReceived;
        }

        public WebSocketManager(string HostUrl, bool isMisskey = true) : this()
        {
            this._HostUrl = HostUrl;
            this._IsMisskey = isMisskey;

            Task.Run(async () => await Watcher());
        }

        // ==== 外部API互換 ====
        public ClientWebSocket GetSocketClient() => this._WebSocket;
        public void SetSocketState(WebSocketState State) => this._State = State;
        public WebSocketState? GetSocketState() => this._WebSocket?.State;

        public bool IsStandBySocketOpen() => this.GetSocketState() == WebSocketState.None;

        public void SetDataGridTimeLine(DataGridTimeLine timeLine)
        {
            if (this._TimeLineObject == null)
            {
                this._TimeLineObject = new DataGridTimeLine[0];
            }
            this._TimeLineObject = this._TimeLineObject.Concat(new[] { timeLine }).ToArray();
        }

        protected WebSocketManager Start(string HostUrl)
        {
            this._HostUrl = HostUrl;
            _ = Task.Run(async () => await Watcher());
            return this;
        }

        protected void ConnectionAbort()
        {
            this._ConnectionClose = true;
            _Cts?.Cancel();
        }

        protected string GetWSURL(string InstanceURL, string? APIKey)
        {
            this._HostDefinition = InstanceURL;
            this._APIKey = APIKey;
            return APIKey != null
                ? $"wss://{InstanceURL}/streaming?i={APIKey}"
                : $"wss://{InstanceURL}/streaming";
        }

        // ==== 内部ループ ====
        private async Task Watcher()
        {
            int tryCnt = 0;
            _Cts = new CancellationTokenSource();

            while (!_ConnectionClose && !_Cts.Token.IsCancellationRequested)
            {
                tryCnt++;

                if (_WebSocket == null || _WebSocket.State != WebSocketState.Open)
                {
                    await CreateAndOpen(this._HostUrl, _Cts.Token);
                }

                // 再接続試行上限
                if (tryCnt > 10)
                {
                    CallConnectionLost();
                    return;
                }

                await Task.Delay(5000, _Cts.Token); // 暴走防止
            }
        }

        private async Task CreateAndOpen(string HostUrl, CancellationToken token)
        {
            if (_WebSocket != null && _WebSocket.State == WebSocketState.Open)
                return;

            try
            {
                var ws = new ClientWebSocket();
                ws.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);

                await ws.ConnectAsync(new Uri(HostUrl), token);

                if (ws.State == WebSocketState.Open)
                {
                    this._WebSocket = ws;
                    this._State = ws.State;

                    _ = Task.Run(() => ReceiveLoop(token), token);
                    _ = Task.Run(() => KeepAliveLoop(token), token);
                }
            }
            catch
            {
                // 接続失敗時は無視 → Watcherがリトライ
            }
        }

        private async Task ReceiveLoop(CancellationToken token)
        {
            var buffer = new byte[8192];

            while (!token.IsCancellationRequested && _WebSocket?.State == WebSocketState.Open)
            {
                try
                {
                    var result = await _WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        CallConnectionLost();
                        return;
                    }

                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    CallDataReceived(message);
                }
                catch
                {
                    CallConnectionLost();
                    return;
                }
            }
        }

        // ==== KeepAlive ====
        private async Task KeepAliveLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (_WebSocket?.State == WebSocketState.Open)
                {
                    try
                    {
                        string message = _IsMisskey ? "{ \"type\": \"ping\" }" : "ping";
                        var buffer = Encoding.UTF8.GetBytes(message);

                        await _WebSocket.SendAsync(
                            new ArraySegment<byte>(buffer),
                            WebSocketMessageType.Text,
                            true,
                            token
                        );
                    }
                    catch
                    {
                        CallConnectionLost();
                        return;
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(30), token);
            }
        }

        // ==== Close ====
        protected async Task Close(string HostUrl)
        {
            _HostUrl = HostUrl;

            if (_WebSocket?.State == WebSocketState.Closed)
                return;

            try
            {
                _Cts?.Cancel();
                await _WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
                _State = _WebSocket.State;
                ConnectionClosed?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        // ==== イベント呼び出し ====
        protected virtual void OnConnectionLost(object? sender, EventArgs e) { }
        protected void CallConnectionLost() => ConnectionLost?.Invoke(this, EventArgs.Empty);

        protected virtual void OnDataReceived(object? sender, ConnectDataReceivedEventArgs e) { }
        protected void CallDataReceived(string ResponseMessage) =>
            DataReceived?.Invoke(this, new ConnectDataReceivedEventArgs() { MessageRaw = ResponseMessage });
    }
}
