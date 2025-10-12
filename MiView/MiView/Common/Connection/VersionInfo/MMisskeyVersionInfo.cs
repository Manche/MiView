using MiView.Common.Connection.REST.Constraint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiView.Common.Connection.VersionInfo
{
    public class MMisskeyVersionInfo : CSoftwareVersionInfo
    {
        /// <summary>
        /// デフォルトソフトウェア名
        /// </summary>
        public override SOFTWARE_LIST _DefaultSoftwareType { get { return SOFTWARE_LIST.MISSKEY; } }
        /// <summary>
        /// デフォルトソフトウェア名
        /// </summary>
        public new const string _DefaultSoftwareName = ConnectionRESTConstraint.MisskeySoftwareName;
    }

    public class MMisskeyVersionConst
    {
        /// <summary>
        /// v2025 meta
        /// </summary>
        public const string meta_V2025 = "/api/meta";
        public const string meta_V2024 = "/api/meta";
        public const string meta_V11 = "/meta";

        public static string Endpoint_Meta(int MajorVersion, int MinorVersion, int BuildVersion)
        {
            switch (MajorVersion)
            {
                case 11:
                    return meta_V11;
                case 2024:
                    return meta_V2024;
                case 2025:
                    return meta_V2025;
            }
            throw new NotImplementedException("URLがありません");
        }

        public static MMisskeyVersionInfo GetVersionInfo(string? VersionPattern)
        {
            // 2024.5.0-io.10
            var res = Regex.Match(VersionPattern, @"(\d+)\.(\d+)\.(\d+)(?:[-.]([A-Za-z0-9_]+))*(?:\.(\d+))?");

            MMisskeyVersionInfo VerInfo = new MMisskeyVersionInfo();

            VersionAttribute Attr = new VersionAttribute();
            Attr.RawVersion = VersionPattern;
            Attr.MajorVersion = int.TryParse(res.Groups[1].Value, out _) ? int.Parse(res.Groups[1].Value) : -1;
            Attr.MinorVersion = int.TryParse(res.Groups[2].Value, out _) ? int.Parse(res.Groups[2].Value) : -1;
            Attr.Revision = int.TryParse(res.Groups[3].Value, out _) ? int.Parse(res.Groups[3].Value) : -1;
            Attr.BuildVersion = int.TryParse(res.Groups[4].Value, out _) ? int.Parse(res.Groups[4].Value) : -1;

            VerInfo.Version = Attr;

            return VerInfo;
        }
    }
}
