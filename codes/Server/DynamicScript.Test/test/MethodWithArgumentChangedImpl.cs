using System;

namespace FS.DynamicScript.Test.test
{
    public class MethodWithArgumentChangedImpl : MethodWithArgumentChanged
    {
        public void CallMe(ref string text)
        {
            text = "Hello Unit.";
        }
    }
}
