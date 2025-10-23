using MiView.Common.Connection.WebSocket;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.ScreenForms.DialogForm.Event
{
    public class SettingChangeEventArgs: EventArgs
    {
        /// <summary>
        /// WebSocketManager
        /// </summary>
        public WebSocketManager? _WSManager { get; set; }
        /// <summary>
        /// WebSocketManager識別値
        /// </summary>
        public int? _WSDefinition {  get; set; }
        /// <summary>
        /// DataGridTimeLine
        /// </summary>
        public DataGridTimeLine? _GridTimeLine { get; set; }
        /// <summary>
        /// DataGridTimeLines
        /// </summary>
        public Dictionary<string, DataGridTimeLine>? _GridTimeLines { get; set; }
        /// <summary>
        /// 統合TLへの反映をするかどうか
        /// </summary>
        public bool? UpdateIntg { get; set; } = null;

        public SettingChangeEventArgs()
        {
        }
    }
}
