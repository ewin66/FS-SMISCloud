#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="ETTest.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20141107 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace NGET.Test
{
    using System;

    using FS.SMIS_Cloud.NGET;
    using FS.SMIS_Cloud.NGET.Model;

    using NUnit.Framework;

    [TestFixture]
    public class NGETTest
    {
        public Sensor GetSensor( Sensor sensor)
        {
            sensor.SensorID = 32;
            sensor.StructId = 4;
            sensor.Name = "K791左侧三阶平台2号测斜孔-中";
            sensor.FactorType = 10;
            sensor.FactorTypeTable = "T_THEMES_DEFORMATION_DEEP_DISPLACEMENT";
            sensor.TableColums =
                "DEEP_DISPLACEMENT_X_VALUE,DEEP_DISPLACEMENT_Y_VALUE,DEEP_CUMULATIVEDISPLACEMENT_X_VALUE,DEEP_CUMULATIVEDISPLACEMENT_Y_VALUE";
            sensor.ProductCode = "FS-GGC01";
            sensor.ProtocolType = 1503;
            var para = new SensorParam(new FormulaParam
            {
                FID = 2,
                Index = 1,
                Name = "x0",
                Alias = "x方向角度初值",
                PID = 20
            })
            {
                Value = 0.7675540000000
            };
            sensor.AddParameter(para);

            para = new SensorParam(new FormulaParam
            {
                FID = 2,
                Index = 2,
                Name = "y0",
                Alias = "y方向角度初值",
                PID = 21
            })
            {
                Value = -0.0599570000000
            };
            sensor.AddParameter(para);

            para = new SensorParam(new FormulaParam
            {
                FID = 2,
                Index = 1,
                Name = "len",
                Alias = "测斜杆长度(mm)",
                PID = 20
            })
            {
                Value = 3000.0000000000000
            };
            sensor.AddParameter(para);
            return sensor;
        }

        private NGEtService service;

        [SetUp]
        public void InI()
        {
            this.service = new NGEtService("EtService.xml", AppDomain.CurrentDomain.BaseDirectory);
            this.service.PowerOn();
        }

        
    }
}