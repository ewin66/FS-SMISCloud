using System;
using System.Collections.Generic;
using System.Text;
using FS.SMIS_Cloud.DAC.DAC;
using log4net;

namespace FS.SMIS_Cloud.DAC.Accessor
{
    public abstract class BaseDataSerializer:IDataSerializer
    {
        private static readonly ILog Log = LogManager.GetLogger("BasicDataSerializer");

        private Dictionary<Type , DataMetaInfo> _metas = new Dictionary<Type, DataMetaInfo>();
        private DataMetaInfo _defaultMeta;

        public DataMetaInfo MetaInfo
        {
            get { return _defaultMeta; }
            private set { this._defaultMeta = value; }
        }

        protected BaseDataSerializer(params DataMetaInfo[] metas)
        {
            if (metas != null && metas.Length > 0)
            {
                this._defaultMeta = metas[0];
                foreach (DataMetaInfo mi in metas)
                {
                    _metas[mi.DataType] = mi;
                }
            }
        }
 

        public DataMetaInfo GetMeta(Type type)
        {
            return _metas.ContainsKey(type) ? _metas[type] : null;
        }

        protected abstract void GetCommColumnValue(SensorAcqResult r, out string commColumn, out string commValue);

        public string GetInsertSql(SensorAcqResult result)
        {
            var data = result.Data;
            var meta = GetMeta(data.GetType());

            if (meta==null || data.GetType() != meta.DataType)
            {
                return null;
            }
            var s = result.Sensor;
            StringBuilder columnStr = new StringBuilder(""), valueStr = new StringBuilder("");

            double[] rawValues = result.Data.RawValues;
            if (rawValues.Length < meta.ThemeColums.Length)
            {
                Log.ErrorFormat("Data length not match! values[{0}] != columns[{1}]", rawValues.Length, meta.ThemeColums.Length);
                return null;
            }
            // 写入主题数据.  SQLite 都是主题数据(origCount);
            for (int i = 0; i < meta.ThemeColums.Length; i++)
            {
                columnStr.Append(", [").Append(meta.ThemeColums[i]).Append("]");
                valueStr.Append(", ").Append(string.Format("{0:0.000000}", rawValues[i + meta.ThemesDataOffset]));
            }
            // MSSql [SENSOR_ID],[SAFETY_FACTOR_TYPE_ID
            // SQLite.
            string commColumn, commValue;
            GetCommColumnValue(result, out commColumn, out commValue);
            string sql = string.Format(@"insert into {0} ({1} {2}) values ({3} {4})",
                meta.TableName, commColumn, columnStr.ToString(), commValue, valueStr.ToString());
 
            return sql;
        }

    }
}
