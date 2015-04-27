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
        #region ��̬����
        
        // added by xiezhen
        public static int RecPort = -1;
        #endregion

        #region ����

        public static string FormTitle = string.Empty;
        string _iconPath = string.Empty;
        // end of added

        private System.Timers.Timer timer_durtime;
        private System.Timers.Timer _timerCount;

        private int dataShowMode = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Mode"]);

        private readonly DateTime _startTime = DateTime.Now;
        public string MId = string.Empty;
        /// <summary>
        /// ����DTU��Ϣ�Ĺ�ϣ��
        /// </summary>
        public static Dictionary<string, DtuInfo> DtuList;

        // ��־�����ʾ����
        private int logviewCount = 100;
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        #region �˵�
        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ab = new AboutBox1();
            ab.ShowDialog(this);
        }

        private void toolStripButton_start_Click(object sender, EventArgs e)
        {
            ��������ToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton_stop_Click(object sender, EventArgs e)
        {
            ֹͣ����ToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton_about_Click(object sender, EventArgs e)
        {
            ����ToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            �˳�ToolStripMenuItem_Click(sender, e);
        }

        private void �˳�ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
               // _work.StartService();
                StartService();
                Log.Info("��������ɹ�");
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
            }
        }

        private void ֹͣ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopService();
        }
        #endregion �˵�

        private void Form1_Load(object sender, EventArgs e)
        {
            
            comboBox_mode.SelectedIndex = 0;
            comboBox_show.SelectedIndex = 0;

            // added by xiezhen
            // ��ȡӦ������
            GetAppSettings();

            // ������ʼ�ɼ�
            toolStripButton_start.PerformClick();
            // end of added
            //MakeMsgToRequestDataCalc(4, DateTime.Now);
            WarningThread.GetWarningThread().Start();
        }

        // added by xiezhen
        /// <summary>
        /// ��ȡӦ��������Ϣ
        /// </summary>
        private void GetAppSettings()
        {
            // ��ȡ�������
            FormTitle = System.Configuration.ConfigurationManager.AppSettings["FormTitle"];
            if ((null != FormTitle) && (FormTitle.Length >= 0))
            {
                Text = FormTitle;
            }
            else
            {
                Text = "δ֪����";
            }

            // ��ȡӦ��ͼ��
            _iconPath = System.Configuration.ConfigurationManager.AppSettings["AppIcon"];
            _iconPath = Application.StartupPath + _iconPath;
            if ((null != _iconPath) && (_iconPath.Length >= 0))
            {
                Icon = File.Exists(_iconPath) ? new Icon(_iconPath) : null;
            }

            // ��ȡӦ�ö˿�
            var recPortStr = System.Configuration.ConfigurationManager.AppSettings["Port"];
            try
            {
                RecPort = int.Parse(recPortStr);
            }
            catch(Exception ex)
            {
                MessageBox.Show("�˿ںŴ���" + recPortStr);
                Log.Warn(ex.Message);
            }

            //��������״̬
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
        #region �һ��˵�
        private void ���ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
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

        private void ȫѡToolStripMenuItem_Click(object sender, EventArgs e)
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
        
        #region ����ֹͣ����

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
        /// ��������
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
                this.SetLogText(string.Format("�����ɹ�,�˿ں�:{0}", numericUpDown_port.Value));
                Log.Info(string.Format("�����ɹ�,�˿ں�:{0}", numericUpDown_port.Value));
                toolStripButton_start.Enabled = false;
                toolStripButton_stop.Enabled = true;
                ��������ToolStripMenuItem.Enabled = false;
                ֹͣ����ToolStripMenuItem.Enabled = true;
                toolStripStatus_status.Text = "����״̬:����";
                Log.Info("����״̬:����");
                toolStripStatus_status.Image = Resources.Green;
                toolStripStatus_port.Text = "�˿ں�:" + numericUpDown_port.Value;
            }
            catch (Exception ex)
            {
                this.SetLogText("����ʧ��:" + ex.Message);
                Log.Error(ex.Message);
            }
        }
        
        /// <summary>
        /// ��ֹ����
        /// </summary>
        private void StopService()
        {
            try
            {
                CollectThread.DtuOnOffLineLogEventArgs -= CollectThread_DtuOnOffLineLog;
                CollectThread.DataHexShowStringLogEventArgs -= CollectThread_ReceiveLog;
                _work.StopService();
                SetLogText("ֹͣ�ɹ�");
                toolStripButton_start.Enabled = true;
                toolStripButton_stop.Enabled = false;
                ��������ToolStripMenuItem.Enabled = true;
                ֹͣ����ToolStripMenuItem.Enabled = false;
                toolStripStatus_status.Text = "����״̬:ֹͣ";
                toolStripStatus_status.Image = Resources.Red;
               // _timerCount.Stop();
                Log.Info("ֹͣ�ɹ�");
            }
            catch (Exception ee)
            {
                SetLogText("ֹͣʧ��:" + ee.Message);
                Log.Error(ee.Message);
            }
        }
        #endregion

        #region ��־

        void CollectThread_ReceiveLog(ReceiveDataInfo e)
        {
            var data = CVT.ByteToHexStr(e.PackagesBytes);
            SetRecText(string.Format("�յ�����,ID:{0},����:{1},����:{2}", e.Sender, e.PackagesBytes.Length, data));
            SetLogText(string.Format("�յ�����,ID:{0},����:{1}", e.Sender, e.PackagesBytes.Length));
        }

        /// <summary>
        /// ϵͳ��־
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
        /// ��������
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
        
        #region DTU�������¼�
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
                SetLogText(string.Format("{0}����", e.DtuId));
                Log.Info(string.Format("{0}����", e.DtuId));
            }
        }

        private void svr_ClientConnect(DTUConnectionEventArgs e)
        {
            if (e == null) return;
            var dtu = e;
            if (DtuList.ContainsKey(dtu.DtuId)) //����б����Ѻ��и�ID�ŵ�DTU
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
            SetLogText(string.Format("{0}����", dtu.DtuId));
            Log.Info(string.Format("{0}����", dtu.DtuId));
        }
        #endregion DTU������
        

        /// <summary>
        /// ��½��Ϣ
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
        
        #region �ؼ��¼�

        private void SetTimeText(int day, int hour, int min, int sen)
        {
           if (this.InvokeRequired)
           {
               this.BeginInvoke(new SetTextDuringTime(SetTimeText), new object[] { day, hour, min, sen });
           }
           else
               toolStripStatus_durtime.Text = string.Format("������:{0}��{1}ʱ{2}��{3}��", day, hour
                                                              , min, sen);
        }
        /// <summary>
        /// ��¼ϵͳ����ʱ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_durtime_Tick(object sender, EventArgs e)
        {
            var ts = DateTime.Now - _startTime;
            SetTimeText(ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
        }


        /// <summary>
        /// ���������ն���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_count_Tick(object sender, EventArgs e)
        {

        }
        
        private void �Ͽ�ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (listView1.Items.Count == 0 || listView1.SelectedItems.Count == 0)
            {
                return;
            }
        }


        #endregion �ؼ��¼�

        private void toolStripBtnOpenAutoRun_Click(object sender, EventArgs e)
        {
            try
            {

                if (toolStripBtnOpenAutoRun.Text == "��������(��)")
                {
                    HelperTools.StartupAutoRun(false);
                    toolStripBtnOpenAutoRun.Text = "��������(x)";
                    toolStripBtnOpenAutoRun.Image = Resources.Red;
                    HelperTools.UpdateAppConfig("OpenRun", Boolean.FalseString);
                    SetLogText("�رտ�������");

                }
                else if (toolStripBtnOpenAutoRun.Text == "��������(x)")
                {
                    HelperTools.StartupAutoRun(true);
                    HelperTools.UpdateAppConfig("OpenRun", Boolean.TrueString);
                    toolStripBtnOpenAutoRun.Text = "��������(��)";
                    toolStripBtnOpenAutoRun.Image = Resources.Green;
                    SetLogText("���ÿ�������");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                SetLogText("����ʧ��");
            }
        }


        private void IniSetOpenRun()
        {
            if (HelperTools.GetConfigString("OpenRun") == Boolean.FalseString)
            {
                toolStripBtnOpenAutoRun.Text = "��������(x)";
                toolStripBtnOpenAutoRun.Image = Resources.Red;
            }
            else if (HelperTools.GetConfigString("OpenRun") == Boolean.TrueString)
            {
                toolStripBtnOpenAutoRun.Text = "��������(��)";
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