using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiView.Common.Notification.Http
{
    /// <summary>
    /// Httpリクエスト通知コントローラ
    /// </summary>
    internal class HttpRequestController : NotificationController
    {
        /// <summary>
        /// httpクライアント
        /// </summary>
        private HttpClient? _Client {  get; set; }

        /// <summary>
        /// リクエストURL
        /// </summary>
        public required string ReqeustUrl { get; set; }

        /// <summary>
        /// リクエストBODY
        /// </summary>
        public string? RequestBody { get; set; }

        /// <summary>
        /// メソッド
        /// </summary>
        public enum HTTP_METHOD
        {
            NONE,
            GET,
            HEAD,
            POST,
            PUT,
            DELETE,
            CONNECT,
            OPTIONS,
            TRACE,
            PATCH
        }

        /// <summary>
        /// メソッド指定
        /// </summary>
        public required HTTP_METHOD _Method { get; set; } = HTTP_METHOD.NONE;

        public override void ExecuteMethod()
        {
            _Client = new HttpClient();

            Task Tk = new Task(async () =>
            {
                string ResponseBody = await ExecuteRequest(_Client);
                if (ResponseBody != null)
                {
                    this.RequestBody = ResponseBody;
                }
                System.Diagnostics.Debug.WriteLine("受信Event：");
                System.Diagnostics.Debug.WriteLine(ResponseBody);
            });
            Tk.Start();
        }

        private async Task<string> ExecuteRequest(HttpClient Client)
        {
            var HttpResponse = await Client.GetAsync(ReqeustUrl);
            string ResponseBody = await HttpResponse.Content.ReadAsStringAsync();

            return ResponseBody;
        }
    }
}
