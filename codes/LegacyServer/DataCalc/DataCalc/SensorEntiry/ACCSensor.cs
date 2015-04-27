#define USE_BULK_COPY
#define READ_CFG_ONECE_X

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.DataAccess;
using DAAS;
using FreeSun.FS_SMISCloud.Server.DataCalc.Model;
using log4net;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry
{
    /// <summary>
    /// 配置文件信息保存类
    /// 与DAAS同步--
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FileParamStruct
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string bfType;                           // 采样数据标识
        public Int16 diVer;                             // 版本号
        public double diSampleFreq;                     // 采样频率
        public Int32 diSize;                            // 采样点数
        public double diSensitivity;                    // 灵敏度
        public byte diSensorType;                       // 传感器类型
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string diTestPointNum;                   // 测点号
        public Int32 diMultiple;                        // 放大倍数
        public double diFilter;                         // 滤波频率
        public byte diUnit;                             // 工程单位
        public Int16 diADBit;                           // AD精度
        public byte diMethod;                           // 采样方式
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string diRemark;                         // 备注
    }

    /// <summary>
    /// 振动数据过滤
    /// </summary>
    public class AccFilter
    {
        public float R;     // 判断范围(峰值以下百分比)
        public float P;     // 判断范围内数据所占比例

        public AccFilter(float r, float p)
        {
            R = r;
            P = p;
            if (r <= 0 || r >= 1 || p <= 0 || p >= 0.5)
                throw new Exception("AccFilter构造参数异常!");
        }

        public bool IsValid(IEnumerable<double> org,out string err)
        {
            err = "";
            var avg = org.Average();
            var max = org.Max() - avg;
            var min = org.Min() - avg;
            max = Math.Abs(max) > Math.Abs(min) ? max : min;
            var gar = Math.Abs(max) * (1 - R);
            var numlimits = P * org.Count();
            var num = org.Count(d => Math.Abs(d - avg) > gar);
            if (num < numlimits)
            {
                err = string.Format("数据在{0}%以上的点数不足{1}%,判断为无效数据", ((1 - R) * 100).ToString("#.##"), (P * 100).ToString("#.##"));
                return false;
            }
            return true;
        }
    }

    class ACCSensor
    {
        private static ILog _logger = LogManager.GetLogger("ACCSensor");

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ACCSensor() { }

        /// <summary>
        /// 振动数据FFT计算/物理量转换
        /// -- 来自数据接收的计算请求
        /// </summary>
        /// <param name="sensorid">传感器ID</param>
        /// <param name="acqTime">采集时间</param>
        /// <param name="filepath">文件路径</param>
        /// <returns>共插入的振动数据个数(原始数据和FFT数据) -1:false </returns>
        public static int CalcAndSave(int sensorid, DateTime acqTime, string filepath)
        {
            int affectCnt = 0;
            double sensitivity = 1;         // 灵敏度系数
            int formulaid = 0;              // 计算公式
            var structId = DataAccessHelper.GetSensorStruct(sensorid);
            if (!DataAccessHelper.GetSensorParameter1(sensorid, out sensitivity, out formulaid))
            {
                _logger.Warn(string.Format("计算振动数据时发生错误，传感器{0}未能找到计算系数", sensorid));
                return -1;
            }

            double[] freq, ampl;
            double fs = ReadParam(filepath).diSampleFreq;
            double[] org = ReadDatum(filepath);
            PhisicalTrans(ref org, sensitivity);
			// --FILTER BEGIN--
            if (structId > 0)
            {
                var filter = ConfigHelper.GetAccFilter(structId);
                string err = "";
                if (filter != null && !filter.IsValid(org, out err))
                {
                    throw new Exception(err);
                }
            }
            // --FILTER END--

            if (formulaid == 21) // 包含滤波
            {
                org = RemoveDcAndFilterWave(org);
                TWFFT.FFTData(org, fs, 1024, out ampl, out freq);//带平均的FFT
            }
            else
            {
                var tmp = (double[])org.Clone();
                TWFFT.FFTData1(tmp, null, fs, out ampl, out freq);
                ampl[0] = 0;    // 消除频谱中的直流分量
            }

            CheckRedundancy(sensorid, acqTime); // 检查数据库中是否有重复数据

#if USE_BULK_COPY
            affectCnt = InsertDataBulk(sensorid, (int) fs, acqTime, org, freq, ampl);
#else
            affectCnt = InsertData(sensorid, (int)fs, acqTime, org, freq, ampl);
#endif
            return affectCnt;
        }

        /// <summary>
        /// 来自iDAU的计算请求
        /// </summary>
        public static int CalcAndSave(int structid, int module, int channel, float fs,DateTime acqTime, float[] orgdata,out string err)
        {
            int affectCnt = 0;
            double sensitivity = 1;   // 灵敏度系数
            int formulaid = 0;   // 计算公式
            err = "";

            var sensorid = DataAccessHelper.GetSensorId(structid, module, channel);
            if (sensorid == -1)
            {
                err = string.Format("传感器 {0}-{1}-{2} 不存在",structid,module,channel);
                return 0;
            }
            if (!DataAccessHelper.GetSensorParameter1(sensorid, out sensitivity, out formulaid))
            {
                err = string.Format("传感器 {0}-{1}-{2} 参数不存在", structid, module, channel);
                return 0;
            }

            double[] freq, ampl;
            var org = new double[orgdata.Length];
            for (int i = 0; i < org.Length; i++)
            {
                org[i] = orgdata[i];
            }

            PhisicalTrans(ref org, sensitivity);

            if (formulaid == 21) // 包含滤波
            {
                org = RemoveDcAndFilterWave(org);
                TWFFT.FFTData(org, fs, 1024, out ampl, out freq);//带平均的FFT
            }
            else
            {
                var tmp = (double[])org.Clone();
                TWFFT.FFTData1(tmp, null, fs, out ampl, out freq);
                ampl[0] = 0;    // 消除频谱中的直流分量
            }

            CheckRedundancy(sensorid, acqTime);
#if USE_BULK_COPY
            affectCnt = InsertDataBulk(sensorid, (int)fs, acqTime, org, freq, ampl);
#else
            affectCnt = InsertData(sensorid, (int)fs, acqTime, org, freq, ampl);
#endif
            return affectCnt;
        }

        #region Private Methods
        private static FileParamStruct ReadParam(string file)
        {
            using (var fs = new FileStream(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + ".sdb", FileMode.Open, FileAccess.Read))
            {
                var br = new BinaryReader(fs);
                var res = (FileParamStruct)BytesToStruct(br.ReadBytes((int)fs.Length), typeof(FileParamStruct));
                br.Close();
                fs.Close();
                return res;
            }
        }

        private static double[] ReadDatum(string file)
        {
            double[] ret = null;
            using (var fs = new FileStream(Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file)) + ".odb", FileMode.Open, FileAccess.Read))
            {
                long n = fs.Length / sizeof(double);
                ret = new double[n];
                var br = new BinaryReader(fs);
                for (int i = 0; i < n; i++)
                {
                    ret[i] = br.ReadDouble();
                }
            }
            return ret;
        }

        private static Object BytesToStruct(Byte[] bytes, Type strcutType)
        {
            Int32 size = Marshal.SizeOf(strcutType);
            IntPtr buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(bytes, 0, buffer, size);
                return Marshal.PtrToStructure(buffer, strcutType);
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        private static void PhisicalTrans(ref double[] org,double sensitivity)
        {
            if (Math.Abs(sensitivity) < 0.00000001)
            {
                _logger.Warn(string.Format("振动传感器灵敏度系数有误(值{0})", sensitivity));
                return;
            }
            for (int i = 0; i < org.Length; i++)
            {
                org[i] = org[i] / sensitivity;
            }
        }

        private static int InsertData(int sensorid, int fs, DateTime acqTime, double[] org, double[] freq, double[] ampl)
        {
            int affected = 0;
            string guid = Guid.NewGuid().ToString();
            string sql, sqlbatch;
            using (SqlConnection conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    // -- 新增批次记录
                    sqlbatch = "insert into T_THEMES_VIBRATION_BATCH (BatchId,SensorId,MaxFrequency,CollectTime) values(@batch,@sensor,@freq,@time)";
                    SqlCommand batchCmd = new SqlCommand(sqlbatch, conn);
                    batchCmd.Transaction = tran;
                    batchCmd.Parameters.AddRange(new SqlParameter[] {
                        new SqlParameter("@batch",guid),
                        new SqlParameter("@sensor",sensorid),
                        new SqlParameter("@freq",fs),
                        new SqlParameter("@time",acqTime)
                    });
                    batchCmd.ExecuteNonQuery();

                    // -- 新增FFT数据
                    sql = "insert into T_THEMES_VIBRATION (BatchId,Value,Frequency) values (@batch,@val,@freq)";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Transaction = tran;

                    var NCount = freq.Length;
                    for (int i = 0; i < NCount; i++)
                    {
                        try
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@batch", guid);
                            cmd.Parameters.AddWithValue("@val", ampl[i]);
                            cmd.Parameters.AddWithValue("@freq", freq[i]);
                            cmd.ExecuteNonQuery();
                            affected++;
                        }
                        catch (Exception)
                        {
                            _logger.Fatal("Vibration data exception: Value="+ampl[i]);
                            throw;
                        }
                    }

                    // -- 新增时域数据
                    sql = "insert into T_THEMES_VIBRATION_ORIGINAL (BatchId ,Speed ,CollectTime ) values(@batch,@speed,@time)";
                    SqlCommand orgCmd = new SqlCommand(sql, conn);
                    orgCmd.Transaction = tran;
                    NCount = org.Length;
                    double interval = 1.0 / fs * 1000;// 采样点之间时间间隔 ms
                    double curtime = (acqTime.Ticks - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).Ticks) / 10000.0;
                    for (int i = 0; i < NCount; i++)
                    {
                        orgCmd.Parameters.Clear();
                        orgCmd.Parameters.AddWithValue("@batch", guid);
                        orgCmd.Parameters.AddWithValue("@speed", org[i]);
                        orgCmd.Parameters.AddWithValue("@time", curtime + interval * i);
                        orgCmd.ExecuteNonQuery();
                        affected++;
                    }

                    tran.Commit();
                    conn.Close();
                }// using tran
            }// using conn
            return affected;
        }

        private static void CheckRedundancy(int sensorid,DateTime acqTime)
        {
            using (var conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                var sql = @"
  if exists (select BatchId from T_THEMES_VIBRATION_BATCH where SensorId=@sensor and CollectTime=@time)
  begin
	declare @batch varchar(50)
	select @batch=BatchId from T_THEMES_VIBRATION_BATCH where SensorId=@sensor and CollectTime=@time
	delete from T_THEMES_VIBRATION_ORIGINAL where BatchId=@batch
	delete from T_THEMES_VIBRATION where BatchId=@batch
	delete from T_THEMES_VIBRATION_BATCH where BatchId=@batch
  end";
                var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddRange(new SqlParameter[] {
                        new SqlParameter("@sensor",sensorid),
                        new SqlParameter("@time",acqTime)});
                var afft = cmd.ExecuteNonQuery();
                conn.Close();
                if (afft > 1)
                    _logger.WarnFormat(@"删除数据库中与该批次重复的数据: 传感器-{0},时间-'{1}'", sensorid, acqTime);
            }
        }
        /// <summary>
        /// using BULKCOPY
        /// </summary>
        private static int InsertDataBulk(int sensorid, int fs, DateTime acqTime, double[] org, double[] freq, double[] ampl)
        {
            int affected = 0;
            var guid = Guid.NewGuid();
            string sql;
            using (var conn = new SqlConnection(DataAccessHelper.loadDBConnName))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    // -- 新增批次记录
                    const string sqlbatch = "insert into T_THEMES_VIBRATION_BATCH (BatchId,SensorId,MaxFrequency,CollectTime) values(@batch,@sensor,@freq,@time)";
                    var batchCmd = new SqlCommand(sqlbatch, conn);
                    batchCmd.Transaction = tran;
                    batchCmd.Parameters.AddRange(new SqlParameter[] {
                        new SqlParameter("@batch",guid.ToString()),
                        new SqlParameter("@sensor",sensorid),
                        new SqlParameter("@freq",fs),
                        new SqlParameter("@time",acqTime)
                    });
                    batchCmd.ExecuteNonQuery();

                    tran.Commit();
                    conn.Close();
                }// using tran
            }// using conn

            using (var bulkCopy=new SqlBulkCopy(DataAccessHelper.loadDBConnName))
            {
                // -- 新增FFT数据
                var ffttable = new DataTable();
                ffttable.Columns.Add("batch", typeof(Guid));
                ffttable.Columns.Add("value", typeof(double));
                ffttable.Columns.Add("freq", typeof(double));
                var NCount = freq.Length;
                for (int i = 0; i < NCount; i++)
                {
                    var dr = ffttable.NewRow();
                    dr["batch"] = guid;
                    dr["value"] = ampl[i];
                    dr["freq"] = freq[i];
                    ffttable.Rows.Add(dr);
                    affected++;
                }
                bulkCopy.DestinationTableName = "T_THEMES_VIBRATION";
                bulkCopy.ColumnMappings.Add("batch", "BatchId");
                bulkCopy.ColumnMappings.Add("value", "Value");
                bulkCopy.ColumnMappings.Add("freq", "Frequency");
                bulkCopy.WriteToServer(ffttable);

            }

            using (var bulkCopy = new SqlBulkCopy(DataAccessHelper.loadDBConnName))
            {
                var NCount = org.Length;
                double interval = 1.0 / fs * 1000;// 采样点之间时间间隔 ms
                double curtime = (acqTime.Ticks - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).Ticks) / 10000.0;
                var orgtable = new DataTable();
                orgtable.Columns.Add("batch", typeof(Guid));
                orgtable.Columns.Add("value", typeof(double));
                orgtable.Columns.Add("time", typeof(double));
                for (int i = 0; i < NCount; i++)
                {
                    var dr = orgtable.NewRow();
                    dr["batch"] = guid;
                    dr["value"] = org[i];
                    dr["time"] = curtime + interval * i;
                    orgtable.Rows.Add(dr);
                    affected++;
                }
                bulkCopy.DestinationTableName = "T_THEMES_VIBRATION_ORIGINAL";
                bulkCopy.ColumnMappings.Clear();
                bulkCopy.ColumnMappings.Add("batch", "BatchId");
                bulkCopy.ColumnMappings.Add("value", "Speed");
                bulkCopy.ColumnMappings.Add("time", "CollectTime");
                bulkCopy.WriteToServer(orgtable);
            }
            return affected;
        }

        /// <summary>
        /// 去直流并实现30Hz低通滤波
        /// </summary>
        private static double[] RemoveDcAndFilterWave(double[] datas)
        {
            var len = datas.Length;
            var avg = datas.Average();
            double[] calcdatas = null;
            for (int i = 0; i < len; i++)
            {
                datas[i] -= avg;
            }

            var flts = Filter(FILTER_COEF20, datas);
            if (len > 50)
            {
                var tmp = new double[len - 50];
                for (int i = 50; i < len; i++)
                {
                    tmp[i - 50] = flts[i];
                }
                calcdatas = tmp;
            }
            else
            {
                calcdatas = flts;
            }
            return calcdatas;
        }

        /// <summary>
        /// 滤波实现方法
        /// </summary>
        /// <param name="coef">滤波系数</param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double[] Filter(double[] coef, double[] x)
        {
            int M = coef.Length;
            int n = x.Length;
            var y = new double[n];
            for (var yi = 0; yi < n; yi++)
            {
                var t = 0.0;
                for (var bi = M - 1; bi >= 0; bi--)
                {
                    if (yi - bi < 0) continue;

                    t += coef[bi] * x[yi - bi];
                }
                y[yi] = t;
            }
            return y;
        }
        /// <summary>
        /// 滤波系数20阶
        /// 30Hz LowPass FIR Window Rectangle
        /// </summary>
        private static double[] FILTER_COEF20 = new double[]
        {
            -0.000000000000000023938558693257665,
            -0.034427042256543347                ,
             0.023936717527506767                ,
             0.027356248602864838                ,
            -0.051640563384815069                ,
             0.000000000000000023938558693257665 ,
             0.077460845077222534                ,
            -0.063831246740018097                ,
            -0.095746870110026971                ,
             0.30984338030889014                 ,
             0.61409706194983849                 ,
             0.30984338030889014                 ,
            -0.095746870110026971                ,
            -0.063831246740018097                ,
             0.077460845077222534                ,
             0.000000000000000023938558693257665 ,
            -0.051640563384815069                ,
             0.027356248602864838                ,
             0.023936717527506767                ,
            -0.034427042256543347                ,
            -0.000000000000000023938558693257665 
        };
        #endregion
    }
}
