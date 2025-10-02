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
        public override void ExecuteMethod()
        {
            using (NotifyIcon Icn = new NotifyIcon())
            {
                Icn.Icon = SystemIcons.Information;
                Icn.Visible = true;

                Icn.ShowBalloonTip(3000, "test", "content", ToolTipIcon.Info);
            }
        }
    }
}
