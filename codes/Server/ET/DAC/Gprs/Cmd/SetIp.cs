#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SetIp.cs" company="江苏飞尚安全监测咨询有限公司">
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

using Newtonsoft.Json;

namespace FS.SMIS_Cloud.DAC.Gprs.Cmd
{
    public class SetIP:ATCommand
    {
        public string IpAddress { get; set; }
        public byte IpIndex { get; set; }

        /// <summary>
        /// 设置IP【x】 
        /// </summary>
        /// <param name="jsonStr">{ <index:[int, 索引号],> ip:[string,IP地址]} </param>
        public SetIP(string jsonStr)
        {
            var anonymous = new { index=1, ip=""};
            var jsonobj = JsonConvert.DeserializeAnonymousType(jsonStr, anonymous);
            this.IpIndex = (byte)jsonobj.index;
            if (this.IpIndex<=0)
                this.IpIndex = 1;
            this.IpAddress = jsonobj.ip;
        }

        public string ToATString()
        {
            if (IpIndex == 1)
            {
                return string.Format("AT+IPAD={0}\r", this.IpAddress);
            }
            else
            {
                return string.Format("AT+IPAD{0}={1}\r", this.IpIndex, this.IpAddress);
            }
        }
    }
}