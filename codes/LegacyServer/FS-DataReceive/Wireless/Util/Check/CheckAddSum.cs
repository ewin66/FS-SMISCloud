using System;

namespace ET.Common.Check
{
    /*************
     * 
     * 加和校验
     * 
     *************/
    public class CheckAddSum:ICheckSum
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startIndex">校验起始位置</param>
        /// <param name="endIndex">校验</param>
        /// <param name="item"></param>
        /// <returns></returns>
        public byte[] CheckSum(int startIndex, int endIndex, byte[] item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (startIndex > endIndex || endIndex > item.Length)
                throw new ArgumentOutOfRangeException();
            var data = item;
            byte result = 0;
            var st = startIndex;
            var count = endIndex;
            for (var i = st; i <= count; i++)
            {
                result = (byte)((result ^ data[i]) & 0x000000ff);
            }
            var by = new[] { result };
            return by;
        }

        public bool CheckSumResult(int startIndex, int endIndex, byte[] item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (startIndex > endIndex || endIndex > item.Length)
                throw new ArgumentOutOfRangeException();
            var data = item;
            byte result = 0;
            var st = startIndex;
            var count = endIndex;
            var by = data[count];
            for (var i = st; i <= count; i++)
            {
                result = (byte)((result ^ data[i]) & 0x000000ff);
            }
            return (by == result);
        }
    }
}
