using Misstab.Common.AnalyzeData.Format.Misskey.v2025.API;
using Misstab.Common.Connection.VersionInfo;
using Misstab.Common.Notification.Http;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;

namespace Misstab.Common.Connection.REST.Misskey
{
    public class MisskeyAPIController
    {
        public enum ControllerState
        {
            NotInitialized,
            Prepare,
            Executing,
            Error,
            Finish,
        }
        [JsonIgnore]
        public ControllerState State { get; set; }

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
            State = ControllerState.NotInitialized;
            if (MisskeyAPIConst.RequiredBearer(this._EndPoint) && (APIKey == null || APIKey == string.Empty))
            {
                State = ControllerState.Error;
                throw new Exception("need API token");
            }
            State = ControllerState.Prepare;
            HttpRequestController Cnt;
            JsonNode? ResJson;

            Cnt = new HttpRequestController();
            Cnt.ReqeustUrl = "https://" + _Host + MisskeyAPIConst.URLPath(this._EndPoint);
            Cnt.ExecuteProcess = MisskeyAPIConst.ExecuteProcess(this._EndPoint);
            Cnt.IsAsync = MisskeyAPIConst.IsAsync(this._EndPoint);

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
            State = ControllerState.Executing;
            Cnt.ExecuteMethod();
            try
            {
                if (!Cnt.IsAsync)
                {
                    RetNode = JsonNode.Parse(Cnt.HttpResponseBody);
                }
                else
                {
                    while (Cnt.HttpResponseBody == null)
                    {
                        Thread.Sleep(1000);
                    }
                    RetNode = JsonNode.Parse(Cnt.HttpResponseBody);
                }
                // System.Diagnostics.Debug.WriteLine(new Common.AnalyzeData.Format.Misskey.v2025.Note() { Node = RetNode });
            }
            catch (Exception ex)
            {
                State = ControllerState.Error;
            }
            State = ControllerState.Finish;
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
            NOTES,
            NOTES_CREATE,
        }

        public static bool RequiredBearer (API_ENDPOINT EndPoint)
        {
            switch (EndPoint)
            {
                case API_ENDPOINT.NOTES_TIMELINE: return true;
                case API_ENDPOINT.NOTES: return true;
                case API_ENDPOINT.NOTES_CREATE: return true;
                default: return false;
            }
        }

        public static string URLPath (API_ENDPOINT EndPoint)
        {
            switch (EndPoint)
            {
                case API_ENDPOINT.NOTES_TIMELINE: return APIPrefix + "/notes/timeline";
                case API_ENDPOINT.NOTES: return APIPrefix + "/notes/show";
                case API_ENDPOINT.NOTES_CREATE: return APIPrefix + "/notes/create";
                default: throw new NotImplementedException("NotImplemented");
            }
        }

        public static bool IsAsync(API_ENDPOINT EndPoint)
        {
            switch (EndPoint)
            {
                case API_ENDPOINT.NOTES:
                case API_ENDPOINT.NOTES_TIMELINE:
                    return false;
                case API_ENDPOINT.NOTES_CREATE:
                    return true;
                default:
                    return true;
            }
        }

        public static HttpRequestController.EXECUTE_PROCESS ExecuteProcess(API_ENDPOINT EndPoint)
        {
            switch (EndPoint)
            {
                case API_ENDPOINT.NOTES_TIMELINE:
                case API_ENDPOINT.NOTES:
                case API_ENDPOINT.NOTES_CREATE:
                    return HttpRequestController.EXECUTE_PROCESS.POST;
                default:
                    throw new NotImplementedException("NotImplemented");
            }
        }

        public static MisskeyAPIController CreateControlInstance(API_ENDPOINT EndPoint)
        {
            switch (EndPoint)
            {
                case API_ENDPOINT.NOTES: return new Misskey.v2025.API.Notes.Notes();
                case API_ENDPOINT.NOTES_TIMELINE: return new Misskey.v2025.API.Notes.TimeLine();
                case API_ENDPOINT.NOTES_CREATE: return new Misskey.v2025.API.Notes.CreateNotes();
                default: throw new NotImplementedException("notimplemented");
            }
        }
    }
}
