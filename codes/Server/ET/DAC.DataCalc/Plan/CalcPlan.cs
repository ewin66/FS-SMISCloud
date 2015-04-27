using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Algorithm;
using FS.SMIS_Cloud.DAC.DataCalc.Model;
using FS.SMIS_Cloud.DAC.Node;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.DataCalc.Plan
{
    class CalcPlan
    {
        private SensorGroup _calcGroup;
        private string _roundGuid;

        private Dictionary<uint, bool> DtuJobStatus = new Dictionary<uint, bool>();

        private static readonly ILog _logger = LogManager.GetLogger("DataCalc");

        public CalcPlan(SensorGroup gp, string taskId)
        {
            _calcGroup = gp;
            _roundGuid = taskId;
            var dtus = gp.GetDtuIdList();
            foreach (var dtu in dtus)
            {
                DtuJobStatus.Add(dtu, false);
            }
        }

        public string GetGuid()
        {
            return _roundGuid;
        }

        public SensorGroup GetSensorGroup()
        {
            return _calcGroup;
        }

        /// <summary>
        /// 将本轮采集内容加入到工作计划中
        /// </summary>
        public void AddToPlan(uint dtuId, IList<SensorAcqResult> acqRes)
        {
            DtuJobStatus[dtuId] = true;
            var groupItems = _calcGroup.GetAllItems();
            foreach (var groupItem in groupItems)
            {
                var sensordata =
                    (from s in acqRes where s.Sensor.SensorID == groupItem.SensorId select s)
                        .FirstOrDefault();
                if (sensordata != null && sensordata.IsOK && sensordata.Data != null && sensordata.Data.ThemeValues != null)
                {
                    var data = ObjectHelper.DeepCopy(sensordata);
                    groupItem.Value = data;
                    sensordata.CalcPlanState = (int)CalcPlanState.AddToPlan;
                }
            }
        }

        /// <summary>
        /// 刷新状态，如果满足计算条件则进行计算
        /// </summary>
        public bool IsReady()
        {
            return DtuJobStatus.All(status => status.Value);
        }

        /// <summary>
        /// 执行计算
        /// </summary>
        public bool DoCalc(out IList<SensorAcqResult> sensordatum)
        {
            sensordatum = new List<SensorAcqResult>();
            bool flag = false;
            try
            {
                flag = DataCalc.Calc(_calcGroup, sensordatum);
            }
            catch (Exception ex)
            {
                _logger.WarnFormat("二次计算出现异常 GROUP_ID={0} GROUP_TYPE={1} ERR={2}", _calcGroup.GroupId, _calcGroup.GroupType, ex.Message);
                flag = false;
            }
            var items = _calcGroup.GetAllItems();
            foreach (var groupItem in items)
            {
                if (groupItem.Value != null && groupItem.Value.IsOK)
                {
                    sensordatum.Add(groupItem.Value);
                }
            }
            return flag;
        }
    }
}
