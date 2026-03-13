using Misstab.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Misstab.Common.Connection.REST.Misskey.v2025.API.Notes
{
    public class Notes : MisskeyAPIv2025Controller
    {
    }

    public class CreateNotes : Notes
    {
        public string? channelId { get; set; }
        public string? cw { get; set; }
        public string[] fileIds { get; set; } = Enumerable.Repeat(string.Empty, 16).ToArray();
        public bool localOnly { get; set; } = false;
        public string[] mediaIds { get; set; } = Enumerable.Repeat(string.Empty, 16).ToArray();
        public bool noExtractEmojis { get; set; } = false;
        public bool noExtractHashTags { get; set; } = false;
        public bool noExtractMentions { get; set; } = false;

        public enum ReactionAcceptances
        {
            NULL,
            LikeOnly,
            LikeOnlyForRemote,
            NonSensitiveOnly,
            NonSensitiveOnlyForLocalLikeOnlyForRemote
        }
        [JsonIgnore]
        public readonly Dictionary<ReactionAcceptances, string> ReactionAcceptanceName =
            new Dictionary<ReactionAcceptances, string>()
            {
                {ReactionAcceptances.NULL, "null" },
                {ReactionAcceptances.LikeOnly, "LikeOnly" },
                {ReactionAcceptances.LikeOnlyForRemote, "LikeOnlyForRemote" },
                {ReactionAcceptances.NonSensitiveOnly, "NonSensitiveOnly" },
                {ReactionAcceptances.NonSensitiveOnlyForLocalLikeOnlyForRemote, "NonSensitiveOnlyForLocalLikeOnlyForRemote" }
            };
        public string? reactionAcceptance { get; set; }
        public string? renoteId { get; set; }
        public string? replyId { get; set; }
        public string? text { get; set; }
        [JsonIgnore]
        public TimeLineContainer.PROTECTED_STATUS VisibilityRaw { get; set; } = TimeLineContainer.PROTECTED_STATUS.Public;
        [JsonIgnore]
        public readonly Dictionary<TimeLineContainer.PROTECTED_STATUS, string> VisibilityNames =
            new Dictionary<TimeLineContainer.PROTECTED_STATUS, string>()
            {
                {TimeLineContainer.PROTECTED_STATUS.Public, "public" },
                {TimeLineContainer.PROTECTED_STATUS.Home, "home"},
                {TimeLineContainer.PROTECTED_STATUS.Follower, "followers" },
                {TimeLineContainer.PROTECTED_STATUS.Direct, "specified" },
            };
        public string? visibility { get { return VisibilityNames[VisibilityRaw]; } }
        public JsonObject CreateRequestBody()
        {
            var j = JsonNode.Parse(JsonSerializer.Serialize(this))?.AsObject();

            List<string> v;
            v = this.fileIds.Distinct().Where(r => !string.IsNullOrEmpty(r)).ToList();
            this.fileIds = v.ToArray();
            v = this.mediaIds.Distinct().Where(r => !string.IsNullOrEmpty(r)).ToList();
            this.mediaIds = v.ToArray();






            v = this.fileIds.Distinct().Where(r => !string.IsNullOrEmpty(r)).ToList();
            if (v.Count == 0)
            {
                j.Remove("fileIds");
            }

            v = this.mediaIds.Distinct().Where(r => !string.IsNullOrEmpty(r)).ToList();
            if (v.Count == 0)
            {
                j.Remove("mediaIds");
            }

            return j;
        }

        /// <summary>
        /// シンプルにnoteするメソッド
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Host"></param>
        /// <param name="APIKey"></param>
        /// <param name="Kind"></param>
        /// <param name="ResultMsg"></param>
        /// <returns></returns>
        public static bool EasyPostNote(string Text, string Host, string APIKey, TimeLineContainer.PROTECTED_STATUS Kind, out string ResultMsg)
        {
            ResultMsg = string.Empty;
            var i = new CreateNotes();
            i.text = Text;
            i.VisibilityRaw = Kind;

            var Ctl = MisskeyAPIController.CreateInstance(MisskeyAPIConst.API_ENDPOINT.NOTES_CREATE);
            try
            {
                Ctl.Request(Host, APIKey, i.CreateRequestBody());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return false;
            }
            return Ctl.State == ControllerState.Finish;
        }
    }
}
