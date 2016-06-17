using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonFoundation.Kbase
{
    /// <summary>
    /// RecordSet数据服务
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RSDataService<T> : IDataService<T, TPI.RecordSet>
    {
        #region 第一行第一列
        /// <summary>
        /// 获得第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string GetScalar(string sql)
        {
            string ret = string.Empty;

            if (!string.IsNullOrEmpty(sql))
            {
                //获得结果集
                TPI.RecordSet rs = TPI.CommonFunc.DBClass.OpenRecordSet(sql);

                if (rs != null)
                {
                    rs.MoveFirst();
                    ret = rs.GetCount() > 0 ? rs.GetValue(0) : string.Empty;
                    rs.Close();
                }
            }

            return ret;
        }
        #endregion

        #region 全部封装
        /// <summary>
        /// 单个对象封装
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public T SingleToObj(string sql, PackingToObjectDelegate<T, TPI.RecordSet> pack)
        {
            return SingleToObj(sql, pack, null, null);
        }
        /// <summary>
        /// 单个对象封装
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pack"></param>
        /// <param name="isPack"></param>
        /// <param name="excepts"></param>
        /// <returns></returns>
        public T SingleToObj(string sql, PackingToObjectDelegate<T, TPI.RecordSet> pack, IsPackingToObjectDelegate<TPI.RecordSet> isPack, string[] excepts)
        {
            T _model = default(T);
            //获得结果集
            TPI.RecordSet rs = null;
            if (!string.IsNullOrEmpty(sql))
            {
                rs = TPI.CommonFunc.DBClass.OpenRecordSet(sql);
            }

            //包装
            if (rs != null)
            {
                if (rs.GetCount() > 0)
                {
                    rs.MoveFirst();
                    if (isPack == null || excepts == null || isPack(rs, excepts))
                    {
                        _model = pack(rs);
                    }
                }
                rs.Close();
            }
            return _model;
        }
        #endregion

        #region 全部封装
        /// <summary>
        /// 全部封装
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public List<T> AllToList(string sql, PackingToObjectDelegate<T, TPI.RecordSet> pack)
        {
            return AllToList(sql, pack, null, null);
        }
        /// <summary>
        /// 全部封装
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pack"></param>
        /// <param name="isPack"></param>
        /// <param name="excepts"></param>
        /// <returns></returns>
        public List<T> AllToList(string sql, PackingToObjectDelegate<T, TPI.RecordSet> pack, IsPackingToObjectDelegate<TPI.RecordSet> isPack, string[] excepts)
        {
            List<T> _list = new List<T>();

            //获得结果集
            TPI.RecordSet rs = null;
            if (!string.IsNullOrEmpty(sql))
            {
                rs = TPI.CommonFunc.DBClass.OpenRecordSet(sql);
            }

            //包装
            if (rs != null)
            {
                if (rs.GetCount() > 0)
                {
                    rs.MoveFirst();
                    while (!rs.IsEOF())
                    {
                        if (isPack == null || excepts == null || isPack(rs, excepts))
                        {
                            T model = pack(rs);
                            if (model != null)
                            {
                                _list.Add(model);
                            }
                        }
                        rs.MoveNext();
                    }
                }
                rs.Close();
            }

            return _list;
        }
        #endregion

        #region 分页封装
        /// <summary>
        /// 分页封装
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pack"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public List<T> PagerToList(ref string handler, string sql, int pageIndex, int pageSize, PackingToObjectDelegate<T, TPI.RecordSet> pack, out int totalCount)
        {
            return PagerToList(ref handler, sql, pageIndex, pageSize, pack, out totalCount, false);
        }
        /// <summary>
        /// 分页封装
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="sql"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="pack"></param>
        /// <param name="totalCount"></param>
        /// <param name="isMarkRed"></param>
        /// <returns></returns>
        public List<T> PagerToList(ref string handler, string sql, int pageIndex, int pageSize, PackingToObjectDelegate<T, TPI.RecordSet> pack, out int totalCount, bool isMarkRed)
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
                rs = TPI.CommonFunc.DBClass.OpenRecordSet(sql);
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
        #endregion

        #region 执行sql
        /// <summary>
        /// 执行sql，插入或更新
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int ExecSQL(string sql)
        {
            int ret = 0;

            if (!string.IsNullOrEmpty(sql))
            {
                TPIManager.CTpiGlobals conn = TPI.CommonFunc.DBClass.BuildConnect();
                ret = conn.GetClient().ExecSQL(sql);
            }

            return ret;
        }
        #endregion

        #region GetCount
        /// <summary>
        /// GetCount
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int GetCount(string sql)
        {
            int ret = 0;

            if (!string.IsNullOrEmpty(sql))
            {
                //获得结果集
                TPI.RecordSet rs = TPI.CommonFunc.DBClass.OpenRecordSet(sql);

                if (rs != null)
                {
                    ret = rs.GetCount();
                    rs.Close();
                }
            }

            return ret;
        }
        #endregion

        #region 索引计算
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
        #endregion
    }
}