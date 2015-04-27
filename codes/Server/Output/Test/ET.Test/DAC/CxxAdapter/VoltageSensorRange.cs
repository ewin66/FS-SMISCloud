namespace FS.SMIS_Cloud.NGDAC.DAC.CxxAdapter
{
    using System.ComponentModel;

    public enum VoltageSensorRange
    {
        /// <summary>
        /// 0~5V
        /// </summary>
        [Description("FS-LF10")]
        FSLF10,

        /// <summary>
        ///  -5~5V
        /// </summary>
        [Description("FS-LF25")]
        FSLF25,

        /// <summary>
        /// 0~10V
        /// </summary>
        [Description("FS-LF50")]
        FSLF50,

        /// <summary>
        /// -10~10V
        /// </summary>
        [Description("FS-LF100")]
        FSLF100,


        /*
         * 2.0 P4添加(按量程区分的LVDT产品型号)
         * */
        /// <summary>
        /// 0~5V
        /// </summary>
        [Description("FS-LFV-V0P5")]
        FS_LFV_V0P5,

        /// <summary>
        ///  -5~5V
        /// </summary>
        [Description("FS-LFV-VM5P5")]
        FS_LFV_VM5P5,

        /// <summary>
        /// 0~10V
        /// </summary>
        [Description("FS-LFV-V0P10")]
        FS_LFV_V0P10,

        /// <summary>
        /// -10~10V
        /// </summary>
        [Description("FS-LFV-VM10P10")]
        FS_LFV_VM10P10
    }
}