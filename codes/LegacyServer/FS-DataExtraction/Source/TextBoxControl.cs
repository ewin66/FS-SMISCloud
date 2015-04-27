
namespace Ascentium.Research.Windows.Forms.Components
{
    using System.Text.RegularExpressions;
    using System.Windows.Forms;

    /// <summary>
    /// The text box float.
    /// </summary>
    public class TextBoxControl : TextBox
    {
       /// <summary>
       /// 浮点数.
       /// </summary>
        private const string PatternFloatstr = @"^[\d|\.]|[\b]";

        /// <summary>
        /// 字母数字中文
        /// </summary>
        private const string PatternString = @"^[a-zA-Z|\d|\u4E00-\u9FA5]|[\b]";


        /// <summary>
        /// 字母数字
        /// </summary>
        private const string PatternStringX = @"^[a-zA-Z|\d]|[\b]";

        /// <summary>
        /// 数字
        /// </summary>
        private const string PatternNumber = @"^[\d]|[\b]";

        /// <summary>
        /// 字母数字中文符号空格等
        /// </summary>
        private const string PatternStringQ = @"^[a-zA-Z|\d|\u4E00-\u9FA5|uF900-uFAFF]*$|[\b]";

        /// <summary>
        /// Ip地址
        /// </summary>
        private const string PatternIpAddress = @"^(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5]).(d{1,2}|1dd|2[0-4]d|25[0-5])$";

        /// <summary>
        ///  输入框输入模式
        /// </summary>
        public InputMode InputModeStyle { get; set; }


        /// <summary>
        /// The on key press.
        /// </summary>
        /// <param name="e">
        /// The e.
        /// </param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            Regex rg;
            switch (this.InputModeStyle)
            {
                case InputMode.Float:
                    rg = new Regex(PatternFloatstr);
                    break;
                case InputMode.Number:
                    rg = new Regex(PatternNumber);
                    break;
                case InputMode.String:
                    rg = new Regex(PatternString);
                    break;
                case InputMode.StringQ:
                    rg = new Regex(PatternStringQ);
                    break;
                    case InputMode.StringX:
                    rg = new Regex(PatternStringX);
                    break;
                default:
                     rg = new Regex(PatternFloatstr);
                    break;
            }

            // rg= new Regex(PatternFloatstr);
            Match m = rg.Match(e.KeyChar.ToString().Trim());
            if (!m.Success || (base.Text.Contains(".") && e.KeyChar == '.'))
            {
                e.Handled = true;
            }


            if (base.Text.Length == 0 && e.KeyChar == '.')
            {
                e.Handled = true;
            }
        }
    }

    /// <summary>
    /// 输入模式
    /// </summary>
    public enum InputMode
    {
        /// <summary>
        /// 浮点数
        /// </summary>
        Float,

        /// <summary>
        /// 数字
        /// </summary>
        Number,

        /// <summary>
        /// 字母数字中文符号空格等
        /// </summary>
        StringQ,

        /// <summary>
        /// 字母数字中文
        /// </summary>
        String,

        /// <summary>
        /// 字母数字
        /// </summary>
        StringX

    }

}
