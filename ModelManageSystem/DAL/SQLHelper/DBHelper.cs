using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Windows.Forms;
using KB.FUNC;

namespace KB.DAL
{
    public class DBHelper : IDisposable
    {

        #region 独创事例连接判断 Only --暂没使用
        public Boolean Only = false;
        #endregion

        #region 当前线程指针 ThreadID
        private int ThreadID = -1;
        #endregion

        #region 厂别 FactoryID
        private int factoryID = 0;
        /// <summary>
        /// 厂别 FactoryID
        /// </summary>
        public int FactoryID
        {
            get
            {
                return this.factoryID;
            }
        }
        #endregion

        #region DBHelper

        #region DBHelper()
        #endregion
        #region 新的构造方法
        public DBHelper()
        {
            Only = true;
            this.factoryID = GlobalVal.UserInfo.FactoryID;
        }
        #endregion
        /// <summary>
        /// 用窗口参数设定连接池
        /// </summary>
        /// <param name="frm"></param>
        public DBHelper(Form frm)
        {
            try
            {
                int Thread = -1;

                Thread = SysMenu.GetMenuThread(frm);

                #region 检查
                if (Thread < 0)
                {
                    MessageBox.Show("数据库连接线程指针必须大于等于零!", "警告");
                    return;
                }
                if (Thread >= 300)
                {
                    MessageBox.Show("数据库连接线程指针必须小于300!", "警告");
                    return;
                }
                #endregion

                this.ThreadID = Thread;

                #region 处理数据连接
                if (GlobalVal.UserInfo.Connection[Thread] != null)
                {
                    if (GlobalVal.UserInfo.Connection[Thread].ConnectionString.Trim().IndexOf(GlobalVal.ConnectionString(GlobalVal.UserInfo.FactoryID).Trim()) < 0)
                        CloseConnection();
                }
                #endregion

                Only = false;
                this.factoryID = GlobalVal.UserInfo.FactoryID;
            }
            catch (Exception e1)
            {
                log.PrintInfo(e1);
            }
        }
        /// <summary>
        /// 用窗口参数设定连接池，特定厂别
        /// </summary>
        /// <param name="frm"></param>
        public DBHelper(Form frm, int fid)
        {
            try
            {
                int Thread = -1;

                Thread = SysMenu.GetMenuThread(frm);

                #region 检查
                if (Thread < 0)
                {
                    MessageBox.Show("数据库连接线程指针必须大于等于零!", "警告");
                    return;
                }
                if (Thread >= 300)
                {
                    MessageBox.Show("数据库连接线程指针必须小于300!", "警告");
                    return;
                }
                #endregion

                this.ThreadID = Thread;

                #region 处理数据连接
                if (GlobalVal.UserInfo.Connection[Thread] != null)
                {
                    if (GlobalVal.UserInfo.Connection[Thread].ConnectionString.Trim().IndexOf(GlobalVal.ConnectionString(GlobalVal.UserInfo.FactoryID).Trim()) < 0)
                        CloseConnection();
                }
                #endregion

                Only = false;
                this.factoryID = fid;
            }
            catch (Exception e1)
            {
                log.PrintInfo(e1);
            }
        }
        /// <summary>
        /// DBHelper 从300开始，倒数回去。因50后是顺序分配给各个模块
        /// </summary>
        /// <param name="Thread">创建实例时，使用的数据连接线程指针，0是保留</param>
        public DBHelper(int Thread)
        {
            try
            {
                #region 检查
                if (Thread < 0)
                {
                    MessageBox.Show("数据库连接线程指针必须大于等于零!", "警告");
                    return;
                }
                if (Thread >= 300)
                {
                    MessageBox.Show("数据库连接线程指针必须小于300!", "警告");
                    return;
                }
                #endregion

                this.ThreadID = Thread;

                #region 处理数据连接
                if (GlobalVal.UserInfo.Connection[Thread] != null)
                {
                    if (GlobalVal.UserInfo.Connection[Thread].ConnectionString.Trim().IndexOf(GlobalVal.ConnectionString(GlobalVal.UserInfo.FactoryID).Trim()) < 0)
                        CloseConnection();
                }
                #endregion

                Only = false;
                this.factoryID = GlobalVal.UserInfo.FactoryID;
            }
            catch (Exception e1)
            {
                log.PrintInfo(e1);
            }
        }
        /// <summary>
        /// DBHelper
        /// </summary>
        /// <param name="Thread">创建实例时，使用的数据连接线程指针，0是保留</param>
        /// <param name="fid">厂别</param>
        public DBHelper(int Thread, int fid)
        {
            try
            {
                #region 检查
                if (Thread < 0)
                {
                    MessageBox.Show("数据库连接线程指针必须大于等于零!", "警告");
                    return;
                }
                if (Thread >= 300)
                {
                    MessageBox.Show("数据库连接线程指针必须小于300!", "警告");
                    return;
                }
                #endregion

                this.ThreadID = Thread;

                #region 处理数据连接
                if (GlobalVal.UserInfo.Connection[Thread] != null)
                {
                    if (GlobalVal.UserInfo.Connection[Thread].ConnectionString.Trim() != GlobalVal.ConnectionString(fid).Trim())
                        CloseConnection();
                }
                #endregion

                Only = false;
                this.factoryID = fid;
            }
            catch (Exception e1)
            {
                log.PrintInfo(e1);
            }
        }
        /// <summary>
        /// DBHelper
        /// </summary>
        /// <param name="factory">工厂</param>
        public DBHelper(Factory factory)
        {
            Only = true;
            this.factoryID = (int)factory;
        }

