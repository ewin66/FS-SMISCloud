#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="IPv4TextBox.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述： 扩展插件UI界面协定。
// 
//  创建标识：20140217 created by Win
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace Ascentium.Research.Windows.Forms.Components
{
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    /// The user control_ i pv 4 text box.
    /// </summary>
    public class IPv4TextBox : UserControl
    {
        private TextBox txtBoxIp1;
        private TextBox txtBoxIp2;
        private TextBox txtBoxIp3;
        private TextBox txtBoxIp4;
        private Label lblIpPoint1;
        private Label lblIpPoint2;
        private Label lblIpPoint3;

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
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            // 通过注册表获取系统默认字体
            string defaultFontFaceName = (string)Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\GRE_Initialize", "GUIFont.Facename", "SimSun");

            this.txtBoxIp1 = new TextBox();
            this.txtBoxIp2 = new TextBox();
            this.txtBoxIp3 = new TextBox();
            this.txtBoxIp4 = new TextBox();
            this.lblIpPoint1 = new Label();
            this.lblIpPoint2 = new Label();
            this.lblIpPoint3 = new Label();
            this.SuspendLayout();

            // 
            // txtBoxIp1
            // 
            this.txtBoxIp1.Location = new System.Drawing.Point(0, 3);
            this.txtBoxIp1.MaxLength = 3;
            this.txtBoxIp1.Name = "txtBoxIp1";
            this.txtBoxIp1.Size = new System.Drawing.Size(37, 21);
            this.txtBoxIp1.TabIndex = 0;
            this.txtBoxIp1.TextAlign = HorizontalAlignment.Center;
            this.txtBoxIp1.KeyPress += new KeyPressEventHandler(this.IPv4TextBox_KeyPress);
            
            // 
            // txtBoxIp2
            // 
            this.txtBoxIp2.Location = new System.Drawing.Point(57, 3);
            this.txtBoxIp2.MaxLength = 3;
            this.txtBoxIp2.Name = "txtBoxIp2";
            this.txtBoxIp2.Size = new System.Drawing.Size(37, 21);
            this.txtBoxIp2.TabIndex = 1;
            this.txtBoxIp2.TextAlign = HorizontalAlignment.Center;
            this.txtBoxIp2.KeyPress += new KeyPressEventHandler(this.IPv4TextBox_KeyPress);
            
            // 
            // txtBoxIp3
            // 
            this.txtBoxIp3.Location = new System.Drawing.Point(114, 3);
            this.txtBoxIp3.MaxLength = 3;
            this.txtBoxIp3.Name = "txtBoxIp3";
            this.txtBoxIp3.Size = new System.Drawing.Size(37, 21);
            this.txtBoxIp3.TabIndex = 2;
            this.txtBoxIp3.TextAlign = HorizontalAlignment.Center;
            this.txtBoxIp3.KeyPress += new KeyPressEventHandler(this.IPv4TextBox_KeyPress);
            
            // 
            // txtBoxIp4
            // 
            this.txtBoxIp4.Location = new System.Drawing.Point(171, 3);
            this.txtBoxIp4.MaxLength = 3;
            this.txtBoxIp4.Name = "txtBoxIp4";
            this.txtBoxIp4.Size = new System.Drawing.Size(37, 21);
            this.txtBoxIp4.TabIndex = 3;
            this.txtBoxIp4.TextAlign = HorizontalAlignment.Center;
            this.txtBoxIp4.KeyPress += new KeyPressEventHandler(this.IPv4TextBox_KeyPress);
            
            // 
            // lblIpPoint1
            // 
            this.lblIpPoint1.AutoSize = true;
            this.lblIpPoint1.Font = new System.Drawing.Font(defaultFontFaceName, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblIpPoint1.Location = new System.Drawing.Point(39, 5);
            this.lblIpPoint1.Name = "lblIpPoint1";
            this.lblIpPoint1.Size = new System.Drawing.Size(16, 16);
            this.lblIpPoint1.TabIndex = 4;
            this.lblIpPoint1.Text = @".";
            this.lblIpPoint1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // 
            // lblIpPoint2
            // 
            this.lblIpPoint2.AutoSize = true;
            this.lblIpPoint2.Font = new System.Drawing.Font(defaultFontFaceName, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblIpPoint2.Location = new System.Drawing.Point(96, 5);
            this.lblIpPoint2.Name = "lblIpPoint2";
            this.lblIpPoint2.Size = new System.Drawing.Size(16, 16);
            this.lblIpPoint2.TabIndex = 5;
            this.lblIpPoint2.Text = @".";
            this.lblIpPoint2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // 
            // lblIpPoint3
            // 
            this.lblIpPoint3.AutoSize = true;
            this.lblIpPoint3.Font = new System.Drawing.Font(defaultFontFaceName, 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblIpPoint3.Location = new System.Drawing.Point(153, 5);
            this.lblIpPoint3.Name = "lblIpPoint3";
            this.lblIpPoint3.Size = new System.Drawing.Size(16, 16);
            this.lblIpPoint3.TabIndex = 6;
            this.lblIpPoint3.Text = @".";
            this.lblIpPoint3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            
            // 
            // UserControl_IPv4TextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.lblIpPoint3);
            this.Controls.Add(this.lblIpPoint2);
            this.Controls.Add(this.lblIpPoint1);
            this.Controls.Add(this.txtBoxIp4);
            this.Controls.Add(this.txtBoxIp3);
            this.Controls.Add(this.txtBoxIp2);
            this.Controls.Add(this.txtBoxIp1);
            this.Name = "UserControlIPv4TextBox";
            this.Size = new System.Drawing.Size(212, 30);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        public IPv4TextBox()
        {
            this.InitializeComponent();
        }

        private void IPv4TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char KeyChar = e.KeyChar;
            int TextLength = ((TextBox)sender).TextLength;

            if (KeyChar == '.' || KeyChar == '。' || KeyChar == ' ')
            {
                if ((((TextBox)sender).SelectedText.Length == 0) && (TextLength > 0) && (((TextBox)sender) != this.txtBoxIp4))
                {   // 进入下一个文本框
                    SendKeys.Send("{Tab}");
                }

                e.Handled = true;
            }

            if (Regex.Match(KeyChar.ToString(), "[0-9]").Success)
            {
                if (TextLength == 2)
                {
                    if (int.Parse(((TextBox)sender).Text + e.KeyChar.ToString()) > 255)
                    {
                        e.Handled = true;
                    }
                }
                else if (TextLength == 0)
                {
                    if (KeyChar == '0')
                    {
                        e.Handled = true;
                    }
                }
            }
            else
            {   // 回删操作
                if (KeyChar == '\b')
                {
                    if (TextLength == 0)
                    {
                        if (((TextBox)sender) != this.txtBoxIp1)
                        {   // 回退到上一个文本框 Shift+Tab
                            SendKeys.Send("+{TAB}{End}");
                        }
                    }
                }
                else
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// string类型的IP地址
        /// </summary>
        override public string Text
        {
            get
            {
                return this.Value.ToString();
            }

            set
            {
                IPAddress address;
                if (IPAddress.TryParse(value, out address))
                {
                    byte[] bytes = address.GetAddressBytes();
                    for (int i = 1; i <= 4; i++)
                    {
                        this.Controls[@"txtBoxIp" + i.ToString()].Text = bytes[i - 1].ToString("D");
                    }
                }
            }
        }

        /// <summary>
        /// IP地址
        /// </summary>
        public IPAddress Value
        {
            get
            {
                IPAddress address;
                var ipstring = new StringBuilder();
                ipstring.Append(this.txtBoxIp1.Text)
                        .Append(".")
                        .Append(this.txtBoxIp2.Text)
                        .Append(".")
                        .Append(this.txtBoxIp3.Text)
                        .Append(".")
                        .Append(this.txtBoxIp4.Text);

                // string ipString = this.txtBoxIp1.Text + "." + this.txtBoxIp2.Text + "." + this.txtBoxIp3.Text + "."
                //                   + this.txtBoxIp4.Text;
                if (IPAddress.TryParse(ipstring.ToString(), out address))
                {
                    return address;
                }

                return new IPAddress(0);
            }

            set
            {
                byte[] bytes = value.GetAddressBytes();
                for (int i = 1; i <= 4; i++)
                {
                    this.Controls["txtBoxIp" + i.ToString()].Text = bytes[i - 1].ToString("D");
                }
            }
        }

        /// <summary>
        /// IP地址分类
        /// </summary>
        public IPType Type
        {
            get
            {
                byte[] bytes = this.Value.GetAddressBytes();
                int firstByte = bytes[0];
                if (firstByte < 128)
                {
                    return IPType.A;
                }
                else if (firstByte < 192)
                {
                    return IPType.B;
                }
                else if (firstByte < 224)
                {
                    return IPType.C;
                }
                else if (firstByte < 240)
                {
                    return IPType.D;
                }
                else
                {
                    return IPType.E;    // 保留做研究用
                }
            }
        }

        /// <summary>
        /// 控件的边框样式
        /// </summary>
        new public BorderStyle BorderStyle
        {
            get
            {
                return this.txtBoxIp1.BorderStyle;
            }

            set
            {
                for (int i = 1; i <= 4; i++)
                {
                    ((TextBox)this.Controls["txtBoxIp" + i.ToString()]).BorderStyle = value;
                }
            }
        }
    }

    /// <summary>
    /// The ip type.
    /// </summary>
    public enum IPType : byte
    {
        A,

        B,

        C,

        D,

        E
    };
}

