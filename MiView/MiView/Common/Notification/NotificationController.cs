using MiView.Common.TimeLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public TimeLineContainer _Container { get; set; } = new TimeLineContainer();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NotificationControllerCommon()
        {
            _ControllerKey = Guid.NewGuid();
            _ControllerName = string.Empty;
        }

        public NotificationControllerCommon Create()
        {
            return new NotificationControllerCommon();
        }
    }
}
