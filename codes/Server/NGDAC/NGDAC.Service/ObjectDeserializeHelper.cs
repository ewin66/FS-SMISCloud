#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="ObjectDeserializeHelper.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150130 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGDAC
{
    using System;

    using FS.SMIS_Cloud.NGDAC.Model;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class ObjectDeserializeHelper
    {
        //TODO 
        public static SensorOperation Json2SensorOperation(string jsonstr)
        {
            JObject jsobj = JObject.Parse(jsonstr);
            var sen = JsonConvert.DeserializeObject<Sensor>(jsobj["Sensor"].ToString());
            uint oldSenId;
            UInt32.TryParse(jsobj["OldSensorId"].ToString(),out oldSenId);
            uint oldDtuId ;
            UInt32.TryParse(jsobj["OldDtuId"].ToString(), out oldDtuId);
            int opera;
            int.TryParse(jsobj["Action"].ToString(), out opera);
            
            var senopera = new SensorOperation
            {
                Sensor = sen,
                OldSensorId = oldSenId,
                OldDtuId = oldDtuId,
                Action = (Operations)opera
            };
            return senopera;
        }

    }
}