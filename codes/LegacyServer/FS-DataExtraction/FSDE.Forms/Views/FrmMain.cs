using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FSDE.Core;
using FSDE.Dictionaries.config;
using FSDE.Forms.Views;
using FSDE.Model.Config;
using FSDE.Model.Events;
using log4net;
using Microsoft.Win32;

namespace FSDE.Forms
{
    using FSDE.DAL;

    public partial class FrmMain : Form
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ExtractionManager));
        public static bool start = true;
        public static bool stop = false;

        public static long Count = 0;

        private delegate void UpdateMsgHander(RichTextBox rtxmsg, string msgstr);
        public FrmMain()
        {
            InitializeComponent();
        }

        private void projectToolStripButton_Click(object sender, EventArgs e)
        {
            DAL.Common.Initialise();
            var projectSetForm = new FrmProject();
            projectSetForm.ShowDialog();
        }

        private void dataBsaeToolStripButton_Click(object sender, EventArgs e)
        {
            Common.Initialise();
            List<ProjectInfo> list = ProjectInfoDic.GetInstance().GetProjectInfos();
            if (list.Count == 0)
            {
                MessageBox.Show(@"请先配置项目信息");
                return;
            }
            else
            {
                var frmConfig = new FrmConfig();
                frmConfig.ShowDialog();
            }
            
        }

        private void checkToolStripButton_Click(object sender, EventArgs e)
        {
            DAL.Common.Initialise();
            var configForm = new FrmShowConfigInfo();
            configForm.ShowDialog();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            lock (sender)
            {
                if (start)
                {
                    if (ProjectInfoDic.GetInstance().GetProjectInfos().Count > 0)
                    {
                        stop = true;
                        start = false;
                        ExtractionManager.GetExtractionManager().StartExtractDb();
                        ExtractionManager.GetExtractionManager().MessagesShowEventHandler += ExInfo;
                        tsstate.Text = @"状态:正在提取数据库";
                    }
                    else
                    {
                        MessageBox.Show(@"请配置项目信息");
                    }
                }
            }
        }

        public void ExInfo(object sender, EventArgs e)
        {
            var msg = e as MessagesShowEventArgs;
            if (msg != null)
            {
                switch (msg.MessageType)
                {
                    case MsgType.Info:
                        this.UpDateMsgText(this.rtxExtracteInfo, msg.MessagesShow);
                        break;
                    case MsgType.TransInfo:
                        this.UpDateMsgText(this.rtxtransportInfo, msg.MessagesShow);
                        break;
                    case MsgType.Error:
                        this.UpDateMsgText(this.rtxErrorInfo, msg.MessagesShow);
                        break;
                    default:
                        this.UpDateMsgText(this.rtxExtracteInfo, msg.MessagesShow);
                        break;
                }
            }
        }

        private void UpDateMsgText(RichTextBox rtxmsg, string text)
        {
            if (rtxmsg.InvokeRequired)
            {
                rtxmsg.Invoke(new UpdateMsgHander(UpDateMsgText), new object[] { rtxmsg, text });
            }
            else
            {
                int lines = rtxmsg.Lines.Length;
                string toShowText = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss ") + text + "\r\n";
                if (lines > 100)
                {
                    rtxmsg.Clear();
                }

                rtxmsg.AppendText(toShowText);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            lock (sender)
            {
                if (stop)
                {
                    start = true;
                    stop = false;
                    ExtractionManager.GetExtractionManager().MessagesShowEventHandler -= ExInfo;
                    ExtractionManager.GetExtractionManager().StopExtract();
                    tsstate.Text = @"状态:停止提取数据库";
                }
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            Common.Initialise();
            //RegistryKey locaKey = Registry.LocalMachine;
            //RegistryKey run_Check = locaKey.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            //if (run_Check.GetValue("WinFormDataExtr").ToString().ToLower() != "false")
            //{
            //    Count++;
            //    chkStarup.CheckState = CheckState.Checked;
            //    startButton.PerformClick();
            //}
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void chkStarup_CheckedChanged(object sender, EventArgs e)
        {
            //设置开机自动启动
           /* string starupPath = Application.ExecutablePath;
            RegistryKey loca = Registry.LocalMachine;
            RegistryKey run = loca.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (Count != 1)
            {
                if (chkStarup.Checked)
                {
                    try
                    {
                        run.SetValue("WinFormDataExtr", starupPath);
                        MessageBox.Show(@"设置开机启动成功!!", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        loca.Close();
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.Message.ToString(), @"提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    try
                    {
                        run.DeleteValue("WinFormDataExtr");
                        MessageBox.Show(@"开机启动取消!!", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        loca.Close();
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.Message.ToString(), @"提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            Count += 2;*/
        }

    }
}
