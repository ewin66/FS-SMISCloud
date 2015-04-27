#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="DbHelperSqLite.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述：Sqlite 数据库操作类
// 
//  创建标识：20140103 created by Win
// 
//  修改标识：
//  修改描述：
// 
//  修改标识：
//  修改描述：
//  </summary>
//  ---------------------------------------------------------------------------------------------
#endregion
namespace FreeSun.Common.DB
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;

    /// <summary>
    /// The db helper sq lite.
    /// </summary>
    public class DbHelperSqLite
    {

        // 数据库连接字符串(web.config来配置)，多数据库可使用DbHelperSqLiteP来实现.
        public static readonly string connectionString = PubConstant.ConnectionString;

        public DbHelperSqLite()
        {            
        }

        #region 公用方法
        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public static bool ColumnExists(string tableName, string columnName)
        {
            string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
            object res = GetSingle(sql);
            if (res == null)
            {
                return false;
            }

            return Convert.ToInt32(res) > 0;
        }

        public static int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        /// <summary>
        /// The exists.
        /// </summary>
        /// <param name="strSql">
        /// The str sql.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString()); //也可能=0
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static bool TabExists(string TableName)
        {
            string strsql = "select count(*) from sysobjects where id = object_id(N'[" + TableName + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
            //string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
            object obj = GetSingle(strsql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// The exists.
        /// </summary>
        /// <param name="strSql">
        /// The str sql.
        /// </param>
        /// <param name="cmdParms">
        /// The cmd parms.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool Exists(string strSql, params SQLiteParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, cmdParms);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult ==0)
            {
                return true ;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region  执行简单SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string sqlString)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// The execute sql by time.
        /// </summary>
        /// <param name="sqlString">
        /// The sql string.
        /// </param>
        /// <param name="times">
        /// The times.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="SQLiteException">
        /// 异常
        /// </exception>
        public static int ExecuteSqlByTime(string sqlString, int times)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand(sqlString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = times;
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
      
        ///// <summary>
        ///// 执行Sql和Oracle滴混合事务
        ///// </summary>
        ///// <param name="list">SQL命令行列表</param>
        ///// <param name="oracleCmdSqlList">Oracle命令行列表</param>
        ///// <returns>执行结果 0-由于SQL造成事务失败 -1 由于Oracle造成事务失败 1-整体事务执行成功</returns>
        //public static int ExecuteSqlTran(List<CommandInfo> list, List<CommandInfo> oracleCmdSqlList)
        //{
        //    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
        //    {
        //        conn.Open();
        //        SQLiteCommand cmd = new SQLiteCommand();
        //        cmd.Connection = conn;
        //        SQLiteTransaction tx = conn.BeginTransaction();
        //        cmd.Transaction = tx;
        //        try
        //        {
        //            foreach (CommandInfo myDE in list)
        //            {
        //                string cmdText = myDE.CommandText;
        //                SQLiteParameter[] cmdParms = (SQLiteParameter[])myDE.Parameters;
        //                PrepareCommand(cmd, conn, tx, cmdText, cmdParms);
        //                if (myDE.EffentNextType == EffentNextType.SolicitationEvent)
        //                {
        //                    if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
        //                    {
        //                        tx.Rollback();
        //                        throw new Exception("违背要求"+myDE.CommandText+"必须符合select count(..的格式");
        //                        //return 0;
        //                    }

        //                    object obj = cmd.ExecuteScalar();
        //                    bool isHave = false;
        //                    if (obj == null && obj == DBNull.Value)
        //                    {
        //                        isHave = false;
        //                    }
        //                    isHave = Convert.ToInt32(obj) > 0;
        //                    if (isHave)
        //                    {
        //                        //引发事件
        //                        myDE.OnSolicitationEvent();
        //                    }
        //                }
        //                if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
        //                {
        //                    if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
        //                    {
        //                        tx.Rollback();
        //                        throw new Exception("SQL:违背要求" + myDE.CommandText + "必须符合select count(..的格式");
        //                        //return 0;
        //                    }

        //                    object obj = cmd.ExecuteScalar();
        //                    bool isHave = false;
        //                    if (obj == null && obj == DBNull.Value)
        //                    {
        //                        isHave = false;
        //                    }
        //                    isHave = Convert.ToInt32(obj) > 0;

        //                    if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
        //                    {
        //                        tx.Rollback();
        //                        throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须大于0");
        //                        //return 0;
        //                    }
        //                    if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
        //                    {
        //                        tx.Rollback();
        //                        throw new Exception("SQL:违背要求" + myDE.CommandText + "返回值必须等于0");
        //                        //return 0;
        //                    }
        //                    continue;
        //                }
        //                int val = cmd.ExecuteNonQuery();
        //                if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
        //                {
        //                    tx.Rollback();
        //                    throw new Exception("SQL:违背要求" + myDE.CommandText + "必须有影响行");
        //                    //return 0;
        //                }
        //                cmd.Parameters.Clear();
        //            }
        //            string oraConnectionString = PubConstant.GetConnectionString("ConnectionStringPPC");
        //            bool res = OracleHelper.ExecuteSqlTran(oraConnectionString, oracleCmdSqlList);
        //            if (!res)
        //            {
        //                tx.Rollback();
        //                throw new Exception("Oracle执行失败");
        //                // return -1;
        //            }
        //            tx.Commit();
        //            return 1;
        //        }
        //        catch (SQLiteException e)
        //        {
        //            tx.Rollback();
        //            throw e;
        //        }
        //        catch (Exception e)
        //        {
        //            tx.Rollback();
        //            throw e;
        //        }
        //    }
        //}        


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="sqlStringList">
        /// 多条SQL语句
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int ExecuteSqlTran(List<string> sqlStringList)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand())
                {
                    cmd.Connection = conn;
                    using (SQLiteTransaction tx = conn.BeginTransaction())
                    {
                        cmd.Transaction = tx;
                        try
                        {
                            int count = 0;
                            for (int n = 0; n < sqlStringList.Count; n++)
                            {
                                string strsql = sqlStringList[n];
                                if (strsql.Trim().Length > 1)
                                {
                                    cmd.CommandText = strsql;
                                    count += cmd.ExecuteNonQuery();
                                }
                            }

                            tx.Commit();
                            return count;
                        }
                        catch
                        {
                            tx.Rollback();
                            return 0;
                        }
                        finally
                        {
                            tx.Dispose();
                            cmd.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string sqlString, string content)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand(sqlString, connection))
                {
                    try
                    {
                        var myParameter = new SQLiteParameter("@content", SqlDbType.NText) { Value = content };

                        cmd.Parameters.Add(myParameter);
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public static object ExecuteSqlGet(string SQLString, string content)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        var myParameter = new SQLiteParameter("@content", SqlDbType.NText);
                        myParameter.Value = content;
                        cmd.Parameters.Add(myParameter);
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((object.Equals(obj, null)) || (object.Equals(obj, DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand(strSQL, connection))
                {
                    try
                    {
                        var myParameter = new SQLiteParameter("@fs", SqlDbType.Image);
                        myParameter.Value = fs;
                        cmd.Parameters.Add(myParameter);
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (SQLiteException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        cmd.Dispose();
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// The get single.
        /// </summary>
        /// <param name="SQLString">
        /// The sql string.
        /// </param>
        /// <param name="Times">
        /// The times.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="SQLiteException">
        /// </exception>
        public static object GetSingle(string SQLString, int Times)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        connection.Close();
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }
        /// <summary>
        /// 执行查询语句，返回SQLiteDataReader ( 注意：调用该方法后，一定要对SQLiteDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        public static SQLiteDataReader ExecuteReader(string strSQL)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand(strSQL, connection))
                {
                    try
                    {
                        connection.Open();
                        using (SQLiteDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            return myReader;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var command = new SQLiteDataAdapter(SQLString, connection))
                {
                    try
                    {
                        var ds = new DataSet();
                        
                            command.Fill(ds, "ds");
                            DbHelperCommon.AddDataSetTableNames(SQLString, ref ds);

                            return ds;
                        
                    }
                    catch (SQLiteException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        command.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        public static DataSet Query(string SQLString, int Times)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var command = new SQLiteDataAdapter(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        var ds = new DataSet();
                        
                            command.SelectCommand.CommandTimeout = Times;
                            command.Fill(ds, "ds");
                            DbHelperCommon.AddDataSetTableNames(SQLString, ref ds);
                            return ds;
                       
                    }
                    catch (SQLiteException ex)
                    {
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        command.Dispose();
                        connection.Close();
                    }
                }
            }
        }



        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="sqlString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public static int ExecuteSql(string sqlString, params SQLiteParameter[] cmdParms)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, sqlString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (SQLiteException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SQLiteParameter[]）</param>
        public static void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand())
                    {
                        try
                        {
                            //循环
                            foreach (DictionaryEntry myDE in SQLStringList)
                            {
                                string cmdText = myDE.Key.ToString();
                                var cmdParms = (SQLiteParameter[])myDE.Value;
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }

                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            trans.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SQLiteParameter[]）</param>
        public static int ExecuteSqlTran(System.Collections.Generic.List<CommandInfo> cmdList)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand())
                    {
                        try
                        { 
                            int count = 0;
                            //循环
                            foreach (CommandInfo myDE in cmdList)
                            {
                                string cmdText = myDE.CommandText;
                                SQLiteParameter[] cmdParms = (SQLiteParameter[])myDE.Parameters;
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                           
                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
                                {
                                    if (myDE.CommandText.ToLower().IndexOf("count(") == -1)
                                    {
                                        trans.Rollback();
                                        return 0;
                                    }

                                    object obj = cmd.ExecuteScalar();
                                    bool isHave = false;
                                    if (obj == null && obj == DBNull.Value)
                                    {
                                        isHave = false;
                                    }
                                    isHave = Convert.ToInt32(obj) > 0;

                                    if (myDE.EffentNextType == EffentNextType.WhenHaveContine && !isHave)
                                    {
                                        trans.Rollback();
                                        return 0;
                                    }
                                    if (myDE.EffentNextType == EffentNextType.WhenNoHaveContine && isHave)
                                    {
                                        trans.Rollback();
                                        return 0;
                                    }

                                    continue;
                                }

                                int val = cmd.ExecuteNonQuery();
                                count += val;
                                if (myDE.EffentNextType == EffentNextType.ExcuteEffectRows && val == 0)
                                {
                                    trans.Rollback();
                                    return 0;
                                }
                                cmd.Parameters.Clear();
                            }

                            trans.Commit();
                            return count;
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                        finally 
                        {
                            cmd.Dispose();
                            trans.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SQLiteParameter[]）</param>
        public static void ExecuteSqlTranWithIndentity(System.Collections.Generic.List<CommandInfo> SQLStringList)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand())
                    {
                        try
                        {
                            int indentity = 0;

                            //循环
                            foreach (CommandInfo myDE in SQLStringList)
                            {
                                string cmdText = myDE.CommandText;
                                SQLiteParameter[] cmdParms = (SQLiteParameter[])myDE.Parameters;
                                foreach (SQLiteParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.InputOutput)
                                    {
                                        q.Value = indentity;
                                    }
                                }
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                foreach (SQLiteParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.Output)
                                    {
                                        indentity = Convert.ToInt32(q.Value);
                                    }
                                }
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            trans.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的SQLiteParameter[]）</param>
        public static void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
        {
            using (var conn = new SQLiteConnection(connectionString))
            {
                conn.Open();
                using (SQLiteTransaction trans = conn.BeginTransaction())
                {
                    using (var cmd = new SQLiteCommand())
                    {
                        try
                        {
                            int indentity = 0;

                            //循环
                            foreach (DictionaryEntry myDE in SQLStringList)
                            {
                                string cmdText = myDE.Key.ToString();
                                var cmdParms = (SQLiteParameter[])myDE.Value;
                                foreach (SQLiteParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.InputOutput)
                                    {
                                        q.Value = indentity;
                                    }
                                }
                                PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                                int val = cmd.ExecuteNonQuery();
                                foreach (SQLiteParameter q in cmdParms)
                                {
                                    if (q.Direction == ParameterDirection.Output)
                                    {
                                        indentity = Convert.ToInt32(q.Value);
                                    }
                                }
                                cmd.Parameters.Clear();
                            }
                            trans.Commit();
                        }
                        catch
                        {
                            trans.Rollback();
                            throw;
                        }
                        finally
                        {
                            cmd.Dispose();
                            trans.Dispose();
                            conn.Close();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public static object GetSingle(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (SQLiteException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回SQLiteDataReader ( 注意：调用该方法后，一定要对SQLiteDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>SQLiteDataReader</returns>
        public static SQLiteDataReader ExecuteReader(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        SQLiteDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                        cmd.Parameters.Clear();
                        return myReader;
                    }
                    catch (SQLiteException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public static DataSet Query(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                using (var cmd = new SQLiteCommand())
                {
                    PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                    using (var da = new SQLiteDataAdapter(cmd))
                    {
                        try
                        {
                            DataSet ds = new DataSet();
                            
                                da.Fill(ds, "ds");
                                DbHelperCommon.AddDataSetTableNames(SQLString, ref ds);
                                cmd.Parameters.Clear();
                                return ds;
                        }
                        catch (SQLiteException ex)
                        {
                            throw new Exception(ex.Message);
                        }
                        finally
                        {
                            da.Dispose();
                            cmd.Dispose();
                            connection.Close();
                        }
                    }
                }
            }
        }


        private static void PrepareCommand(SQLiteCommand cmd, SQLiteConnection conn, SQLiteTransaction trans, string cmdText, SQLiteParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {


                foreach (SQLiteParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程，返回SQLiteDataReader ( 注意：调用该方法后，一定要对SQLiteDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SQLiteDataReader</returns>
        public static SQLiteDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {  
                using (SQLiteCommand command = BuildQueryCommand(connection, storedProcName, parameters))
                {
                    try
                    {
                        connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        using (SQLiteDataReader returnReader = command.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            return returnReader;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Dispose();
                        connection.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var sqlDA = new SQLiteDataAdapter())
                {
                    try
                    {
                        sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                        using (var dataSet = new DataSet())
                        {
                            sqlDA.Fill(dataSet, tableName);
                            return dataSet;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlDA.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// The run procedure.
        /// </summary>
        /// <param name="storedProcName">
        /// The stored proc name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <param name="Times">
        /// The times.
        /// </param>
        /// <returns>
        /// The <see cref="DataSet"/>.
        /// </returns>
        public static DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (var sqlDA = new SQLiteDataAdapter())
                {
                    try
                    {
                        sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                        sqlDA.SelectCommand.CommandTimeout = Times;
                        using (var dataSet = new DataSet())
                        {
                            sqlDA.Fill(dataSet, tableName);
                            return dataSet;
                        }
                    }
                    catch (SQLiteException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        sqlDA.Dispose();
                        connection.Close();
                    }
                }
            }
        }


        /// <summary>
        /// 构建 SQLiteCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SQLiteCommand</returns>
        private static SQLiteCommand BuildQueryCommand(SQLiteConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            using (var command = new SQLiteCommand(storedProcName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                foreach (SQLiteParameter parameter in parameters)
                {
                    if (parameter != null)
                    {
                        // 检查未分配值的输出参数,将其分配以DBNull.Value.
                        if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                            (parameter.Value == null))
                        {
                            parameter.Value = DBNull.Value;
                        }

                        command.Parameters.Add(parameter);
                    }
                }

                return command;
            }
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数		
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public static int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (var connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = BuildIntCommand(connection, storedProcName, parameters))
                {
                    try
                    {
                        int result;
                        rowsAffected = command.ExecuteNonQuery();
                        result = (int)command.Parameters["ReturnValue"].Value;
                        return result;
                    }
                    catch (SQLiteException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        command.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 创建 SQLiteCommand 对象实例(用来返回一个整数值)	
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SQLiteCommand 对象实例</returns>
        private static SQLiteCommand BuildIntCommand(SQLiteConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            using (SQLiteCommand command = BuildQueryCommand(connection, storedProcName, parameters))
            {
                command.Parameters.Add(
                    new SQLiteParameter(
                        "ReturnValue",
                        DbType.Int32,
                        4,
                        ParameterDirection.ReturnValue,
                        false,
                        0,
                        0,
                        string.Empty,
                        DataRowVersion.Default,
                        null));
                return command;
            }
        }
        #endregion
    }

}

