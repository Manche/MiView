using MiView.Common.Notification;
using MiView.Common.Notification.Baloon;
using MiView.Common.TimeLine;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
                //var Tm = JsonSerializer.Deserialize<SettingTimeLine[]>(json, new JsonSerializerOptions() { MaxDepth = 1 });
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
        #region Alert

        public static void SaveAlertNotification(string Definition, List<NotificationController> config)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config.Select(r => { return (object)r; }), options);
            SettingConst.SettingDirCheck();
            File.WriteAllText(SettingConst.ALERT_NOTIFICATION_SETTINGS_FILE_NAME(Definition), json);
        }

        public static List<NotificationController> LoadAlertNotification(string Definition)
        {
            if (!File.Exists(SettingConst.ALERT_NOTIFICATION_SETTINGS_FILE_NAME(Definition)))
                return new List<NotificationController>();

            string json = File.ReadAllText(SettingConst.ALERT_NOTIFICATION_SETTINGS_FILE_NAME(Definition));
            try
            {
                List<NotificationController> Notifications = new List<NotificationController>();
                JsonArray? JoDatas = JsonSerializer.Deserialize<JsonArray>(json);
                if (JoDatas == null)
                {
                    return new List<NotificationController>();
                }
                foreach (var JoData in JoDatas)
                {
                    JsonNode? TypeNameRaw = JoData["TypeName"];
                    if (TypeNameRaw == null)
                    {
                        continue;
                    }
                    string TypeName = TypeNameRaw.ToString();
                    if (TypeName == string.Empty)
                    {
                        continue;
                    }
                    try
                    {
                        var TmpObj = Type.GetType(TypeName);
                        if (TmpObj == null)
                        {
                            continue;
                        }
                        var TmpCls = System.Activator.CreateInstance(TmpObj);
                        if (TmpCls == null ||
                            TmpCls.GetType().BaseType != typeof(NotificationController))
                        {
                            continue;
                        }
                        foreach (PropertyInfo PropInfo in TmpCls.GetType().GetProperties())
                        {
                            try
                            {
                                if (PropInfo.PropertyType == typeof(string))
                                {
                                    PropInfo.SetValue(TmpCls, JoData[PropInfo.Name].ToString());
                                    //continue;
                                }
                                if (PropInfo.PropertyType == typeof(int))
                                {
                                    PropInfo.SetValue(TmpCls, int.Parse(JoData[PropInfo.Name].ToString()));
                                    //continue;
                                }
                                if (PropInfo.PropertyType == typeof(float))
                                {
                                    PropInfo.SetValue(TmpCls, float.Parse(JoData[PropInfo.Name].ToString()));
                                    //continue;
                                }
                                if (PropInfo.PropertyType == typeof(bool))
                                {
                                    PropInfo.SetValue(TmpCls, bool.Parse(JoData[PropInfo.Name].ToString()));
                                    //continue;
                                }
                            }
                            catch
                            {
                            }
                        }
                        Notifications.Add((NotificationController)TmpCls);
                    }
                    catch
                    {
                    }
                }
                System.Diagnostics.Debug.WriteLine(JsonSerializer.Deserialize<object>(json));
                return Notifications;
            }
            catch
            {
                return new List<NotificationController>();
            }
        }
        #endregion

        #region Debug関連
        public static void SaveDebugInfo(SettingDebug[] config)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(SettingConst.DEBUG_SETTINGS_FILE, json);
        }

        public static SettingDebug[] LoadDebugInfo()
        {
            if (!File.Exists(SettingConst.DEBUG_SETTINGS_FILE))
                return new SettingDebug[] { }; // ファイルがなければデフォルト値で作成

            string json = File.ReadAllText(SettingConst.DEBUG_SETTINGS_FILE);
            try
            {
                //var Tm = JsonSerializer.Deserialize<SettingTimeLine[]>(json, new JsonSerializerOptions() { MaxDepth = 1 });
                return JsonSerializer.Deserialize<SettingDebug[]>(json) ?? new SettingDebug[] { };
            }
            catch
            {
                return new SettingDebug[0];
            }
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
        public static readonly string ALERT_DIR = "\\ALERT";
        public static readonly string NOTIFICATION_DIR = "\\ALERT";
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
        /// デバッグ情報
        /// </summary>
        public static readonly string DEBUG_SETTINGS_FILE = Path.Combine(SETTINGS_DIR, "settings_debug.json");
        /// <summary>
        /// アラート
        /// </summary>
        public static readonly string ALERT_SETTINGS_FILE_FRONT = Path.Combine(SETTINGS_DIR, "settings_alert_");
        public static readonly string ALERT_SETTINGS_EXTENSION = ".json";
        public static string ALERT_SETTINGS_FILE_NAME(string Name) { return Path.Combine(ALERT_SETTINGS_FILE_FRONT + Name + ALERT_SETTINGS_EXTENSION); }

        public static readonly string ALERT_NOTIFICATION_SETTINGS_FILE_FRONT = Path.Combine(SETTINGS_DIR + NOTIFICATION_DIR, "settings_alert_notification_");
        public static readonly string ALERT_NOTIFICATION_SETTINGS_EXTENSION = ".json";
        public static string ALERT_NOTIFICATION_SETTINGS_FILE_NAME(string Name)
        {
            return Path.Combine(ALERT_NOTIFICATION_SETTINGS_FILE_FRONT + Name + ALERT_NOTIFICATION_SETTINGS_EXTENSION);
        }

        // 以下逐一更新
        public static void SettingDirCheck()
        {
            if (!Directory.Exists(SETTINGS_DIR + NOTIFICATION_DIR))
            {
                Directory.CreateDirectory(SETTINGS_DIR + NOTIFICATION_DIR);
            }
        }
    }
}
