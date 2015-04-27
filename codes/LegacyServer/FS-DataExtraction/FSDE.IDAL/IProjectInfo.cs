﻿// // --------------------------------------------------------------------------------------------
// // <copyright file="IProjectInfo.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140609
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System.Collections.Generic;
using FSDE.Model.Config;

namespace FSDE.IDAL
{
    public interface IProjectInfo
    {
        int Add(ProjectInfo project);
        bool Delete(int projectID);
        bool UpDate(ProjectInfo project);
        IList<ProjectInfo> GetProjectList();
        bool DeleteList(string projectids);
    }
}