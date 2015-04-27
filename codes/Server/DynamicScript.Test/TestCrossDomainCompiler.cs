using System;
using System.IO;
using NUnit.Framework;

namespace FS.DynamicScript.Test
{
    public interface SimpleInterface
    {
        void CallMe();
    }

    public interface MethodWithArgument
    {
        void CallMe(string text);
    }

    public interface MethodWithArgumentChanged
    {
        void CallMe(ref string text);
    }

    [TestFixture]
    public class TestCrossDomainCompiler
    {
        [Test]
        [ExpectedException(typeof (FileNotFoundException))]
        public void TestFileNotFoundException()
        {
            CrossDomainCompiler.Call(@"test\error\NotExist.cs", @"test\SimpleInterface.json",
                typeof (SimpleInterface), "CallMe");
        }

        [Test]
        [ExpectedException(typeof (CompileErrorException))]
        public void TestCompileErrorException()
        {
            CrossDomainCompiler.Call(@"test\error\CompileError.cs", @"test\SimpleInterface.json",
                typeof (SimpleInterface), "CallMe");
        }

        [Test]
        [ExpectedException(typeof (NotImplementedException))]
        public void TestNoImplementsError()
        {
            CrossDomainCompiler.Call(@"test\error\NoImplementsError.cs", @"test\SimpleInterface.json",
                typeof (SimpleInterface), "CallMe");
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void TestArgumentException()
        {
            CrossDomainCompiler.Call(@"test\error\NoImplementsError.cs", @"test\SimpleInterface.json",
                typeof(String), "CallMe");
        }

        [Test]
        public void TestSimpleInterface()
        {
            CrossDomainCompiler.Call(@"test\SimpleInterfaceImpl.cs", @"test\SimpleInterface.json",
                typeof (SimpleInterface), "CallMe");
        }

        [Test]
        public void TestInterfaceWithConstructor()
        {
            var cp = new object[] {};
            CrossDomainCompiler.Call(@"test\ImplWithConstructor.cs", @"test\SimpleInterface.json",
                typeof (SimpleInterface), "CallMe", ref cp);
        }

        [Test]
        public void TestMethodWithArgumentImpl()
        {
            var cp = new object[] {};
            var mp = new object[] {"Hellow world!"};
            CrossDomainCompiler.Call(@"test\MethodWithArgumentImpl.cs", @"test\SimpleInterface.json",
                typeof (MethodWithArgument), "CallMe", ref cp, ref mp);
        }

        [Test]
        public void TestMethodWithArgumentImplModify()
        {
            var cp = new object[] {};
            var change = "Hello world";
            var mp = new object[] {change};
            CrossDomainCompiler.Call(@"test\MethodWithArgumentChangedImpl.cs", @"test\SimpleInterface.json",
                typeof (MethodWithArgumentChanged), "CallMe", ref cp, ref mp);
            StringAssert.AreEqualIgnoringCase("Hello Unit.", (string) mp[0]);
        }

        [Test]
        public void TestSimpleInterfacePerf()
        {
            var start = DateTime.Now;
            const int upper = 10;
            for (var i = 0; i < upper; i++)
            {
                CrossDomainCompiler.Call(@"test\SimpleInterfaceImpl.cs", @"test\SimpleInterface.json",
                    typeof (SimpleInterface), "CallMe");
            }
            var end = DateTime.Now;
            var tpercall = ((end.Ticks - start.Ticks)/upper/10000);
            Assert.Less(tpercall, 1000);
        }
    }
}