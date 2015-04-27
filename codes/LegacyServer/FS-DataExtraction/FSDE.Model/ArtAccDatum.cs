#region FileHeader
//  --------------------------------------------------------------------------------------------
//  <copyright file="ArtAccDatum.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：Created in 20140620 by WIN .
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Model
{
    using System;
    using System.Runtime.InteropServices;
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi)] 
    public class ArtAccDatum
    {
        public static int AD_LSB_COUNT = 65536;             // LSB码数量
        public static int AD_LSB_HALF = 32768;              // 当取偏移码时，其原点位置

        [MarshalAsAttribute(UnmanagedType.I4)]
        private int headSizeBytes;
        // 文件头信息长度
        public Int32 HeadSizeBytes
        {
            get
            {
                return headSizeBytes;
            }
            set
            {
                headSizeBytes = value;
            }
        }


        
        public FileHeader FileHeader; // 文件头正文
        public int ChannelCount;            // 通道个数
        public long DatumCount;             // 每通道的数据项个数
        public float[][] MilliVolt;         // 电压数据(mv)

        //构造函数
        public ArtAccDatum()
        {
        }

        //分配内存空间
        public void AllocSpace()
        {
            MilliVolt = new float[ChannelCount][];
            for (int i = 0; i < ChannelCount; i++)
            {
                MilliVolt[i] = new float[DatumCount];
            }
        }

    }

    //振动数据文件头
    public struct FileHeader
    {
        public Int32 FileType;              // 该设备数据文件共有的成员
        public Int32 BusType;				// 设备总线类型(DEFAULT_BUS_TYPE)
        public Int32 DeviceNum;				// 该设备的编号(DEFAULT_DEVICE_NUM)
        public Int32 HeadVersion;      		// 头信息版本(D31-D16=Major  D15-D0=Minijor) = 1.0
        public Int32 VoltBottomRange;       // 量程下限(mV)
        public Int32 VoltTopRange;	        // 量程上限(mV)	
        public USB2852_Para_AD ADPara;      // 保存硬件参数
        public Int64 nTriggerPos;           // 触发点位置
        public Int32 BatCode;               // 同批文件识别码
        public Int32 HeadEndFlag;			// 文件结束位
    }

    //振动数据文件中保存的硬件参数
    public struct USB2852_Para_AD
    {
        public Int32 CheckStsMode;		// 检查状态模式
        public Int32 ADMode;            // AD模式选择(连续采集/分组采集)
        public Int32 FirstChannel;      // 首通道,取值范围为[0, 31]
        public Int32 LastChannel;		// 末通道,取值范围为[0, 31]
        public Int32 Frequency;         // 采集频率,单位为Hz,取值范围为[31, 250000]
        public Int32 GroupInterval;     // 分组采样时的组间间隔(单位：微秒),取值范围为[1, 32767]
        public Int32 LoopsOfGroup;		// 分组采样时，每组循环次数，取值范围为[1, 255]
        public Int32 Gains;				// 增益控制字
        public Int32 TriggerMode;       // 触发模式选择(软件触发、后触发)
        public Int32 TriggerType;		// 触发类型选择(边沿触发/脉冲触发)
        public Int32 TriggerDir;		// 触发方向选择(正向/负向触发)
        public Int32 TrigLevelVolt;     // 触发电平(0mV -- +10000mV)
        public Int32 TrigWindow;		// 触发灵敏度(1-255)，单位:0.5微秒
        public Int32 GroundingMode;		// 接地方式（单端或双端选择）
        public Int32 ClockSource;		// 时钟源选择(内/外时钟源)
        public Int32 bClockOutput;      // 是否允许本地AD转换时钟输出，=TRUE:允许输出到CN1上的CLKOUT，=FALSE:禁止输出到CN1上的CLKOUT
    }
}