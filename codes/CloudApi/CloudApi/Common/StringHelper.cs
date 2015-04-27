using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FreeSun.FS_SMISCloud.Server.CloudApi.Common
{
    using System.Text;

    using Newtonsoft.Json.Linq;

    /// <summary>
    /// 字符串生成帮助类
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Get message string.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>Message String.
        /// </returns>
        public static JObject GetMessageString(string message)
        {
            // return string.Format("{{ \"Message\" : \"{0}\" }}", message);
            return new JObject(new JProperty("Message", message));
        }

        /// <summary>
        /// 混淆字符串（例如：密码123456，混淆为1****6；密码liuxinyi,混淆为l*****i;密码：123，混淆为***）
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Confuse(string source)
        {
            if (source.Length <= 3)
            {
                return "***";
            }

            var sb = new StringBuilder(source.Length);

            for (int i = 0; i < source.Length; i++)
            {
                if (i == 0 || i == source.Length - 1)
                {
                    sb.Append(source[i]);
                }
                else
                {
                    sb.Append("*");
                }
            }

            return sb.ToString();
        }
    }
}