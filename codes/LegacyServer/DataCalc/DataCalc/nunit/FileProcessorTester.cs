using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.Communication;
using NUnit.Framework;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.nunit
{
    [TestFixture]
    class FileProcessorTester
    {
        [Test]
        public void Test()
        {
            var method = typeof(FileProcessor).GetMethod("ScanFiles",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            var paras = new Dictionary<int, string>();
            paras.Add(42, @"nunit\Resource");
            method.Invoke(new FileProcessor(), new[] { paras });
        }
    }
}
