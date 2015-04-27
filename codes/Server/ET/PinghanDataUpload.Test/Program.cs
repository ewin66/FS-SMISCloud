using System.Reflection;
using NUnit.Util;

namespace FS.DynamicScript.Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var loc = Assembly.GetExecutingAssembly().Location;
            NUnitTestRunner.Run(loc, args);
        }
    }
}
