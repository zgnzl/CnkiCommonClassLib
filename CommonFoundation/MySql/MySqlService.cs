//--------------------------------------------------------------------------------
// 文件描述：MySql数据库助手
// 文件作者：罗刚
// 创建日期：
// 修改记录：
// 2016-5-12 刘畅 修改为对象实例化调用
//--------------------------------------------------------------------------------
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Linq.Expressions;

namespace CommonFoundation.MySql
{
    public class MySqlService
    {
        #region 委托
        /// <summary>
        /// 声明委托，从DataRow中读取相应的数据到实体对象中
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="dr">DataRow</param>
        /// <returns>加载数据的实体对象</returns>
        public delegate T DataRowToModel<T>(DataRow dr);
        #endregion

        #region 变量
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string m_strCon = string.Empty;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connStr"></param>
        public MySqlService(string connStr)
        {
            m_strCon = connStr;
        }
        #endregion

        #region 创建连接
        /// <summary>
        /// 创建连接
        /// </summary>
        public MySqlConnection GetConnection()
        {
            return GetConnection(m_strCon);
        }
        /// <summary>
        /// 创建连接
        /// </summary>
        public MySqlConnection GetConnection(string connstr)
        {
            MySqlConnection connection = null;

            if (!string.IsNullOrEmpty(connstr))
            {
                connection = new MySqlConnection(connstr);
                if (connection != null && (connection.State == System.Data.ConnectionState.Closed || connection.State == System.Data.ConnectionState.Broken))
                {
                    connection.Open();
                }
            }
            return connection;
        }
        #endregion

        #region 执行SQL语句或存储过程（ExecuteScalar）返回单个值
        /// <summary>
        /// 执行SQL语句，从数据库中检索单个值
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>返回结果（object）</returns>
        public object ExecuteScalar(string strSql)
        {
            return ExecuteScalar(CommandType.Text, strSql, null);
        }

        /// <summary>
        /// 执行SQL语句，从数据库中检索单个值
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>返回结果（object）</returns>
        public object ExecuteScalar(string strSql, Parameters cmdParams)
        {
            return ExecuteScalar(CommandType.Text, strSql, cmdParams);
        }

        /// <summary>
        /// 执行存储过程，从数据库中检索单个值
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <returns>返回结果（object）</returns>
        public object ExecuteScalarProc(string storedProcName)
        {
            return ExecuteScalar(CommandType.StoredProcedure, storedProcName, null);
        }

        /// <summary>
        /// 执行存储过程，从数据库中检索单个值
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>返回结果（object）</returns>
        public object ExecuteScalarProc(string storedProcName, Parameters cmdParams)
        {
            return ExecuteScalar(CommandType.StoredProcedure, storedProcName, cmdParams);
        }

        /// <summary>
        /// 执行SQL语句或存储过程，从数据库中检索单个值
        /// </summary>
        /// <param name="cmdType">命令字符串类型</param>
        /// <param name="cmdText">SQL语句或者存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>返回结果（object）</returns>
        public object ExecuteScalar(CommandType cmdType, string cmdText, Parameters cmdParams)
        {
            using (MySqlConnection conn = GetConnection())
            {
                return ExecuteScalar(conn, cmdType, cmdText, cmdParams);
            }
        }
        /// <summary>
        /// 执行SQL语句或存储过程，从数据库中检索单个值(使用原有连接)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public object ExecuteScalar(MySqlConnection conn, CommandType cmdType, string cmdText, Parameters cmdParams)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = cmdType;
            cmd.CommandText = cmdText;
            cmd.Connection = conn;
            if (cmdParams != null)
            {
                foreach (Parameter parameter in cmdParams.Entries)
                {
                    MySqlParameter parm = new MySqlParameter(parameter.Name, parameter.DBType, parameter.Size);
                    parm.Value = parameter.Value;
                    cmd.Parameters.Add(parm);
                }
            }
            return cmd.ExecuteScalar();
        }
        #endregion

