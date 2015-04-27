#region File Header
//  --------------------------------------------------------------------------------------------
//  <copyright file="DbHelperSqLiteP.cs" company="江苏飞尚安全监测咨询有限公司">
//  Copyright (C) 2014 飞尚科技
//  版权所有。 
//  </copyright>
//  <summary>
//  文件功能描述： 动态连接数据库
// 
//  创建标识：20140224 created by Win
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
    /// The db helper sq lite p.
    /// </summary>
    public class DbHelperSqLiteP
    {
        // 数据库连接字符串(web.config来配置)，多数据库可使用DbHelperSQLP来实现.
        private string connectionString = PubConstant.ConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbHelperSqLiteP"/> class.
        /// </summary>
        /// <param name="configString">
        /// The config string.
        /// </param>
        public DbHelperSqLiteP(string configString)
        {
            this.connectionString = configString;
        }
        
        #region 公用方法

        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public bool ColumnExists(string tableName, string columnName)
        {
            string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='"
                         + columnName + "'";
            object res = this.GetSingle(sql);
            if (res == null)
            {
                return false;
            }

            return Convert.ToInt32(res) > 0;
        }

        public int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = this.GetSingle(strsql);
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
        public bool Exists(string strSql)
        {
            object obj = this.GetSingle(strSql);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, DBNull.Value)))
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

            return true;
        }

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool TabExists(string TableName)
        {
            string strsql = "select count(*) from sysobjects where id = object_id(N'[" + TableName
                            + "]') and OBJECTPROPERTY(id, N'IsUserTable') = 1";
            //string strsql = "SELECT count(*) FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[" + TableName + "]') AND type in (N'U')";
            object obj = this.GetSingle(strsql);
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
        public bool Exists(string strSql, params SQLiteParameter[] cmdParms)
        {
            object obj = this.GetSingle(strSql, cmdParms);
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
                return true;
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
        public int ExecuteSql(string sqlString)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
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
        public int ExecuteSqlByTime(string sqlString, int times)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
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

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="sqlStringList">
        /// 多条SQL语句
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int ExecuteSqlTran(List<string> sqlStringList)
        {
            using (var conn = new SQLiteConnection(this.connectionString))
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
        public int ExecuteSql(string sqlString, string content)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
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
        public object ExecuteSqlGet(string SQLString, string content)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
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
        public int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
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
        public object GetSingle(string SQLString)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
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

                        return obj;
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
        public object GetSingle(string SQLString, int Times)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var cmd = new SQLiteCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        cmd.CommandTimeout = Times;
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
        public SQLiteDataReader ExecuteReader(string strSQL)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
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
        public DataSet Query(string SQLString)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();
                using (var command = new SQLiteDataAdapter(SQLString, connection))
                {
                    try
                    {
                        DataSet ds = new DataSet();
                       
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

        public DataSet Query(string SQLString, int Times)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var command = new SQLiteDataAdapter(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        using (var ds = new DataSet())
                        {
                            command.SelectCommand.CommandTimeout = Times;
                            command.Fill(ds, "ds");
                            return ds;
                        }
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
        public int ExecuteSql(string sqlString, params SQLiteParameter[] cmdParms)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var cmd = new SQLiteCommand())
                {
                    try
                    {
                        this.PrepareCommand(cmd, connection, null, sqlString, cmdParms);
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
        public void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (var conn = new SQLiteConnection(this.connectionString))
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
                                this.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
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
        public int ExecuteSqlTran(System.Collections.Generic.List<CommandInfo> cmdList)
        {
            using (var conn = new SQLiteConnection(this.connectionString))
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
                                this.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);

                                if (myDE.EffentNextType == EffentNextType.WhenHaveContine
                                    || myDE.EffentNextType == EffentNextType.WhenNoHaveContine)
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
        public void ExecuteSqlTranWithIndentity(System.Collections.Generic.List<CommandInfo> SQLStringList)
        {
            using (var conn = new SQLiteConnection(this.connectionString))
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
                                this.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
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
        public void ExecuteSqlTranWithIndentity(Hashtable SQLStringList)
        {
            using (var conn = new SQLiteConnection(this.connectionString))
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
                                this.PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
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
        public object GetSingle(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var cmd = new SQLiteCommand())
                {
                    try
                    {
                        this.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
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
        public SQLiteDataReader ExecuteReader(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var cmd = new SQLiteCommand())
                {
                    try
                    {
                        this.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
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
        public DataSet Query(string SQLString, params SQLiteParameter[] cmdParms)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (var cmd = new SQLiteCommand())
                {
                    this.PrepareCommand(cmd, connection, null, SQLString, cmdParms);
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


        private void PrepareCommand(
            SQLiteCommand cmd,
            SQLiteConnection conn,
            SQLiteTransaction trans,
            string cmdText,
            SQLiteParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open) conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null) cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text; //cmdType;
            if (cmdParms != null)
            {


                foreach (SQLiteParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput
                         || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
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
        public SQLiteDataReader RunProcedure(string storedProcName, IDataParameter[] parameters)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                using (SQLiteCommand command = this.BuildQueryCommand(connection, storedProcName, parameters))
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
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();
                using (var sqlDA = new SQLiteDataAdapter())
                {
                    try
                    {
                        sqlDA.SelectCommand = this.BuildQueryCommand(connection, storedProcName, parameters);
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
        public DataSet RunProcedure(string storedProcName, IDataParameter[] parameters, string tableName, int Times)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();
                using (var sqlDA = new SQLiteDataAdapter())
                {
                    try
                    {
                        sqlDA.SelectCommand = this.BuildQueryCommand(connection, storedProcName, parameters);
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
        private SQLiteCommand BuildQueryCommand(
            SQLiteConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            using (var command = new SQLiteCommand(storedProcName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                foreach (SQLiteParameter parameter in parameters)
                {
                    if (parameter != null)
                    {
                        // 检查未分配值的输出参数,将其分配以DBNull.Value.
                        if ((parameter.Direction == ParameterDirection.InputOutput
                             || parameter.Direction == ParameterDirection.Input) && (parameter.Value == null))
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
        /// The 执行存储过程，返回影响的行数.
        /// </summary>
        /// <param name="storedProcName">
        /// The stored proc name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <param name="rowsAffected">
        /// The rows affected.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        /// <exception cref="SQLiteException">
        /// </exception>
        public int RunProcedure(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();
                using (SQLiteCommand command = this.buildIntCommand(connection, storedProcName, parameters))
                {
                    try
                    {
                        rowsAffected = command.ExecuteNonQuery();
                        var result = (int)command.Parameters["ReturnValue"].Value;
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
        /// The build int command.
        /// </summary>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <param name="storedProcName">
        /// The stored proc name.
        /// </param>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <returns>
        /// The <see cref="SQLiteCommand"/>.
        /// </returns>
        private SQLiteCommand buildIntCommand(
            SQLiteConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            using (SQLiteCommand command = this.BuildQueryCommand(connection, storedProcName, parameters))
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

        /// <summary>
        /// 将 <see cref="DataTable"/> 的数据批量插入到数据库中。
        /// </summary>
        /// <param name="dataTable">
        /// 要批量插入的 <see cref="DataTable"/>。
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Insert(DataTable dataTable)
        {
            if (dataTable.Rows.Count == 0)
            {
                return false;
            }

            using (var connection = new SQLiteConnection(this.connectionString))
            {
                connection.Open();
                using (SQLiteTransaction transcation = connection.BeginTransaction())
                {
                    using (var command = new SQLiteCommand())
                    {
                        try
                        {
                            command.Connection = connection;
                            this.GenerateInserSqlParameters(dataTable, command, connection);
                            if (command.CommandText == string.Empty)
                            {
                                return false;
                            }

                            command.Transaction = transcation;
                            foreach (DataRow row in dataTable.Rows)
                            {
                                for (var c = 0; c < dataTable.Columns.Count; c++)
                                {
                                    command.Parameters[c].Value = row[c];
                                }

                                command.ExecuteNonQuery();
                            }

                            command.Parameters.Clear();
                            transcation.Commit();
                            return true;
                        }
                        catch (Exception exp)
                        {
                            if (transcation != null)
                            {
                                transcation.Rollback();
                            }

                            throw exp;
                        }
                        finally
                        {
                            command.Dispose();
                            if (transcation != null)
                            {
                                transcation.Dispose();
                            }

                            connection.Close();
                            connection.Dispose();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 生成插入数据的sql语句。
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        private void GenerateInserSqlParameters(DataTable table, SQLiteCommand command, SQLiteConnection connection)
        {
            command.Connection = connection;
            var names = new StringBuilder();
            var values = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                string name = table.Columns[i].ColumnName;
                SQLiteParameter parameter = command.CreateParameter();
                parameter.ParameterName = name;
                command.Parameters.Add(parameter);
                names.Append(name);
                values.Append("@").Append(name);
                if (i == table.Columns.Count - 1)
                {
                    break;
                }

                names.Append(",");
                values.Append(",");
            }

            command.CommandText = string.Format("INSERT INTO {0}({1}) VALUES ({2})", table.TableName, names, values);
        }
    }
}