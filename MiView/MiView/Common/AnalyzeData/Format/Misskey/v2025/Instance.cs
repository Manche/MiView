using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiView.Common.AnalyzeData.Format
{
    public class Instance
    {
        public JsonNode? Node {  get; set; }
        public JsonNode? Name { get { return this.Node?["name"]; } }
        public JsonNode? SoftwareName { get { return this.Node?["softwareName"]; } }
        public JsonNode? SoftwareVersion { get { return this.Node?["softwareVersion"]; } }
        public JsonNode? IconUrl { get { return this.Node?["iconUrl"]; } }
        public JsonNode? FaviconUrl { get { return this.Node?["faviconUrl"]; } }
        public JsonNode? ThemeColor { get { return this.Node?["themeColor"]; } }
        // 予約
        // public JsonNode? ForeColor { get { return this.Node?["themeColor"]; } }
        public bool IsInvalidatedVersion { get { return this.GetInvalidated(); } }

        /// <summary>
        /// バージョンが正しいかどうかのチェック
        /// </summary>
        /// <returns>
        /// ソフトウェア偽装等があるかどうか
        /// </returns>
        private bool GetInvalidated()
        {
            if (this.SoftwareName == null)
            {
                return false;
            }
            switch (this.SoftwareName.ToString())
            {
                case "misskey":
                    return InvalidateMisskeyCheck();
                default:
                    return false;
            }
        }

        /// <summary>
        /// Misskeyのバージョンチェック
        /// </summary>
        /// <returns>
        /// source: https://wiki.misskey.io/ja/software/misskey , https://hide.ac/articles/TLlbF9CMp
        /// 
        /// Misskey v10～で正しいバージョンかどうかのチェックをする
        /// カレンダーバージョニング以降はとりあえずOKの扱い
        /// </returns>
        private bool InvalidateMisskeyCheck()
        {
            if (this.SoftwareVersion == null)
            {
                return false;
            }
            string sVer = this.SoftwareVersion.ToString();
            // 10,11,12,13,2023,2024,2025,3000,3010…
            if (Regex.IsMatch(sVer, @"^(1[0-9]|20[0-9]+|[0-9]{4})"))
            {
                // バージョンが正しい(はず)
                return false;
            }
            else
            {
                // ソフトウェアバージョン偽装している
                return true;
            }
        }
    }
}
