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
    internal class WebSocketManager
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

            int TryCnt = 0;
            while (!this._ConnectionClose)
            {
                TryCnt++;

                Thread.Sleep(1000);

                if (_WebSocket.State != WebSocketState.Open)
                {
                    await CreateAndOpen(this._HostUrl);
                }
                if (TryCnt > 10)
                {
                    return;
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
                throw new InvalidOperationException("Socket is already opened");
            }

            ClientWebSocket? WS = null;
            try
            {
                WS = new ClientWebSocket();
                //if (this._WebSocket != null)
                //{
                //    WS = this._WebSocket;
                //}
                WS.Options.KeepAliveInterval = TimeSpan.Zero;

                await WS.ConnectAsync(new Uri(this._HostUrl), CancellationToken.None);

                if (WS.State != WebSocketState.Open)
                {
                    // throw new InvalidOperationException("connection is not opened.");
                }

                this._WebSocket = WS;
                this._State = WS.State;
            }
            catch (Exception ex)
            {
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
    }
}
