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
        /// <summary>
        /// 名称
        /// </summary>
        public const string ControllerName = "バルーン";
        /// <summary>
        /// バルーンタイトル
        /// </summary>
        public string BaloonTitle { get; set; } = string.Empty;
        /// <summary>
        /// バルーン本文
        /// </summary>
        public string BaloonContent { get; set; } = string.Empty;

        public BaloonController()
        {
            this._ControllerKind = CONTROLLER_KIND.Baloon;
        }

        /// <summary>
        /// 通知処理本体
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void ExecuteMethod()
        {
            using (NotifyIcon Icn = new NotifyIcon())
            {
                Icn.Icon = SystemIcons.Information;
                Icn.Visible = true;

                Icn.ShowBalloonTip(3000, GetFormattedStr(BaloonTitle), GetFormattedStr(BaloonContent), ToolTipIcon.Info);
            }
        }

        public override NotificationControlForm GetControllerForm()
        {
            return new BaloonControlForm();
        }

        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override string ToString()
        {
            return $"通知方法：バルーン, タイトル：{BaloonTitle}, 本文：{BaloonContent}";
        }
    }
}
