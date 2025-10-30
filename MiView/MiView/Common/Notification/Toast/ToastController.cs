using MiView.Common.Notification.Baloon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Notification.Toast
{
    /// <summary>
    /// トースト通知コントローラ
    /// </summary>
    internal class ToastController : NotificationController
    {
        public const string ControllerName = "トースト";
        /// <summary>
        /// 通知処理本体
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void ExecuteMethod()
        {
            return;
        }


        public override Control GetControllerForm()
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
            return $"通知方法：トースト, {"未実装"}";
        }
    }
}
