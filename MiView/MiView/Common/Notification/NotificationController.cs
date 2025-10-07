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
    abstract class NotificationController : NotificationControllerCommon
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

        /// <summary>
        /// 通知実行処理
        /// </summary>
        public abstract void ExecuteMethod();
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
