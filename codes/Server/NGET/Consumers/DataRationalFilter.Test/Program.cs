namespace DataRationalFilter.Test
{
    using System.Reflection;

    using NUnit.Util;

    class Program
    {
        static void Main(string[] args)
        {
            string loc = Assembly.GetExecutingAssembly().Location;
            NUnitTestRunner.Run(loc, args);
        }
    }
}
