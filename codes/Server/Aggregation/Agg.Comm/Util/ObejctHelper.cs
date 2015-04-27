// // --------------------------------------------------------------------------------------------
// // <copyright file="ObejctHelper.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150316
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System;

namespace Agg.Comm.Util
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public class ObjectHelper
    {
        /// <summary>
        /// 对象深拷贝
        /// </summary>
        /// <param name="source">源对象</param>
        /// <returns>拷贝对象</returns>
        public static T DeepCopy<T>(T source)
        {
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(ms, source);
            ms.Seek(0, SeekOrigin.Begin);

            var result = (T)bf.Deserialize(ms);
            return result;
        }
    }
}