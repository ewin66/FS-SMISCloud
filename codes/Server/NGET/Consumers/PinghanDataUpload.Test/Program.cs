namespace DataUpload.Test
{
    using System.Reflection;

    using NUnit.Util;

    internal class Program
    {
        private static void Main(string[] args)
        {
            var loc = Assembly.GetExecutingAssembly().Location;
            NUnitTestRunner.Run(loc, args);
        }
    }
}
