using System.Collections;
using System.Reflection;
using NUnit.Core;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Core.Filters;

namespace NUnit.Util
{
    public class NUnitTestRunner
    {
        private static Dictionary<string, int> filtered = new Dictionary<string, int>();

        public static void Run(string loc, string[] args)
        {
            MainArgs ma = MainArgs.ValueOf(args);
            string excludes = ma.Get("exclude", null);
            string includes = ma.Get("include", null);
            string outfile = ma.Get("out", "TestResult.xml");

            CoreExtensions.Host.InitializeService();
            TestSuiteBuilder builder = new TestSuiteBuilder();
            TestPackage testPackage = new TestPackage(loc);
            RemoteTestRunner remoteTestRunner = new RemoteTestRunner();
            remoteTestRunner.Load(testPackage);
            TestSuite suite = builder.Build(testPackage);
            TestSuite root = suite.Tests[0] as TestSuite;
            List<TestFixture> fixtures = new List<TestFixture>();
            ScanFixtures(root, fixtures);
            Console.WriteLine("--------------- {0} TextFixtures --------------- \n", fixtures.Count);
            //TestName testName = ((TestMethod) ((TestFixture) test.Tests[0]).Tests[0]).MethodName;

            ITestFilter filter = null;
            bool hasInclude = !string.IsNullOrEmpty(includes);
            bool hasExclude = !string.IsNullOrEmpty(excludes);
            if (hasInclude )
            {
                if (hasExclude)
                {
                    // incldue+exclude; exclude first
                    filter =
                        new AndFilter(new ITestFilter[]
                        {new NotFilter(new CategoryFilter(excludes.Split(','))), new SimpleNameFilter(includes.Split(',')), });
                }
                else
                {
                    // include 
                    filter = new SimpleNameFilter(includes.Split(','));
                }
            }
            else // no include
            {
                if (hasExclude)
                {
                    // Only exclude
                    filter = new NotFilter(new CategoryFilter(excludes.Split(',')));
                }
                else
                {
                    // none 
                    filter = new TrueFilter();
                }
            }
            
            int succCnt = 0, failCnt = 0, errorCnt = 0, assertCnt = 0;
            TestResult tr = new TestResult(new TestName());
            RunEventListener eventLsn = new RunEventListener();
            foreach (TestFixture tf in fixtures)
            {
                TestResult result = tf.Run(eventLsn, filter);
                FixtureResult fr = null;
                if (result.Results != null)
                {
                    fr = new FixtureResult(result);
                    succCnt += fr.succCnt;
                    failCnt += fr.failCnt;
                    errorCnt += fr.errorCnt;
                    assertCnt += fr.assertCnt;
                    Console.WriteLine("  Done: " + fr.ToString());
                }
                else
                {
                    Console.WriteLine("  Done: no result.");
                }
                tr.AddResult(result);
            }
            if (failCnt + errorCnt == 0)
            {
                Console.WriteLine(
@"=========================================
Test Success! Cases: {0}, asserts: {1}
=========================================",
                    succCnt, assertCnt);
            }
            else
            {
                Console.WriteLine(
@"=================================================================================
 Test with errors: Cases: {0}, asserts: {4}, Succ: {1}, fail:{2}, error: {3}
=================================================================================",
                    succCnt + errorCnt + failCnt, succCnt, failCnt, errorCnt, assertCnt);
            }
            XmlResultWriter w = new XmlResultWriter(outfile);
            w.SaveTestResult(tr);
            Console.WriteLine("Result save to: {0}", outfile);
        }

        class FixtureResult
        {
            public readonly int succCnt = 0, failCnt = 0, errorCnt = 0, assertCnt = 0;
            public FixtureResult(TestResult r)
            {
                foreach (TestResult tr in r.Results)
                {
                    succCnt += tr.IsSuccess ? 1 : 0;
                    failCnt += tr.IsFailure ? 1 : 0;
                    errorCnt += tr.IsError ? 1 : 0;
                    assertCnt += tr.AssertCount;
                }
            }

            public override string ToString()
            {
                return string.Format("Total: {0}, Success: {1}, failured:{2}, Error: {3}, Asserts:{4}",
                   succCnt + errorCnt + failCnt, succCnt, failCnt, errorCnt, assertCnt);
            }
        }

        static void WriteConsole(string msg, ConsoleColor clr)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = clr;
            Console.Write(msg);
            Console.ForegroundColor = oldColor;
        }

