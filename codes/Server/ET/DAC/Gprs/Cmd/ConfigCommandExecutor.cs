using System;
using FS.SMIS_Cloud.DAC.Task;

namespace FS.SMIS_Cloud.DAC.Gprs.Cmd
{
    public class ConfigCommandExecutor
    {
        public ExecuteResult Execute(ATTask task, DacTaskContext context, ushort timeout = 2)
        {
            CommandExecutor ce = new CommandExecutor();
            ExecuteResult r = new ExecuteResult();
            r.Task = task;
            r.Task.Status = DACTaskStatus.RUNNING;
            context.DtuConnection.Connect();
            // 循环发送数据
            long totalElapsed = 0;
            int index = 0;
            for (; index < r.Task.AtCommands.AtCommands.Count; index++)
            {
                var ac = r.Task.AtCommands.AtCommands[index];
                ATCommandResult atr = ce.Execute((GprsDtuConnection) context.DtuConnection, ac, timeout);
                if (ac.ToATString() == "***COMMIT CONFIG***")
                {
                    int i = 0;
                    while (!atr.IsOK)
                    {
                        if (i >= 10 || (r.Task.AtCommands.AtCommands.Count == 2))
                        {
                            break;
                        }
                        atr = ce.Execute((GprsDtuConnection) context.DtuConnection, ac, timeout);
                        totalElapsed += atr.Elapsed;
                        i++;
                    }
                }

                if (!atr.IsOK)
                {
                    if (r.Task.AtCommands.AtCommands.Count == 2 && index == 0)
                    {
                        continue;
                    }
                    break;
                }
                r.AddAtResult(atr);
                totalElapsed += atr.Elapsed;
                r.ToJsonString();
            }
            if (index < r.Task.AtCommands.AtCommands.Count - 1)
            {
                for (; index < r.Task.AtCommands.AtCommands.Count; index++)
                {
                    var ar = new ATCommandResult()
                    {
                        ResultBuffer = null,
                        IsOK = false,
                    };
                    ar.GetJsonResult(r.Task.AtCommands.AtCommands[index].ToATString());
                    r.AddAtResult(ar);
                }
            }
            r.Elapsed = totalElapsed;
            r.Finished = DateTime.Now;
            r.Task.Status = DACTaskStatus.DONE;
            context.DtuConnection.Disconnect();
            return r;
        }
    }
}
