using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace FS.DynamicScript.Test
{
    [TestFixture]
    public class TestCDCLoad
    {
        [TearDown]
        public void TestCleanup()
        {
            CrossDomainCompiler.Cleanup();
        }

        [Test]
        [ExpectedException(typeof (CompileErrorException))]
        public void TestLoadCompileError()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test\error");
        }

        [Test]
        [ExpectedException(typeof (FileNotFoundException))]
        public void TestLoadDirectoryNotFound()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test\notexist");
        }

        [Test]
        [ExpectedException(typeof (FileNotFoundException))]
        public void TestLoadDirectoryEmpty()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test\out");
        }

        [Test]
        [ExpectedException(typeof (FileNotFoundException))]
        public void TestLoadDependConfigNotFound()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface1.json", @"test\out");
        }

        [Test]
        [ExpectedException(typeof (CompilerNotFoundException))]
        public void TestLoadCompilerNotFoundCauseNotExist()
        {
            CrossDomainCompiler.Call(@"notexist", typeof(SimpleInterface), "ImplWithConstructor", "CallMe");
        }

        [Test]
        [ExpectedException(typeof (CompilerNotFoundException))]
        public void TestLoadCompilerNotFoundCauseNotLoad()
        {
            CrossDomainCompiler.Call(@"test", typeof(SimpleInterface), "ImplWithConstructor", "CallMe");
        }

        [Test]
        public void TestLoad()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test");
            //reload
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test");
        }

        [Test]
        [Category("MANUAL")]
        public void TestLoadWatcher()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test");
            //watcher
            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        [Test]
        public void TestLoadAndCall()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test");
            CrossDomainCompiler.Call(@"test", typeof(SimpleInterface), "ImplWithConstructor", "CallMe");
        }


        [Test]
        public void TestLoadAndInterfaceWithConstructor()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test");
            var cp = new object[] {};
            CrossDomainCompiler.Call(@"test", typeof(SimpleInterface), "ImplWithConstructor", "CallMe", ref cp);
        }

        [Test]
        public void TestLoadAndCallMethodWithArgumentImpl()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test");
            var cp = new object[] {};
            var mp = new object[] {"Hellow world!"};
            CrossDomainCompiler.Call(@"test", typeof(MethodWithArgument), "MethodWithArgumentImpl", "CallMe", ref cp, ref mp);
        }

        [Test]
        public void TestLoadAndCallMethodWithArgumentModifyImpl()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test");
            var cp = new object[] {};
            var mp = new object[] {"Hellow world!"};
            CrossDomainCompiler.Call(@"test", typeof(MethodWithArgumentChanged), "MethodWithArgumentChangedImpl", "CallMe", ref cp, ref mp);
            StringAssert.AreEqualIgnoringCase("Hello Unit.", (string) mp[0]);
        }

        [Test]
        public void TestLoadAndCallPerf()
        {
            CrossDomainCompiler.LoadScript(@"test\SimpleInterface.json", @"test");
            var start = DateTime.Now;
            const int upper = 100;
            for (var i = 0; i < upper; i++)
            {
                CrossDomainCompiler.Call(@"test", typeof(SimpleInterface), "ImplWithConstructor", "CallMe");
            }
            var end = DateTime.Now;
            var tpercall = ((end.Ticks - start.Ticks)/upper/10000);
            Assert.Less(tpercall, 20);
        }
    }
}