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

namespace NGDACService.Test
{
    using System;

    using FS.Service;
    using FS.SMIS_Cloud.NGDAC;
    using FS.SMIS_Cloud.NGDAC.Model;

    using NUnit.Framework;

    [TestFixture]
    public class DacTest
    {
        public Sensor GetSensor( Sensor sensor)
        {
            sensor.SensorID = 32;
            sensor.StructId = 4;
            sensor.ModuleNo = 9590;
            sensor.ChannelNo = 1;
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

        private DacService service;

       
        public void InI()
        {
            this.service = new DacService("DacService.xml", AppDomain.CurrentDomain.BaseDirectory);
            this.service.PowerOn();
        }

        static FsMessage newMessage(object body)
        {
            Guid token = Guid.NewGuid();
            Guid msgId = Guid.NewGuid();
            FsMessage msg = new FsMessage();
            msg.Header = new FsMessageHeader();
            msg.Header.A = "GET";
            msg.Header.D = "LOG";
            msg.Header.L = 20;
            msg.Header.R = "log";
            msg.Header.S = "中文";
            msg.Header.T = token;
            msg.Header.U = msgId;
            msg.Body = body;
            return msg;
        }

        [Test]
        public void TestJson()
        {
            var msg = newMessage(new
            {
                structId = 3
            });

            string str = msg.ToJson();

            msg = FsMessage.FromJson(str);



            var i = msg.BodyValue<uint>("structId");
            Console.WriteLine(i);
        }


    }
}