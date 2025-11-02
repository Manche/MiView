using MiView.Common.Notification.Baloon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Notification.Mail
{
    /// <summary>
    /// メール通知コントローラ
    /// </summary>
    internal class MailController : NotificationController
    {
        public const string ControllerName = "メール";

        public MailController()
        {
            this._ControllerKind = CONTROLLER_KIND.Mail;
        }

        /// <summary>
        /// 通知処理本体
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void ExecuteMethod()
        {
            return;
        }

        public override NotificationControlForm GetControllerForm()
        {
            return new MailControlForm();
        }

        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override string ToString()
        {
            return $"通知方法：メール, {"未実装"}";
        }
    }
}
