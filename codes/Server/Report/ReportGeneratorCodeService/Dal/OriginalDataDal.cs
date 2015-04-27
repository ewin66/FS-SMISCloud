// // --------------------------------------------------------------------------------------------
// // <copyright file="OriginalDataDal.cs" company="江苏飞尚安全监测咨询有限公司">
// // Copyright (C) 2014 飞尚科技
// // 版权所有。 
// // </copyright>
// // <summary>
// // 文件功能描述：
// //
// // 创建标识：xusuwei 20141031
// //
// // 修改标识：
// // 修改描述：
// //
// // 修改标识：
// // 修改描述：
// // </summary>
// // ---------------------------------------------------------------------------------------------
namespace ReportGeneratorService.Dal
{
    using System.Configuration;
    using System.Data;
    public class OriginalDataDal
    {
        private static string connectString = ConfigurationManager.ConnectionStrings["SecureCloud"].ConnectionString; 
        public static DataTable GetOriginalData(string sql)
        {
            DataTableCollection tableCollection = SqlHelper.GetTable(connectString, CommandType.Text, sql, null);
            if (tableCollection.Count == 1)
            {
                return tableCollection[0];
            }
            else
            {
                return null;
            }
        }
    }
}