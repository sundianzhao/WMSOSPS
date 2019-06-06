using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using WMSOSPS.Cloud.Code.Web;

namespace WMSOSPS.Cloud.Code.SqlHelper
{
    public class SqlHelper
    {
        
        protected static readonly log4net.ILog SqlConnectLog = log4net.LogManager.GetLogger("SqlConnectLog");

        public static readonly string connstr = ConfigurationManager.ConnectionStrings["CloudDB"].ConnectionString;
        public static readonly string connstrs = ConfigurationManager.ConnectionStrings["CloudDB"].ConnectionString;
        public static readonly string conn = ConfigurationManager.ConnectionStrings["CloudDB"].ConnectionString;
        public static DataTable ExecuteDataTable(string connectionstring, CommandType cmdtype, string commandText, params SqlParameter[] cmdParms)
        {
            int CNum = 0;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                System.Data.SqlClient.SqlCommand command = new SqlCommand();
                command.CommandText = commandText;
                command.CommandType = cmdtype;
                command.Connection = con;
                command.CommandTimeout = 1000 * 60 * 10;
                if (cmdParms != null)
                {
                    command.Parameters.AddRange(cmdParms);
                }
                adapter.SelectCommand = command;
            ConRest:
                try
                {
                    adapter.Fill(ds, "tb");
                }
                catch (SqlException sqlex)
                {
                    if (sqlex.State == 0 && CNum < 3)//连接失败
                    {
                        CNum++;
                        SqlHelper.SqlConnectLog.Info(sqlex.Message + "_第" + CNum + "次重试");
                        goto ConRest;
                    }
                }
                finally
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
            return ds.Tables["tb"];
        }
        public static DataTable ExecuteDataTable(string connectionstring, CommandType cmdtype, string commandText, int timeout, params SqlParameter[] cmdParms)
        {
            int CNum = 0;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                System.Data.SqlClient.SqlCommand command = new SqlCommand();
                command.CommandText = commandText;
                command.CommandType = cmdtype;
                command.Connection = con;
                command.CommandTimeout = timeout;
                if (cmdParms != null)
                {
                    command.Parameters.AddRange(cmdParms);
                }
                adapter.SelectCommand = command; ConRest:
                try
                {
                    adapter.Fill(ds, "tb");
                }
                catch (SqlException sqlex)
                {
                    if (sqlex.State == 0 && CNum < 3)//连接失败
                    {
                        CNum++;
                        SqlHelper.SqlConnectLog.Info(sqlex.Message + "_第" + CNum + "次重试");
                        goto ConRest;
                    }
                }
                finally
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }

