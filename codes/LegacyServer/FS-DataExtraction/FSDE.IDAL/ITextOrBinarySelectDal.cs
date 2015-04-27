#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ITextOrBinarySelectDal.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140623 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.IDAL
{
    using System.Collections.Generic;

    using FSDE.Model;
    using FSDE.Model.Config;

    public interface ITextOrBinarySelectDal
    {
        IList<Data> TextOrBinarySelect(DataBaseName path);
    }
}