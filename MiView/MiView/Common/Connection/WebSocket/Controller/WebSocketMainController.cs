using MiView.Common.Connection.VersionInfo;
using MiView.Common.Connection.WebSocket.Misskey.v2025;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Connection.WebSocket.Controller
{
    internal class WebSocketMainController
    {
        /// <summary>
        /// WebSocketTimeLineManagerを作成
        /// </summary>
        /// <param name="Software"></param>
        /// <param name="Version"></param>
        /// <returns></returns>
        public static WebSocketMain? CreateWSTLManager(CSoftwareVersionInfo.SOFTWARE_LIST Software,
                                                         VersionAttribute? Version)
        {
            // WebsocketManager
            WebSocketMain? WSManager = null;

            // バージョン情報
            CSoftwareVersionInfo VerInfo = new CSoftwareVersionInfo();
            VerInfo.SoftwareType = Software;
            VerInfo.Version = Version;

            // インスタンスを作って返す
            switch (Software)
            {
                case CSoftwareVersionInfo.SOFTWARE_LIST.MISSKEY:
                    WSManager = CreateMisskeyWSManager(Version);
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
        private static WebSocketMain? CreateMisskeyWSManager(VersionAttribute Version)
        {
            switch (Version.MajorVersion)
            {
                case 2025:
                case 2024:
                    return new WebSocketMainMisskey2025();
                case 11:
                    break;
            }
            return null;
        }
    }
}
