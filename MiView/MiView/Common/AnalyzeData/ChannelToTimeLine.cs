using MiView.Common.AnalyzeData.Format.Misskey.v2025;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MiView.Common.AnalyzeData
{
    /// <summary>
    /// チャンネルレスポンス→タイムライン用データ
    /// </summary>
    internal class ChannelToTimeLineData
    {
        public static ChannelToTimeLineData Get(JsonNode Input) { return new ChannelToTimeLineData() { Node =  Input}; }
        public JsonNode? Node { get; set; }
        public JsonNode? ResponseType { get { return this.Node?["type"]; } }
        public JsonNode? ResponseBody { get { return this.Node?["body"]; } }
        public JsonNode? ResponseId { get { return this.ResponseBody?["id"]; } }
        public JsonNode? ResponseNoteType { get { return this.ResponseBody?["type"]; } }
        public Note Note { get { return new Note() { Node = this.ResponseBody?["body"] }; } }
    }

    internal class ChannelToTimeLineContainer
    {
        protected const string _RenoteSign = " RN:";

        public static TimeLineContainer ConvertTimeLineContainer(string OriginalHost, JsonNode? Input)
        {
            if (Input == null)
            {
                throw new ArgumentNullException(nameof(Input));
            }
            TimeLineContainer Container = new TimeLineContainer();

            string Protected = ChannelToTimeLineData.Get(Input).Note.Visibility != null ? JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.Visibility) : string.Empty;
            Container.IDENTIFIED = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.User.UserName) +
                                   JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.User.Name) +
                                   JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.CreatedAt);
            Container.PROTECTED = StringToProtectedStatus(Protected);
            try
            {
                Container.ISLOCAL = bool.Parse(JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.LocalOnly));
            }
            catch (Exception)
            {
                // 取ってこれない時がある
                Container.ISLOCAL = false;
            }
            Container.RENOTED = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.RenoteId) != string.Empty;
            Container.REPLAYED = (JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.ReplyId) != string.Empty) ||
                                 (JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.RenoteId) != string.Empty &&
                                  JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.Text) != string.Empty);
            // Container.CW = JsonConverterCommon.GetStr(ChannelToTimeLineData.Note(Input).CW) != string.Empty;
            Container.ISCHANNEL = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.ChannelId) != string.Empty;
            Container.CHANNEL_NAME = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.Channel.Name);
            // Container.DETAIL = Container.CW ? JsonConverterCommon.GetStr(ChannelToTimeLineData.Note(Input).CW) : JsonConverterCommon.GetStr(ChannelToTimeLineData.Note(Input).Text);
            Container.USERID = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.User.UserName);
            Container.USERNAME = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.User.Name);
            if (Container.USERNAME == string.Empty)
            {
                Container.USERNAME = "[" + Container.USERID + "]";
            }
            Container.UPDATEDAT = DateTime.Parse(JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.CreatedAt));
            Container.SOFTWARE = (ChannelToTimeLineData.Get(Input).Note.User.Instance.IsInvalidatedVersion ? "[☆]" : "") +
                                 JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.User.Instance.SoftwareName) +
                                 JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.User.Instance.SoftwareVersion);
            Container.TLFROM = OriginalHost;
            Container.ORIGINAL = Input;
            Container.ORIGINAL_HOST = OriginalHost;

            GetCW(Input, ref Container);
            GetDetail(Input, ref Container);


            return Container;
        }

        private static void GetCW(JsonNode Input, ref TimeLineContainer Container)
        {
            Container.CW = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.CW) != string.Empty ||
                           JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.Renote.CW) != string.Empty;
        }

        private static void GetDetail(JsonNode Input, ref TimeLineContainer Container)
        {
            string CW = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.CW);
            string ReNoteCW = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.Renote.CW);

            string NoteText = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.Text);
            string ReNoteText = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.Renote.Text);

            string ReNoteSourceUser = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.Renote.User.UserName);
            string ReNoteSourceUserName = JsonConverterCommon.GetStr(ChannelToTimeLineData.Get(Input).Note.Renote.User.Name);

            // Renoteのみ
            if (Container.RENOTED && NoteText == string.Empty)
            {
                if (Container.CW)
                {
                    Container.DETAIL = _RenoteSign + ReNoteSourceUser + "/" + ReNoteSourceUserName + " " + CW;
                }
                else
                {
                    Container.DETAIL = _RenoteSign + ReNoteSourceUser + "/" + ReNoteSourceUserName + " " + ReNoteText;
                }
                return;
            }
            // 引用RN
            if (Container.RENOTED && NoteText != string.Empty)
            {
                if (Container.CW)
                {
                    Container.DETAIL = CW + _RenoteSign + ReNoteSourceUser + "/" + ReNoteSourceUserName + " " + ReNoteCW;
                }
                else
                {
                    Container.DETAIL = NoteText + _RenoteSign + ReNoteSourceUser + "/" + ReNoteSourceUserName + " " + ReNoteText;
                }
                return;
            }

            if (Container.CW)
            {
                Container.DETAIL = CW;
            }
            else
            {
                Container.DETAIL = NoteText;
            }
        }

        public static TimeLineContainer.PROTECTED_STATUS StringToProtectedStatus(string Str)
        {
            TimeLineContainer.PROTECTED_STATUS Status = TimeLineContainer.PROTECTED_STATUS.Public;
            switch (Str)
            {
                case "public":
                    Status = TimeLineContainer.PROTECTED_STATUS.Public;
                    break;
                case "home":
                    Status = TimeLineContainer.PROTECTED_STATUS.Home;
                    break;
                case "followers":
                    Status = TimeLineContainer.PROTECTED_STATUS.Follower;
                    break;
                case "specified":
                    Status = TimeLineContainer.PROTECTED_STATUS.Direct;
                    break;
                default:
                    break;
            }
            return Status;
        }
    }
}
