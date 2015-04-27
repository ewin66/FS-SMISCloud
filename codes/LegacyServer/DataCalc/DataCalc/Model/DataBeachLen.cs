using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Model
{
    class DataBeachLen:Data
    {
        public double waterlevel;
        public double beachlen;

        /// <summary>
        /// 插入干滩数据
        /// </summary>
        public static bool InsertData(IEnumerable<Data> datum)
        {
            bool flag = false;
            using (SqlConnection conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    string sql = "insert into  T_THEMES_ENVI_BEACH (SENSOR_ID,SAFETY_FACTOR_TYPE_ID,WATER_LEVEL,BEACH_LENGTH,ACQUISITION_DATETIME) values(@sensorId,@typeid,@value1,@value2,@time)";
                    SqlCommand Cmd = new SqlCommand(sql, conn);
                    Cmd.Transaction = tran;

                    foreach (DataBeachLen d in datum)
                    {
                        Cmd.Parameters.Clear();
                        Cmd.Parameters.AddWithValue("@sensorId", d.SensorId);
                        Cmd.Parameters.AddWithValue("@typeid", d.Safetyfactor);
                        Cmd.Parameters.AddWithValue("@value1", d.waterlevel);
                        Cmd.Parameters.AddWithValue("@value2", double.IsNaN(d.beachlen) ? 0 : d.beachlen);
                        Cmd.Parameters.AddWithValue("@time", d.CollectTime);
                        Cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                    flag = true;
                    conn.Close();
                }
            }
            return flag;
        }
    }
}
