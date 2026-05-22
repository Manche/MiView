using Misstab.Common.Connection.WebSocket;
using Misstab.Common.TimeLine;
using Misstab.Common.TimeLine.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Misstab.Common.Setting
{
    public class SettingTimeLine
    {
        public string LastUpdated { get; set; } = DateTime.Now.ToString();
        public required string Definition { get; set; }
        public required string TabName { get; set; }
        public bool FilterMode { get; set; } = true;
        public bool IsFiltered { get; set; } = false;
        public bool IsVisible { get; set; } = true;
        public bool IsUpdateTL { get; set; } = true;
        public List<TimeLineFilterlingOption>? FilteringOptions { get; set; }
        public List<TimeLineAlertOption>? AlertOptions { get; set; }
        public DataGridTimeLineViewSetting? ViewSetting { get; set; }
        public bool IsSaveIcon { get; set; } = true;
        public static SettingTimeLine ConvertDataGridTimeLineToSettingObj(DataGridTimeLine WSTimeLine)
        {
            WSTimeLine._FilteringOptions?.ForEach(r => { r._Container = null; });
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
                ViewSetting = WSTimeLine._ViewSetting,

                IsSaveIcon = WSTimeLine._IsSaveIcon,
            };
        }
    }

    public class SettingTimeLineConst
    {
        public const int MAX_TIMELINE_COUNT = 20000;
    }
}
