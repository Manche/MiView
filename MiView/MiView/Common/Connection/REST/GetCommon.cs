using MiView.Common.AnalyzeData.Format.Misskey.v2025.API;
using MiView.Common.Connection.VersionInfo;
using MiView.Common.Notification.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MiView.Common.Connection.REST
{
    internal class GetCommon
    {
        private CSoftwareVersionInfo.SOFTWARE_LIST _SOFTWARE = CSoftwareVersionInfo.SOFTWARE_LIST.NONE;

        private const string GetMetaJsonStr = "{\"detail\":true}";

        public static GetCommon Instance { get; } = new GetCommon();

        #region インスタンス情報取得
        public CSoftwareVersionInfo? GetSoftwareVersion(string InstanceURL,
                                                        string APIKey,
                                                        CSoftwareVersionInfo.SOFTWARE_LIST Target = CSoftwareVersionInfo.SOFTWARE_LIST.NONE,
                                                        int MajorVersion = -1,
                                                        int MinorVersion = -1,
                                                        int BuildVersion = -1)
        {
            CSoftwareVersionInfo? VerInfo = null;

            switch (Target)
            {
                case CSoftwareVersionInfo.SOFTWARE_LIST.MISSKEY:
                    VerInfo = GetMisskeySoftwareVersion(InstanceURL);
                    if (VerInfo != null)
                    {
                        return VerInfo;
                    }
                    break;
                default:
                    return GetMisskeySoftwareVersion(InstanceURL);
                    // break;
            }

            return null;
        }

        /// <summary>
        /// バージョン情報取得
        /// </summary>
        /// <param name="InstanceURL"></param>
        /// <returns></returns>
        public static CSoftwareVersionInfo? GetMisskeySoftwareVersion(string InstanceURL)
        {
            HttpRequestController Cnt;
            JsonNode? ResJson;
            Meta ServerInfo;

            Cnt = new HttpRequestController();
            Cnt.ReqeustUrl = "https://" + InstanceURL + MMisskeyVersionConst.meta_V2025;
            Cnt.ExecuteProcess = HttpRequestController.EXECUTE_PROCESS.POST;
            Cnt.IsAsync = false;
            Cnt.RequestBody = GetMetaJsonStr;
            //Cnt.RequestHeader.Add("Content-Type", "application/json");
            Cnt.ExecuteMethod();
            try
            {
                ResJson = JsonNode.Parse(Cnt.HttpResponseBody);
                ServerInfo = new Meta(ResJson);
                return MMisskeyVersionConst.GetVersionInfo(ServerInfo.Version.ToString());
            }
            catch (Exception ex)
            {
            }

            Cnt = new HttpRequestController();
            Cnt.ReqeustUrl = "http://" + InstanceURL + MMisskeyVersionConst.meta_V2024;
            Cnt.ExecuteProcess = HttpRequestController.EXECUTE_PROCESS.POST;
            Cnt.IsAsync = false;
            Cnt.ExecuteMethod();
            try
            {
                ResJson = JsonNode.Parse(Cnt.HttpResponseBody);
                ServerInfo = new Meta(ResJson);
                return MMisskeyVersionConst.GetVersionInfo(ServerInfo.Version.ToString());
            }
            catch (Exception ex)
            {
            }

            Cnt = new HttpRequestController();
            Cnt.ReqeustUrl = "http://" + InstanceURL + MMisskeyVersionConst.meta_V11;
            Cnt.ExecuteProcess = HttpRequestController.EXECUTE_PROCESS.POST;
            Cnt.IsAsync = false;
            Cnt.ExecuteMethod();
            System.Diagnostics.Debug.WriteLine(Cnt.HttpResponseBody);
            System.Diagnostics.Debug.WriteLine(Cnt.HttpResponseBody == null);
            try
            {
                ResJson = JsonNode.Parse(Cnt.HttpResponseBody);
                ServerInfo = new Meta(ResJson);
                return MMisskeyVersionConst.GetVersionInfo(ServerInfo.Version.ToString());
            }
            catch (Exception ex)
            {
            }


            return null;
        }
        #endregion
    }
}
