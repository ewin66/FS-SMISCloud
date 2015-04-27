#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="ValueHelper.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：
// 
//  创建标识：20140305 created by Win
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FSDE.Commn
{
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// The value helper.
    /// </summary>
    public class ValueHelper
    {
        #region 大小端判断

        private static readonly bool LittleEndian;

        /// <summary>
        /// Initializes static members of the <see cref="ValueHelper"/> class.
        /// </summary>
        static ValueHelper()
        {
            unsafe
            {
                int tester = 1;
                LittleEndian = (*(byte*)(&tester)) == 1;
            }
        }

        #endregion

        #region Factory

        private static ValueHelper instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static ValueHelper Instance
        {
            get
            {
                return instance ?? (instance = LittleEndian ? new LittleEndianValueHelper() : new ValueHelper());
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ValueHelper"/> class.
        /// </summary>
        protected ValueHelper()
        {
        }

        /// <summary>
        /// The 判断是否是偶数.
        /// </summary>
        /// <param name="num">
        /// The num.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool IsOdd(int num)
        {
            return (num & 1) == 0;
        }

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public virtual byte[] GetBytes(float value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public virtual byte[] GetBytes(short value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// The get bytes.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public virtual byte[] GetBytes(ushort value)
        {
            return BitConverter.GetBytes(value);
        }

        /// <summary>
        /// The get short.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="short"/>.
        /// </returns>
        public static short GetShort(byte[] data, int index)
        {
            var by = new byte[2];
            Array.Copy(data, index, by, 0, 2);
            Array.Reverse(by);
            return BitConverter.ToInt16(by, 0);
        }

        /// <summary>
        /// The get int 32.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float GetInt32(byte[] data, int index)
        {
            var by = new byte[4];
            Array.Copy(data, index, by, 0, 4);
            Array.Reverse(by);
            return BitConverter.ToInt32(by, 0);
        }

        /// <summary>
        /// The get float.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="float"/>.
        /// </returns>
        public static float GetFloat(byte[] data, int index)
        {
            var by = new byte[4];
            Array.Copy(data, index, by, 0, 4);
            Array.Reverse(by);
            return BitConverter.ToSingle(by, 0);
        }

        /// <summary>
        /// The str 2 hex byte.
        /// </summary>
        /// <param name="hexString">
        /// The hex string.
        /// </param>
        /// <returns>
        /// The <see cref="byte[]"/>.
        /// </returns>
        public static byte[] Str2HexByte(string hexString)
        {
            hexString = hexString.Replace(" ", string.Empty);
            hexString = hexString.Replace(",", string.Empty);
            if ((hexString.Length % 2) != 0)
            {
                hexString += " ";
            }

            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }

            return returnBytes;
        }

        /// <summary>
        /// The byte to hex string.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        /// <param name="start">
        /// The start.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Byte2HexStr(byte[] data, int start, int length)
        {
            string str = string.Empty;
            for (int i = start; i < (start + length); i++)
            {
                str = str + Convert.ToString(data[i], 0x10).PadLeft(2, '0') + " ";
            }

            return str;
        }

        /// <summary>
        /// The byte array equals.
        /// </summary>
        /// <param name="arrayA">
        /// The array a.
        /// </param>
        /// <param name="arrayB">
        /// The array b.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ByteArrayEquals(byte[] arrayA, byte[] arrayB)
        {
            if (arrayA.Length != arrayB.Length)
            {
                return false;
            }

            return !arrayA.Where((t, i) => t != arrayB[i]).Any();
        }

        /// <summary>
        /// The get smaller one.
        /// </summary>
        /// <param name="left">
        /// The left.
        /// </param>
        /// <param name="right">
        /// The right.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetSmallerOne(int left, int right)
        {
            if (left < right)
            {
                return left;
            }

            if (left > right)
            {
                return right;
            }

            return left;
        }
        
        /// <summary>
        /// TODO The str to float.
        /// </summary>
        /// <param name="floatString">
        /// TODO The float string.
        /// </param>
        /// <returns>
        /// 转换后的float.
        /// </returns>
        public virtual float StrToFloat(object floatString)
        {
            float result;
            if (floatString != null)
            {
                if (float.TryParse(floatString.ToString(), out result))
                {
                    return result;
                }

                return (float)0.00;
            }

            return (float)0.00;
        }

        /// <summary>
        /// 时间转换
        /// </summary>
        /// <param name="timestr"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static bool String2Time(string timestr, out DateTime time)
        {
            string[] timeformats =
                {
                    "yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd HH:mm:ss.fff", "yyyy-MM-dd HH:mm:ss",
                    "yyyy-MM-dd HH:mm:ss.fff", "yyyyMMddHHmmss", "yyyyMMddHHmmss.fff",
                    "yyyy-MM-dd h:mm:ss","yyyyMMddHHmmssfff"
                };
            bool isSuccess = DateTime.TryParseExact(
                timestr,
                timeformats,
                CultureInfo.CurrentCulture,
                DateTimeStyles.None,
                out time);
            if (!isSuccess)
            {
               time= Convert.ToDateTime(timestr);
            }
            return isSuccess;
        }
    }

    /// <summary>
    /// TODO The little endian value helper.
    /// </summary>
    internal class LittleEndianValueHelper : ValueHelper
    {
        /// <summary>
        /// TODO The get bytes.
        /// </summary>
        /// <param name="value">
        /// TODO The value.
        /// </param>
        /// <returns>
        /// 字节数组.
        /// </returns>
        public override byte[] GetBytes(short value)
        {
            return this.Reverse(BitConverter.GetBytes(value));
        }

        /// <summary>
        /// TODO The reverse.
        /// </summary>
        /// <param name="data">
        /// TODO The data.
        /// </param>
        /// <returns>
        /// 吉祥物.
        /// </returns>
        private byte[] Reverse(byte[] data)
        {
            Array.Reverse(data);
            return data;
        }
    }

    /// <summary>
    /// TODO The help fun.
    /// </summary>
    internal class HelpFun
        {
        /// <summary>
        /// TODO The calculate grade.
        /// </summary>
        /// <param name="value">
        /// TODO The value.
        /// </param>
        /// <param name="grading">
        /// TODO The grading.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        internal static int CalculateGrade(float value, float[] grading)
        {
            if (grading == null)
            {
                return 0;
            }

            int i = 0;
            while (i < grading.Length)
            {
                if (value < grading[i])
                {
                    return i + 1;
                }

                i++;
            }

            return grading.Length;
        }

        /// <summary>
        /// TODO The byte to string.
        /// </summary>
        /// <param name="by">
        /// TODO The by.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string ByteToString(byte[] by)
        {
            return string.Empty;
        }

        /// <summary>
        /// TODO The byte to hex str.
        /// </summary>
        /// <param name="by">
        /// TODO The by.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal static string ByteToHexStr(byte[] by)
            {
                string text = string.Empty;
                for (int i = 0; i < by.Length; i++)
                {
                    text = text + Convert.ToString(by[i], 16).PadLeft(2, '0') + " ";
                }

                return text;
            }
        }
}