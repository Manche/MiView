using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.Connection.WebSocket.Structures;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MiView.Common.Connection.WebSocket.Misskey.v2025
{
    internal class WebSocketMainMisskey2025 : WebSocketMain
    {
        /// <summary>
        /// 接続識別子
        /// </summary>
        protected override ConnectMainBody _WebSocketConnectionObj
        {
            get { return new ConnectMainBody() { channel = "main", id = "hoge" }; }
        }

        protected override string GetWSURL(string InstanceURL, string? APIKey)
        {
            this._HostDefinition = InstanceURL;
            this._APIKey = APIKey;

            _OHost = APIKey != null ? $"wss://{InstanceURL}/streaming?i={APIKey}" : $"wss://{InstanceURL}/streaming";
            return APIKey != null ? $"wss://{InstanceURL}/streaming?i={APIKey}" : $"wss://{InstanceURL}/streaming";
        }

        /// <summary>
        /// ソケットオープン
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        public override WebSocketMainMisskey2025 OpenMain(string InstanceURL, string? ApiKey)
        {
            // タイムライン用WebSocket Open
            this.Start(this.GetWSURL(InstanceURL, ApiKey));
            if (this.GetSocketClient() == null || this._WebSocketConnectionObj == null)
            {
                throw new InvalidOperationException("connection is not opened.");
            }

            while (this.GetSocketState() != WebSocketState.Open)
            {
                Thread.Sleep(1000);
            }
            int RetryCnt = 0;
            while (this.IsStandBySocketOpen())
            {
                Thread.Sleep(1000);
                RetryCnt++;
                if (RetryCnt > 10)
                {
                    throw new InvalidOperationException("connection is not opened.");
                }
                else
                {
                    this.OnConnectionLost(this, new EventArgs());
                }
            }

            // チャンネル接続用
            ConnectMain SendObj = new ConnectMain();
            ConnectMainBody SendBody = this._WebSocketConnectionObj;
            SendObj.type = "connect";
            SendObj.body = SendBody;

            var SendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(SendObj));
            var Buffers = new ArraySegment<byte>(SendBytes);

            // ソケットのステータスを一旦リセットする(同じソケット使うので)
            this.SetSocketState(WebSocketState.None);
            Task.Run(async () =>
            {
                // 本チャンのwebsocket接続
                await this.GetSocketClient().SendAsync(Buffers, WebSocketMessageType.Text, true, CancellationToken.None);
            });
            while (this.IsStandBySocketOpen())
            {
                Thread.Sleep(1000);
            }

            return this;
        }

        /// <summary>
        /// ソケット展開(持続的)
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override WebSocketMainMisskey2025 OpenMainDynamic(string InstanceURL, string ApiKey)
        {
            // WS取得
            WebSocketMainMisskey2025 WSTimeLine = new WebSocketMainMisskey2025();

            // タイムライン用WebSocket Open
            this.Start(WSTimeLine.GetWSURL(InstanceURL, ApiKey));
            if (this.GetSocketClient() == null || this._WebSocketConnectionObj == null)
            {
                throw new InvalidOperationException("connection is not opened.");
            }

            while (WSTimeLine.GetSocketState() != WebSocketState.Open)
            {
                Thread.Sleep(1000);
            }
            int RetryCnt = 0;
            while (WSTimeLine.IsStandBySocketOpen())
            {
                Thread.Sleep(1000);
                RetryCnt++;
                if (RetryCnt > 10)
                {
                    if (WSTimeLine.GetSocketState() != WebSocketState.Open)
                    {
                        WSTimeLine.OnConnectionLost(WSTimeLine, new EventArgs());
                    }
                }
            }

            // チャンネル接続用
            ConnectMain SendObj = new ConnectMain();
            ConnectMainBody SendBody = this._WebSocketConnectionObj;
            SendObj.type = "connect";
            SendObj.body = SendBody;

            var SendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(SendObj));
            var Buffers = new ArraySegment<byte>(SendBytes);

            // ソケットのステータスを一旦リセットする(同じソケット使うので)
            this.SetSocketState(WebSocketState.None);
            Task.Run(async () =>
            {
                // 本チャンのwebsocket接続
                await this.GetSocketClient().SendAsync(Buffers, WebSocketMessageType.Text, true, CancellationToken.None);
            });
            while (this.IsStandBySocketOpen())
            {
                Thread.Sleep(1000);
            }

            return this;
        }

        /// <summary>
        /// 接続喪失時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnConnectionLost(object? sender, EventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            if (sender.GetType() != typeof(WebSocketMainMisskey2025))
            {
                return;
            }
            // オープンを待つ
            WebSocketMainMisskey2025 WS = (WebSocketMainMisskey2025)sender;
            while (WS.GetSocketState() != WebSocketState.Open)
            {
                // 1分おき
                Thread.Sleep(1000 * 60 * 1);
                System.Diagnostics.Debug.WriteLine("待機中（　＾ω＾）");
                try
                {
                    WS.OpenMainDynamic(this._HostDefinition, this._APIKey);
                }
                catch (Exception)
                {
                }
                if (((WebSocketMainMisskey2025)sender).GetSocketClient() == null)
                {
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("現在の状態：" + ((WebSocketMainMisskey2025)sender).GetSocketClient().State);
                }
            }
            if (WS == null)
            {
                // 必ず入ってるはず
                return;
            }

            ReadMainContinuous(WS);
        }

        protected override void OnDataReceived(object? sender, ConnectDataReceivedEventArgs e)
        {
            if (e.MessageRaw == null)
            {
                // データ受信不可能の場合
                return;
            }

            try
            {
                dynamic Res = System.Text.Json.JsonDocument.Parse(e.MessageRaw);
                var t = JsonNode.Parse(e.MessageRaw);
                System.Diagnostics.Debug.WriteLine(t);
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine(e.MessageRaw);
            }
        }
    }
}
