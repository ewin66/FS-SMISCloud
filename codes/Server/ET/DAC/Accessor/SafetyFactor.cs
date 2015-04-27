namespace FS.SMIS_Cloud.DAC.Accessor
{
    /// <summary>
    /// 安全检测因素
    /// </summary>
    public enum SafetyFactor
    {
        /// <summary>
        /// 温湿度
        /// </summary>
        TempHumidity = 5,

        /// <summary>
        /// 降雨量
        /// </summary>
        Rainfall = 6,

        /// <summary>
        /// 表面位移
        /// </summary>
        GpsSurfaceDisplacement = 9,

        /// <summary>
        /// 内部位移
        /// </summary>
        DeepDisplacement = 10,

        /// <summary>
        /// 沉降
        /// </summary>
        Settlement = 11,

        /// <summary>
        /// 孔隙水压力
        /// </summary>
        StressStrainPoreWaterPressure = 12,

        /// <summary>
        /// 重要挡土墙应变
        /// </summary>
        ForceSteelbar = 13,

        /// <summary>
        /// 土体压力
        /// </summary>
        ForceEarthPressure = 14,

        /// <summary>
        /// 挡土墙钢筋受力监测
        /// </summary>
        ForceSteelbarRetainingWall = 15,

        /// <summary>
        /// 锚杆受力监测
        /// </summary>
        ForceAnchor = 16, 

        /// <summary>
        /// 地下水位监测
        /// </summary>
        WaterLevel = 17,

        /// <summary>
        /// 风速风向监测
        /// </summary>
        Wind2D = 18,

        /// <summary>
        /// 塔顶偏位监测
        /// </summary>
        TopOfTowerDeviation = 19,

        /// <summary>
        /// 桥墩倾斜监测
        /// </summary>
        DeformationDeepDisplacement = 20,

        /// <summary>
        /// 梁段挠度监测
        /// </summary>
        DeformationBridgeDeflection = 21,

        /// <summary>
        /// 桥梁伸缩缝监测
        /// </summary>
        DeformationCrackJoint = 22,

        /// <summary>
        /// 应力应变监测
        /// </summary>
        Forcesteelbar = 23,

        /// <summary>
        /// 表面位移监测
        /// </summary>
        LvdtSurfaceDisplacement = 25,

        /// <summary>
        /// 温度监测
        /// </summary>
        Temp = 26,

        /// <summary>
        /// 桥面振动监测
        /// </summary>
        VibrationDeckVibration = 27,

        /// <summary>
        /// 裂缝监测
        /// </summary>
        DeformationCrack = 28,

        /// <summary>
        /// 索力监测
        /// </summary>
        CableForce = 29,

        /// <summary>
        /// 风速风向风仰角监测
        /// </summary>
        Wind3D = 30,

        /// <summary>
        /// 沉降监测(带分组)
        /// </summary>
        SettlementGroup = 31,

        /// <summary>
        /// 支撑轴力监测
        /// </summary>
        ForceAxial = 32,

        /// <summary>
        /// 振动监测
        /// </summary>
        Vibration = 33,

        /// <summary>
        /// 浸润线监测
        /// </summary>
        SaturationLine = 34,

        /// <summary>
        /// 干滩和库水位监测
        /// </summary>
        Beach = 35,

        /// <summary>
        /// 渗流监测
        /// </summary>
        Seepage = 36,

        /// <summary>
        /// 衬砌应变监测
        /// </summary>
        LiningStress=37,

        /// <summary>
        /// 衬砌受压力
        /// </summary>
        IningPressure=38
    }
}
