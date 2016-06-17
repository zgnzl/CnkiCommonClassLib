using CommonFoundation.Log;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.KBase
{
    public class KBaseManager
    {
        #region 私有变量
        /// <summary>
        /// Client
        /// </summary>
        private TPI.Client m_client;
        /// <summary>
        /// 连接信息
        /// </summary>
        private KConnInfo m_connInfo;
        /// <summary>
        /// debug日志
        /// </summary>
        private LogManager m_logManager;
        #endregion

        #region 属性
        /// <summary>
        /// 是否debug，输出sql日志
        /// </summary>
        public bool IsDebug { get; set; }
        #endregion

        #region 构造函数，析构函数
        /// <summary>
        /// 初始化建立链接
        /// </summary>
        /// <param name="ftsServerIP"></param>
        /// <param name="ftsServerPort"></param>
        /// <param name="ftsServerUserName"></param>
        /// <param name="ftsServerPsw"></param>
        public KBaseManager(string ftsServerIP, int ftsServerPort, string ftsServerUserName, string ftsServerPsw)
        {
            Initialize(new KConnInfo()
            {
                ServerIP = ftsServerIP,
                ServerPort = ftsServerPort,
                UserName = ftsServerUserName,
                Password = ftsServerPsw
            });
        }
        /// <summary>
        /// 初始化建立链接
        /// </summary>
        /// <param name="info"></param>
        public KBaseManager(KConnInfo info)
        {
            if (info == null)
            {
                info = new KConnInfo();
            }
            Initialize(info);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="info"></param>
        private void Initialize(KConnInfo info)
        {
            m_connInfo = info;
            IsDebug = false;
            m_logManager = new LogManager("KbaseManagerTraceLog");

            DebugTrace(string.Format("KbaseManager Initialize:{0}", info.ServerIP));
        }
        #endregion

        #region 连接操作
        /// <summary>
        /// 创建连接,初始化默认创建连接
        /// </summary>
        public void BuildConnection()
        {
            if (m_client == null || !m_client.IsActiveCon() || !m_client.IsConnected())
            {
                m_client = new TPI.Client();
                if (m_connInfo != null)
                {
                    bool ret = m_client.Connect(m_connInfo.ServerIP, m_connInfo.ServerPort, m_connInfo.UserName, m_connInfo.Password);

                    DebugTrace(string.Format("BuildConnection:{0},return:{1}", m_connInfo.ServerIP, ret.ToString()));
                }
            }
        }

        /// <summary>
        /// 检查连接
        /// </summary>
        /// <returns></returns>
        public bool CheckConnection()
        {
            BuildConnection();
            return (m_client != null && m_client.IsActiveCon() && m_client.IsConnected());
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnection()
        {
            if (m_client != null)
            {
                bool ret = m_client.Close();

                DebugTrace(string.Format("CloseConnection:{0},return:{1}", m_connInfo.ServerIP, ret.ToString()));
            }
        }
        #endregion

        #region 基本操作
        /// <summary>
        /// 执行SQL，等待执行结果
        /// </summary>
        /// <param name="sql">待执行的sql语句</param>
        /// <returns></returns>
        public KReturnInfo ExecSql(string sql)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(sql))
            {
                BuildConnection();
                int val = m_client.ExecSQL(sql);

                ret.ReturnValue = val;
                ret.SQL = sql;
                if (val < 0) ret.Msg = m_client.GetErrorMsg(val);

                DebugTrace(string.Format("Server:{0},ExecSql:{1},Return:{2}", m_connInfo.ServerIP, sql, val));
            }
            else
            {
                ret.Msg = "Excute Sql is Empty";

                DebugTrace(string.Format("Server:{0},Msg:{1}", m_connInfo.ServerIP, ret.Msg));
            }
            return ret;
        }

        /// <summary>
        /// 执行SQL，抛任务，等待执行结果
        /// </summary>
        /// <param name="sql">待执行的sql语句</param>
        /// <returns></returns>
        public KReturnInfo ExecMgrSql(string sql)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(sql))
            {
                BuildConnection();
                int val = m_client.ExecMgrSQL(sql);

                ret.ReturnValue = val;
                ret.SQL = sql;
                if (val < 0) ret.Msg = m_client.GetErrorMsg(val);

                DebugTrace(string.Format("Server:{0},ExecMgrSQL:{1},Return:{2}", m_connInfo.ServerIP, sql, val));
            }
            else
            {
                ret.Msg = "ExecMgr Sql is Empty";

                DebugTrace(string.Format("Server:{0},Msg:{1}", m_connInfo.ServerIP, ret.Msg));
            }
            return ret;
        }
        /// <summary>
        /// 执行SQL，抛任务，立即返回结果
        /// </summary>
        /// <param name="sql">待执行的sql语句</param>
        /// <returns></returns>
        public KReturnInfo ExecMgrSqlAsyn(string sql)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(sql))
            {
                BuildConnection();
                int val = m_client.ExecMgrSQL(sql,true);

                ret.ReturnValue = val;
                ret.SQL = sql;
                if (val < 0) ret.Msg = m_client.GetErrorMsg(val);

                DebugTrace(string.Format("Server:{0},ExecMgrSqlAsyn:{1},Return:{2}", m_connInfo.ServerIP, sql, val));
            }
            else
            {
                ret.Msg = "ExecMgrAsyn Sql is Empty";

                DebugTrace(string.Format("Server:{0},Msg:{1}", m_connInfo.ServerIP, ret.Msg));
            }
            return ret;
        }

        /// <summary>
        /// 执行一组sql,抛任务，立即返回结果
        /// </summary>
        /// <param name="sqlList">待执行的sql语句列表</param>
        /// <returns></returns>
        public KReturnInfo ExecMgrSqlListAsyn(List<string> sqlList)
        {
            KReturnInfo ret = new KReturnInfo();
            ret.ReturnValue = 0;
            ret.Msg = "ExecMgrSqlListAsyn";

            if (sqlList != null && sqlList.Count > 0)
            {
                ret.SQL = string.Join(";", sqlList);

                foreach (string sql in sqlList)
                {
                    KReturnInfo val = ExecMgrSqlAsyn(sql);
                    if (!val.IsSuccess)
                    {
                        ret.ReturnValue = val.ReturnValue;
                        ret.Msg = string.Format("Server:{0},ExecMgrSqlListAsyn,Error SQL:{1},Return:{2}", m_connInfo.ServerIP, val.SQL, val.ReturnValue);
                        break;
                    }
                }
             
            }

            return ret;
        }
        #endregion

        #region 数据查询
        /// <summary>
        /// 得到结果集
        /// </summary>
        /// <param name="sql">待执行sql语句</param>
        /// <returns></returns>
        public TPI.RecordSet GetRecord(string sql)
        {
            TPI.RecordSet rs = null;
            if (!string.IsNullOrEmpty(sql))
            {
                BuildConnection();
                rs = m_client.OpenRecordSet(sql);

                DebugTrace(string.Format("Server:{0},ExecSql:{1},RecoudSet:{2}", m_connInfo.ServerIP, sql, rs == null ? "null" : rs.GetCount().ToString()));
            }
            return rs;
        }
        /// <summary>
        /// 得到结果集数量
        /// </summary>
        /// <param name="sql">待执行sql语句</param>
        /// <returns></returns>
        public int GetCount(string sql)
        {
            int ret = 0;
            TPI.RecordSet rs = GetRecord(sql);
            if (rs != null)
            {
                ret = rs.GetCount();
                rs.Close();
            }
            return ret;
        }
        /// <summary>
        /// 数据库是否已经创建
        /// </summary>
        /// <param name="dbName">库名</param>
        /// <returns></returns>
        public bool IsExistDB(string dbName)
        {
            bool ret = false;

            if (!string.IsNullOrWhiteSpace(dbName))
            {
                string sql = string.Format("SELECT DBASE FROM SYS_DB WHERE DBASE = '{0}'", dbName);
                int count = GetCount(sql);
                ret = count > 0;
            }

            return ret;
        }
        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public bool IsExistTable(string tableName)
        {
            bool ret = false;

            if (!string.IsNullOrEmpty(tableName))
            {
                string sql = string.Format("SELECT TABLENAME FROM SYS_HOTSTAR_SYSTEM WHERE TABLENAME = '{0}' ", tableName);
                int count = GetCount(sql);
                ret = count > 0;
            }
            return ret;

        }
        /// <summary>
        /// 获得某一db下的所有表信息
        /// </summary>
        /// <param name="dbName">库名</param>
        /// <returns></returns>
        public TPI.RecordSet GetTableAllInDB(string dbName)
        {
            TPI.RecordSet rs = null;
            if (!string.IsNullOrWhiteSpace(dbName))
            {
                string sql = string.Format("SELECT * FROM SYS_HOTSTAR_SYSTEM WHERE DBASE = '{0}'", dbName);
                rs = GetRecord(sql);
            }
            return rs;
        }
        /// <summary>
        /// 获得服务器所有表信息
        /// </summary>
        /// <returns></returns>
        public TPI.RecordSet GetTableAll()
        {
            string sql = "SELECT * FROM SYS_HOTSTAR_SYSTEM WHERE ";
            return GetRecord(sql);
        }
        #endregion

        #region 数据定义
        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbName">数据库名称</param>
        /// <returns></returns>
        public KReturnInfo CreateDataBase(string dbName)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(dbName))
            {
                if (IsExistDB(dbName))
                {
                    ret.ReturnValue = 0;
                    ret.Msg = string.Format("Database {0} is Exist", dbName);

                    DebugTrace(string.Format("Server:{0},Msg:{1}", m_connInfo.ServerIP, ret.Msg));
                }
                else
                {
                    string sql = string.Format("CREATE DATABASE {0}", dbName);
                    ret = ExecSql(sql);
                }
            }
            else
            {
                ret.Msg = "CreateDataBase param is null ";
            }
            return ret;
        }
        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public KReturnInfo DropTable(string tableName)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                string sql = string.Format("DROP TABLE {0} ", tableName);
                ret = ExecSql(sql);
            }
            else
            {
                ret.Msg = "DropTable param is null ";
            }
            return ret;
        }
        /// <summary>
        /// 删除视图
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <returns></returns>
        public KReturnInfo DropView(string viewName)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(viewName))
            {
                string sql = string.Format("DROP VIEW {0} ", viewName);
                ret = ExecSql(sql);
            }
            else
            {
                ret.Msg = "DropView param is null ";
            }
            return ret;
        }
        /// <summary>
        /// 建索引,同步
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public KReturnInfo CreateIndex(string tableName)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                string sql = string.Format("INDEX {0} all", tableName);
                ret = ExecSql(sql);
            }
            else
            {
                ret.Msg = "CreateIndex param is null ";
            }
            return ret;
        }
        /// <summary>
        /// 建立索引，异步，直接返回
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public KReturnInfo CreateIndexAsyn(string tableName)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                //建立索引
                string sql = string.Format("INDEX {0} all", tableName);
                ret = ExecMgrSqlAsyn(sql);
            }
            else
            {
                ret.Msg = "CreateIndexAsyn param is null ";
            }
            return ret;
        }
        /// <summary>
        /// 并表操作
        /// </summary>
        /// <param name="server">KBase服务器连接信息</param>
        /// <param name="tableName">表名</param>
        /// <param name="dbname">库名</param>
        /// <returns></returns>
        public KReturnInfo CombineTableToDB(KConnInfo server,string path, string tableName, string dbname)
        {
            KReturnInfo ret = new KReturnInfo();
            if (server != null && !string.IsNullOrWhiteSpace(path) && !string.IsNullOrWhiteSpace(tableName) && !string.IsNullOrWhiteSpace(dbname))
            {
                string sql = string.Format("CONNECT TELE TABLE {0}  PATH  '{1}'  '{2}'  {3} {4}  '{5}' at {6}",
                    tableName, path, server.ServerIP, server.ServerPort, server.UserName, server.Password, dbname);
                ret = ExecSql(sql);
            }
            else
            {
                ret.Msg = "CombineTableToDB param is error ";
            }

            return ret;
        }
        /// <summary>
        /// 同步排序
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public KReturnInfo UpdateSort(string tablename)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(tablename))
            {
                string sql = string.Format("DBUM UPDATE SORTFILE OF TABLE '{0}' ", tablename);
                ret = ExecSql(sql);
            }
            else
            {
                ret.Msg = "UpdateSort param is null ";
            }
            return ret;
        }
        /// <summary>
        /// 重整表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public KReturnInfo PackTable(string tableName)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                string sql = string.Format("PACK TABLE {0}", tableName);
                ret = ExecMgrSql(sql);
            }
            else
            {
                ret.Msg = "PackTable param is null ";
            }
            return ret;
        }
        #endregion

        #region 数据操作
        /// <summary>
        /// 删除表中一条数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldValue">字段值</param>
        /// <returns></returns>
        public KReturnInfo DeleteData(string tableName, string fieldName, string fieldValue)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(fieldValue))
            {
                string sql = string.Format("DELETE FROM {0} WHERE {1}='{2}' ", tableName, fieldName, fieldValue);
                ret = ExecSql(sql);
            }
            else
            {
                ret.Msg = "DeleteData param is error ";
            }
            return ret;
        }
        /// <summary>
        /// 删除表中多条数据
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldName"></param>
        /// <param name="fieldValues"></param>
        /// <returns></returns>
        public KReturnInfo DeleteDataMany(string tableName, string fieldName, string fieldValues)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(fieldName) && !string.IsNullOrEmpty(fieldValues))
            {
                string sql = string.Format("DELETE FROM {0} WHERE {1}={2} ", tableName, fieldName, fieldValues);
                ret = ExecSql(sql);
            }
            else
            {
                ret.Msg = "DeleteData param is error ";
            }
            return ret;
        }
        /// <summary>
        /// 清空表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public KReturnInfo ClearData(string tableName)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrWhiteSpace(tableName))
            {
                string sql = string.Format("CLEAR TABLE {0}", tableName);
                ret = ExecSql(sql);
            }
            else
            {
                ret.Msg = "ClearData param is null ";
            }
            return ret;
        }
        /// <summary>
        /// 使用Rec文件更新表
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="path">Rec文件存储全路径</param>
        /// <returns></returns>
        public KReturnInfo BulkLoadRecToTable(string tableName, string path)
        {
            KReturnInfo ret = new KReturnInfo();
            if (!string.IsNullOrEmpty(tableName) && !string.IsNullOrEmpty(path))
            {
                //批量写数据
                string sql = string.Format("BULKLOAD TABLE {0} '{1}'", tableName, path);
                ret = ExecMgrSql(sql);
            }
            else
            {
                ret.Msg = "BulkLoadRecToTable param is error ";
            }
            return ret;
        }
        #endregion

        #region 日志
        /// <summary>
        /// 记录输出日志
        /// </summary>
        /// <param name="?"></param>
        private void DebugTrace(string msg)
        {
            if (IsDebug)
            {
                m_logManager.Print(msg);
            }
        }
        #endregion
    }

    /// <summary>
    /// Kbase数据库连接信息类
    /// </summary>
    public class KConnInfo
    {
        /// <summary>
        /// 数据库IP地址
        /// </summary>
        public string ServerIP { get; set; }
        /// <summary>
        /// 数据库连接端口
        /// </summary>
        public int ServerPort { get; set; }
        /// <summary>
        /// 连接用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 连接密码
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// Kbase返回结果
    /// </summary>
    public class KReturnInfo
    {
        /// <summary>
        /// 执行sql
        /// </summary>
        public string SQL { get; set; }
        /// <summary>
        /// 执行返回值
        /// </summary>
        public int ReturnValue { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get { return ReturnValue >= 0; } }

        /// <summary>
        /// 构造函数
        /// </summary>
        public KReturnInfo()
        {
            SQL = string.Empty;
            ReturnValue = -1;
            Msg = string.Empty;
        }
    }
}
