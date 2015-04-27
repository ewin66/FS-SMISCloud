/*----------------------------------------------------------------
// <copyright file="Program.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。
// </copyright>
// <summary>
 
// 文件名：BatchZipTester.cs
// 功能描述：
// 
// 创建标识： 2014/12/4 10:52:14
// 
// 修改标识：
// 修改描述：
//
// 修改标识：
// 修改描述：
//
// </summary>

//----------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SecureCloud.Support;

namespace SecureCloud.Test
{
    [TestFixture]
    internal class BatchZipTester
    {
        [Test]
        public void TestZipFile()
        {
            var toTest = 100;
            long started = System.DateTime.Now.Ticks / 10000;

            string srcDir = Path.GetFullPath("test");
            string desZipName = srcDir + ".zip";

            for (var i = 0; i < toTest; i++)
            {
                try
                {
                    MultiDownLoadReport.CreateZip(srcDir, desZipName);
                    if (i < toTest - 1)
                    {
                        //删除压缩包
                        var zipFile = new FileInfo(desZipName);
                        zipFile.Delete();
                    }

                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }
            }
            long costed = System.DateTime.Now.Ticks / 10000 - started;

            Console.WriteLine("生成的压缩包...{0}", desZipName);
            Console.WriteLine("{0} tested in {1} ms, speed = {2:##0.00}", toTest, costed, costed / toTest);
        }
    }
}
