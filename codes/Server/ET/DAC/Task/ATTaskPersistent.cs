using System;
using System.Collections.Concurrent;
using System.Threading;
using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.Gprs.Cmd;
using log4net;

namespace FS.SMIS_Cloud.DAC.Task
{
    public class ATTaskPersistent
    {
        private static CancellationTokenSource _source;
        private static CancellationToken _token;
        private static ConcurrentQueue<ATTask> _atTaskPool = new ConcurrentQueue<ATTask>();
        private static ConcurrentQueue<ExecuteResult> _atTaskResultPool = new ConcurrentQueue<ExecuteResult>();
        internal static ILog log = LogManager.GetLogger("ATTaskPersistent");

        public static void Init()
        {
            if (_source == null)
            {
                _source=new CancellationTokenSource();
                _token = _source.Token;
                System.Threading.Tasks.Task.Factory.StartNew(_DoSaving, _token);
            }
        }

        private static void _DoSaving()
        {
            while (!_source.IsCancellationRequested)
            {
                if (!_atTaskPool.IsEmpty)
                {
                    ATTask task;
                    _atTaskPool.TryDequeue(out task);
                    if (task != null)
                    {
                        try
                        {
                            DbAccessorHelper.AtDbAccessor.SaveInstantTask(task);
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("SaveATInstantTask Failed! : -- {0}", ex.Message);
                        }
                    }
                }
                if (!_atTaskResultPool.IsEmpty)
                {
                    ExecuteResult result;
                    _atTaskResultPool.TryDequeue(out result);
                    if (result != null)
                    {
                        try
                        {
                            DbAccessorHelper.AtDbAccessor.UpdateInstantTask(result);
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("UpdateATInstantTask Failed! : -- {0}", ex.Message);
                        }
                    }
                }
                Thread.Sleep(5);
            }
        }


        public static void Stop()
        {
            if (_source != null)
            {
                _source.Cancel();
            }
        }

        internal static void SaveTaskResult(ExecuteResult result)
        {
            _atTaskResultPool.Enqueue(result);
        }

        internal static void SaveTaskImmedate(ATTask task)
        {
            try
            {
                DbAccessorHelper.AtDbAccessor.SaveInstantTask(task);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SaveATTaskImmedate Failed! : -- {0}", ex.Message);
            }
        }

    }
}