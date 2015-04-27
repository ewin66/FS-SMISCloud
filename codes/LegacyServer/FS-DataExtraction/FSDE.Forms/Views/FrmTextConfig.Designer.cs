namespace FSDE.Forms
{
    partial class FrmTextConfig
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
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.lblText = new System.Windows.Forms.Label();
            this.rdoShake = new System.Windows.Forms.RadioButton();
            this.rdoLight = new System.Windows.Forms.RadioButton();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnScan = new System.Windows.Forms.Button();
            this.rbVir = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // txtLocation
            // 
            this.txtLocation.Location = new System.Drawing.Point(12, 119);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(253, 21);
            this.txtLocation.TabIndex = 0;
            // 
            // lblText
            // 
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(12, 104);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(149, 12);
            this.lblText.TabIndex = 1;
            this.lblText.Text = "请填写文本数据所在路径：";
            // 
            // rdoShake
            // 
            this.rdoShake.AutoSize = true;
            this.rdoShake.Location = new System.Drawing.Point(54, 42);
            this.rdoShake.Name = "rdoShake";
            this.rdoShake.Size = new System.Drawing.Size(47, 16);
            this.rdoShake.TabIndex = 3;
            this.rdoShake.TabStop = true;
            this.rdoShake.Text = "振动";
            this.rdoShake.UseVisualStyleBackColor = true;
            // 
            // rdoLight
            // 
            this.rdoLight.AutoSize = true;
            this.rdoLight.Location = new System.Drawing.Point(54, 64);
            this.rdoLight.Name = "rdoLight";
            this.rdoLight.Size = new System.Drawing.Size(71, 16);
            this.rdoLight.TabIndex = 4;
            this.rdoLight.TabStop = true;
            this.rdoLight.Text = "光栅光纤";
            this.rdoLight.UseVisualStyleBackColor = true;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(257, 162);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(272, 119);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(60, 23);
            this.btnScan.TabIndex = 6;
            this.btnScan.Text = "浏览";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // rbVir
            // 
            this.rbVir.AutoSize = true;
            this.rbVir.Location = new System.Drawing.Point(54, 20);
            this.rbVir.Name = "rbVir";
            this.rbVir.Size = new System.Drawing.Size(59, 16);
            this.rbVir.TabIndex = 7;
            this.rbVir.TabStop = true;
            this.rbVir.Text = "振动ZJ";
            this.rbVir.UseVisualStyleBackColor = true;
            // 
            // FrmTextConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(349, 209);
            this.Controls.Add(this.rbVir);
            this.Controls.Add(this.btnScan);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.rdoLight);
            this.Controls.Add(this.rdoShake);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.txtLocation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmTextConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "文本数据提取配置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTextConfig_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.RadioButton rdoShake;
        private System.Windows.Forms.RadioButton rdoLight;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.RadioButton rbVir;
    }
}