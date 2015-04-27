using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Model
{
    /// <summary>
    /// 渗流量数据
    /// </summary>
    class DataSeepage : Data
    {
        public double pressure;
        public double seepage;

        /// <summary>
        /// 插入浸润线数据
        /// </summary>
        public static bool InsertData(IEnumerable<Data> datum)
        {
            bool flag = false;
            using (SqlConnection conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    string sql = "insert into  T_THEMES_ENVI_SEEPAGE (SENSOR_ID,SAFETY_FACTOR_TYPE_ID,SEEPAGE,ACQUISITION_DATETIME) values(@sensorId,@typeid,@value,@time)";
                    SqlCommand Cmd = new SqlCommand(sql, conn);
                    Cmd.Transaction = tran;

                    foreach (DataSeepage d in datum)
                    {
                        Cmd.Parameters.Clear();
                        Cmd.Parameters.AddWithValue("@sensorId", d.SensorId);
                        Cmd.Parameters.AddWithValue("@typeid", d.Safetyfactor);
                        Cmd.Parameters.AddWithValue("@value", d.seepage);
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
