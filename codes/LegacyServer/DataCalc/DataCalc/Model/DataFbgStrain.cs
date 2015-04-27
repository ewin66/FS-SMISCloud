using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;


namespace FreeSun.FS_SMISCloud.Server.DataCalc.Model
{
    class DataFbgStrain : Data
    {
        public double strain;

        public double temperature;

        /// <summary>
        /// 插入光纤光栅数据
        /// </summary>
        public static bool InsertData(IEnumerable<Data> datum)
        {
            var flag = false;
            using (var conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    var sql = "insert into  T_THEMES_FBG_STRESS_STRAIN (SENSOR_ID,SAFETY_FACTOR_TYPE_ID,STRESS_STRAIN_VALUE,ACQUISITION_DATETIME,TEMPERATURE_VALUE) values(@sensorId,@typeid,@value,@time,@temp)";
                    var Cmd = new SqlCommand(sql, conn);
                    Cmd.Transaction = tran;

                    foreach (var data in datum)
                    {
                        var d = (DataFbgStrain)data;
                        Cmd.Parameters.Clear();
                        Cmd.Parameters.AddWithValue("@sensorId", d.SensorId);
                        Cmd.Parameters.AddWithValue("@typeid", d.Safetyfactor);
                        Cmd.Parameters.AddWithValue("@value", d.strain);
                        Cmd.Parameters.AddWithValue("@time", d.CollectTime);
                        Cmd.Parameters.AddWithValue("@temp", d.temperature);
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
