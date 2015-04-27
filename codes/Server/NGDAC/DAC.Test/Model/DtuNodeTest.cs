#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="DtuNodeTest.cs" company="江苏飞尚安全监测咨询有限公司">
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

namespace NGDAC.Test.Model
{
    using System;
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Model;

    using NUnit.Framework;

    [TestFixture]
    public class DtuNodeTest
    {
        [Test]
        public void TestAddSensor()
        {
            DtuNode node = new DtuNode()
            {
                DtuCode = "12345678",
                DtuId = 2,
                Type = DtuType.Gprs,
                Name = "Test",
                DacInterval = 5
            };

            new Thread(t =>
            {
                uint i = 1;
                while (i<100)
                {
                    var senopera = new SensorOperation
                    {
                        Sensor = new Sensor
                        {
                            SensorID = i,
                            ModuleNo = 2000 + 1,
                            ChannelNo = 1,
                            DtuID = 2
                        },
                        Action = Operations.Add
                    };
                    node.AddSensorOperation(senopera);
                    i++;
                }
            }).Start();
            Thread.Sleep(10);
            new Thread(t =>
            {
                uint i = 5;
                while (i<50)
                {
                    var senopera = new SensorOperation
                    {
                        OldSensorId = i,
                        Action = Operations.Delete
                    };
                    node.AddSensorOperation(senopera);
                    i++;
                }
            }).Start();

            Thread.Sleep(100);
            var t2 = new Thread(t =>
            {
                node.UpDateSensor();
            });
            t2.Start();
            t2.Join();

            Console.WriteLine("dtunode's sensors count is {0}", node.Sensors.Count);
        }
         
    }
}