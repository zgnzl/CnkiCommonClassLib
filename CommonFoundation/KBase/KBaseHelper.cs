using CommonFoundation.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPI;

namespace CommonFoundation.KBase
{
    public class KBaseHelper
    {
        #region 初始化
        /// <summary>
        /// Client
        /// </summary>
        private TPI.Client client;

        private string m_ftsServerIP;
        private int m_ftsServerPort;
        private string m_ftsServerUserName;
        private string m_ftsServerPwd;

        /// <summary>
        /// 初始化建立链接
        /// </summary>
        /// <param name="ftsServerIP"></param>
        /// <param name="ftsServerPort"></param>
        /// <param name="ftsServerUserName"></param>
        /// <param name="ftsServerPsw"></param>
        public KBaseHelper(string ftsServerIP, int ftsServerPort, string ftsServerUserName, string ftsServerPsw)
        {
            m_ftsServerIP = ftsServerIP;
            m_ftsServerPort = ftsServerPort;
            m_ftsServerUserName = ftsServerUserName;
            m_ftsServerPwd = ftsServerPsw;

            BuildConnection();
        }
        #endregion

        #region 基本方法
        /// <summary>
        /// 创建连接
        /// </summary>
        private void BuildConnection()
        {
            if (client == null)
            {
                client = new TPI.Client();
            }
            client.Connect(m_ftsServerIP, m_ftsServerPort, m_ftsServerUserName, m_ftsServerPwd);
        }
        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecSql(string sql)
        {
            int ret = -1;
            if (!string.IsNullOrEmpty(sql))
            {
                if (client == null || !client.IsActiveCon() || !client.IsConnected())
                {
                    BuildConnection();
                }

                ret = client.ExecSQL(sql);
            }
            return ret;
        }
        /// <summary>
        /// 得到结果集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public TPI.RecordSet GetRecord(string sql)
        {
            TPI.RecordSet rs = null;
            if (!string.IsNullOrEmpty(sql))
            {
                if (client == null || !client.IsActiveCon() || !client.IsConnected())
                {
                    BuildConnection();
                }

                rs = client.OpenRecordSet(sql);
            }
            return rs;
        }
        /// <summary>
        /// 得到记录条数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int GetRSCount(string sql)
        {
            int count = 0;
            TPI.RecordSet rs = GetRecord(sql);
            if (rs != null)
            {
                count = rs.GetCount();
                rs.Close();
            }
            return count;
        }
        /// <summary>
        /// 声明委托，从DataReader中读取相应的数据到实体对象中
        /// </summary>
        /// <typeparam name="T">实体对象类型</typeparam>
        /// <param name="dt">DataReader</param>
        /// <returns>加载数据的实体对象</returns>
        public delegate T RsToModel<T>(TPI.RecordSet rs);
        public List<T> PagerToList<T>(ref string handler, string sql, int pageIndex, int pageSize, PackingToObjectDelegate<T, TPI.RecordSet> pack, out int totalCount, bool isMarkRed)
        {
            List<T> _list = new List<T>();
            totalCount = 0;//返回总数

            //获得结果集
            TPI.RecordSet rs = null;
            if (!string.IsNullOrEmpty(handler))
            {
                rs = TPI.CommonFunc.DBClass.GetRecordSetFromQueryID(handler);
            }
            if ((rs == null || rs.GetCount() <= 0) && !string.IsNullOrEmpty(sql))
            {
                rs = GetRecord(sql);
                handler = rs == null ? string.Empty : rs.Handler.ToString();
            }

            //分页
            if (rs != null && rs.GetCount() > 0)
            {
                if (isMarkRed)
                {
                    rs.SetHitWordMarkFlag("###", "$$$");
                }

                totalCount = rs.GetCount();

                if (totalCount > 0)
                {
                    rs.MoveFirst();
                    int startIndex = GetRecordSetPageStartIndex(pageIndex, pageSize);
                    int endIndex = GetRecordSetPageEndIndex(totalCount, pageIndex, pageSize);
                    rs.Move(startIndex);
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        T model = pack(rs);
                        if (model != null)
                        {
                            _list.Add(model);
                        }
                        rs.MoveNext();
                    }
                }
                //rs.Close();
            }

