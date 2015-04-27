// // --------------------------------------------------------------------------------------------
// // <copyright file="ProjectInfoDal.cs" company="江苏飞尚安全监测咨询有限公司">
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

using System;
using System.Linq;
using FSDE.IDAL;
using FSDE.Model.Config;
using SqliteORM;

namespace FSDE.DAL.Config
{
    public class ProjectInfoDal : IProjectInfo
    {

        public int Add(ProjectInfo project)
        {
            using (DbConnection conn = new DbConnection())
            {
                return project.Save();
            }
        }

        public bool Delete(int id)
        {
            using (DbConnection conn = new DbConnection())
            {
                try
                {
                    ProjectInfo.Delete(Where.Equal("ID", id));
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool UpDate(ProjectInfo project)
        {
            using (DbConnection conn = new DbConnection())
            {
                if (project.Save() > 0)
                {
                    return true;
                }
                return false;
            }
        }

        public System.Collections.Generic.IList<ProjectInfo> GetProjectList()
        {
            using (DbConnection conn = new DbConnection())
            {
                using (TableAdapter<ProjectInfo> adapter = TableAdapter<ProjectInfo>.Open())
                {
                    return adapter.Select().ToList();
                }
            }
        }

        public bool DeleteList(string projectids)
        {
            throw new System.NotImplementedException();
        }
    }
}