using System.Collections.Generic;
using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.Task;
using NUnit.Framework;

namespace DAC.Test.Task
{
    using FS.SMIS_Cloud.DAC.Accessor.MSSQL;

    [TestFixture]
    public class TaskResultTester
    {
        static string connstr = "server=192.168.1.128;database=DW_iSecureCloud_Empty2.2;uid=sa;pwd=861004";

        [Test]
        public void TestSaveTask()
        {
            DbAccessorHelper.Init(new MsDbAccessor(connstr));

            DACTask task = new DACTask();
            task.Status = DACTaskStatus.RUNNING;
            task.DtuID = 1;
            task.Sensors = new List<uint>(0);
            task.Sensors.Add(1);
            task.Sensors.Add(2);
            task.Sensors.Add(3);
            task.Saved = false;
            task.Requester = "LIU";
            task.Requested = System.DateTime.Now;
            Assert.IsTrue(DbAccessorHelper.DbAccessor.SaveInstantTask(task) > 0);

            IList<DACTask> unfinished = DbAccessorHelper.DbAccessor.GetUnfinishedTasks();
            Assert.IsTrue(unfinished.Count > 0);
        }


        [Test]
        public void TestTaskResult()
        {
            DACTaskResult r = new DACTaskResult();
            DACTask task = new DACTask();
            task.Status = DACTaskStatus.RUNNING;
            task.ID = 3;
            task.DtuID = 1;
            r.Task = task;
            task.Status = DACTaskStatus.RUNNING;
            r.ErrorCode = 123;
            r.ErrorMsg = "Hello error";
            r.Elapsed = 321;
            r.Finished = System.DateTime.Now;
            DbAccessorHelper.Init(new MsDbAccessor(connstr));
            Assert.IsTrue(DbAccessorHelper.DbAccessor.UpdateInstantTask(r) > 0);

            IList<DACTask> unfinished = DbAccessorHelper.DbAccessor.GetUnfinishedTasks(); //status=1/2
            DACTask ti = unfinished[0];
            Assert.AreEqual(ti.ID, 3);
            Assert.AreEqual(ti.Status,  DACTaskStatus.RUNNING);
        }
    }
}
