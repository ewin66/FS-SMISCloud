using System;

namespace FS.SMIS_Cloud.Alarm.Forwarder.Model
{
    public class CreateSMSException : Exception
    {
        public CreateSMSException(string str) : base(str)
        {
        }
    }
}