using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using FS.SMIS_Cloud.DAC.Tran.Db;

namespace FS.SMIS_Cloud.DAC.Tran
{
    using FS.DbHelper;

    class TableDataProvider
    {
        private ISqlHelper _sqlHelper;
        public DataSourseTableInfo Meta { get; private set; }
        public int Remainder { get; private set; }

        public TableDataProvider(DataSourseTableInfo dmi, ISqlHelper helper)
        {
            this.Meta = dmi;
            this._sqlHelper = helper;
            this.Remainder = 0;
        }

        public int GetRemainder()
        {
            if (Remainder <= 0)
            {
                string sql = string.Format("select count(ID) as CNT from {0} where lastSyncTime is null", Meta.TableName);
                DataSet ds = _sqlHelper.Query(sql);
                Remainder = Convert.ToInt32(ds.Tables[0].Rows[0]["CNT"]);
            }
            return Remainder;
        }

        public int OnDataSynchronized(int[] ids)
        {
            int updated = 0;
            if (ids.Length >= 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (int i in ids)
                {
                    sb.Append(",").Append(i);
                }
                string sql = string.Format("update {0} set lastSyncTime='{2}' where id in ('' {1})", Meta.TableName, sb.ToString(), System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                updated = _sqlHelper.ExecuteSql(sql);
                this.Remainder -= updated;
            }
            return updated;
        }

        public SensorOriginalData[] QueryRows(int rowCount)
        {
            IList<SensorOriginalData> rows = new List<SensorOriginalData>();
            StringBuilder fields = new StringBuilder();
            int idx = 0;
            for (int i = Meta.ThemesDataOffset; i < Meta.ThemeColums.Length; i++)
            {
                fields.Append(",").Append(Meta.ThemeColums[i]);//.Append(" val_").Append(idx++);
                idx++;
            }
            string sql = string.Format("select ID,SENSOR_SET_ID,ModuleNo,ChannelID,ACQUISITION_DATETIME ACQ_TIME {0} from {1} where lastSyncTime is null limit 0,{2}", fields,
                Meta.TableName, rowCount);
            DataSet ds = _sqlHelper.Query(sql);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                SensorOriginalData d = new SensorOriginalData
                {
                    ID = Convert.ToInt32(row["ID"]),
                    ChannelNo = Convert.ToInt32(row["ChannelID"]),
                    ModuleNo = Convert.ToInt32(row["ModuleNo"]),
                    SID = Convert.ToInt32(row["SENSOR_SET_ID"]),
                    AcqTime = DateTime.Parse(Convert.ToString(row["ACQ_TIME"]))
                };
                d.Values = new double[idx];
                for (int i = 0; i < idx; i++)
                {
                    d.Values[i] = Convert.ToDouble(row[5 + i]);
                }
                rows.Add(d);
            }
            return rows.ToArray();
        }
    }


}
