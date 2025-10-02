using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Notification.Http
{
    internal class HttpRequestController : NotificationController
    {
        /// <summary>
        /// リクエストURL
        /// </summary>
        public string ReqeustUrl { get; set; }

        public override void ExecuteMethod()
        {
            throw new NotImplementedException();
        }
    }
}
