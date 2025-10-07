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
        /// リクエストメソッド
        /// </summary>
        private HttpMethod _HttpMethod { get; set; } = HttpMethod.Get;
        public HttpMethod HttpMethod { get { return _HttpMethod; } }

        /// <summary>
        /// httpクライアント
        /// </summary>
        private HttpClient _HttpClient {  get; set; } = new HttpClient();
        public HttpClient HttpClient { get { return _HttpClient; } }

        /// <summary>
        /// Httpリクエスト
        /// </summary>
        private HttpRequestMessage? _HttpRequest {  get; set; }
        public HttpRequestMessage? HttpRequest { get { return _HttpRequest; } }

        /// <summary>
        /// Httpレスポンス
        /// </summary>
        private HttpResponseMessage? _HttpResponse { get; set; }
        public HttpResponseMessage? HttpResponse { get { return _HttpResponse; } }

        /// <summary>
        /// レスポンスボディ
        /// </summary>
        private string? _HttpResponseBody {  get; set; }
        public string? HttpResponseBody { get { return _HttpResponseBody; } }

        /// <summary>
        /// リクエストURL
        /// </summary>
        public string ReqeustUrl { get; set; } = string.Empty;
        /// <summary>
        /// リクエストBody
        /// </summary>
        /// <remarks>
        /// サイトによって異なる
        /// </remarks>
        public string RequestBody { get; set; } = string.Empty;
        /// <summary>
        /// エンコード
        /// </summary>
        public Encoding RequestEncode {  get; set; } = Encoding.UTF8;
        /// <summary>
        /// リクエストアプリ
        /// </summary>
        public string RequestType {  get; set; } = "application/json";
        /// <summary>
        /// リクエスト処理本体
        /// </summary>
        private StringContent _RequestContent { get { return new StringContent(this.RequestBody, RequestEncode, RequestType); } }
        /// <summary>
        /// リクエストヘッダー
        /// </summary>
        public Dictionary<string, string> RequestHeader = new Dictionary<string, string>();

        /// <summary>
        /// リクエスト方法
        /// </summary>
        public enum EXECUTE_PROCESS
        {
            /// <summary>
            /// シンプル、GET
            /// </summary>
            GET,
            /// <summary>
            /// POST
            /// </summary>
            POST,
        }
        public EXECUTE_PROCESS ExecuteProcess { get; set; } = EXECUTE_PROCESS.GET;

        /// <summary>
        /// 通知処理本体
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void ExecuteMethod()
        {
            if (ReqeustUrl == string.Empty)
            {
                throw new NotImplementedException();
            }
            Task Mt;
            switch (ExecuteProcess)
            {
                case EXECUTE_PROCESS.GET:
                    Mt = new Task(async () =>
                    {
                        await GetSubHttpRoutine();
                    });
                    Mt.Start();
                    break;
                case EXECUTE_PROCESS.POST:
                    Mt = new Task(async () =>
                    {
                        await PostSubHttpRoutine();
                    });
                    Mt.Start();
                    break;
            }
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        private async Task GetSubHttpRoutine()
        {
            /// リクエスト作成
            this._HttpRequest = new HttpRequestMessage(_HttpMethod, this.ReqeustUrl);
            if (this._HttpRequest == null)
            {
                return;
            }

            // ボディ
            this._HttpRequest.Content = this._RequestContent;

            // ヘッダ
            foreach (var Head in this.RequestHeader)
            {
                this._HttpRequest?.Headers.Add(Head.Key, Head.Value);
            }

            this._HttpResponse = await this._HttpClient.SendAsync(this._HttpRequest);
            this._HttpResponseBody = await this._HttpResponse.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <returns></returns>
        private async Task PostSubHttpRoutine()
        {
        }
    }
}
