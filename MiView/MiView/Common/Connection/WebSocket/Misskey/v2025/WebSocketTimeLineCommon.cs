using MiView.Common.AnalyzeData;
using MiView.Common.Connection.VersionInfo;
using MiView.Common.Connection.WebSocket.Controller;
using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.Connection.WebSocket.Structures;
using MiView.Common.TimeLine;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MiView.Common.Connection.WebSocket.Misskey.v2025
{
    abstract class WebSocketTimeLineCommon : WebSocketManager
    {
        protected override string GetWSURL(string InstanceURL, string? APIKey)
        {
            this._HostDefinition = InstanceURL;
            this._APIKey = APIKey;

            _OHost = APIKey != null ? $"wss://{InstanceURL}/streaming?i={APIKey}" : $"wss://{InstanceURL}/streaming";
            return APIKey != null ? $"wss://{InstanceURL}/streaming?i={APIKey}" : $"wss://{InstanceURL}/streaming";
        }

        #region タイムライン操作
        /// <summary>
        /// タイムライン展開
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override WebSocketManager OpenTimeLine(string InstanceURL, string? ApiKey)
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
                Thread.Sleep(1000);
            }

            return this;
        }


        #endregion

        /// <summary>
        /// タイムライン展開(持続的)
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override WebSocketTimeLineCommon OpenTimeLineDynamic(string InstanceURL, string ApiKey)
        {
            // WS取得
            WebSocketManager? WSTimeLine =
                WebSocketTimeLineController.CreateWSTLManager(this.SoftwareVersion.SoftwareType, this.SoftwareVersion.Version, this._TLKind);
            if (WSTimeLine == null && WSTimeLine.GetType() != typeof(WebSocketTimeLineCommon))
            {
                throw new InvalidOperationException("インスタンスの生成に失敗しました。");
            }
            WSTimeLine = ((WebSocketTimeLineCommon)WSTimeLine);

            // タイムライン用WebSocket Open
            this.Start(this.GetWSURL(InstanceURL, ApiKey));
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
                        this.OnConnectionLost(WSTimeLine, new EventArgs());
                        _IsOpenTimeLine = false;
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
        /// タイムライン取得
        /// </summary>
        /// <param name="WSTimeLine"></param>
        public override void ReadTimeLineContinuous(WebSocketManager WSTimeLine)
        {
            // バッファは多めに取っておく(どうせあとでカットする)
            var ResponseBuffer = new byte[4096 * 4];
            try
            {
                _ = Task.Run(async () =>
                {
                    while (true)
                    {
                        try
                        {
                            // 接続確認
                            if (WSTimeLine.GetSocketClient().State != WebSocketState.Open)
                            {
                                Debug.WriteLine($"[WebSocket] State={WSTimeLine.GetSocketClient().State}, reconnecting...");
                                WSTimeLine.CreateAndReOpen();
                                await Task.Delay(1000);
                                continue;
                            }

                            // 受信処理
                            var Response = await WSTimeLine.GetSocketClient().ReceiveAsync(
                                new ArraySegment<byte>(ResponseBuffer),
                                CancellationToken.None);

                            if (Response.MessageType == WebSocketMessageType.Close)
                            {
                                Debug.WriteLine("WebSocket closed by server — reconnecting...");
                                WSTimeLine.CreateAndReOpen();
                                await Task.Delay(1000);
                                continue;
                            }

                            // 正常受信
                            var ResponseMessage = Encoding.UTF8.GetString(ResponseBuffer, 0, Response.Count);
                            DbgOutputSocketReceived(ResponseMessage);
                            WSTimeLine.CallDataReceived(ResponseMessage);
                            WSTimeLine._IsOpenTimeLine = true;
                        }
                        catch (WebSocketException ex)
                        {
                            Debug.WriteLine($"WebSocketException: {ex.Message} — reconnecting...");
                            WSTimeLine.CreateAndReOpen();
                            await Task.Delay(1000);
                            continue;
                        }
                        catch (OperationCanceledException)
                        {
                            Debug.WriteLine("WebSocket receive aborted — reconnecting...");
                            WSTimeLine.CreateAndReOpen();
                            await Task.Delay(1000);
                            continue;
                        }
                        catch (Exception ce)
                        {
                            Debug.WriteLine("receive failed");
                            Debug.WriteLine(WSTimeLine._HostUrl);
                            Debug.WriteLine(ce);

                            WSTimeLine._IsOpenTimeLine = false;
                            WSTimeLine.CallConnectionLost();
                            WSTimeLine.SetSocketState(WebSocketState.Closed);

                            WSTimeLine.CreateAndReOpen();
                            await Task.Delay(1000);
                        }

                        await Task.Delay(1000);
                    }
                });
            }
            catch (OperationCanceledException)
            {
                WSTimeLine._IsOpenTimeLine = false;
            }
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
                WS._IsOpenTimeLine = false;

                _IsOpenTimeLine = false;
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

                _IsOpenTimeLine = true;
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
                            DGrid.Invoke(() => {

                                lock (DGrid)
                                {
                                    DGrid.SetTimeLineFilter(TLCon);

                                    int Found = DGrid._FilteringOptions.FindAll(r => { return r.FilterResult(); }).Count();
                                    int Filted = DGrid._FilteringOptions.Count();

                                    bool CountRet = false;
                                    if (DGrid._FilterMode)
                                    {
                                        CountRet = Found == Filted;
                                    }
                                    else
                                    {
                                        CountRet = Found > 0;
                                    }

                                    if (CountRet)
                                    {
                                        // 通常TL
                                        try
                                        {
                                            DGrid.InsertTimeLineData(TLCon);

                                            foreach (TimeLineAlertOption Opt in DGrid._AlertAccept)
                                            {
                                                Found = Opt._FilterOptions.FindAll(r => { return r.FilterResult(); }).Count();
                                                Filted = Opt._FilterOptions.Count();

                                                CountRet = false;
                                                if (Opt._FilterMode)
                                                {
                                                    CountRet = Found == Filted;
                                                }
                                                else
                                                {
                                                    CountRet = Found > 0;
                                                }
                                                if (CountRet)
                                                {
                                                    Opt.ExecuteAlert(TLCon);
                                                }
                                            }
                                            CallDataAccepted(TLCon);
                                        }
                                        catch (Exception ce)
                                        {
                                            System.Diagnostics.Debug.WriteLine(ce.ToString());
                                        }
                                    }
                                    else
                                    {
                                        foreach (TimeLineAlertOption Opt in DGrid._AlertReject)
                                        {
                                            Found = Opt._FilterOptions.FindAll(r => { return r.FilterResult(); }).Count();
                                            Filted = Opt._FilterOptions.Count();

                                            CountRet = false;
                                            if (Opt._FilterMode)
                                            {
                                                CountRet = Found == Filted;
                                            }
                                            else
                                            {
                                                CountRet = Found > 0;
                                            }
                                            if (CountRet)
                                            {
                                                Opt.ExecuteAlert(TLCon);
                                            }
                                        }
                                        CallDataRejected(TLCon);
                                    }
                                    //System.Diagnostics.Debug.WriteLine(DGrid.Name);
                                    //System.Diagnostics.Debug.WriteLine("サーチ数：" + DGrid._FilteringOptions.FindAll(r => { return r.FilterResult(); }).Count() + "/結果：" + DGrid._FilteringOptions.Count());
                                }
                            });
                        }
                        else
                        {
                            // フィルタTL
                            DGrid.Invoke(() => {

                                lock (DGrid)
                                {
                                    DGrid.SetTimeLineFilter(TLCon);

                                    int Found = DGrid._FilteringOptions.FindAll(r => { return r.FilterResult(); }).Count();
                                    int Filted = DGrid._FilteringOptions.Count();

                                    bool CountRet = false;
                                    if (DGrid._FilterMode)
                                    {
                                        CountRet = Found == Filted;
                                    }
                                    else
                                    {
                                        CountRet = Found > 0;
                                    }

                                    if (CountRet)
                                    {
                                        // 通常TL
                                        try
                                        {
                                            DGrid.InsertTimeLineData(TLCon);
                                            foreach (TimeLineAlertOption Opt in DGrid._AlertAccept)
                                            {
                                                Found = Opt._FilterOptions.FindAll(r => { return r.FilterResult(); }).Count();
                                                Filted = Opt._FilterOptions.Count();

                                                CountRet = false;
                                                if (Opt._FilterMode)
                                                {
                                                    CountRet = Found == Filted;
                                                }
                                                else
                                                {
                                                    CountRet = Found > 0;
                                                }
                                                if (CountRet)
                                                {
                                                    Opt.ExecuteAlert(TLCon);
                                                }
                                            }
                                            CallDataAccepted(TLCon);
                                        }
                                        catch (Exception ce)
                                        {
                                            System.Diagnostics.Debug.WriteLine(ce.ToString());
                                        }
                                    }
                                    else
                                    {
                                        foreach (TimeLineAlertOption Opt in DGrid._AlertReject)
                                        {
                                            Found = Opt._FilterOptions.FindAll(r => { return r.FilterResult(); }).Count();
                                            Filted = Opt._FilterOptions.Count();

                                            CountRet = false;
                                            if (Opt._FilterMode)
                                            {
                                                CountRet = Found == Filted;
                                            }
                                            else
                                            {
                                                CountRet = Found > 0;
                                            }
                                            if (CountRet)
                                            {
                                                Opt.ExecuteAlert(TLCon);
                                            }
                                        }
                                        CallDataRejected(TLCon);
                                    }
                                    //System.Diagnostics.Debug.WriteLine(DGrid.Name);
                                    //System.Diagnostics.Debug.WriteLine("サーチ数：" + DGrid._FilteringOptions.FindAll(r => { return r.FilterResult(); }).Count() + "/結果：" + DGrid._FilteringOptions.Count());
                                }
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
