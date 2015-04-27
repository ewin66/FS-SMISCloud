// // --------------------------------------------------------------------------------------------
// // <copyright file="AggTaskKey.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2015 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20150303
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace Agg.Comm.DataModle
{
    using System;

    [Serializable]
    public class AggTaskKey
    {
        public AggTaskKey(int StructId, int FactorId, AggType Type)
        {
            this.FactorId = FactorId;
            this.StructId = StructId;
            this.Type = Type;
        }

        public override string ToString()
        {
            return this.StructId.ToString("D4") + "-" + this.FactorId.ToString("D3") + "-" + Convert.ToInt32(Type).ToString("D2");
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            AggTaskKey key = obj as AggTaskKey;
            if (key == null)
            {
                return false;
            }

            return this.FactorId == key.FactorId && this.StructId == key.StructId && this.Type == key.Type;
        }

        public override int GetHashCode()
        {
            return this.FactorId.GetHashCode() + this.StructId.GetHashCode() + this.Type.GetHashCode();
        }

        public int StructId { get; set; }
        public int FactorId { get; set; }
        public AggType Type { get; set; }
    }
}