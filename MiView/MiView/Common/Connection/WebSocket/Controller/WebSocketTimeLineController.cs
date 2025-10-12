using MiView.Common.Connection.VersionInfo;
using MiView.Common.Connection.WebSocket.Misskey.v2025;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Connection.WebSocket.Controller
{
    internal class WebSocketTimeLineController
    {
        /// <summary>
        /// WebSocketTimeLineManagerを作成
        /// </summary>
        /// <param name="Software"></param>
        /// <param name="Version"></param>
        /// <returns></returns>
        public static WebSocketManager? CreateWSTLManager(CSoftwareVersionInfo.SOFTWARE_LIST Software,
                                                         VersionAttribute Version,
                                                         TimeLineBasic.ConnectTimeLineKind TLKind)
        {
            // WebsocketManager
            WebSocketManager? WSManager = null;

            // バージョン情報
            CSoftwareVersionInfo VerInfo = new CSoftwareVersionInfo();
            VerInfo.SoftwareType = Software;
            VerInfo.Version = Version;

            // インスタンスを作って返す
            switch (Software)
            {
                case CSoftwareVersionInfo.SOFTWARE_LIST.MISSKEY:
                    WSManager = CreateMisskeyWSManager(Version, TLKind);
                    break;
            }
            // バージョンを設定
            if (WSManager != null)
            {
                WSManager.SoftwareVersion = VerInfo;
            }
            return WSManager;
        }

        /// <summary>
        /// Misskey
        /// </summary>
        /// <param name="Version"></param>
        /// <returns></returns>
        private static WebSocketManager? CreateMisskeyWSManager(VersionAttribute Version,
                                                               TimeLineBasic.ConnectTimeLineKind TLKind)
        {
            switch (Version.MajorVersion)
            {
                case 2025:
                case 2024:
                    return CreateMisskeyWSManager2025(TLKind);
                case 11:
                    return CreateMisskeyWSManager11(TLKind);
            }
            return null;
        }

        private static WebSocketManager? CreateMisskeyWSManager2025(TimeLineBasic.ConnectTimeLineKind TLKind)
        {
            switch (TLKind)
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

        private static WebSocketManager? CreateMisskeyWSManager11(TimeLineBasic.ConnectTimeLineKind TLKind)
        {
            switch (TLKind)
            {
            }
            return null;
        }
    }
}
