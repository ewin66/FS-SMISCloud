#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="SelectCacheDataPacketDal.cs" company="江苏飞尚安全监测咨询有限公司">
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
namespace FSDE.DAL.Select
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using FSDE.Commn;
    using FSDE.IDAL;
    using FSDE.Model.Config;

    using SqliteORM;

    public class CacheDataPacketDal : ICacheDataPackets
    {
        public IList<PacketsToSend> SelectAllCacheDataPackets()
        {
            using (DbConnection conn =new DbConnection())
            {
                using (TableAdapter<PacketsToSend> adapter= TableAdapter<PacketsToSend>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }

        public bool DeleteAllCacheDataPackets()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<PacketsToSend> adapter = TableAdapter<PacketsToSend>.Open())
                {
                    try
                    {
                        adapter.DeleteAll();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }

        public bool InsertCacheDataPackets(IList<byte[]> cacheDataPackets)
        {
            using (DbConnection conn = new DbConnection())
            {
                using (DbTransaction tran = DbTransaction.Open())
                {
                    try
                    {
                        foreach (byte[] bytes in cacheDataPackets)
                        {
                            string str = ValueHelper.Byte2HexStr(bytes, 0, bytes.Length);
                            PacketsToSend packet = new PacketsToSend { DataPacket = str.Trim() };
                            packet.Save();
                        }
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
            }
        }
    }
}