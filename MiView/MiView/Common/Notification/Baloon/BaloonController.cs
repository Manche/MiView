using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Notification.Baloon
{
    /// <summary>
    /// バルーン通知コントローラ
    /// </summary>
    internal class BaloonController : NotificationController
    {
        public string BaloonTitle { get; set; } = string.Empty;
        public string BaloonContent { get; set; } = string.Empty;

        public override void ExecuteMethod()
        {
            using (NotifyIcon Icn = new NotifyIcon())
            {
                Icn.Icon = SystemIcons.Information;
                Icn.Visible = true;

                Icn.ShowBalloonTip(3000, GetFormattedStr(BaloonTitle), GetFormattedStr(BaloonContent), ToolTipIcon.Info);
            }
        }
    }
}