        // 找到TextFixture和对应的
        private static void ScanFixtures(TestSuite suit, List<TestFixture> fixtures)
        {
            foreach (ITest ts in suit.Tests)
            {
                if (ts.IsSuite)
                    ScanFixtures((TestSuite)ts, fixtures);
                if (ts.TestType == "TestFixture")
                {
                    fixtures.Add((TestFixture)ts);
                }
            }
        }

        class RunEventListener : EventListener
        {
            public void RunStarted(string name, int testCount)
            {
            }

            public void RunFinished(TestResult result)
            {
            }

            public void RunFinished(Exception exception)
            {
                Console.Write(" Exception: {0}", exception.Message);
            }

            public void TestStarted(TestName testName)
            {
                Console.Write("  Test: {0} ==> ", testName.Name);
            }

            public void TestFinished(TestResult result)
            {
                Console.Write(" ({0:###0.0} ms) ", result.Time * 1000);
                if (result.IsSuccess)
                {
                    WriteConsole("OK", ConsoleColor.Green);
                }
                else if (result.IsFailure)
                {
                    WriteConsole("Failed", ConsoleColor.Red);
                }
                else if (result.IsError)
                {
                    WriteConsole("Error", ConsoleColor.Red);
                }
                else
                {
                    WriteConsole("Error", ConsoleColor.Gray);
                }
                Console.Write("\n");
            }

            public void SuiteStarted(TestName testName)
            {
                Console.Write("Fixture: {0}\n", testName.FullName);
            }

            public void SuiteFinished(TestResult result)
            {
            }

            public void UnhandledException(Exception exception)
            {
                Console.WriteLine("Exception {0}", exception);
            }

            public void TestOutput(TestOutput testOutput)
            {
                // testOutput.
            }
        }
        delegate void OnFilter(ITest test);

        class TrueFilter : TestFilter
        {
            public override bool Match(ITest test)
            {
                return true;
            }
        }
        #region NameCatFilter
        class NameCatFilter : TestFilter
        {
            readonly ArrayList _exclude_categories;
            private readonly ArrayList _include_names;
            public OnFilter filtered;

            public NameCatFilter(string[] excludes, string[] includes)
            {
                _exclude_categories = new ArrayList();
                _include_names = new ArrayList();
                if (excludes != null)
                    _exclude_categories.AddRange(excludes);
                if (includes != null)
                    _include_names.AddRange(includes);
            }
            public  override bool Match(ITest test)
            {
                bool catok = true;
                bool nameok = true;
                if (test.Categories != null && test.Categories.Count > 0)
                {
                    foreach (string cat in _exclude_categories)
                    {
                        if (test.Categories.Contains(cat))
                        {
                            catok = false;
                            break;
                        }
                    }
                }
                if (_include_names.Count > 0)
                    nameok = _include_names.Contains(test.TestName.Name);
Console.WriteLine("{0},{1}",catok,nameok);
                bool shouldTest = catok & nameok;
                if (!shouldTest && test.IsSuite)
                {
                    filtered(test);
                }
                return shouldTest;
            }
            /*
            public override bool Match(ITest test)
            {
                if (test.Categories == null)
                    return false;
                foreach (string cat in _exclude_categories)
                    if (test.Categories.Contains(cat))
                    {
                        if (filtered != null)
                            filtered(test);
                        return true;
                    }

                return false;
            }
            */
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < _exclude_categories.Count; i++)
                {
                    if (i > 0) sb.Append(',');
                    sb.Append(_exclude_categories[i]);
                }
                return sb.ToString();
            }
        }
    }
        #endregion

    #region MainArgs
    class MainArgs
    {

        private Dictionary<string, string> values;
        public string Get(string key, string dv = null)
        {
            return values.ContainsKey(key) ? values[key] : dv;
        }
        public int GetInt(string key, int dv = 0)
        {
            return values.ContainsKey(key) ? Convert.ToInt32(values[key]) : dv;
        }
        private MainArgs()
        {
        }

        public static MainArgs ValueOf(string[] args)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            if (args != null && args.Length>0)
            {
                foreach (string s in args)
                {
                    string[] ss = s.Split('=');
                    if (ss.Length != 2) continue;
                    string key = ss[0].Trim();
                    string value = ss[1].Trim();
                    values[key] = value;
                }
            }
            MainArgs a = new MainArgs { values = values };
            return a;
        }
    }
    #endregion

}