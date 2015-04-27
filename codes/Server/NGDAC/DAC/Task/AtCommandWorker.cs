namespace FS.SMIS_Cloud.NGDAC.Task
{
    using System;
    using System.Threading;

    using FS.SMIS_Cloud.NGDAC.Gprs.Cmd;
    using FS.SMIS_Cloud.NGDAC.Node;

    using log4net;

    public class AtCommandWorker
    {
        private static ILog Log = LogManager.GetLogger("AtWorker");
        private DacTaskContext _context; //
        private ATTask task { get; set; }

        private CancellationTokenSource _source;
        private CancellationToken _token;

        public AtCommandWorker(ATTask task, DacTaskContext context)
        {
            this.task = task;
            this._source = new CancellationTokenSource();
            this._token = this._source.Token;
            this._context = context;
        }

        public void StartWork()
        {
            System.Threading.Tasks.Task.Factory.StartNew(this._DoWork, this._token);
        }

        private void _DoWork()
        {
            if (!this._source.IsCancellationRequested)
            {
                if (this.task!=null)
                {
                    this.LetsDoIt(this.task);
                }
                Thread.Sleep(10);
            }
            Log.Debug("Worker stopped.");
        }

        private void LetsDoIt(ATTask atTask)
        {
            ExecuteResult result = null;
            if (this._context.IsAvaliable())
            {
                ushort timeout = 10;
                Log.Info("_context valid, start robot");
                try
                {
                    result = new ConfigCommandExecutor().Execute(this.task, this._context, timeout);
                    result.ErrorMsg = "OK";
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                    result.ErrorMsg = "FAILED";
                }
            }
            else
            {
                Log.Error("_context Busy, can't run.");
                result = new ExecuteResult
                {
                    IsOK = false,
                    ErrorCode = (int)Errors.ERR_NOT_CONNECTED,
                    ErrorMsg = "FAILED",
                    Task=atTask,
                    Elapsed=0,
                    Finished=DateTime.Now
                };
                foreach (var ac in atTask.AtCommands.AtCommands)
                {
                    ATCommandResult r = new ATCommandResult();
                    r.GetJsonResult(ac.ToATString());
                    result.AddAtResult(r);
                }
                result.Task.Status = DACTaskStatus.FAIL;
            }

            if (atTask.Consumer != null)
                atTask.Consumer(result);
            this.Stop();
        }

        private void Stop()
        {
            if (this._source != null)
            {
                this._source.Cancel();
            }
        }
    }
}