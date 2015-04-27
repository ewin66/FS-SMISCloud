// --------------------------------------------------------------------------------------------
// <copyright file="IArithmetic.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：算法接口
// 
// 创建标识：刘歆毅20140217
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Arithmetic
{
    using System.Collections.Generic;

    using FreeSun.FS_SMISCloud.Server.DataCalc.Model;

    /// <summary>
    /// 算法接口
    /// </summary>
    public interface IArithmetic
    {
        /// <summary>
        /// 初始化算法参数
        /// </summary>
        /// <returns></returns>
        bool Initial();

        /// <summary>
        /// 二次计算核心方法
        /// </summary>
        /// <param name="rawData">原始数据</param>
        /// <returns>计算后的数据</returns>
        void Calculate(Dictionary<int, Data> rawData);
    }
}