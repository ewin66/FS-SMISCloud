#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="DataTypeFactory.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140619 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System.Collections.Generic;

namespace DataCenter.WirelessCustomTransmission
{
    public class DataTypeFactory
    {

        public void ReloveTransData(string dtuid,byte[] bytes)
        {
            IReloveTransData reloveTransData;
            switch (bytes[4])
            {
                case 8: //振动数据
                        reloveTransData = new ReloveBigDataTransData();
                        reloveTransData.ReloveReceivedData(dtuid, bytes); 
                    break;
                case 13: // 光纤光栅
                case 17: // 杆件主应力
                case 3:
                case 4:
                case 5:
                case 9:         
                case 16:
                    reloveTransData = new ReloveGeneralNoCalcuTransData();
                    reloveTransData.ReloveReceivedData(dtuid,bytes);
                    break;
                case 1:
                case 2:
                case 6:
                case 11:
                case 12:
                case 14:
                    reloveTransData = new ReloveGeneralHasCalcuTransData(); // 包含计算数据字段的情况(默认一个计算值)
                    reloveTransData.ReloveReceivedData(dtuid,bytes);
                    break;
                default:
                    break;
            }
        }
    }
}