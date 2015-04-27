namespace DataCenter
{
    using System.Windows.Forms;

    public class DtuInfo
    {
        /// <summary>
        /// 保存ＤＴＵ列表信息
        /// </summary>
        public ListViewItem Lvi;

        public bool IsOnline
        {
            get;
            set;
        }

        /// <summary>
        /// 为以后扩展功能保留的成员变量
        /// </summary>
        public object Obj;
    }
}