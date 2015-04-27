#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="ErrorsTest.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150204 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace NGDAC.Test.DAC
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;

    using FS.SMIS_Cloud.NGDAC.Model;

    using NUnit.Framework;

    [TestFixture]
    public class ErrorsTest
    {
        [Test]
        public void TestFistOrDefault()
        {
            var dacErrorCodes = new ConcurrentDictionary<string, DacErrorCode>();
            dacErrorCodes.TryAdd("succeed", new DacErrorCode()
            {
                ErrorCode = 0,
                ErrorDescription = "OK",
                ErrorNameUs = "succeed"
            });
            dacErrorCodes.TryAdd("default", new DacErrorCode()
            {
                ErrorCode = 1000,
                ErrorDescription = "default",
                ErrorNameUs = "default"
            });

           DacErrorCode code = dacErrorCodes.FirstOrDefault(e => e.Value.ErrorNameUs == "default").Value;

            if (code == null)
            {
                Console.WriteLine("DAC ERROR IS Empty");
            }

            code = dacErrorCodes.FirstOrDefault(e => e.Value.ErrorNameUs == "OK").Value;

            if (code == null)
            {
                Console.WriteLine("DAC ERROR IS Empty");
            }

        }


    }
}