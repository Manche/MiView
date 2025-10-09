using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.Connection.WebSocket.Misskey.v2025;
using MiView.Common.Connection.WebSocket.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MiView.Common.Connection.WebSocket
{
    internal class WebSocketMain : WebSocketManager
    {
        /// <summary>
        /// ソケットオープン
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        public virtual WebSocketMain OpenMain(string InstanceURL, string? ApiKey)
        {
            throw new NotImplementedException("Mainは実装されていません。");
        }

        /// <summary>
        /// ソケット展開(持続的)
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <param name="ApiKey"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual WebSocketMain OpenMainDynamic(string InstanceURL, string ApiKey)
        {
            throw new NotImplementedException("Mainは実装されていません[2]");
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
                    Thread.Sleep(1000);
                }
            });
        }


        private static void DbgOutputSocketReceived(string Response)
        {
            System.Diagnostics.Debug.WriteLine(Response);
        }

        #region イベント
        /// <summary>
        /// 接続喪失時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnConnectionLost(object? sender, EventArgs e)
        {
            throw new NotImplementedException("接続喪失時の実装はありません。");
        }

        /// <summary>
        /// データ受信時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnDataReceived(object? sender, ConnectDataReceivedEventArgs e)
        {
            throw new NotImplementedException("受信時の実装はありません。");
        }
        #endregion
    }
}
