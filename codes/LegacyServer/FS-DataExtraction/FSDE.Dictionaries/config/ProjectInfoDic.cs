#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ProjectInfoDic.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140606 by WIN .
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
using System.Linq;
using FSDE.BLL.Config;
using FSDE.Model.Config;

namespace FSDE.Dictionaries.config
{
    using System.Collections.Generic;
    using System.Linq;

    using FSDE.Model.Config;

    public class ProjectInfoDic
    {

        private Dictionary<int, ProjectInfo> projectInfos;
         
        private static ProjectInfoDic projectInfoDic =new ProjectInfoDic();
        

        private ProjectInfoDic()
        {
            if (projectInfos == null)
            {
                projectInfos = new Dictionary<int, ProjectInfo>();
                var bll = new ProjectInfoBll();
                List<ProjectInfo> list = bll.SelectList().ToList();
                foreach (var info in list)
                {
                    projectInfos.Add(Convert.ToInt32(info.Id), info);
                }
            }
        }

        
        private static readonly object Locker = new Object();

        public static ProjectInfoDic GetInstance()
        {
            if (projectInfoDic == null)
            {
                lock (Locker)
                {
                    if (projectInfoDic == null)
                    {
                        projectInfoDic = new ProjectInfoDic();
                    }
                }
            }

            return projectInfoDic;
        }


        public int AddProjectSetInfo(ProjectInfo projectSet)
        {
            var bll = new ProjectInfoBll();
            int id = bll.AddProjectInfo(projectSet);
            if (id > 0)
            {
                projectSet.Id = id;
                projectInfoDic.projectInfos.Add(id, projectSet);
                return id;
            }

            return 0;
        }

        public bool Delete(int id)
        {
            var bll = new ProjectInfoBll();
            projectInfos.Remove(id);
            return bll.Delete(id);
        }

        public bool UpdateProjectInfo(ProjectInfo projectSet)
        {
            var bll = new ProjectInfoBll();
            bool ret = bll.UpdateProjectInfo(projectSet);
            if (ret)
            {
                this.projectInfos[Convert.ToInt32(projectSet.Id)] = projectSet;
                return true;
            }
            return false;
        }

        public ProjectInfo GetProjectInfo()
        {
            return (this.projectInfos.Values.ToArray())[0];
        }

        public List<ProjectInfo> GetProjectInfos()
        {
            return projectInfoDic.projectInfos.Values.ToList();
        } 

    }
}