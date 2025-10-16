using MiView.Common.Connection.VersionInfo;
using MiView.Common.Connection.WebSocket;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Setting
{
    public class SettingWebSocket
    {
        /// <summary>
        /// TimeLine識別設定値
        /// </summary>
        public string[]? TimeLineDefinition { get; set; }
        /// <summary>
        /// タイムライン種類
        /// </summary>
        public TimeLineBasic.ConnectTimeLineKind ConnectTimeLineKind { get; set; } = TimeLineBasic.ConnectTimeLineKind.None;
        /// <summary>
        /// バージョン情報
        /// </summary>
        /// <remarks>
        /// 今のところなくてもいいかも
        /// SoftwareVersion
        /// </remarks>
        public CSoftwareVersionInfo? SoftwareVersionInfo { get; set; }
        /// <summary>
        /// インスタンスURL
        /// </summary>
        public string? InstanceURL {  get; set; }
        /// <summary>
        /// APIトークン
        /// </summary>
        public string? APIKey { get; set; }
        /// <summary>
        /// フィルタかどうか
        /// </summary>
        public bool? IsFiltered { get; set; } = false;
        /// <summary>
        /// 統合タイムラインに反映する・しない
        /// </summary>
        public bool AvoidIntg { get; set; } = false;

        public static SettingWebSocket ConvertWebSocketManagerToSettingObj(WebSocketManager WSManager)
        {
            return new SettingWebSocket()
            {
                TimeLineDefinition = WSManager.TimeLineObject?.Select(r => { return r._Definition; }).ToArray() ?? [],
                ConnectTimeLineKind = WSManager.TLKind,
                SoftwareVersionInfo = WSManager.SoftwareVersion,
                InstanceURL = WSManager._HostDefinition,
                APIKey = WSManager.APIKey,
                AvoidIntg = !WSManager.IncludedDataGridTimeLine(new List<Func<DataGridTimeLine, bool>>() { new Func<DataGridTimeLine, bool>(r => { return r._Definition == "Main"; }) }.ToArray())
            };
        }
    }
    /**
     *                 public void AddTimeLine(string InstanceURL,//
                                string APIKey,//
                                TimeLineBasic.ConnectTimeLineKind sTLKind,//
                                CSoftwareVersionInfo? SoftwareVersionInfo,//
                                string TabName,
                                bool IsFiltered = false,
                                bool AvoidIntg = false,
                                bool IsVisible = true)
        public static WebSocketManager? CreateWSTLManager(CSoftwareVersionInfo.SOFTWARE_LIST Software,
                                                         VersionAttribute Version,
                                                         TimeLineBasic.ConnectTimeLineKind TLKind)
            var WTManager = WebSocketMainController.CreateWSTLManager(WSManager.SoftwareVersion.SoftwareType, WSManager.SoftwareVersion.Version);
     * 
     * 
     * 
     */
}
