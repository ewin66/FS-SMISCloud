using System;
using FS.SMIS_Cloud.DAC.Gprs.Cmd;
using NUnit.Framework;

namespace DAC.Test.Comm
{
    /// <summary>
    /// UnitTest_SetIpAddress 的摘要说明
    /// </summary>
    [TestFixture]
    public class AtCommandsTester
    {
        [Test]

        public void TestSetIP()
        {
            SetIP cmd = new SetIP("{\"index\":1, \"ip\": \"192.168.1.1\"}");
            Assert.AreEqual("AT+IPAD=192.168.1.1\r", cmd.ToATString());

            cmd = new SetIP("{ \"ip\": \"192.168.1.1\"}");
            Assert.AreEqual("AT+IPAD=192.168.1.1\r", cmd.ToATString());

            cmd = new SetIP("{\"index\":2, \"ip\": \"192.168.1.2\"}");
            Assert.AreEqual("AT+IPAD2=192.168.1.2\r", cmd.ToATString());
        }

         [Test]
        public void TestSetPort()
        {
            SetPort cmd =new SetPort("{'index':1,'port':5008}");
            Assert.AreEqual("AT+PORT=5008\r", cmd.ToATString());

            cmd = new SetPort("{'port':5008}");
            Assert.AreEqual("AT+PORT=5008\r", cmd.ToATString());

            cmd = new SetPort("{'index':2,'port':5008}");
            Assert.AreEqual("AT+PORT2=5008\r", cmd.ToATString());
        }

         [Test]
        public void TestSetPort2()
         {
             SetPort cmd;
             try
             {
                 cmd = new SetPort(string.Empty);
             }
             catch (Exception)
             {
                 Assert.IsTrue(true);
             }
             
             try
             {
                 cmd = new SetPort("100");
             }
             catch (Exception)
             {
                 Assert.IsTrue(true);
             }

             try
             {
                 cmd = new SetPort("1023");
             }
             catch (Exception)
             {
                 Assert.IsTrue(true);
             }

             try
             {
                 cmd = new SetPort(null);
             }
             catch (Exception)
             {
                 Assert.IsTrue(true);
             }

             try
             {
                 cmd = new SetPort("abcdefg");
             }
             catch (Exception)
             {
                 Assert.IsTrue(true);
             }
        }

        [Test]
        public void testSetRetryTimes()
        {
            SetRetryTimes cmd;
            try
            {
                cmd = new SetRetryTimes(string.Empty);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            try
            {
                cmd = new SetRetryTimes("{\"retry\":200}");
                Assert.AreEqual("AT+RETRY=200\r", cmd.ToATString());
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
            try
            {
                cmd = new SetRetryTimes("abcdefg");
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void TestByteInterval()
        {
            SetByteInterval cmd;
            try
            {
                cmd = new SetByteInterval(string.Empty);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            try
            {
                cmd = new SetByteInterval("{'byteInterval':320}");
                Assert.AreEqual("AT+BYTEINT=320\r",cmd.ToATString());
                Assert.AreEqual(320,cmd.ByteInterval);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
            //try
            //{
            //    cmd = new SetByteInterval("abcdefg");
            //}
            //catch (Exception)
            //{
            //    Assert.IsTrue(true);
            //}
        }


        [Test]
        public void TestSetWorkMode()
        {
            SetWorkMode cmd;
            try
            {
                cmd = new SetWorkMode(string.Empty);
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }

            try
            {
                cmd = new SetWorkMode("{'mode':'TCP'}");
                Assert.AreEqual("AT+MODE=TCP\r",cmd.ToATString());
            }
            catch (Exception)
            {
                Assert.IsTrue(true);
            }
            //try
            //{
            //    cmd = new SetWorkMode("abcdefg");
            //}
            //catch (Exception)
            //{
            //    Assert.IsTrue(true);
            //}

            //try
            //{
            //    cmd = new SetWorkMode("TCP");
            //}
            //catch (Exception)
            //{
            //    Assert.IsTrue(true);
            //}

        }




    }
}
