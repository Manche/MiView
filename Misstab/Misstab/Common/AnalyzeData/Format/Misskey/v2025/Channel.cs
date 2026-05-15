using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Misstab.Common.AnalyzeData.Format.Misskey.v2025
{
    public class Channel
    {
        public JsonNode? Node { get; set; }
        public JsonNode? Id { get { return this.Node?["id"]; } }
        public JsonNode? Name { get { return this.Node?["name"]; } }
        public JsonNode? Color { get { return this.Node?["color"]; } }


        public JsonNode? isFollowing { get { return this.Node?["isFollowing"]; } }
        public JsonNode? isFavorited { get { return this.Node?["isFavorited"]; } }


        // 以下考え中
    }
}
