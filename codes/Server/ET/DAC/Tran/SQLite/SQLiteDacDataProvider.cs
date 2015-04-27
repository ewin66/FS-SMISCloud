using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.Linq;
using FS.SMIS_Cloud.DAC.Accessor;
using FS.SMIS_Cloud.DAC.Model;
using FS.SMIS_Cloud.DAC.Tran.Db;
using FS.SMIS_Cloud.DAC.Util;
using log4net;

namespace FS.SMIS_Cloud.DAC.Tran
{
    using FS.DbHelper;
    using FS.SMIS_Cloud.DAC.Accessor.SQLite;

    public class SQLiteDacDataProvider: ITranDataProvider
    {
        private static readonly ILog Log = LogManager.GetLogger("SQLiteDacDataProvider");
        private const int MAX_SIZE = 1014;
        private TableDataProvider[] _dataProviders;
        private ISqlHelper sqlHelper;
        private Dictionary<uint, Sensor> _sensors;
        private int _remainder;
        public int Remainder { get { return _remainder; } }
        private TableDataProvider _lastTable;
        private List<int> _lastRows = new List<int>();

        private bool _initialized = false;
        public void Init(Dictionary<string, string> args)
        {
            if (_initialized) return;
            _initialized = true;
            string dbMapFile = args["sqlitedbcongxml"];
            Log.DebugFormat("Loading mapping from {0}", dbMapFile);
            var ldcx = new LoadDbConfigXml(dbMapFile);
            string[] connStrs = ldcx.GetSqlConnectionStrings("/config/databases");
            if (connStrs == null || connStrs.Length == 0) return;
            sqlHelper = SqlHelperFactory.Create(DbType.SQLite, connStrs[0]);
            DbAccessorHelper.Init(new SQLiteDbAccessor(connStrs[0]));
            //IList<DtuNode> nodes = DbAccessorHelper.DbAccessor.QueryDtuNodes(null);
            //_sensors = new Dictionary<uint, Sensor>();
            //foreach (DtuNode dn in nodes)
            //{
            //    if (dn.Sensors == null) continue;
            //    foreach (Sensor si in dn.Sensors)
            //    {
            //        _sensors[si.SensorID] = si;
            //    }
            //}
            //var catalog = new AssemblyCatalog(typeof(IDataSerializer).Assembly);
            //CompositionContainer cc = new CompositionContainer(catalog);
            //cc.ComposeParts(this);
            DataSourseTableInfo[] tableInfos = ldcx.GetDataSourseTableInfo("/config/databases");
            this.DoUpdateAsNecessary(tableInfos);
        }

        private void DoUpdateAsNecessary(IEnumerable<DataSourseTableInfo> tableInfos)
        {
            //DataMetaInfo[] dms = DbAccessorHelper.DbAccessor.GetDataMetas();
            //Log.DebugFormat("{0} tables found.", dms.Length);
            //if (dms.Length <= 0)
             //   return;
            int updatedCnt = 0;
            IList<TableDataProvider> pvs = new List<TableDataProvider>();
            foreach (DataSourseTableInfo dmi in tableInfos)
            {
                if (ShouldUpdate(dmi.TableName))
                {
                    updatedCnt++;
                    Log.DebugFormat("Upgrading table {0}...", dmi.TableName);
                    sqlHelper.ExecuteSql( // 2014-08-08 08:08:08, 19字符长度.
                        string.Format("alter table {0} add column lastSyncTime nchar(20) null", dmi.TableName));
                }
                pvs.Add(new TableDataProvider(dmi, sqlHelper));
            }
            this._dataProviders = pvs.ToArray();
            Log.InfoFormat("Upgraded {0} tables...", updatedCnt);
        }

        private bool ShouldUpdate(string tableName)
        {
            string sql = string.Format("select sql from SQLITE_MASTER where tbl_name = '{0}'", tableName);
            DataSet ds = sqlHelper.Query(sql);
            string createSql = (string) ds.Tables[0].Rows[0]["sql"];
            bool alreadyUpdated = createSql.Contains("lastSyncTime");
            return !alreadyUpdated;
        }

        // Query.
        public bool HasMoreData()
        {
            if (_remainder <= 0)
            {
                RefreshDatabase();
            }
            return _remainder > 0;
        }
 
        private void RefreshDatabase()
        {
            _remainder = 0;
            foreach (TableDataProvider tdp in this._dataProviders)
            {
                _remainder += tdp.GetRemainder();
            }
        }

        private TableDataProvider FindProvider()
        {
            foreach (TableDataProvider tdp in this._dataProviders)
            {
                if (tdp.GetRemainder() > 0)
                {
                    return tdp;
                }
            }
            return null;
        }


        // 每包数据为
        public TranMsg[] NextPackages(out int len)
        {
            byte[] buff = new byte[MAX_SIZE];
            TableDataProvider t = FindProvider();
            _lastRows.Clear();
            int maxRows = MAX_SIZE/CalcDataLength(t.Meta.OriginalDataCount);
            SensorOriginalData[] rows = t.QueryRows(maxRows);
            int offset = 0;
            foreach (SensorOriginalData row in rows)
            {
                offset += DataToSegment(row, buff, offset);
                _lastRows.Add(row.ID);
            }
            _lastTable = t;
            len = offset;

            return Splite(buff, len);
        }

        public TranMsg[] Splite(byte[] buff, int len)
        {
            TranMsg msg = new TranMsg();
            msg.Data = new byte[len];
            Array.Copy(buff, msg.Data, len);
            msg.LoadSize = (ushort) len;
            msg.PackageIndex = 0;
            msg.PackageCount = 1;
            msg.Type = TranMsgType.Dac;
            return new TranMsg[] {msg};
        }

        public void OnPackageSent()
        {
            if (_lastTable != null && _lastRows != null)
            {
                this._remainder -= _lastTable.OnDataSynchronized(_lastRows.ToArray());
            }
        }


        // Tested
        public int CalcDataLength(int dataSize)
        {
            return dataSize * 8 + 15;
        }

        // Tested
        public int DataToSegment(SensorOriginalData data, byte[] buff, int offset)
        {
            /*
            0-1(2)	     2(1)	            3,4(2)	    5,6(2) 	7-13 (8)	         数据N*4
            数据类型 数据个数(N)	模块号	通道号	采集时间timestamp(ms) double(8) * 数据个数.
            */
            double[] values = data.Values;
            int toWrite = CalcDataLength(values.Length);
            ValueHelper.WriteShort(buff, offset, (short)data.Type); // 0,1: type
            buff[offset+2] = (byte)values.Length; // 数据个数
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
