using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Model
{
    class DataBeamForce : Data
    {
        public double stress;

        /// <summary>
        /// 插入杆件应力数据
        /// </summary>
        public static bool InsertData(IEnumerable<Data> datum)
        {
            var flag = false;
            using (var conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    var sql = "insert into  T_THEMES_FORCE_BEAM (SENSOR_ID,SAFETY_FACTOR_TYPE_ID,FORCE_VALUE,ACQUISITION_DATETIME) values(@sensorId,@typeid,@value,@time)";
                    var Cmd = new SqlCommand(sql, conn);
                    Cmd.Transaction = tran;

                    foreach (var data in datum)
                    {
                        var d = (DataBeamForce)data;
                        Cmd.Parameters.Clear();
                        Cmd.Parameters.AddWithValue("@sensorId", d.SensorId);
                        Cmd.Parameters.AddWithValue("@typeid", d.Safetyfactor);
                        Cmd.Parameters.AddWithValue("@value", d.stress);
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
