using System.Reflection;
using NUnit.Util;

namespace FS.Service.Test
{
    class Program
    {     
        static void Main(string[] args)
        {
            string loc = Assembly.GetExecutingAssembly().Location;
            NUnitTestRunner.Run(loc, args);
        }
    }
}
