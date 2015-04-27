using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FreeSun.FS_SMISCloud.Server.DataCalc.SensorEntiry;
using FreeSun.FS_SMISCloud.Server.DataCalc.Utility;

namespace FreeSun.FS_SMISCloud.Server.DataCalc.Calculation
{
    /// <summary>
    /// 文件解析类
    /// </summary>
    class VibrationFileParser
    {
        /// <summary>
        /// 单文件格式振动数据的解析和计算
        /// </summary>
        public static ParserReturn Parse(int structid, string file)
        {
            ParserReturn pr = null;

            if (IsFileOccupation(structid, file, out pr))
            {
                return pr;
            }

            try
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    var br = new BinaryReader(fs);
                    ushort T = br.ReadUInt16();
                    ushort L = br.ReadUInt16();
                    TriggerSaveFileTitle title =
                        (TriggerSaveFileTitle)
                            StructConvert.BytesToStruct(br.ReadBytes(L), typeof (TriggerSaveFileTitle));
                    var ch = title.CHNum;
                    var mod = title.DeviceID;
                    var fre = title.SampleFreq;
                    var time = new DateTime(2000 + title.year, title.mon, title.day, title.hour, title.min, title.sec);
                    var cnt = title.L_Date/4;
                    var org = new float[cnt];

                    for (var i = 0; i < cnt; i++)
                    {
                        org[i] = br.ReadSingle();
                    }
                    fs.Close();
#if DEBUG
    //-- 生成数据文件
                    if (!Directory.Exists("debug"))
                        Directory.CreateDirectory("debug");
                    var debugfile = Path.Combine("debug", Path.GetFileNameWithoutExtension(file) + DateTime.Now.ToString("HHmmss") + ".txt");
                    using (var fsw = new FileStream(debugfile, FileMode.Create,FileAccess.ReadWrite))
                    {
                        StreamWriter sw = new StreamWriter(fsw);
                        for (var i = 0; i < cnt; i++)
                        {
                            sw.WriteLine(org[i]);
                        }
                        fsw.Close();
                    }
#endif

                    string err;
                    var affects = ACCSensor.CalcAndSave(structid, mod, ch, fre, time, org, out err);

                    pr = new ParserReturn
                    {
                        Error = err,
                        Struct = structid,
                        File = file,
                        Time = time,
                        Affects = affects,
                        mod = mod,
                        ch = ch,
                        Period = (int)(cnt / fre),
                        freq = (int)fre
                    };
                }
            }
            catch (Exception ex)
            {
                pr = new ParserReturn
                {
                    Error = ex.Message,
                    Struct = structid,
                    File = file,
                    Time = DateTime.Now,
                    Affects = 0,
                    mod = 0,
                    ch = 0,
                    Period = 0,
                    freq = 0
                };
            }
            return pr;
        }

        /// <summary>
        /// 判断文件是否被占用
        /// </summary>
        /// <param name="structid">结构物ID</param>
        /// <param name="file">文件路径</param>
        /// <param name="pr">返回错误信息</param>
        /// <returns>true-被占用</returns>
        private static bool IsFileOccupation(int structid, string file, out ParserReturn pr)
        {
            pr = null;
            bool flag = false;
            try
            {
                using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                {
                    fs.Close();
                }
            }
            catch (Exception) // 文件正在写入时不处理
            {
                flag = true;
                pr = new ParserReturn
                {
                    Error = "Writing",
                    Struct = structid,
                    File = file,
                    Time = DateTime.Now,
                    Affects = 0,
                    mod = 0,
                    ch = 0,
                    Period = 0,
                    freq = 0
                };
            }
            return flag;
        }
    }

    public class ParserReturn
    {
        public string Error;
        public int Struct;
        public string File;
        public DateTime Time;
        public int Affects;
        public int freq;
        public int Period;
        public int mod;
        public int ch;
    }

    public struct TriggerSaveFileTitle
    {
        //public ushort T; //参数类型
        //public ushort L; //参数长度
        public byte diVer; //版本号
        public byte CHNum; //通道号
        public ushort DeviceID; //设备ID
        public float SampleFreq; //采样频率
        public float FilterFreq;
        public byte GainAmplifier; //放大倍数
        public byte TriggerType; //采样方式
        public byte year;
        public byte mon;
        public byte day;
        public byte hour;
        public byte min;
        public byte sec;
        public uint Reserved1;
        public uint Reserved2;
        public ushort Reserved3;
        public ushort T_Data;
        public uint L_Date;
    }
}
