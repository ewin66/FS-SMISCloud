namespace FreeSun.FS_SMISCloud.Server.CloudApi.DAL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using FreeSun.FS_SMISCloud.Server.CloudApi.Entity;

    public class DataService
    {
        public static ServiceUser GetServiceUser(string username)
        {
            string sql = string.Format(@"
SELECT USER_NAME as name, API_KEY as [key]
FROM T_DIM_SERVICE_USER
WHERE USER_NAME='{0}'", username);

            var dt = SqlHelper.ExecuteDataSetText(sql, null).Tables[0];

            if (dt.Rows.Count > 0)
            {
                return new ServiceUser
                           {
                               UserName = dt.Rows[0]["name"].ToString(),
                               ApiKey = dt.Rows[0]["key"].ToString(),
                               RoleId = 6
                           };
            }

            return null;
        }

        public static List<string> GetServiceUserStruct(string username)
        {
            string sql = string.Format(@"
SELECT STRUCT_ID as stc
FROM T_DIM_SERVICE_USER_STRUCT
WHERE USER_NAME='{0}'", username);

            var dt = SqlHelper.ExecuteDataSetText(sql, null).Tables[0];
            var stcs = new List<string>(dt.Rows.Count);
            if (dt.Rows.Count > 0)
            {
                stcs.AddRange(from DataRow row in dt.Rows select row[0].ToString());
            }

            return stcs;
        }
    }
}
