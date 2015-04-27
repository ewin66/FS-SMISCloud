using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using FSDE.Dictionaries.config;
using FSDE.Model;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.Forms
{
    public partial class FrmTextConfig : Form
    {
        public delegate void CacelText();

        public event CacelText cacelText;

        public delegate void CloseF();

        public CloseF closeFather;

        public int DbType = -1;

        public FrmTextConfig()
        {
            InitializeComponent();
        }

        private void FrmTextConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            cacelText();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            
            if (rdoShake.Checked)
            {
                DbType = (int)DataBaseType.Shake;
            }
            if (rdoLight.Checked)
            {
                DbType = (int) DataBaseType.Fiber;
            }
            if (rbVir.Checked)
            {
                DbType = (int)DataBaseType.Vibration;
            }
            if (DbType == -1)
            {
                MessageBox.Show(@"请选择文本数据类型");
                return;
            }
            else
            {
                if (txtLocation.Text == "")
                {
                    MessageBox.Show(@"请填写文本数据路径");
                    return;
                }
                else
                {
                    if (AddDataBaseName("文本数据", txtLocation.Text, DbType))
                    {
                        DialogResult result = MessageBox.Show(@"保存成功,是否继续配置", @"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {

                        }
                        else
                        {
                            Close();
                            closeFather();
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"保存失败，该路径已使用");
                    }
                }
            }
        }

        public bool AddDataBaseName(string dbName, string path,int daType)
        {
            using (DbConnection conn = new DbConnection())
            {
                bool flag = false;
                DataBaseName dataBaseName = new DataBaseName()
                {
                    DataBaseCode = dbName,
                    Location = path,
                    DataBaseType = daType,
                };
                DataBaseNameDic.GetDataBaseNameDic();
                List<DataBaseName> dataBaseNameList = DataBaseNameDic.GetDataBaseNameDic().GetAllBaseNames();
                for (int i = 0; i < dataBaseNameList.Count; i++)
                {
                    if (dataBaseNameList[i].Location == path)
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

        private void btnScan_Click(object sender, EventArgs e)
        {
            var fld = new FolderBrowserDialog();
            fld.ShowDialog();
            string path = fld.SelectedPath;
            if (path == "")
            {
                txtLocation.Text = null;
            }
            else
            {
                txtLocation.Text = path;
            }
        }
    }
}
