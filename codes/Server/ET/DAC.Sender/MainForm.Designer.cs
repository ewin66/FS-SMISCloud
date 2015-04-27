namespace DAC.Sender
{
    partial class MainForm
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
            this.textDtuCode = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.textMsg = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textDataPath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonStop = new System.Windows.Forms.Button();
            this.textBaudRate = new System.Windows.Forms.TextBox();
            this.comboBoxCom = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // textDtuCode
            // 
            this.textDtuCode.Location = new System.Drawing.Point(80, 11);
            this.textDtuCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textDtuCode.Name = "textDtuCode";
            this.textDtuCode.Size = new System.Drawing.Size(116, 23);
            this.textDtuCode.TabIndex = 0;
            this.textDtuCode.Text = "20120049";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "DTU ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "COM";
            // 
            // buttonStart
            // 
            this.buttonStart.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.buttonStart.Location = new System.Drawing.Point(441, 11);
            this.buttonStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(119, 76);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "开始";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(80, 101);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(581, 25);
            this.progressBar1.TabIndex = 5;
            // 
            // textMsg
            // 
            this.textMsg.Location = new System.Drawing.Point(80, 134);
            this.textMsg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textMsg.Multiline = true;
            this.textMsg.Name = "textMsg";
            this.textMsg.ReadOnly = true;
            this.textMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textMsg.Size = new System.Drawing.Size(581, 264);
            this.textMsg.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "进度";
            // 
            // textDataPath
            // 
            this.textDataPath.Location = new System.Drawing.Point(80, 71);
            this.textDataPath.Name = "textDataPath";
            this.textDataPath.Size = new System.Drawing.Size(204, 23);
            this.textDataPath.TabIndex = 8;
            this.textDataPath.Text = "D:\\123";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 73);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "数据目录";
            // 
            // buttonStop
            // 
            this.buttonStop.Location = new System.Drawing.Point(566, 11);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(95, 76);
            this.buttonStop.TabIndex = 10;
            this.buttonStop.Text = "停止";
            this.buttonStop.UseVisualStyleBackColor = true;
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // textBaudRate
            // 
            this.textBaudRate.Location = new System.Drawing.Point(212, 41);
            this.textBaudRate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.textBaudRate.Name = "textBaudRate";
            this.textBaudRate.Size = new System.Drawing.Size(72, 23);
            this.textBaudRate.TabIndex = 11;
            this.textBaudRate.Text = "9600";
            // 
            // comboBoxCom
            // 
            this.comboBoxCom.FormattingEnabled = true;
            this.comboBoxCom.Location = new System.Drawing.Point(80, 41);
            this.comboBoxCom.Name = "comboBoxCom";
            this.comboBoxCom.Size = new System.Drawing.Size(121, 25);
            this.comboBoxCom.TabIndex = 12;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 411);
            this.Controls.Add(this.comboBoxCom);
            this.Controls.Add(this.textBaudRate);
            this.Controls.Add(this.buttonStop);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textDataPath);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textMsg);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textDtuCode);
            this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DAC Sender";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textDtuCode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.TextBox textMsg;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textDataPath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.TextBox textBaudRate;
        private System.Windows.Forms.ComboBox comboBoxCom;
    }
}

