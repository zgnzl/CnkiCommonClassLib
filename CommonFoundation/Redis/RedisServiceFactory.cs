using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Redis
{
    public static class RedisServiceFactory
    {
        /// <summary>
        /// Redis缓存连接
        /// </summary>
        private static string m_cache_redisConnection = ConfigurationManager.AppSettings["RedisCacheUrl"]!=null?
            ConfigurationManager.AppSettings["RedisCacheUrl"].ToString():null;
        /// <summary>
        /// Redis日志连接
        /// </summary>
        private static string m_log_redisConnection = ConfigurationManager.AppSettings["RedisLogUrl"] !=null?
            ConfigurationManager.AppSettings["RedisLogUrl"].ToString():null;

        /// <summary>
        /// Redis缓存数据库序号
        /// </summary>
        private static readonly int m_cache_databaseSerial = 11;
        /// <summary>
        /// Redis日志数据库序号
        /// </summary>
        private static readonly int m_log_databaseSerial = 13;

        /// <summary>
        /// Redis缓存服务，连接字符串“RedisCacheUrl”
        /// </summary>
        /// <returns></returns>
        public static RedisService CreateRedisCacheService()
        {
            return new RedisService(m_cache_redisConnection, key => string.Format("Cache:{0}", key), m_cache_databaseSerial);
        }
        /// <summary>
        /// Redis缓存服务，连接字符串“RedisCacheUrl”
        /// </summary>
        /// <param name="func">名称构造方法Cache:{func}:key</param>
        /// <returns></returns>
        public static RedisService CreateRedisCacheService(Func<string,string> func)
        {
            return new RedisService(m_cache_redisConnection, key => string.Format("Cache:{0}", func(key)), m_cache_databaseSerial);
        }

        #region Log
        /// <summary>
        /// Redis日志服务，连接字符串“RedisLogUrl”
        /// </summary>
        /// <returns></returns>
        public static RedisService CreateRedisLogService()
        {
            return new RedisService(m_log_redisConnection, key => string.Format("Log:{0}", key), m_log_databaseSerial);
        }

        /// <summary>
        /// Redis日志服务
        /// </summary>
        /// <returns></returns>
        public static RedisService CreateRedisLogService(string redisConnection, int databaseSerial)
        {
            return new RedisService(redisConnection, null, databaseSerial);
        }

        /// <summary>
        /// Redis日志服务，连接字符串“RedisLogUrl”
        /// </summary>
        /// <param name="func">名称构造方法Cache:{func}:key</param>
        /// <returns></returns>
        public static RedisService CreateRedisLogService(Func<string, string> func)
        {
            return new RedisService(m_log_redisConnection, key => string.Format("Log:{0}", func(key)), m_log_databaseSerial);
        }

        /// <summary>
        /// Redis日志服务，连接字符串“RedisLogUrl”
        /// </summary>
        /// <param name="redisConnection"></param>
        /// <param name="func"></param>
        /// <param name="databaseSerial"></param>
        /// <returns></returns>
        public static RedisService CreateRedisLogService(string redisConnection, Func<string, string> func, int databaseSerial)
        {
            return new RedisService(redisConnection, key => string.Format("Log:{0}", func(key)), databaseSerial);
        }

        /// <summary>
        /// Redis服务
        /// </summary>
        /// <param name="redisConnStr"></param>
        /// <param name="func"></param>
        /// <param name="databaseSerial"></param>
        /// <returns></returns>
        public static RedisService CreateRedisOtherService(string redisConnStr, Func<string, string> func = null, int databaseSerial = 0)
        {
            return new RedisService(m_log_redisConnection, key => string.Format("Other:{0}", func(key)), m_log_databaseSerial);
        }
        #endregion
    }
}
