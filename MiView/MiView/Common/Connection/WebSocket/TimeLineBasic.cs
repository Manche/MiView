using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Connection.WebSocket
{
    public class TimeLineBasic
    {

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
    }
}