        #endregion

        #region Connection
        /// <summary>
        /// 数据连接
        /// </summary>
        private SqlConnection connection;
        public SqlConnection Connection
        {
            get
            {
                string connectionString = "";
                if (Only)
                {
                    if (connection == null || connection.ConnectionString.Trim() == "")
                    {
                        connectionString = GlobalVal.ConnectionString(FactoryID);
                        connection = new SqlConnection(connectionString);
                        connection.Open();

                        if (connection.State == System.Data.ConnectionState.Closed)
                        {
                            connection.Open();
                        }
                    }
                    else if (connection.State == System.Data.ConnectionState.Closed)
                    {
                        connection.Open();
                    }
                    else if (connection.State == System.Data.ConnectionState.Broken)
                    {
                        connection.Close();
                        connection.Open();
                    }
                    return connection;
                }
                else
                {
                    if (GlobalVal.UserInfo.Connection[ThreadID] == null || GlobalVal.UserInfo.Connection[ThreadID].ConnectionString.Trim() == "")
                    {
                        connectionString = GlobalVal.ConnectionString(FactoryID);
                        GlobalVal.UserInfo.Connection[ThreadID] = new SqlConnection(connectionString);
                        GlobalVal.UserInfo.Connection[ThreadID].Open();

                        if (GlobalVal.UserInfo.Connection[ThreadID].State == System.Data.ConnectionState.Closed)
                        {
                            GlobalVal.UserInfo.Connection[ThreadID].Open();
                        }
                    }
                    else if (GlobalVal.UserInfo.Connection[ThreadID].State == System.Data.ConnectionState.Closed)
                    {
                        GlobalVal.UserInfo.Connection[ThreadID].Open();
                    }
                    else if (GlobalVal.UserInfo.Connection[ThreadID].State == System.Data.ConnectionState.Broken)
                    {
                        GlobalVal.UserInfo.Connection[ThreadID].Close();
                        GlobalVal.UserInfo.Connection[ThreadID].Open();
                    }
                    return GlobalVal.UserInfo.Connection[ThreadID];
                }
            }
        }
        #endregion

