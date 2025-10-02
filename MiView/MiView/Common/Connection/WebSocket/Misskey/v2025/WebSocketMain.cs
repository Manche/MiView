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
    internal class WebSocketMain : WebSocketManager
    {
        public static WebSocketMain CreateInstance()
        {
            return new WebSocketMain();
        }

        /// <summary>
        /// 接続識別子
        /// </summary>
        protected ConnectMainBody _WebSocketConnectionObj
        {
            get { return new ConnectMainBody() { channel = "main", id = "hoge" }; }
        }

        /// <summary>
        /// ソケットオープン
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        public WebSocketMain OpenMain(string InstanceURL, string? ApiKey)
        {
            // タイムライン用WebSocket Open
            this.Start(this.GetWSURL(InstanceURL, ApiKey));
            if (this.GetSocketClient() == null || this._WebSocketConnectionObj == null)
            {
                throw new InvalidOperationException("connection is not opened.");
            }

            while (this.GetSocketState() != WebSocketState.Open)
            {
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
        public WebSocketMain OpenMainDynamic(string InstanceURL, string ApiKey)
        {
            // WS取得
            WebSocketMain WSTimeLine = new WebSocketMain();

            // タイムライン用WebSocket Open
            this.Start(WSTimeLine.GetWSURL(InstanceURL, ApiKey));
            if (this.GetSocketClient() == null || this._WebSocketConnectionObj == null)
            {
                throw new InvalidOperationException("connection is not opened.");
            }

            while (WSTimeLine.GetSocketState() != WebSocketState.Open)
            {
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
            }

            return this;
        }

        /// <summary>
        /// main取得
        /// </summary>
        /// <param name="WSTimeLine"></param>
        public static void ReadMainContinuous(WebSocketMain WSTimeLine)
        {
            // バッファは多めに取っておく(どうせあとでカットする)
            var ResponseBuffer = new byte[4096 * 4];
            _ = Task.Run(async () =>
            {
                //if (WSTimeLine.GetSocketState() != WebSocketState.Open)
                //{
                //    WSTimeLine.OnConnectionLost(WSTimeLine, new EventArgs());
                //}
                while (WSTimeLine.GetSocketState() == WebSocketState.Open)
                {
                    // 受信本体
                    try
                    {
                        // 受信可能になるまで待機
                        if (WSTimeLine.GetSocketClient().State != WebSocketState.Open)
                        {
                            System.Diagnostics.Debug.WriteLine(WSTimeLine.GetSocketClient().State);
                        }
                        if (WSTimeLine.GetSocketClient().State != WebSocketState.Open && WSTimeLine._HostUrl != null)
                        {
                            // 再接続
                            await WSTimeLine.GetSocketClient().ConnectAsync(new Uri(WSTimeLine._HostUrl), CancellationToken.None);
                        }
                        while (WSTimeLine.GetSocketState() == WebSocketState.Closed)
                        {
                            // 接続スタンバイ
                        }
                        var Response = await WSTimeLine.GetSocketClient().ReceiveAsync(new ArraySegment<byte>(ResponseBuffer), CancellationToken.None);
                        if (Response.MessageType == WebSocketMessageType.Close)
                        {
                            WSTimeLine.ConnectionAbort();
                            return;
                        }
                        else
                        {
                            var ResponseMessage = Encoding.UTF8.GetString(ResponseBuffer, 0, Response.Count);
                            DbgOutputSocketReceived(ResponseMessage);

                            WSTimeLine.CallDataReceived(ResponseMessage);
                        }
                    }
                    catch (Exception ce)
                    {
                        System.Diagnostics.Debug.WriteLine("receive failed");
                        System.Diagnostics.Debug.WriteLine(WSTimeLine._HostUrl);
                        System.Diagnostics.Debug.WriteLine(ce);

                        try
                        {
                            if (WSTimeLine.GetSocketClient() != null && WSTimeLine.GetSocketClient().State != WebSocketState.Open)
                            {
                                Thread.Sleep(1000);

                                WebSocketMain.ReadMainContinuous(WSTimeLine);
                            }
                        }
                        catch
                        {
                        }

                        WSTimeLine.CallConnectionLost();
                    }
                }
            });
        }

        private static void DbgOutputSocketReceived(string Response)
        {
            System.Diagnostics.Debug.WriteLine(Response);
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
            if (sender.GetType() != typeof(WebSocketMain))
            {
                return;
            }
            // オープンを待つ
            WebSocketMain WS = (WebSocketMain)sender;
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
                if (((WebSocketMain)sender).GetSocketClient() == null)
                {
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("現在の状態：" + ((WebSocketMain)sender).GetSocketClient().State);
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
