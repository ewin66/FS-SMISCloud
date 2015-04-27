using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using FSDE.Dictionaries.config;
using FSDE.Model.Config;

namespace FSDE.Forms
{
    public partial class FrmProject : Form
    {

        public FrmProject()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.CheckInfo())
            {
                List<ProjectInfo> list = ProjectInfoDic.GetInstance().GetProjectInfos();
                if (list.Count > 0)
                {
                    MessageBox.Show(@"项目只能添加一个!请删除后再添加", @"ERROR");
                }
                else
                {
                    var projectSet = new ProjectInfo
                    {
                        ProjectName = proJectNameTextBox.Text,
                        ProjectCode = Convert.ToInt32(projectNumTextBox.Text),
                        TargetName = commPortComBox.Text,
                        IntervalTime = Convert.ToInt32(IntervalTimeComboBox.Text),
                        Id = 1,
                    };

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].IntervalTime == projectSet.IntervalTime
                            && list[i].ProjectCode == projectSet.ProjectCode
                            && list[i].ProjectName == projectSet.ProjectName
                            && list[i].TargetName == projectSet.TargetName)
                        {
                            MessageBox.Show(@"项目已存在", @"WARING");
                            return;
                        }
                    }

                    int issucceed = ProjectInfoDic.GetInstance().AddProjectSetInfo(projectSet);
                    if (issucceed > 0)
                    {
                        MessageBox.Show(@"添加成功", @"INFO");
                        BingDataSource();
                        proSetDataGridView.Rows[0].Selected = false;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(@"添加失败", @"INFO");
                    }
                }
                
            }
        }

        private bool CheckInfo()
        {
            if (string.IsNullOrEmpty(proJectNameTextBox.Text))
            {
                MessageBox.Show(@"请填写项目名称", @"WARING");
                return false;
            }
            if (string.IsNullOrEmpty(projectNumTextBox.Text))
            {
                MessageBox.Show(@"请填写项目编号", @"WARING");
                return false;
            }
            if (projectNumTextBox.Text.Length > 9)
            {
                MessageBox.Show(@"请重新填写项目编号，编号范围应在0-4294967296之间", @"WARING");
                return false;
            }
            if (string.IsNullOrEmpty(commPortComBox.Text))
            {
                MessageBox.Show(@"请填写串口号", @"WARING");
                return false;
            }

            if (string.IsNullOrEmpty(IntervalTimeComboBox.Text))
            {
                MessageBox.Show(@"请填写提取粒度", @"WARING");
                return false;
            }

            return true;
        }

        private void FrmProject_Load(object sender, EventArgs e)
        {
            InitializePortComBox();
            IniteLizeInital();
            BingDataSource();
            if (proSetDataGridView.Rows.Count > 0)
            {
                proSetDataGridView.Rows[0].Selected = false;
            }
            
        }

        public void IniteLizeInital()
        {
            IntervalTimeComboBox.Items.Add("1");
            IntervalTimeComboBox.Items.Add("5");
            IntervalTimeComboBox.Items.Add("10");
            IntervalTimeComboBox.Items.Add("15");
            IntervalTimeComboBox.Items.Add("30");
            IntervalTimeComboBox.Items.Add("60");
        }

        private void InitializePortComBox()
        {
            string[] ports = SerialPort.GetPortNames();
            foreach (string port in ports)
            {
                commPortComBox.Items.Add(port);
            }
        }

        private void BingDataSource()
        {
            proSetDataGridView.Columns.Clear();
            proSetDataGridView.DataSource = null;
            AddColumns();
            List<ProjectInfo> list = ProjectInfoDic.GetInstance().GetProjectInfos();
            proSetDataGridView.DataSource = list;
        }


        private void AddColumns()
        {
            var colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                HeaderText = @"Id",
                Name = "colId",
                Resizable = DataGridViewTriState.True,
                Visible = false
            };
            proSetDataGridView.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProjectName",
                HeaderText = @"项目名",
                Name = "colProjectName",
                Resizable = DataGridViewTriState.True
            };
            proSetDataGridView.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProjectCode",
                HeaderText = @"项目编号",
                Name = "colProjectCode",
                Resizable = DataGridViewTriState.True
            };
            proSetDataGridView.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TargetName",
                HeaderText = @"串口号",
                Name = "colTargetName",
                Resizable = DataGridViewTriState.True
            };
            proSetDataGridView.Columns.Add(colIp);

            colIp = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "IntervalTime",
                HeaderText = @"提取粒度",
                Name = "colIntervalTime",
                Resizable = DataGridViewTriState.True,
            };
            proSetDataGridView.Columns.Add(colIp);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (proJectNameTextBox.Text != ""
                && projectNumTextBox.Text != ""
                //&& commPortComBox.Text != ""
                && IntervalTimeComboBox.Text != "")
            {
                var projectSet = new ProjectInfo
                {
                    ProjectName = proJectNameTextBox.Text,
                    ProjectCode = Convert.ToInt32(projectNumTextBox.Text),
                    TargetName = commPortComBox.Text,
                    IntervalTime = Convert.ToInt32(IntervalTimeComboBox.Text),
                };
                List<ProjectInfo> list = ProjectInfoDic.GetInstance().GetProjectInfos();
                int id = 0;
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].IntervalTime == projectSet.IntervalTime
                        && list[i].ProjectCode == projectSet.ProjectCode
                        && list[i].ProjectName == projectSet.ProjectName
                        //&& list[i].TargetName == projectSet.TargetName
                        )
                    {
                        id = Convert.ToInt32(list[i].Id);
                        break;
                    }
                }
                
                if (id > 0)
                {
                    DialogResult result = MessageBox.Show(@"是否删除？", @"提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        ProjectInfoDic.GetInstance().Delete(id);
                        BingDataSource();
                        MessageBox.Show(@"删除成功!");
                        if (proSetDataGridView.Rows.Count > 0)
                        {
                            proSetDataGridView.Rows[0].Selected = false;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    MessageBox.Show(@"删除项不存在!");
                }

            }
            else
            {
                MessageBox.Show(@"信息不全，无法删除!");
            }
        }

        private void proSetDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < proSetDataGridView.Rows.Count; i++)
            {
                if (proSetDataGridView.Rows[i].Selected)
                {
                    string name = proSetDataGridView.Rows[i].Cells[1].Value.ToString();
                    string code = proSetDataGridView.Rows[i].Cells[2].Value.ToString();
                    string tarName = proSetDataGridView.Rows[i].Cells[3].Value.ToString();
                    string intime = proSetDataGridView.Rows[i].Cells[4].Value.ToString();
                    proJectNameTextBox.Text = name;
                    projectNumTextBox.Text = code;
                    commPortComBox.Text = tarName;
                    IntervalTimeComboBox.Text = intime;
                    break;
                }
            }
            
        }

    }
}
