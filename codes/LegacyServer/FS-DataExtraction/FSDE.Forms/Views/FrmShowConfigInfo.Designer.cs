namespace FSDE.Forms
{
    partial class FrmShowConfigInfo
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
            this.dgvDataBase = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.gdvConfigTable = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.dgvDataTableInfo = new System.Windows.Forms.DataGridView();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataBase)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdvConfigTable)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataTableInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvDataBase
            // 
            this.dgvDataBase.AllowUserToAddRows = false;
            this.dgvDataBase.AllowUserToDeleteRows = false;
            this.dgvDataBase.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDataBase.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvDataBase.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDataBase.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDataBase.Location = new System.Drawing.Point(11, 39);
            this.dgvDataBase.Name = "dgvDataBase";
            this.dgvDataBase.ReadOnly = true;
            this.dgvDataBase.RowTemplate.Height = 23;
            this.dgvDataBase.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDataBase.Size = new System.Drawing.Size(616, 115);
            this.dgvDataBase.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "已添加的数据库信息：";
            // 
            // gdvConfigTable
            // 
            this.gdvConfigTable.AllowUserToAddRows = false;
            this.gdvConfigTable.AllowUserToDeleteRows = false;
            this.gdvConfigTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gdvConfigTable.Location = new System.Drawing.Point(11, 191);
            this.gdvConfigTable.Name = "gdvConfigTable";
            this.gdvConfigTable.ReadOnly = true;
            this.gdvConfigTable.RowTemplate.Height = 23;
            this.gdvConfigTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gdvConfigTable.Size = new System.Drawing.Size(616, 115);
            this.gdvConfigTable.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 167);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "配置表信息：";
            // 
            // dgvDataTableInfo
            // 
            this.dgvDataTableInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDataTableInfo.Location = new System.Drawing.Point(11, 346);
            this.dgvDataTableInfo.Name = "dgvDataTableInfo";
            this.dgvDataTableInfo.ReadOnly = true;
            this.dgvDataTableInfo.RowTemplate.Height = 23;
            this.dgvDataTableInfo.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDataTableInfo.Size = new System.Drawing.Size(616, 152);
            this.dgvDataTableInfo.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 328);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "数据表信息：";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(498, 161);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(101, 23);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "删除选定数据库";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // FrmShowConfigInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 541);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dgvDataTableInfo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gdvConfigTable);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvDataBase);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmShowConfigInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "已配置信息";
            this.Load += new System.EventHandler(this.FrmShowConfigInfo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataBase)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gdvConfigTable)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDataTableInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDataBase;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView gdvConfigTable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dgvDataTableInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDelete;
    }
}