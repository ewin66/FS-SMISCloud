namespace NGDAC.Test
{
    using System.Reflection;

    using NGDAC.Test.Task;

    using NUnit.Util;

    internal static class Program
    {
        private static void Main(string[] args)
        {
//            if (args.Length == 0)
//                args = new string[] {"excluded=MANUAL"};
            // NUnitTestRunner.Run(loc, args);
            if (args.Length >= 1)
            {
                string loc = Assembly.GetExecutingAssembly().Location;
                NUnitTestRunner.Run(loc, args);
            }
            else
            {
                // QuartzTester.TestExecute
                new QuartzTester().TestExecute();
            }
        }
    }
}
