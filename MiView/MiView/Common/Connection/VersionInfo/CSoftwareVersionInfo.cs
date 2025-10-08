using MiView.ScreenForms.Controls.Combo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Connection.VersionInfo
{
    public class CSoftwareVersionInfo
    {
        /// <summary>
        /// ソフトウェア一覧
        /// </summary>
        public enum SOFTWARE_LIST
        {
            NONE,
            MISSKEY,
        }
        public static Dictionary<SOFTWARE_LIST, string> SoftwareNames = new Dictionary<SOFTWARE_LIST, string>()
        {
            { SOFTWARE_LIST.NONE, "" },
            { SOFTWARE_LIST.MISSKEY, MMisskeyVersionInfo._DefaultSoftwareName },
        };
        /// <summary>
        /// デフォルトソフトウェア名
        /// </summary>
        public const string _DefaultSoftwareName = "";
        /// <summary>
        /// ソフトウェア名
        /// </summary>
        public virtual string? SoftwareName {  get; set; }
        /// <summary>
        /// バージョン
        /// </summary>
        public VersionAttribute Version { get; set; } = new VersionAttribute();
    }

    public class VersionAttribute
    {
        public int MajorVersion { get; set; } = 0;
        public int MinorVersion { get; set; } = 0;
        public int Revision { get; set; } = 0;
        public int BuildVersion { get; set; } = 0;
    }
}
