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
        protected string _HostUrl { get; set; } = string.Empty;
        protected string _HostDefinition { get; set; } = string.Empty;
        protected string? _APIKey { get; set; } = string.Empty;

        private WebSocketState _State { get; set; } = WebSocketState.None;
        private WebSocketState _State_Command { get; set; } = WebSocketState.None;
        private bool _ConnectionClose { get; set; } = false;

        protected MainForm _MainForm { get; set; } = new MainForm();
        protected DataGridTimeLine[]? _TimeLineObject { get; set; } = new DataGridTimeLine[0];

        private ClientWebSocket _WebSocket { get; set; } = new ClientWebSocket();
        private CancellationTokenSource _Cancellation = new CancellationTokenSource();

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
            this._HostUrl = HostUrl;
            _ = Task.Run(async () => await Watcher());
        }

        public void SetDataGridTimeLine(DataGridTimeLine timeLine)
        {
            if (this._TimeLineObject == null) this._TimeLineObject = new DataGridTimeLine[0];
            this._TimeLineObject = this._TimeLineObject.Concat(new DataGridTimeLine[] { timeLine }).ToArray();
        }

        public ClientWebSocket GetSocketClient() => this._WebSocket;
        public WebSocketState? GetSocketState() => this._WebSocket?.State;

        public void SetSocketState(WebSocketState State) => this._State = State;
        public bool IsStandBySocketOpen() => GetSocketState() == WebSocketState.None;
        protected void ConnectionAbort() => this._ConnectionClose = true;

        protected WebSocketManager Start(string HostUrl)
        {
            this._HostUrl = HostUrl;
            _ = Task.Run(async () => await Watcher());
            return this;
        }

        protected string GetWSURL(string InstanceURL, string? APIKey)
        {
            this._HostDefinition = InstanceURL;
            this._APIKey = APIKey;

            return APIKey != null ? $"wss://{InstanceURL}/streaming?i={APIKey}" : $"wss://{InstanceURL}/streaming";
        }

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

        private async Task Watcher()
        {
            if (_WebSocket == null) _WebSocket = new ClientWebSocket();

            while (!_ConnectionClose)
            {
                try
                {
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
            _HostUrl = HostUrl;

            if (_State == WebSocketState.Open)
                return;

            try
            {
                var WS = new ClientWebSocket();
                WS.Options.KeepAliveInterval = TimeSpan.Zero;
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
            _HostUrl = HostUrl;

            if (_State == WebSocketState.Closed)
                throw new InvalidOperationException("Socket is already closed");

            try
            {
                await _WebSocket.CloseAsync(WebSocketCloseStatus.InternalServerError, null, CancellationToken.None);
                while (_WebSocket.State != WebSocketState.Closed && _WebSocket.State != WebSocketState.Aborted) ;
                _State = _WebSocket.State;
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
                while (!token.IsCancellationRequested && _WebSocket.State == WebSocketState.Open)
                {
                    sb.Clear();
                    WebSocketReceiveResult result;

                    do
                    {
                        result = await _WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await _WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
                            CallConnectionLost();
                            return;
                        }

                        sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                    } while (!result.EndOfMessage);

                    var message = sb.ToString();
                    var totalLength = Encoding.UTF8.GetByteCount(message);

                    Debug.WriteLine($"受信完了: {totalLength} bytes"); // 内部バイト長確認
                    CallDataReceived(message);
                }
            }
            catch (Exception ex)
            {
                CallError(ex);
            }
            finally
            {
                // CPU暴走防止用に少し待ってから再接続
                await Task.Delay(1000, token);
            }
        }

        private void CallError(Exception ex)
        {
            Debug.WriteLine($"WebSocketManager Error: {ex}");
        }
    }
}
