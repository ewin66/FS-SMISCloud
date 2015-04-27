using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms;
using FSDE.BLL;
using FSDE.BLL.Config;
using FSDE.Dictionaries.config;
using FSDE.Forms.Views;
using FSDE.Model;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.Forms
{
    public partial class FrmUnion : Form
    {
        public static bool IsFilData;//判断是否要进行数据帅选
        public static bool IsDivGroup;//判断是否要进行传感器分组
        public static string DBPath = null;
        public static string DBConfigPath = null;

        public static string DBLocation = null;

        public static string pathOfTextBox = null;

        public delegate void CloseF();

        public CloseF closeF;

        public FrmUnion()
        {
            InitializeComponent();
            pathTextBox.Text = pathOfTextBox;
        }

        //对统一采集软件配置界面做限制
        public void DisableTab1()
        {
            choosePathButton.Enabled = false;
            testConnectButton.Enabled = false;
            divGroupCheckBox.Enabled = false;
            filDataCheckBox.Enabled = false;
            nextButton.Enabled = false;
            pathTextBox.Enabled = false;
        }

        public void EnableTab1()
        {
            choosePathButton.Enabled = true;
            testConnectButton.Enabled = true;
            divGroupCheckBox.Enabled = true;
            filDataCheckBox.Enabled = true;
            nextButton.Enabled = true;
            pathTextBox.Enabled = true;
        }

        public void DisableTab2()
        {
            safeTypeComboBox.Enabled = false;
            sensorListBox.Enabled = false;
            addButton.Enabled = false;
            removeButton.Enabled = false;
            filOrChoGroupComboBox.Enabled = false;
            choSensorListBox.Enabled = false;
            addGroupButton.Enabled = false;
            btnRemoveGroup.Enabled = false;
            divCalCheckBox.Enabled = false;
            btnNext.Enabled = false;
        }

        public void EnableTab2()
        {
            safeTypeComboBox.Enabled = true;
            sensorListBox.Enabled = true;
            addButton.Enabled = true;
            removeButton.Enabled = true;
            filOrChoGroupComboBox.Enabled = true;
            choSensorListBox.Enabled = true;
            addGroupButton.Enabled = true;
            btnRemoveGroup.Enabled = true;
            divCalCheckBox.Enabled = true;
            btnNext.Enabled = true;
        }

        public void DisableTab3()
        {
            cmbSafeFactor.Enabled = false;
            filDateComboBox.Enabled = false;
            finishButton.Enabled = false;
        }

        public void EnableTab3()
        {
            cmbSafeFactor.Enabled = true;
            filDateComboBox.Enabled = true;
            finishButton.Enabled = true;
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
            GetSensorInfo();
            if (nextButton.Text == "下一步" && divGroupCheckBox.Checked)
            {
                tabControl1.SelectTab(1);
                //初始化Tab2的控件
                if (IsFilData)
                {
                    this.btnNext.Text = "下一步";
                }
                else
                {
                    this.btnNext.Text = "完成";
                }
                InitlizeTypeComBox();
                InitlizeFilOrChoGroupComboBox();
                IniteLizeSensorListBox();
                IniteLizeChoSensorListBox();
                DisableTab1();
                EnableTab2();
            }
            if (nextButton.Text == "下一步" && (!divGroupCheckBox.Checked) && filDataCheckBox.Checked)
            {
                tabControl1.SelectTab(2);
                InitlizeSafeType();
                InitelizeFilDateComboBox();
                IniteLizeSensorListBox();
                IniteLizeChoSensorListBox();
                finishButton.Text = "完成";
                DisableTab1();
                EnableTab3();
            }
            if (nextButton.Text == "完成")
            {
                DialogResult result = MessageBox.Show(@"是否继续配置", @"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    tabControl1.SelectTab(0);
                    EnableTab1();
                    nextButton.Enabled = false;
                }
                else
                {
                    closeF();
                    this.Close();
                }
            }
        }

        public void IniteLizeSensorListBox()
        {
            sensorList.Clear();
            sensorSource.DataSource = sensorList;
            sensorListBox.DataSource = sensorSource;
            sensorSource.ResetBindings(true);
        }

        public void IniteLizeChoSensorListBox()
        {
            choSensorList.Clear();
            choSensoSource.DataSource = choSensorList;
            choSensorListBox.DataSource = choSensoSource;
            sensorSource.ResetBindings(true);
        }

        public List<MoChaSenId> sensorInfoList = new List<MoChaSenId>();

        public void GetSensorInfo()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection(DBConfigPath))
                {
                    conn.Open();
                    SQLiteCommand cmd = new SQLiteCommand(DBConfigPath, conn);
                    cmd.CommandText = "select S_DAI_SET.MODULE_NO as M_NO,S_SENSOR_SET.CHANNEL_ID as CH_ID " +
                                      ",S_SENSOR_SET.SENSOR_SET_ID as S_ID " +
                                      "from S_DAI_SET  left join S_SENSOR_SET  " +
                                      "on S_DAI_SET.DAI_SET_ID = S_SENSOR_SET.DAI_SET_ID ";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            sensorList.Add("模块号:" + reader["M_NO"] + " 通道号:" + reader["CH_ID"]);
                            MoChaSenId moChaSenId = new MoChaSenId()
                            {
                                MoudleId = reader["M_NO"].ToString(),
                                ChannelId = reader["CH_ID"].ToString(),
                                SensorId = reader["S_ID"].ToString()
                            };
                            sensorInfoList.Add(moChaSenId);
                        }
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(@"传感器表读取异常");
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

        private void divGroupCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (filDataCheckBox.Checked | divGroupCheckBox.Checked)
            {
                nextButton.Text = "下一步";
                if (filDataCheckBox.Checked)
                {
                    IsFilData = true;
                }
                else
                {
                    IsFilData = false;
                }
                if (divGroupCheckBox.Checked)
                {
                    IsDivGroup = true;
                }

                else
                {
                    IsDivGroup = false;
                }
            }
            else
            {
                nextButton.Text = "完成";
                IsFilData = false;
                IsDivGroup = false;
            }
        }

        private void filDataCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (filDataCheckBox.Checked | divGroupCheckBox.Checked)
            {
                nextButton.Text = "下一步";
                if (filDataCheckBox.Checked)
                {
                    IsFilData = true;
                }
                else
                {
                    IsFilData = false;
                }
                if (divGroupCheckBox.Checked)
                {
                    IsDivGroup = true;
                }

                else
                {
                    IsDivGroup = false;
                }
            }
            else
            {
                nextButton.Text = "完成";
                IsFilData = false;
                IsDivGroup = false;
            }
        }


        #region tab2 部分代码
        public static List<IDandName> filOrChoComBox = new List<IDandName>(); //为filOrChoGroupComboBox提供绑定源
        public static List<IDandName> IdName = new List<IDandName>();//用于存放监测因素ID
        public static Dictionary<int, GroupInfo> groupInfoDic = new Dictionary<int, GroupInfo>();
        public static Dictionary<int, GroupSensors> groupSensorsesDic = new Dictionary<int, GroupSensors>();
        public static List<MoChaSenId> moChaSenIdsList = new List<MoChaSenId>();
        public List<string> itemsList = new List<string>();//用于存放sersorListBox中选中的item


        public List<string> safeTypeList = new List<string>();
        public BindingSource safeTypeSource = new BindingSource();

        public List<string> filOrChoList = new List<string>();
        public BindingSource filOrChoSorSource = new BindingSource();

        public List<string> sensorList = new List<string>();
        public BindingSource sensorSource = new BindingSource();

        public List<string> choSensorList = new List<string>();
        public BindingSource choSensoSource = new BindingSource();

        public static bool EnableAddButton = false;

        //对safeTypeComboBox数据显示进行初始化
        public void InitlizeTypeComBox()
        {
            //IdName.Clear();
            safeTypeList.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(DBConfigPath))
            {
                try
                {
                    conn.Open();
                    SQLiteCommand cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT DISTINCT SafeTypeID FROM S_SENSOR_SET";
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);

                            if (TypeIdName.TypeAndName.ContainsKey(id))
                            {
                                IDandName v = new IDandName();
                                v.ID = id;
                                v.Name = TypeIdName.TypeAndName[id];
                                IdName.Add(v);
                                safeTypeList.Add(v.Name);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(@"数据库连接失败");
                }
                
            }
            safeTypeSource.DataSource = safeTypeList;
            safeTypeComboBox.DataSource = safeTypeSource;
            safeTypeSource.ResetBindings(true);
            safeTypeComboBox.SelectedIndex = -1;
        }

        public void InitlizeFilOrChoGroupComboBox()
        {
            filOrChoList.Clear();
            using (DbConnection conn = new DbConnection())
            {
                List<GroupInfo> list = GroupInfosDic.GetGroupInfoDic().GetAllGroups();
                for (int i = 0; i < list.Count; i++)
                {
                    filOrChoList.Add(list[i].GroupName);
                }
            }
            filOrChoSorSource.DataSource = filOrChoList;
            filOrChoGroupComboBox.DataSource = filOrChoSorSource;
            filOrChoSorSource.ResetBindings(true);
            filOrChoGroupComboBox.SelectedIndex = -1;
        }

        public void InitlizeDic()
        {
            groupInfoDic = GroupInfosDic.GetGroupInfoDic().GetDic();
            groupSensorsesDic = GroupSensorInfoDic.GetGroupInfosDic().GetDic();
        }

        private void sensorListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection sic = sensorListBox.SelectedIndices;

            if (sic.Count == 0)
            {
                addButton.Enabled = false;
                return;
            }
            else
            {
                string orgin = "填写或选择分组";
                EnableAddButton = true;
                if (filOrChoGroupComboBox.Text != orgin
                    && filOrChoGroupComboBox.Text != ""
                    && EnableAddButton
                    && sensorListBox.Items.Count > 0)
                    addButton.Enabled = true;
                itemsList.Clear();
                Collection<string> TempList = new Collection<string>();
                TempList.Clear();
                for (int i = 0; i < sensorListBox.SelectedItems.Count; i++)
                {
                    TempList.Add(sensorListBox.SelectedItems[i].ToString());
                }
                itemsList.AddRange(TempList);
            }
        }

        private void filOrChoGroupComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            choSensorList.Clear();
            InitlizeDic();
            string groupName = filOrChoGroupComboBox.Text;
            int sensorId = 0;
            foreach (KeyValuePair<int, GroupInfo> pair in groupInfoDic)
            {
                if (pair.Value.GroupName == groupName)
                {
                    foreach (KeyValuePair<int, GroupSensors> valuePair in groupSensorsesDic)
                    {
                        if (pair.Value.GroupID == valuePair.Value.GroupID)
                        {
                            sensorId = valuePair.Value.SensorID;
                            for (int i = 0; i < sensorInfoList.Count; i++)
                            {
                                if (sensorId.ToString() == sensorInfoList[i].SensorId)
                                {
                                    choSensorList.Add("模块号:" + sensorInfoList[i].MoudleId
                                                               + " 通道号:" + sensorInfoList[i].ChannelId);
                                }
                            }
                        }
                    }
                }
            }
            choSensoSource.ResetBindings(true);

        }

        private void filOrChoGroupComboBox_TextChanged(object sender, EventArgs e)
        {
            if (filOrChoGroupComboBox.Text != "" && filOrChoGroupComboBox.Text != "\r" && choSensorListBox.Items.Count > 0)
            {
                addGroupButton.Enabled = true;
                if (EnableAddButton && sensorListBox.Items.Count != 0)
                    addButton.Enabled = true;

                //查找组名是否存在
                int groupId = 0;
                List<GroupInfo> groupList = GroupInfosDic.GetGroupInfosDic().GetAllGroups();
                //根据组的名字找到组的ID
                for (int i = 0; i < groupList.Count; i++)
                {
                    if (filOrChoGroupComboBox.Text == groupList[i].GroupName)
                    {
                        groupId = Convert.ToInt32(groupList[i].GroupID);
                        break;
                    }
                }

                List<GroupSensors> groupSensorList = GroupSensorInfoDic.GetGroupInfosDic().GetDic().Values.ToList();
                //根据组的ID，找到组中的传感器的Id
                int sensorId = 0;
                choSensorList.Clear();
                for (int i = 0; i < groupSensorList.Count; i++)
                {
                    if (groupId == groupSensorList[i].GroupID)
                    {
                        sensorId = groupSensorList[i].SensorID;
                        for (int j=0; j<sensorInfoList.Count;j++)
                        {
                            if (sensorInfoList[j].SensorId == sensorId.ToString())
                            {
                                choSensorList.Add("模块号:" + sensorInfoList[j].MoudleId + " 通道号:" + sensorInfoList[j].ChannelId);
                                break;
                            }
                        }
                    }
                }
                
                choSensoSource.ResetBindings(true);
            }
            else
            {
                addGroupButton.Enabled = false;
            }


            //当Text发生变化时，可以根据用户的输入产看组名是否存在，并显示组中传感器
            choSensorList.Clear();
            InitlizeDic();
            string groupName = filOrChoGroupComboBox.Text;
            int sensorId1 = 0;
            foreach (KeyValuePair<int, GroupInfo> pair in groupInfoDic)
            {
                if (pair.Value.GroupName == groupName)
                {
                    foreach (KeyValuePair<int, GroupSensors> valuePair in groupSensorsesDic)
                    {
                        if (pair.Value.GroupID == valuePair.Value.GroupID)
                        {
                            sensorId1 = valuePair.Value.SensorID;
                            for (int i = 0; i < sensorInfoList.Count; i++)
                            {
                                if (sensorId1.ToString() == sensorInfoList[i].SensorId)
                                {
                                    choSensorList.Add("模块号:" + sensorInfoList[i].MoudleId
                                                               + " 通道号:" + sensorInfoList[i].ChannelId);
                                }
                            }
                        }
                    }
                }
            }
            choSensoSource.ResetBindings(true);
        }

        private void choSensorListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection sic = choSensorListBox.SelectedIndices;
            if (sic.Count == 0)
            {
                removeButton.Enabled = false;
                return;
            }
            else
            {
                removeButton.Enabled = true;
                itemsList.Clear();
                Collection<string> TempList = new Collection<string>();
                TempList.Clear();
                for (int i = 0; i < choSensorListBox.SelectedItems.Count; i++)
                {
                    TempList.Add(choSensorListBox.SelectedItems[i].ToString());
                }
                itemsList.AddRange(TempList);
            }
        }

        private void addGroupButton_Click(object sender, EventArgs e)
        {
            string orgin = "填写或选择分组";
            bool divCal = false;
            int DBNameID = 0;
            if (filOrChoGroupComboBox.Text != orgin)
            {
                if (!filOrChoGroupComboBox.Items.Contains(filOrChoGroupComboBox.Text)
                    && filOrChoGroupComboBox.Text != "")
                {
                    using (DbConnection conn = new DbConnection())
                    {
                        DataBaseNameDic.GetDataBaseNameDic();
                        List<DataBaseName> dataBaseNameList = DataBaseNameDic.GetDataBaseNameDic().GetAllBaseNames();

                        for (int i = 0; i < dataBaseNameList.Count; i++)
                        {
                            if (DBLocation == dataBaseNameList[i].Location)
                            {
                                DBNameID = Convert.ToInt32(dataBaseNameList[i].ID);
                            }
                        }

                        int safetypeid = -1;

                        foreach (IDandName idandName in IdName)
                        {
                            if (idandName.Name == safeTypeComboBox.Text)
                            {
                                safetypeid = idandName.ID;
                            }
                        }
                        if (divCalCheckBox.Checked)
                        {
                            divCal = true;
                        }

                        GroupInfo group = new GroupInfo()
                        {
                            GroupName = filOrChoGroupComboBox.Text,
                            DataBaseID = DBNameID,
                            Safetyfactortypeid = safetypeid,
                            CombinationCalculate = divCal
                        };
                        int groupId = GroupInfosDic.GetGroupInfoDic().CheckAdd(group);
                        if (AddSensorToGroup(groupId))
                        {
                            MessageBox.Show("添加成功");
                        }
                    }
                }
            }
            InitlizeFilOrChoGroupComboBox();
        }

        //将choSensorListBox内的传感器添加到分组的传感器表中
        public bool AddSensorToGroup(int groupId)
        {
            bool ret = false;
            string[] sensorGroup = new string[choSensorListBox.Items.Count];//通道号在sensorGroup[1],模块号在sensorGroup[3]

            for (int i = 0; i < choSensorListBox.Items.Count; i++)
            {
                sensorGroup[i] = choSensorListBox.Items[i].ToString();
            }
            for (int i = 0; i < sensorGroup.Count(); i++)
            {
                string[] sensorInfo = sensorGroup[i].Split(new char[2] { ':', ' ' });
                string channelId = sensorInfo[3];
                string moudleId = sensorInfo[1];
                
                foreach (MoChaSenId v in moChaSenIdsList)
                {
                    if (v.ChannelId == channelId && v.MoudleId == moudleId)
                    {
                        GroupSensors groupSensor = new GroupSensors()
                        {
                            GroupID = groupId,
                            SensorID = Convert.ToInt32(v.SensorId),
                        };
                        ret = GroupSensorInfoDic.GetGroupInfosDic().Add(groupSensor);
                    }
                }
            }
            return ret;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (btnNext.Text == @"完成")
            {
                DialogResult result = MessageBox.Show(@"是否继续配置", @"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    tabControl1.SelectTab(0);
                    DisableTab2();
                    EnableTab1();
                    nextButton.Enabled = false;
                }
                else
                {
                    DisableTab2();
                    closeF();
                    this.Close();
                }
            }
            else
            {
                tabControl1.SelectTab(2);
                InitlizeSafeType();
                InitelizeFilDateComboBox();
                DisableTab2();
                EnableTab3();

                finishButton.Text = "完成";
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < sensorListBox.Items.Count; i++)
            {
                string stemp = sensorListBox.SelectedItems[i].ToString();
                sensorList.Remove(sensorListBox.SelectedItems[i].ToString());
                sensorSource.ResetBindings(true);
                if (!choSensorList.Contains(stemp))
                {
                    choSensorList.Add(stemp);
                }
                choSensoSource.ResetBindings(true);
            }
            if (choSensorList.Count > 0
                && filOrChoGroupComboBox.Text != ""
                && filOrChoGroupComboBox.Text != "\r"
                && filOrChoGroupComboBox.Text != "填写或选择分组")
            {
                addGroupButton.Enabled = true;
            }
            else
            {
                addGroupButton.Enabled = false;
            }
        }


        private void removeButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < choSensorListBox.Items.Count; i++)
            {
                string stemp = choSensorListBox.SelectedItems[i].ToString();
                if (!sensorList.Contains(stemp))
                {
                    sensorList.Add(stemp);
                }
                sensorSource.ResetBindings(true);
                choSensorList.Remove(stemp);
                choSensoSource.ResetBindings(true);
            }
            if (choSensorList.Count > 0
                && filOrChoGroupComboBox.Text != ""
                && filOrChoGroupComboBox.Text != "\r"
                && filOrChoGroupComboBox.Text != "填写或选择分组")
            {
                addGroupButton.Enabled = true;
            }
            else
            {
                addGroupButton.Enabled = false;
            }

        }

        private void safeTypeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string safeTyperName = safeTypeComboBox.Items[safeTypeComboBox.SelectedIndex].ToString();
            try
            {
                foreach (var idname in IdName)
                {
                    if (safeTyperName == idname.Name)
                    {
                        sensorList.Clear();
                        using (SQLiteConnection conn = new SQLiteConnection(DBConfigPath))
                        {
                            conn.Open();
                            SQLiteCommand cmd = new SQLiteCommand(DBConfigPath, conn);
                            cmd.CommandText = "select S_DAI_SET.MODULE_NO as M_NO,S_SENSOR_SET.CHANNEL_ID as CH_ID " +
                                              ",S_SENSOR_SET.SENSOR_SET_ID as S_ID " +
                                              "from S_DAI_SET  left join S_SENSOR_SET  " +
                                              "on S_DAI_SET.DAI_SET_ID = S_SENSOR_SET.DAI_SET_ID " +
                                              "where S_SENSOR_SET.SafeTypeID=" + idname.ID.ToString();
                            SQLiteDataReader reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                //sensorListBox.Items.Clear();
                                moChaSenIdsList.Clear();
                                while (reader.Read())
                                {
                                    sensorList.Add("模块号:" + reader["M_NO"] + " 通道号:" + reader["CH_ID"]);
                                    MoChaSenId moChaSenId = new MoChaSenId()
                                    {
                                        MoudleId = reader["M_NO"].ToString(),
                                        ChannelId = reader["CH_ID"].ToString(),
                                        SensorId = reader["S_ID"].ToString()
                                    };
                                    if (!moChaSenIdsList.Contains(moChaSenId))
                                        moChaSenIdsList.Add(moChaSenId);
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(@"传感器表读取异常");
            }

            sensorSource.DataSource = sensorList;
            sensorListBox.DataSource = sensorSource;
            sensorSource.ResetBindings(true);
        }

        #endregion

        #region tab3部分代码

        public static List<IDandName> filterComBoxList = new List<IDandName>();//为filDateComboBox提供绑定源
        public static List<DataFilterType> filterList = new List<DataFilterType>();

        private void InitlizeSafeType()
        {
            IdName.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(DBConfigPath))
            {
                conn.Open();
                SQLiteCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT DISTINCT SafeTypeID FROM S_SENSOR_SET";
                SQLiteDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0);

                        if (TypeIdName.TypeAndName.ContainsKey(id))
                        {
                            IDandName v = new IDandName();
                            v.ID = id;
                            v.Name = TypeIdName.TypeAndName[id];
                            IdName.Add(v);
                        }
                    }
                }
            }
        }

        public void InitelizeFilDateComboBox()
        {
            filterComBoxList.Clear();
            var bll = new DataFilterTypeBll();
            filterList = bll.SelectList().ToList();
            IDandName temp = new IDandName();
            for (int i = 0; i < filterList.Count; i++)
            {
                temp.ID = Convert.ToInt32(filterList[i].ID);
                temp.Name = filterList[i].FilterDesc;
                filterComBoxList.Add(temp);
            }
            filDateComboBox.Text = "";
        }


        #endregion

        private void cmbSafeFactor_DropDown(object sender, EventArgs e)
        {
            cmbSafeFactor.DataSource = IdName;
            cmbSafeFactor.ValueMember = "ID";
            cmbSafeFactor.DisplayMember = "Name";
        }

        private void filDateComboBox_DropDown(object sender, EventArgs e)
        {
            filDateComboBox.DataSource = filterComBoxList;
            filDateComboBox.ValueMember = "ID";
            filDateComboBox.DisplayMember = "Name";
        }

        private void finishButton_Click(object sender, EventArgs e)
        {
            //保存安全监测监测因素和数据过滤方式到DataFilter表
            string filDec = filDateComboBox.Text;
            string safeType = cmbSafeFactor.Text;
            int safeId = 0;
            int dataFilterTypeId = 0;
            int dataBaseId = 0;
            for (int i = 0; i < filterList.Count; i++)
            {
                if (filDec == filterList[i].FilterDesc)
                {
                    dataFilterTypeId = Convert.ToInt32(filterList[i].ID);
                    break;
                }
            }
            foreach (IDandName idname in IdName)
            {
                if (idname.Name == safeType)
                {
                    safeId = idname.ID;
                    break;
                }
            }

            List<DataBaseName> list = DataBaseNameDic.GetDataBaseNameDic().GetAllBaseNames();
            foreach (DataBaseName name in list)
            {
                if (DBLocation == name.Location)
                {
                    dataBaseId = Convert.ToInt32(name.ID);
                    break;
                }
            }

            using (DbConnection conn = new DbConnection())
            {
                DataFilter v = new DataFilter()
                {
                    SafetyFactorType = safeId,
                    FilterType = dataFilterTypeId,
                    DataBaseId = dataBaseId
                };
                if (v.FilterType != 0 && v.SafetyFactorType != 0)
                {
                    int[] id = DataFilterDic.GetDataFilterDic().GetSafetyfactortypes();
                    if (!id.Contains(v.SafetyFactorType))
                    {
                        if (DataFilterDic.GetDataFilterDic().Add(v))
                        {
                            MessageBox.Show(@"添加完成");
                        }
                    }
                    else
                    {
                        MessageBox.Show(@"已存在");
                    }
                }
            }

            DialogResult result = MessageBox.Show(@"是否继续配置", @"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                tabControl1.SelectTab(0);
                DisableTab3();
                EnableTab1();
                nextButton.Enabled = false;
            }
            else
            {
                DisableTab3();
                closeF();
                this.Close();
            }
            
        }

        private void divCalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (divCalCheckBox.Checked)
            {
                using (DbConnection conn = new DbConnection())
                {
                    List<GroupInfo> groupList = GroupInfosDic.GetGroupInfosDic().GetDic().Values.ToList();
                    for (int i = 0; i < groupList.Count; i++)
                    {
                        if (filOrChoGroupComboBox.Text == groupList[i].GroupName
                            && groupList[i].CombinationCalculate == false)
                        {
                            GroupInfo group = groupList[i];
                            group.CombinationCalculate = true;
                            GroupInfosDic.GetGroupInfosDic().UpdateSensorGroupInfo(group);
                        }
                    }
                }
            }
            else
            {
                using (DbConnection conn = new DbConnection())
                {
                    List<GroupInfo> groupList = GroupInfosDic.GetGroupInfosDic().GetDic().Values.ToList();
                    for (int i = 0; i < groupList.Count; i++)
                    {
                        if (filOrChoGroupComboBox.Text == groupList[i].GroupName
                            && groupList[i].CombinationCalculate == true)
                        {
                            GroupInfo group = groupList[i];
                            group.CombinationCalculate = false;
                            GroupInfosDic.GetGroupInfosDic().UpdateSensorGroupInfo(group);
                        }
                    }
                }
            }
        }

        private void btnRemoveGroup_Click(object sender, EventArgs e)
        {
            if (filOrChoGroupComboBox.Text != "")
            {
                int groupId = 0;
                using (DbConnection conn = new DbConnection())
                {
                    List<GroupInfo> groupList = GroupInfosDic.GetGroupInfosDic().GetDic().Values.ToList();
                    for (int i = 0; i < groupList.Count; i++)
                    {
                        if (filOrChoGroupComboBox.Text == groupList[i].GroupName)
                        {
                            groupId = Convert.ToInt32(groupList[i].GroupID);
                            if (GroupInfosDic.GetGroupInfoDic().Delete(Convert.ToInt32(groupList[i].GroupID)))
                            {
                                MessageBox.Show(@"组删除成功");
                            }
                            break;
                        }
                        if (i == groupList.Count)
                        {
                            MessageBox.Show(@"组不存在");
                        }
                    }
                    if (groupList.Count == 0)
                    {
                        MessageBox.Show(@"还未添加组");
                    }
                }
                //组删除的同时删除掉组中的传感器

                List<GroupSensors> sensorsList = GroupSensorInfoDic.GetGroupInfosDic().GetDic().Values.ToList();
                bool ret = false;
                foreach (GroupSensors sensors in sensorsList)
                {
                    if (sensors.GroupID == groupId)
                    {
                        ret = GroupSensorInfoDic.GetGroupInfosDic().Delete(Convert.ToInt32(sensors.ID));
                    }
                }
                if (ret)
                {
                    MessageBox.Show(@"传感器删除成功");
                }
            }
            else
            {
                MessageBox.Show(@"请选择组名");
            }
            InitlizeFilOrChoGroupComboBox();
            
        }

        private void FrmUnion_Load(object sender, EventArgs e)
        {
            DisableTab2();
            DisableTab3();
        }
    }
}
