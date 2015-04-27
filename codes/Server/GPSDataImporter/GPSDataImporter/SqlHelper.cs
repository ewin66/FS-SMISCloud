using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GPSDataImporter
{
    using System.Collections;
    using System.Data;
    using System.Data.SqlClient;

    public abstract class SqlHelper
    {
        public static readonly string ConnectionString = null;
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());
        public static int ExecteNonQuery(string ConnectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand sqlCommand = new SqlCommand();
            int result;
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                SqlHelper.PrepareCommand(sqlCommand, sqlConnection, null, cmdType, cmdText, commandParameters);
                int num = sqlCommand.ExecuteNonQuery();
                sqlCommand.Parameters.Clear();
                result = num;
            }
            return result;
        }
        public static int ExecteNonQuery(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecteNonQuery(SqlHelper.ConnectionString, cmdType, cmdText, commandParameters);
        }
        public static int ExecteNonQueryProducts(string cmdText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecteNonQuery(CommandType.StoredProcedure, cmdText, commandParameters);
        }
        public static int ExecteNonQueryText(string cmdText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecteNonQuery(CommandType.Text, cmdText, commandParameters);
        }
        public static DataTableCollection GetTable(string connecttionString, CommandType cmdTye, string cmdText, SqlParameter[] commandParameters)
        {
            SqlCommand sqlCommand = new SqlCommand();
            DataSet dataSet = new DataSet();
            using (SqlConnection sqlConnection = new SqlConnection(connecttionString))
            {
                SqlHelper.PrepareCommand(sqlCommand, sqlConnection, null, cmdTye, cmdText, commandParameters);
                new SqlDataAdapter
                {
                    SelectCommand = sqlCommand
                }.Fill(dataSet);
            }
            return dataSet.Tables;
        }
        public static DataTableCollection GetTable(CommandType cmdTye, string cmdText, SqlParameter[] commandParameters)
        {
            return SqlHelper.GetTable(cmdTye, cmdText, commandParameters);
        }
        public static DataTableCollection GetTableProducts(string cmdText, SqlParameter[] commandParameters)
        {
            return SqlHelper.GetTable(CommandType.StoredProcedure, cmdText, commandParameters);
        }
        public static DataTableCollection GetTableText(string cmdText, SqlParameter[] commandParameters)
        {
            return SqlHelper.GetTable(CommandType.Text, cmdText, commandParameters);
        }
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                for (int i = 0; i < cmdParms.Length; i++)
                {
                    SqlParameter value = cmdParms[i];
                    cmd.Parameters.Add(value);
                }
            }
        }
        public static SqlDataReader ExecuteReader(string ConnectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand sqlCommand = new SqlCommand();
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            SqlDataReader result;
            try
            {
                SqlHelper.PrepareCommand(sqlCommand, sqlConnection, null, cmdType, cmdText, commandParameters);
                SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
                sqlCommand.Parameters.Clear();
                result = sqlDataReader;
            }
            catch
            {
                sqlConnection.Close();
                throw;
            }
            return result;
        }
        public static DataSet ExecuteDataSet(string ConnectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            SqlCommand sqlCommand = new SqlCommand();
            DataSet result;
            try
            {
                SqlHelper.PrepareCommand(sqlCommand, sqlConnection, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                DataSet dataSet = new DataSet();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlDataAdapter.Fill(dataSet);
                sqlConnection.Close();
                sqlConnection.Dispose();
                result = dataSet;
            }
            catch
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
                throw;
            }
            return result;
        }
        public static DataSet ExecuteDataSet(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteDataSet(SqlHelper.ConnectionString, cmdType, cmdText, commandParameters);
        }
        public static DataSet ExecuteDataSetProducts(string cmdText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteDataSet(SqlHelper.ConnectionString, CommandType.StoredProcedure, cmdText, commandParameters);
        }
        public static DataSet ExecuteDataSetText(string cmdText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteDataSet(SqlHelper.ConnectionString, CommandType.Text, cmdText, commandParameters);
        }
        public static DataView ExecuteDataSet(string ConnectionString, string sortExpression, string direction, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlConnection sqlConnection = new SqlConnection(ConnectionString);
            SqlCommand sqlCommand = new SqlCommand();
            DataView result;
            try
            {
                SqlHelper.PrepareCommand(sqlCommand, sqlConnection, null, cmdType, cmdText, commandParameters);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter();
                DataSet dataSet = new DataSet();
                sqlDataAdapter.SelectCommand = sqlCommand;
                sqlDataAdapter.Fill(dataSet);
                DataView defaultView = dataSet.Tables[0].DefaultView;
                defaultView.Sort = sortExpression + " " + direction;
                result = defaultView;
            }
            catch
            {
                sqlConnection.Close();
                throw;
            }
            return result;
        }
        public static object ExecuteScalar(CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteScalar(SqlHelper.ConnectionString, cmdType, cmdText, commandParameters);
        }
        public static object ExecuteScalarProducts(string cmdText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteScalar(SqlHelper.ConnectionString, CommandType.StoredProcedure, cmdText, commandParameters);
        }
        public static object ExecuteScalarText(string cmdText, params SqlParameter[] commandParameters)
        {
            return SqlHelper.ExecuteScalar(SqlHelper.ConnectionString, CommandType.Text, cmdText, commandParameters);
        }
        public static object ExecuteScalar(string ConnectionString, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand sqlCommand = new SqlCommand();
            object result;
            using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
            {
                SqlHelper.PrepareCommand(sqlCommand, sqlConnection, null, cmdType, cmdText, commandParameters);
                object obj = sqlCommand.ExecuteScalar();
                sqlCommand.Parameters.Clear();
                result = obj;
            }
            return result;
        }
        public static object ExecuteScalar(SqlConnection connection, CommandType cmdType, string cmdText, params SqlParameter[] commandParameters)
        {
            SqlCommand sqlCommand = new SqlCommand();
            SqlHelper.PrepareCommand(sqlCommand, connection, null, cmdType, cmdText, commandParameters);
            object result = sqlCommand.ExecuteScalar();
            sqlCommand.Parameters.Clear();
            return result;
        }
        public static void CacheParameters(string cacheKey, params SqlParameter[] commandParameters)
        {
            SqlHelper.parmCache[cacheKey] = commandParameters;
        }
        public static SqlParameter[] GetCachedParameters(string cacheKey)
        {
            SqlParameter[] array = (SqlParameter[])SqlHelper.parmCache[cacheKey];
            if (array == null)
            {
                return null;
            }
            SqlParameter[] array2 = new SqlParameter[array.Length];
            int i = 0;
            int num = array.Length;
            while (i < num)
            {
                array2[i] = (SqlParameter)((ICloneable)array[i]).Clone();
                i++;
            }
            return array2;
        }
        public static bool Exists(string strSql)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnectionString, CommandType.Text, strSql, null)) != 0;
        }
        public static bool Exists(string strSql, params SqlParameter[] cmdParms)
        {
            return Convert.ToInt32(SqlHelper.ExecuteScalar(SqlHelper.ConnectionString, CommandType.Text, strSql, cmdParms)) != 0;
        }
    }
}
