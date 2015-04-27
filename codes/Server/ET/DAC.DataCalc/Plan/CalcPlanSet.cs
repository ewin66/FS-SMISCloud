using System.Collections.Concurrent;
using System.Collections.Generic;
using FS.SMIS_Cloud.DAC.DAC;
using FS.SMIS_Cloud.DAC.DataCalc.Model;

namespace FS.SMIS_Cloud.DAC.DataCalc.Plan
{
    static class CalcPlanSet
    {
        private static ConcurrentDictionary<int, CalcPlan> _calcPlans = new ConcurrentDictionary<int, CalcPlan>();

        public static int Update(uint dtuId, SensorGroup group, IList<SensorAcqResult> acqRes, string taskId)
        {
            int calccnt = 0;
            if (_calcPlans.ContainsKey(group.GroupId))
            {
                var plan = _calcPlans[group.GroupId];
                IList<SensorAcqResult> calRes = null;
                if (plan.GetGuid() == taskId)
                {
                    plan.AddToPlan(dtuId, acqRes);
                    if (plan.IsReady())
                    {
                        if (plan.DoCalc(out calRes))
                        {
                            calccnt++;
                        }
                        if (calRes != null)
                        {
                            foreach (var sar in calRes)
                            {
                                acqRes.Add(sar);
                            }
                        }
                        CalcPlan cp;
                        _calcPlans.TryRemove(group.GroupId, out cp);
                    }
                }
                else //new round acquisition
                {
                    if (plan.DoCalc(out calRes))
                    {
                        calccnt++;
                    }
                    CalcPlan cp;
                    _calcPlans.TryRemove(group.GroupId, out cp);
                    var nplan = _calcPlans.GetOrAdd(group.GroupId, new CalcPlan(group, taskId));
                    nplan.AddToPlan(dtuId, acqRes);
                    if (calRes != null)
                    {
                        foreach (var sar in calRes)
                        {
                            acqRes.Add(sar);
                        }
                    }
                }
            }
            else
            {
                var nplan = _calcPlans.GetOrAdd(group.GroupId, new CalcPlan(group, taskId));
                nplan.AddToPlan(dtuId, acqRes);
            }
            return calccnt;
        }
    }
}
