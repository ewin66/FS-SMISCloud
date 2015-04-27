#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="PacketsToSend.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140611 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Dictionaries.config
{
    using System.Collections.Generic;

    using FSDE.BLL.Select;
    using FSDE.Commn;

    public class PacketsToSendDic
    {
        private Dictionary<int, string> packets2Send; 
        private PacketsToSendDic()
        {
            if (null == packets2Send)
            {
                packets2Send = new Dictionary<int, string>();
            }
        }

        private static PacketsToSendDic packetsToSendDic=new PacketsToSendDic();

        public static PacketsToSendDic GetPacketsToSendDic()
        {
            return packetsToSendDic;
        }

        public List<byte[]> GetAllPackets2Send()
        {
            List<byte[]> list =new List<byte[]>();
            foreach (string val in packets2Send.Values)
            {
                byte[] bytes = ValueHelper.Str2HexByte(val);
                list.Add(bytes);
            }
            return list;
        }

        public void DeleteAll()
        {
            CacheDataPacketBll bll =new CacheDataPacketBll();
            bll.DeleteAllCacheDataPackets();
            this.packets2Send.Clear();
        }

        public void InsertAllPackets(IList<byte[]> packets)
        {
            CacheDataPacketBll bll = new CacheDataPacketBll();
            bll.InsertCacheDataPackets(packets);
        }

    }
}