using MiView.Common.Connection.WebSocket.Event;
using MiView.Common.Connection.WebSocket.Structures;
using MiView.Common.TimeLine;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MiView.Common.Connection.WebSocket.Misskey.v2025
{
    internal class WebSocketTimeLineHome : WebSocketTimeLineCommon
    {
        public WebSocketTimeLineHome(string host, string apikey) : base(host, apikey)
        {
        }

        /// <summary>
        /// 接続識別子
        /// </summary>
        protected override ConnectMainBody _WebSocketConnectionObj
        {
            get { return new ConnectMainBody() { channel = "homeTimeline", id = "hoge" }; }
        }
    }

    internal class WebSocketTimeLineSocial : WebSocketTimeLineCommon
    {
        public WebSocketTimeLineSocial(string host, string apikey) : base(host, apikey)
        {
        }

        /// <summary>
        /// 接続識別子
        /// </summary>
        protected override ConnectMainBody _WebSocketConnectionObj
        {
            get { return new ConnectMainBody() { channel = "hybridTimeline", id = "hoge" }; }
        }
    }


    internal class WebSocketTimeLineGlobal : WebSocketTimeLineCommon
    {
        public WebSocketTimeLineGlobal(string host, string apikey) : base(host, apikey)
        {
        }

        /// <summary>
        /// 接続識別子
        /// </summary>
        protected override ConnectMainBody _WebSocketConnectionObj
        {
            get { return new ConnectMainBody() { channel = "globalTimeline", id = "hoge" }; }
        }
    }


    internal class WebSocketTimeLineLocal : WebSocketTimeLineCommon
    {
        public WebSocketTimeLineLocal(string host, string apikey) : base(host, apikey)
        {
        }

        /// <summary>
        /// 接続識別子
        /// </summary>
        protected override ConnectMainBody _WebSocketConnectionObj
        {
            get { return new ConnectMainBody() { channel = "localTimeline", id = "hoge" }; }
        }
    }
}
