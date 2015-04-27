#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ConfigCommand.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140909 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

namespace FS.SMIS_Cloud.NGDAC.Gprs.Cmd
{
    using System.Collections.Generic;

    public class ConfigCommand
    {
        public ConfigCommand()
        {
            this.AtCommands = new List<ATCommand>();
            this.AtCommands.Add(new EnterConfig());
        }
        public List<ATCommand> AtCommands { get; private set; }

        public void Add(ATCommand cmd)
        {
             this.AtCommands.Add(cmd);
        }
        public void AddRange(List<ATCommand> cmds)
        {
            this.AtCommands.AddRange(cmds);
        }
    }
}