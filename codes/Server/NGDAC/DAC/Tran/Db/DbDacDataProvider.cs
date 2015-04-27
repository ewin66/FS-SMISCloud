namespace FS.SMIS_Cloud.NGDAC.Tran.Db
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FS.DbHelper;
    using FS.SMIS_Cloud.NGDAC.Util;

    using log4net;

    public class DbDacDataProvider : ITranDataProvider
    {
        private static readonly ILog Log = LogManager.GetLogger("DbDacDataProvider");
        private const int MAX_SIZE = 1024;
        private bool _initialized = false;

        private ISqlHelper sqlHelper;
        private TableDataProvider[] DataProviders { get; set; }

        private int _remainder;

        public int Remainder
        {
            get { return this._remainder; }
        }

        private TableDataProvider _lastTable;

        private List<int> _lastRows = new List<int>();

        public void Init(Dictionary<string, string> args)
        {
             if (this._initialized) return;
             this._initialized = true;
            string dbMapFile = args["dbcongxml"];
            Log.DebugFormat("Loading mapping from {0}", dbMapFile);
            var ldcx = new LoadDbConfigXml(dbMapFile);
            DataSourseTableInfo[] tableInfos = ldcx.GetDataSourseTableInfo("/config/databases");
            this.DoUpdateAsNecessary(tableInfos);
        }

        /// <summary>
        /// 统一采集软件
        /// </summary>
        /// <param name="args"></param>
        public void Init_Sqlite(Dictionary<string, string> args)
        {
            if (this._initialized) return;
            this._initialized = true;
            string dbMapFile = args["sqlitedbcongxml"];
            Log.DebugFormat("Loading mapping from {0}", dbMapFile);
            var ldcx = new LoadDbConfigXml(dbMapFile);
            string[] connStrs = ldcx.GetSqlConnectionStrings("/config/databases");
            if (connStrs == null || connStrs.Length == 0) return;

            this.sqlHelper = SqlHelperFactory.Create(DbType.SQLite, connStrs[0]);
            //DbAccessorHelper.Init(new SQLiteDbAccessor(connStrs[0]));
            DataSourseTableInfo[] tableInfos = ldcx.GetDataSourseTableInfo("/config/databases");
            this.DoUpdateAsNecessary_Sqlite(tableInfos);
        }

        private void DoUpdateAsNecessary_Sqlite(IEnumerable<DataSourseTableInfo> tableInfos)
        {
            //DataMetaInfo[] dms = DbAccessorHelper.DbAccessor.GetDataMetas();
            //Log.DebugFormat("{0} tables found.", dms.Length);
            //if (dms.Length <= 0)
            //   return;
            int updatedCnt = 0;
            IList<TableDataProvider> pvs = new List<TableDataProvider>();
            foreach (DataSourseTableInfo dmi in tableInfos)
            {
                if (this.ShouldUpdate(dmi.TableName))
                {
                    updatedCnt++;
                    Log.DebugFormat("Upgrading table {0}...", dmi.TableName);
                    this.sqlHelper.ExecuteSql( // 2014-08-08 08:08:08, 19字符长度.
                        string.Format("alter table {0} add column lastSyncTime nchar(20) null", dmi.TableName));
                }
                pvs.Add(new TableDataProvider(dmi, this.sqlHelper));
            }
            this.DataProviders = pvs.ToArray();
            Log.InfoFormat("Upgraded {0} tables...", updatedCnt);
        }
        
        private void DoUpdateAsNecessary(IEnumerable<DataSourseTableInfo> tableInfos)
        {
            int updatedCnt = 0;
            IList<TableDataProvider> pvs = new List<TableDataProvider>();
            foreach (DataSourseTableInfo tableInfo in tableInfos)
            {
                this.sqlHelper = SqlHelperFactory.Create(tableInfo.DbType,tableInfo.ConnectionString);
                if (this.ShouldUpdate(tableInfo.TableName))
                {
                    string datatype = "nchar(20)";
                    string columnstr = " column";
                    if (tableInfo.DbType == FS.DbHelper.DbType.Access)
                        datatype = "text(20)";
                    if (tableInfo.DbType == FS.DbHelper.DbType.MSSQL)
                        columnstr = string.Empty;
                    Log.DebugFormat("Upgrading table {0}...", tableInfo.TableName);
                    this.sqlHelper.ExecuteSql(string.Format("alter table {0} add{1} lastSyncTime {2} null", tableInfo.TableName, columnstr, datatype));
                }
                pvs.Add( new TableDataProvider(tableInfo, this.sqlHelper));
            }
            this.DataProviders = pvs.ToArray();
            Log.InfoFormat("Upgraded {0} tables...", updatedCnt);
        }

        private bool ShouldUpdate(string tableName)
        {
            bool alreadyUpdated = this.sqlHelper.IsExistColumn(tableName, "lastSyncTime");
            return !alreadyUpdated;
        }

        public bool HasMoreData()
        {
            if (this._remainder <= 0)
            {
                this.RefreshDatabase();
            }
            return this._remainder > 0;
        }

        private void RefreshDatabase()
        {
            this._remainder = 0;
            foreach (TableDataProvider tdp in this.DataProviders)
            {
                this._remainder += tdp.GetRemainder();
            }
        }

        public TranMsg[] NextPackages(out int len)
        {
            byte[] buff = new byte[MAX_SIZE];
            TableDataProvider t = this.FindProvider();
            this._lastRows.Clear();
            int maxRows = MAX_SIZE / this.CalcDataLength(t.TableInfo.DataCount);
            SensorOriginalData[] rows = t.QueryRows(maxRows);
            int offset = 0;
            foreach (SensorOriginalData row in rows)
            {
                offset += this.DataToSegment(row, buff, offset);
                this._lastRows.Add(row.ID);
            }
            this._lastTable = t;
            len = offset;
            return this.Splite(buff, len);
        }

        public TranMsg[] Splite(byte[] buff, int len)
        {
            TranMsg msg = new TranMsg();
            msg.Data = new byte[len];
            Array.Copy(buff, msg.Data, len);
            msg.LoadSize = (ushort)len;
            msg.PackageIndex = 0;
            msg.PackageCount = 1;
            msg.Type = TranMsgType.Dac;
            return new TranMsg[] { msg };
        }

        public int CalcDataLength(int dataSize)
        {
            return dataSize * 8 + 15;
        }

        private TableDataProvider FindProvider()
        {
            foreach (TableDataProvider tdp in this.DataProviders)
            {
                if (tdp.GetRemainder() > 0)
                {
                    return tdp;
                }
            }
            return null;
        }

        public void OnPackageSent()
        {
            if (this._lastTable != null && this._lastRows != null)
            {
                this._remainder -= this._lastTable.OnDataSynchronized(this._lastRows.ToArray());
            }
        }

        public int DataToSegment(SensorOriginalData data, byte[] buff, int offset)
        {
            /*
            0-1(2)	     2(1)	            3,4(2)	    5,6(2) 	7-13 (8)	         数据N*4
            数据类型 数据个数(N)	模块号	通道号	采集时间timestamp(ms) double(8) * 数据个数.
            */
            double[] values = data.Values;
            int toWrite = this.CalcDataLength(values.Length);
            ValueHelper.WriteShort(buff, offset, (short)data.Type); // 0,1: type
            buff[offset + 2] = (byte)values.Length; // 数据个数
            ValueHelper.WriteShort(buff, offset + 3, (short)data.ModuleNo);  // 3-4: 模块号
            ValueHelper.WriteShort(buff, offset + 5, (short)data.ChannelNo); // 5-6: 通道号
            ValueHelper.WriteLong(buff, offset + 7, ValueHelper.MillSeconds(data.AcqTime)); //时间: 8
            for (int i = 0; i < values.Length; i++)
            {
                ValueHelper.WriteDouble(buff, offset + 15 + i * 8, (float)values[i]);
            }
            return toWrite;
        }
    }
}