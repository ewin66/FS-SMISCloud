using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Web;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using Newtonsoft.Json;


namespace SecureCloud.Support
{
    /// <summary>
    /// MultiDownLoadReport 的摘要说明
    /// </summary>
    public class MultiDownLoadReport : IHttpHandler
    {
        private ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public void ProcessRequest(HttpContext context)
        {
          context.Response.ContentType = "application/javascript";
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();
                string multiDownRptParams = context.Server.HtmlDecode(context.Request.QueryString["multiDownRptParams"]);
                var rptIdArray = multiDownRptParams.Split('@');
                var fileUrlArray = new List<string>();
                foreach (var rptId in rptIdArray)
                {
                    string json = MultiDelReport.HttpGet(context, rptId);
                    if (json == "[]" || json == null || json == "null" || json == string.Empty)
                    {
                        continue;
                    }
                    IList<ReportInfo> jsonArray = JsonConvert.DeserializeObject<IList<ReportInfo>>(json);
                    var data = jsonArray[0];
                    var Status = data.status;
                    var downRptUrl = string.Empty;
                    if (Status == "0")
                    {
                        downRptUrl = data.UnconfirmedUrl;
                    }
                    else 
                    {
                        downRptUrl = data.ConfirmedUrl;
                    }
                    if (File.Exists(downRptUrl))
                    {
                        fileUrlArray.Add(downRptUrl);
                    }
                }
                //剔除重复的元素
                fileUrlArray = fileUrlArray.Distinct().ToList();
                if (!fileUrlArray.Any())
                {
                    context.Response.ContentType = "text/html";
                    context.Response.Write("<script  language='javascript'>window.alert('待下载的文件被移动或删除！');window.close(); </script>");
                }
                else
                {
                    Stopwatch watch2 = new Stopwatch();
                    watch2.Start();
                    string rootPath = Path.GetTempPath();
                    string zipFileName = "report_" + (DateTime.Now.Ticks / 10000);
                    string tempDirectory = rootPath + "\\" + zipFileName;//压缩目录
                    string zipFullName = tempDirectory + ".zip";//压缩文件全名
                    string downloadAsName = "监测报表.zip";

                    if (Directory.Exists(tempDirectory))
                    {
                        Directory.Delete(tempDirectory, true);
                    }
                    Directory.CreateDirectory(tempDirectory);
                    foreach (var fileName in fileUrlArray)
                    {
                        var zipFileFullName = tempDirectory + "\\" + new FileInfo(fileName).Name;
                        if (File.Exists(fileName))
                        {
                            File.Copy(fileName, zipFileFullName);
                        }
                    }
                    watch2.Stop();
                    string time2 = watch2.ElapsedMilliseconds.ToString();
                    logger.Debug(string.Format("拷贝文件..总耗时..{0} 毫秒\r\n", time2));
                    Stopwatch watch3 = new Stopwatch();
                    watch3.Start();
                    CreateZip(tempDirectory, zipFullName);
                    watch3.Stop();
                    string time3 = watch3.ElapsedMilliseconds.ToString();
                    logger.Debug(string.Format("压缩文件..总耗时..{0} 毫秒\r\n", time3));
                    var tempPathDir = new DirectoryInfo(tempDirectory);
                    tempPathDir.Delete(true);
                    Stopwatch watch4 = new Stopwatch();
                    watch4.Start();
                    //var speed = 1*1024*1024; //1M
                    var speed = 0;
                    Int32.TryParse(ConfigurationManager.AppSettings["MultiDownloadSpeed"], out speed);
                    DownLoad.DownloadFile(context, zipFullName, speed * 1024 * 1024, downloadAsName);
                    watch4.Stop();
                    string time4 = watch4.ElapsedMilliseconds.ToString();
                    logger.Debug(string.Format("下载文件..总耗时..{0} 毫秒\r\n", time4));
                    watch.Stop();
                    string time = watch.ElapsedMilliseconds.ToString();
                    logger.Debug(string.Format("批量下载..总耗时..{0} 毫秒\r\n", time));
                    //删除临时文件
                    var zipFile = new FileInfo(zipFullName);
                    zipFile.Delete();
                }
            }
            catch (Exception)
            {
                context.Response.ContentType = "text/html";
                context.Response.Write("<script  language='javascript'>window.alert('下载文件时发生异常!');window.history.go(-2);window.opener.location.reload(); " + "</ " + "script>");
            }
        }

        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="destinationZipFilePath"></param>
        public static void CreateZip(string sourceFilePath, string destinationZipFilePath)
        {
            if (sourceFilePath[sourceFilePath.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                sourceFilePath += System.IO.Path.DirectorySeparatorChar;
            ZipOutputStream zipStream = new ZipOutputStream(File.Create(destinationZipFilePath));
            zipStream.SetLevel(6);  // 压缩级别 0-9
            CreateZipFiles(sourceFilePath, zipStream, sourceFilePath);
            zipStream.Finish();
            zipStream.Close();
        }

        /// <summary>
        /// 递归压缩文件
        /// </summary>
        /// <param name="sourceFilePath">待压缩的文件或文件夹路径</param>
        /// <param name="zipStream">打包结果的zip文件路径（类似 D:\WorkSpace\a.zip）,全路径包括文件名和.zip扩展名</param>
        /// <param name="staticFile"></param>
        private static void CreateZipFiles(string sourceFilePath, ZipOutputStream zipStream, string staticFile)
        {
            Crc32 crc = new Crc32();
            string[] filesArray = Directory.GetFileSystemEntries(sourceFilePath);
            foreach (string file in filesArray)
            {
                if (Directory.Exists(file))                     //如果当前是文件夹，递归
                {
                    CreateZipFiles(file, zipStream, staticFile);
                }

                else                                            //如果是文件，开始压缩
                {
                    FileStream fileStream = File.OpenRead(file);

                    byte[] buffer = new byte[fileStream.Length];
                    fileStream.Read(buffer, 0, buffer.Length);
                    string tempFile = file.Substring(staticFile.LastIndexOf("\\") + 1);
                    ZipEntry entry = new ZipEntry(tempFile);

                    entry.DateTime = DateTime.Now;
                    entry.Size = fileStream.Length;
                    fileStream.Close();
                    crc.Reset();
                    crc.Update(buffer);
                    entry.Crc = crc.Value;
                    zipStream.PutNextEntry(entry);

                    zipStream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}