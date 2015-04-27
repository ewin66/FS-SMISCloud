using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using FSDE.DAL.Config;
using FSDE.Dictionaries;
using FSDE.Dictionaries.config;
using FSDE.Model.Config;

namespace FSDE.Forms
{
    public partial class FrmShowConfigInfo : Form
    {
        public static List<DataBaseName> dataGriewDataBaseSource = new List<DataBaseName>();
        public static List<ConfigTable> dataGriewConfigTableSource = new List<ConfigTable>();
        public static List<TableFieldInfo> dataGriewDataTableSource = new List<TableFieldInfo>();

        public FrmShowConfigInfo()
        {
            InitializeComponent();
        }

        private void FrmShowConfigInfo_Load(object sender, EventArgs e)
        {
            BingDataSource();
            if (dgvDataBase.Rows.Count > 0)
            {
                dgvDataBase.Rows[0].Selected = false;
            }
        }

        private void BingDataSource()
        {
            int width = 0;//定义一个局部变量，用于存储自动调整列宽以后整个DtaGridView的宽度

            //数据库信息绑定
            dgvDataBase.Columns.Clear();
            dgvDataBase.DataSource = null;
            DataBaseAddColumns();
            dataGriewDataBaseSource = DataBaseNameDic.GetDataBaseNameDic().GetAllBaseNames();
            dgvDataBase.DataSource = dataGriewDataBaseSource;
            dgvDataBase.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //配置表信息绑定
            gdvConfigTable.Columns.Clear();
            gdvConfigTable.DataSource = null;
            ConfigTableAddColumns();
            dataGriewConfigTableSource = ConfigTableDic.GetConfigTableDic().SelectList();
            gdvConfigTable.DataSource = dataGriewConfigTableSource;
            gdvConfigTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            for (int i = 0; i < this.gdvConfigTable.Columns.Count; i++)//对于DataGridView的每一个列都调整
            {
                this.gdvConfigTable.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);//将每一列都调整为自动适应模式
                width += this.gdvConfigTable.Columns[i].Width;//记录整个DataGridView的宽度
            }
            if (width > this.gdvConfigTable.Size.Width)//判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，则将DataGridView的列自动调整模式设置为显示的列即可，如果是小于原来设定的宽度，将模式改为填充。
            {
                this.gdvConfigTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            else
            {
                this.gdvConfigTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }


            //数据表信息配置
            dgvDataTableInfo.Columns.Clear();
            dgvDataTableInfo.DataSource = null;
            DataTablesAddColumns();
            dataGriewDataTableSource = TableFieldInfoDic.GetTableFieldInfoDic().GetAllTableFieldInfos();
            dgvDataTableInfo.DataSource = dataGriewDataTableSource;
            dgvDataTableInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            width = 0;
            for (int i = 0; i < this.dgvDataTableInfo.Columns.Count; i++)//对于DataGridView的每一个列都调整
            {
                this.dgvDataTableInfo.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);//将每一列都调整为自动适应模式
                width += this.dgvDataTableInfo.Columns[i].Width;//记录整个DataGridView的宽度
            }
            if (width > this.dgvDataTableInfo.Size.Width)//判断调整后的宽度与原来设定的宽度的关系，如果是调整后的宽度大于原来设定的宽度，则将DataGridView的列自动调整模式设置为显示的列即可，如果是小于原来设定的宽度，将模式改为填充。
            {
                this.dgvDataTableInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            }
            else
            {
                this.dgvDataTableInfo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            if (dgvDataBase.Rows.Count > 0)
            {
                dgvDataBase.Rows[0].Selected = false;
            }
        }

