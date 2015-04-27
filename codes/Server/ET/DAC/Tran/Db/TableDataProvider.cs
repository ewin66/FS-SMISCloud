using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace FS.SMIS_Cloud.DAC.Tran.Db
{
    using DbHelper;

    public class TableDataProvider
    {
        private string _sqlstr = string.Empty;

        private ISqlHelper SqlHelper { get; set; }

        public int Remainder { get; private set; }

        public DataSourseTableInfo TableInfo { get; set; }

        public TableDataProvider(DataSourseTableInfo tableInfo, ISqlHelper helper)
        {
            this.TableInfo = tableInfo;
            this.SqlHelper = helper;
            this.Remainder = 0;
            this.GetSqlStr();
        }
        
        public int GetRemainder()
        {
            if (Remainder <= 0)
            {
                string sql = string.Format("select count(*) as CNT from {0} where lastSyncTime is null", TableInfo.TableName);
                DataSet ds = SqlHelper.Query(sql);
                Remainder = Convert.ToInt32(ds.Tables[0].Rows[0]["CNT"]);
            }
            return Remainder;
        }

        public int OnDataSynchronized(int[] ids)
        {
            int updated = 0;
            if (ids.Length >= 0)
            {
                var sb = new StringBuilder();
                for (int i = 0; i < ids.Length; i++)
                {
                    sb.Append(i);
                    if (i != ids.Length - 1)
                    {
                        sb.Append(",");
                    }
                }

                string sql = string.Format("update {0} set lastSyncTime='{2}' where ID in ({1})", TableInfo.TableName, sb, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                updated = SqlHelper.ExecuteSql(sql);
                this.Remainder -= updated;
            }
            return updated;
        }

        public SensorOriginalData[] QueryRows(int maxRows)
        {
            var sqlstr = new StringBuilder(this._sqlstr);
            if (this.TableInfo.DbType == DbType.SQLite)
            {
                sqlstr.AppendFormat(" limit 0,{0}", maxRows);
            }
            else
            {
                sqlstr.Insert(6, string.Format(" top {0} ", maxRows));
            }
            DataSet ds = this.SqlHelper.Query(sqlstr.ToString());
            IList<SensorOriginalData> rows = new List<SensorOriginalData>();
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                var d = new SensorOriginalData()
                {
                    Type = this.TableInfo.Type
                };
                if (this.TableInfo.StandardFields.Contains("ID"))
                {
                    d.ID = Convert.ToInt32(row["ID"]);
                }
                if (this.TableInfo.StandardFields.Contains("SID"))
                {
                    d.SID = Convert.ToInt32(row["SID"]);
                }
                if (this.TableInfo.StandardFields.Contains("ModuleNo"))
                {
                    d.ModuleNo = Convert.ToInt32(row["ModuleNo"]);
                }
                if (this.TableInfo.StandardFields.Contains("ChannelNo"))
                {
                    d.ChannelNo = Convert.ToInt32(row["ChannelNo"]);
                }
                if (this.TableInfo.StandardFields.Contains("AcqTime"))
                {
                    d.AcqTime = DateTime.Parse(Convert.ToString(row["AcqTime"]));
                }
                d.Values=new double[this.TableInfo.DataCount];
                for (int i = 1; i <= this.TableInfo.DataCount; i++)
                {
                    d.Values[i-1] = Convert.ToDouble(row[string.Format("Value{0}", i)]);
                }
                rows.Add(d);
            }

            return rows.ToArray();
        }

        private void GetSqlStr()
        {
            if (_sqlstr != string.Empty)
            {
                return;
            }
            var columstr = new StringBuilder();
            for (int i = 0; i < this.TableInfo.Colums.Length; i++)
            {
                columstr.AppendFormat(this.TableInfo.Colums[i]).AppendFormat(" {0},", this.TableInfo.StandardFields[i]);
            }
            string orderstr = string.Empty;
            if (this.TableInfo.StandardFields.Contains("AcqTime"))
            {
                orderstr = "order by AcqTime asc";
            }
            columstr.Remove(columstr.Length - 1, 1);

            this._sqlstr = string.Format("select {0} from {1} {2} {3}", columstr, this.TableInfo.TableName, this.TableInfo.Filter, orderstr);
        }
    }
}