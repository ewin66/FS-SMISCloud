#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="StenographerManager.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FS.SMIS_Cloud.DAC.Util
{
    /// <summary>
    /// The stenographer manager.
    /// </summary>
    public class StenographerManager
    {
        private static IDictionary<string, Stenographer> stenographerList = new Dictionary<string, Stenographer>();

        private static int count;

        /// <summary>
        /// The get stenographer.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="encode">
        /// The encode.
        /// </param>
        /// <returns>
        /// The <see cref="Stenographer"/>.
        /// </returns>
        private static Stenographer GetStenographer(string filename, Encoding encode)
        {
            Stenographer stenographer = null;
            if (!stenographerList.ContainsKey(filename.ToLower()))
            {
                Monitor.Enter(stenographerList);
                try
                {
                    if (!stenographerList.ContainsKey(filename.ToLower()))
                    {
                        stenographer = new Stenographer(filename, encode);
                        stenographerList.Add(filename.ToLower(), stenographer);
                        Console.WriteLine("manager:" + count++);
                    }
                }
                finally
                {
                    Monitor.Exit(stenographerList);
                }
            }
            else
            {
                stenographer = stenographerList[filename.ToLower()];
            }

            return stenographer;
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="encode">
        /// The encode.
        /// </param>
        public static void Write(string filename, string content, Encoding encode = null)
        {
            GetStenographer(filename, encode).Write(content);
        }

        /// <summary>
        /// The append.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="encode">
        /// The encode.
        /// </param>
        public static void Append(string filename, string content, Encoding encode = null)
        {
            GetStenographer(filename, encode).Append(content);
        }

        /// <summary>
        /// The append line.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="encode">
        /// The encode.
        /// </param>
        public static void AppendLine(string filename, string content, Encoding encode = null)
        {
            GetStenographer(filename, encode).AppendLine(content);
        }

        /// <summary>
        /// The clear resource.
        /// </summary>
        public static void ClearResource()
        {
            foreach (string key in stenographerList.Keys)
            {
                stenographerList[key].Dispose();
            }

            stenographerList.Clear();
        }
    }
}
