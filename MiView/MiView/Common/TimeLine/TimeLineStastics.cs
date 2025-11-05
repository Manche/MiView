using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.TimeLine
{
    public class TimeLineStastics
    {
        /// <summary>
        /// タブ識別値
        /// </summary>
        public string? _TabDefinition { get; set; }
        /// <summary>
        /// タブ名
        /// </summary>
        public string? _TabName {  get; set; }
        /// <summary>
        /// ノート数
        /// </summary>
        public int _NoteCount { get; set; } = 0;
        /// <summary>
        /// 最終更新日時
        /// </summary>
        public DateTime? _LatestUpdate { get; set; } = DateTime.MinValue;
    }
}
