using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using FreeSun.FS_SMISCloud.Server.DataCalc.Calculation;
using log4net;


namespace FreeSun.FS_SMISCloud.Server.DataCalc.Communication
{
    class FileProcessor
    {
        private ILog _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().GetType());
        private Timer filescantimer;

        public void Start()
        {
            //int interval = ConfigHelper.GetFileScanIntervalMinutes();
            //filescantimer = new Timer(interval * 60 * 1000);
            filescantimer = new Timer(5 * 1000);
            filescantimer.Elapsed += filescantimer_Elapsed;
            filescantimer.Start();
        }

        public void Stop()
        {
            if (filescantimer != null)
            {
                filescantimer.Stop();
                filescantimer = null;
            }
        }

        /// <summary>
        /// 定时触发事件
        /// </summary>
        void filescantimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            filescantimer.Stop();

            var dicpaths = ReadConfig();

            if (dicpaths == null || dicpaths.Count == 0)
            {
                filescantimer.Start();
                return;
            }

            Task task = new Task(() => ScanFiles(dicpaths));
            task.Start();
            task.ContinueWith(t =>
            {
                if (t.Exception != null)
                {
                    this._logger.Warn("处理文件数据异常", t.Exception);
                }
                filescantimer.Start();
            });
        }

        /// <summary>
        /// 扫描文件任务
        /// </summary>
        /// <param name="structpathes"></param>
        void ScanFiles(Dictionary<int, string> structpathes)
        {
            if (structpathes == null || structpathes.Count == 0) return;
            foreach (KeyValuePair<int, string> structpath in structpathes)
            {
                var structid = structpath.Key;
                var path = structpath.Value;
                if(!Directory.Exists(path)) continue;

                try
                {
                    var files = Directory.GetFiles(path, "*.dat", SearchOption.TopDirectoryOnly);
                    if (files == null || !files.Any()) continue;
                    _logger.InfoFormat("结构物:{0},目录:{1},文件个数{2}", structid, path, files.Count());
                    foreach (var file in files)
                    {
                        ProcessFile(structid, file);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Fatal(ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 数据文件处理操作
        /// </summary>
        /// <param name="structid">结构物ID</param>
        /// <param name="file">文件名</param>
        void ProcessFile(int structid, string file)
        {
            try
            {
                ActionAfter(VibrationFileParser.Parse(structid, file));
            }
            catch (Exception ex)
            {
                _logger.Warn(string.Format(@"结构物{0}下原始数据文件‘{1}’处理异常,{2}", structid, file, ex.Message));
            }
        }

        /// <summary>
        /// 数据分析后处理操作
        /// </summary>
        /// <param name="ps"></param>
        void ActionAfter(ParserReturn ps)
        {
            if (ps.Error != string.Empty)
            {
                if (ps.Error == "Writing")  //文件正在写入时不处理
                {
                    _logger.Warn(string.Format(@"结构物{0}下原始数据文件‘{1}’正在被占用", ps.Struct, ps.File));
                }
                else
                {
                    MoveFileToError(ps.File);
                    _logger.Warn(string.Format(@"结构物{0}下原始数据文件‘{1}’计算失败,错误:{2}", ps.Struct, ps.File, ps.Error));
                }
            }
            else
            {
                MoveFileToFinish(ps.File);
                _logger.InfoFormat("处理成功:模块{0},通道{1},时间{2},频率{3}Hz,时长{4}s", ps.mod, ps.ch, ps.Time, ps.freq, ps.Period);
            }
        }

        /// <summary>
        /// 移动文件到处理后文件夹
        /// </summary>
        /// <param name="file"></param>
        void MoveFileToFinish(string file)
        {
            var destdir = Path.Combine(file.Substring(0, file.LastIndexOf(Path.DirectorySeparatorChar)), "Done");
            if (!Directory.Exists(destdir))
                Directory.CreateDirectory(destdir);
            var destfile = Path.Combine(destdir, Path.GetFileName(file));
            if (File.Exists(destfile))
                File.Delete(destfile);
            File.Move(file, destfile);
        }

        /// <summary>
        /// 移动文件到处理后文件夹
        /// </summary>
        /// <param name="file"></param>
        void MoveFileToError(string file)
        {
            var destdir = Path.Combine(file.Substring(0, file.LastIndexOf(Path.DirectorySeparatorChar)), "Error");
            if (!Directory.Exists(destdir))
                Directory.CreateDirectory(destdir);
            var destfile = Path.Combine(destdir, Path.GetFileName(file));
            if (File.Exists(destfile))
                File.Delete(destfile);
            File.Move(file, destfile);
        }

        /// <summary>
        /// 获取项目二次计算配置 -- 原始文件存放路径
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, string> ReadConfig()
        {
            try
            {
                return ConfigHelper.GetStructWorkPaths();
            }
            catch (Exception ex)
            {
                _logger.Warn("查询原始数据路径异常，"+ex.Message);
            }
            return null;
        }
    }
}