        public void DataTablesAddColumns()
        {
            List<TableFieldInfo> tableFieldList = TableFieldInfoDic.GetTableFieldInfoDic().GetAllTableFieldInfos();
            int maxTables = 0;

            for (int i = 0; i < tableFieldList.Count; i++)
            {
                if (maxTables < tableFieldList[i].ValueNameCount)
                {
                    maxTables = tableFieldList[i].ValueNameCount;
                }
            }


            DataGridViewTextBoxColumn colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataBaseId",
                HeaderText = @"数据库ID",
                Name = "colDataBaseId",
                //Resizable = DataGridViewTriState.True
            };
            dgvDataTableInfo.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TableName",
                HeaderText = @"表名",
                Name = "colTableName",
                //Resizable = DataGridViewTriState.True
            };
            dgvDataTableInfo.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "OtherFlag",
                HeaderText = @"标记位",
                Name = "colOtherFlag",
                //Resizable = DataGridViewTriState.True
            };
            dgvDataTableInfo.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SensorType",
                HeaderText = @"传感器类型",
                Name = "colSensorType",
                //Resizable = DataGridViewTriState.True,
            };
            dgvDataTableInfo.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ValueNameCount",
                HeaderText = @"映射字段个数",
                Name = "colValueNameCount",
                //Resizable = DataGridViewTriState.True,
            };
            dgvDataTableInfo.Columns.Add(colIp);

            if (maxTables >= 1)
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName1",
                    HeaderText = @"字段1 Name",
                    Name = "colExtractFieldName1",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId1",
                    HeaderText = @"字段1 ID",
                    Name = "colExtractValueNameId1",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }
            else
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName1",
                    HeaderText = @"字段1 Name",
                    Name = "colExtractFieldName1",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId1",
                    HeaderText = @"字段1 ID",
                    Name = "colExtractValueNameId1",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }

            if (maxTables >= 2)
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName2",
                    HeaderText = @"字段2 Name",
                    Name = "colExtractFieldName2",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId2",
                    HeaderText = @"字段2 ID",
                    Name = "colExtractValueNameId2",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }
            else
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName2",
                    HeaderText = @"ExtractFieldName2",
                    Name = "colExtractFieldName2",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);
                
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameID2",
                    HeaderText = @"ExtractValueNameID2",
                    Name = "colExtractValueNameID2",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }

            if (maxTables >= 3)
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName3",
                    HeaderText = @"字段3 Name",
                    Name = "colExtractFieldName3",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId3",
                    HeaderText = @"字段3 ID",
                    Name = "colExtractValueNameId3",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }
            else
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName3",
                    HeaderText = @"ExtractFieldName3",
                    Name = "colExtractFieldName3",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId3",
                    HeaderText = @"ExtractValueNameId3",
                    Name = "colExtractValueNameId3",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);               
            }

            if (maxTables >= 4)
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName4",
                    HeaderText = @"字段4 Name",
                    Name = "colExtractFieldName4",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId4",
                    HeaderText = @"字段4 ID",
                    Name = "colExtractValueNameId4",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }
            else
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName4",
                    HeaderText = @"ExtractFieldName4",
                    Name = "colExtractFieldName4",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId4",
                    HeaderText = @"ExtractValueNameId4",
                    Name = "colExtractValueNameId4",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }

            if (maxTables >= 5)
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName5",
                    HeaderText = @"字段5 Name",
                    Name = "colExtractFieldName5",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId5",
                    HeaderText = @"字段5 ID",
                    Name = "colExtractValueNameId5",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }
            else
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName5",
                    HeaderText = @"ExtractFieldName5",
                    Name = "colExtractFieldName5",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId5",
                    HeaderText = @"ExtractValueNameId5",
                    Name = "colExtractValueNameId5",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }

            if (maxTables >= 6)
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName6",
                    HeaderText = @"字段6 Name",
                    Name = "colExtractFieldName6",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId6",
                    HeaderText = @"字段6 ID",
                    Name = "colExtractValueNameId6",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }
            else
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName6",
                    HeaderText = @"ExtractFieldName6",
                    Name = "colExtractFieldName6",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId6",
                    HeaderText = @"ExtractValueNameId6",
                    Name = "colExtractValueNameId6",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }
            if (maxTables >= 7)
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName7",
                    HeaderText = @"字段7 Name",
                    Name = "colExtractFieldName7",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId7",
                    HeaderText = @"字段7 ID",
                    Name = "colExtractValueNameId7",
                    Resizable = DataGridViewTriState.True,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }
            else
            {
                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractFieldName7",
                    HeaderText = @"ExtractFieldName7",
                    Name = "colExtractFieldName7",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);

                colIp = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = "ExtractValueNameId7",
                    HeaderText = @"ExtractValueNameId7",
                    Name = "colExtractValueNameId7",
                    Resizable = DataGridViewTriState.True,
                    Visible = false,
                };
                dgvDataTableInfo.Columns.Add(colIp);
            }

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = @"ID",
                Name = "colId",
                Resizable = DataGridViewTriState.True,
                Visible = false,
            };
            dgvDataTableInfo.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ModuleNo",
                HeaderText = @"ModuleNo",
                Name = "colModuleNo",
                Resizable = DataGridViewTriState.True,
                Visible = false,
            };
            dgvDataTableInfo.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ChannelId",
                HeaderText = @"ChannelId",
                Name = "colChannelId",
                Resizable = DataGridViewTriState.True,
                Visible = false,
            };
            dgvDataTableInfo.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AcqTime",
                HeaderText = @"AcqTime",
                Name = "colAcqTime",
                Resizable = DataGridViewTriState.True,
                Visible = false,
            };
            dgvDataTableInfo.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SensorID",
                HeaderText = @"SensorID",
                Name = "colSensorID",
                Resizable = DataGridViewTriState.True,
                Visible = false,
            };
            dgvDataTableInfo.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Reserved2",
                HeaderText = @"Reserved2",
                Name = "colReserved2",
                Resizable = DataGridViewTriState.True,
                Visible = false,
            };
            dgvDataTableInfo.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ValueType",
                HeaderText = @"ValueType",
                Name = "colValueType",
                Resizable = DataGridViewTriState.True,
                Visible = false,
            };
            dgvDataTableInfo.Columns.Add(colIp);

            this.dgvDataTableInfo.AutoGenerateColumns = false;  
        }

        private void DataBaseAddColumns()
        {
            DataGridViewTextBoxColumn colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataBaseCode",
                HeaderText = @"数据库名",
                Name = "colDataBaseCode",
                Resizable = DataGridViewTriState.True
            };
            dgvDataBase.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Location",
                HeaderText = @"数据库路径",
                Name = "colLocation",
                Resizable = DataGridViewTriState.True
            };
            dgvDataBase.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataBaseType",
                HeaderText = @"数据库类型",
                Name = "colDataBaseType",
                Resizable = DataGridViewTriState.True
            };
            dgvDataBase.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ID",
                HeaderText = @"数据库ID",
                Name = "colID",
                Resizable = DataGridViewTriState.True,
            };
            dgvDataBase.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "UserId",
                HeaderText = @"UserId",
                Name = "colUserId",
                Resizable = DataGridViewTriState.True,
                Visible = false
            };
            dgvDataBase.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Password",
                HeaderText = @"Password",
                Name = "colPassword",
                Resizable = DataGridViewTriState.True,
                Visible = false
            };
            dgvDataBase.Columns.Add(colIp);

        }

        public void ConfigTableAddColumns()
        {
            DataGridViewTextBoxColumn colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "SensorID",
                HeaderText = @"传感器ID",
                Name = "colSensorID",
                Resizable = DataGridViewTriState.True
            };
            gdvConfigTable.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ModuleNo",
                HeaderText = @"模块号",
                Name = "colModuleNo",
                Resizable = DataGridViewTriState.True
            };
            gdvConfigTable.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ChannelId",
                HeaderText = @"通道号",
                Name = "colChannelId",
                Resizable = DataGridViewTriState.True
            };
            gdvConfigTable.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "OtherFlag",
                HeaderText = @"标记位",
                Name = "colOtherFlag",
                Resizable = DataGridViewTriState.True,
            };
            gdvConfigTable.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TableName",
                HeaderText = @"表名",
                Name = "colTableName",
                Resizable = DataGridViewTriState.True,
            };
            gdvConfigTable.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DataBaseId",
                HeaderText = @"数据库ID",
                Name = "colDataBaseId",
                Resizable = DataGridViewTriState.True,
            };
            gdvConfigTable.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ID",
                HeaderText = @"ID",
                Name = "colID",
                Resizable = DataGridViewTriState.True,
                Visible = false,
            };
            gdvConfigTable.Columns.Add(colIp);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int m = 0;
            bool flag = false;
            for (m = 0; m < dgvDataBase.Rows.Count; m++)
            {
                if (dgvDataBase.Rows[m].Selected)
                {
                    string Id = dgvDataBase.Rows[m].Cells[0].Value.ToString();
                    DialogResult result = MessageBox.Show(@"是否删除？", @"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    {
                        if (result == DialogResult.Yes)
                        {
                            if (DataBaseNameDic.GetDataBaseNameDic().Delete(Convert.ToInt32(Id)))
                            {
                                bool deleResult = false;
                                //同时删除与该数据库有关的配置

                                //删除配置表的信息
                                List<ConfigTable> listConfigTable = ConfigTableDic.GetConfigTableDic().SelectList();
                                for (int i = 0; i < listConfigTable.Count; i++)
                                {
                                    if (listConfigTable[i].DataBaseId == Convert.ToInt32(Id))
                                    {
                                        deleResult = ConfigTableDic.GetConfigTableDic().Delete(Convert.ToInt32(listConfigTable[i].ID));
                                    }
                                }
                                
                                //删除数据表的信息
                                List<TableFieldInfo> listTable = TableFieldInfoDic.GetTableFieldInfoDic().GetAllTableFieldInfos();
                                for (int i = 0; i < listTable.Count; i++)
                                {
                                    if (listTable[i].DataBaseId == Convert.ToInt32(Id))
                                    {
                                        deleResult = (deleResult && TableFieldInfoDic.GetTableFieldInfoDic().Delete(Convert.ToInt32(listTable[i].Id)));
                                    }
                                }

                               
                                MessageBox.Show(@"删除成功");
                                flag = true;
                            }
                            else
                            {
                                MessageBox.Show(@"删除失败");
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            if (m == dgvDataBase.Rows.Count && !flag)
            {
                MessageBox.Show(@"请选择要删除的数据库信息");
                m = 0;
            }
            dgvDataBase.Columns.Clear();
            dgvDataBase.DataSource = null;
            DataBaseAddColumns();
            dataGriewDataBaseSource = DataBaseNameDic.GetDataBaseNameDic().GetAllBaseNames();
            dgvDataBase.DataSource = dataGriewDataBaseSource;
            dgvDataBase.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            if (dgvDataBase.Rows.Count > 0)
            {
                dgvDataBase.Rows[0].Selected = false;
            }

            BingDataSource();


        }
    }
}
