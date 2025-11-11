using MiView.Common.Notification.Baloon;
using MiView.Common.Setting;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        /// <summary>
        /// 再生間隔
        /// </summary>
        public int Distance { get; set; } = 0;

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
            if (SettingState.Instance.IsMuted)
            {
                return;
            }
            if (!File.Exists(FilePath))
            {
                return;
            }
            for (int i = 0; i < PlayTimes; i++)
            {
                NotificationSoundAudioHelper.Instance.PlaySound(FilePath, Volume);
                Thread.Sleep(Distance);
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
            return $"通知方法：音声, 音量：{Volume}, 回数：{PlayTimes}, ファイルパス：{FilePath}";
        }
    }

    public class NotificationSoundAudioHelper
    {
        public static NotificationSoundAudioHelper Instance { get; } = new NotificationSoundAudioHelper();
        private Dictionary<string, int> PlayState = new Dictionary<string, int>();

        private int _MaxBuf = 3;

        public void PlaySound(string FilePath, int Volume)
        {
            if (!PlayState.ContainsKey(FilePath))
            {
                PlayState[FilePath] = 0;
            }
            Console.WriteLine(PlayState[FilePath]);
            System.Diagnostics.Debug.WriteLine(PlayState[FilePath]);
            if (PlayState[FilePath] > _MaxBuf)
            {
                return;
            }
            _ = Task.Run(async () => {
                lock (PlayState)
                {
                    PlayState[FilePath]++;
                }
                using (var audioFile = new AudioFileReader(FilePath))
                using (var outputDevice = new WaveOutEvent())
                {

                    outputDevice.Init(audioFile);
                    outputDevice.Volume = (float)Volume / 100;
                    outputDevice.Play();
                    while (outputDevice.PlaybackState == PlaybackState.Playing)
                    {
                        await Task.Delay(10);
                    }
                }
                lock (PlayState)
                {
                    PlayState[FilePath]--;
                }
            });
        }
    }
}
