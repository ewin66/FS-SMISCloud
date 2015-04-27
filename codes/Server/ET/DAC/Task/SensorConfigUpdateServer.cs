#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="SensorConfigUpdateServer.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2015 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20150130 by LINGWENLONG .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using FS.SMIS_Cloud.DAC.Model;
using log4net;

namespace FS.SMIS_Cloud.DAC.Task
{
    public class SensorConfigUpdateServer
    {
        private readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ConcurrentQueue<SensorOperation> _sensorOperationPool = new ConcurrentQueue<SensorOperation>();

        private readonly CancellationTokenSource _source;
        private readonly CancellationToken _token;

        internal event GetDtuNodeHandler GetDtuNodeListener;

        public SensorConfigUpdateServer()
        {
            _source = new CancellationTokenSource();
            _token = _source.Token;
        }
        
        public void TryAddNewSensorOperation(SensorOperation senopera)
        {
            if (_sensorOperationPool != null)
                _sensorOperationPool.Enqueue(senopera);
        }

        public void StartWork()
        {
            System.Threading.Tasks.Task.Factory.StartNew(_doWork, _token);
        }

        public void Stop()
        {
            if (_source != null)
                _source.Cancel();
        }

        private void _doWork()
        {
            while (!_source.IsCancellationRequested)
            {
                SensorOperation senopera;
                if (!_sensorOperationPool.IsEmpty && _sensorOperationPool.TryDequeue(out senopera))
                {
                    if (senopera != null)
                    {
                        DoIt(senopera);
                    }
                }
                Thread.Sleep(100);
            }
        }

        private void DoIt(SensorOperation senopera)
        {
            try
            {
                UpdateSensorConfig(senopera);
            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Update sensor config error :{0}", ex.Message);
            }
            
        }

        private void UpdateSensorConfig(SensorOperation operation)
        {
            if (operation == null) return;
            if (operation.Action != Operations.ChangedDtu)
            {
                if (GetDtuNodeListener != null)
                {
                    DtuNode node = GetDtuNodeListener(operation.Sensor.DtuID);
                    if (node != null && operation.Sensor.SensorType != SensorType.Virtual)
                    {
                        node.AddSensorOperation(operation);
                    }
                }
            }
            else
            {
                try
                {
                    SensorChangedDtu(operation);
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("sensor {0} changed Dtu error {1}", operation.Sensor.SensorID, ex.Message);
                }
            }
        }

        private void SensorChangedDtu(SensorOperation operation)
        {
            var so = new SensorOperation
            {
                Sensor = new Sensor
                {
                    SensorID = operation.Sensor.SensorID,
                    DtuID = operation.OldDtuId
                },
                OldDtuId = operation.OldDtuId,
                OldSensorId = operation.OldSensorId,
                Action = Operations.Delete
            };
            UpdateSensorConfig(so);
            so = new SensorOperation
            {
                Sensor = operation.Sensor,
                OldDtuId = operation.OldDtuId,
                OldSensorId = operation.OldSensorId,
                Action = Operations.Add
            };
            UpdateSensorConfig(so);
        }

    }
}