using MiView.Common.ApplicationData;
using MiView.Common.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiView.Common.Util
{
    public static class LogOutput
    {
        public enum LOG_LEVEL
        {
            INFO,
            DEBUG,
            ERROR,
            WARNING,
            FATAL,
        }

        static LogOutput()
        {
            if (!Directory.Exists(LogConst.LOG_DIR))
            {
                Directory.CreateDirectory(LogConst.LOG_DIR);
            }
        }

        public static void Write(LOG_LEVEL Level, string Message)
        {
            try
            {
                string OutputLogFile = Path.Combine(LogConst.LOG_DIR, $"{DateTime.Now:yyyyMMdd}.log");

                lock (OutputLogFile)
                {
                    try
                    {
                        File.AppendAllText(OutputLogFile, $"{DateTime.Now:yyyyMMddHHmmss}\t[{Level}] {Message}\r\n");
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception e)
            {
                // WriteEventLog(System.Diagnostics.EventLogEntryType.Error, $"ログファイルの書き込みに失敗しました。{e.ToString()} {e.StackTrace}");
            }
        }

        public static void WriteEventLog(System.Diagnostics.EventLogEntryType EventType, string Message)
        {
            System.Diagnostics.EventLog.WriteEntry(ApplicationConst.ApplicationLogicalName, Message, EventType, 9, 1);
        }
    }

    public static class LogConst
    {
        /// <summary>
        /// 設定ディレクトリ
        /// </summary>
        public static readonly string LOG_DIR = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MiView");
    }
}
