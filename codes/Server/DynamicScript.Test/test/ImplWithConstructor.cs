using System;
using System.IO;
using FS.DynamicScript;

namespace FS.DynamicScript.Test.test
{
    class ImplWithConstructor: SimpleInterface
    {
        public ImplWithConstructor()
        {
            
        }

        public void CallMe()
        {
            File.WriteAllText(@"test\out\ImplWithConstructor.data", DateTime.Now.ToString());
        }
    }
}
