
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Redis
{
    public static class RedisClientProvider
    {
        /// <summary>
        /// Redis连接缓存
        /// </summary>
        private static ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>> m_connectionMultiplexers = new ConcurrentDictionary<string, Lazy<ConnectionMultiplexer>>();

        /// <summary>
        /// 获得Redis连接
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static ConnectionMultiplexer GetConnection(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new Exception("Redis Connection String is Empty");

            // when using ConcurrentDictionary, multiple threads can create the value
            // at the same time, so we need to pass a Lazy so that it's only 
            // the object which is added that will create a ConnectionMultiplexer,
            // even when a delegate is passed
            return m_connectionMultiplexers.GetOrAdd(connectionString,
                new Lazy<ConnectionMultiplexer>(() =>
                {
                    return ConnectionMultiplexer.Connect(connectionString);
                })).Value;
        }

    }
}
