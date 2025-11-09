using MiView.Common.Notification.Baloon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

using System.Reflection;

namespace MiView.Common.Notification.Sound
{
    public class NotificationSoundController : NotificationController
    {
        public const string ControllerName = "音声再生";
        /// <summary>
        /// ファイルパス
        /// </summary>
        public string FilePath {  get; set; } = string.Empty;
        /// <summary>
        /// ボリューム
        /// </summary>
        public int Volume { get; set; } = 100;
        /// <summary>
        /// 再生回数
        /// </summary>
        public int PlayTimes { get; set; } = 1;

        public NotificationSoundController()
        {
            this._ControllerKind = CONTROLLER_KIND.NotificationSound;
        }

        /// <summary>
        /// 通知処理本体
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void ExecuteMethod()
        {
            if (!File.Exists(FilePath))
            {
                return;
            }
            for (int i = 0; i < PlayTimes; i++)
            {
                using (var audioFile = new AudioFileReader(FilePath))
                using (var outputDevice = new WaveOutEvent())
                {
                    outputDevice.Init(audioFile);
                    outputDevice.Volume = (float)Volume / 100;
                    outputDevice.Play();

                    // 再生が終了するまで待機する
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                    }
                }
            }
        }

        public override NotificationSoundControlForm GetControllerForm()
        {
            return new NotificationSoundControlForm();
        }

        /// <summary>
        /// ToString()
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override string ToString()
        {
            return $"通知方法：音声, 音量：{Volume}, ファイルパス：{FilePath}";
        }
    }
}
