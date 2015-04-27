// --------------------------------------------------------------------------------------------
// <copyright file="DbTest.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2015 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20150329
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace NGET.Test
{
    using FS.SMIS_Cloud.NGET.DbAccessor;
    using FS.SMIS_Cloud.NGET.Model;
    using FS.SMIS_Cloud.NGET;

    using NUnit.Framework;

    [TestFixture]
    public class DbTest
    {
        [Test]
        public void TestGetSensorConfig()
        {
            GlobalConfig.ConnectionString = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004;pooling=false";

            Sensor s = DbConfigAccessor.GetSensorInfo(1265);

            Assert.AreEqual(SensorType.Virtual, s.SensorType);
            Assert.AreEqual(true, s.Enabled);
            Assert.AreEqual(0, s.Parameters[0].Value);
            Assert.AreEqual(41, s.Parameters[1].FormulaParam.PID);
            Assert.AreEqual(2, s.Parameters.Count);
        }
    }
}