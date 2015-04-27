#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="FileService.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140412 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.DAC.Util
{
    /// <summary>
    /// The file service.
    /// </summary>
    public class FileService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileService"/> class.
        /// </summary>
        public FileService()
        {
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="value">
        /// The value.
        /// </param>
        public void Write(string filepath, string value)
        {
            StenographerManager.AppendLine(filepath, value);
        }
    }
}