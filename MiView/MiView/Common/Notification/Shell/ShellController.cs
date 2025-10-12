using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public string? Script { get; set; }
        public string? Output { get { return _Output; } }
        private string? _Output {  get; set; }
        public string? OutError { get { return _OutError; } }
        private string? _OutError { get; set; }

        /// <summary>
        /// 通知処理本体
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void ExecuteMethod()
        {
            if (Script == null)
            {
                throw new ArgumentNullException(nameof(Script));
            }
            var psi = new ProcessStartInfo()
            {
                FileName = "cmd.exe",           // Windowsの場合
                Arguments = $"/c {Script}",     // /c は実行してすぐ終了
                RedirectStandardOutput = true,   // 標準出力を取得
                RedirectStandardError = true,    // 標準エラーも取得
                UseShellExecute = false,         // シェルを使わない
                CreateNoWindow = true            // ウィンドウを表示しない
            };

            using var process = new Process();
            process.StartInfo = psi;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            Console.WriteLine("出力:");
            Console.WriteLine(output);
            _Output = output;

            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("エラー:");
                Console.WriteLine(error);
                _OutError = error;
            }
        }
    }
}
