namespace FSDE.Forms
{
    partial class FrmProject
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
            this.button2 = new System.Windows.Forms.Button();
            this.IntervalTimeComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.commPortComBox = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.proJectNameTextBox = new System.Windows.Forms.TextBox();
            this.proSetDataGridView = new System.Windows.Forms.DataGridView();
            this.projectNumTextBox = new Ascentium.Research.Windows.Forms.Components.TextBoxControl();
            ((System.ComponentModel.ISupportInitialize)(this.proSetDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(385, 247);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "删除";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // IntervalTimeComboBox
            // 
            this.IntervalTimeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.IntervalTimeComboBox.FormattingEnabled = true;
            this.IntervalTimeComboBox.Location = new System.Drawing.Point(313, 194);
            this.IntervalTimeComboBox.Name = "IntervalTimeComboBox";
            this.IntervalTimeComboBox.Size = new System.Drawing.Size(147, 20);
            this.IntervalTimeComboBox.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(242, 197);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 12);
            this.label4.TabIndex = 18;
            this.label4.Text = "提取粒度：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 197);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 17;
            this.label3.Text = "串口号：";
            // 
            // commPortComBox
            // 
            this.commPortComBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.commPortComBox.FormattingEnabled = true;
            this.commPortComBox.Location = new System.Drawing.Point(78, 194);
            this.commPortComBox.Name = "commPortComBox";
            this.commPortComBox.Size = new System.Drawing.Size(140, 20);
            this.commPortComBox.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(284, 247);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "保存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(242, 148);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "项目编号：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 148);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 13;
            this.label1.Text = "项目名称：";
            // 
            // proJectNameTextBox
            // 
            this.proJectNameTextBox.Location = new System.Drawing.Point(78, 145);
            this.proJectNameTextBox.Name = "proJectNameTextBox";
            this.proJectNameTextBox.Size = new System.Drawing.Size(140, 21);
            this.proJectNameTextBox.TabIndex = 1;
            // 
            // proSetDataGridView
            // 
            this.proSetDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.proSetDataGridView.Location = new System.Drawing.Point(21, 12);
            this.proSetDataGridView.MultiSelect = false;
            this.proSetDataGridView.Name = "proSetDataGridView";
            this.proSetDataGridView.ReadOnly = true;
            this.proSetDataGridView.RowTemplate.Height = 23;
            this.proSetDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.proSetDataGridView.Size = new System.Drawing.Size(439, 108);
            this.proSetDataGridView.TabIndex = 22;
            this.proSetDataGridView.SelectionChanged += new System.EventHandler(this.proSetDataGridView_SelectionChanged);
            // 
            // projectNumTextBox
            // 
            this.projectNumTextBox.InputModeStyle = Ascentium.Research.Windows.Forms.Components.InputMode.Number;
            this.projectNumTextBox.Location = new System.Drawing.Point(313, 145);
            this.projectNumTextBox.Name = "projectNumTextBox";
            this.projectNumTextBox.Size = new System.Drawing.Size(147, 21);
            this.projectNumTextBox.TabIndex = 23;
            // 
            // FrmProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 282);
            this.Controls.Add(this.projectNumTextBox);
            this.Controls.Add(this.proSetDataGridView);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.IntervalTimeComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.commPortComBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.proJectNameTextBox);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(499, 309);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(499, 309);
            this.Name = "FrmProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "项目信息设置";
            this.Load += new System.EventHandler(this.FrmProject_Load);
            ((System.ComponentModel.ISupportInitialize)(this.proSetDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox IntervalTimeComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox commPortComBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox proJectNameTextBox;
        private System.Windows.Forms.DataGridView proSetDataGridView;
        private Ascentium.Research.Windows.Forms.Components.TextBoxControl projectNumTextBox;
    }
}