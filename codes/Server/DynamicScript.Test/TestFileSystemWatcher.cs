using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace FS.DynamicScript.Test
{
    [TestFixture]
    public class TestFileSystemWatcher
    {
        [Test]
        [Category("MANUAL")]
        public void TestWatcher()
        {
            var watcher = new FileSystemWatcher
            {
                Path = @"test\out",
                IncludeSubdirectories = false,
                Filter = "*.cs"
            };
            watcher.Changed += (sender, e) => { Console.WriteLine(e.ChangeType); };
            watcher.EnableRaisingEvents = true;
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}