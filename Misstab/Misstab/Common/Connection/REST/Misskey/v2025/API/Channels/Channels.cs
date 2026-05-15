using Misstab.Common.AnalyzeData.Format.Misskey.v2025;
using Misstab.Common.Connection.REST.Misskey.v2025.API.Notes;
using Misstab.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Misstab.Common.Connection.REST.Misskey.v2025.API.Notes.CreateNotes;

namespace Misstab.Common.Connection.REST.Misskey.v2025.API.Channels
{
    public class Channels : MisskeyAPIv2025Controller
    {
    }

    public class GetChannels : Channels
    {
        /// <summary>
        /// 検索クエリ（必須）
        /// </summary>
        public string query { get; set; } = string.Empty;

        /// <summary>
        /// 取得タイプ
        /// </summary>
        public enum ResponseTypes
        {
            nameAndDescription,
            nameOnly
        }
        public ResponseTypes responseType { get; set; } = ResponseTypes.nameAndDescription;
        [JsonIgnore]
        public readonly Dictionary<ResponseTypes, string> ResponseTypeName =
            new Dictionary<ResponseTypes, string>()
            {
                {ResponseTypes.nameAndDescription, "nameAndDescription" },
                {ResponseTypes.nameOnly, "nameOnly" },
            };
        public string? type { get { return ResponseTypeName[responseType]; } }

        /// <summary>
        /// 取得件数
        /// </summary>
        public int? limit { get; set; }

        /// <summary>
        /// シンプルにチャンネルを取得するメソッド
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="APIKey"></param>
        /// <param name="ResultMsg"></param>
        /// <returns></returns>
        public static bool EasyGetChannel(string Host,
                                          string APIKey,
                                          out string ResultMsg,
                                          string Query = "",
                                          ResponseTypes ResponseType = ResponseTypes.nameAndDescription,
                                          int Limit = -1)
        {
            ResultMsg = string.Empty;
            var i = new GetChannels();
            i.query = Query;
            i.limit = Limit;
            i.responseType = ResponseType;

            var Ctl = MisskeyAPIController.CreateInstance(MisskeyAPIConst.API_ENDPOINT.CHANNELS);
            try
            {
                Ctl.Request(Host, APIKey, i.CreateRequestBody());
                var rs = Ctl.GetResponse();
                ResultMsg = rs.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
            }
            return Ctl.State == ControllerState.Finish;
        }
        public JsonObject CreateRequestBody()
        {
            var j = JsonNode.Parse(JsonSerializer.Serialize(this))?.AsObject();
            if (this.limit < 0)
            {
                j.Remove("limit");
            }

            return j;
        }
    }

    public class GetChannelsFollowed : Channels
    {
        /// <summary>
        /// 検索クエリ（必須）
        /// </summary>
        public string query { get; set; } = string.Empty;

        /// <summary>
        /// 取得件数
        /// </summary>
        public int? limit { get; set; }

        /// <summary>
        /// シンプルにチャンネルを取得するメソッド
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="APIKey"></param>
        /// <param name="ResultMsg"></param>
        /// <returns></returns>
        public static bool EasyGetChannel(string Host,
                                          string APIKey,
                                          out Channel[] ResultArray,
                                          out string ResultMsg,
                                          string Query = "",
                                          int Limit = -1)
        {
            ResultMsg = string.Empty;
            ResultArray = new Channel[0];
            var i = new GetChannels();
            i.query = Query;
            i.limit = Limit;

            var Ctl = MisskeyAPIController.CreateInstance(MisskeyAPIConst.API_ENDPOINT.CHANNELS_FOLLOWED);
            try
            {
                Ctl.Request(Host, APIKey, i.CreateRequestBody());
                var rs = Ctl.GetResponse();
                ResultMsg = rs.ToString();

                ResultArray = rs.AsArray().Select(r => { return new Misstab.Common.AnalyzeData.Format.Misskey.v2025.Channel() { Node = r }; }).ToArray();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
            }
            return Ctl.State == ControllerState.Finish;
        }
        public JsonObject CreateRequestBody()
        {
            var j = JsonNode.Parse(JsonSerializer.Serialize(this))?.AsObject();
            if (this.limit < 0)
            {
                j.Remove("limit");
            }

            return j;
        }
    }
}
