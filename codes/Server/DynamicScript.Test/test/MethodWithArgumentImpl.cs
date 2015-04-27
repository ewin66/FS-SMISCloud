using System;
using System.IO;

namespace FS.DynamicScript.Test.test
{
    public class MethodWithArgumentImpl: MethodWithArgument
    {
        public void CallMe(string text)
        {
            File.WriteAllText(@"test\out\MethodWithArgumentImpl.data", text);
        }
    }
}
