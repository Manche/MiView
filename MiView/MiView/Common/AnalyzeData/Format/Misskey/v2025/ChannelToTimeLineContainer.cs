using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MiView.Common.AnalyzeData.Format.Misskey.v2025
{
    internal class ChannelToTimeLineContainer : Common.AnalyzeData.ChannelToTimeLineContainer
    {
        public TimeLineContainer ConvertTimeLineContainer(string OriginalHost, Common.AnalyzeData.Format.Misskey.v2025.Note? Note)
        {
            if (Note == null)
            {
                throw new ArgumentNullException(nameof(Note));
            }
            TimeLineContainer Container = new TimeLineContainer();

            string Protected = Note.Visibility != null ? JsonConverterCommon.GetStr(Note.Visibility) : string.Empty;
            Container.IDENTIFIED = JsonConverterCommon.GetStr(Note.User.UserName) +
                                   JsonConverterCommon.GetStr(Note.User.Name) +
                                   JsonConverterCommon.GetStr(Note.CreatedAt);
            Container.PROTECTED = StringToProtectedStatus(Protected, Note);
            try
            {
                Container.ISLOCAL = bool.Parse(JsonConverterCommon.GetStr(Note.LocalOnly));
            }
            catch (Exception)
            {
                // 取ってこれない時がある
                Container.ISLOCAL = false;
            }
            Container.RENOTED = JsonConverterCommon.GetStr(Note.RenoteId) != string.Empty;
            Container.REPLAYED = JsonConverterCommon.GetStr(Note.ReplyId) != string.Empty;
            // Container.CW = JsonConverterCommon.GetStr(ChannelToTimeLineData.Note(Input).CW) != string.Empty;
            Container.ISCHANNEL = JsonConverterCommon.GetStr(Note.ChannelId) != string.Empty;
            Container.CHANNEL_NAME = JsonConverterCommon.GetStr(Note.Channel.Name);
            // Container.DETAIL = Container.CW ? JsonConverterCommon.GetStr(ChannelToTimeLineData.Note(Input).CW) : JsonConverterCommon.GetStr(ChannelToTimeLineData.Note(Input).Text);
            Container.USERID = JsonConverterCommon.GetStr(Note.User.UserName);
            Container.USERNAME = JsonConverterCommon.GetStr(Note.User.Name);
            if (Container.USERNAME == string.Empty)
            {
                Container.USERNAME = "[" + Container.USERID + "]";
            }
            try
            {
                Container.UPDATEDAT = DateTime.Parse(JsonConverterCommon.GetStr(Note.CreatedAt));
            }
            catch (Exception)
            {
            }
            Container.SOFTWARE = (Note.User.Instance.IsInvalidatedVersion ? "[☆]" : "") +
                                 JsonConverterCommon.GetStr(Note.User.Instance.SoftwareName) +
                                 JsonConverterCommon.GetStr(Note.User.Instance.SoftwareVersion);
            Container.TLFROM = OriginalHost;
            if (Note.Node != null)
            {
                Container.ORIGINAL = Note.Node;
            }
            Container.ORIGINAL_HOST = OriginalHost;

            GetCW(Note, ref Container);
            GetDetail(Note, ref Container);

            return Container;
        }
        protected void GetCW(Common.AnalyzeData.Format.Misskey.v2025.Note Input, ref TimeLineContainer Container)
        {
            Container.CW = JsonConverterCommon.GetStr(Input.CW) != string.Empty ||
                           JsonConverterCommon.GetStr(Input.Renote.CW) != string.Empty;
        }

        protected void GetDetail(Common.AnalyzeData.Format.Misskey.v2025.Note Input, ref TimeLineContainer Container)
        {
            string CW = JsonConverterCommon.GetStr(Input.CW);
            string ReNoteCW = JsonConverterCommon.GetStr(Input.CW);

            string NoteText = JsonConverterCommon.GetStr(Input.Text);
            string ReNoteText = JsonConverterCommon.GetStr(Input.Text);

            string ReNoteSourceUser = JsonConverterCommon.GetStr(Input.Renote.User.UserName);
            string ReNoteSourceUserName = JsonConverterCommon.GetStr(Input.Renote.User.Name);

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
    }
}
