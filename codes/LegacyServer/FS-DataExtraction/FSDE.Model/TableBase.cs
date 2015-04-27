#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="TableBase.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140526 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Model
{
    using System;

    using SqliteORM;

    [Serializable]
    public abstract class TableBase<T>
    {
        public virtual int Save()
        {
            using (TableAdapter<T> adapter = TableAdapter<T>.Open())
            {
                Int64 i = adapter.CreateUpdate(this);
                if (i > 0)
                {
                    return (int)i;
                }
                return 0;
            }
        }

        public static void Delete(params object[] args)
        {
            using (TableAdapter<T> adapter = TableAdapter<T>.Open())
            {
                adapter.Delete(args);
            }
        }

        public static T Read(params object[] args)
        {
            using (TableAdapter<T> adapter = TableAdapter<T>.Open())
            {
                return adapter.Read(args);
            }
        }



    }
}