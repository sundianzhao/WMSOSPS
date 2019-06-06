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
                    if (sqlex.State == 0 && CNum < 3)//����ʧ��
                    {
                        CNum++;
                        SqlHelper.SqlConnectLog.Info(sqlex.Message + "_��" + CNum + "������");
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
                    if (sqlex.State == 0 && CNum < 3)//����ʧ��
                    {
                        CNum++;
                        SqlHelper.SqlConnectLog.Info(sqlex.Message + "_��" + CNum + "������");
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
                    if (sqlex.State == 0 && CNum < 3)//����ʧ��
                    {
                        CNum++;
                        SqlHelper.SqlConnectLog.Info(sqlex.Message + "_��" + CNum + "������");
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

            //ʹ������
            using (SqlConnection con = new SqlConnection(connectionstring))
            {
                if (con.State == ConnectionState.Closed)
                    con.Open();
                SqlTransaction tran = con.BeginTransaction();//��������
                try
                {
                    SqlCommand command = new SqlCommand();
                    command.Transaction = tran;
                    PrepareCommand(command, con, null, cmdtype, cmdText, cmdParms);
                    int i = command.ExecuteNonQuery();
                    command.Parameters.Clear();
                    tran.Commit();//�����ύ
                    return i;
                }
                catch
                {
                    tran.Rollback();//����ع�
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
        /// ��ɾ�ķ���
        /// </summary>
        /// <param name="sql">sql���</param>
        /// <param name="cy">sql�������</param>
        /// <param name="sq">������ֵ</param>
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

        #region ��������DataSet��ѯ
        /// <summary>
        /// ִ�д洢����
        /// </summary>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <param name="tableName">DataSet����еı���</param>
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
        /// ���� SqlCommand ����(��������һ���������������һ������ֵ)
        /// </summary>
        /// <param name="connection">���ݿ�����</param>
        /// <param name="storedProcName">�洢������</param>
        /// <param name="parameters">�洢���̲���</param>
        /// <returns>SqlCommand</returns>
        private static SqlCommand BuildQueryCommand(SqlConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            SqlCommand command = new SqlCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter parameter in parameters)
            {
                if (parameter != null)
                {
                    // ���δ����ֵ���������,���������DBNull.Value.
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
--@currentPageIndex:��ʼ����  @pageSize:��ҳ������ 
--declare @currentPageIndex int
--declare @pageSize int
SELECT COUNT(*) AS Row_Count FROM (
--SQL���--start--
{0}
--SQL���--end--
) TT
SELECT * FROM (SELECT Row_Number() OVER(ORDER BY tbl_Data.{1}) as rowNum, tbl_Data.* FROM (
--SQL���--start--
{0}
--SQL���--end--
) AS tbl_Data) AS T where T.rowNum between ({2}-1)*{3}+1 and {2}*{3} ORDER BY T.{1} 
", originationSql, pagination.sidx, pagination.page, pagination.rows);

            return sqlString;
        }

        /// <summary>
        /// SqlHelperִ�д洢����ͨ�÷���:����SqlDataReader����
        /// </summary>
        /// <param name="proText">�洢��������</param>
        /// <param name="cmdType">��ѯ����</param>
        /// <param name="pars">�����б�</param>
        /// <returns>����:SqlDataReader</returns>
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
                    //��������,��ӵ�SqlCommand ������ִ��
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
                    //��������,��ӵ�SqlCommand ������ִ��
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
                throw new Exception("ת������!", ex);
            }

        }

        /// <summary>
        /// DataTable��ҳ��ȡ��ָ��ҳ�������
        /// </summary>
        /// <param name="dtAll">DataTable</param>
        /// <param name="pageNo">ҳ��,ע�⣺��1��ʼ</param>
        /// <param name="pageSize">ÿҳ����</param>
        /// <returns>ָ��ҳ���DataTable����</returns>
        private DataTable GetOnePageTable(DataTable dtAll, int pageNo, int pageSize)
        {
            var totalCount = dtAll.Rows.Count;
            var totalPage = GetTotalPage(totalCount, pageSize);
            var currentPage = pageNo;
            currentPage = (currentPage > totalPage ? totalPage : currentPage);//���PageNo���������PageNo=PageCount
            currentPage = (currentPage <= 0 ? 1 : currentPage);//���PageNo<=0�����Ϊ��ҳ
            //----��¡��ṹ���±�
            var onePageTable = dtAll.Clone();
            //----ȡ��1ҳ���ݵ��±�
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
        /// ���ط�ҳ�����ҳ��
        /// </summary>
        /// <param name="totalCount">�ܼ�¼����</param>
        /// <param name="pageSize">ÿҳ��ʾ����</param>
        /// <returns>��ҳ��</returns>
        public int GetTotalPage(int totalCount, int pageSize)
        {
            var totalPage = (totalCount / pageSize) + (totalCount % pageSize > 0 ? 1 : 0);
            return totalPage;
        }
    }
}
