using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Model
{
    /// <summary>
    /// 内部位移数据
    /// </summary>
    class DataDeepDisplacement : Data
    {
        public double XDisplacement;
        public double YDisplacement;
        public double XAccumulate;
        public double YAccumulate;

        /// <summary>
        /// 插入内部位移
        /// </summary>
        public static bool InsertData(IEnumerable<Data> datum)
        {
            bool flag = false;
            using (SqlConnection conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    string sql = "insert into  T_THEMES_DEFORMATION_DEEP_DISPLACEMENT (SENSOR_ID,SAFETY_FACTOR_TYPE_ID,DEEP_DISPLACEMENT_X_VALUE,DEEP_DISPLACEMENT_Y_VALUE,DEEP_CUMULATIVEDISPLACEMENT_X_VALUE,DEEP_CUMULATIVEDISPLACEMENT_Y_VALUE,ACQUISITION_DATETIME) values(@sensorId,@typeid,@x,@y,@cumx,@cumy,@time)";
                    SqlCommand Cmd = new SqlCommand(sql, conn);
                    Cmd.Transaction = tran;

                    foreach (DataDeepDisplacement d in datum)
                    {
                        Cmd.Parameters.Clear();
                        Cmd.Parameters.AddWithValue("@sensorId", d.SensorId);
                        Cmd.Parameters.AddWithValue("@typeid", d.Safetyfactor);
                        Cmd.Parameters.AddWithValue("@x", d.XDisplacement);
                        Cmd.Parameters.AddWithValue("@y", d.YDisplacement);
                        Cmd.Parameters.AddWithValue("@cumx", d.XAccumulate);
                        Cmd.Parameters.AddWithValue("@cumy", d.YAccumulate);
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
