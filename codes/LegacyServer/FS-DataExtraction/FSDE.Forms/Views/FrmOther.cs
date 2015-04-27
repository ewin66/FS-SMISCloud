using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using FSDE.BLL;
using FSDE.BLL.Config;
using FSDE.Core;
using FSDE.Dictionaries;
using FSDE.Dictionaries.config;
using FSDE.Forms.DbOperation;
using FSDE.Model;
using FSDE.Model.Config;
//using FSUS.Common;
using log4net;
using SqliteORM;


namespace FSDE.Forms.Views
{
    using System;
    using System.Windows.Forms;

    public partial class FrmOther : Form
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ExtractionManager));
        //用于标记选择的数据库类型
        public static bool IsAccess = false;
        public static bool IsSQLite = false;
        public static bool IsSqlServer = false;

        //存放数据库的路径
        public static string SQLitePath = null;
        public string SQLitebak = null;
        public static string SQLServerPath = null;
        public static string AccessPath = null;

        public static string AccessConnStr;//Access数据库连接字符串
        public static string SqliteConnStr;

        public static List<string> tableNames = new List<string>();//存放选择数据库中表的名字

        public static List<string> fieldsList = new List<string>();//为tablesListBox提供绑定源
        public static IList<ProductCategory> choDataTypeList = new List<ProductCategory>(); //为choDataTypeComboBox提供绑定源

        public static List<ExtractValueName> NormaListBoxList = new List<ExtractValueName>(); //normalListBox中的所有数据


        public List<string> addListBoxDs = new List<string>();//存放addListBox中的Items
        public BindingSource bs = new BindingSource();//绑定到addListBox的数据源操作

        public List<string> normalListBoxDs = new List<string>(); //保存normalListBox中的Items

        public List<string> HadAddTableList = new List<string>();

        public static bool passTest = false;//测试连接标志

        public BindingSource bsTableNames = new BindingSource();//为choTableComboBox提供邦定源
        public BindingSource bsTableListBox = new BindingSource();//为tableListBox提供邦定源

        public List<string> inDataBaseNameList = new List<string>();//inDataBaseComboBox提供邦定源
        public List<DataBaseName> InDataBaseNameList = new List<DataBaseName>();
        public BindingSource inDataBaseNameSource = new BindingSource();

        public List<string> sensorTypeList = new List<string>();//为sensorTypeComboBox提供邦定源
        public List<FSDE.Model.Fixed.SensorType> SensorTypeList = new List<Model.Fixed.SensorType>();
        public BindingSource sensorTypeSource = new BindingSource();

        public List<string> safeFactorList = new List<string>();//为safeFactorComboBox提供邦定源
        public List<IDandName> SafetyfactortypeEnumsList = new List<IDandName>(); 
        public BindingSource safeFactorSource = new BindingSource();

        //cmbChoConTab改变时lstOrigin中字段数据源
        public List<string> choConTabList = new List<string>();

        public List<string> lstOrginList = new List<string>();

        public static string rememberDbpath = null;//记录之前配置数据库的路径

        List<string> cmbHadAddTableList = new List<string>();//为已添加的配置表提供数据源


        public delegate void CloseF();

        public CloseF closeFather;

        public FrmOther()
        {
            this.InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
            DisableTab1();
            EnableTab2();
            btnAddConfig.Enabled = false;
            //获取选定数据库中的表名
            if (IsAccess)
            {
                choConTabList.Clear();
                choConTabList = AccessOperation.GetAccessTableNames().ToList();
                cmbChoConTab.DataSource = choConTabList;
            }
            if (IsSQLite)
            {
                choConTabList.Clear();
                choConTabList = SQLiteOperatiopn.GetSQLiteTableNames().ToList();
                cmbChoConTab.DataSource = choConTabList;
            }
            if (IsSqlServer)
            {
                bsTableNames.DataSource = tableNames;
                cmbChoConTab.DataSource = bsTableNames;
                bsTableNames.ResetBindings(true);
            }

            //初始化已添加配置表的ComBox
            List<ConfigTable> listTable = ConfigTableDic.GetConfigTableDic().SelectList();
            cmbHadAddTableList.Clear();
            for (int i = 0; i < listTable.Count; i++)
            {
                cmbHadAddTableList.Add(listTable[i].TableName);
            }
            cmbHadAddConfig.DataSource = null;
            cmbHadAddConfig.DataSource = cmbHadAddTableList;
            cmbHadAddConfig.SelectedIndex = -1;

            cmbChoConTab.SelectedIndex = -1;//选择配置字段表默认不选任何项

            //控制检查是否设置配置表
            grpConfigTable.Enabled = false;
            chkHasConfigTable.Checked = false;
        }


        public void ChangeToTab4()
        {
            tabControl1.SelectTab(4);   
        }

        public void ChangeToTab0()
        {
            tabControl1.SelectTab(0);
        }

        #region 启用禁用tabpage的设置

        public void DisableTab1()
        {
            AccessRadioButton.Enabled = false;
            sqliteRadioButton.Enabled = false;
            sqlServerRadioButton.Enabled = false;
            dbNameTextBox.Enabled = false;
            userNameTextBox.Enabled = false;
            passWordTextBox.Enabled = false;
            scanButton.Enabled = false;
            testLineButton.Enabled = false;
            nextButton.Enabled = false;
            btnAdv.Enabled = false;
        }

        public void EnableTab1()
        {
            AccessRadioButton.Enabled = true;
            sqliteRadioButton.Enabled = true;
            sqlServerRadioButton.Enabled = true;
            dbNameTextBox.Enabled = true;
            userNameTextBox.Enabled = true;
            passWordTextBox.Enabled = true;
            scanButton.Enabled = true;
            testLineButton.Enabled = true;
            nextButton.Enabled = true;
            btnAdv.Enabled = true;
        }
        public void DiseableTab1Section()
        {
            dbNameTextBox.Enabled = false;
            userNameTextBox.Enabled = false;
            passWordTextBox.Enabled = false;
            scanButton.Enabled = false;
            testLineButton.Enabled = false;
        }

        public void EableTab1Section()
        {
            dbNameTextBox.Enabled = true;
            userNameTextBox.Enabled = true;
            passWordTextBox.Enabled = true;
            scanButton.Enabled = true;
            testLineButton.Enabled = true;
        }
        public void DisableTab2()
        {
            cmbDbName.Enabled = false;
            chkHasConfigTable.Enabled = false;
            grpConfigTable.Enabled = false;
            btnNext.Enabled = false;
            btnBack1.Enabled = false;
        }

        public void EnableTab2()
        {
            cmbDbName.Enabled = true;
            chkHasConfigTable.Enabled = true;
            grpConfigTable.Enabled = true;
            btnNext.Enabled = true;
            btnBack1.Enabled = true;
        }

        public void DisEnableTab3()
        {
            choTableComboBox.Enabled = false;
            choDataTypeComboBox.Enabled = false;
            hadAddTableComboBox.Enabled = false;
            nextButtonOther.Enabled = false;
            dbNameComboBox.Enabled = false;
            tablesListBox.Enabled = false;
            addListBox.Enabled = false;
            normalListBox.Enabled = false;
            addButton.Enabled = false;
            removeBtton.Enabled = false;
            removeAddTableButton.Enabled = false;
            addToTabButton.Enabled = false;
            btnBack2.Enabled = false;
        }

        public void EnableTab3()
        {
            choTableComboBox.Enabled = true;
            choDataTypeComboBox.Enabled = true;
            hadAddTableComboBox.Enabled = true;
            nextButtonOther.Enabled = true;
            dbNameComboBox.Enabled = true;
            tablesListBox.Enabled = true;
            addListBox.Enabled = true;
            normalListBox.Enabled = true;
            addButton.Enabled = true;
            removeBtton.Enabled = true;
            removeAddTableButton.Enabled = true;
            addToTabButton.Enabled = true;
            btnBack2.Enabled = true;
        }

        #endregion

        private void AccessRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (AccessRadioButton.Checked)
            {
                IsAccess = true;
                IsSQLite = false;
                IsSqlServer = false;
                dbLabel.Visible = true;
                serverLabel.Visible = false;
                EableTab1Section();
                nextButton.Enabled = false;
                grpUserPasswd.Visible = false;
            }
            else
            {
                IsAccess = false;
            }
        }

        private void sqliteRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sqliteRadioButton.Checked)
            {
                IsSQLite = true;
                IsAccess = false;
                IsSqlServer = false;
                dbLabel.Visible = true;
                serverLabel.Visible = false;
                EableTab1Section();
                nextButton.Enabled = false;
                grpUserPasswd.Visible = false;
            }
            else
            {
                IsSQLite = false;
            }
        }

        private void sqlServerRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (sqlServerRadioButton.Checked)
            {
                IsSqlServer = true;
                IsSQLite = false;
                IsAccess = false;
                serverLabel.Visible = true;
                dbLabel.Visible = false;
                EableTab1Section();
                scanButton.Enabled = false;
                nextButton.Enabled = false;
                grpUserPasswd.Visible = true;
            }
            else
            {
                IsSqlServer = false;
            }
        }
        

        public bool AddDataBaseName(string dbName,string path,int dataBaseType)
        {
            using (DbConnection conn = new DbConnection())
            {
                bool flag = false;
                DataBaseName dataBaseName = new DataBaseName()
                {
                    DataBaseCode = dbName,
                    Location = path,
                    DataBaseType = dataBaseType,
                    UserId = userNameTextBox.Text,
                    Password = passWordTextBox.Text,
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

        public void InitelizeSqliteCompents()
        {
            //初始化choTableComboBox
            tableNames.Clear();
            tableNames = SQLiteOperatiopn.GetSQLiteTableNames().ToList();
            choTableComboBox.DataSource = tableNames;
            if (choTableComboBox.Items.Count > 0)
            {
                choTableComboBox.SelectedIndex = -1;
            }
        }

        public void InitelizeAccessCompents()
        {
            tableNames.Clear();
            tableNames = AccessOperation.GetAccessTableNames().ToList();
            choTableComboBox.DataSource = tableNames;
            if (choTableComboBox.Items.Count > 0)
            {
                choTableComboBox.SelectedIndex = -1;
            }
        }

        public void InitelizeSqlServerCompents()
        {
            bsTableNames.DataSource = tableNames;
            choTableComboBox.DataSource = bsTableNames;
            bsTableNames.ResetBindings(true);
            //choTableComboBox.DataSource = tableNames;
        }

        //三种数据库都会用到的初始化方法
        public void CommInitelize()
        {
            //初始化choDataTypeComboBox
            choDataTypeList.Clear();
            choDataTypeList = AccessOperation.GetDataTypeList();//读取C_PRODUCT_CATEGORY表，得其中的数据
            string[] traList = new string[choDataTypeList.Count];
            for (int i = 0; i < choDataTypeList.Count; i++)
            {
                traList[i] = choDataTypeList[i].CatagoryName;
            }
            choDataTypeComboBox.DataSource = traList;//将C_PRODUCT_CATEGORY表的CatagoryName绑定到choDataTypeComboBox

            //第一次加载页面时的显示
            NormaListBoxList.Clear();
            NormaListBoxList = GetExtractValueNameList().ToList();
            int catagoryId = 0;
            foreach (ProductCategory v in choDataTypeList)
            {
                if (v.CatagoryName == choDataTypeComboBox.Text)
                {
                    catagoryId = Convert.ToInt32(v.CatagoryId);
                    break;
                }
            }
            List<string> list = new List<string>();

            if (chkHasConfigTable.Checked)
            {
                list.Add("传感器ID");
            }
            else
            {
                list.Add("模块号");
                list.Add("通道号");
            }
            
            list.Add("标记位");
            list.Add("时间");
            //string[] traList = new string[NormaListBoxList.Count];
            for (int i = 0; i < NormaListBoxList.Count; i++)
            {
                if (NormaListBoxList[i].CatagoryId == catagoryId)
                {
                    list.Add(NormaListBoxList[i].ValueName);
                    break;
                }

            }
            normalListBoxDs.Clear();
            normalListBoxDs.AddRange(list);
            normalListBox.DataSource = list;

            //为addListBox绑定数据
            bs.DataSource = addListBoxDs;
            addListBox.DataSource = bs;

            if (hadAddTableComboBox.Items.Count > 0)
            {
                hadAddTableComboBox.SelectedIndex = -1;
            }
            if (choDataTypeComboBox.Items.Count > 0)
            {
                choDataTypeComboBox.SelectedIndex = -1;
            }
        }

        private void choTableComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (IsAccess)
            {
                fieldsList.Clear();
                fieldsList = AccessOperation.GetTableFieldNameList(choTableComboBox.Text);
                fieldsList.Add("空");
                tablesListBox.DataSource = fieldsList;
            }
            if (IsSQLite)
            {
                fieldsList.Clear();
                fieldsList = SQLiteOperatiopn.GetSQLiteFieldNames(choTableComboBox.Text);
                fieldsList.Add("空");
                tablesListBox.DataSource = fieldsList;
            }

            if (IsSqlServer)
            {
                #region//绑定tablesListBox
                string str_sqlcon = null;
                string str_sqlTableName = null;
                string str_sqlTableCount = null;
                str_sqlcon = str_sqlcon + "Data Source = " + this.dbNameTextBox.Text + ";";
                //   str_sqlcon = str_sqlcon + "," + "1433" + ";"; //端口
                str_sqlcon = str_sqlcon + "Network Library = " + "DBMSSOCN" + ";";
                str_sqlcon = str_sqlcon + "Initial Catalog = " + ((DataRowView)this.dbNameComboBox.SelectedItem).Row.ItemArray[0].ToString() + ";"; //库名
                str_sqlcon = str_sqlcon + "User ID = " + this.userNameTextBox.Text + ";";
                str_sqlcon = str_sqlcon + "Password = " + this.passWordTextBox.Text + ";";
                str_sqlcon = str_sqlcon + "Persist Security Info = True";
                fieldsList.Clear();
                SqlConnection objConnetion = new SqlConnection(str_sqlcon);
                objConnetion.Open();
                try
                {
                    if (objConnetion.State == ConnectionState.Closed)
                    {
                        objConnetion.Open();
                    }

                    SqlCommand cmd = new SqlCommand("Select Name FROM SysColumns Where id=Object_Id('" + choTableComboBox.Text + "')", objConnetion);
                    SqlDataReader objReader = cmd.ExecuteReader();

                    while (objReader.Read())
                    {
                        fieldsList.Add(objReader[0].ToString());
                    }
                }
                catch
                {

                }
                fieldsList.Add("空");
                objConnetion.Close();
                BindingSource bsField = new BindingSource();
                bsField.DataSource = fieldsList;
                tablesListBox.DataSource = bsField;
                bsField.ResetBindings(true);
                //hadAddTableComboBox.Text = choTableComboBox.Text;
                #endregion
            }
            //
            if (hadAddTableComboBox.Items.Count > 0)
            {
                hadAddTableComboBox.SelectedIndex = -1;
            }

            choDataTypeComboBox.SelectedIndex = -1;
            normalListBox.DataSource = null;

            addListBoxDs.Clear();
            addListBox.DataSource = null;
            addListBox.DataSource = addListBoxDs;
        }

        List<string> traList = new List<string>();
        private void choDataTypeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int catagoryId = 0;
            traList.Clear();
            foreach (ProductCategory v in choDataTypeList)
            {
                if (v.CatagoryName == choDataTypeComboBox.Text)
                {
                    catagoryId = Convert.ToInt32(v.CatagoryId);
                    break;
                }
            }

            //根据是否设置配置表来确定数据表配置的不同字段
            if (chkHasConfigTable.Checked)
            {
                traList.Add("传感器ID");
            }
            else
            {
                traList.Add("模块号");
                traList.Add("通道号");
            }

            traList.Add("标记位");
            traList.Add("时间");
            for (int i = 0; i < NormaListBoxList.Count; i++)
            {
                if (NormaListBoxList[i].CatagoryId == catagoryId)
                {
                    traList.Add(NormaListBoxList[i].ValueName);
                }

            }
            normalListBoxDs.Clear();
            normalListBoxDs.AddRange(traList);
            normalListBox.DataSource = null;
            normalListBox.DataSource = traList;

            addListBoxDs.Clear();
            bs.ResetBindings(true);
            addToTabButton.Enabled = false;
        }

        private void choDataTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        public List<ExtractValueName> GetExtractValueNameList()
        {
            DAL.Common.Initialise();
            using (DbConnection conn = new DbConnection())
            {
                {
                    var bll = new ExtractValueNameBll();
                    return bll.SelectList().ToList();
                }
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tablesListBox.SelectedItems.Count; i++)
            {
                if (!(addListBoxDs.Contains(tablesListBox.SelectedItems[i].ToString())
                    && tablesListBox.SelectedItems[i].ToString() != "空")
                    && addListBox.Items.Count<normalListBox.Items.Count)
                {
                    addListBoxDs.Add(tablesListBox.SelectedItems[i].ToString());
                }
                
            }
            addListBox.DataSource = null;
            addListBox.DataSource = addListBoxDs;
            if (addListBoxDs.Count == normalListBox.Items.Count && addListBoxDs.Count !=0)
            {
                addToTabButton.Enabled = true;
            }
            else
            {
                addToTabButton.Enabled = false;
            }
        }

        private void tablesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection sic = tablesListBox.SelectedIndices;
            if (sic.Count != 0)
            {
                addButton.Enabled = true;             
            }
        }

        private void removeBtton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < addListBox.SelectedItems.Count; i++)
            {
                addListBoxDs.Remove(addListBox.SelectedItems[i].ToString());
            }
            addListBox.DataSource = null;
            addListBox.DataSource = addListBoxDs;
            if (addListBoxDs.Count == normalListBox.Items.Count && addListBoxDs.Count!=0)
            {
                addToTabButton.Enabled = true;
            }
            else
            {
                addToTabButton.Enabled = false;
            }
        }

        private void addListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox.SelectedIndexCollection sic = addListBox.SelectedIndices;
            if (sic.Count != 0)
            {
                removeBtton.Enabled = true;
            }
        }

        private void addToTabButton_Click(object sender, EventArgs e)
        {
            int ValueNameCount = 0;//字段表中参数个数
            if (!chkHasConfigTable.Checked)
            {
                ValueNameCount = normalListBoxDs.Count - 4;
            }
            else
            {
                ValueNameCount = normalListBoxDs.Count - 3;
            }
            string DBPath = "";
            IDandName[] extrValueNames = new IDandName[normalListBoxDs.Count];
            int catagoryId = 0;
            for (int i = 0; i < normalListBoxDs.Count; i++)
            {
                extrValueNames[i] = new IDandName();
            }
            if (IsAccess)
            {
                DBPath = AccessPath;
            }

            if (IsSQLite)
            {
                DBPath = SQLitebak;
            }

            if (IsSqlServer)
            {
                SQLServerPath = dbNameTextBox.Text + ":" + dbNameComboBox.Text;
                DBPath = SQLServerPath;
            }

            for (int i = 0; i < normalListBoxDs.Count; i++)
            {
                if (addListBox.Items[i].ToString() == "空")
                {
                    extrValueNames[i].Name = "";
                }
                else
                {
                    extrValueNames[i].Name = addListBoxDs[i];
                }
            }


            foreach (ProductCategory v in choDataTypeList)
            {
                if (v.CatagoryName == choDataTypeComboBox.Text)
                {
                    catagoryId = Convert.ToInt32(v.CatagoryId);
                    break;
                }
            }

            for (int i = 0; i < NormaListBoxList.Count; i++)
            {
                for (int j = (chkHasConfigTable.Checked?3:4); j < addListBox.Items.Count; j++)
                {
                    if (normalListBox.Items[j].ToString() == NormaListBoxList[i].ValueName
                        && catagoryId == NormaListBoxList[i].CatagoryId)
                    {
                        extrValueNames[j].ID = Convert.ToInt32(NormaListBoxList[i].Id);
                    }
                }

            }

            using (DbConnection conn = new DbConnection())
            {
                int dataBaseId = 0;
                DataBaseNameDic.GetDataBaseNameDic();
                List<DataBaseName> dataBaseNameList = DataBaseNameDic.GetDataBaseNameDic().GetAllBaseNames();

                for (int i = 0; i < dataBaseNameList.Count; i++)
                {
                    if (DBPath == dataBaseNameList[i].Location)
                    {
                        dataBaseId = Convert.ToInt32(dataBaseNameList[i].ID);
                        break;
                    }
                }

                //对于SQLServer数据库，数据库可能选择后再添加到DataBaseName表
                if (dataBaseId == 0)
                {
                    if (dbNameComboBox.Text != "")
                    {
                        int dataType = 0;
                        if (IsSqlServer)
                        {
                            dataType =(int)DataBaseType.SQLServer;
                        }
                        if (IsSQLite)
                        {
                            dataType = (int)DataBaseType.SQLite;
                        }
                        if (IsAccess)
                        {
                            if ("accdb" == Path.GetExtension(dbNameComboBox.Text))
                            {
                                dataType = (int)DataBaseType.ACCESSNew;
                            }
                            else
                            {
                                dataType = (int) DataBaseType.ACCESSOld;
                            }
                        }
                        if (!AddDataBaseName(dbNameComboBox.Text, SQLServerPath, dataType))
                        {
                            MessageBox.Show(@"数据库不存在并添加失败");
                            return;
                        }
                        else
                        {
                            dataBaseNameList = DataBaseNameDic.GetDataBaseNameDic().GetAllBaseNames();
                            for (int i = 0; i < dataBaseNameList.Count; i++)
                            {
                                if (DBPath == dataBaseNameList[i].Location)
                                {
                                    dataBaseId = Convert.ToInt32(dataBaseNameList[i].ID);
                                    break;
                                }
                            }
                        }
                    }
                }
                string tableName = null;

                tableName = choTableComboBox.Items[choTableComboBox.SelectedIndex].ToString();


                TableFieldInfo tableFieldInfo = new TableFieldInfo();
                if (!chkHasConfigTable.Checked)
                {
                    tableFieldInfo.DataBaseId = dataBaseId;
                    tableFieldInfo.TableName = tableName;
                    tableFieldInfo.ModuleNo = ((addListBox.Items[0].ToString() == "空")
                        ? ""
                        : addListBox.Items[0].ToString());
                    tableFieldInfo.ChannelId = ((addListBox.Items[1].ToString() == "空")
                        ? ""
                        : addListBox.Items[1].ToString());
                    tableFieldInfo.OtherFlag = ((addListBox.Items[2].ToString() == "空")
                        ? ""
                        : addListBox.Items[2].ToString());
                    tableFieldInfo.AcqTime = ((addListBox.Items[3].ToString() == "空")
                        ? ""
                        : addListBox.Items[3].ToString());
                    tableFieldInfo.ValueNameCount = ValueNameCount;
                    tableFieldInfo.SensorType = catagoryId;
                }
                else
                {
                    tableFieldInfo.DataBaseId = dataBaseId;
                    tableFieldInfo.TableName = tableName;
                    tableFieldInfo.SensorID =((addListBox.Items[0].ToString() == "空")
                        ? ""
                        : addListBox.Items[0].ToString());
                    tableFieldInfo.OtherFlag = ((addListBox.Items[1].ToString() == "空")
                        ? ""
                        : addListBox.Items[1].ToString());
                    tableFieldInfo.AcqTime = ((addListBox.Items[2].ToString() == "空")
                        ? ""
                        : addListBox.Items[2].ToString());
                    tableFieldInfo.ValueNameCount = ValueNameCount;
                    tableFieldInfo.SensorType = catagoryId;
                }

                #region 判断填充元素

                if (!chkHasConfigTable.Checked)
                {
                    switch (ValueNameCount)
                    {

                        case 1:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[4].ID;
                            break;
                        case 2:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[5].ID;
                            break;
                        case 3:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[5].ID;
                            tableFieldInfo.ExtractFieldName3 = extrValueNames[6].Name;
                            tableFieldInfo.ExtractValueNameId3 = extrValueNames[6].ID;
                            break;
                        case 4:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[5].ID;
                            tableFieldInfo.ExtractFieldName3 = extrValueNames[6].Name;
                            tableFieldInfo.ExtractValueNameId3 = extrValueNames[6].ID;
                            tableFieldInfo.ExtractFieldName4 = extrValueNames[7].Name;
                            tableFieldInfo.ExtractValueNameId4 = extrValueNames[7].ID;
                            break;
                        case 5:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[5].ID;
                            tableFieldInfo.ExtractFieldName3 = extrValueNames[6].Name;
                            tableFieldInfo.ExtractValueNameId3 = extrValueNames[6].ID;
                            tableFieldInfo.ExtractFieldName4 = extrValueNames[7].Name;
                            tableFieldInfo.ExtractValueNameId4 = extrValueNames[7].ID;
                            tableFieldInfo.ExtractFieldName5 = extrValueNames[8].Name;
                            tableFieldInfo.ExtractValueNameId5 = extrValueNames[8].ID;
                            break;
                        case 6:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[5].ID;
                            tableFieldInfo.ExtractFieldName3 = extrValueNames[6].Name;
                            tableFieldInfo.ExtractValueNameId3 = extrValueNames[6].ID;
                            tableFieldInfo.ExtractFieldName4 = extrValueNames[7].Name;
                            tableFieldInfo.ExtractValueNameId4 = extrValueNames[7].ID;
                            tableFieldInfo.ExtractFieldName5 = extrValueNames[8].Name;
                            tableFieldInfo.ExtractValueNameId5 = extrValueNames[8].ID;
                            tableFieldInfo.ExtractFieldName6 = extrValueNames[9].Name;
                            tableFieldInfo.ExtractValueNameId6 = extrValueNames[9].ID;
                            break;
                        case 7:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[5].ID;
                            tableFieldInfo.ExtractFieldName3 = extrValueNames[6].Name;
                            tableFieldInfo.ExtractValueNameId3 = extrValueNames[6].ID;
                            tableFieldInfo.ExtractFieldName4 = extrValueNames[7].Name;
                            tableFieldInfo.ExtractValueNameId4 = extrValueNames[7].ID;
                            tableFieldInfo.ExtractFieldName5 = extrValueNames[8].Name;
                            tableFieldInfo.ExtractValueNameId5 = extrValueNames[8].ID;
                            tableFieldInfo.ExtractFieldName6 = extrValueNames[9].Name;
                            tableFieldInfo.ExtractValueNameId6 = extrValueNames[9].ID;
                            tableFieldInfo.ExtractFieldName7 = extrValueNames[10].Name;
                            tableFieldInfo.ExtractValueNameId7 = extrValueNames[10].ID;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (ValueNameCount)
                    {

                        case 1:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[3].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[3].ID;
                            break;
                        case 2:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[3].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[3].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[4].ID;
                            break;
                        case 3:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[3].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[3].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName3 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId3 = extrValueNames[5].ID;
                            break;
                        case 4:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[3].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[3].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName3 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId3 = extrValueNames[5].ID;
                            tableFieldInfo.ExtractFieldName4 = extrValueNames[6].Name;
                            tableFieldInfo.ExtractValueNameId4 = extrValueNames[6].ID;
                            break;
                        case 5:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[3].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[3].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName3 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId3 = extrValueNames[5].ID;
                            tableFieldInfo.ExtractFieldName4 = extrValueNames[6].Name;
                            tableFieldInfo.ExtractValueNameId4 = extrValueNames[6].ID;
                            tableFieldInfo.ExtractFieldName5 = extrValueNames[7].Name;
                            tableFieldInfo.ExtractValueNameId5 = extrValueNames[7].ID;
                            break;
                        case 6:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[3].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[3].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName3 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId3 = extrValueNames[5].ID;
                            tableFieldInfo.ExtractFieldName4 = extrValueNames[6].Name;
                            tableFieldInfo.ExtractValueNameId4 = extrValueNames[6].ID;
                            tableFieldInfo.ExtractFieldName5 = extrValueNames[7].Name;
                            tableFieldInfo.ExtractValueNameId5 = extrValueNames[7].ID;
                            tableFieldInfo.ExtractFieldName6 = extrValueNames[8].Name;
                            tableFieldInfo.ExtractValueNameId6 = extrValueNames[8].ID;
                            break;
                        case 7:
                            tableFieldInfo.ExtractFieldName1 = extrValueNames[3].Name;
                            tableFieldInfo.ExtractValueNameId1 = extrValueNames[3].ID;
                            tableFieldInfo.ExtractFieldName2 = extrValueNames[4].Name;
                            tableFieldInfo.ExtractValueNameId2 = extrValueNames[4].ID;
                            tableFieldInfo.ExtractFieldName3 = extrValueNames[5].Name;
                            tableFieldInfo.ExtractValueNameId3 = extrValueNames[5].ID;
                            tableFieldInfo.ExtractFieldName4 = extrValueNames[6].Name;
                            tableFieldInfo.ExtractValueNameId4 = extrValueNames[6].ID;
                            tableFieldInfo.ExtractFieldName5 = extrValueNames[7].Name;
                            tableFieldInfo.ExtractValueNameId5 = extrValueNames[7].ID;
                            tableFieldInfo.ExtractFieldName6 = extrValueNames[8].Name;
                            tableFieldInfo.ExtractValueNameId6 = extrValueNames[8].ID;
                            tableFieldInfo.ExtractFieldName7 = extrValueNames[9].Name;
                            tableFieldInfo.ExtractValueNameId7 = extrValueNames[9].ID;
                            break;
                        default:
                            break;
                    }
                }
                #endregion

                if (TableFieldInfoDic.GetTableFieldInfoDic().CheckAdd(tableFieldInfo) > 0)
                {
                    MessageBox.Show("已添加到表");
                }
            }
            if (choDataTypeComboBox.Items.Count > 0)
                choDataTypeComboBox.SelectedIndex = -1;
        }

        private void hadAddTableComboBox_DropDown(object sender, EventArgs e)
        {
            List<TableFieldInfo> list = TableFieldInfoDic.GetTableFieldInfoDic().GetAllTableFieldInfos();
            HadAddTableList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                HadAddTableList.Add(list[i].TableName);
            }
            hadAddTableComboBox.DataSource = HadAddTableList;
            BindingSource bdSource = new BindingSource();
            bdSource.DataSource = HadAddTableList;
            hadAddTableComboBox.DataSource = bdSource;
            bdSource.ResetBindings(true);
        }

        private void removeAddTableButton_Click(object sender, EventArgs e)
        {
            int id = 0;
            List<TableFieldInfo> list = TableFieldInfoDic.GetTableFieldInfoDic().GetAllTableFieldInfos();
            for (int i = 0; i < list.Count; i++)
            {
                if (hadAddTableComboBox.Text == list[i].TableName)
                {
                    id = Convert.ToInt32(list[i].Id);
                }
            }
            if (id == 0)
            {
                MessageBox.Show(@"表不存在");
                return;
            }
            if (TableFieldInfoDic.GetTableFieldInfoDic().Delete(id))
            {
                MessageBox.Show(@"表已经删除");
            }
            if (choDataTypeComboBox.Items.Count > 0)
            {
                choDataTypeComboBox.SelectedIndex = -1;
            }
            IniteLizeHadAddTable();//重新获取添加的表到hadAddTableComboBox
            addListBoxDs.Clear();
            addListBox.DataSource = null;
            addListBox.DataSource = addListBoxDs;

            if (hadAddTableComboBox.Items.Count > 0)
            {
                hadAddTableComboBox.SelectedIndex = -1;
            }
            normalListBox.DataSource = null;

        }

        

        private void dbNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (passTest == true)
            {
                string str_sqlcon = null;
                string str_sqlTableName = null;
                string str_sqlTableCount = null;
                str_sqlcon = str_sqlcon + "Data Source = " + this.dbNameTextBox.Text + ";";
                //   str_sqlcon = str_sqlcon + "," + "1433" + ";"; //端口
                str_sqlcon = str_sqlcon + "Network Library = " + "DBMSSOCN" + ";";
                str_sqlcon = str_sqlcon + "Initial Catalog = " + ((DataRowView)this.dbNameComboBox.SelectedItem).Row.ItemArray[0].ToString() + ";"; //库名
                str_sqlcon = str_sqlcon + "User ID = " + this.userNameTextBox.Text + ";";
                str_sqlcon = str_sqlcon + "Password = " + this.passWordTextBox.Text + ";";
                str_sqlcon = str_sqlcon + "Persist Security Info = True";
                
                SqlConnection objConnetion = new SqlConnection(str_sqlcon);
                try
                {
                    if (objConnetion.State == ConnectionState.Closed)
                    {
                        tableNames.Clear();
                        objConnetion.Open();
                        DataTable objTable = objConnetion.GetSchema("Tables");
                        foreach (DataRow row in objTable.Rows)
                        {
                            tableNames.Add(row[2].ToString());
                        }
                    }
                    choTableComboBox.Text = tableNames[0];
                }
                catch
                {
                    MessageBox.Show(@"表存在异常");
                }
                finally
                {
                    if (objConnetion != null && objConnetion.State == ConnectionState.Closed)
                    {
                        objConnetion.Dispose();
                    }

                }

                //tableListBox相关
                SqlConnection objConnetion2 = new SqlConnection(str_sqlcon);
                try
                {
                    if (objConnetion2.State == ConnectionState.Closed)
                    {
                        objConnetion2.Open();
                    }
                    fieldsList.Clear();
                    SqlCommand cmd =
                        new SqlCommand(
                            "Select Name FROM SysColumns Where id=Object_Id('" + choTableComboBox.Text + "')",
                            objConnetion);
                    SqlDataReader objReader = cmd.ExecuteReader();

                    while (objReader.Read())
                    {
                        fieldsList.Add(objReader[0].ToString());
                    }
                    fieldsList.Add("空");
                }
                catch
                {
                    MessageBox.Show(@"表存在异常");
                }
                finally
                {
                    if (objConnetion2 != null && objConnetion2.State == ConnectionState.Closed)
                    {
                        objConnetion2.Dispose();
                    }

                }

            }
            bsTableNames.ResetBindings(true);
            BindingSource bsField = new BindingSource();
            bsField.DataSource = fieldsList;
            tablesListBox.DataSource = bsField;
            bsField.ResetBindings(true);
            hadAddTableComboBox.Text = choDataTypeComboBox.Text;
        }

        private void nextButtonOther_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(@"是否继续配置", @"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                tabControl1.SelectTab(0);
                DisEnableTab3();
                DisableTab1();
                AccessRadioButton.Enabled = true;
                sqliteRadioButton.Enabled = true;
                sqlServerRadioButton.Enabled = true;
                AccessRadioButton.Checked = false;
                sqliteRadioButton.Checked = false;
                sqlServerRadioButton.Checked = false;
            }
            else
            {
                this.Close();
                closeFather(); //连同父窗口一同关闭
            }
        }
        public void IniteLizeHadAddTable()
        {
            List<TableFieldInfo> list = TableFieldInfoDic.GetTableFieldInfoDic().GetAllTableFieldInfos();
            HadAddTableList.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                HadAddTableList.Add(list[i].TableName);
            }
            hadAddTableComboBox.DataSource = HadAddTableList;
            BindingSource bdSource = new BindingSource();
            bdSource.DataSource = HadAddTableList;
            hadAddTableComboBox.DataSource = bdSource;
            bdSource.ResetBindings(true);
        }

        private void Form9_Load(object sender, EventArgs e)
        {
            DiseableTab1Section();
            DisableTab2();
            DisEnableTab3();
            grpUserPasswd.Visible = false;
        }

        private void testLineButton_Click(object sender, EventArgs e)
        {
            if (IsAccess)
            {
                if (dbNameTextBox.Text == "")
                {
                    MessageBox.Show(@"请选择数据库文件", "WARING");
                    return;
                }

                string newPath = null;
                if (AccessPath != null)
                {
                    string[] strs = AccessPath.Split("\\".ToCharArray());
                    strs[strs.Count() - 1] = dbNameTextBox.Text;
                    for (int i = 0; i < strs.Count(); i++)
                    {
                        if (i != strs.Count() - 1)
                            newPath = newPath + strs[i] + @"\";
                    }
                    newPath += dbNameTextBox.Text;
                }
                else
                {
                    MessageBox.Show(@"数据库不正确");
                    nextButton.Enabled = false;
                    return;
                }

                string[] str = newPath.Split(@".".ToCharArray());
                if (str[str.Count() - 1] == "accdb")
                {
                    AccessConnStr = "Provider=Microsoft.ACE.OLEDB.12.0;";
                    AccessConnStr += @"Data Source=" + newPath+";";
                    if (userNameTextBox.Text != "" || passWordTextBox.Text != "")
                    {
                        AccessConnStr += "Jet OLEDB:Database Password='" + passWordTextBox.Text + "';";
                    }
                    else
                    {
                        AccessConnStr += "Persist Security Info=False;";
                    }
                }
                else
                {
                    AccessConnStr = "Provider=Microsoft.Jet.OLEDB.4.0;";
                    AccessConnStr += @"Data Source=" + newPath+";";
                    if (userNameTextBox.Text != "" || passWordTextBox.Text != "")
                    {
                        AccessConnStr += "Jet OLEDB:Database Password='" + passWordTextBox.Text + "';";
                    }
                    else
                    {
                        AccessConnStr += "Persist Security Info=False;";
                    }
                }

                using (OleDbConnection objConnection = new OleDbConnection(AccessConnStr))
                {
                    try
                    {
                        objConnection.Open();
                        if (objConnection.State == ConnectionState.Open)
                        {
                            passTest = true;
                            MessageBox.Show(@"连接成功");
                            nextButton.Enabled = true;
                        }
                        else
                        {
                            MessageBox.Show(@"无法连接到指定数据库");
                            nextButton.Enabled = false;
                            return;
                        }
                        InitelizeAccessCompents();
                        if (str[str.Count() - 1] == "accdb")
                        {
                            AddDataBaseName(dbNameTextBox.Text, AccessPath, (int)DataBaseType.ACCESSNew);
                        }
                        else
                        {
                            AddDataBaseName(dbNameTextBox.Text, AccessPath, (int)DataBaseType.ACCESSOld);
                        }
                        
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(@"数据库连接失败");
                        nextButton.Enabled = false;
                    }
                    finally
                    {
                        objConnection.Close();
                    }
                }

            }

            if (IsSQLite)
            {
                if (dbNameTextBox.Text == "")
                {
                    MessageBox.Show(@"请选择数据库文件", "WARING");
                    return;
                }
                string strStart = @"Data Source=";
                string strEnd = @";Version=3;Pooling=False;Max Pool Size=100";
                string newPath = null;
                if (SQLitePath != null)
                {
                    string[] strs = SQLitePath.Split("\\".ToCharArray());
                    if (strs[strs.Count() - 1] != "")
                    {
                        this.dbNameTextBox.Text = strs[strs.Count() - 1];
                        
                    }
                    strs[strs.Count() - 1] = dbNameTextBox.Text;
                    SQLitebak = SQLitePath;
                    SQLitePath = null;
                    for (int i = 0; i < strs.Count(); i++)
                    {
                        if (i != strs.Count() - 1)
                            newPath = newPath + strs[i] + @"\";
                    }
                    newPath += dbNameTextBox.Text;
                    SQLitePath = newPath;
                    SqliteConnStr = strStart + newPath + strEnd;

                    var bll = new ConnectTestBll();
                    if (bll.IsConnect(SqliteConnStr))
                    {
                        passTest = true;
                        MessageBox.Show(@"连接成功");
                        nextButton.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show(@"无法连接到指定数据库");
                        nextButton.Enabled = false;
                        return;
                    }
                    InitelizeSqliteCompents();
                    AddDataBaseName(dbNameTextBox.Text, SQLitePath,(int)DataBaseType.SQLite);
                    SQLitePath = null;
                }
                else
                {
                    MessageBox.Show(@"请选择数据库");
                    nextButton.Enabled = false;
                }

            }

            if (IsSqlServer)
            {
                #region 连接测试
                string str_sqlcon = null;
                string str_sqlDbName = null;
                string portNum = "1433";
                string[] getPortNum = new string[2];
                getPortNum = dbNameTextBox.Text.Split(':');
                if (getPortNum.Count() == 2)
                {
                    portNum = getPortNum[1];
                }
                ///检查格式
                if (this.dbNameTextBox.Text.Trim() == "")
                {
                    MessageBox.Show(@"请输入服务器IP", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.testLineButton.Enabled = true;
                    return;
                }
                try
                {
                    String ip = this.dbNameTextBox.Text.Trim();
                    int num = ip.Split('.').Length;
                    if (num != 4)
                    {
                        MessageBox.Show(@"IP地址输入格式错误", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        this.testLineButton.Enabled = true;
                        return;
                    }
                    else
                    {
                        String[] lines = new String[5];
                        String s = ".";
                        lines = ip.Split(new[] { '.', ':' });
                        for (int i = 0; i < 4; i++)
                        {
                            if (Convert.ToInt32(lines[i]) > 255 || Convert.ToInt32(lines[i]) < 0)
                            {
                                MessageBox.Show(@"IP地址输入格式错误", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.testLineButton.Enabled = true;
                                return;
                            }
                        }
                    }
                }
                catch
                {
                    MessageBox.Show(@"IP地址输入格式错误", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.testLineButton.Enabled = true;
                    return;
                }
                if (portNum == "")
                {
                    MessageBox.Show(@"请输入数据库端口号", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    Convert.ToInt32(portNum.Trim());
                }
                catch
                {
                    MessageBox.Show(@"端口号格式不正确",@"提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if ((Convert.ToInt32(portNum.Trim()) < 0) || (Convert.ToInt32(portNum.Trim()) > 65535))
                {
                    MessageBox.Show(@"端口号输入不正确", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (this.userNameTextBox.Text.Trim() == "")
                {
                    MessageBox.Show(@"请输入数据库用户名", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.testLineButton.Enabled = true;
                    return;
                }
                if (this.passWordTextBox.Text.Trim() == "")
                {
                    MessageBox.Show(@"请输入数据库密码", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.testLineButton.Enabled = true;
                    return;
                }

                str_sqlcon = str_sqlcon + "Data Source = " + this.dbNameTextBox.Text;
                str_sqlcon = str_sqlcon + "," + portNum + ";"; //端口
                str_sqlcon = str_sqlcon + "Network Library = " + "DBMSSOCN" + ";";
                str_sqlcon = str_sqlcon + "User ID = " + this.userNameTextBox.Text + ";";
                str_sqlcon = str_sqlcon + "Password = " + this.passWordTextBox.Text + ";";
                str_sqlcon = str_sqlcon + "Persist Security Info = True";
                SqlConnection con = new SqlConnection(str_sqlcon);
                try
                {
                    con.Open();
                }
                catch
                {
                    MessageBox.Show(@"测试连接失败...", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    passTest = false;
                    this.testLineButton.Enabled = true;
                    this.dbNameComboBox.DataSource = null;
                    this.choTableComboBox.DataSource = null;
                    this.dbNameComboBox.Items.Clear();
                    this.choTableComboBox.Items.Clear();
                    dbNameComboBox.Text = "";
                    choTableComboBox.Text = "";
                    con.Close();
                    nextButton.Enabled = false;

                    this.cmbDbName.DataSource = null;
                    this.cmbDbName.Items.Clear();
                    cmbDbName.Text = "";

                    return;
                }
                this.testLineButton.Enabled = true;
                MessageBox.Show(@"测试连接通过...", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                passTest = true;
                nextButtonOther.Enabled = true;
                if (passTest == true)
                {
                    str_sqlDbName = "select name from master..sysdatabases";
                    DataSet d1 = new DataSet();
                    //        SqlCommand cmd = new SqlCommand(str_sqlDbName,con);  
                    SqlDataAdapter DataAdapter = new SqlDataAdapter(str_sqlDbName, con);
                    DataAdapter.Fill(d1, "[d1]");
                    this.dbNameComboBox.DataSource = d1.Tables["[d1]"];
                    this.dbNameComboBox.DisplayMember = d1.Tables["[d1]"].Columns[0].ColumnName;
                    this.dbNameComboBox.ValueMember = d1.Tables["[d1]"].Columns[0].ColumnName;

                    this.cmbDbName.DataSource = d1.Tables["[d1]"];
                    this.cmbDbName.DisplayMember = d1.Tables["[d1]"].Columns[0].ColumnName;
                    this.cmbDbName.ValueMember = d1.Tables["[d1]"].Columns[0].ColumnName;

                    d1.Dispose();
                    nextButton.Enabled = true;
                }
                #endregion
                InitelizeSqlServerCompents();
                //AddDataBaseName(dbNameComboBox.Text, dbNameTextBox.Text + ":" + dbNameComboBox.Text,(int)DataBaseType.SQLServer);
            }
            
        }

        private void scanButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = "C://";
            if (IsSQLite)
            {
                fileDialog.Filter = "Db3 files (*.db3)|*.db3|All files (*.*)|*.*";
            }
            if (IsAccess)
            {
                fileDialog.Filter = "Mdb files (*.mdb)|*.mdb|Accdb files (*.accdb)|*.accdb|All files (*.*)|*.*";
            }
            fileDialog.FilterIndex = 1;
            fileDialog.RestoreDirectory = true;
            fileDialog.FileName = "";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (IsSQLite)
                {
                    SQLitePath = fileDialog.FileName;
                }
                else
                {
                    SQLitePath = dbNameTextBox.Text;
                }
                if (IsAccess)
                {
                    AccessPath = fileDialog.FileName;
                }
                else
                {
                    AccessPath = dbNameTextBox.Text;
                }
                if (IsSqlServer)
                {
                    SQLServerPath = fileDialog.FileName;
                }

                string[] strs = fileDialog.FileName.Split("\\".ToCharArray());
                this.dbNameTextBox.Text = strs[strs.Count() - 1];
                rememberDbpath = strs[strs.Count() - 1];
            }
        }

        private void hadAddTableComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ModifyAddListBox();
            ModifyNormalListbox();//normalListBox内容跟随选择的添加表变化
        }

        public void ModifyNormalListbox()
        {
            List<TableFieldInfo> list = TableFieldInfoDic.GetTableFieldInfoDic().GetAllTableFieldInfos();
            int SersonType = 0;
            for (int i = 0; i < list.Count; i++)
            {
                if (hadAddTableComboBox.Items[hadAddTableComboBox.SelectedIndex].ToString() == list[i].TableName)
                {
                    SersonType = list[i].SensorType;
                    break;
                }
            }
            traList.Clear();

            //根据是否设置配置表来确定数据表配置的不同字段
            //if (chkHasConfigTable.Checked)
            //{
            //    traList.Add("传感器ID");
            //}
            //else
            //{
            //    traList.Add("模块号");
            //    traList.Add("通道号");
            //}
            traList.Add("传感器ID");
            traList.Add("模块号");
            traList.Add("通道号");
            traList.Add("标记位");
            traList.Add("时间");
            for (int i = 0; i < NormaListBoxList.Count; i++)
            {
                if (NormaListBoxList[i].CatagoryId == SersonType)
                {
                    traList.Add(NormaListBoxList[i].ValueName);
                }
            }
            normalListBox.DataSource = null;
            normalListBox.DataSource = traList;

        }

        public void ModifyAddListBox()
        {
            addToTabButton.Enabled = false;
            addListBoxDs.Clear();
            List<TableFieldInfo> list = TableFieldInfoDic.GetTableFieldInfoDic().GetAllTableFieldInfos();
            for (int i = 0; i < list.Count; i++)
            {
                if (hadAddTableComboBox.Items[hadAddTableComboBox.SelectedIndex].ToString() == list[i].TableName)
                {
                    //if (chkHasConfigTable.Checked)
                    //{
                    //    addListBoxDs.Add(list[i].SensorID);
                    //}
                    //else
                    //{
                    //    addListBoxDs.Add(list[i].ModuleNo);
                    //    addListBoxDs.Add(list[i].ChannelId);
                    //}
                    addListBoxDs.Add(list[i].SensorID);
                    addListBoxDs.Add(list[i].ModuleNo);
                    addListBoxDs.Add(list[i].ChannelId);


                    addListBoxDs.Add(list[i].OtherFlag);
                    addListBoxDs.Add(list[i].AcqTime);
                    if (list[i].ExtractFieldName1 != null)
                    {
                        addListBoxDs.Add(list[i].ExtractFieldName1);
                    }
                    if (list[i].ExtractFieldName2 != null)
                    {
                        addListBoxDs.Add(list[i].ExtractFieldName2);
                    }
                    if (list[i].ExtractFieldName3 != null)
                    {
                        addListBoxDs.Add(list[i].ExtractFieldName3);
                    }
                    if (list[i].ExtractFieldName4 != null)
                    {
                        addListBoxDs.Add(list[i].ExtractFieldName4);
                    }
                    if (list[i].ExtractFieldName5 != null)
                    {
                        addListBoxDs.Add(list[i].ExtractFieldName5);
                    }
                    if (list[i].ExtractFieldName6 != null)
                    {
                        addListBoxDs.Add(list[i].ExtractFieldName6);
                    }
                    if (list[i].ExtractFieldName7 != null)
                    {
                        addListBoxDs.Add(list[i].ExtractFieldName7);
                    }
                    break;
                }
            }
            for (int i = 0; i < addListBoxDs.Count; i++)
            {
                if (addListBoxDs[i] == null || addListBoxDs[i]=="")
                {
                    addListBoxDs[i] = "空";
                }
            }
            addListBox.DataSource = null;
            addListBox.DataSource = addListBoxDs;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(2);
            IniteLizeHadAddTable();
            DisableTab2();
            EnableTab3();
            CommInitelize();
            addToTabButton.Enabled = false;
        }

        

        private void cmbChoConTab_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (IsAccess)
            {
                lstOrginList.Clear();
                lstOrginList = AccessOperation.GetTableFieldNameList
                    (cmbChoConTab.Items[cmbChoConTab.SelectedIndex].ToString());
                lstOrginList.Add("空");
                lstOrigin.DataSource = lstOrginList;
            }
            if (IsSQLite)
            {
                lstOrginList.Clear();
                lstOrginList = SQLiteOperatiopn.GetSQLiteFieldNames
                    (cmbChoConTab.Items[cmbChoConTab.SelectedIndex].ToString());
                lstOrginList.Add("空");
                lstOrigin.DataSource = lstOrginList;
            }

            if (IsSqlServer)
            {
                #region//绑定tablesListBox
                string str_sqlcon = null;
                lstOrginList.Clear();
                str_sqlcon = str_sqlcon + "Data Source = " + this.dbNameTextBox.Text + ";";
                //   str_sqlcon = str_sqlcon + "," + "1433" + ";"; //端口
                str_sqlcon = str_sqlcon + "Network Library = " + "DBMSSOCN" + ";";
                str_sqlcon = str_sqlcon + "Initial Catalog = " + ((DataRowView)this.dbNameComboBox.SelectedItem).Row.ItemArray[0].ToString() + ";"; //库名
                str_sqlcon = str_sqlcon + "User ID = " + this.userNameTextBox.Text + ";";
                str_sqlcon = str_sqlcon + "Password = " + this.passWordTextBox.Text + ";";
                str_sqlcon = str_sqlcon + "Persist Security Info = True";
                choConTabList.Clear();
                SqlConnection objConnetion = new SqlConnection(str_sqlcon);
                try
                {
                    objConnetion.Open();
                    if (objConnetion.State == ConnectionState.Closed)
                    {
                        objConnetion.Open();
                    }

                    SqlCommand cmd = new SqlCommand("Select Name FROM SysColumns Where id=Object_Id('" + cmbChoConTab.Text + "')", objConnetion);
                    SqlDataReader objReader = cmd.ExecuteReader();

                    while (objReader.Read())
                    {
                        lstOrginList.Add(objReader[0].ToString());
                    }
                }
                catch
                {
                    MessageBox.Show(@"数据库打开失败");
                }
                objConnetion.Close();
                lstOrginList.Add("空");
                lstOrigin.DataSource = null;
                lstOrigin.DataSource = lstOrginList;
                #endregion
            }

            //cmbHadAddConfig随cmbChoConTab的改变有相应变化
            cmbHadAddConfig.SelectedIndex = -1;
            lstAddList.Clear();
            lstAdd.DataSource = null;
            lstAdd.DataSource = lstAddList;

            btnAddConfig.Enabled = false;

        }

        public List<string> lstAddList = new List<string>(); 
        private void btnToConfig_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstOrigin.SelectedItems.Count; i++)
            {
                if ((!lstAdd.Items.Contains(lstOrigin.SelectedItems[i].ToString())
                    || lstOrigin.SelectedItems[i].ToString() == "空") 
                    && lstAdd.Items.Count < 4)
                {
                    lstAddList.Add(lstOrigin.SelectedItems[i].ToString());
                }
                lstAdd.DataSource = null;
                lstAdd.DataSource = lstAddList;
            }
            if (lstAdd.Items.Count == 4)
            {
                btnAddConfig.Enabled = true;
            }
            else
            {
                btnAddConfig.Enabled = false;
            }
        }

        private void btnLeaveConfig_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < lstAdd.SelectedItems.Count; i++)
            {
                lstAddList.Remove(lstAdd.SelectedItems[i].ToString());
                lstAdd.DataSource = null;
                lstAdd.DataSource = lstAddList;
            }
            if (lstAdd.Items.Count == 4)
            {
                btnAddConfig.Enabled = true;
            }
            else
            {
                btnAddConfig.Enabled = false;
            }
        }

        private void btnAddConfig_Click(object sender, EventArgs e)
        {
            if (lstAddList.Count == 4)
            {
                string sensorId = lstAddList[0];
                string moduleId = lstAddList[1];
                string channelId = lstAddList[2];
                string otherFlag = lstAddList[3];

                ConfigTable configTable = new ConfigTable()
                {
                    SensorId = (sensorId == "空") ? "" : sensorId,
                    ModuleNo = (moduleId == "空") ? "" : moduleId,
                    ChannelId = (channelId == "空") ? "" : channelId,
                    OtherFlag = (otherFlag == "空") ? "" : otherFlag,
                    TableName = cmbChoConTab.Items[cmbChoConTab.SelectedIndex].ToString(),
                };

                #region 确定数据库ID
                //在此判断选择的数据库是否为SQLServer数据库，SQLServer数据库的增加方式特殊
                if (IsSqlServer)
                {
                    int dataType = 0;
                    if (IsSqlServer)
                    {
                        dataType = (int)DataBaseType.SQLServer;
                    }
                    if (IsSQLite)
                    {
                        dataType = (int)DataBaseType.SQLite;
                    }
                    if (IsAccess)
                    {
                        if ("accdb" == Path.GetExtension(dbNameComboBox.Text))
                        {
                            dataType = (int)DataBaseType.ACCESSNew;
                        }
                        else
                        {
                            dataType = (int)DataBaseType.ACCESSOld;
                        }
                    }

                    AddDataBaseName(cmbDbName.Text, dbNameTextBox.Text + ":" + cmbDbName.Text, dataType);
                    

                    List<DataBaseName> list = DataBaseNameDic.GetDataBaseNameDic().GetAllBaseNames();

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (dbNameComboBox.Text == list[i].DataBaseCode)
                        {
                            configTable.DataBaseId = (int)list[i].ID;
                            SQLServerPath = list[i].Location;
                            break;
                        }
                    }
                }
                else
                {
                    List<DataBaseName> list = DataBaseNameDic.GetDataBaseNameDic().GetAllBaseNames();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (dbNameTextBox.Text == list[i].DataBaseCode)
                        {
                            configTable.DataBaseId = (int)list[i].ID;
                            break;
                        }
                    }
                }
                #endregion
                if (ConfigTableDic.GetConfigTableDic().CheckAdd(configTable) > 0)
                {
                    MessageBox.Show(@"添加成功");
                } 
            }
            else
            {
                MessageBox.Show(@"添加失败");
            }

            //为cmbHadAddConfig重新绑定数据源
            List<ConfigTable> listTable = ConfigTableDic.GetConfigTableDic().SelectList();
            cmbHadAddTableList.Clear();
            for (int i = 0; i < listTable.Count; i++)
            {
                cmbHadAddTableList.Add(listTable[i].TableName);
            }
            cmbHadAddConfig.DataSource = null;
            cmbHadAddConfig.DataSource = cmbHadAddTableList;

            cmbHadAddConfig.SelectedIndex = -1;

            lstAddList.Clear();
        }

        private void cmbHadAddConfig_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string haddAddTableName = cmbHadAddConfig.Items[cmbHadAddConfig.SelectedIndex].ToString();

            List<ConfigTable> listTable = ConfigTableDic.GetConfigTableDic().SelectList();
            for (int i = 0; i < listTable.Count; i++)
            {
                if (haddAddTableName == listTable[i].TableName)
                {
                    lstAddList.Clear();
                    lstAddList.Add((listTable[i].SensorId) == "" ? "空" : listTable[i].SensorId);
                    lstAddList.Add((listTable[i].ModuleNo) == "" ? "空" : listTable[i].ModuleNo);
                    lstAddList.Add((listTable[i].ChannelId == "") ? "空" : listTable[i].ChannelId);
                    lstAddList.Add((listTable[i].OtherFlag == "") ? "空" : listTable[i].OtherFlag);
                    break;
                }
                
            }
            lstAdd.DataSource = null;
            lstAdd.DataSource = lstAddList;
        }

        private void btnRemoveConfig_Click(object sender, EventArgs e)
        {
            string removeTableName = null;
            if (cmbHadAddConfig.Items.Count > 0)
            {
                if (cmbHadAddConfig.Text == "")
                {
                    MessageBox.Show(@"删除失败");
                    return;
                }
                removeTableName = cmbHadAddConfig.Items[cmbHadAddConfig.SelectedIndex].ToString();
            }
            else
            {
                MessageBox.Show(@"删除失败");
                return;
            }
            List<ConfigTable> listTable = ConfigTableDic.GetConfigTableDic().SelectList();
            for (int i = 0; i < listTable.Count; i++)
            {
                if (removeTableName == listTable[i].TableName)
                {
                    ConfigTableDic.GetConfigTableDic().Delete(Convert.ToInt32(listTable[i].ID));
                    MessageBox.Show(@"删除成功");
                    break;
                }
            }

            listTable = ConfigTableDic.GetConfigTableDic().SelectList();
            cmbHadAddTableList.Clear();
            for (int i = 0; i < listTable.Count; i++)
            {
                cmbHadAddTableList.Add(listTable[i].TableName);
            }
            cmbHadAddConfig.DataSource = null;
            cmbHadAddConfig.DataSource = cmbHadAddTableList;

            //删除添加表后清空lstAdd
            lstAddList.Clear();
            lstAdd.DataSource = null;
            lstAdd.DataSource = lstAddList;
        }

        private void chkHasConfigTable_CheckedChanged(object sender, EventArgs e)
        {
            if (chkHasConfigTable.Checked)
            {
                grpConfigTable.Enabled = true;
            }
            else
            {
                grpConfigTable.Enabled = false;
            }
        }

        private void btnAdv_Click(object sender, EventArgs e)
        {
            if (grpUserPasswd.Visible == false)
            {
                grpUserPasswd.Visible = true;
            }
            else
            {
                grpUserPasswd.Visible = false;
            }

        }

        private void rdoText_CheckedChanged(object sender, EventArgs e)
        {
            if (rdoText.Checked)
            {
                var FrmText = new FrmTextConfig();
                FrmText.cacelText += Cacelradio;
                FrmText.closeFather += TxtCloseFather;
                FrmText.ShowDialog();
            }
        }

        public void TxtCloseFather()
        {
            this.Close();
        }

        public void Cacelradio()
        {
            rdoText.Checked = false;
        }

        private void btnBack1_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(0);
            DisableTab2();
            EnableTab1();    
        }

        private void btnBack2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
            DisEnableTab3();
            EnableTab2();
        }
    }
}
