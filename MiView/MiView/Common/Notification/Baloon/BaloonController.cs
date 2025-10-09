﻿using System;
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
        /// バルーンタイトル
        /// </summary>
        public string BaloonTitle { get; set; } = string.Empty;
        /// <summary>
        /// バルーン本文
        /// </summary>
        public string BaloonContent { get; set; } = string.Empty;

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
    }
}
