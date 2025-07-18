using System.Text.Json.Nodes;

namespace MiView.Common.TimeLine
{
    public class TimeLineContainer
    {
        public enum PROTECTED_STATUS
        {
            NONE = 0,
            PROTECTED = 1,
            FOLLOWER_ONLY = 2
        }

        public string USERID { get; set; } = "";
        public string USERNAME { get; set; } = "";
        public string TLFROM { get; set; } = "";
        public bool RENOTED { get; set; } = false;
        public bool REPLAYED { get; set; } = false;
        public PROTECTED_STATUS ProtectedStatus { get; set; } = PROTECTED_STATUS.NONE;
        public JsonNode? OriginalNode { get; set; }
    }
}