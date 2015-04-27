// // --------------------------------------------------------------------------------------------
// // <copyright file="TypeIdName.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：20140527
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace FSDE.Model
{
    //监测因素ID和对应中文名
    public class TypeIdName
    {
         public static Dictionary<int,string> TypeAndName = new Dictionary<int, string>();

         static TypeIdName() 
         {
            TypeAndName.Add(5,"温湿度监测");
            TypeAndName.Add(6, "降雨量监测");
            TypeAndName.Add(8, "Cl离子含量监测");
            TypeAndName.Add(9, "表面位移监测");
            TypeAndName.Add(10, "内部位移监测");
            TypeAndName.Add(11, "沉降监测");
            TypeAndName.Add(12, "孔隙水压监测");
            TypeAndName.Add(13, "重要挡土墙应变监测");
            TypeAndName.Add(14, "土体压力监测");
            TypeAndName.Add(15, "挡土墙钢筋受力监测");
            TypeAndName.Add(16, "锚杆受力监测");
            TypeAndName.Add(17, "地下水位监测");
            TypeAndName.Add(18, "风速风向监测");
            TypeAndName.Add(19, "桥墩沉降监测");
            TypeAndName.Add(20, "桥墩倾斜监测");
            TypeAndName.Add(21, "梁段挠度监测");
            TypeAndName.Add(22, "桥梁伸缩缝监测");
            TypeAndName.Add(23, "混凝土应力应变监测");
            TypeAndName.Add(24, "振动主题");
            TypeAndName.Add(25, "表面位移监测");
            TypeAndName.Add(26, "温度监测");
            TypeAndName.Add(27, "桥面振动监测");
            TypeAndName.Add(29, "索力监测");
            TypeAndName.Add(30, "风速风向风仰角监测");
            TypeAndName.Add(31, "沉降监测");
            
         }
    }

    //为绑定数据源提供数据类型
    public class IDandName
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    //保存查询的模块号，通道号，传感器编号
    public class MoChaSenId
    {
        public string MoudleId { get; set; }
        public string ChannelId { get; set; }
        public string SensorId { get; set; }
    }

}