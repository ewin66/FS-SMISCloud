using System;
using System.Text;

namespace ET.Common
{
   public class ValueHelper
    {
        #region 大小端判断
        public static bool LittleEndian = false;

        static ValueHelper()
        {
            unsafe
            {
                var tester = 1;
                LittleEndian = (*(byte*)(&tester)) == 1;
            }
        }
        #endregion

        #region Factory
// ReSharper disable InconsistentNaming
        private static ValueHelper _Instance = null;
// ReSharper restore InconsistentNaming
        internal static ValueHelper Instance
        {
            get { return _Instance ?? (_Instance = LittleEndian ? new LittleEndianValueHelper() : new ValueHelper()); }
        }
        #endregion

        protected ValueHelper()
        {

        }

        public static Int32 StringToInt32(string str)
        {
            try
            {
                if (!string.IsNullOrEmpty(str))
                {
                    return Convert.ToInt32(str);
                }
                throw new Exception("字符串不能为空");
            }
            catch (FormatException)
            {
                var sb = new StringBuilder(str);
                throw new FormatException (sb.Append(" ： value不是由一个可选符号后跟数字序列（0 到 9）组成的").ToString());
            }
            catch (OverflowException)
            {
                var sb = new StringBuilder(str);
                throw new OverflowException(sb.Append(" ： value 表示小于 System.Int32.MinValue 或大于 System.Int32.MaxValue 的数字").ToString());
            }
            catch (Exception ex)
            {
                
                throw new Exception(str,ex);
            }
        }

        public virtual Byte[] GetBytes(float value)
        {
            return BitConverter.GetBytes(value);
        }

        public virtual Byte[] GetBytes(short value)
        {
            return BitConverter.GetBytes(value);
        }

        public virtual Byte[] GetBytes(ushort value)
        {
            return BitConverter.GetBytes(value);
        }

        public static short GetShort(byte[] data, int index)
        {
            var by = new byte[2];
            Array.Copy(data, index, by, 0, 2);
            Reverse(by);
            return BitConverter.ToInt16(by, 0);
        }

        public static ushort GetUShort(byte[] data, int index)
        {
            var by = new byte[2];
            Array.Copy(data, index, by, 0, 2);
            Array.Reverse(by);
            return BitConverter.ToUInt16(by, 0);
        }

        private static void Reverse(byte[] data)
        {
            Array.Reverse(data);
        }

        public static bool ByteArrayEquals(byte[] arrayA, byte[] arrayB)
        {
            if (arrayA == null || arrayB == null) return false;
            if (arrayA.Length != arrayB.Length) return false;
            //  return !arrayA.Where((t, i) => t != arrayB[i]).Any();
            for (var i = 0; i < arrayA.Length; i++)
            {
                if (arrayA[i] != arrayB[i]) return false;
            }
            return true;
        }

        public static float GetInt32(byte[] data, int index)
        {
            var by = new byte[4];
            Array.Copy(data, index, by, 0, 4);
            Reverse(by);
            return BitConverter.ToInt32(by, 0);
        }

        public static float GetUInt32(byte[] data, int index)
        {
            var by = new byte[4];
            Array.Copy(data, index, by, 0, 4);
            Reverse(by);
            return BitConverter.ToUInt32(by, 0);
        }

        public static float GetFloat(byte[] data, int index)
        {
            var by = new byte[4];
            Array.Copy(data, index, by, 0, 4);
            Reverse(by);
            return BitConverter.ToSingle(by, 0);
        }

        public virtual float StrToFloat(object floatString)
        {
            float result;
            if (floatString == null||floatString==DBNull.Value)
            {
                return (float)0.00;
            }
            if (float.TryParse(floatString.ToString(), out result))
                return result;
            return (float)0.00;
        }

