using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.Connection.WebSocket.Structures;
using MiView.Common.TimeLine;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using MiView.Common.AnalyzeData;

namespace MiView.Common.Connection.WebSocket.Misskey.v2025
{
    abstract class WebSocketTimeLineCommon : WebSocketManager
    {
        /// <summary>
        /// 接続識別子
        /// </summary>
        abstract protected ConnectMainBody? _WebSocketConnectionObj { get; }

        /// <summary>
        /// 接続先タイムライン
        /// </summary>
        protected TimeLineBasic.ConnectTimeLineKind _TLKind
        {
            set; get;
        } = TimeLineBasic.ConnectTimeLineKind.None;

        /// <summary>
        /// インスタンス作成
        /// </summary>
        /// <returns></returns>
        public static WebSocketTimeLineCommon CreateInstance(TimeLineBasic.ConnectTimeLineKind TLKind)
        {
            switch(TLKind)
            {
                case TimeLineBasic.ConnectTimeLineKind.None:
                    break;
                case TimeLineBasic.ConnectTimeLineKind.Home:
                    return new WebSocketTimeLineHome();
                case TimeLineBasic.ConnectTimeLineKind.Local:
                    return new WebSocketTimeLineLocal();
                case TimeLineBasic.ConnectTimeLineKind.Social:
                    return new WebSocketTimeLineSocial();
                case TimeLineBasic.ConnectTimeLineKind.Global:
                    return new WebSocketTimeLineGlobal();
            }
            return null;
        }

        // あとで
        //public WebSocketTimeLineCommon OpenTimeLine(ConnectTimeLineKind TLKind, string InstanceURL, string? ApiKey)
        //{
        //}

        /// <summary>
        /// タイムライン展開
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public WebSocketTimeLineCommon OpenTimeLine(string InstanceURL, string? ApiKey)
        {
            // タイムライン用WebSocket Open
            this.Start(this.GetWSURL(InstanceURL, ApiKey));
            if (this.GetSocketClient() == null || this._WebSocketConnectionObj == null)
            {
                throw new InvalidOperationException("connection is not opened.");
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
        /// タイムライン展開(持続的)
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public WebSocketTimeLineCommon OpenTimeLineDynamic(string InstanceURL, string ApiKey)
        {
            // WS取得
            WebSocketTimeLineCommon WSTimeLine = WebSocketTimeLineCommon.CreateInstance(this._TLKind);

            // タイムライン用WebSocket Open
            this.Start(WSTimeLine.GetWSURL(InstanceURL, ApiKey));
            if (this.GetSocketClient() == null || this._WebSocketConnectionObj == null)
            {
                throw new InvalidOperationException("connection is not opened.");
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
        /// タイムライン取得
        /// </summary>
        /// <param name="WSTimeLine"></param>
        public static void ReadTimeLineContinuous(WebSocketTimeLineCommon WSTimeLine)
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

                        if (WSTimeLine.GetSocketClient() != null && WSTimeLine.GetSocketClient().State != WebSocketState.Open)
                        {
                            Thread.Sleep(1000);

                            WebSocketTimeLineCommon.ReadTimeLineContinuous(WSTimeLine);
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
            if (sender.GetType() != typeof(WebSocketTimeLineCommon))
            {
                return;
            }
            // オープンを待つ
            WebSocketTimeLineCommon WS = (WebSocketTimeLineCommon)sender;
            while (WS.GetSocketState() != WebSocketState.Open)
            {
                // 1分おき
                Thread.Sleep(1000 * 60 * 1);
                System.Diagnostics.Debug.WriteLine("待機中（　＾ω＾）");
                try
                {
                    WS.OpenTimeLineDynamic(this._HostDefinition, this._APIKey);
                }
                catch (Exception)
                {
                }
                System.Diagnostics.Debug.WriteLine("現在の状態：" + ((WebSocketTimeLineCommon)sender).GetSocketClient().State);
            }
            if (WS == null)
            {
                // 必ず入ってるはず
                return;
            }

            ReadTimeLineContinuous(WS);
        }

        protected override void OnDataReceived(object? sender, ConnectDataReceivedEventArgs e)
        {
            if (this._TimeLineObject == null)
            {
                // objectがない場合
                return;
            }
            if (e.MessageRaw == null)
            {
                // データ受信不可能の場合
                return;
            }
            try
            {
                dynamic Res = System.Text.Json.JsonDocument.Parse(e.MessageRaw);
                var t = JsonNode.Parse(e.MessageRaw);

                // ChannelToTimeLineData.Type(t);

                foreach (DataGridTimeLine DGrid in this._TimeLineObject)
                {
                    TimeLineContainer TLCon = ChannelToTimeLineContainer.ConvertTimeLineContainer(this._HostDefinition, t);
                    if (DGrid.InvokeRequired)
                    {
                        if (!DGrid._IsFiltered)
                        {
                            // 通常TL
                            try
                            {
                                DGrid.Invoke(() => { DGrid.InsertTimeLineData(TLCon); });
                            }
                            catch (Exception ce)
                            {
                                System.Diagnostics.Debug.WriteLine(ce.ToString());
                            }
                        }
                        else
                        {
                            // フィルタTL
                            DGrid.Invoke(() => {

                                DGrid.SetTimeLineFilter(TLCon);

                                if (DGrid._FilteringOptions.FindAll(r => { return r.FilterResult(); }).Count() == DGrid._FilteringOptions.Count())
                                {
                                    // 通常TL
                                    try
                                    {
                                        DGrid.InsertTimeLineData(TLCon);
                                    }
                                    catch (Exception ce)
                                    {
                                        System.Diagnostics.Debug.WriteLine(ce.ToString());
                                    }
                                }
                                System.Diagnostics.Debug.WriteLine(DGrid.Name);
                                System.Diagnostics.Debug.WriteLine("サーチ数：" + DGrid._FilteringOptions.FindAll(r => { return r.FilterResult(); }).Count() + "/結果：" + DGrid._FilteringOptions.Count());
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine(e.MessageRaw);
            }
        }
    }
}
