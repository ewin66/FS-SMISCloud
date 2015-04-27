#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="CacheDataPacketBll.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace FSDE.BLL.Select
{
    using System.Collections.Generic;

    using FSDE.DALFactory;
    using FSDE.IDAL;
    using FSDE.Model.Config;

    public class CacheDataPacketBll
    {
        private static readonly ICacheDataPackets Dal = DataAccess.CreateCacheDataPacketsDal();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<PacketsToSend> SelectAllCacheDataPackets()
        {
            return Dal.SelectAllCacheDataPackets();
        }

        public bool DeleteAllCacheDataPackets()
        {
            return Dal.DeleteAllCacheDataPackets();
        }

        public bool InsertCacheDataPackets(IList<byte[]> cacheDataPackets)
        {
            return Dal.InsertCacheDataPackets(cacheDataPackets);
        }
    }
}