        /// <summary>
        /// 32位无符号整数中有多少位为1
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int Get1FromUInt32(UInt32 x)
        {
            x = (x & 0x55555555) + ((x >> 1) & 0x55555555);
            x = (x & 0x33333333) + ((x >> 2) & 0x33333333);
            x = (x & 0x0f0f0f0f) + ((x >> 4) & 0x0f0f0f0f);
            x = (x & 0x00ff00ff) + ((x >> 8) & 0x00ff00ff);
            x = (x & 0x0000ffff) + ((x >> 16) & 0x0000ffff);
            return (int)x;
        }

        /// <summary>
        /// 将16进制字符串转换成byte数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] StrToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0) hexString += " ";
            var returnBytes = new byte[hexString.Length / 2];
            for (var i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 将byte数组转成16进制字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ByteToHexStr(byte[] da)
        {
            string str = "";
            for (int i = 0; i < da.Length; i++)
            {
                str = str + Convert.ToString(da[i], 0x10).PadLeft(2, '0')+" ";
            }
            return str;
        }


        public static string BcdToStr(byte[] bytes)
        {
            var temp = new char[bytes.Length * 2];
            char val;
            for (var i = 0; i < bytes.Length; i++)
            {
                val = (char)(((bytes[i] & 0xf0) >> 4) & 0x0f);
                temp[i * 2] = (char)(val > 9 ? val + 'A' - 10 : val + '0');

                val = (char)(bytes[i] & 0x0f);
                temp[i * 2 + 1] = (char)(val > 9 ? val + 'A' - 10 : val + '0');
            }
            return new String(temp);
        }

         /// <summary>
         ///  将汉字转成16进制字符串
         /// </summary>
         /// <param name="s"></param>
        /// <param name="charset">编码,如"utf-8","gb2312"</param>
         /// <param name="fenge"></param>
         /// <returns></returns>
        public static string ToHex(string s, string charset, bool fenge)
        {
            if ((s.Length % 2) != 0)
            {
                s += " ";
                //空格                 //throw new ArgumentException("s is not valid chinese string!");             
            }
            Encoding chs = Encoding.GetEncoding(charset);
            var bytes = chs.GetBytes(s);
            var str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += string.Format("{0:X}", bytes[i]);
                if (fenge && (i != bytes.Length - 1))
                {
                    str += string.Format("{0}", ",");
                }
            }
            return str.ToLower();
        }

