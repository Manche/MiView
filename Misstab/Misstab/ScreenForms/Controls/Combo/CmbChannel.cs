using Misstab.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misstab.ScreenForms.Controls.Combo
{
    internal class CmbChannel
    {
        public string? _ChannelId { get; set; }
        public string? _ChannelName { get; set; }

        public CmbChannel()
        {
        }
        public CmbChannel(string ChannelId, string ChannelName)
        {
            _ChannelId = ChannelId;
            _ChannelName = ChannelName;
        }

        public override string ToString()
        {
            return _ChannelName??"";
        }
    }
}
