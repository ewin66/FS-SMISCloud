using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Model
{
    class DataWind:Data
    {
        public double speed;
        public double azimuth;
        public double elevation;

        public static bool InsertData(IEnumerable<Data> datum)
        {
            bool flag = false;
            using (var conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    string sql = "insert into  T_THEMES_ENVI_WIND (SENSOR_ID,SAFETY_FACTOR_TYPE_ID,WIND_SPEED_VALUE,WIND_DIRECTION_VALUE,WIND_ELEVATION_VALUE,ACQUISITION_DATETIME) " +
                                 "values(@sensorId,@typeid,@speed,@direc,@eleva,@time)";
                    SqlCommand Cmd = new SqlCommand(sql, conn);
                    Cmd.Transaction = tran;

                    foreach (DataWind d in datum)
                    {
                        Cmd.Parameters.Clear();
                        Cmd.Parameters.AddWithValue("@sensorId", d.SensorId);
                        Cmd.Parameters.AddWithValue("@typeid", d.Safetyfactor);
                        Cmd.Parameters.AddWithValue("@speed", d.speed);
                        Cmd.Parameters.AddWithValue("@direc", d.azimuth);
                        Cmd.Parameters.AddWithValue("@eleva", double.IsNaN(d.elevation) ? 0 : d.elevation);
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
