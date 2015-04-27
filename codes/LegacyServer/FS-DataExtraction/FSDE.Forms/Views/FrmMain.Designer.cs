namespace FSDE.Forms
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkStarup = new System.Windows.Forms.CheckBox();
            this.stopButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsstate = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.rtxExtracteInfo = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.rtxtransportInfo = new System.Windows.Forms.RichTextBox();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.rtxErrorInfo = new System.Windows.Forms.RichTextBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.projectToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.dataBsaeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.checkToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkStarup);
            this.panel1.Controls.Add(this.stopButton);
            this.panel1.Controls.Add(this.startButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(588, 73);
            this.panel1.TabIndex = 0;
            // 
            // chkStarup
            // 
            this.chkStarup.AutoSize = true;
            this.chkStarup.Location = new System.Drawing.Point(482, 22);
            this.chkStarup.Name = "chkStarup";
            this.chkStarup.Size = new System.Drawing.Size(96, 16);
            this.chkStarup.TabIndex = 2;
            this.chkStarup.Text = "开机自动运行";
            this.chkStarup.UseVisualStyleBackColor = true;
            this.chkStarup.CheckedChanged += new System.EventHandler(this.chkStarup_CheckedChanged);
            // 
            // stopButton
            // 
            this.stopButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.stopButton.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.stopButton.Image = ((System.Drawing.Image)(resources.GetObject("stopButton.Image")));
            this.stopButton.Location = new System.Drawing.Point(92, 3);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(70, 68);
            this.stopButton.TabIndex = 1;
            this.stopButton.UseVisualStyleBackColor = false;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // startButton
            // 
            this.startButton.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.startButton.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.startButton.Image = ((System.Drawing.Image)(resources.GetObject("startButton.Image")));
            this.startButton.Location = new System.Drawing.Point(10, 3);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(70, 68);
            this.startButton.TabIndex = 0;
            this.startButton.UseVisualStyleBackColor = false;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.statusStrip1);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.toolStrip1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 73);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(588, 356);
            this.panel2.TabIndex = 1;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsstate});
            this.statusStrip1.Location = new System.Drawing.Point(0, 334);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(588, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsstate
            // 
            this.tsstate.Name = "tsstate";
            this.tsstate.Size = new System.Drawing.Size(44, 17);
            this.tsstate.Text = "状态：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabControl1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 25);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(588, 331);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 17);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(582, 311);
            this.tabControl1.TabIndex = 3;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.rtxExtracteInfo);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(574, 285);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "提取信息";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // rtxExtracteInfo
            // 
            this.rtxExtracteInfo.BackColor = System.Drawing.SystemColors.InfoText;
            this.rtxExtracteInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxExtracteInfo.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rtxExtracteInfo.ForeColor = System.Drawing.SystemColors.Info;
            this.rtxExtracteInfo.Location = new System.Drawing.Point(3, 3);
            this.rtxExtracteInfo.Name = "rtxExtracteInfo";
            this.rtxExtracteInfo.Size = new System.Drawing.Size(568, 279);
            this.rtxExtracteInfo.TabIndex = 2;
            this.rtxExtracteInfo.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.rtxtransportInfo);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(574, 285);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "数据传输信息";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // rtxtransportInfo
            // 
            this.rtxtransportInfo.BackColor = System.Drawing.SystemColors.InfoText;
            this.rtxtransportInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxtransportInfo.ForeColor = System.Drawing.SystemColors.Info;
            this.rtxtransportInfo.Location = new System.Drawing.Point(3, 3);
            this.rtxtransportInfo.Name = "rtxtransportInfo";
            this.rtxtransportInfo.Size = new System.Drawing.Size(568, 279);
            this.rtxtransportInfo.TabIndex = 3;
            this.rtxtransportInfo.Text = "";
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.rtxErrorInfo);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(574, 285);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "错误信息";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // rtxErrorInfo
            // 
            this.rtxErrorInfo.BackColor = System.Drawing.SystemColors.InfoText;
            this.rtxErrorInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtxErrorInfo.ForeColor = System.Drawing.Color.Red;
            this.rtxErrorInfo.Location = new System.Drawing.Point(0, 0);
            this.rtxErrorInfo.Name = "rtxErrorInfo";
            this.rtxErrorInfo.Size = new System.Drawing.Size(574, 285);
            this.rtxErrorInfo.TabIndex = 3;
            this.rtxErrorInfo.Text = "";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripButton,
            this.dataBsaeToolStripButton,
            this.checkToolStripButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(588, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // projectToolStripButton
            // 
            this.projectToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("projectToolStripButton.Image")));
            this.projectToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.projectToolStripButton.Name = "projectToolStripButton";
            this.projectToolStripButton.Size = new System.Drawing.Size(76, 22);
            this.projectToolStripButton.Text = "项目配置";
            this.projectToolStripButton.Click += new System.EventHandler(this.projectToolStripButton_Click);
            // 
            // dataBsaeToolStripButton
            // 
            this.dataBsaeToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("dataBsaeToolStripButton.Image")));
            this.dataBsaeToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dataBsaeToolStripButton.Name = "dataBsaeToolStripButton";
            this.dataBsaeToolStripButton.Size = new System.Drawing.Size(88, 22);
            this.dataBsaeToolStripButton.Text = "数据库配置";
            this.dataBsaeToolStripButton.Click += new System.EventHandler(this.dataBsaeToolStripButton_Click);
            // 
            // checkToolStripButton
            // 
            this.checkToolStripButton.Image = ((System.Drawing.Image)(resources.GetObject("checkToolStripButton.Image")));
            this.checkToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.checkToolStripButton.Name = "checkToolStripButton";
            this.checkToolStripButton.Size = new System.Drawing.Size(88, 22);
            this.checkToolStripButton.Text = "查看已配置";
            this.checkToolStripButton.Click += new System.EventHandler(this.checkToolStripButton_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 429);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FrmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "统一提取软件";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton checkToolStripButton;
        private System.Windows.Forms.ToolStripButton dataBsaeToolStripButton;
        private System.Windows.Forms.ToolStripButton projectToolStripButton;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.RichTextBox rtxExtracteInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.RichTextBox rtxtransportInfo;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.RichTextBox rtxErrorInfo;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.CheckBox chkStarup;
        private System.Windows.Forms.ToolStripStatusLabel tsstate;
    }
}

