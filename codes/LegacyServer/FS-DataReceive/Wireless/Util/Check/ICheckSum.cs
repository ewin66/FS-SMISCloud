namespace ET.Common.Check
{/****************
  *
  * 校验
  * 
  * 返回校验值
  * 
  * 验证校验是否正确
  * 
  ****************/
    interface ICheckSum
    {
        /// <summary>
        /// 打包时校验
        /// </summary>
        /// <param name="startIndex">起始位置</param>
        /// <param name="endIndex">校验结束位置</param>
        /// <param name="item">字节数组</param>
        /// <returns>返回校验值</returns>
        byte[] CheckSum(int startIndex, int endIndex, byte[] item);//, CheckFunctions function

        /// <summary>
        /// 解析时验证
        /// </summary>
        /// <param name="startIndex">起始位置</param>
        /// <param name="endIndex">校验结束位置</param>
        /// <param name="item">字节数组</param>
        /// <returns>返回验证结果，相同为真，否则为假</returns>
        bool CheckSumResult(int startIndex, int endIndex, byte[] item);//, CheckFunctions function

    }

    public enum CheckFunctions:byte
    {
        AddSum,
        Crc8,
        Crc16
    }

}