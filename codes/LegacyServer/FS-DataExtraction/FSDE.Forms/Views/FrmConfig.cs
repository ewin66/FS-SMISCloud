using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FSDE.Forms.Views;

namespace FSDE.Forms
{
    public partial class FrmConfig : Form
    {
        public FrmConfig()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var frmUnion = new FrmUnion();
            //frmUnion.closeF += thisclose;
            //frmUnion.ShowDialog();
            var frmUnion = new FrmUnionModify();
            frmUnion.closeF += thisclose;
            frmUnion.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var frmOther = new FrmOther();
            frmOther.closeFather += thisclose;
            frmOther.ShowDialog();
        }

        public void thisclose()
        {
            Close();
        }
    }
}
