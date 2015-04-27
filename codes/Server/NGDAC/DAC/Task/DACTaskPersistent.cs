namespace FS.SMIS_Cloud.NGDAC.Task
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Accessor;

    using log4net;

    public class DACTaskPersistent
    {
        private static CancellationTokenSource _source;
        private static CancellationToken _token;
        private static ConcurrentQueue<DACTask> _dacTaskPool = new ConcurrentQueue<DACTask>();
        private static ConcurrentQueue<DACTaskResult> _dacTaskResultPool = new ConcurrentQueue<DACTaskResult>();
        internal static ILog log = LogManager.GetLogger("DACTaskPersistent");

        public static void Init()
        {
            if (_source == null)
            {
                _source = new CancellationTokenSource();
                _token = _source.Token;
                System.Threading.Tasks.Task.Factory.StartNew(_DoSaving, _token);
            }
        }
        public static void Stop()
        {
            if (_source != null)
            {
                _source.Cancel();
            }
        }

        internal static void _DoSaving()
        {
            while (!_source.IsCancellationRequested)
            {
                if (!_dacTaskPool.IsEmpty)
                {
                    DACTask task;
                    _dacTaskPool.TryDequeue(out task);
                    if (task != null)
                    {
                        try
                        {
                            DbAccessorHelper.DbAccessor.SaveInstantTask(task);
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("SaveInstantTask Failed! : -- {0}" ,ex.Message);
                        }
                        
                    }
                }
                if (!_dacTaskResultPool.IsEmpty)
                {
                    DACTaskResult result;
                    _dacTaskResultPool.TryDequeue(out result);
                    if (result != null)
                    {
                        try
                        {
                            DbAccessorHelper.DbAccessor.UpdateInstantTask(result);
                        }
                        catch (Exception ex)
                        {
                            log.ErrorFormat("UpdateInstantTask Failed! : -- {0}", ex.Message);
                        }
                    }
                }
                Thread.Sleep(5);
            }
        }

        internal static void SaveTaskImmedate(DACTask task)
        {
            try
            {
                DbAccessorHelper.DbAccessor.SaveInstantTask(task);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("SaveTaskImmedate Failed! : -- {0}", ex.Message);
            }
        }

        // TODO
        internal static void SaveTask(DACTask task)
        {
            _dacTaskPool.Enqueue(task);
        }


        internal static void SaveTaskResult(DACTaskResult result)
        {
            _dacTaskResultPool.Enqueue(result);
        }


        //internal static void SaveDailyTaskResult(DACTaskResult result)
        //{
        //    try
        //    {
        //     //   DbAccessorHelper.DbAccessor.SaveDacResult(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        log.ErrorFormat("SaveDailyTaskResult Failed! : -- {0}", ex.Message);
        //    }
        //}

    }
}
