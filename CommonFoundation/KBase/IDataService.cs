using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation
{
    /// <summary>
    /// 代理，封装对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="set"></param>
    /// <returns></returns>
    public delegate T PackingToObjectDelegate<T, R>(R set);
    /// <summary>
    /// 代理，是否封装这条记录
    /// </summary>
    /// <param name="set"></param>
    /// <param name="excepts"></param>
    /// <returns></returns>
    public delegate bool IsPackingToObjectDelegate<R>(R set, string[] excepts);

    /// <summary>
    /// 数据层接口，负责获得数据并封装对象，
    /// 服务层不直接饮用数据，通过对象操作，便于数据源的切换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    public interface IDataService<T,R>
    {
        /// <summary>
        /// 获得第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string GetScalar(string sql);
        /// <summary>
        /// 封装单个对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        T SingleToObj(string sql, PackingToObjectDelegate<T, R> pack);
        /// <summary>
        /// 封装单个对象，判断是否封装
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pack"></param>
        /// <param name="isPack"></param>
        /// <param name="excepts"></param>
        /// <returns></returns>
        T SingleToObj(string sql, PackingToObjectDelegate<T, R> pack, IsPackingToObjectDelegate<R> isPack, string[] excepts);

        /// <summary>
        /// 封装所有对象
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        List<T> AllToList(string sql, PackingToObjectDelegate<T, R> pack);
        /// <summary>
        /// 封装所有对象，判断是否封装
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pack"></param>
        /// <param name="isPack"></param>
        /// <param name="excepts"></param>
        /// <returns></returns>
        List<T> AllToList(string sql, PackingToObjectDelegate<T, R> pack, IsPackingToObjectDelegate<R> isPack, string[] excepts);
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
        List<T> PagerToList(ref string handler, string sql, int pageIndex, int pageSize, PackingToObjectDelegate<T, R> pack, out int totalCount);
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
        List<T> PagerToList(ref string handler, string sql, int pageIndex, int pageSize, PackingToObjectDelegate<T, R> pack, out int totalCount, bool isMarkRed);
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int ExecSQL(string sql);
        /// <summary>
        /// GetCount
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        int GetCount(string sql);
    }
}
