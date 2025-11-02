using MiView.Common.AnalyzeData;
using MiView.Common.Connection.VersionInfo;
using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.Connection.WebSocket.Misskey.v2025;
using MiView.Common.Connection.WebSocket.Structures;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
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
        /// 再接続
        /// </summary>
        public void CreateAndReOpen()
        {
            var _ = Task.Run(async () => { await _CreateAndOpen(this._HostDefinition); });
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
                while (!token.IsCancellationRequested && _WebSocket.State == WebSocketState.Open)
                {
                    sb.Clear();
                    WebSocketReceiveResult result;

                    do
                    {
                        result = await _WebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                        if (result.MessageType == WebSocketMessageType.Close ||
                            _WebSocket.State != WebSocketState.Open)
                        {
                            try
                            {
                                try
                                {
                                    await _WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token);
                                }
                                catch
                                {
                                    // 解放済み
                                }
                                _IsOpenTimeLine = false;
                                CallConnectionLost();

                                // 即座に再接続
                                Debug.WriteLine("WebSocket closed. Attempting immediate reconnect...");
                                try
                                {
                                    CreateAndReOpen();
                                }
                                catch (Exception rex)
                                {
                                    CallError(rex);
                                }

                                return;
                            }
                            catch
                            {
                            }
                        }

                        sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                        Thread.Sleep(1000);

                    } while (!result.EndOfMessage);

                    var message = sb.ToString();
                    var totalLength = Encoding.UTF8.GetByteCount(message);

                    Debug.WriteLine($"受信完了: {totalLength} bytes"); // 内部バイト長確認
                    CallDataReceived(message);
                    Thread.Sleep(1000);
                    _IsOpenTimeLine = true;
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
