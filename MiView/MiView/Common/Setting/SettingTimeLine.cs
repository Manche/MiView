using MiView.Common.Connection.WebSocket;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Setting
{
    public class SettingTimeLine
    {
        public required string Definition { get; set; }
        public required string TabName { get; set; }
        public bool FilterMode { get; set; } = true;
        public bool IsFiltered { get; set; } = false;
        public bool IsVisible { get; set; } = true;
        public bool IsUpdateTL { get; set; } = true;
        public required List<TimeLineFilterlingOption> FilteringOptions { get; set; }
        public required List<TimeLineAlertOption> AlertOptions { get; set; }
        public static SettingTimeLine ConvertDataGridTimeLineToSettingObj(DataGridTimeLine WSTimeLine)
        {
            WSTimeLine._FilteringOptions.ForEach(r => { r._Container = null; });
            return new SettingTimeLine()
            {
                Definition = WSTimeLine._Definition,
                TabName = WSTimeLine._TabName,
                FilterMode = WSTimeLine._FilterMode,
                IsFiltered = WSTimeLine._IsFiltered,
                IsVisible = WSTimeLine.Visible,
                IsUpdateTL = WSTimeLine._IsUpdateTL,

                FilteringOptions = WSTimeLine._FilteringOptions,
                AlertOptions = WSTimeLine._AlertOptions,
            };
        }
    }
}
