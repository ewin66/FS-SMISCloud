// --------------------------------------------------------------------------------------------
// <copyright file="EtController.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2015 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：
// 
// 创建标识：20150329
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FS.SMIS_Cloud.NGET
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using FS.SMIS_Cloud.NGET.Consumer;
    using FS.SMIS_Cloud.NGET.DataParser;
    using FS.SMIS_Cloud.NGET.Model;

    using log4net;

    public class EtController
    {
        private static ILog Log = LogManager.GetLogger("EtController");

        private static Timer Timer;

        private static bool DoingTimerWork;

        private static bool IsExiting = false;

        private static List<Task> tasks = new List<Task>();

        private static readonly object LockObj = new object();

        private static readonly object TimeLock = new object();

        private static readonly object FileLock = new object();

        private static List<string> ProcessiongFiles = new List<string>();

        private DataStatusJudge dataStatusJudge;

        public EtController(Service.Service service)
        {
            this.dataStatusJudge = new DataStatusJudge(service);

            if (GlobalConfig.ConnectionString == null)
            {
                throw new Exception("GlobalConfig.ConnectionString is null");
            }

            if (GlobalConfig.DataSourcePath == null)
            {
                throw new Exception("GlobalConfig.DataSourcePath is null");
            }

            if (GlobalConfig.ErrorFilePath == null)
            {
                throw new Exception("GlobalConfig.ErrorFilePath is null");
            }
            if (!Directory.Exists(GlobalConfig.ErrorFilePath))
            {
                Directory.CreateDirectory(GlobalConfig.ErrorFilePath);
            }

            if (GlobalConfig.ParsedFilePath == null)
            {
                throw new Exception("GlobalConfig.ParsedFilePath is null");
            }
            if (!Directory.Exists(GlobalConfig.ParsedFilePath))
            {
                Directory.CreateDirectory(GlobalConfig.ParsedFilePath);
            }

            DacTaskResultConsumerService.Init();
            Log.Info("ET init success, scaning file to consume");
        }

        public void DoTimerWork()
        {
            Timer = new Timer(
                o =>
                {
                    lock (TimeLock)
                    {
                        if (IsExiting)
                        {
                            return;
                        }
                        if (DoingTimerWork)
                        {
                            return;
                        }
                        DoingTimerWork = true;
                        DoFileParseWork();
                        DoingTimerWork = false;
                    }
                },
                null,
                100,
                GlobalConfig.FileScanInterval);
        }

        public void DoFileParseWork()
        {
            var files = new DirectoryInfo(GlobalConfig.DataSourcePath).GetFiles();
            if (files.Length == 0)
            {
                return;
            }
            Log.InfoFormat("{0} files is found to parse, start..", files.Length);
            var sw = new Stopwatch();
            sw.Start();
            int count = 0;
            foreach (FileInfo file in files)
            {
                try
                {
                    lock (FileLock)
                    {
                        if (ProcessiongFiles.Exists(f => f == file.FullName))
                        {
                            return;
                        }

                        ProcessiongFiles.Add(file.FullName);
                    }

                    var data = new List<SensorAcqResult>();
                    var hasError = false;
                    lock (LockObj)
                    {
                        if (IsExiting)
                        {
                            return;
                        }
                    }
                    Log.InfoFormat("parsing file: {0}", file.FullName);
                    // 扫描数据
                    using (var fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        using (var sr = new StreamReader(fs))
                        {
                            var line = 1;
                            while (sr.Peek() > -1)
                            {
                                string s = sr.ReadLine();
                                var parser = new JsonParser();
                                try
                                {
                                    var rslt = parser.Parse(s);
                                    if (this.dataStatusJudge.JudgeDataStatusIsOk(rslt))
                                    {
                                        var sensorId = rslt.Sensor.SensorID;
                                        rslt.Sensor = DbAccessor.DbConfigAccessor.GetSensorInfo(sensorId);
                                        data.Add(rslt);
                                    }
                                }
                                catch (Exception e)
                                {
                                    Log.Error(
                                        string.Format("file: {0},line: {1},json parse error", file.Name, line),
                                        e);
                                    hasError = true;
                                }
                                line++;
                            }
                        }
                        var filePath = file.FullName;
                        var fileName = file.Name;
                        lock (LockObj)
                        {
                            if (IsExiting)
                            {
                                return;
                            }
                            Log.InfoFormat("parsing file: {0} end, send to consumers", file.FullName);
                            var consumeTask = new Task(
                                () =>
                                {
                                    var task = DacTaskResultConsumerService.OnDacTaskResultProduced(data);
                                    task.ContinueWith(
                                        t =>
                                            {
                                                hasError = hasError || t.Exception != null;
                                                ArchiveDataFile(hasError, fileName, filePath);
                                                Log.InfoFormat("consume file: {0} end", filePath);
                                                lock (FileLock)
                                                {
                                                    ProcessiongFiles.Remove(filePath);
                                                }
                                            }).Wait();
                                });
                            tasks.Add(consumeTask);
                            consumeTask.Start();
                            consumeTask.ContinueWith(
                                t =>
                                    {
                                        lock (LockObj)
                                        {
                                            tasks.Remove(t);
                                        }
                                    });
                        }
                        count++;
                    }
                }
                catch (IOException e)
                {
                    Log.Warn(string.Format("file:{0} is being opened", file.Name), e);
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("unknow exception when open file:{0}", file.Name), e);
                }
            }

            sw.Stop();
            Log.InfoFormat(
                "{0} files has parsed in {1} seconds, error: {2}",
                count,
                sw.Elapsed.TotalSeconds,
                files.Length - count);
        }

        private static void ArchiveDataFile(bool hasError, string fileName, string filePath)
        {
            try
            {
                if (hasError)
                {
                    string p = string.Format("{0}\\{1}", GlobalConfig.ErrorFilePath, DateTime.Now.ToString("yyyyMMdd"));
                    if (!Directory.Exists(p))
                    {
                        Directory.CreateDirectory(p);
                    }
                    var des = p + "\\" + fileName;
                    int i = 0;
                    string desFile = des;
                    while (File.Exists(desFile))
                    {
                        i++;
                        desFile = des + "(" + i + ")";
                    }
                    File.Move(filePath, desFile);
                }
                else
                {
                    string path = string.Format(
                        "{0}\\{1}",
                        GlobalConfig.ParsedFilePath,
                        DateTime.Now.ToString("yyyyMMdd"));
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    var des = path + "\\" + fileName;
                    int i = 0;
                    string desFile = des;
                    while (File.Exists(desFile))
                    {
                        i++;
                        desFile = des + "(" + i + ")";
                    }
                    File.Move(filePath, desFile);
                }
                Log.Info("file: [" + filePath + "] archive success");
            }
            catch (Exception e)
            {
                Log.Error("file: [" + filePath + "] archive failed", e);
                string path = string.Format(
                        "{0}\\{1}\\archError\\{2}.{3}",
                        GlobalConfig.ParsedFilePath,
                        DateTime.Now.ToString("yyyyMMdd"),
                        fileName,
                        Guid.NewGuid());
                if (File.Exists(fileName))
                {
                    File.Move(filePath, path);
                }
            }
        }

        public void Exit()
        {
            Log.Info("Et Controller received exit cmd");
            Console.WriteLine("Et Controller received exit cmd");
            Task[] ts;
            lock (LockObj)
            {
                if (IsExiting)
                {
                    return;
                }
                IsExiting = true;
                ts = tasks.ToArray();
            }

            if (ts.Length > 0)
            {
                Log.InfoFormat("{0} tasks is executing, please wait..", ts.Length);
                Console.WriteLine("{0} tasks is executing, please wait..", ts.Length);
                Task.WaitAll(ts);
            }
            Log.Info("Et Controller is exiting..");
            Console.WriteLine("Et Controller is exiting..");
        }
    }
}