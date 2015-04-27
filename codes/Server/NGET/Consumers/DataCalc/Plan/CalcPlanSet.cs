using System.Linq;
using System.Text.RegularExpressions;

namespace FS.SMIS_Cloud.NGET.DataCalc.Plan
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using FS.SMIS_Cloud.NGET.DataCalc.Model;
    using FS.SMIS_Cloud.NGET.Model;

    static class CalcPlanSet
    {
        private static ConcurrentDictionary<int, CalcPlan> _calcPlans = new ConcurrentDictionary<int, CalcPlan>();
        
        private readonly static object LockObj = new object();
/*
        public static void Update(SensorGroup group, IList<SensorAcqResult> acqRes)
        {
            lock (LockObj)
            {
                if (_calcPlans.ContainsKey(group.GroupId))
                {
                    var plan = _calcPlans[group.GroupId];
                    if (plan.GetGuid() == taskId)
                    {
                        plan.AddToPlan(dtuId, acqRes);
                        if (plan.IsReady())
                        {
                            var calRes = plan.DoCalc();
                            foreach (var sar in calRes)
                            {
                                acqRes.Add(sar);
                            }
                            CalcPlan cp;
                            _calcPlans.TryRemove(group.GroupId, out cp);
                        }
                    }
                    else //new round acquisition
                    {
                        var calRes = plan.DoCalc();
                        CalcPlan cp;
                        _calcPlans.TryRemove(group.GroupId, out cp);
                        var nplan = _calcPlans.GetOrAdd(group.GroupId, new CalcPlan(group, taskId));
                        nplan.AddToPlan(dtuId, acqRes);
                        foreach (var sar in calRes)
                        {
                            acqRes.Add(sar);
                        }
                    }
                }
                else
                {
                    var nplan = _calcPlans.GetOrAdd(group.GroupId, new CalcPlan(group, taskId));
                    nplan.AddToPlan(dtuId, acqRes);
                }
            }
        }*/


        public static List<SensorAcqResult> Update(IList<SensorGroup> groups, IEnumerable<SensorAcqResult> acqRes)
        {
            lock (LockObj)
            {
                var orderls = acqRes.OrderBy(a => a.AcqNum);
                var acqResult = new List<SensorAcqResult>();
                CalcPlan cp;
                foreach (var source in orderls)
                {
                    var group = (from g in groups
                                 where g.Items.Any(a => a.SensorId == source.Sensor.SensorID)
                                 select g).FirstOrDefault();
                    if (group == null)
                    {
                        acqResult.Add(source);
                        continue;
                    }

                    IEnumerable<SensorAcqResult> asensordatum = null;
                    if (_calcPlans.ContainsKey(group.GroupId))
                    {
                        var plan = _calcPlans[group.GroupId];
                        if (plan.GetAcqNum() == source.AcqNum)
                        {
                            if (plan.AddToPlan(source, out asensordatum))
                            {
                                acqResult.AddRange(asensordatum);
                                _calcPlans.TryRemove(group.GroupId, out cp);
                            }
                        }
                        else if (source.AcqNum > plan.GetAcqNum())//new round acquisition
                        {
                            var calRes = plan.DoCalc();
                            acqResult.AddRange(calRes);
                            _calcPlans.TryRemove(group.GroupId, out cp);

                            var nplan = _calcPlans.GetOrAdd(group.GroupId, new CalcPlan(group, source.AcqNum));
                            if (nplan.AddToPlan(source, out asensordatum))
                            {
                                acqResult.AddRange(asensordatum);
                                _calcPlans.TryRemove(group.GroupId, out cp);
                            }
                        }
                    }
                    else
                    {
                        var nplan = _calcPlans.GetOrAdd(group.GroupId, new CalcPlan(group, source.AcqNum));
                        if (nplan.AddToPlan(source, out asensordatum))
                        {
                            acqResult.AddRange(asensordatum);
                            _calcPlans.TryRemove(group.GroupId, out cp);
                        }
                    }
                }//foreach
                return acqResult;
            }//lock
        }
    }
}