            return _list;
        }
        /// <summary>
        /// 获取kabase结果集分页开始记录索引
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private static int GetRecordSetPageStartIndex(int pageIndex, int pageSize)
        {
            return Math.Max(pageIndex * pageSize, 0) + 1;
        }
        /// <summary>
        /// 获取kabase结果集分页结束记录索引
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private static int GetRecordSetPageEndIndex(int totalCount, int pageIndex, int pageSize)
        {
            int startIndex = GetRecordSetPageStartIndex(pageIndex, pageSize);
            return Math.Min(totalCount + 1, (startIndex + pageSize));
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConn()
        {
            if (client != null)
            {
                client.Close();
            }
        }
        /// <summary>
        /// 获取kbase表中的所有字段 
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public List<string> GetAllFieldName(string strSql)
        {
            List<string> fieldList = new List<string>();
            if (!string.IsNullOrEmpty(strSql))
            {
                TPI.RecordSet rs = GetRecord(strSql);
                if (rs != null && rs.GetCount() > 0)
                {
                    rs.MoveFirst();
                    while (!rs.IsEOF())
                    {
                        int cnt = rs.GetFieldCount();
                        for (int i = 0; i < cnt; i++)
                        {
                            string fieldName = rs.GetFieldName(i);
                            fieldList.Add(fieldName);
                        }
                        rs.MoveNext();
                    }
                }
                if (rs != null) rs.Close();
            }
            return fieldList;
        }
        #endregion

        #region 扩展方法

        #region 返回DataTable表格
        /// <summary>
        /// 返回DataTable表格
        /// </summary>
        /// <param name="rs">RecordSet</param>
        /// <returns>DataTable</returns>
        public DataTable RecordSetToDataTable(RecordSet rs)
        {
            int recordCount = 0;
            DataTable dt = new DataTable();
            int startNo = 0;
            rs.Move(startNo);
            recordCount = rs.GetCount();
            for (int j = 0; j < rs.GetFieldCount(); j++)
            {
                dt.Columns.Add(rs.GetFieldName(j));
            }
            for (int i = 0; i < recordCount; i++)
            {
                if (rs.IsEOF())
                    break;
                DataRow row = dt.NewRow();
                for (int j = 0; j < rs.GetFieldCount(); j++)
                {
                    string s = rs.GetValue(j);
                    if (s == null)
                        s = "";
                    row[j] = s;
                }
                dt.Rows.Add(row);
                rs.MoveNext();
            }
            rs.Close();
            return dt;
        }
        #endregion
       
        #region GetKeyToOrFormatFromRS从rs中取key字段，组合成sql的+格式
        public static string SqlKeyValueDefault = "key";
        /// <summary>
        /// 从rs中取key字段，组合成sql的+格式，使用RSHelper.SqlKeyValueDefault作为sql key名
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public  string GetKeyToOrFormatFromRS(string sql)
        {
            return GetKeyToOrFormatFromRS(sql, SqlKeyValueDefault);
        }
        /// <summary>
        /// 从rs中取key字段，组合成sql的+格式
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="keyField">key字段名</param>
        /// <param name="useUnicode">是否unicode编码</param>
        /// <returns></returns>
        public  string GetKeyToOrFormatFromRS(string sql, string keyField)
        {
            string ret = string.Empty;
            if (!string.IsNullOrEmpty(sql) && !string.IsNullOrEmpty(keyField))
            {
                TPI.RecordSet rs = GetRecord(sql);//此处调用方法

                if (rs != null)
                {
                    List<string> list = new List<string>();
                    string key = string.Empty;
                    rs.MoveFirst();
                    while (!rs.IsEOF())
                    {
                        key = rs.GetValue(keyField);
                        if (!string.IsNullOrEmpty(key) && !list.Contains(key))
                        {
                            list.Add(string.Format("'{0}'", key));
                        }
                        rs.MoveNext();
                    }
                    rs.Close();
                    ret = string.Join("+", list);
                }
            }
            return ret;
        }
        #endregion

        #region 构造多值查询
        /// <summary>
        /// 将多值字段转换为或查询形式
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ConvertMutiValToOrFormat(string str)
        {
            string ret = string.Empty;
            if (!string.IsNullOrEmpty(str))
            {
                ret = ConvertMutiValToOrFormat(CommonFunc.SplitString(str));//;分号分割的字符串
            }
            return ret;
        }
        /// <summary>
        /// 将数组转换为或查询形式
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static string ConvertMutiValToOrFormat(string[] strs)
        {
            string ret = string.Empty;
            if (strs != null && strs.Length > 0)
            {
                List<string> list = new List<string>();
                foreach (string str in strs)
                {
                    if (!list.Contains(str))
                    {
                        list.Add(str);
                    }
                }
                if (list.Count > 0)
                {
                    ret = string.Join("+", list);
                }
            }
            return ret;
        }
        /// <summary>
        /// 将数组转换为或查询形式
        /// </summary>
        /// <param name="strs"></param>
        /// <returns></returns>
        public static string ConvertMutiValToOrFormat(List<string> strs)
        {
            string ret = string.Empty;
            if (strs != null && strs.Count > 0)
            {
                List<string> list = new List<string>();
                foreach (string str in strs)
                {
                    if (!list.Contains(str))
                    {
                        list.Add(str);
                    }
                }
                if (list.Count > 0)
                {
                    ret = string.Join("+", list);
                }
            }
            return ret;
        }
        #endregion

        #region 获取单值
        /// <summary>
        /// 查询获取单一值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="keyField"></param>
        /// <param name="useUnicode"></param>
        /// <returns></returns>
        public  string GetSingleValFormRS(string sql, string keyField)
        {
            string ret = string.Empty;
            if (!string.IsNullOrEmpty(sql) && !string.IsNullOrEmpty(keyField))
            {
                TPI.RecordSet rs = GetRecord(sql);//此处调用方法

                if (rs != null)
                {
                    rs.MoveFirst();
                    ret = rs.GetValue(keyField);
                    rs.Close();
                }
            }
            return ret;
        }
        #endregion

        #region 获取单值列表
        /// <summary>
        /// 查询获取单一值列表
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="keyField"></param>
        /// <param name="useUnicode"></param>
        /// <returns></returns>
        public  List<string> GetSingleValListFormRS(string sql, string keyField)
        {
            List<string> list = null;
            if (!string.IsNullOrEmpty(sql) && !string.IsNullOrEmpty(keyField))
            {
                TPI.RecordSet rs = GetRecord(sql);//此处调用方法
                if (rs != null)
                {
                    list = new List<string>();
                    rs.MoveFirst();
                    while (!rs.IsEOF())
                    {
                        string key = rs.GetValue(keyField);
                        if (!string.IsNullOrEmpty(key) && !list.Contains(key))
                        {
                            list.Add(key);
                        }
                        rs.MoveNext();
                    }
                    rs.Close();
                }
            }
            return list;
        }
        #endregion

        #region 更新大字段
        /// <summary>
        /// 大字段更新
        /// </summary>
        public void Update()
        {
            string ip = "127.0.0.1";
            int port = 4567;
            string userName = "DBOWN";
            string psd = "";
            KBaseHelper kBaseHelper = new KBaseHelper(ip, port, userName, psd);//连接kbase数据库

            TPI.RecordSet rs = kBaseHelper.GetRecord("SELECT * FROM EXPERT_BASEINFO");//获取所有的需要更新的记录
            string[] fields = { "论文", "研究方向", "基本信息", "履历", "著作", "科研项目", "社会活动", "所获荣誉", "成果荣誉综述", "个人综合报道", "联系信息" };//需要更新的字段

            if (rs != null)
            {
                int icnt = rs.GetCount();
                if (icnt > 0)
                {
                    int iseek = 0;
                    rs.MoveFirst();
                    while (!rs.IsEOF())
                    {
                        iseek++;
                        rs.Edit();//编辑
                        string id = rs.GetValue("专家编号");//当前rs所指向的一条记录
                        foreach (string field in fields)
                        {
                            rs.SetValue(field, "field字段对应的值");
                        }
                        rs.Update();//更新
                        //Application.DoEvents();//程序集 System.Windows.Forms.dll, v4.0.0.0
                        rs.MoveNext();
                    }
                }
            }
        }
        #endregion
       

        #endregion

    }
}

