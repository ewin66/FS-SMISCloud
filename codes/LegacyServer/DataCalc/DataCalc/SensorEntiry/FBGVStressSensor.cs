using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    // 虚拟类型传感器数据直接接入(非计算)
    class FBGVStressSensor:Sensor
    {
        public FBGVStressSensor(DataRow dr) : base(dr)
        {
            int.TryParse(dr["MODULE_NO"].ToString(), out _moudleNo);
            int.TryParse(dr["DAI_CHANNEL_NUMBER"].ToString(), out _channelNo);
        }

        #region Properties
        private int _moudleNo;
        public int MoudleNo
        {
            get { return _moudleNo; }
            set { _moudleNo = value; }
        }
        private int _channelNo;
        public int ChannelNo
        {
            get { return _channelNo; }
            set { _channelNo = value; }
        }
        #endregion

        public override Model.Data CalcValue(System.Data.DataRow dr)
        {
            var stress = Convert.ToSingle(dr["Value1"]);//应力
            return new DataBeamForce()
            {
                Safetyfactor = SafetyFactor,
                SensorId = Convert.ToInt32(Id),
                CollectTime = dr.Field<DateTime>("CollectTime"),
                stress = stress
            };
        }
    }
}
