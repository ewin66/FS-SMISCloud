// // --------------------------------------------------------------------------------------------
// // <copyright file="DirectoryHelper.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20141030
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace ReportGeneratorService.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public class DirectoryHelper
    {
        /// <summary>
        /// 创建多层文件夹
        /// </summary>
        /// <param name="directoryName">文件夹名称</param>
        /// <returns></returns>
        public static bool CreateDirectory(string directoryName)
        {
            char separator = Path.DirectorySeparatorChar;
            string path  = directoryName;
            string rootPath = Path.GetPathRoot(path);

            if (!Directory.Exists(rootPath)) return false;

            if (Directory.Exists(path)) 
                return true;
            //List<string> directorys = 
            string[] directorys = path.Split(separator);
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var directory in directorys)
                {
                    sb.Append(directory);
                    sb.Append(separator);
                    ////根路径不需要创建
                    if (sb.ToString() == rootPath) 
                        continue;

                    if (!Directory.Exists(sb.ToString()))
                    {
                        Directory.CreateDirectory(sb.ToString());
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return true;
        }
    }
}