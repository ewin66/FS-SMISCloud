﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FS.SMIS_Cloud.DAC.Util;

namespace FS.SMIS_Cloud.DAC.Tran
{
    public class VibFileDataProvider:ITranDataProvider
    {
        private const int MAX_PACKAGE_SIZE = 1014;
        public string DataPath { get; private set; }
        public int Remainder { get; private set; }
        private FileInfo[] _indexFiles;
        private FileInfo[] _lastSynchronized = null;
        private const string SUB_BACKUP = "backup";
        private bool _initialized = false;

        // File Path.
        public void Init(Dictionary<string, string> args)
        {
            if (_initialized) return;
            _initialized = true;
            this.DataPath = args["DataPath"];
            DirectoryInfo di = new DirectoryInfo(DataPath);
            try { di.CreateSubdirectory(SUB_BACKUP); }
            catch
            {
            }
        }

        public bool HasMoreData()
        {
            if (Remainder <= 0)
            {
                Remainder = ScanDataPath();
            }
            return Remainder > 0;
        }

        private int ScanDataPath()
        {
            DirectoryInfo di = new DirectoryInfo(DataPath);
            _indexFiles = di.GetFiles("*.sdb");
            return Remainder = _indexFiles.Length;
        }

        public TranMsg[] NextPackages(out int len)
        {
            if (_indexFiles.Length > 0)
            {
                FileInfo fi = _indexFiles[0];
                int flen = fi.FullName.Length;
                FileInfo fd = new FileInfo(fi.FullName.Substring(0, flen - 4) + ".odb");
                _lastSynchronized = new FileInfo[] { fi, fd };

                byte[] buff1 = CompressHelper.Compress(fi.FullName);
                byte[] buff2 = CompressHelper.Compress(fd.FullName);
                len = buff1.Length + buff2.Length + 4;
                byte[] bb = new byte[len];
                ValueHelper.WriteShort(bb, 0, (short)buff1.Length);
                Array.Copy(buff1, 0, bb, 2, buff1.Length);
                ValueHelper.WriteShort(bb, buff1.Length + 2, (short) buff2.Length);
                Array.Copy(buff2, 0, bb, buff1.Length + 4, buff2.Length);
                return Splite(bb,len);
            }
            else
            {
                len = 0;
                return null;
            }
        }

        public TranMsg[] Splite(byte[] buff, int len)
        {
            int packCnt = (int)Math.Ceiling(len / (MAX_PACKAGE_SIZE*1.0));
            IList<TranMsg> msgs = new List<TranMsg>();
            int restSize = len;
            for (int i = 0; i < packCnt; i++)
            {
                int pLen = (i == packCnt - 1) ? restSize : MAX_PACKAGE_SIZE;
                TranMsg msg = new TranMsg();
                msg.Data = new byte[pLen];
                Array.Copy(buff, i*1014,msg.Data,0,pLen);
                msg.LoadSize = (ushort)pLen;
                msg.PackageIndex = (byte)i;
                msg.PackageCount = (byte)packCnt;
                msg.Type = TranMsgType.Dac;
                msgs.Add(msg);
                restSize -= pLen;
            }
            return msgs.ToArray();
        }

        public void OnPackageSent()
        {
            if (_lastSynchronized != null)
            {
                DirectoryInfo di = new DirectoryInfo(DataPath+"/"+SUB_BACKUP);
                try
                {
                    new FileInfo(di.FullName + "/" + _lastSynchronized[0].Name).Delete();
                    new FileInfo(di.FullName + "/" + _lastSynchronized[1].Name).Delete();
                    // move to backup;
                    _lastSynchronized[0].MoveTo(di.FullName + "/" + _lastSynchronized[0].Name);
                    _lastSynchronized[1].MoveTo(di.FullName + "/" + _lastSynchronized[1].Name);
                }
                catch
                {
                }
                ScanDataPath();
            }
            _lastSynchronized = null;
        }
    }
}
