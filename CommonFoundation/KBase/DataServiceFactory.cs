using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CommonFoundation
{
    public static class DataServiceFactory
    {
        /// <summary>
        /// 创建数据服务
        /// 统一调用为后续扩展superlink
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDataService<string, TPI.RecordSet> CreateKbaseDataService()
        {
            return new Kbase.RSDataService<string>();
        }
        /// <summary>
        /// 创建数据服务
        /// 统一调用为后续扩展superlink
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IDataService<T, TPI.RecordSet> CreateKbaseDataService<T>()
        {
            return new Kbase.RSDataService<T>();
        }
    }
}