using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiViewUpdater.Controller
{
    public class BinaryUpdateInfo
    {
        private static List<string> InitialBinaryURLs = new List<string>()
        {
            "",
        };

        private List<string> BinaryURLs { get; } = new List<string>()
        {
        };
    }

    public class BinaryUpdateInfoConst
    {
        /// <summary>
        /// 設定ディレクトリ
        /// </summary>
        public static readonly string SETTINGS_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MiView");
        public static readonly string UPDATEINFO_DIR = "\\UPDATEINFO";
        /// <summary>
        /// websocket
        /// </summary>
        public static readonly string UPDATEINFO_SETTINGS_FILE = Path.Combine(SETTINGS_DIR, "updateinfo.json");
    }
}
