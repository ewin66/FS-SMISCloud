#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="FileAppender.cs" company="江苏飞尚安全监测咨询有限公司">
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
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// The file appender.
    /// </summary>
    class FileAppender : IDisposable
    {
        private readonly string fileName;

        private readonly Encoding fileEncode;

        private FileStream fileStream;

        private StreamWriter streamWriter;

        private bool isAppend; // 是否为追加模式默认false

        private const int ReTimes = 5; // 尝试读取次数

        private readonly object mutex = new object();

        /// <summary>
        /// Gets or sets the last call time.
        /// </summary>
        public DateTime LastCallTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAppender"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public FileAppender(string filename)
        {
            this.fileName = filename.Replace("/", "\\");
            this.fileEncode = Encoding.Default;
            this.CheckDirectory();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAppender"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="encode">
        /// The encode.
        /// </param>
        public FileAppender(string filename, Encoding encode)
            : this(filename)
        {
            this.fileEncode = encode;
        }

        /// <summary>
        /// The check directory.
        /// </summary>
        private void CheckDirectory()
        {
            string dir = this.fileName.Substring(0, this.fileName.LastIndexOf("\\", StringComparison.Ordinal));
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// The open file.
        /// </summary>
        /// <param name="append">
        /// The append.
        /// </param>
        /// <exception cref="Exception">
        ///  new Exception();
        /// </exception>
        private void OpenFile(bool append)
        {
            Exception ex = null;
            for (int i = 0; i < ReTimes; i++)
            {
                try
                {
                    this.fileStream = new FileStream(
                        this.fileName,
                        (append ? FileMode.Append : FileMode.Create),
                        (append ? FileAccess.Write : FileAccess.ReadWrite),
                        FileShare.Read);
                    break;
                }
                catch (Exception e)
                {
                    ex = e;
                }
            }

            if (this.fileStream == null)
            {
                if (ex != null)
                {
                    throw ex;
                }
            }

            var stream = this.fileStream;
            if (stream != null)
            {
                this.streamWriter = new StreamWriter(stream, this.fileEncode);
            }
        }

        /// <summary>
        /// The close.
        /// </summary>
        private void Close()
        {
            Monitor.Enter(this.mutex);
            try
            {
                if (this.streamWriter != null)
                {
                    this.streamWriter.Close();
                    this.streamWriter.Dispose();
                }

                if (this.fileStream != null)
                {
                    this.fileStream.Close();
                    this.fileStream.Dispose();
                }
            }
            finally
            {
                Monitor.Exit(this.mutex);
            }
        }

        /// <summary>
        /// The reset.
        /// </summary>
        private void Reset()
        {
            this.Close();
            this.OpenFile(this.isAppend);
        }

        /// <summary>
        /// The call addpender.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="append">
        /// The append.
        /// </param>
        public void CallAddpender(string content, bool append)
        {
            if (this.fileName == null || this.isAppend != append)
            {
                this.isAppend = append;
                this.Reset();
            }

            Monitor.Enter(this.mutex);
            try
            {
                this.streamWriter.Write(content);
                this.streamWriter.Flush();
            }
            finally
            {
                Monitor.Exit(this.mutex);
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }
    }
}
