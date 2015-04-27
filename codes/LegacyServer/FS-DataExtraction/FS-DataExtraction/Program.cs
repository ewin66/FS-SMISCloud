

namespace FS_DataExtraction
{
    using System;
    using System.Windows.Forms;
    using System.Reflection;

    using log4net;

    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            ILog log = LogManager.GetLogger(typeof(Program));
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // 程序升级替换*.dll在此实现（未实现）
                var assType = Assembly.Load("FSDE.Forms");
                //var objType = assType.CreateInstance("FSDE.Forms.Views.Form9"); // 
                var objType = assType.CreateInstance("FSDE.Forms.FrmMain"); // 
                if (object.Equals(objType, null))
                {
                    if (MessageBox.Show(@"系统加载重要组件失败，不能运行系统", @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        == DialogResult.OK)
                    {
                        return;
                    }
                }

                Application.Run(objType as Form);
            }
            catch (Exception ex)
            {
                log.Fatal(ex.Message);
                MessageBox.Show(ex.Message, @"错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
