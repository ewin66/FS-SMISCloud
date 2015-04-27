using System;
using System.Windows.Forms;
using DataCenter.View;
using log4net;

namespace DataCenter
{
    using System.Threading;

    static class Program
    {
        public static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                //
                Application.ThreadException += new ThreadExceptionEventHandler(UIThreadException);
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                //

                Application.Run(new Form1());
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message);
            }
        }

        private static void UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            try
            {
                string errorMsg = "Windows窗体线程异常: \n\n";
                //MessageBox.Show(errorMsg + t.Exception.Message + Environment.NewLine + t.Exception.StackTrace);
                 Log.Fatal(t.Exception.Message + Environment.NewLine + t.Exception.StackTrace);
                MessageBox.Show(errorMsg + t.Exception.Message + Environment.NewLine);
            }
            catch(Exception ex)
            {
                Log.Fatal(ex.Message);
                MessageBox.Show("不可恢复的Windows窗体异常，应用程序将退出！");
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                Log.Fatal(ex.Message);
                string errorMsg = "未捕获的非窗体线程异常=: \n\n";
                
                MessageBox.Show(errorMsg + ex.Message + Environment.NewLine);
            }
            catch(Exception ex)
            {
                Log.Fatal(ex.Message);
                MessageBox.Show("不可恢复的非Windows窗体线程异常，应用程序将退出！");
            }
        }
    }
}