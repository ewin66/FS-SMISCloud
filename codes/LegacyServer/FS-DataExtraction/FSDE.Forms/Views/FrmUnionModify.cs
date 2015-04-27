using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FSDE.BLL;
using FSDE.Dictionaries.config;
using FSDE.Model;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.Forms
{
    public partial class FrmUnionModify : Form
    {
        public static string DBPath = null;
        public static string DBConfigPath = null;

        public static string DBLocation = null;

        public static string pathOfTextBox = null;

        public delegate void CloseF();

        public CloseF closeF;
        
        public FrmUnionModify()
        {
            InitializeComponent();
        }

        private void choosePathButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = "C://";
            fileDialog.Filter = "Db3 files (*.db3)|*.db3|All files (*.*)|*.*";
            fileDialog.FilterIndex = 1;
            fileDialog.RestoreDirectory = true;
            fileDialog.FileName = "";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                this.pathTextBox.Text = fileDialog.FileName;
                pathOfTextBox = fileDialog.FileName;
            }
        }

        private void testConnectButton_Click(object sender, EventArgs e)
        {
            if (pathTextBox.Text.ToString() == "")
            {
                MessageBox.Show("请选择数据库文件", "WARING");
                return;
            }
            else
            {
                string strStart = @"Data Source=";
                string strEnd = @";Version=3;Pooling=False;Max Pool Size=100";
                DBPath = strStart + pathTextBox.Text.ToString().Trim() + strEnd;
                DBConfigPath = DBPath.Replace("FSUSDataValueDB.db3", "FSUSConfigDB.db3");
                var bll = new ConnectTestBll();
                if (bll.IsConnect(DBPath))
                {
                    DBLocation = pathTextBox.Text;
                    nextButton.Enabled = true;
                    MessageBox.Show("连接成功");
                }
                else
                {
                    nextButton.Enabled = false;
                }
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            AddDataBaseName();

            DialogResult result = MessageBox.Show(@"是否继续配置", @"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                nextButton.Enabled = false;
            }
            else
            {
                closeF();
                this.Close();
            }

        }

        public bool AddDataBaseName()
        {
            DAL.Common.Initialise();
            using (DbConnection conn = new DbConnection())
            {
                bool flag = false;
                string[] names = pathTextBox.Text.Split("\\".ToCharArray());
                DataBaseName dataBaseName = new DataBaseName()
                {
                    DataBaseCode = names[names.Count() - 1],
                    Location = pathTextBox.Text,
                    DataBaseType = (int)DataBaseType.SQLite
                };
                List<DataBaseName> dataBaseNameList = DataBaseNameDic.GetDataBaseNameDic().GetAllBaseNames();
                for (int i = 0; i < dataBaseNameList.Count; i++)
                {
                    if (dataBaseNameList[i].Location == pathTextBox.Text)
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    DataBaseNameDic.GetDataBaseNameDic().Add(dataBaseName);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
