using System;

namespace DataCenter.Task
{
    public class LogShowEventArgs : EventArgs
    {
        public string Dtuid { get; set; }

        public byte[] DataValueShow { get; set; }

    }
}