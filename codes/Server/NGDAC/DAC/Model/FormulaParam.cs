﻿
namespace FS.SMIS_Cloud.NGDAC.Model
{
    using System;

    /// <summary>
    /// 计算参数
    /// </summary>
    [Serializable]
    public class FormulaParam 
    {
        // 公式ID
        public int FID { get; set; }

        // 参数序号
        public int Index { get; set; }

        // 参数名称
        public string Name { get; set; }

        // 参数别名（中文）
        public string Alias { get; set; }

        // 参数ID
        public int PID { get; set; }

 
    }
}
