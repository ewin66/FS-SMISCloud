﻿#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="CommunicationCreate.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：20140303 created by Win
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace Communication
{
    /// <summary>
    /// The communication create.
    /// </summary>
    public abstract class CommunicationCreate
    {
        /// <summary>
        /// The create communication factory.
        /// </summary>
        /// <param name="port">
        /// The port.
        /// </param>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <returns>
        /// The <see cref="ICommunicationBase"/>.
        /// </returns>
        public abstract ICommunicationBase CreateCommunicationFactory(string port, int[] config);
    }
}