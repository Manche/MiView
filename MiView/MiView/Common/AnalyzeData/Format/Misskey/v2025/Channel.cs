using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace MiView.Common.AnalyzeData.Format
{
    public class Channel
    {
        public JsonNode? Node { get; set; }
        public JsonNode? Id { get { return this.Node?["id"]; } }
        public JsonNode? Name { get { return this.Node?["name"]; } }
        public JsonNode? Color { get { return this.Node?["color"]; } }
    }
}
