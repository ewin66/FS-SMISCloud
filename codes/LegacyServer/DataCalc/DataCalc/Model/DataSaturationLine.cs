using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Model
{
    /// <summary>
    /// 浸润线数据
    /// </summary>
    class DataSaturationLine:Data
    {
        // 孔口距离
        public double holedis;
        // 水位高程
        public double height;

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
                    string sql = "insert into  T_THEMES_ENVI_SATURATION_LINE (SENSOR_ID,SAFETY_FACTOR_TYPE_ID,HOLE_DIS,HEIGHT,ACQUISITION_DATETIME) values(@sensorId,@typeid,@hole,@value,@time)";
                    SqlCommand Cmd = new SqlCommand(sql, conn);
                    Cmd.Transaction = tran;

                    foreach (DataSaturationLine d in datum)
                    {
                        Cmd.Parameters.Clear();
                        Cmd.Parameters.AddWithValue("@sensorId", d.SensorId);
                        Cmd.Parameters.AddWithValue("@typeid", d.Safetyfactor);
                        Cmd.Parameters.AddWithValue("@hole", d.holedis);
                        Cmd.Parameters.AddWithValue("@value", d.height);
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
