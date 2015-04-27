#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SetPacketTime.cs" company="江苏飞尚安全监测咨询有限公司">
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

    /// <summary>
    /// 设置封包间隔
    /// </summary>
    public class SetByteInterval:ATCommand
    {
        public ushort ByteInterval { get; set; }

        /// <summary>
        /// 设置封包间隔。
        /// </summary>
        /// <param name="packtime">{byteInterval: [int 1-65523,毫秒] }</param>
        public SetByteInterval(string jsonStr)
        {
            var anonymous = new { byteInterval = 200 };
            var jsonobj = JsonConvert.DeserializeAnonymousType(jsonStr, anonymous);
            this.ByteInterval = (byte)jsonobj.byteInterval;
        }

        public string ToATString()
        {
            return string.Format("AT+BYTEINT={0}\r", this.ByteInterval);
        }
    }
}