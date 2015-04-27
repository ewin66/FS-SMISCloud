// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReloveCustomTransData.cs" company="江苏飞尚安全监测咨询有限公司">
//   Copyright (C) 2013 飞尚科技
//   //  版权所有。
// </copyright>
//  <summary>
//   文件功能描述：
//   创建标识：20131223
//   修改标识：
//   修改描述：
//   修改标识：
//   修改描述：
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Configuration;
using DataCenter.Model;

namespace DataCenter.WirelessCustomTransmission
{
    /// <summary>
    ///  解析自定的无线数据传输.
    /// </summary>
    public class ReloveCustomTransData 
    {
        private static Lazy<ReloveCustomTransData> Instance = new Lazy<ReloveCustomTransData>(() => new ReloveCustomTransData());
        private static string LCJDTUCODE = string.Empty;
        private ReloveCustomTransData()
        {
            LCJDTUCODE = ConfigurationManager.AppSettings["LCJDTUCode"];
        }

        public static ReloveCustomTransData GetInstance
        {
            get
            {
                return Instance.Value;
            }
        }


        /// <summary>
        /// The protocol order.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object ProtocolOrder(ReceiveDataInfo data)
        {
            if (String.CompareOrdinal(data.Sender, LCJDTUCODE)==0)
            {
               return new ReloveCustomTransLCJData().ProtocolOrder(data);
            }
            var packet = data.PackagesBytes;
            if (data.PackagesBytes != null && packet != null)
            {
                var dataTypeFactory = new DataTypeFactory();
                string dtuId = data.Sender;
                dataTypeFactory.ReloveTransData(dtuId, packet);
            }
            return null;
        }
    }
}