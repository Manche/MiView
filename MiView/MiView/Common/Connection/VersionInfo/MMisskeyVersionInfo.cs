using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Connection.VersionInfo
{
    public class MMisskeyVersionInfo : CSoftwareVersionInfo
    {
        /// <summary>
        /// デフォルトソフトウェア名
        /// </summary>
        public new const string _DefaultSoftwareName = "Misskey";
        public override string? SoftwareName { get; set; } = _DefaultSoftwareName;
    }
}
