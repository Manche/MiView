using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MiView.Common.AnalyzeData.Format.Misskey.v2025
{
    public class Note
    {
        public JsonNode? Node {  get; set; }
        public JsonNode? Id { get { return Node?["Id"]; } }
        public JsonNode? CreatedAt { get { return Node?["createdAt"]; } }
        public JsonNode? UserId { get { return Node?["userId"]; } }
        public JsonNode? UserDetail { get { return Node?["user"]; } }
        public User User { get { return new User() { Node = Node?["user"] }; } }
        public JsonNode? Text { get { return Node?["text"]; } }
        public JsonNode? CW { get { return Node?["cw"]; } }
        public JsonNode? Visibility { get { return Node?["visibility"]; } }
        public JsonNode? LocalOnly { get { return Node?["localOnly"]; } }
        public JsonNode? ReactionAcceptance { get { return Node?["reactionAcceptance"]; } }
        public JsonNode? RenoteCount { get { return Node?["renoteCount"]; } }
        public JsonNode? ReplyId { get { return Node?["replyId"]; } }
        public Note Reply { get { return new Note() { Node = Node?["reply"] }; } }
        public JsonNode? RenoteId { get { return Node?["renoteId"]; } }
        public Note Renote { get { return new Note() { Node = Node?["renote"] }; } }
        public JsonNode? ChannelId { get { return Node?["channelId"]; } }
        public Channel Channel { get { return new Channel() { Node = Node?["channel"] }; } }
    }
}