        #region 公用方法
        /// <summary>
        /// 判断是否存在某表的某个字段
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="columnName">列名称</param>
        /// <returns>是否存在</returns>
        public bool ColumnExists(string tableName, string columnName)
        {
            string sql = "select count(1) from syscolumns where [id]=object_id('" + tableName + "') and [name]='" + columnName + "'";
            object res = GetSingle(sql);
            if (res == null)
            {
                return false;
            }
            return Convert.ToInt32(res) > 0;
        }
        public int GetMaxID(string FieldName, string TableName)
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
        /// 记录是否存在
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public bool Exists(string strSql)
        {
            object obj = GetSingle(strSql);
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
        public bool Exists(string strSql, params SqlParameter[] cmdParms)
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
        public bool TabExists(string TableName)
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
        /// 视图是否存在
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public bool ViewExists(string ViewName)
        {
            string strsql = "select count(*) from sysobjects where id = object_id(N'[" + ViewName + "]') and OBJECTPROPERTY(id, N'IsView') = 1";
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


        #endregion

        #region ExecuteNonQuery 返回影响行数
        /// <summary>
        /// ExecuteNonQuery 返回影响行数
        /// </summary>
        /// <param name="safeSql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string safeSql)
        {
            int result = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log.Error("ExecuteCommand function:" + e.Message + "\r\nSQL 语法：" + safeSql, e);
                throw e;
            }
            return result;
        }
        public int ExecuteNonQuery(string sql, params SqlParameter[] values)
        {
            int result = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, Connection);
                PrepareCommand(cmd, null, sql, values);
                result = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                log.Error("ExecuteCommand function:" + e.Message + "\r\nSQL 语法：" + sql, e);
                throw e;
            }
            return result;
        }
        public int ExecuteNonQueryByTime(string safeSql, int Times)
        {

            int result = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                cmd.CommandTimeout = Times;
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log.Error("ExecuteCommand function:" + e.Message + "\r\nSQL 语法：" + safeSql, e);
                throw e;
            }
            return result;

        }
        #endregion

        #region GetSingle 执行一条计算查询结果语句，返回查询结果（object）。
        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString)
        {
            object obj = null;
            try
            {
                SqlCommand cmd = new SqlCommand(SQLString, Connection);
                obj = cmd.ExecuteScalar();
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                log.Error("ExecuteCommand function:" + e.Message + "\r\nSQL 语法：" + SQLString, e);
                throw e;
            }
            return obj;
        }
        public object GetSingle(string SQLString, int Times)
        {
            object obj = null;
            try
            {
                SqlCommand cmd = new SqlCommand(SQLString, Connection);
                cmd.CommandTimeout = Times;
                obj = cmd.ExecuteScalar();
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                log.Error("ExecuteCommand function:" + e.Message + "\r\nSQL 语法：" + SQLString, e);
                throw e;
            }
            return obj;
        }
        public object GetSingle(string SQLString, params SqlParameter[] values)
        {
            object obj = null;
            try
            {
                SqlCommand cmd = new SqlCommand(SQLString, Connection);
                PrepareCommand(cmd, null, SQLString, values);
                obj = cmd.ExecuteScalar();
                if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                log.Error("ExecuteCommand function:" + e.Message + "\r\nSQL 语法：" + SQLString, e);
                throw e;
            }
            return obj;
        }
        #endregion

        #region ExecuteCommandProc
        ///<sumary>
        ///执行存储过程
        ///</sumary>
        public int ExecuteCommandProc(string sql, params SqlParameter[] values)
        {
            int result = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(values);
                cmd.ExecuteNonQuery();
                result = int.Parse(cmd.Parameters["@returnID"].Value.ToString());
            }
            catch (Exception e)
            {
                log.Error("ExecuteCommandProc function:" + e.Message + "\r\nSQL 语法：" + sql, e);
                throw e;
            }
            return result;
        }
        ///<sumary>
        ///执行存储过程
        ///</sumary>
        public void RunProcedure(string sql, params SqlParameter[] values)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();
                cmd.Parameters.AddRange(values);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log.Error("ExecuteCommandProc function:" + e.Message + "\r\nSQL 语法：" + sql, e);
                throw e;
            }
        }
        #endregion

        #region ExecuteCommand
        /// <summary>
        /// 执行SQL语法
        /// </summary>
        /// <param name="safeSql"></param>
        /// <returns></returns>
        public int ExecuteCommand(string safeSql)
        {
            int result = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                log.Error("ExecuteCommand function:" + e.Message + "\r\nSQL 语法：" + safeSql, e);
                throw e;
            }
            return result;
        }
        public int ExecuteCommand(string sql, params SqlParameter[] values)
        {
            int result = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.Parameters.AddRange(values);
                result = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                log.Error("ExecuteCommand function:" + e.Message + "\r\nSQL 语法：" + sql, e);
                throw e;
            }
            return result;
        }
        #endregion

        #region ExecuteSqlTran ExecuteTranByID  ExecuteTranByNone 带事务回滚 yangwm add
        /// <summary>
        /// ExecuteSqlTran 执行SQL语句，带事务回滚
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="SQLString"></param>
        public void ExecuteSqlTran(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string SQLString)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = SQLString.ToString();
            cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;

            cmd.ExecuteNonQuery();
        }
        public int ExecuteTranByID(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string SQLString, params SqlParameter[] parameters)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = SQLString.ToString();
            cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }

            int indentity = 0;
            object obj = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                indentity = 0;
            }
            else
            {
                indentity = int.Parse(obj.ToString());
            }
            return indentity;
        }
        public void ExecuteTranByNone(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, string SQLString, params SqlParameter[] parameters)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = SQLString.ToString();
            cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;

            if (parameters != null)
            {
                foreach (SqlParameter parameter in parameters)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }

            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }
        #endregion

        #region 执行多行sql,返回执行结果
        /// <summary>
        /// SQLExecuteNonQueryTran(StringList)
        /// </summary>
        /// <param name="SQLStringList"></param>
        /// <returns></returns>
        public int SQLExecuteNonQueryTran(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, List<String> SQLStringList)
        {

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            //开始事务处理
            cmd.Transaction = trans;
            try
            {
                int count = 0;
                for (int i = 0; i < SQLStringList.Count; i++)
                {
                    string strSql = SQLStringList[i];
                    if (strSql.Trim().Length > 1)
                    {
                        cmd.CommandText = strSql;
                        count += cmd.ExecuteNonQuery();
                    }
                }
                //提交数据库事务
                trans.Commit();
                return count;
            }
            catch
            {
                //异常时回滚事务
                trans.Rollback();
                return 0;
            }

        }
        #endregion

        #region GetScalar 可返回类型是object类型
        /// <summary>
        /// GetScalar 可返回类型是object类型
        /// </summary>
        /// <param name="safeSql"></param>
        /// <returns></returns>
        public int GetScalar(string safeSql)
        {
            int result = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                result = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                log.Error("GetScalar function:" + e.Message + "\r\nSQL 语法：" + safeSql, e);
                throw e;
            }
            return result;
        }
        public int GetScalar(string sql, params SqlParameter[] values)
        {
            int result = 0;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.Parameters.AddRange(values);
                result = Convert.ToInt32(cmd.ExecuteScalar());
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                log.Error("GetScalar function:" + e.Message + "\r\nSQL 语法：" + sql, e);
                throw e;
            }
            return result;
        }
        #endregion

        #region GetReader 返回Datareader对象
        /// <summary>
        /// GetReader 返回Datareader对象
        /// </summary>
        /// <param name="safeSql"></param>
        /// <returns></returns>
        public SqlDataReader GetReader(string safeSql)
        {
            SqlDataReader reader = null;
            try
            {
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                reader = cmd.ExecuteReader();
            }
            catch (Exception e)
            {
                log.Error("GetReader function:" + e.Message + "\r\nSQL 语法：" + safeSql, e);
                throw e;
            }
            return reader;
        }
        public SqlDataReader GetReader(string sql, params SqlParameter[] values)
        {
            SqlDataReader reader = null;
            try
            {
                SqlCommand cmd = new SqlCommand(sql, Connection);
                cmd.Parameters.AddRange(values);
                reader = cmd.ExecuteReader();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                log.Error("GetReader function:" + e.Message + "\r\nSQL 语法：" + sql, e);
                throw e;
            }
            return reader;
        }
        #endregion

        #region GetDataSet
        /// <summary>
        /// 执行SQL语法，返回DataTable
        /// </summary>
        /// <param name="safeSql"></param>
        /// <returns></returns>
        public DataTable GetDataSet(string safeSql)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(ds);
            }
            catch (Exception e)
            {
                log.Error("GetDataSet function:" + e.Message + "\r\nSQL 语法：" + safeSql, e);
                throw e;
            }
            return ds.Tables[0];
        }
        public DataTable GetDataSet(string safeSql, int Times)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.SelectCommand.CommandTimeout = Times;
                da.Fill(ds);
            }
            catch (Exception e)
            {
                log.Error("GetDataSet function:" + e.Message + "\r\nSQL 语法：" + safeSql, e);
                throw e;
            }
            return ds.Tables[0];
        }
        public DataTable GetDataSet(string sql, params SqlParameter[] values)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand(sql, Connection);
                PrepareCommand(cmd, null, sql, values);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                log.Error("GetDataSet function:" + e.Message + "\r\nSQL 语法：" + sql, e);
                throw e;
            }
            return ds.Tables[0];
        }
        public DataSet GetDataSet2(string safeSql)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception e)
            {
                log.Error("GetDataSet function:" + e.Message + "\r\nSQL 语法：" + safeSql, e);
                throw e;
            }
            return ds;
        }
        public DataSet GetDataSet2(string safeSql, params SqlParameter[] values)
        {
            DataSet ds = new DataSet();
            try
            {
                SqlCommand cmd = new SqlCommand(safeSql, Connection);
                cmd.Parameters.AddRange(values);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception e)
            {
                log.Error("GetDataSet function:" + e.Message + "\r\nSQL 语法：" + safeSql, e);
                throw e;
            }
            return ds;
        }
        #endregion

        #region 参数赋值,排除无值参数出错问题
        private void PrepareCommand(SqlCommand cmd, SqlTransaction trans, string cmdText, SqlParameter[] cmdParms)
        {
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (SqlParameter parameter in cmdParms)
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

        #region 锁表 LockTable
        /// <summary>
        /// 锁表
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="rkey">RKEY值</param>
        /// <param name="s_SystemName">模块名</param>
        /// <returns>0为正常，-1为锁，-2为出错</returns>
        public long LockTable(string table, string rkey, string s_SystemName)
        {
            long i_ret = -1;
            string s_SQL;

            try
            {
                s_SQL = @"
SET lock_timeout 1000;
set implicit_transactions on;
SET NO_BROWSETABLE ON;
SELECT RKEY FROM " + table + " WITH (UPDLOCK, ROWLOCK)  WHERE RKEY = " + rkey.ToString() + @"
";
                SqlCommand cmd = new SqlCommand(s_SQL, Connection);
                try
                {
                    cmd.ExecuteNonQuery();
                    i_ret = 0;
                }
                catch (Exception e1)
                {
                    if (e1.Message.IndexOf("锁请求") >= 0)
                    {
                        i_ret = -1;//被锁
                        log.Error("LockTable function:" + e1.Message + "\r\nSQL 语法：" + s_SQL, e1);
                    }
                    else
                        i_ret = 0;
                }

                if (i_ret == 0)
                {
                    ExecuteNonQuery("DELETE FROM DATA0540 WHERE (TABLENAME = '" + table.ToUpper() + "') AND (LOCKED_RECORD_KEY = " + rkey.ToString() + ")");
                    ExecuteNonQuery("exec SP_INSERTDATA0540 '" + table.ToUpper() + "'," + rkey.ToString() + ",'" + System.Net.Dns.GetHostName() + "','" + GlobalVal.UserInfo.AD.Cn.Trim() + "','" + s_SystemName + "','" + GlobalVal.UserInfo.AD.Title.Trim() + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "',0");
                }
            }
            catch
            {
                i_ret = -2;//出错
            }
            return i_ret;
        }
        /// <summary>
        /// 锁表
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="Primary">主键名</param>
        /// <param name="rkey">RKEY值</param>
        /// <param name="s_SystemName">模块名</param>
        /// <returns>0为正常，-1为锁，-2为出错</returns>
        public int LockTable(string table, string Primary, string rkey, string s_SystemName)
        {
            int i_ret = -1;
            string s_SQL;

            try
            {
                s_SQL = @"
SET lock_timeout 1000;
set implicit_transactions on;
SET NO_BROWSETABLE ON;
SELECT " + Primary + " FROM " + table + " WITH (UPDLOCK, ROWLOCK)  WHERE " + Primary + " = " + rkey.ToString() + @"
";
                SqlCommand cmd = new SqlCommand(s_SQL, Connection);
                try
                {
                    cmd.ExecuteNonQuery();
                    i_ret = 0;
                }
                catch (Exception e1)
                {
                    if (e1.Message.IndexOf("锁请求") >= 0)
                    {
                        i_ret = -1;//被锁
                        log.Error("LockTable function:" + e1.Message + "\r\nSQL 语法：" + s_SQL, e1);
                    }
                    else
                        i_ret = 0;
                }

                if (i_ret == 0)
                {
                    // ExecuteNonQuery("DELETE FROM DATA0540 WHERE (TABLENAME = '" + table.ToUpper() + "') AND (LOCKED_RECORD_KEY = " + rkey.ToString() + ")");
                    //ExecuteNonQuery("exec SP_INSERTDATA0540 '" + table.ToUpper() + "'," + rkey.ToString() + ",'" + System.Net.Dns.GetHostName() + "','" + GlobalVal.UserInfo.AD.Cn.Trim() + "','" + s_SystemName + "','" + GlobalVal.UserInfo.AD.Title.Trim() + "','" + DateTime.Now.ToString() + "',0");
                }
            }
            catch
            {
                i_ret = -2;//出错
            }
            return i_ret;
        }
        #endregion

        #region 解锁 UnLockTable
        /// <summary>
        /// 解锁
        /// </summary>
        /// <returns></returns>
        public int UnLockTable()
        {
            int i_ret = -1;
            string s_SQL;

            try
            {
                s_SQL = "COMMIT TRAN;";

                SqlCommand cmd = new SqlCommand(s_SQL, Connection);
                try
                {
                    cmd.ExecuteNonQuery();
                    i_ret = 0;
                }
                catch (Exception e1)
                {
                    i_ret = -1;//出错 
                    log.Error("LockTable function:" + e1.Message + "\r\nSQL 语法：" + s_SQL, e1);
                }
            }
            catch
            {
                i_ret = -2;//出错
            }

            return i_ret;
        }
        #endregion

        #region 检查锁 CheckLockTable
        /// <summary>
        /// 检查锁
        /// </summary>
        /// <param name="table"></param>
        /// <param name="Primary"></param>
        /// <param name="rkey"></param>
        /// <returns></returns>
        public Boolean CheckLockTable(string table, string Primary, string rkey)
        {
            try
            {
                string s_SQL = @"
SET lock_timeout 1000; 
SELECT " + Primary + " FROM " + table + " WITH (UPDLOCK, ROWLOCK)  WHERE " + Primary + " = " + rkey.ToString() + @"
";
                SqlCommand cmd = new SqlCommand(s_SQL, Connection);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    PrintLock(table, rkey);
                    return true;
                }
                return false;
            }
            catch
            {
                PrintLock(table, rkey);
                return true;
            }
        }
        /// <summary>
        /// 检查锁
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rkey"></param>
        /// <returns></returns>
        public Boolean CheckLockTable(string table, string rkey)
        {
            try
            {
                string s_SQL = @"
SET lock_timeout 1000; 
SELECT rkey FROM " + table + " WITH (UPDLOCK, ROWLOCK)  WHERE rkey = " + rkey.ToString() + @"
";
                SqlCommand cmd = new SqlCommand(s_SQL, Connection);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    PrintLock(table, rkey);
                    return true;
                }
                return false;
            }
            catch
            {
                PrintLock(table, rkey);
                return true;
            }
        }
        /// <summary>
        /// 检查锁
        /// </summary>
        /// <param name="table"></param>
        /// <param name="rkey"></param>
        /// <returns></returns>
        public Boolean CheckLockTableWithNoAlert(string table, string rkey)
        {
            try
            {
                string s_SQL = @"
                            SET lock_timeout 1000; 
                            SELECT rkey FROM " + table + " WITH (UPDLOCK, ROWLOCK)  WHERE rkey = " + rkey.ToString();
                SqlCommand cmd = new SqlCommand(s_SQL, Connection);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return true;
            }
        }
        #endregion

        #region 锁表情况显示 PrintLock
        /// <summary>
        /// 锁表情况显示
        /// </summary>
        /// <param name="table">表名</param>
        /// <param name="rkey">RKEY值</param>
        public void PrintLock(string table, string rkey)
        {
            string s_SQL;
            DataTable tb;

            s_SQL = "select * from data0540 with (nolock) where tablename='" + table.ToUpper().Trim() + "' and locked_record_key=" + rkey.ToString() + " order by tdate_time desc";
            tb = GetDataSet(s_SQL);
            if (tb.Rows.Count > 0)
                MessageBox.Show("锁住的表：" + table + "\n\r锁住的记录主键：" + rkey.ToString() + "\n\r计算机名：" + tb.Rows[0]["COMPUTER_NAME"].ToString() + "\n\r网络用户名：" + tb.Rows[0]["NETWORK_USER"].ToString() + "\n\r程序名：" + tb.Rows[0]["APPLICATION_NAME"].ToString() + "\n\r用户：" + tb.Rows[0]["PARADIGM_USER"].ToString() + "\n\r时间：" + tb.Rows[0]["TDATE_TIME"].ToString(), "警告");
            else
                MessageBox.Show("锁住的表：" + table + "\n\r锁住的记录主键：" + rkey.ToString() + " 已被锁定！", "警告");
        }
        #endregion

        #region CloseConnection
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void CloseConnection()
        {
            if (Only)
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                {
                    try
                    {
                        connection.Close();
                    }
                    catch (Exception e1)
                    {
                        log.PrintInfo(e1);
                    }
                }
            }
            else
            {
                if (GlobalVal.UserInfo.Connection[ThreadID] != null)
                {
                    try
                    {
                        GlobalVal.UserInfo.Connection[ThreadID].Close();
                    }
                    catch (Exception e1)
                    {
                        log.PrintInfo(e1);
                    }
                }
            }
        }
        #endregion

        #region 释放资源
        //// 是否已经释放资源的标志
        private bool disposed = false;
        ///// <summary>
        ///// 析构函数
        ///// </summary>
        //~DBHelper()
        //{
        //    if (this.disposed == false)
        //    {
        //        CloseConnection();
        //        disposed = true;
        //    }
        //}
        /// <summary>
        /// 资源释放,提供给外部用户显式调用的方法
        /// </summary>
        public void Dispose()
        {
            if (this.disposed == false)
            {
                CloseConnection();
                if (Only)
                {
                    if (connection != null)
                        connection.Dispose();
                }
                else
                {
                    if (GlobalVal.UserInfo.Connection[ThreadID] != null)
                        GlobalVal.UserInfo.Connection[ThreadID].Dispose();
                }
                disposed = true;
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DBHelper() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion

    }
    /// <summary>
    /// 工厂
    /// </summary>
    public enum Factory
    {
        /// <summary>
        /// 珠海(1)
        /// </summary>
        ZH = 1,
        /// <summary>
        /// 郑州(2)
        /// </summary>
        ZZ = 2,
        /// <summary>
        /// 重庆(3)
        /// </summary>
        CQ = 3,
        /// <summary>
        /// 合肥(4)
        /// </summary>
        HF = 4
        
    }
}