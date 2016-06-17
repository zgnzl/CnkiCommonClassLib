using CommonFoundation.Log.Recorder;
using CommonFoundation.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CommonFoundation.Log
{
    public static class LogServiceFactory
    {
        /// <summary>
        /// 默认日志服务（本地文件）
        /// </summary>
        /// <returns></returns>
        public static ILogRecorder CreateLogService()
        {
            return new FileLogRecorder();
        }

        /// <summary>
        /// 文件日志服务
        /// </summary>
        /// <param name="logname"></param>
        /// <returns></returns>
        public static ILogRecorder CreateFileLogService(string logname="")
        {
            return new FileLogRecorder(logname);
        }

        /// <summary>
        /// 错误日志记录
        /// </summary>
        /// <returns></returns>
        public static ILogRecorder CreateRedisErrorLogService()
        {
            RedisService service = RedisServiceFactory.CreateRedisLogService(key => string.Format("Error:{0}", key));
            return new RedisLogRecorder(service, "ErrorLog");
        }

        /// <summary>
        /// Redis记录日志服务
        /// </summary>
        /// <param name="logname"></param>
        /// <returns></returns>
        public static ILogRecorder CreateRedisLogService(string redisConnection, Func<string, string> func, int databaseSerial, string logname = "")
        {
            RedisService service = RedisServiceFactory.CreateRedisLogService(redisConnection, func, databaseSerial);
            return new RedisLogRecorder(service, logname);
        }

        
    }

    
}
