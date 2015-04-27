using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.SMIS_Cloud.ET.Task
{
    public interface TaskResultConsumer
    {
        void OnTaskFinished(DACTaskResult result);
    }
}
