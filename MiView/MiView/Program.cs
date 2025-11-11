using MiView.Common.Util;
using MiView.ScreenForms.Controls.Notify;
using MiView.ScreenForms.DialogForm;

namespace MiView
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            try
            {
                Splash.instance.Show();
                Splash.instance.Refresh();
                LogOutput.Write(LogOutput.LOG_LEVEL.INFO, "\r\n");
                LogOutput.Write(LogOutput.LOG_LEVEL.INFO, "開始");
                NotifyView NView = new NotifyView();
                Application.Run(new MainForm() { NotifyView = NView });
                LogOutput.Write(LogOutput.LOG_LEVEL.INFO, "終了");
                LogOutput.Write(LogOutput.LOG_LEVEL.INFO, "\r\n");
                LogOutput.Write(LogOutput.LOG_LEVEL.INFO, "----");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(ex.StackTrace);
                LogOutput.Write(LogOutput.LOG_LEVEL.INFO, "異常終了");
                LogOutput.Write(LogOutput.LOG_LEVEL.INFO, "\r\n");
                LogOutput.Write(LogOutput.LOG_LEVEL.INFO, "----");
            }
        }
    }
}