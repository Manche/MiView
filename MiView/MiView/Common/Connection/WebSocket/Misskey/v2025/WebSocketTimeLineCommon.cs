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
        protected ConnectTimeLineKind _TLKind
        {
            set; get;
        } = ConnectTimeLineKind.None;

        /// <summary>
        /// 接続先タイムライン指定
        /// </summary>
        public enum ConnectTimeLineKind
        {
            None,
            Home,
            Local,
            Social,
            Global,
        }

        /// <summary>
        /// インスタンス作成
        /// </summary>
        /// <returns></returns>
        public static WebSocketTimeLineCommon? CreateInstance(ConnectTimeLineKind TLKind)
        {
            switch(TLKind)
            {
                case ConnectTimeLineKind.None:
                    break;
                case ConnectTimeLineKind.Home:
                    return new WebSocketTimeLineHome();
                case ConnectTimeLineKind.Local:
                    return new WebSocketTimeLineLocal();
                case ConnectTimeLineKind.Social:
                    return new WebSocketTimeLineSocial();
                case ConnectTimeLineKind.Global:
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
            try
            {
                // タイムライン用WebSocket Open
                var wsUrl = this.GetWSURL(InstanceURL, ApiKey);
                System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Attempting to connect to: {wsUrl}");
                System.Diagnostics.Debug.WriteLine($"OpenTimeLine: InstanceURL={InstanceURL}, ApiKey provided={!string.IsNullOrEmpty(ApiKey)}");
                
                this.Start(wsUrl);
                
                // 接続完了を待つ（タイムアウト付き）
                int RetryCnt = 0;
                const int maxRetries = 15; // タイムアウトを15秒に延長
                
                while (this.GetSocketState() != WebSocketState.Open && RetryCnt < maxRetries)
                {
                    Thread.Sleep(1000);
                    RetryCnt++;
                    System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Connection attempt {RetryCnt}/{maxRetries}, State: {this.GetSocketState()}");
                    
                    if (RetryCnt >= maxRetries)
                    {
                        if (this.GetSocketState() != WebSocketState.Open)
                        {
                            System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Connection failed after {maxRetries} attempts. Final state: {this.GetSocketState()}");
                            this.OnConnectionLost(this, new EventArgs());
                            throw new InvalidOperationException($"WebSocket connection failed to open after {maxRetries} seconds");
                        }
                    }
                }
                
                System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Connection check - SocketClient: {this.GetSocketClient() != null}, WebSocketConnectionObj: {this._WebSocketConnectionObj != null}");
                if (this.GetSocketClient() == null || this._WebSocketConnectionObj == null)
                {
                    System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Connection objects are null - SocketClient: {this.GetSocketClient()}, WebSocketConnectionObj: {this._WebSocketConnectionObj}");
                    throw new InvalidOperationException("connection is not opened.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OpenTimeLine error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"OpenTimeLine error details: {ex}");
                throw;
            }

            // チャンネル接続用
            System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Creating connection message");
            ConnectMain SendObj = new ConnectMain();
            ConnectMainBody SendBody = this._WebSocketConnectionObj;
            SendObj.type = "connect";
            SendObj.body = SendBody;
            System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Connection message created - type: {SendObj.type}, channel: {SendBody?.channel}");

            var SendBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(SendObj));
            var Buffers = new ArraySegment<byte>(SendBytes);
            System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Message serialized, length: {SendBytes.Length}");

            // ソケットのステータスを一旦リセットする(同じソケット使うので)
            this.SetSocketState(WebSocketState.None);
            System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Sending connection message");
            Task.Run(async () =>
            {
                try
                {
                    // 本チャンのwebsocket接続
                    await this.GetSocketClient().SendAsync(Buffers, WebSocketMessageType.Text, true, CancellationToken.None);
                    System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Connection message sent successfully");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Error sending connection message: {ex.Message}");
                }
            });
            
            // 送信完了を待つ（タイムアウト付き）
            int retryCount = 0;
            while (this.IsStandBySocketOpen() && retryCount < 5)
            {
                Thread.Sleep(1000);
                retryCount++;
                System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Waiting for send completion, attempt {retryCount}/5");
            }

            System.Diagnostics.Debug.WriteLine($"OpenTimeLine: Connection setup completed");
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
            try
            {
                // WS取得
                WebSocketTimeLineCommon? WSTimeLine = WebSocketTimeLineCommon.CreateInstance(this._TLKind);
                if (WSTimeLine == null)
                {
                    throw new InvalidOperationException("Failed to create WebSocket timeline instance");
                }

                // タイムライン用WebSocket Open
                var wsUrl = WSTimeLine.GetWSURL(InstanceURL, ApiKey);
                System.Diagnostics.Debug.WriteLine($"Attempting to connect to: {wsUrl}");
                
                this.Start(wsUrl);
                
                if (this.GetSocketClient() == null || this._WebSocketConnectionObj == null)
                {
                    throw new InvalidOperationException("connection is not opened.");
                }
                
                // 接続完了を待つ（タイムアウト付き）
                int RetryCnt = 0;
                const int maxRetries = 15; // タイムアウトを15秒に延長
                
                while (WSTimeLine.GetSocketState() != WebSocketState.Open && RetryCnt < maxRetries)
                {
                    Thread.Sleep(1000);
                    RetryCnt++;
                    System.Diagnostics.Debug.WriteLine($"Connection attempt {RetryCnt}/{maxRetries}, State: {WSTimeLine.GetSocketState()}");
                    
                    if (RetryCnt >= maxRetries)
                    {
                        if (WSTimeLine.GetSocketState() != WebSocketState.Open)
                        {
                            System.Diagnostics.Debug.WriteLine($"Connection failed after {maxRetries} attempts. Final state: {WSTimeLine.GetSocketState()}");
                            WSTimeLine.OnConnectionLost(WSTimeLine, new EventArgs());
                            throw new InvalidOperationException($"WebSocket connection failed to open after {maxRetries} seconds");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OpenTimeLineDynamic error: {ex.Message}");
                throw;
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
            
            // 送信完了を待つ（タイムアウト付き）
            int retryCount = 0;
            while (this.IsStandBySocketOpen() && retryCount < 5)
            {
                Thread.Sleep(1000);
                retryCount++;
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
            var threadId = Thread.CurrentThread.ManagedThreadId;
#if DEBUG
            Console.WriteLine($"📡 READER THREAD STARTED: {WSTimeLine._HostDefinition} (Thread: {threadId})");
#endif
            
            _ = Task.Run(async () =>
            {
                var taskId = Task.CurrentId;
#if DEBUG
                Console.WriteLine($"🎯 ASYNC READER TASK: {WSTimeLine._HostDefinition} (Task: {taskId}, Thread: {Thread.CurrentThread.ManagedThreadId})");
#endif
                
                //if (WSTimeLine.GetSocketState() != WebSocketState.Open)
                //{
                //    WSTimeLine.OnConnectionLost(WSTimeLine, new EventArgs());
                //}
                var loopCount = 0;
                while (WSTimeLine.GetSocketState() == WebSocketState.Open)
                {
                    loopCount++;
#if DEBUG
                    if (loopCount % 10 == 1) // 10回に1回ログを出力
                    {
                        Console.WriteLine($"🔄 READING LOOP: {WSTimeLine._HostDefinition} - Loop #{loopCount} (Task: {taskId})");
                    }
#endif
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
#if DEBUG
                        if (loopCount % 50 == 1) // 50回に1回、受信待機状態をログ出力
                        {
                            Console.WriteLine($"⏳ WAITING FOR DATA: {WSTimeLine._HostDefinition} (Task: {taskId})");
                        }
#endif
                        var Response = await WSTimeLine.GetSocketClient().ReceiveAsync(new ArraySegment<byte>(ResponseBuffer), CancellationToken.None);
                        if (Response.MessageType == WebSocketMessageType.Close)
                        {
                            WSTimeLine.ConnectionAbort();
                            return;
                        }
                        else
                        {
                            var ResponseMessage = Encoding.UTF8.GetString(ResponseBuffer, 0, Response.Count);
#if DEBUG
                            Console.WriteLine($"💬 MSG RECEIVED: {WSTimeLine._HostDefinition} - Length: {Response.Count} (Task: {taskId})");
#endif
                            DbgOutputSocketReceived(ResponseMessage);

                            WSTimeLine.CallDataReceived(ResponseMessage);
                        }
                    }
                    catch (Exception ce)
                    {
#if DEBUG
                        Console.WriteLine($"❌ RECEIVE FAILED: {WSTimeLine._HostDefinition} - {ce.Message} (Task: {taskId})");
#endif
                        System.Diagnostics.Debug.WriteLine("receive failed");
                        System.Diagnostics.Debug.WriteLine(WSTimeLine._HostUrl);
                        System.Diagnostics.Debug.WriteLine(ce);

                        if (WSTimeLine.GetSocketClient().State != WebSocketState.Open)
                        {
#if DEBUG
                            Console.WriteLine($"🔄 CONNECTION LOST: {WSTimeLine._HostDefinition} - Exiting reader task (Task: {taskId})");
#endif
                            WSTimeLine.CallConnectionLost();
                            return; // 再帰呼び出しを防ぐため、タスクを終了
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

            // ユーザー操作による切断でない場合のみ再接続
            if (this.IsUserInitiatedDisconnect())
            {
                System.Diagnostics.Debug.WriteLine("ユーザー操作による切断のため、再接続しません");
                return;
            }

            // オープンを待つ
            WebSocketTimeLineCommon WS = (WebSocketTimeLineCommon)sender;
            int retryCount = 0;
            const int maxRetries = 10;
            
            while (WS.GetSocketState() != WebSocketState.Open && retryCount < maxRetries)
            {
                retryCount++;
                // 指数バックオフで再接続間隔を調整（1分、2分、4分...）
                int waitSeconds = Math.Min(60 * (int)Math.Pow(2, retryCount - 1), 300); // 最大5分
                Thread.Sleep(1000 * waitSeconds);
                System.Diagnostics.Debug.WriteLine($"再接続試行 {retryCount}/{maxRetries} - 待機中（　＾ω＾）");
                try
                {
                    if (this._APIKey != null)
                    {
                        WS.OpenTimeLineDynamic(this._HostDefinition, this._APIKey);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("API key is null, cannot reconnect");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"再接続失敗: {ex.Message}");
                }
                System.Diagnostics.Debug.WriteLine("現在の状態：" + ((WebSocketTimeLineCommon)sender).GetSocketClient().State);
            }
            
            if (WS == null)
            {
                // 必ず入ってるはず
                return;
            }

            if (WS.GetSocketState() == WebSocketState.Open)
            {
                ReadTimeLineContinuous(WS);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("最大再接続回数に達しました");
            }
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

            dynamic Res = System.Text.Json.JsonDocument.Parse(e.MessageRaw);
            var t = JsonNode.Parse(e.MessageRaw);

            // ChannelToTimeLineData.Type(t);

            // Avalonia用にTimeLineContainerを作成してMainWindowに渡す
            try
            {
                var timelineContainer = ChannelToTimeLineContainer.ConvertTimeLineContainer(this._HostDefinition, t);
                
                // MainWindowに渡すためのイベントを発火
                // 実際の実装では適切なイベントハンドラーを使用
                OnTimeLineDataReceived(timelineContainer);
            }
            catch (Exception ce)
            {
                System.Diagnostics.Debug.WriteLine(ce.ToString());
            }
        }

        /// <summary>
        /// タイムラインデータ受信時のイベント
        /// </summary>
        public event EventHandler<TimeLineContainer>? TimeLineDataReceived;
        
        protected virtual void OnTimeLineDataReceived(TimeLineContainer container)
        {
            TimeLineDataReceived?.Invoke(this, container);
        }
    }
}
