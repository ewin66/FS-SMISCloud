// --------------------------------------------------------------------------------------------
// <copyright file="ArithmeticFactory.cs" company="江苏飞尚安全监测咨询有限公司">
// Copyright (C) 2014 飞尚科技
// 版权所有。 
// </copyright>
// <summary>
// 文件功能描述：算法工厂
// 
// 创建标识：刘歆毅20140217
// 
// 修改标识：
// 修改描述：
// 
// 修改标识：
// 修改描述：
// </summary>
// ---------------------------------------------------------------------------------------------
namespace FreeSun.FS_SMISCloud.Server.DataCalc.Arithmetic
{
    public class ArithmeticFactory
    {
        /// <summary>
        /// 创建算法对象
        /// </summary>
        /// <param name="arithIden">算法标记</param>
        /// <returns>算法对象</returns>
        public static IArithmetic CreateArithmetic(string arithIden)
        {
            IArithmetic arith = null;
//             switch (arithIden.ToLower())
//             {
//                 case "group":
//                     arith = new CxGroupArithmetic();
//                     break;
//                 case "average":
//                     arith = new AverageArithmetic();
//                     break;
//             }

            return arith;
        }
    }
}