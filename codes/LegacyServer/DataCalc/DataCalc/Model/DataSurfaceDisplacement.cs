
using System.Collections.Generic;
using System.Data.SqlClient;
using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;


namespace FreeSun.FS_SMISCloud.Server.DataCalc.Model
{
    /// <summary>
    /// 表面位移数据
    /// </summary>
    class DataSurfaceDisplacement:Data
    {
        public double XDisplacement;
        public double YDisplacement;
        public double ZDisplacement;

        /// <summary>
        /// 插入表面位移数据
        /// </summary>
        public static bool InsertData(IEnumerable<Data> datum)
        {
            bool flag = false;
            using (SqlConnection conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    string sql = "insert into  T_THEMES_DEFORMATION_SURFACE_DISPLACEMENT (SENSOR_ID,SAFETY_FACTOR_TYPE_ID,SURFACE_DISPLACEMENT_X_VALUE,SURFACE_DISPLACEMENT_Y_VALUE,SURFACE_DISPLACEMENT_Z_VALUE,ACQUISITION_DATETIME) values(@sensorId,@typeid,@x,@y,@z,@time)";
                    SqlCommand Cmd = new SqlCommand(sql, conn);
                    Cmd.Transaction = tran;

                    foreach (DataSurfaceDisplacement d in datum)
                    {
                        Cmd.Parameters.Clear();
                        Cmd.Parameters.AddWithValue("@sensorId", d.SensorId);
                        Cmd.Parameters.AddWithValue("@typeid", d.Safetyfactor);
                        Cmd.Parameters.AddWithValue("@x", d.XDisplacement);
                        Cmd.Parameters.AddWithValue("@y", d.YDisplacement);
                        Cmd.Parameters.AddWithValue("@z", d.ZDisplacement);
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
