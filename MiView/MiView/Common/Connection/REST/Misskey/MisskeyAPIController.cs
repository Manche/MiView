using MiView.Common.AnalyzeData.Format.Misskey.v2025.API;
using MiView.Common.Connection.VersionInfo;
using MiView.Common.Notification.Http;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;

namespace MiView.Common.Connection.REST.Misskey
{
    public class MisskeyAPIController
    {
        protected MisskeyAPIConst.API_ENDPOINT _EndPoint { get; set; }
        public static MisskeyAPIController CreateInstance(MisskeyAPIConst.API_ENDPOINT EndPoint)
        {
            MisskeyAPIController Controller = MisskeyAPIConst.CreateControlInstance(EndPoint);
            Controller._EndPoint = EndPoint;
            return Controller;
        }

        protected JsonNode? RetNode { get; set; }

        public void Request(string _Host,
                            string? APIKey = null,
                            JsonObject? RequestBody = null,
                            Dictionary<string, string>? RequestParam = null)
        {
            if (MisskeyAPIConst.RequiredBearer(this._EndPoint) && (APIKey == null || APIKey == string.Empty))
            {
                throw new Exception("need API token");
            }
            HttpRequestController Cnt;
            JsonNode? ResJson;

            Cnt = new HttpRequestController();
            Cnt.ReqeustUrl = "https://" + _Host + MisskeyAPIConst.URLPath(this._EndPoint);
            Cnt.ExecuteProcess = MisskeyAPIConst.ExecuteProcess(this._EndPoint);
            Cnt.IsAsync = false;

            if (MisskeyAPIConst.RequiredBearer(this._EndPoint))
            {
                Cnt.BearerToken = APIKey;
            }

            if (RequestBody != null)
            {
                Cnt.RequestBody = RequestBody.ToJsonString();
            }
            else
            {
                Cnt.RequestBody = "{}";
            }
            Cnt.ExecuteMethod();
            try
            {
                RetNode = JsonNode.Parse(Cnt.HttpResponseBody);

                System.Diagnostics.Debug.WriteLine(new Common.AnalyzeData.Format.Misskey.v2025.Note() { Node = RetNode });
            }
            catch (Exception ex)
            {
            }
        }

        public virtual Common.AnalyzeData.Format.Misskey.v2025.Note[]? GetNotes()
        {
            if (this.RetNode == null)
            {
                throw new Exception("NotInitialized");
            }
            if (!(this.RetNode is JsonArray))
            {
                return null;
            }
            return GetNotes((JsonArray)this.RetNode);
        }

        public virtual Common.AnalyzeData.Format.Misskey.v2025.Note[]? GetNotes(JsonArray Nodes)
        {
            var Rt = Nodes
                .ToArray()
                .Select(nd => {
                    return new Common.AnalyzeData.Format.Misskey.v2025.Note() { Node = nd };
                })
                .ToList()
                .FindAll(nd => {
                    try
                    {
                        return nd.CreatedAt != null;
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                })
                .ToArray();
            return Rt.Length > 0 ? Rt : null;
        }
    }

    public class MisskeyAPIConst
    {
        private const string APIPrefix = "/api";
        public enum API_ENDPOINT
        {
            NOTES_TIMELINE = 0,
        }

        public static bool RequiredBearer (API_ENDPOINT EndPoint)
        {
            switch (EndPoint)
            {
                case API_ENDPOINT.NOTES_TIMELINE: return true;
                default: return false;
            }
        }

        public static string URLPath (API_ENDPOINT EndPoint)
        {
            switch (EndPoint)
            {
                case API_ENDPOINT.NOTES_TIMELINE: return APIPrefix + "/notes/timeline";
                default: throw new NotImplementedException("NotImplemented");
            }
        }

        public static HttpRequestController.EXECUTE_PROCESS ExecuteProcess(API_ENDPOINT EndPoint)
        {
            switch (EndPoint)
            {
                case API_ENDPOINT.NOTES_TIMELINE:
                    return HttpRequestController.EXECUTE_PROCESS.POST;
                default:
                    throw new NotImplementedException("NotImplemented");
            }
        }

        public static MisskeyAPIController CreateControlInstance(API_ENDPOINT EndPoint)
        {
            switch (EndPoint)
            {
                case API_ENDPOINT.NOTES_TIMELINE: return new Misskey.v2025.API.Notes.TimeLine();
                default: throw new NotImplementedException("notimplemented");
            }
        }
    }
}
