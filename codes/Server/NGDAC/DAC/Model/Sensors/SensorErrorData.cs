#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorErrorData.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150202 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGDAC.Model.Sensors
{
    using System;

    using FS.SMIS_Cloud.NGDAC.Node;
    using FS.SMIS_Cloud.NGDAC.Util;

    [Serializable]
    public class SensorErrorData:BasicSensorData
    {
        public SensorErrorData(uint sensorId,int errorcode)
        {
            this._themsValues = null;
            this.JsonResultData = ValueHelper.CreateJsonResultStr(sensorId, EnumHelper.GetDescription((Errors)errorcode));
        }

        public override double[] RawValues
        {
            get { return null; }
        }

        public override double[] PhyValues
        {
            get { return null; }
        }

        public override double[] CollectPhyValues
        {
            get { return null; }
        }

        public override void DropThemeValue(int colphyindex)
        {
            return;
        }
    }
}