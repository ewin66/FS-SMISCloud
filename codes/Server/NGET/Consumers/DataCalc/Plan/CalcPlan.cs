using System;
using FS.SMIS_Cloud.NGET.Util;

namespace FS.SMIS_Cloud.NGET.DataCalc.Plan
{
    using System.Collections.Generic;
    using System.Linq;

    using FS.SMIS_Cloud.NGET.DataCalc.Algorithm;
    using FS.SMIS_Cloud.NGET.DataCalc.Model;
    using FS.SMIS_Cloud.NGET.Model;

    class CalcPlan
    {
        private SensorGroup _calcGroup;
        private int _acqNum;

        public CalcPlan(SensorGroup gp, int acqNum)
        {
            this._calcGroup = gp;
            this._acqNum = acqNum;
        }

        public int GetAcqNum()
        {
            return this._acqNum;
        }

        /// <summary>
        /// 将本轮采集内容加入到工作计划中
        /// </summary>
        public bool AddToPlan(SensorAcqResult sensordata,out IEnumerable<SensorAcqResult> calcres)
        {
            calcres = null;
            var groupItem =
                (from gi in _calcGroup.Items where (gi.SensorId == sensordata.Sensor.SensorID) && !gi.Paramters.ContainsKey("hasValue") select gi).FirstOrDefault();
            if (groupItem != null)
            {
                groupItem.Paramters.Add("hasValue", true);
                if (sensordata != null && sensordata.IsOK && sensordata.Data != null)
                {
                    groupItem.Paramters.Add("value", sensordata); // 计算参数放在组内元素的参数value列中(引用传递)
                }
                if (IsReady())//数据完整,开始计算
                {
                    calcres = DoCalc();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断本轮是否满足计算
        /// </summary>
        /// <returns></returns>
        private bool IsReady()
        {
            return _calcGroup.Items.All(a => a.Paramters.ContainsKey("hasValue"));
        }

        /// <summary>
        /// 本轮数据不全，将把本来组内数据置为Error,同时在Group中深拷贝备份
        /// </summary>
        private void SaveAndMakeCurrentNotAvailable()
        {
            foreach (var item in _calcGroup.Items)
            {
                if (item.Paramters.ContainsKey("value"))
                {
                    var p = item.Paramters["value"] as SensorAcqResult;
                    var data = ObjectHelper.DeepCopy(p);
                    item.Paramters["value"] = data;
                    p.ErrorCode = (int)Errors.ERR_DEFAULT;
                }
            }
        }

        /// <summary>
        /// 执行计算
        /// </summary>
        public IEnumerable<SensorAcqResult> DoCalc()
        {
            var sensordatum = new List<SensorAcqResult>();
            AlgorithmFactory.CreateAlgorithm(this._calcGroup).CalcData(sensordatum);
            if (this._calcGroup.GroupType != GroupType.VirtualSensor)
            {
                foreach (var groupItem in this._calcGroup.Items)
                {
                    if (groupItem.Paramters.ContainsKey("value"))
                    {
                        var sar = (SensorAcqResult)groupItem.Paramters["value"];
                        if (sar.IsOK)
                            sensordatum.Add(sar);
                    }
                }
            }
            _calcGroup.ClearParams();
            return sensordatum;
        }
    }
}
