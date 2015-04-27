using DataCenter.Model;
using DataCenter.Task;

namespace DataCenter.View
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Threading;
    using System.Windows.Forms;
    using Properties;
    using log4net;
    using MakeWarningInfo;
    using ZYB;
    using DataCenter.Util;

    public partial class Form1 : Form
    {
        #region 

        private delegate void SetTextHandler(string text);
        private delegate void DeleTextLogHandler();
        private delegate void SetListViewHandler(ListViewItem lvi, DTUConnectionEventArgs dtu);
        public delegate void TransferEventHandler(string str);

        private delegate void SetTextDuringTime(int day, int hour, int min, int sen);

        private static readonly ILog Log = LogManager.GetLogger(typeof(Form1));
        #endregion

        private readonly WorkThread _work = WorkThread.GetWorkThreadInstance();
       // private Thread _thread;
        #region 静态变量
        
        // added by xiezhen
        public static int RecPort = -1;
        #endregion

        #region 变量

        public static string FormTitle = string.Empty;
        string _iconPath = string.Empty;
        // end of added

        private System.Timers.Timer timer_durtime;
        private System.Timers.Timer _timerCount;

        private int dataShowMode = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Mode"]);

        private readonly DateTime _startTime = DateTime.Now;
        public string MId = string.Empty;
        /// <summary>
        /// 保存DTU信息的哈希表
        /// </summary>
        public static Dictionary<string, DtuInfo> DtuList;

        // 日志最大显示行数
        private int logviewCount = 100;
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        #region 菜单
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ab = new AboutBox1();
            ab.ShowDialog(this);
        }

        private void toolStripButton_start_Click(object sender, EventArgs e)
        {
            启动服务ToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton_stop_Click(object sender, EventArgs e)
        {
            停止服务ToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton_about_Click(object sender, EventArgs e)
        {
            关于ToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            退出ToolStripMenuItem_Click(sender, e);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void 启动服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
               // _work.StartService();
                StartService();
                Log.Info("启动服务成功");
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
            }
        }

        private void 停止服务ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopService();
        }
        #endregion 菜单

        private void Form1_Load(object sender, EventArgs e)
        {
            
            comboBox_mode.SelectedIndex = 0;
            comboBox_show.SelectedIndex = 0;

            // added by xiezhen
            // 读取应用配置
            GetAppSettings();

            // 启动后开始采集
            toolStripButton_start.PerformClick();
            // end of added
            //MakeMsgToRequestDataCalc(4, DateTime.Now);
            WarningThread.GetWarningThread().Start();
        }

        // added by xiezhen
        /// <summary>
        /// 读取应用配置信息
        /// </summary>
        private void GetAppSettings()
        {
            // 读取窗体标题
            FormTitle = System.Configuration.ConfigurationManager.AppSettings["FormTitle"];
            if ((null != FormTitle) && (FormTitle.Length >= 0))
            {
                Text = FormTitle;
            }
            else
            {
                Text = "未知程序";
            }

            // 读取应用图标
            _iconPath = System.Configuration.ConfigurationManager.AppSettings["AppIcon"];
            _iconPath = Application.StartupPath + _iconPath;
            if ((null != _iconPath) && (_iconPath.Length >= 0))
            {
                Icon = File.Exists(_iconPath) ? new Icon(_iconPath) : null;
            }

            // 读取应用端口
            var recPortStr = System.Configuration.ConfigurationManager.AppSettings["Port"];
            try
            {
                RecPort = int.Parse(recPortStr);
            }
            catch(Exception ex)
            {
                MessageBox.Show("端口号错误：" + recPortStr);
                Log.Warn(ex.Message);
            }

            //开机自启状态
            IniSetOpenRun();
            numericUpDown_port.Value = RecPort > 0 ? RecPort : 0;
            numericUpDown_port.Enabled = false;

            string _linecountstr = System.Configuration.ConfigurationManager.AppSettings["logviewCount"];
            if (!string.IsNullOrEmpty(_linecountstr))
            {
                logviewCount = Convert.ToInt32(_linecountstr);
            }
        }

        // end of added
        #region 右击菜单
        private void 清空ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl3.SelectedTab == tabPage4)
            {
                richTextBox_log.Clear();
            }
            else if (tabControl3.SelectedTab == tabPage5)
            {
                richTextBox_rec.Clear();
            }
            else if (tabControl3.SelectedTab == tabPage6)
            {
                richTextBox_sended.Clear();
            }
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl3.SelectedTab == tabPage4)
            {
                richTextBox_log.Copy();
            }
            else if (tabControl3.SelectedTab == tabPage5)
            {
                richTextBox_rec.Copy();
            }
            else if (tabControl3.SelectedTab == tabPage6)
            {
                richTextBox_sended.Copy();
            }
        }

        private void 全选ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl3.SelectedTab == tabPage4)
            {
                richTextBox_log.SelectAll();
            }
            else if (tabControl3.SelectedTab == tabPage5)
            {
                richTextBox_rec.SelectAll();
            }
            else if (tabControl3.SelectedTab == tabPage6)
            {
                richTextBox_sended.SelectAll();
            }
        }
        #endregion
        
        #region 启动停止服务

        private void StartTimer()
        {
            timer_durtime = new System.Timers.Timer(1000);
            timer_durtime.Elapsed += timer_durtime_Tick;
            timer_durtime.Start();
            _timerCount=new System.Timers.Timer(3000);
            _timerCount.Elapsed += timer_count_Tick;
            _timerCount.Start();
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        private void StartService()
        {
            DtuList = new Dictionary<string, DtuInfo>();
            listView1.Items.Clear();
            try
            {
                StartTimer();
                _work.StartService();
                CollectThread.DtuOnOffLineLogEventArgs += CollectThread_DtuOnOffLineLog;
                CollectThread.DataHexShowStringLogEventArgs += CollectThread_ReceiveLog;
                this.SetLogText(string.Format("启动成功,端口号:{0}", numericUpDown_port.Value));
                Log.Info(string.Format("启动成功,端口号:{0}", numericUpDown_port.Value));
                toolStripButton_start.Enabled = false;
                toolStripButton_stop.Enabled = true;
                启动服务ToolStripMenuItem.Enabled = false;
                停止服务ToolStripMenuItem.Enabled = true;
                toolStripStatus_status.Text = "服务状态:启动";
                Log.Info("服务状态:启动");
                toolStripStatus_status.Image = Resources.Green;
                toolStripStatus_port.Text = "端口号:" + numericUpDown_port.Value;
            }
            catch (Exception ex)
            {
                this.SetLogText("启动失败:" + ex.Message);
                Log.Error(ex.Message);
            }
        }
        
        /// <summary>
        /// 终止服务
        /// </summary>
        private void StopService()
        {
            try
            {
                CollectThread.DtuOnOffLineLogEventArgs -= CollectThread_DtuOnOffLineLog;
                CollectThread.DataHexShowStringLogEventArgs -= CollectThread_ReceiveLog;
                _work.StopService();
                SetLogText("停止成功");
                toolStripButton_start.Enabled = true;
                toolStripButton_stop.Enabled = false;
                启动服务ToolStripMenuItem.Enabled = true;
                停止服务ToolStripMenuItem.Enabled = false;
                toolStripStatus_status.Text = "服务状态:停止";
                toolStripStatus_status.Image = Resources.Red;
               // _timerCount.Stop();
                Log.Info("停止成功");
            }
            catch (Exception ee)
            {
                SetLogText("停止失败:" + ee.Message);
                Log.Error(ee.Message);
            }
        }
        #endregion

        #region 日志

        void CollectThread_ReceiveLog(ReceiveDataInfo e)
        {
            var data = CVT.ByteToHexStr(e.PackagesBytes);
            SetRecText(string.Format("收到数据,ID:{0},长度:{1},数据:{2}", e.Sender, e.PackagesBytes.Length, data));
            SetLogText(string.Format("收到数据,ID:{0},长度:{1}", e.Sender, e.PackagesBytes.Length));
        }

        /// <summary>
        /// 系统日志
        /// </summary>
        /// <param name="text"></param>
        private void SetLogText(string text)
        {
            if (richTextBox_log.InvokeRequired)
            {
                richTextBox_log.Invoke(new SetTextHandler(SetLogText), new object[] { text });
            }
            else
            {
                if (richTextBox_log.Lines.Length >= logviewCount)
                    richTextBox_log.Clear();
                richTextBox_log.Focus();
                richTextBox_log.AppendText(string.Format("<{0}>:{1}\r\n", DateTime.Now, text));
            }
        }

        /// <summary>
        /// 已收数据
        /// </summary>
        /// <param name="text"></param>
        private void SetRecText(string text)
        {
            if (richTextBox_rec.InvokeRequired)
            {
                richTextBox_rec.Invoke(new SetTextHandler(SetRecText), new object[] { text });
            }
            else
            {
                if (richTextBox_rec.Lines.Length >= logviewCount)
                    richTextBox_rec.Clear();
                richTextBox_rec.Focus();
                richTextBox_rec.AppendText(string.Format("<{0}>:{1}\r\n", DateTime.Now, text));
                Log.Info(text);
            }
        }
        
        #endregion
        
        #region DTU上下线事件
        void CollectThread_DtuOnOffLineLog(DTUConnectionEventArgs e)
        {
            if (e != null)
            {
                switch (e.Status)
                {
                    case ReceiveType.Online:
                        this.svr_ClientConnect(e);
                        break;
                    case ReceiveType.Offline:
                        this.svr_ClientClose(e);
                        break;
                    default:
                        break;
                }
            }
        }

        private void svr_ClientClose(DTUConnectionEventArgs e)
        {
            if (DtuList.ContainsKey(e.DtuId))
            {
                var lvi = DtuList[e.DtuId].Lvi;
                lvi.BackColor = Color.Pink;

                try
                {
                    var dtuId = int.Parse(e.DtuId);
                    DtuList[e.DtuId].IsOnline = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Log.Error(ex.Message);
                }
                SetLogText(string.Format("{0}下线", e.DtuId));
                Log.Info(string.Format("{0}下线", e.DtuId));
            }
        }

        private void svr_ClientConnect(DTUConnectionEventArgs e)
        {
            if (e == null) return;
            var dtu = e;
            if (DtuList.ContainsKey(dtu.DtuId)) //如果列表中已含有该ID号的DTU
            {
                var lvi = DtuList[dtu.DtuId].Lvi;
                SetListView(lvi, dtu);
                DtuList[dtu.DtuId].IsOnline = true;
            }
            else
            {
                var lvi =
                    new ListViewItem(new[] { dtu.DtuId, dtu.Ip, dtu.PhoneNumber, dtu.LoginTime.ToString(), dtu.RefreshTime.ToString() })
                    {
                        BackColor = Color.LightGreen
                    };
                SetListView(lvi,null);
                var dinf = new DtuInfo { Lvi = lvi,IsOnline = true};
                DtuList.Add(dtu.DtuId, dinf);
            }
            try
            {
                var dtuId = int.Parse(dtu.DtuId);
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
            }
            SetLogText(string.Format("{0}上线", dtu.DtuId));
            Log.Info(string.Format("{0}上线", dtu.DtuId));
        }
        #endregion DTU上下线
        

        /// <summary>
        /// 登陆信息
        /// </summary>
        /// <param name="lvi"></param>
        private void SetListView(ListViewItem lvi, DTUConnectionEventArgs dtu)
        {
            if (listView1.InvokeRequired)
            {

                listView1.Invoke(new SetListViewHandler(SetListView), new object[] { lvi,dtu });
            }
            else
            {
                if (dtu == null)
                    listView1.Items.Add(lvi);
                else
                {
                    lvi.BackColor = Color.LightGreen;
                    lvi.SubItems[0].Text = dtu.DtuId;
                    lvi.SubItems[1].Text = dtu.Ip;
                    lvi.SubItems[2].Text = dtu.PhoneNumber;
                    lvi.SubItems[3].Text = dtu.LoginTime.ToString();
                    lvi.SubItems[4].Text = dtu.RefreshTime.ToString();
                }
            }
        }
        
        #region 控件事件

        private void SetTimeText(int day, int hour, int min, int sen)
        {
           if (this.InvokeRequired)
           {
               this.BeginInvoke(new SetTextDuringTime(SetTimeText), new object[] { day, hour, min, sen });
           }
           else
               toolStripStatus_durtime.Text = string.Format("已运行:{0}天{1}时{2}分{3}秒", day, hour
                                                              , min, sen);
        }
        /// <summary>
        /// 记录系统运行时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_durtime_Tick(object sender, EventArgs e)
        {
            var ts = DateTime.Now - _startTime;
            SetTimeText(ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
        }


        /// <summary>
        /// 计算在线终端数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_count_Tick(object sender, EventArgs e)
        {

        }
        
        private void 断开ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (listView1.Items.Count == 0 || listView1.SelectedItems.Count == 0)
            {
                return;
            }
        }


        #endregion 控件事件

        private void toolStripBtnOpenAutoRun_Click(object sender, EventArgs e)
        {
            try
            {

                if (toolStripBtnOpenAutoRun.Text == "开机自启(√)")
                {
                    HelperTools.StartupAutoRun(false);
                    toolStripBtnOpenAutoRun.Text = "开机自启(x)";
                    toolStripBtnOpenAutoRun.Image = Resources.Red;
                    HelperTools.UpdateAppConfig("OpenRun", Boolean.FalseString);
                    SetLogText("关闭开机自启");

                }
                else if (toolStripBtnOpenAutoRun.Text == "开机自启(x)")
                {
                    HelperTools.StartupAutoRun(true);
                    HelperTools.UpdateAppConfig("OpenRun", Boolean.TrueString);
                    toolStripBtnOpenAutoRun.Text = "开机自启(√)";
                    toolStripBtnOpenAutoRun.Image = Resources.Green;
                    SetLogText("设置开机自启");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                SetLogText("设置失败");
            }
        }


        private void IniSetOpenRun()
        {
            if (HelperTools.GetConfigString("OpenRun") == Boolean.FalseString)
            {
                toolStripBtnOpenAutoRun.Text = "开机自启(x)";
                toolStripBtnOpenAutoRun.Image = Resources.Red;
            }
            else if (HelperTools.GetConfigString("OpenRun") == Boolean.TrueString)
            {
                toolStripBtnOpenAutoRun.Text = "开机自启(√)";
                toolStripBtnOpenAutoRun.Image = Resources.Green;
            }
        }

        private void toolStripBtnRestart_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CollectThread.UpdateAllOnlineDtus();
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}