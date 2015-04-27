using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Model
{
    class DataTempAndHumi:Data  
    {
        public double tmperature;
        public double huminity;

        /// <summary>
        /// 插入温湿度数据
        /// </summary>
        public static bool InsertData(IEnumerable<Data> datum)
        {
            bool flag = false;
            using (SqlConnection conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    // TEMPERATURE_VALUE,HUMILITY_VALUE
                    string sql = "insert into  T_THEMES_ENVI_TEMP_HUMI (SENSOR_ID,SAFETY_FACTOR_TYPE_ID,TEMPERATURE_VALUE,HUMILITY_VALUE,ACQUISITION_DATETIME) values(@sensorId,@typeid,@value1,@value2,@time)";
                    SqlCommand Cmd = new SqlCommand(sql, conn);
                    Cmd.Transaction = tran;

                    foreach (DataTempAndHumi d in datum)
                    {
                        Cmd.Parameters.Clear();
                        Cmd.Parameters.AddWithValue("@sensorId", d.SensorId);
                        Cmd.Parameters.AddWithValue("@typeid", d.Safetyfactor);
                        Cmd.Parameters.AddWithValue("@value1", d.tmperature);
                        Cmd.Parameters.AddWithValue("@value2", d.huminity);
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
