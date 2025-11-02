using MiView.Common.Notification.Baloon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Notification.Shell
{
    /// <summary>
    /// シェル通知コントローラ
    /// </summary>
    internal class ShellController : NotificationController
    {
        public const string ControllerName = "シェル";
        public ShellController()
        {
            this._ControllerKind = CONTROLLER_KIND.Shell;
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
            return new ShellControlForm();
        }

        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override string ToString()
        {
            return $"通知方法：シェル, {"未実装"}";
        }
    }
}
