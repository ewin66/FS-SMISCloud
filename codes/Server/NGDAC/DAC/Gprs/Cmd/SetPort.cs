#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SetPort.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140909 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGDAC.Gprs.Cmd
{
    using Newtonsoft.Json;

    public class SetPort:ATCommand
    {
        public ushort Port { get; set; }
        public ushort Index { get; set; }
        /// <summary>
        /// 设置IP【x】端口号
        /// </summary>
        /// <param name="jsonStr">{ <index:[int, 索引号],> port:[int,端口号]} </param>
        public SetPort(string jsonStr)
        {
            var anonymous = new { index = 1, port = 5000 };
            var jsonobj = JsonConvert.DeserializeAnonymousType(jsonStr, anonymous);
             this.Index = (byte)jsonobj.index;
             if (this.Index <= 0)
                 this.Index = 1;
            this.Port = (ushort) jsonobj.port;
        }

        public string ToATString()
        {
            if (this.Index == 1)
            {
                return string.Format("AT+PORT={0}\r", this.Port);
            }
            else
            {
                return string.Format("AT+PORT{0}={1}\r", this.Index, this.Port);
            }
        }
    }
}