            return ds.Tables["tb"];

        }
        public static DataTable ExecuteM_LogDataTable(string connectionstring, CommandType cmdtype, string commandText)
        {
            int CNum = 0;
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                System.Data.SqlClient.SqlCommand command = new SqlCommand();
                command.CommandText = commandText;
                command.CommandType = cmdtype;
                command.Connection = con;
                adapter.SelectCommand = command;
            ConRest:
                try
                {
                    adapter.Fill(ds, "tb");
                }
                catch (SqlException sqlex)
                {
                    if (sqlex.State == 0 && CNum < 3)//连接失败
                    {
                        CNum++;
                        SqlHelper.SqlConnectLog.Info(sqlex.Message + "_第" + CNum + "次重试");
                        goto ConRest;
                    }
                }
                finally
                {
                    command.Connection.Close();
                    command.Dispose();
                }
            }
            return ds.Tables["tb"];
        }
        public static int InsertAndReturnID(string connectionstring, CommandType cmdtype, string commandText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                SqlCommand myCommand = new SqlCommand();
                conn.Open();
                SqlTransaction myTrans = conn.BeginTransaction();
                myCommand.Transaction = myTrans;
                try
                {
                    PrepareCommand(myCommand, conn, null, cmdtype, commandText + ";select scope_identity();", cmdParms);
                    Object o = myCommand.ExecuteScalar();
                    myCommand.Parameters.Clear();
                    myTrans.Commit();
                    return Convert.ToInt32(o);
                }
                catch (Exception e)
                {
                    try
                    {
                        myTrans.Rollback();
                        return 0;
                    }
                    catch (SqlException ex)
                    {
                        return 0;
                    }
                }
                finally
                {
                    conn.Close();
                }

            }
        }
        public static int ExecuteUpdate(string connectionstring, CommandType cmdtype, string cmdText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                try
                {
                    SqlCommand command = new SqlCommand();
                    PrepareCommand(command, con, null, cmdtype, cmdText, cmdParms);
                    int i = command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    return i;

                }
                catch (Exception sqlex)
                {
                    SqlHelper.SqlConnectLog.Info(connectionstring + "*****************" + sqlex.Message);
                    return 0;
                }
                finally
                {
                    con.Close();
                }
            }

        }
        public static int ExecuteUpdate(string connectionstring, CommandType cmdtype, string cmdText, bool useTrans, params SqlParameter[] cmdParms)
        {
            if (!useTrans)
                return ExecuteUpdate(connectionstring, cmdtype, cmdText, cmdParms);

            //使用事务
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlTransaction tran = con.BeginTransaction();//开启事务
                try
                {
                    SqlCommand command = new SqlCommand();
                    command.Transaction = tran;
                    PrepareCommand(command, con, null, cmdtype, cmdText, cmdParms);
                    int i = command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    tran.Commit();//事务提交
                    return i;
                }
                catch
                {
                    tran.Rollback();//事务回滚
                    return 0;
                }
                finally
                {
                    con.Close();
                }
            }

        }
        public static int DeleteInfo(string sql)
        {
            var result = 0;
            using (var conn = new SqlConnection(connstrs))
            {
                conn.Open();
                using (var com=conn.CreateCommand())
                {
                    com.CommandText = sql;
                    result = com.ExecuteNonQuery();
                    return result;
                }   
            }
        }
        public static DataTable QuerySQL(string sql, string tableName,int str = 0)
        {
            var ctr = str > 0 ? connstr : connstrs;
            using (SqlConnection connection = new SqlConnection(ctr))
            {
                connection.Open();
                SqlDataAdapter sqlda = new SqlDataAdapter(sql, ctr);
                DataSet ds = new DataSet();
                sqlda.Fill(ds);
                return ds.Tables[0];
            }
        }
        public static int SQLExecuteNonQuery(string sql, int str = 0)
        {
            var result = 0;
            var ctr = str > 0 ? connstr : connstrs;
            using (var conn = new SqlConnection(ctr))
            {
                conn.Open();
                using (var com = new SqlCommand(sql, conn) )
                {
                    result = com.ExecuteNonQuery();
                    return result;
                }
            }
        }
        public static int ExecuteUpdateorInsertorDelete(string connectionstring, CommandType cmdtype, string cmdText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                try
                {
                    SqlCommand command = new SqlCommand();
                    PrepareCommand(command, con, null, cmdtype, cmdText, cmdParms);
                    int i = command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    con.Close();
                    return i;
                }
                catch (Exception sqlex)
                {
                    SqlHelper.SqlConnectLog.Info(connectionstring + "*****************" + sqlex.Message);
                    return 0;
                }
                finally
                {
                    con.Close();
                }
            }
          
        }
        public static int ExecuteUpdateorInsertorDelete(SqlTransaction trans, CommandType cmdtype, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, trans.Connection, trans, cmdtype, cmdText, cmdParms);
            int i = command.ExecuteNonQuery();
            command.Parameters.Clear();
            trans.Connection.Close();
            return i;
        }
        public static Object ExecuteScalar(string connectionstring, CommandType cmdtype, string cmdText, params SqlParameter[] cmdParms)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand command = new SqlCommand();
                PrepareCommand(command, con, null, cmdtype, cmdText, cmdParms);
                Object o = command.ExecuteScalar();
                command.Parameters.Clear();
                return o;
            }

        }
        public static Object ExecuteScalar(SqlConnection con, CommandType cmdtype, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, con, null, cmdtype, cmdText, cmdParms);
            Object o = command.ExecuteScalar();
            command.Parameters.Clear();
            return o;
        }
        public static int ExecuteScalar(SqlTransaction trans, CommandType cmdtype, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand command = new SqlCommand();
            PrepareCommand(command, trans.Connection, trans, cmdtype, cmdText, cmdParms);
            int i = command.ExecuteNonQuery();
            command.Parameters.Clear();
            return i;
        }
        private static void PrepareCommand(SqlCommand cmd, SqlConnection conn, SqlTransaction trans, CommandType cmdType, string cmdText, SqlParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            if (cmdParms != null)
            {
                foreach (SqlParameter sp in cmdParms)
                {
                    if (sp.Value == null)
                        sp.Value = DBNull.Value;
                }
                cmd.Parameters.AddRange(cmdParms);
            }
        }
        public static SqlDataReader ExecuteReader(string connectionstring, CommandType cmdtype, string cmdText, params SqlParameter[] cmdParms)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = new SqlConnection(connectionstring);
            try
            {
                PrepareCommand(cmd, conn, null, cmdtype, cmdText, cmdParms);
                SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch
            {
                conn.Close();
                throw;
            }

        }
        public static DataSet ExecuteDataSet(string connectionstring, CommandType cmdtype, string commandText)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                System.Data.SqlClient.SqlCommand command = new SqlCommand();
                command.CommandText = commandText;
                command.CommandType = cmdtype;
                command.Connection = con;
                adapter.SelectCommand = command;
                adapter.Fill(ds);
            }
            return ds;
        }

        /// <summary>
        /// 增删改方法
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="cy">sql语句类型</param>
        /// <param name="sq">参数赋值</param>
        /// <returns></returns>
        public static int executeNonQuery(string connectionstring, string sql, CommandType cy)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = con.CreateCommand();
                cmd.CommandText = sql;
                cmd.CommandType = cy;
                con.Open();
                cmd.CommandType = cy;
                return cmd.ExecuteNonQuery();
            }
        }

        #region 带参数的DataSet查询
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="tableName">DataSet结果中的表名</param>
        /// <returns>DataSet</returns>
        public static DataSet RunProcedure(string connectionString, string storedProcName, IDataParameter[] parameters, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                SqlDataAdapter sqlDA = new SqlDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, storedProcName, parameters);
                sqlDA.Fill(dataSet, tableName);
                connection.Close();
                return dataSet;
            }
        }
        /// <summary>
        /// 构建 SqlCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
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
        #endregion

        public static DataSet ExecuteDataSet(string connectionstring, CommandType cmdtype, string commandText, params SqlParameter[] cmdParms)
        {
            DataSet ds = new DataSet();
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                System.Data.SqlClient.SqlCommand command = new SqlCommand();
                command.CommandText = commandText;
                command.CommandType = cmdtype;
                command.Connection = con;
                command.Parameters.AddRange(cmdParms);
                adapter.SelectCommand = command;
                adapter.Fill(ds);
            }
            return ds;
        }

        public static string PagerStr(string originationSql, Pagination pagination)
        {
            string sqlString = string.Empty;
            sqlString = string.Format(@"
--@currentPageIndex:起始行数  @pageSize:本页的行数 
--declare @currentPageIndex int
--declare @pageSize int
SELECT COUNT(*) AS Row_Count FROM (
--SQL语句--start--
{0}
--SQL语句--end--
) TT
SELECT * FROM (SELECT Row_Number() OVER(ORDER BY tbl_Data.{1}) as rowNum, tbl_Data.* FROM (
--SQL语句--start--
{0}
--SQL语句--end--
) AS tbl_Data) AS T where T.rowNum between ({2}-1)*{3}+1 and {2}*{3} ORDER BY T.{1} 
", originationSql, pagination.sidx, pagination.page, pagination.rows);

            return sqlString;
        }

        /// <summary>
        /// SqlHelper执行存储过程通用方法:返回SqlDataReader对象
        /// </summary>
        /// <param name="proText">存储过程名字</param>
        /// <param name="cmdType">查询类型</param>
        /// <param name="pars">参数列表</param>
        /// <returns>返回:SqlDataReader</returns>
        public static SqlDataReader ExecProcdureReturnDataReader(string connectionstring,  CommandType cmdType, string cmdText, params SqlParameter[] pars)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = cmdType;
                cmd.CommandText = cmdText;
                if (pars != null)
                {
                    //遍历参数,添加到SqlCommand 对象子执行
                    foreach (SqlParameter parm in pars)
                        cmd.Parameters.Add(parm);//
                }
                return cmd.ExecuteReader();
            }
         
        }

        public static DataTable ExecProcdureReturnDataReader1(string connectionstring, CommandType cmdType, string cmdText, params SqlParameter[] pars)
        {
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandType = cmdType;
                cmd.CommandText = cmdText;
                if (pars != null)
                {
                    //遍历参数,添加到SqlCommand 对象子执行
                    foreach (SqlParameter parm in pars)
                        cmd.Parameters.Add(parm);//
                }
                var reader= cmd.ExecuteReader();

                DataTable objDataTable = new DataTable();
                int intFieldCount = reader.FieldCount;
                for (int intCounter = 0; intCounter < intFieldCount; ++intCounter)
                {
                    objDataTable.Columns.Add(reader.GetName(intCounter), reader.GetFieldType(intCounter));
                }

                objDataTable.BeginLoadData();

                object[] objValues = new object[intFieldCount];
                while (reader.Read())
                {
                    reader.GetValues(objValues);
                    objDataTable.LoadDataRow(objValues, true);
                }
                reader.Close();
                objDataTable.EndLoadData();

                return objDataTable;
            }


        }

        public static DataTable ConvertDataReaderToDataTable(SqlDataReader reader)
        {
            try
            {
                DataTable objDataTable = new DataTable();
                int intFieldCount = reader.FieldCount;
                for (int intCounter = 0; intCounter < intFieldCount; ++intCounter)
                {
                    objDataTable.Columns.Add(reader.GetName(intCounter), reader.GetFieldType(intCounter));
                }

                objDataTable.BeginLoadData();

                object[] objValues = new object[intFieldCount];
                while (reader.Read())
                {
                    reader.GetValues(objValues);
                    objDataTable.LoadDataRow(objValues, true);
                }
                reader.Close();
                objDataTable.EndLoadData();

                return objDataTable;

            }
            catch (Exception ex)
            {
                throw new Exception("转换出错!", ex);
            }

        }

        /// <summary>
        /// DataTable分页并取出指定页码的数据
        /// </summary>
        /// <param name="dtAll">DataTable</param>
        /// <param name="pageNo">页码,注意：从1开始</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns>指定页码的DataTable数据</returns>
        private DataTable GetOnePageTable(DataTable dtAll, int pageNo, int pageSize)
        {
            var totalCount = dtAll.Rows.Count;
            var totalPage = GetTotalPage(totalCount, pageSize);
            var currentPage = pageNo;
            currentPage = (currentPage > totalPage ? totalPage : currentPage);//如果PageNo过大，则较正PageNo=PageCount
            currentPage = (currentPage <= 0 ? 1 : currentPage);//如果PageNo<=0，则改为首页
            //----克隆表结构到新表
            var onePageTable = dtAll.Clone();
            //----取出1页数据到新表
            var rowBegin = (currentPage - 1) * pageSize;
            var rowEnd = currentPage * pageSize;
            rowEnd = (rowEnd > totalCount ? totalCount : rowEnd);
            for (var i = rowBegin; i <= rowEnd - 1; i++)
            {
                var newRow = onePageTable.NewRow();
                var oldRow = dtAll.Rows[i];
                foreach (DataColumn column in dtAll.Columns)
                {
                    newRow[column.ColumnName] = oldRow[column.ColumnName];
                }
                onePageTable.Rows.Add(newRow);
            }
            return onePageTable;
        }

        /// <summary>
        /// 返回分页后的总页数
        /// </summary>
        /// <param name="totalCount">总记录条数</param>
        /// <param name="pageSize">每页显示条数</param>
        /// <returns>总页数</returns>
        public int GetTotalPage(int totalCount, int pageSize)
        {
            var totalPage = (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
            return totalPage;
        }
    }
}
