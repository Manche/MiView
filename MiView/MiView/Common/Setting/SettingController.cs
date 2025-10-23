using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiView.Common.Setting
{
    /// <summary>
    /// 設定コントローラ
    /// </summary>
    public static class SettingController
    {
        /// <summary>
        /// ディレクトリチェックと作成
        /// </summary>
        public static void SettingDirectoryCheckCreate()
        {
            if (!Directory.Exists(SettingConst.SETTINGS_DIR))
            {
                Directory.CreateDirectory(SettingConst.SETTINGS_DIR);
            }
        }

        #region WebSocket
        public static SettingWebSocket[] LoadWebSockets()
        {
            if (!File.Exists(SettingConst.WEBSOCKET_SETTINGS_FILE))
                return new SettingWebSocket[] { }; // ファイルがなければデフォルト値で作成

            string json = File.ReadAllText(SettingConst.WEBSOCKET_SETTINGS_FILE);
            try
            {
                return JsonSerializer.Deserialize<SettingWebSocket[]>(json) ?? new SettingWebSocket[] { };
            }
            catch
            {
                return new SettingWebSocket[0];
            }
        }

        public static void SaveWebSockets(SettingWebSocket[] config)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(SettingConst.WEBSOCKET_SETTINGS_FILE, json);
        }
        #endregion
        #region TimeLine
        public static SettingTimeLine[] LoadTimeLine()
        {
            if (!File.Exists(SettingConst.TIMELINE_SETTINGS_FILE))
                return new SettingTimeLine[] { }; // ファイルがなければデフォルト値で作成

            string json = File.ReadAllText(SettingConst.TIMELINE_SETTINGS_FILE);
            try
            {
                return JsonSerializer.Deserialize<SettingTimeLine[]>(json) ?? new SettingTimeLine[] { };
            }
            catch
            {
                return new SettingTimeLine[0];
            }
        }

        public static void SaveTimeLine(SettingTimeLine[] config)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(SettingConst.TIMELINE_SETTINGS_FILE, json);
        }
        #endregion
    }

    /// <summary>
    /// 設定コントローラ定数
    /// </summary>
    public static class SettingConst
    {
        /// <summary>
        /// 設定ディレクトリ
        /// </summary>
        public static readonly string SETTINGS_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MiView");
        /// <summary>
        /// websocket
        /// </summary>
        public static readonly string WEBSOCKET_SETTINGS_FILE = Path.Combine(SETTINGS_DIR, "settings_websocket.json");
        /// <summary>
        /// timeline
        /// </summary>
        public static readonly string TIMELINE_SETTINGS_FILE = Path.Combine(SETTINGS_DIR, "settings_timeline.json");
        /// <summary>
        /// フィルタ
        /// </summary>
        public static readonly string FILTER_SETTINGS_FILE = Path.Combine(SETTINGS_DIR, "settings_filter.json");
        /// <summary>
        /// アラート
        /// </summary>
        public static readonly string ALERT_SETTINGS_FILE = Path.Combine(SETTINGS_DIR, "settings_alert.json");

        // 以下逐一更新
    }
}
