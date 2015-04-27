// --------------------------------------------------------------------------------------------
// <copyright file="HighchartsConvert.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：HightCharts图片生成类
// 
// 创建标识：liuxinyi20140529
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

using System.Reflection;

namespace HightChartsGenerator.NET
{
    using System;
    using System.Diagnostics;//使用process类
    using System.IO;
    using System.Text;

    public static class HighchartsConvert
    {//C#调用外部程序Process类
        //在程序开发中，一个程序经常需要去调用其他的程序，C#中Process类正好提供了这样的功能，
        //它提供对本地和远程进程的访问并使你能启动和停止本地系统进程。
        //private static Process phantomjsProcess;

        private static Process PhantomjsProcess
        {
            get
            {
                var phantomjsProcess = new Process();
                phantomjsProcess.StartInfo.FileName = @"ChartConvert\phantomjs.exe";
                phantomjsProcess.StartInfo.CreateNoWindow = true;
                phantomjsProcess.StartInfo.UseShellExecute = false;
                //重定向标准输出 
                phantomjsProcess.StartInfo.RedirectStandardOutput = true;
                phantomjsProcess.StartInfo.RedirectStandardInput = true;
                //重定向错误输出 
                phantomjsProcess.StartInfo.RedirectStandardError = false;
                phantomjsProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;

                return phantomjsProcess;
            }
        }

        /// <summary>
        /// 根据配置项生成图表
        /// </summary>
        /// <param name="option"></param>
        /// <param name="rslt"></param>
        /// <returns></returns>
        public static Stream GenerateViaOption(string option, out string[] rslt)
        {
            if (!Directory.Exists("ChartConvert/Temp"))
            {
                Directory.CreateDirectory("ChartConvert/Temp");
            }
            //Guid.NewGuid()全球唯一标识符，是一个字母数字标识符，用于指示产品的唯一性安装
            //GUID 的格式为“xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx”，其中每个 x 是 0-9 或 a-f 范围内的一个32位十六进制数。
            string optionFile = "ChartConvert/Temp/" + Guid.NewGuid() + ".json";
            //FileStream(String, FileMode, FileAccess) 使用指定的路径、创建模式和读写权限初始化Filestream类的新实例
            using (var fs = new FileStream(optionFile, FileMode.Create, FileAccess.Write))
            {
                using (var sw = new StreamWriter(fs, Encoding.UTF8))
                {
                    //StreamWriter的Write方法是直接将string写入到文件中
                    sw.Write(option);
                    sw.Close();
                }
                fs.Close();
            }
            //optionFile 是JSON文件，包含图的配置信息，option是图的配置信息
            var stream = GenerateViaOptionFile(optionFile, out rslt);
            File.Delete(optionFile);//删除产生的临时JSON文件
            return stream;//返回输出的内存流图片
        }

        /// <summary>
        /// 根据配置项文件生成图表
        /// </summary>        
        /// <param name="optionFileName">配置项文件</param>        
        /// <param name="rslt">处理信息</param>
        /// <returns>图片流</returns>
        public static Stream GenerateViaOptionFile(string optionFileName, out string[] rslt)
        {
            //定义图片名称
            string filename = Guid.NewGuid().ToString();
            string outfile = @"ChartConvert/Image/" + filename + ".png";
            string infile = optionFileName; //输入图片的参数，可以是JSON文件或JS文件，此处使用的是JSON格式文本

            string excuteArg = @"ChartConvert/highcharts-convert.js"
                               + " -infile " + infile
                               + " -outfile " + outfile
                               + " -scale 5 -width 700 -constr Chart";
            Process phantomjsProcess = PhantomjsProcess;
            phantomjsProcess.StartInfo.Arguments = excuteArg;
            if (!phantomjsProcess.Start())
            {
                throw new Exception("phantomjs无法启动");
            }
            rslt = phantomjsProcess.StandardOutput.ReadToEnd().Split(new[] { '\r', '\n' });//图形生成信息及可能出错的信息
            phantomjsProcess.WaitForExit();
            phantomjsProcess.Close();
            var stream = new MemoryStream();
            if (File.Exists(outfile))
            {
                using (var fs = new FileStream(outfile, FileMode.Open, FileAccess.Read))
                {
                    //rslt = new string[] {outfile};
                    fs.CopyTo(stream); //CopyTo(stream)从当前流中读取字节并将其写入到另一种流中。
                    fs.Close();
                }
              File.Delete(outfile);                
            }
            //else
            //{
            //    rslt = new string[] {"图形生成失败。"};
            //}
            return stream;
        }
    }
}