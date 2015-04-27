namespace FSDE.Forms
{
    partial class FrmUnionModify
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
            this.testConnectButton = new System.Windows.Forms.Button();
            this.choosePathButton = new System.Windows.Forms.Button();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.unoinLabel = new System.Windows.Forms.Label();
            this.nextButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // testConnectButton
            // 
            this.testConnectButton.Location = new System.Drawing.Point(128, 83);
            this.testConnectButton.Name = "testConnectButton";
            this.testConnectButton.Size = new System.Drawing.Size(80, 23);
            this.testConnectButton.TabIndex = 14;
            this.testConnectButton.Text = "测试连接";
            this.testConnectButton.UseVisualStyleBackColor = true;
            this.testConnectButton.Click += new System.EventHandler(this.testConnectButton_Click);
            // 
            // choosePathButton
            // 
            this.choosePathButton.Location = new System.Drawing.Point(28, 83);
            this.choosePathButton.Name = "choosePathButton";
            this.choosePathButton.Size = new System.Drawing.Size(75, 23);
            this.choosePathButton.TabIndex = 13;
            this.choosePathButton.Text = "选择路径";
            this.choosePathButton.UseVisualStyleBackColor = true;
            this.choosePathButton.Click += new System.EventHandler(this.choosePathButton_Click);
            // 
            // pathTextBox
            // 
            this.pathTextBox.Location = new System.Drawing.Point(28, 41);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.Size = new System.Drawing.Size(309, 21);
            this.pathTextBox.TabIndex = 11;
            // 
            // unoinLabel
            // 
            this.unoinLabel.AutoSize = true;
            this.unoinLabel.Location = new System.Drawing.Point(26, 26);
            this.unoinLabel.Name = "unoinLabel";
            this.unoinLabel.Size = new System.Drawing.Size(89, 12);
            this.unoinLabel.TabIndex = 12;
            this.unoinLabel.Text = "数据库文件名：";
            // 
            // nextButton
            // 
            this.nextButton.Enabled = false;
            this.nextButton.Location = new System.Drawing.Point(262, 175);
            this.nextButton.Name = "nextButton";
            this.nextButton.Size = new System.Drawing.Size(75, 23);
            this.nextButton.TabIndex = 15;
            this.nextButton.Text = "完成";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // FrmUnionModify
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 210);
            this.Controls.Add(this.nextButton);
            this.Controls.Add(this.testConnectButton);
            this.Controls.Add(this.choosePathButton);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.unoinLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmUnionModify";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "统一采集软件配置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button testConnectButton;
        private System.Windows.Forms.Button choosePathButton;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Label unoinLabel;
        private System.Windows.Forms.Button nextButton;
    }
}