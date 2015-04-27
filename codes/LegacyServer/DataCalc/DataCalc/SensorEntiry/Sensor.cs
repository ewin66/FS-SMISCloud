using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    /// <summary>
    /// 监测因素枚举
    /// </summary>
    public enum SAFE_FACT
    {
        /// <summary>
        /// 温湿度监测
        /// </summary>
        humiture = 5,
        /// <summary>
        /// 降雨量监测
        /// </summary>
        rainfall = 6,
        /// <summary>
        /// 表面位移监测
        /// </summary>
        surf_displace = 9,
        /// <summary>
        /// 内部位移监测
        /// </summary>
        deep_displace = 10,
        /// <summary>
        /// 沉降监测
        /// </summary>
        settle = 11,
        /// <summary>
        /// 孔隙水压力监测
        /// </summary>
        pore_water_pressure = 12,
        /// <summary>
        /// 重要挡土墙应变监测
        /// </summary>
        retaining_wall_strain,
        /// <summary>
        /// 土体压力监测
        /// </summary>
        ssoil_pressure,
        /// <summary>
        /// 挡土墙钢筋受力监测
        /// </summary>
        steel_bar_stress,
        /// <summary>
        /// 锚杆受力监测
        /// </summary>
        anchor_rod_stress,
        /// <summary>
        /// 地下水位监测
        /// </summary>
        water_level,
        /// <summary>
        /// 风速风向监测
        /// </summary>
        wind2,
        /// <summary>
        /// 塔顶偏位监测
        /// </summary>
        bridge_settle,
        /// <summary>
        /// 桥墩倾斜监测
        /// </summary>
        bridge_incline,
        /// <summary>
        /// 梁段挠度监测
        /// </summary>
        bridge_deflection,
        /// <summary>
        /// 桥梁伸缩缝监测
        /// </summary>
        bridge_crack,
        /// <summary>
        /// 应力应变监测
        /// </summary>
        concrete_strain,
        /// <summary>
        /// 表面位移监测
        /// </summary>
        stayguy = 25,
        /// <summary>
        /// 温度监测
        /// </summary>
        temperature,
        /// <summary>
        /// 桥面振动监测
        /// </summary>
        bridge_vibration,
        /// <summary>
        /// 裂缝监测
        /// </summary>
        crack,
        /// <summary>
        /// 索力监测
        /// </summary>
        cable_force,
        /// <summary>
        /// 风速风向风仰角监测
        /// </summary>
        wind3,
        /// <summary>
        /// 沉降监测
        /// </summary>
        settle2,
        /// <summary>
        /// 支撑轴力监测
        /// </summary>
        axial_force,
        /// <summary>
        /// 振动监测
        /// </summary>
        vibration,
        /// <summary>
        /// 浸润线监测
        /// </summary>
        saturation_line,
        /// <summary>
        /// 干滩监测
        /// </summary>
        beach,
        /// <summary>
        /// 渗流监测
        /// </summary>
        seepage,
        /// <summary>
        /// 衬砌应变监测
        /// </summary>
        liningstress,
        /// <summary>
        /// 衬砌受压力
        /// </summary>
        liningpressure,
        /// <summary>
        /// 建筑物倾斜
        /// </summary>
        inclinedbuilding,
        /// <summary>
        /// 拱顶沉降监测
        /// </summary>
        crownsettlement,
        /// <summary>
        /// 净空收敛监测
        /// </summary>
        clearanceconvergence,
        /// <summary>
        /// 支座位移
        /// </summary>
        bearingdisplace = 50,
        /// <summary>
        /// 杆件应变监测
        /// </summary>
        beamstrain,
        /// <summary>
        /// 杆件应力监测
        /// </summary>
        beamstress,
        /// <summary>
        /// 焊缝应变监测
        /// </summary>
        weldsstrain,
        /// <summary>
        /// 网壳振动监测
        /// </summary>
        shellvibration,
    }

    public enum AcqistionType : byte
    {
        Broadcast = 1,
        Read = 2,
        Write = 3,
        Collect = 4,
        UnKnown
    }

    public interface ISensor
    {
    }

    /// <summary>
    /// 传感器基类
    /// </summary>
    public abstract class Sensor : ISensor
    {
        #region Constructors
        /// <summary>
        /// 配置项构造函数
        /// </summary>
        /// <param name="dr"></param>
        protected Sensor(System.Data.DataRow dr)
        {
            Id = dr["SENSOR_ID"].ToString();
            ProtocolType = Convert.ToInt32(dr["PROTOCOL_CODE"]);
            ProductType = Convert.ToInt32(dr["PRODUCT_TYPE_KEY"]);
            _structId = Convert.ToInt32(dr["STRUCT_ID"]);
            _dtuId = Convert.ToInt32(dr["REMOTE_DTU_NUMBER"]);
            int safefact = Convert.ToInt32(dr["SAFETY_FACTOR_TYPE_ID"]);
            int.TryParse(dr["FORMAULAID"].ToString(), out _formaulaid);
            _safetyFactor = (SAFE_FACT)safefact;
        }
        #endregion

        #region Properties
        /// <summary>
        /// 监测因素
        /// </summary>
        private SAFE_FACT _safetyFactor;
        public SAFE_FACT SafetyFactor
        {
            get { return _safetyFactor; }
            set { _safetyFactor = value; }
        }
        /// <summary>
        /// 传感器ID
        /// </summary>
        private string _id;
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// 公式编号
        /// </summary>
        private int _formaulaid;
        public int Formaulaid
        {
            get { return _formaulaid; }
            set { _formaulaid = value; }
        }

        /// <summary>
        /// 单位
        /// </summary>
        private string _unit;
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }

        /// <summary>
        /// 协议类型
        /// </summary>
        private int _protocolType;
        public int ProtocolType
        {
            get { return _protocolType; }
            set
            {
                _protocolType = value;
            }
        }

        /// <summary>
        /// 传感器产品类型
        /// </summary>
        private int _productType;
        public int ProductType
        {
            get { return _productType; }
            set { _productType = value; }
        }

        /// <summary>
        /// 归属结构物ID
        /// </summary>
        private int _structId;
        public int StructID
        {
            get { return _structId; }
            set { _structId = value; }
        }
        /// <summary>
        /// 归属结构物ID
        /// </summary>
        private int _dtuId;
        public int DTUId
        {
            get { return _dtuId; }
            set { _dtuId = value; }
        }
        #endregion

        #region Methods
        public virtual Data CalcValue(System.Data.DataRow dr)
        {
            return null;
        }
        #endregion
    }
}
