namespace FSDE.Forms
{
    partial class FrmUnion
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.nextButton = new System.Windows.Forms.Button();
            this.filDataCheckBox = new System.Windows.Forms.CheckBox();
            this.divGroupCheckBox = new System.Windows.Forms.CheckBox();
            this.testConnectButton = new System.Windows.Forms.Button();
            this.choosePathButton = new System.Windows.Forms.Button();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.unoinLabel = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRemoveGroup = new System.Windows.Forms.Button();
            this.divCalCheckBox = new System.Windows.Forms.CheckBox();
            this.btnNext = new System.Windows.Forms.Button();
            this.addGroupButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.choSensorListBox = new System.Windows.Forms.ListBox();
            this.filOrChoGroupComboBox = new System.Windows.Forms.ComboBox();
            this.sensorListBox = new System.Windows.Forms.ListBox();
            this.safeTypeComboBox = new System.Windows.Forms.ComboBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.finishButton = new System.Windows.Forms.Button();
            this.filDataLabel = new System.Windows.Forms.Label();
            this.filDateComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbSafeFactor = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(2, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(578, 404);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.nextButton);
            this.tabPage1.Controls.Add(this.filDataCheckBox);
            this.tabPage1.Controls.Add(this.divGroupCheckBox);
            this.tabPage1.Controls.Add(this.testConnectButton);
            this.tabPage1.Controls.Add(this.choosePathButton);
            this.tabPage1.Controls.Add(this.pathTextBox);
            this.tabPage1.Controls.Add(this.unoinLabel);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(570, 378);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "数据库选择";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // nextButton
            // 
            this.nextButton.Enabled = false;
            this.nextButton.Location = new System.Drawing.Point(478, 335);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 13;
            this.nextButton.Text = "完成";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // filDataCheckBox
            // 
            this.filDataCheckBox.AutoSize = true;
            this.filDataCheckBox.Location = new System.Drawing.Point(41, 204);
            this.filDataCheckBox.Name = "filDataCheckBox";
            this.filDataCheckBox.Size = new System.Drawing.Size(132, 16);
            this.filDataCheckBox.TabIndex = 3;
            this.filDataCheckBox.Text = "是否对数据进行过滤";
            this.filDataCheckBox.UseVisualStyleBackColor = true;
            this.filDataCheckBox.CheckedChanged += new System.EventHandler(this.filDataCheckBox_CheckedChanged);
            // 
            // divGroupCheckBox
            // 
            this.divGroupCheckBox.AutoSize = true;
            this.divGroupCheckBox.Location = new System.Drawing.Point(41, 164);
            this.divGroupCheckBox.Name = "divGroupCheckBox";
            this.divGroupCheckBox.Size = new System.Drawing.Size(108, 16);
            this.divGroupCheckBox.TabIndex = 2;
            this.divGroupCheckBox.Text = "是否有分组计算";
            this.divGroupCheckBox.UseVisualStyleBackColor = true;
            this.divGroupCheckBox.CheckedChanged += new System.EventHandler(this.divGroupCheckBox_CheckedChanged);
            // 
            // testConnectButton
            // 
            this.testConnectButton.Location = new System.Drawing.Point(426, 117);
            this.testConnectButton.Name = "testConnectButton";
            this.testConnectButton.Size = new System.Drawing.Size(75, 23);
            this.testConnectButton.TabIndex = 10;
            this.testConnectButton.Text = "测试连接";
            this.testConnectButton.UseVisualStyleBackColor = true;
            this.testConnectButton.Click += new System.EventHandler(this.testConnectButton_Click);
            // 
            // choosePathButton
            // 
            this.choosePathButton.Location = new System.Drawing.Point(426, 67);
            this.choosePathButton.Name = "choosePathButton";
            this.choosePathButton.Size = new System.Drawing.Size(75, 23);
            this.choosePathButton.TabIndex = 9;
            this.choosePathButton.Text = "选择路径";
            this.choosePathButton.UseVisualStyleBackColor = true;
            this.choosePathButton.Click += new System.EventHandler(this.choosePathButton_Click);
            // 
            // pathTextBox
            // 
            this.pathTextBox.Location = new System.Drawing.Point(41, 69);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(348, 21);
            this.pathTextBox.TabIndex = 1;
            // 
            // unoinLabel
            // 
            this.unoinLabel.AutoSize = true;
            this.unoinLabel.Location = new System.Drawing.Point(39, 37);
            this.unoinLabel.Name = "unoinLabel";
            this.unoinLabel.Size = new System.Drawing.Size(89, 12);
            this.unoinLabel.TabIndex = 7;
            this.unoinLabel.Text = "数据库文件名：";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label3);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Controls.Add(this.btnRemoveGroup);
            this.tabPage2.Controls.Add(this.divCalCheckBox);
            this.tabPage2.Controls.Add(this.btnNext);
            this.tabPage2.Controls.Add(this.addGroupButton);
            this.tabPage2.Controls.Add(this.removeButton);
            this.tabPage2.Controls.Add(this.addButton);
            this.tabPage2.Controls.Add(this.choSensorListBox);
            this.tabPage2.Controls.Add(this.filOrChoGroupComboBox);
            this.tabPage2.Controls.Add(this.sensorListBox);
            this.tabPage2.Controls.Add(this.safeTypeComboBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(570, 378);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "监测因素设置";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(269, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "请填写或选择分组：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 15;
            this.label2.Text = "请选择监测因素：";
            // 
            // btnRemoveGroup
            // 
            this.btnRemoveGroup.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRemoveGroup.Location = new System.Drawing.Point(472, 81);
            this.btnRemoveGroup.Name = "btnRemoveGroup";
            this.btnRemoveGroup.Size = new System.Drawing.Size(62, 23);
            this.btnRemoveGroup.TabIndex = 14;
            this.btnRemoveGroup.Text = "删除组";
            this.btnRemoveGroup.UseVisualStyleBackColor = true;
            this.btnRemoveGroup.Click += new System.EventHandler(this.btnRemoveGroup_Click);
            // 
            // divCalCheckBox
            // 
            this.divCalCheckBox.AutoSize = true;
            this.divCalCheckBox.Location = new System.Drawing.Point(472, 129);
            this.divCalCheckBox.Name = "divCalCheckBox";
            this.divCalCheckBox.Size = new System.Drawing.Size(72, 16);
            this.divCalCheckBox.TabIndex = 6;
            this.divCalCheckBox.Text = "分组计算";
            this.divCalCheckBox.UseVisualStyleBackColor = true;
            this.divCalCheckBox.CheckedChanged += new System.EventHandler(this.divCalCheckBox_CheckedChanged);
            // 
            // btnNext
            // 
            this.btnNext.Location = new System.Drawing.Point(472, 331);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 7;
            this.btnNext.Text = "下一步";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // addGroupButton
            // 
            this.addGroupButton.Enabled = false;
            this.addGroupButton.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.addGroupButton.Location = new System.Drawing.Point(472, 43);
            this.addGroupButton.Name = "addGroupButton";
            this.addGroupButton.Size = new System.Drawing.Size(62, 23);
            this.addGroupButton.TabIndex = 5;
            this.addGroupButton.Text = "添加组";
            this.addGroupButton.UseVisualStyleBackColor = true;
            this.addGroupButton.Click += new System.EventHandler(this.addGroupButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Enabled = false;
            this.removeButton.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.removeButton.Location = new System.Drawing.Point(212, 175);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(53, 22);
            this.removeButton.TabIndex = 4;
            this.removeButton.Text = "<<";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addButton
            // 
            this.addButton.Enabled = false;
            this.addButton.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.addButton.Location = new System.Drawing.Point(212, 129);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(53, 22);
            this.addButton.TabIndex = 3;
            this.addButton.Text = ">>";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // choSensorListBox
            // 
            this.choSensorListBox.FormattingEnabled = true;
            this.choSensorListBox.ItemHeight = 12;
            this.choSensorListBox.Location = new System.Drawing.Point(271, 98);
            this.choSensorListBox.Name = "choSensorListBox";
            this.choSensorListBox.Size = new System.Drawing.Size(177, 256);
            this.choSensorListBox.TabIndex = 13;
            this.choSensorListBox.SelectedIndexChanged += new System.EventHandler(this.choSensorListBox_SelectedIndexChanged);
            // 
            // filOrChoGroupComboBox
            // 
            this.filOrChoGroupComboBox.FormattingEnabled = true;
            this.filOrChoGroupComboBox.Location = new System.Drawing.Point(271, 43);
            this.filOrChoGroupComboBox.Name = "filOrChoGroupComboBox";
            this.filOrChoGroupComboBox.Size = new System.Drawing.Size(177, 20);
            this.filOrChoGroupComboBox.TabIndex = 2;
            this.filOrChoGroupComboBox.Text = "填写或选择分组";
            this.filOrChoGroupComboBox.SelectionChangeCommitted += new System.EventHandler(this.filOrChoGroupComboBox_SelectionChangeCommitted);
            this.filOrChoGroupComboBox.TextChanged += new System.EventHandler(this.filOrChoGroupComboBox_TextChanged);
            // 
            // sensorListBox
            // 
            this.sensorListBox.FormattingEnabled = true;
            this.sensorListBox.ItemHeight = 12;
            this.sensorListBox.Location = new System.Drawing.Point(23, 98);
            this.sensorListBox.Name = "sensorListBox";
            this.sensorListBox.Size = new System.Drawing.Size(183, 256);
            this.sensorListBox.TabIndex = 11;
            this.sensorListBox.SelectedIndexChanged += new System.EventHandler(this.sensorListBox_SelectedIndexChanged);
            // 
            // safeTypeComboBox
            // 
            this.safeTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.safeTypeComboBox.FormattingEnabled = true;
            this.safeTypeComboBox.Location = new System.Drawing.Point(23, 43);
            this.safeTypeComboBox.Name = "safeTypeComboBox";
            this.safeTypeComboBox.Size = new System.Drawing.Size(183, 20);
            this.safeTypeComboBox.TabIndex = 1;
            this.safeTypeComboBox.SelectionChangeCommitted += new System.EventHandler(this.safeTypeComboBox_SelectionChangeCommitted);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.finishButton);
            this.tabPage3.Controls.Add(this.filDataLabel);
            this.tabPage3.Controls.Add(this.filDateComboBox);
            this.tabPage3.Controls.Add(this.label1);
            this.tabPage3.Controls.Add(this.cmbSafeFactor);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(570, 378);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "数据过滤设置";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // finishButton
            // 
            this.finishButton.Location = new System.Drawing.Point(467, 320);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 3;
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // filDataLabel
            // 
            this.filDataLabel.AutoSize = true;
            this.filDataLabel.Location = new System.Drawing.Point(91, 170);
            this.filDataLabel.Name = "filDataLabel";
            this.filDataLabel.Size = new System.Drawing.Size(89, 12);
            this.filDataLabel.TabIndex = 8;
            this.filDataLabel.Text = "数据过滤方式：";
            // 
            // filDateComboBox
            // 
            this.filDateComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filDateComboBox.FormattingEnabled = true;
            this.filDateComboBox.Location = new System.Drawing.Point(186, 170);
            this.filDateComboBox.Name = "filDateComboBox";
            this.filDateComboBox.Size = new System.Drawing.Size(267, 20);
            this.filDateComboBox.TabIndex = 2;
            this.filDateComboBox.DropDown += new System.EventHandler(this.filDateComboBox_DropDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(91, 112);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "安全监测因素：";
            // 
            // cmbSafeFactor
            // 
            this.cmbSafeFactor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSafeFactor.FormattingEnabled = true;
            this.cmbSafeFactor.Location = new System.Drawing.Point(186, 109);
            this.cmbSafeFactor.Name = "cmbSafeFactor";
            this.cmbSafeFactor.Size = new System.Drawing.Size(267, 20);
            this.cmbSafeFactor.TabIndex = 1;
            this.cmbSafeFactor.DropDown += new System.EventHandler(this.cmbSafeFactor_DropDown);
            // 
            // FrmUnion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(585, 405);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "FrmUnion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmUnion";
            this.Load += new System.EventHandler(this.FrmUnion_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button nextButton;
        public System.Windows.Forms.CheckBox filDataCheckBox;
        public System.Windows.Forms.CheckBox divGroupCheckBox;
        private System.Windows.Forms.Button testConnectButton;
        private System.Windows.Forms.Button choosePathButton;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Label unoinLabel;
        private System.Windows.Forms.CheckBox divCalCheckBox;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button addGroupButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.ListBox choSensorListBox;
        private System.Windows.Forms.ComboBox filOrChoGroupComboBox;
        private System.Windows.Forms.ListBox sensorListBox;
        private System.Windows.Forms.ComboBox safeTypeComboBox;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.Label filDataLabel;
        private System.Windows.Forms.ComboBox filDateComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbSafeFactor;
        private System.Windows.Forms.Button btnRemoveGroup;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
    }
}