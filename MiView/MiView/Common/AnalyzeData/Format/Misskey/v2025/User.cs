using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MiView.Common.AnalyzeData.Format.Misskey.v2025
{
    public class User
    {
        public JsonNode? Node { get; set; }
        public JsonNode? Id { get { return Node?["id"]; } }
        public JsonNode? Name { get { return Node?["name"]; } }
        public JsonNode? UserName { get { return Node?["username"]; } }
        public JsonNode? Host { get { return Node?["host"]; } }
        public JsonNode? AvatarUrl { get { return Node?["avatarUrl"]; } }
        public JsonNode? AvatarBlurhash { get { return Node?["avatarBlurhash"]; } }
        public JsonNode? AvatarDecorations { get { return Node?["avatarDecorations"]; } }
        public JsonNode? IsBot { get { return Node?["isBot"]; } }
        public JsonNode? IsCat { get { return Node?["isCat"]; } }
        public JsonNode? Emojis { get { return Node?["emojis"]; } }
        public Instance Instance { get { return new Instance() { Node = Node?["instance"] }; } }
        public JsonNode? OnlineStatus { get { return Node?["onlineStatus"]; } }
        public JsonNode? Roles { get { return Node?["badgeRoles"]; } }
    }
}
