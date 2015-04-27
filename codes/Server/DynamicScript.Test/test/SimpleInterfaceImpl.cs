using System;
using System.IO;
using FS.DynamicScript;

namespace FS.DynamicScript.Test.test
{
    public class SimpleInterfaceImpl : SimpleInterface
    {
        public void CallMe()
        {
            File.WriteAllText(@"test\out\SimpleInterfaceImpl.data", DateTime.Now.ToString());
        }
    }
}
