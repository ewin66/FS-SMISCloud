#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="Stenographer.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：20140211 created by Win
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGET.Util
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// The stenographer.
    /// </summary>
    class Stenographer : IDisposable
    {
        private static Dictionary<string, FileAppender> logLst = new Dictionary<string, FileAppender>();
        private const string NewLine = "\r\n";
        static int count; // 0
        #region 定时释放对象

        // private const double dSleep = 5;//停止访问某文件5秒后将其释放
        // private const int iPeriod = 10000;//计时器执行时间间隔
        // //定时器，用来定时释放不再使用的文件对象
        // private static readonly Timer timer = new Timer(new TimerCallback(TimerCall), null, iPeriod, iPeriod);

        // private static void TimerCall(object state)
        // {
        //     DateTime now = DateTime.Now;
        //     foreach (string key in logLst.Keys)
        //     {
        //         if ((now - logLst[key].LastCallTime).TotalSeconds > dSleep)
        //         {
        //             logLst[key].Dispose();
        //         }
        //     }
        // }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="Stenographer"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public Stenographer(string filename)
        {
            this.FileName = filename;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Stenographer"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="encode">
        /// The encode.
        /// </param>
        public Stenographer(string filename, Encoding encode)
        {
            this.FileName = filename;
            if (encode != null)
            {
                this.Encode = encode;
            }
        }

        /// <summary>
        /// Gets or sets the file name.
        /// </summary>
        public string FileName { private get; set; }

        /// <summary>
        /// Gets or sets the encode.
        /// </summary>
        public Encoding Encode { private get; set; }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public void Write(string content)
        {
            this.WriteText(content, false);
        }

        /// <summary>
        /// The write line.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public void WriteLine(string content)
        {
            this.WriteText(content + NewLine, false);
        }

        /// <summary>
        /// The append.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public void Append(string content)
        {
            this.WriteText(content, true);
        }

        /// <summary>
        /// The append line.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public void AppendLine(string content)
        {
            this.WriteText(content + NewLine, true);
        }

        /// <summary>
        /// The write text.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="append">
        /// The append.
        /// </param>
        private void WriteText(string content, bool append)
        {
            string filename = this.FileName.ToLower();
            FileAppender logger = null;
            if (!logLst.ContainsKey(filename))
            {
                Monitor.Enter(logLst);
                try
                {
                    if (!logLst.ContainsKey(filename))
                    {
                        logger = new FileAppender(this.FileName, this.Encode != null ? this.Encode : Encoding.Default);
                        logLst.Add(filename, logger);
                        Console.WriteLine("stenographer:" + count++);
                    }
                }
                finally
                {
                    Monitor.Exit(logLst);
                }
            }
            else
            {
                logger = logLst[filename];
            }

            // Debug.Assert(logger != null, "logger != null");
            if (logger != null)
            {
                logger.CallAddpender(content, append);
                logger.LastCallTime = DateTime.Now;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            string filename = this.FileName.ToLower();
            if (logLst.ContainsKey(filename))
            {
                logLst[filename].Dispose();
                logLst.Remove(filename);
                Console.WriteLine("释放" + filename);
            }
        }
    }
}
