using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FS.SMIS_Cloud.DAC.Tran;
using FS.SMIS_Cloud.DAC.Tran.Db;
using log4net;

namespace DAC.Sender
{
    public partial class MainForm : Form
    {
        private static ILog Log = LogManager.GetLogger("DS");
        public MainForm()
        {
            InitializeComponent();
            this.textMsg.Clear();
            Console.SetOut(new TextBoxWriter(this.textMsg));
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBoxCom.Items.AddRange(SerialPort.GetPortNames());  
            comboBoxCom.Text = "COM1";
            if (comboBoxCom.Items.Count == 1)
            {
                comboBoxCom.Text = comboBoxCom.Items[0].ToString();
            }
        }

        private ITranDataProvider _dataProvider;
        private ComDataSender _comSender;
        private TranDataSender _dataSender;
        private int _totalRemainder = 0;

        private void InitSender()
        {
           //  _dataProvider = new VibFileDataProvider();
            _dataProvider = new DbDacDataProvider();
           
            _comSender = new ComDataSender
            {
                DtuCode = Convert.ToInt32(this.textDtuCode.Text)
            };

            _dataSender = new TranDataSender(_comSender, _dataProvider);
            _dataSender.OnMessageSent = OnMsgSent;
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {
            if (this._dataSender != null)
            {
                this._dataSender.Stop();
                _dataSender.OnMessageSent = null;
            }
        }

        private void OnMsgSent(TranMsg req, TranMsg resp)
        {
            int remained = _dataSender.Remainder;
            if (_totalRemainder > 0)
                this.progressBar1.Value = (int) (remained/_totalRemainder*100.0);
            else
                this.progressBar1.Value = 0;
            _totalRemainder = remained;
            Log.InfoFormat("Msg sent: {0} bytes.", req.LoadSize);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (_dataProvider == null)
            {
                InitSender();
            }
            _comSender.DtuCode = Convert.ToInt32(this.textDtuCode.Text);
            Dictionary<string, string> args = new Dictionary<string, string>();
            args["PortName"] = this.comboBoxCom.Text;
            args["BaudRate"] = textBaudRate.Text;
            args["Parity"] = Convert.ToString((int)Parity.None);
            args["DataBits"] = "8";
            args["StopBits"] = Convert.ToString((int)StopBits.One);
            args["ReadTimeOut"] = "6";
            args["DataPath"] = this.textDataPath.Text;
            args.Add("dbcongxml", "./DbMapping.xml");
            Log.InfoFormat("Init sender:");
            foreach (string k in args.Keys)
            {
                Console.WriteLine("  {0} = {1}", k, args[k]);
            }

            _comSender.Init(args);
            _dataProvider.Init(args);

            this._totalRemainder  = _dataSender.Remainder;
            _dataSender.DoWork();
        }

        private class TextBoxWriter : TextWriter
        {
            readonly TextBox _txtBox;
            delegate void VoidAction();

            private int msg_count = 0;
            private int max_count = 1000;
            public TextBoxWriter(TextBox box)
            {
                _txtBox = box; //transfer the enternal TextBox in
            }

            private void CheckFull()
            {
                if (msg_count++ >= max_count)
                {
                    msg_count = 0;
                    _txtBox.Clear();
                }
            }
            public override void Write(char value)
            {
                CheckFull();
                VoidAction action = () => _txtBox.AppendText(value.ToString());
                _txtBox.BeginInvoke(action);
            }

            public override void Write(string str )
            {
                CheckFull();
                VoidAction action = () => _txtBox.AppendText(str);
                _txtBox.BeginInvoke(action);
            }

            public override void Write(string fmt, params object[] args)
            {
                CheckFull();
                VoidAction action = () => _txtBox.AppendText(string.Format(fmt,args));
                _txtBox.BeginInvoke(action);
            }

            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }


    }
}
