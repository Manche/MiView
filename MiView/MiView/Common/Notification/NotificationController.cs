using MiView.Common.Notification.Baloon;
using MiView.Common.Notification.Http;
using MiView.Common.Notification.Mail;
using MiView.Common.Notification.Shell;
using MiView.Common.Notification.Toast;
using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiView.Common.Notification
{
    /// <summary>
    /// 通知コントローラー
    /// </summary>
    /// <remarks>
    /// 通知追加にはこのコントローラを参照する
    /// </remarks>
    public abstract class NotificationController : NotificationControllerCommon
    {
        /// <summary>
        /// 通知実行
        /// </summary>
        public void Execute()
        {
            try
            {
                ExecuteMethod();
            }
            catch (Exception ce)
            {
            }
        }

        public enum CONTROLLER_KIND
        {
            None = 0,
            Baloon,
            HttpRequest,
            Mail,
            Shell,
            Toast
        }
        public static Dictionary<CONTROLLER_KIND, string> ControllerKindName = new Dictionary<CONTROLLER_KIND, string>()
        {
            {CONTROLLER_KIND.Baloon, BaloonController.ControllerName},
            {CONTROLLER_KIND.Mail, MailController.ControllerName},
            {CONTROLLER_KIND.Shell, ShellController.ControllerName},
            {CONTROLLER_KIND.HttpRequest, HttpRequestController.ControllerName},
            {CONTROLLER_KIND.Toast, ToastController.ControllerName},
        };

        private CONTROLLER_KIND _ControllerKind { get; set; } = CONTROLLER_KIND.None;
        public string ControllerKindToString { get { return ControllerKindName[_ControllerKind]; } }
        public abstract Control GetControllerForm();

        /// <summary>
        /// インスタンス作成
        /// </summary>
        /// <param name="Kind"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static NotificationController Create(CONTROLLER_KIND Kind)
        {
            switch (Kind)
            {
                case CONTROLLER_KIND.Baloon:
                    return new BaloonController() { _ControllerKind = CONTROLLER_KIND.Baloon};
                case CONTROLLER_KIND.HttpRequest:
                    return new HttpRequestController() { _ControllerKind = CONTROLLER_KIND.HttpRequest };
                case CONTROLLER_KIND.Mail:
                    return new MailController() { _ControllerKind = CONTROLLER_KIND.Mail };
                case CONTROLLER_KIND.Shell:
                    return new ShellController() { _ControllerKind = CONTROLLER_KIND.Shell };
                case CONTROLLER_KIND.Toast:
                    return new ToastController() { _ControllerKind = CONTROLLER_KIND.Toast };
            }

            throw new NotImplementedException();
        }

        /// <summary>
        /// 通知処理本体
        /// </summary>
        public abstract void ExecuteMethod();
        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();
    }

    /// <summary>
    /// 通知コントローラ共通処理
    /// </summary>
    public class NotificationControllerCommon
    {
        /// <summary>
        /// コントローラ識別キー
        /// </summary>
        public Guid _ControllerKey { get; set; }

        /// <summary>
        /// コントローラ名
        /// </summary>
        public string _ControllerName { get; set; }

        /// <summary>
        /// タイムラインコンテナ
        /// </summary>
        private TimeLineContainer _Container { get; set; } = new TimeLineContainer();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NotificationControllerCommon()
        {
            _ControllerKey = Guid.NewGuid();
            _ControllerName = string.Empty;
        }

        /// <summary>
        /// タイムラインコンテナの設定
        /// </summary>
        /// <param name="Container"></param>
        public void SetTimeLineContainer(TimeLineContainer Container)
        {
            _Container = Container;
        }

        public string GetFormattedStr(string StrInput)
        {
            PropertyInfo[] PropInfos = typeof(TimeLineContainer).GetProperties();
            string[] PropNames = PropInfos.ToList().FindAll(r => { return TimeLineContainer.TRANSABLE.Contains(r.Name); }).Select(p => { return p.Name; }).ToArray();

            foreach (string Name in PropNames)
            {
                string Mt = @"\[["+Name+@"\]]+\";
                var Prp = this._Container.GetType().GetProperty(Name);
                if (Prp == null)
                {
                    continue;
                }
                var Rt = Prp.GetValue(this._Container);
                if (Rt == null)
                {
                    continue;
                }

                string Ptn = @"\[(.*?)\]"; // [～] の中身をキャプチャ
                string Rs = Regex.Replace(StrInput, Ptn, match =>
                {
                    string k = match.Groups[1].Value; // [NAME] → "NAME"

                    if (k == Name)
                    {
                        return Prp.GetValue(this._Container).ToString();
                    }
                    return match.Value;
                });
                StrInput = Rs;
            }
            if (StrInput == string.Empty)
            {
                return "ERR";
            }

            return StrInput;
        }
    }
}
