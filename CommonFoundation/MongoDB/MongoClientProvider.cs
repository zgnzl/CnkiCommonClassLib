using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.MongoDB
{
    /// <summary>
    /// mongoclient 缓存
    /// </summary>
    public static class MongoClientProvider
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private static ConcurrentDictionary<string, Lazy<MongoClient>> m_mongoClientCache = new ConcurrentDictionary<string, Lazy<MongoClient>>();

        /// <summary>
        /// 获得Mongo客户端
        /// </summary>
        /// <param name="connStr">连接串</param>
        /// <returns></returns>
        public static MongoClient GetClient(string connStr)
        {
            if (string.IsNullOrWhiteSpace(connStr)) throw new Exception("MongoDB Connection String is Empty");

            return m_mongoClientCache.GetOrAdd(connStr,
                new Lazy<MongoClient>(() =>
                {
                    return new MongoClient(connStr);
                })).Value;
        }
    }
}