        /// <summary>
        /// 将16进制转成汉字
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="charset">编码,如"utf-8","gb2312"</param>
        /// <returns></returns>
        public static string UnHex(string hex, string charset)
        {
            if (hex == null) throw new ArgumentNullException("hex");
            hex = hex.Replace(",", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace("\\", "");
            hex = hex.Replace(" ", "");
            if (hex.Length % 2 != 0)
            {
                hex += "20"; //空格         
            } // 需要将 hex 转换成 byte 数组。         
            var bytes = new byte[hex.Length / 2];

            for (var i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。                   
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message.                     
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }
            Encoding chs = Encoding.GetEncoding(charset);
            return chs.GetString(bytes);
        }

        #region CRC8校验表
        private static readonly byte[] Crc8Table ={
  0, 94, 188, 226, 97, 63, 221, 131, 194, 156, 126, 32, 163, 253, 31, 65,
  157, 195, 33, 127, 252, 162, 64, 30, 95, 1, 227, 189, 62, 96, 130, 220,
  35, 125, 159, 193, 66, 28, 254, 160, 225, 191, 93, 3, 128, 222, 60, 98,
  190, 224, 2, 92, 223, 129, 99, 61, 124, 34, 192, 158, 29, 67, 161, 255,
  70, 24, 250, 164, 39, 121, 155, 197, 132, 218, 56, 102, 229, 187, 89, 7,
  219, 133, 103, 57, 186, 228, 6, 88, 25, 71, 165, 251, 120, 38, 196, 154,
  101, 59, 217, 135, 4, 90, 184, 230, 167, 249, 27, 69, 198, 152, 122, 36,
  248, 166, 68, 26, 153, 199, 37, 123, 58, 100, 134, 216, 91, 5, 231, 185,
  140, 210, 48, 110, 237, 179, 81, 15, 78, 16, 242, 172, 47, 113, 147, 205,
  17, 79, 173, 243, 112, 46, 204, 146, 211, 141, 111, 49, 178, 236, 14, 80,
  175, 241, 19, 77, 206, 144, 114, 44, 109, 51, 209, 143, 12, 82, 176, 238,
  50, 108, 142, 208, 83, 13, 239, 177, 240, 174, 76, 18, 145, 207, 45, 115,
  202, 148, 118, 40, 171, 245, 23, 73, 8, 86, 180, 234, 105, 55, 213, 139,
  87, 9, 235, 181, 54, 104, 138, 212, 149, 203, 41, 119, 244, 170, 72, 22,
  233, 183, 85, 11, 136, 214, 52, 106, 43, 117, 151, 201, 74, 20, 246, 168,
  116, 42, 200, 150, 21, 75, 169, 247, 182, 232, 10, 84, 215, 137, 107, 53
};
        #endregion CRC8校验表
        public static byte CalculateCrc8(byte[] val, int length, out byte c)
        {
            if (val == null)
                throw new ArgumentNullException("val");
             c = 0;
            for (var i = 0; i < length; i++)
            {
                c = Crc8Table[c ^ val[i]];
            }
                //foreach (byte b in val)
                //{
                //    c = Crc8Table[c ^ b];
                //}
            return c;
        }

        public static void ComCrc8(byte[] compOBJ,int length)
        {
            if (compOBJ.Length > length)
            {
                byte[] val = new byte[length-1];
                for (int i = 1, j = 0; i < length; i++)
                {
                    val[j] = compOBJ[i];
                    j++;
                }
                compOBJ[length] = Checksum(val);
            }
        }

        public static byte Checksum(params byte[] val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            byte c = 0;

            foreach (byte b in val)
            {
                c = Crc8Table[c ^ b];
            }

            return c;
        }

        public static byte Checksum(byte[] buffer, int length, out byte c)
        {
            return CalculateCrc8(buffer, length, out c);
        }

        public static ushort CalculateCrc16(byte[] buffer, out byte crcHi, out byte crcLo)
        {
            crcHi = 0xff;  // high crc byte initialized
            crcLo = 0xff;  // low crc byte initialized

            foreach (var t in buffer)
            {
                var crcIndex = crcLo ^ t; // calculate the crc lookup index


                crcLo = (byte)(crcHi ^ CrcTableH[crcIndex]);
                crcHi = CrcTableL[crcIndex];
                //crcHi = (byte)(crcLo ^ crc_table_h[crcIndex]);
                //crcLo = crc_table_l[crcIndex];
            }

            return (ushort)(crcHi << 8 | crcLo);
        }

        public static ushort CalculateCrc16(byte[] buffer, int length, out byte crcHi, out byte crcLo)
        {
            crcHi = 0xff;  // high crc byte initialized
            crcLo = 0xff;  // low crc byte initialized

            for (int i = 0; i < length; i++)
            {
                int crcIndex = crcLo ^ buffer[i]; // calculate the crc lookup index


                crcLo = (byte)(crcHi ^ CrcTableH[crcIndex]);
                crcHi = CrcTableL[crcIndex];
                //crcHi = (byte)(crcLo ^ crc_table_h[crcIndex]);
                //crcLo = crc_table_l[crcIndex];
            }

            return (ushort)(crcHi << 8 | crcLo);
        }

        #region CRC16校验表
        private static readonly byte[] CrcTableH ={
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,
0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40,0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,
0x00,0xC1,0x81,0x40,0x01,0xC0,0x80,0x41,0x01,0xC0,0x80,0x41,0x00,0xC1,0x81,0x40
        };
        private static readonly byte[] CrcTableL ={
0x00,0xC0,0xC1,0x01,0xC3,0x03,0x02,0xC2,0xC6,0x06,0x07,0xC7,0x05,0xC5,0xC4,0x04,
0xCC,0x0C,0x0D,0xCD,0x0F,0xCF,0xCE,0x0E,0x0A,0xCA,0xCB,0x0B,0xC9,0x09,0x08,0xC8,
0xD8,0x18,0x19,0xD9,0x1B,0xDB,0xDA,0x1A,0x1E,0xDE,0xDF,0x1F,0xDD,0x1D,0x1C,0xDC,
0x14,0xD4,0xD5,0x15,0xD7,0x17,0x16,0xD6,0xD2,0x12,0x13,0xD3,0x11,0xD1,0xD0,0x10,
0xF0,0x30,0x31,0xF1,0x33,0xF3,0xF2,0x32,0x36,0xF6,0xF7,0x37,0xF5,0x35,0x34,0xF4,
0x3C,0xFC,0xFD,0x3D,0xFF,0x3F,0x3E,0xFE,0xFA,0x3A,0x3B,0xFB,0x39,0xF9,0xF8,0x38,
0x28,0xE8,0xE9,0x29,0xEB,0x2B,0x2A,0xEA,0xEE,0x2E,0x2F,0xEF,0x2D,0xED,0xEC,0x2C,
0xE4,0x24,0x25,0xE5,0x27,0xE7,0xE6,0x26,0x22,0xE2,0xE3,0x23,0xE1,0x21,0x20,0xE0,
0xA0,0x60,0x61,0xA1,0x63,0xA3,0xA2,0x62,0x66,0xA6,0xA7,0x67,0xA5,0x65,0x64,0xA4,
0x6C,0xAC,0xAD,0x6D,0xAF,0x6F,0x6E,0xAE,0xAA,0x6A,0x6B,0xAB,0x69,0xA9,0xA8,0x68,
0x78,0xB8,0xB9,0x79,0xBB,0x7B,0x7A,0xBA,0xBE,0x7E,0x7F,0xBF,0x7D,0xBD,0xBC,0x7C,
0xB4,0x74,0x75,0xB5,0x77,0xB7,0xB6,0x76,0x72,0xB2,0xB3,0x73,0xB1,0x71,0x70,0xB0,
0x50,0x90,0x91,0x51,0x93,0x53,0x52,0x92,0x96,0x56,0x57,0x97,0x55,0x95,0x94,0x54,
0x9C,0x5C,0x5D,0x9D,0x5F,0x9F,0x9E,0x5E,0x5A,0x9A,0x9B,0x5B,0x99,0x59,0x58,0x98,
0x88,0x48,0x49,0x89,0x4B,0x8B,0x8A,0x4A,0x4E,0x8E,0x8F,0x4F,0x8D,0x4D,0x4C,0x8C,
0x44,0x84,0x85,0x45,0x87,0x47,0x46,0x86,0x82,0x42,0x43,0x83,0x41,0x81,0x80,0x40
        };
        #endregion

        public static ushort Checksum(byte[] data, out byte crcHi, out byte crcLo)
        {
            return CalculateCrc16(data, out crcHi, out crcLo);
        }

        public static ushort Checksum(byte[] data, int index, out byte crcHi, out byte crcLo)
        {

            return CalculateCrc16(data, index, out crcHi, out crcLo);
        }

        public virtual void ComCheckSum(ref byte[] data, int length)
        {
            byte result = 0;
            for (var i = 0; i < length; i++)
            {
                result = (byte)((result ^ data[i]) & 0x000000ff);
            }
            data[length - 1] = result;
        }

    }

    internal class LittleEndianValueHelper : ValueHelper
    {
        public override Byte[] GetBytes(float value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public override Byte[] GetBytes(ushort value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        public override Byte[] GetBytes(short value)
        {
            return Reverse(BitConverter.GetBytes(value));
        }

        private static Byte[] Reverse(Byte[] data)
        {
            Array.Reverse(data);
            return data;
        }
    }

    internal class HelpFun
    {
        internal static int CalculateGrade(float value, float[] grading)
        {
            if (grading == null)
                return 0;
            var i = 0;
            while (i < grading.Length)
            {
                if (value < grading[i])
                    return i + 1;
                i++;
            }
            return grading.Length;
        }
        internal static string ByteToString(byte[] by)
        {
            return "";
        }
    }
}
