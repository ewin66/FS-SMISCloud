#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="Adapter.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150310 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FS.SMIS_Cloud.NGDAC.Model
{
    public class Adapter
    {
        public Adapter(uint protocol, string classname)
        {
            this.Protocol = protocol;
            this.ClassName = classname;
        }

        public uint Protocol { get; private set; }

        public string ClassName { get; private set; }

        public string ScriptPath { get; set; }
    }
}