        #region 检测记录是否存在
        /// <summary>
        /// 执行SQL语句，检测记录是否存在
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>如果记录存在，则为 true；否则为 false。</returns>
        public bool Exists(string strSql)
        {
            return Exists(CommandType.Text, strSql, null);
        }

        /// <summary>
        /// 执行SQL语句，检测记录是否存在
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>如果记录存在，则为 true；否则为 false。</returns>
        public bool Exists(string strSql, Parameters cmdParams)
        {
            return Exists(CommandType.Text, strSql, cmdParams);
        }

        /// <summary>
        /// 执行存储过程，检测记录是否存在
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <returns>如果记录存在，则为 true；否则为 false。</returns>
        public bool ExistsProc(string storedProcName)
        {
            return Exists(CommandType.StoredProcedure, storedProcName, null);
        }

        /// <summary>
        /// 执行存储过程，检测记录是否存在
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>如果记录存在，则为 true；否则为 false。</returns>
        public bool ExistsProc(string storedProcName, Parameters cmdParams)
        {
            return Exists(CommandType.StoredProcedure, storedProcName, cmdParams);
        }

        /// <summary>
        /// 执行SQL语句或者存储过程，检测记录是否存在
        /// </summary>
        /// <param name="commandType">命令字符串类型</param>
        /// <param name="strCommand">SQL语句或者存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>如果记录存在，则为 true；否则为 false。</returns>
        public bool Exists(CommandType commandType, string strCommand, Parameters cmdParams)
        {
            using (MySqlConnection conn = GetConnection())
            {
                return Exists(conn, commandType, strCommand, cmdParams);
            }
        }
        /// <summary>
        /// 执行SQL语句或者存储过程，检测记录是否存在(传入连接)
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandType"></param>
        /// <param name="strCommand"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public bool Exists(MySqlConnection conn, CommandType commandType, string strCommand, Parameters cmdParams)
        {
            bool ret = false;
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = strCommand;
            cmd.Connection = conn;
            if (cmdParams != null)
            {
                foreach (Parameter parameter in cmdParams.Entries)
                {
                    MySqlParameter parm = new MySqlParameter(parameter.Name, parameter.DBType, parameter.Size);
                    parm.Value = parameter.Value;
                    cmd.Parameters.Add(parm);
                }
            }

            using (DataSet ds = new DataSet())
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                    ret = (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0) ? true : false;
                }
            }
            return ret;
        }

        #endregion

        #region 返回第一列
        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <param name="strSql"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public List<string> ExecuteFirstColumn(string strSql, Parameters cmdParams)
        {
            using (MySqlConnection conn = GetConnection())
            {
                return ExecuteFirstColumn(conn, CommandType.Text, strSql, cmdParams);
            }
        }

        /// <summary>
        /// 返回第一列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storedProcName"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public List<string> GetFirstColumnProc(string storedProcName, Parameters cmdParams)
        {
            using (MySqlConnection conn = GetConnection())
            {
                return ExecuteFirstColumn(conn, CommandType.StoredProcedure, storedProcName, cmdParams);
            }
        }

        /// <summary>
        ///  返回第一列
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="cmdType"></param>
        /// <param name="strSql"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public List<string> ExecuteFirstColumn(MySqlConnection conn, CommandType cmdType, string strSql, Parameters cmdParams)
        {
            List<string> list = null;

            using (DataTable dt = ExecuteReader(conn, cmdType, strSql, cmdParams))
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    list = new List<string>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string str = dt.Rows[i][0].ToString();
                        if (!string.IsNullOrEmpty(str))
                        {
                            list.Add(str);
                        }
                    }
                }
            }
            return list;
        }


        #endregion

        #region 执行查询，返回实体对象
        /// <summary>
        /// 使用SQL语句将数据库记录读取到实体对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strSql"></param>
        /// <param name="dataRowToModel"></param>
        /// <returns></returns>
        public T ExecuteModel<T>(string strSql, DataRowToModel<T> dataRowToModel)
        {
            return ExecuteModel(strSql, null, dataRowToModel);
        }
        /// <summary>
        /// 使用SQL语句将数据库记录读取到实体对象中
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <param name="datareaderToModel">委托的方法</param>
        /// <returns>加载数据的实体对象</returns>
        public T ExecuteModel<T>(string strSql, Parameters cmdParams, DataRowToModel<T> dataRowToModel)
        {
            T model = default(T);
            using (DataTable dt = ExecuteReader(strSql, cmdParams))
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    model = dataRowToModel(dt.Rows[0]);
                }
            }
            return model;
        }


        /// <summary>
        /// 使用存储过程将数据库记录读取到实体对象中
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <param name="model">实体对象</param>
        /// <param name="datareaderToModel">委托的方法</param>
        /// <returns>加载数据的实体对象</returns>
        public T ExecuteModelProc<T>(string storedProcName, Parameters cmdParams, DataRowToModel<T> dataRowToModel)
        {
            T model = default(T);
            using (DataTable dt = ExecuteReaderProc(storedProcName, cmdParams))
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    model = dataRowToModel(dt.Rows[0]);
                }
            }
            return model;
        }
        /// <summary>
        /// 使用SQL语句将数据库记录读取到实体对象中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strSql"></param>
        /// <param name="cmdParams"></param>
        /// <param name="dataRowToModel"></param>
        /// <returns></returns>
        public List<T> ExecuteModelList<T>(string strSql, DataRowToModel<T> dataRowToModel)
        {
            return ExecuteModelList(strSql, null, dataRowToModel);
        }

        /// <summary>
        /// 使用SQL语句将数据库记录读取到实体对象中
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <param name="datareaderToModel">委托的方法</param>
        /// <returns>加载数据的实体对象</returns>
        public List<T> ExecuteModelList<T>(string strSql, Parameters cmdParams, DataRowToModel<T> dataRowToModel)
        {
            using (MySqlConnection conn = GetConnection())
            {
                return ExecuteModelList<T>(conn, strSql, cmdParams, dataRowToModel);
            }
        }

        /// <summary>
        /// 使用SQL语句将数据库记录读取到实体对象中
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="sqlConn">mysql连接</param>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <param name="datareaderToModel">委托的方法</param>
        /// <returns>加载数据的实体对象</returns>
        public List<T> ExecuteModelList<T>(MySqlConnection sqlConn, string strSql, Parameters cmdParams, DataRowToModel<T> dataRowToModel)
        {
            List<T> list = null;
            using (DataTable dt = ExecuteReader(sqlConn, CommandType.Text, strSql, cmdParams))
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    list = new List<T>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        T model = dataRowToModel(dt.Rows[i]);
                        if (model != null)
                        {
                            list.Add(model);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 使用存储过程将数据库记录读取到实体对象中
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <param name="datareaderToModel">委托的方法</param>
        /// <returns>加载数据的实体对象</returns>
        public List<T> ExecuteModeListProc<T>(string storedProcName, Parameters cmdParams, DataRowToModel<T> dataRowToModel)
        {
            List<T> list = null;
            using (DataTable dt = ExecuteReaderProc(storedProcName, cmdParams))
            {
                if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                {
                    list = new List<T>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        list.Add(dataRowToModel(dt.Rows[i]));
                    }
                }
            }
            return list;
        }

        #endregion

        #region 执行SQL语句或存储过程（ExecuteDataSet）返回DataSet
        /// <summary>
        /// 执行SQL语句，返回DataSet
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSetSql(string strSql)
        {
            return ExecuteDataSet(CommandType.Text, strSql, null);
        }

        /// <summary>
        /// 执行SQL语句，返回DataSet
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSetSql(string strSql, Parameters cmdParams)
        {
            return ExecuteDataSet(CommandType.Text, strSql, cmdParams);
        }

        /// <summary>
        /// 执行存储过程，返回DataSet
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSetProc(string storedProcName)
        {
            return ExecuteDataSet(CommandType.StoredProcedure, storedProcName, null);
        }

        /// <summary>
        /// 执行存储过程，返回DataSet
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSetProc(string storedProcName, Parameters cmdParams)
        {
            return ExecuteDataSet(CommandType.StoredProcedure, storedProcName, cmdParams);
        }

        /// <summary>
        /// 执行SQL语句或存储过程，返回DataSet
        /// </summary>
        /// <param name="commandType">命令字符串类型</param>
        /// <param name="strCommand">SQL语句或者存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>DataSet</returns>
        public DataSet ExecuteDataSet(CommandType commandType, string strCommand, Parameters cmdParams)
        {
            using (MySqlConnection conn = GetConnection())
            {
                return ExecuteDataSet(conn, commandType, strCommand, cmdParams);
            }
        }
        /// <summary>
        /// 执行SQL语句或存储过程，返回DataSet
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandType"></param>
        /// <param name="strCommand"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(MySqlConnection conn, CommandType commandType, string strCommand, Parameters cmdParams)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = strCommand;
            cmd.Connection = conn;

            if (cmdParams != null)
            {
                foreach (Parameter parameter in cmdParams.Entries)
                {
                    MySqlParameter parm = new MySqlParameter(parameter.Name, parameter.DBType, parameter.Size);
                    parm.Value = parameter.Value;
                    cmd.Parameters.Add(parm);
                }
            }

            DataSet ds = new DataSet();
            using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
            {
                da.Fill(ds);
            }
            return ds;
        }
        #endregion

        #region 执行SQL语句或存储过程（ExecuteReader）返回DataTable
        /// <summary>
        /// 执行SQL语句，返回DataTable
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteReader(string strSql)
        {
            return ExecuteReader(CommandType.Text, strSql, null);
        }

        /// <summary>
        /// 执行SQL语句，返回DataTable
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteReader(string strSql, Parameters cmdParams)
        {
            return ExecuteReader(CommandType.Text, strSql, cmdParams);
        }

        /// <summary>
        /// 执行存储过程，返回DataTable
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteReaderProc(string storedProcName)
        {
            return ExecuteReader(CommandType.StoredProcedure, storedProcName, null);
        }

        /// <summary>
        /// 执行存储过程，返回DataTable
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteReaderProc(string storedProcName, Parameters cmdParams)
        {
            return ExecuteReader(CommandType.StoredProcedure, storedProcName, cmdParams);
        }

        /// <summary>
        /// 执行SQL语句或存储过程，返回DataTable
        /// </summary>
        /// <param name="commandType">命令字符串类型</param>
        /// <param name="strCommand">SQL语句或者存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>DataTable</returns>
        public DataTable ExecuteReader(CommandType commandType, string strCommand, Parameters cmdParams)
        {
            using (MySqlConnection conn = GetConnection())
            {
                return ExecuteReader(conn, commandType, strCommand, cmdParams);
            }
        }

        /// <summary>
        /// 返回datatable(使用已有连接)
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="strCommand"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public DataTable ExecuteReader(MySqlConnection conn, CommandType commandType, string strCommand, Parameters cmdParams)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = strCommand;
            cmd.Connection = conn;

            if (cmdParams != null)
            {
                foreach (Parameter parameter in cmdParams.Entries)
                {
                    MySqlParameter parm = new MySqlParameter(parameter.Name, parameter.DBType, parameter.Size);
                    parm.Value = parameter.Value;
                    cmd.Parameters.Add(parm);
                }
            }

            DataTable dt = null;
            using (DataSet ds = new DataSet())
            {
                using (MySqlDataAdapter da = new MySqlDataAdapter(cmd))
                {
                    da.Fill(ds);
                }

                if (ds.Tables != null && ds.Tables.Count > 0)
                {
                    dt = ds.Tables[0];
                }
            }
            return dt;
        }
        #endregion

        #region 执行SQL语句或存储过程（ExecuteNonQuery）返回影响的记录数
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(string strSql)
        {
            return ExecuteNonQuery(CommandType.Text, strSql, null);
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(string strSql, Parameters cmdParams)
        {
            return ExecuteNonQuery(CommandType.Text, strSql, cmdParams);
        }

        /// <summary>
        /// 执行存储过程，返回影响的记录数
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQueryProc(string storedProcName)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, storedProcName, null);
        }

        /// <summary>
        /// 执行存储过程，返回影响的记录数
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQueryProc(string storedProcName, Parameters cmdParams)
        {
            return ExecuteNonQuery(CommandType.StoredProcedure, storedProcName, cmdParams);
        }

        /// <summary>
        /// 执行数据库操作
        /// </summary>
        /// <param name="commandType">命令字符串类型</param>
        /// <param name="strCommand">SQL语句或者存储过程名</param>
        /// <param name="cmdParams">参数数组</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteNonQuery(CommandType commandType, string strCommand, Parameters cmdParams)
        {
            using (MySqlConnection conn = GetConnection())
            {
                return ExecuteNonQuery(conn, commandType, strCommand, cmdParams);
            }
        }
        /// <summary>
        /// 执行数据库操作,使用已有连接
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="commandType"></param>
        /// <param name="strCommand"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(MySqlConnection conn, CommandType commandType, string strCommand, Parameters cmdParams)
        {
            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandType = commandType;
            cmd.CommandText = strCommand;
            cmd.Connection = conn;
            if (cmdParams != null)
            {
                foreach (Parameter parameter in cmdParams.Entries)
                {
                    MySqlParameter parm = new MySqlParameter(parameter.Name, parameter.DBType, parameter.Size);
                    parm.Value = parameter.Value;
                    cmd.Parameters.Add(parm);
                }
            }
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 返回刚插入数据库的自增id
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="strCommand"></param>
        /// <param name="cmdParams"></param>
        /// <param name="id"></param>
        public void ExecuteNonQuery(CommandType commandType, string strCommand, Parameters cmdParams, out int id)
        {
            using (MySqlConnection conn = GetConnection())
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.CommandType = commandType;
                cmd.CommandText = strCommand;
                cmd.Connection = conn;
                if (cmdParams != null)
                {
                    foreach (Parameter parameter in cmdParams.Entries)
                    {
                        MySqlParameter parm = new MySqlParameter(parameter.Name, parameter.DBType, parameter.Size);
                        parm.Value = parameter.Value;
                        cmd.Parameters.Add(parm);
                    }
                }
                if (cmd.ExecuteNonQuery() > 0)
                {
                    id = (int)cmd.LastInsertedId;
                }
                else
                {
                    id = -1;
                }
            }
        }

        #endregion

        #region 执行事务
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="sqlList"></param>
        public void ExecuteTransaction(List<string> sqlList)
        {
            if (sqlList != null && sqlList.Count > 0)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    MySqlTransaction trans = conn.BeginTransaction();
                    cmd.Transaction = trans;
                    try
                    {
                        foreach (string str in sqlList)
                        {
                            if (!string.IsNullOrEmpty(str))
                            {
                                cmd.CommandText = str;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        trans.Commit();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        trans.Rollback();
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
        }


        /// <summary>
        ///  执行事务
        /// </summary>
        /// <param name="sqlList"></param>
        /// <param name="paramList"></param>
        public bool ExecuteTransaction(List<string> sqlList, List<Parameters> paramList)
        {
            bool ret = false;
            if (sqlList != null && sqlList.Count > 0 && paramList != null && paramList.Count > 0)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    MySqlTransaction trans = conn.BeginTransaction();
                    cmd.Transaction = trans;
                    try
                    {
                        int count = sqlList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (!string.IsNullOrEmpty(sqlList[i]))
                            {
                                cmd.CommandText = sqlList[i];

                                if (paramList[i] != null)
                                {
                                    foreach (Parameter parameter in paramList[i].Entries)
                                    {
                                        MySqlParameter parm = new MySqlParameter(parameter.Name, parameter.DBType, parameter.Size);
                                        parm.Value = parameter.Value;
                                        cmd.Parameters.Add(parm);
                                    }
                                }
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                        trans.Commit();
                        ret = true;
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        trans.Rollback();
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
            return ret;
        }


        /// <summary>
        ///  执行事务，并获取插入记录的lastInsertId
        /// </summary>
        /// <param name="sqlList"></param>
        /// <param name="paramList"></param>
        /// <param name="lastInsertId"></param>
        /// <param name="lastIdPosition">获取第lastIdPosition个Sql语句执行完后的lastInsertedId</param>
        public void ExecuteTransaction(List<string> sqlList, List<Parameters> paramList, out int lastInsertId, int lastIdPosition)
        {
            lastInsertId = -1;
            if (sqlList != null && sqlList.Count > 0 && paramList != null && paramList.Count > 0)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    MySqlCommand cmd = new MySqlCommand { Connection = conn };
                    MySqlTransaction trans = conn.BeginTransaction();
                    cmd.Transaction = trans;
                    try
                    {
                        int count = sqlList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (!string.IsNullOrEmpty(sqlList[i]))
                            {
                                cmd.CommandText = sqlList[i];

                                if (paramList[i] != null)
                                {
                                    foreach (Parameter parameter in paramList[i].Entries)
                                    {
                                        MySqlParameter parm = new MySqlParameter(parameter.Name, parameter.DBType, parameter.Size);
                                        parm.Value = parameter.Value;
                                        cmd.Parameters.Add(parm);
                                    }
                                }
                                cmd.ExecuteNonQuery();
                                if (lastIdPosition - 1 == i)
                                {
                                    lastInsertId = (int)cmd.LastInsertedId;
                                }
                                cmd.Parameters.Clear();
                            }
                        }
                        trans.Commit();
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        trans.Rollback();
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
        }

        /// <summary>
        ///  执行事务（附加执行带一个参数的委托）
        /// </summary>
        /// <param name="sqlList"></param>
        /// <param name="paramList"></param>
        /// <param name="action">外部带一个参数返回bool类型方法</param>
        /// <param name="info">action参数</param>
        public bool ExecuteTransaction<T>(List<string> sqlList, List<Parameters> paramList, Func<T, bool> action, T info)
        {
            bool isSuccess = false;
            if (sqlList != null && sqlList.Count > 0 && paramList != null && paramList.Count > 0)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    MySqlCommand cmd = new MySqlCommand { Connection = conn };
                    MySqlTransaction trans = conn.BeginTransaction();
                    cmd.Transaction = trans;
                    try
                    {
                        int count = sqlList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (!string.IsNullOrEmpty(sqlList[i]))
                            {
                                cmd.CommandText = sqlList[i];

                                if (paramList[i] != null)
                                {
                                    foreach (Parameter parameter in paramList[i].Entries)
                                    {
                                        var parm = new MySqlParameter(parameter.Name, parameter.DBType,
                                            parameter.Size) { Value = parameter.Value };
                                        cmd.Parameters.Add(parm);
                                    }
                                }
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                        if (!action(info))
                        {
                            trans.Rollback();
                        }
                        else
                        {
                            trans.Commit();
                            isSuccess = true;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        trans.Rollback();
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
            return isSuccess;
        }

        /// <summary>
        /// 执行事务（附加执行带两个参数的委托 Func<T1, T2, bool>）
        /// </summary>
        /// <typeparam name="T1">Func的参数1</typeparam>
        /// <typeparam name="T2">Func的参数2</typeparam>
        /// <param name="sqlList"></param>
        /// <param name="paramList"></param>
        /// <param name="action"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        public bool ExecuteTransaction<T1, T2>(List<string> sqlList, List<Parameters> paramList, Func<T1, T2, bool> action, T1 t1, T2 t2)
        {
            bool isSuccess = false;
            if (sqlList != null && sqlList.Count > 0 && paramList != null && paramList.Count > 0)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    MySqlCommand cmd = new MySqlCommand { Connection = conn };
                    MySqlTransaction trans = conn.BeginTransaction();
                    cmd.Transaction = trans;
                    try
                    {
                        int count = sqlList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (!string.IsNullOrEmpty(sqlList[i]))
                            {
                                cmd.CommandText = sqlList[i];

                                if (paramList[i] != null)
                                {
                                    foreach (Parameter parameter in paramList[i].Entries)
                                    {
                                        var parm = new MySqlParameter(parameter.Name, parameter.DBType,
                                            parameter.Size) { Value = parameter.Value };
                                        cmd.Parameters.Add(parm);
                                    }
                                }
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                        if (!action(t1,t2))
                        {
                            trans.Rollback();
                        }
                        else
                        {
                            trans.Commit();
                            isSuccess = true;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        trans.Rollback();
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
            return isSuccess;
        }


        /// <summary>
        ///  执行事务（附加执行带两个参数的委托Fun<T,int,bool>）(这个方法需要获取lastInsertedId)
        /// </summary>
        /// <param name="sqlList"></param>
        /// <param name="paramList"></param>
        /// <param name="action">外部带两个参数返回bool类型方法</param>
        /// <param name="info">action参数</param>
        /// <param name="lastIdPosition">获取第lastIdPosition个Sql语句执行完后的lastInsertedId</param>      
        public bool ExecuteTransaction<T>(List<string> sqlList, List<Parameters> paramList, Func<T, int, bool> action, T info, int lastIdPosition)
        {
            bool isSuccess = false;
            int lastInsertedId = -1;
            if (sqlList != null && sqlList.Count > 0 && paramList != null && paramList.Count > 0)
            {
                using (MySqlConnection conn = GetConnection())
                {
                    MySqlCommand cmd = new MySqlCommand { Connection = conn };
                    MySqlTransaction trans = conn.BeginTransaction();
                    cmd.Transaction = trans;
                    try
                    {
                        int count = sqlList.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (!string.IsNullOrEmpty(sqlList[i]))
                            {
                                cmd.CommandText = sqlList[i];

                                if (paramList[i] != null)
                                {
                                    foreach (Parameter parameter in paramList[i].Entries)
                                    {
                                        var parm = new MySqlParameter(parameter.Name, parameter.DBType,
                                            parameter.Size) { Value = parameter.Value };
                                        cmd.Parameters.Add(parm);
                                    }
                                }
                                cmd.ExecuteNonQuery();
                                if (lastIdPosition - 1 == i)
                                {
                                    lastInsertedId = (int)cmd.LastInsertedId;
                                }
                                cmd.Parameters.Clear();
                            }
                        }
                        if (lastInsertedId == -1 || !action(info, lastInsertedId))
                        {
                            trans.Rollback();
                        }
                        else
                        {
                            trans.Commit();
                            isSuccess = true;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException ex)
                    {
                        trans.Rollback();
                        throw new Exception(ex.Message);
                    }
                    finally
                    {
                        trans.Dispose();
                    }
                }
            }
            return isSuccess;
        }
        #endregion

        #region 辅助
        /// <summary>
        /// where条件构造器
        /// </summary>
        /// <param name="conditions">条件列表</param>
        /// <param name="type">关系组合类型</param>
        /// <returns>条件串</returns>
        public static string BuildSqlCondition(List<string> conditions, MysqlConditionRelationType type = MysqlConditionRelationType.AND)
        {
            string ret = string.Empty;

            if (conditions != null && conditions.Count > 0)
            {
                ret = string.Join(string.Format(" {0} ",type.ToString()), conditions);
            }

            return ret;
        }

        #endregion
    }
    /// <summary>
    /// where条件关系
    /// </summary>
    public enum MysqlConditionRelationType
    {
        AND,
        OR
    }
}
