using System;
using System.Collections.Generic;

namespace FS.SMIS_Cloud.DAC.Model
{
    public interface ISensorData
    {
        /// <summary>
        /// 原始采集数据
        /// </summary>
        double[] RawValues { get; }
        
        /// <summary>
        /// 这是经过计算转换后的数据
        /// </summary>
        double[] PhyValues { get; }

        /// <summary>
        /// 主题数据(过滤后的数据和二次计算后的数据组合) 
        /// </summary>
        IList<double?> ThemeValues { get;}
        
        /// <summary>
        /// Json数据
        /// </summary>
        string JsonResultData { get; }

        /// <summary>
        /// 用于判断是否超量程范围
        /// </summary>
        double[] CollectPhyValues { get; }

        /// <summary>
        /// 删除主题数据
        /// </summary>
        /// <param name="colphyindex"></param>
        void DropThemeValue(int colphyindex);

        /// <summary>
        /// 存储数据标识
        /// </summary>
        bool IsSaveDataOriginal { get; }
    }
}
