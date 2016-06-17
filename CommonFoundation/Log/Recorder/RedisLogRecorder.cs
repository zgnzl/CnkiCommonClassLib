using CommonFoundation.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace CommonFoundation.Log.Recorder
{
    public class RedisLogRecorder:ILogRecorder
    {
        /// <summary>
        /// Redis连接
        /// </summary>
        private RedisService m_redisService = null;
        /// <summary>
        /// 日志名称
        /// </summary>
        private string m_logName = string.Empty;

        /// <summary>
        /// 日志名称
        /// </summary>
        public string LogName
        {
            get { return m_logName; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public RedisLogRecorder(string redisConnection, Func<string, string> func, int databaseSerial,string logName)
        {
            m_redisService = RedisServiceFactory.CreateRedisLogService(redisConnection, func, databaseSerial);
            m_logName = logName;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="redisService"></param>
        public RedisLogRecorder(RedisService redisService,string logName)
        {
            m_redisService = redisService;
            m_logName = logName;
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="msg"></param>
        public void RecordMsg(string msg)
        {
            if (string.IsNullOrWhiteSpace(m_logName)) return;
            if (string.IsNullOrWhiteSpace(msg)) return;
            if(m_redisService==null) return;

            var task = m_redisService.AddAsync<String>(m_logName, msg);
        }
        /// <summary>
        /// 记录日志信息
        /// </summary>
        /// <param name="info"></param>
        public void RecordLogInfo(LogInfo info)
        {
            if (string.IsNullOrWhiteSpace(m_logName)) return;
            if (m_redisService == null) return;
            if (info==null) return;

            var task = m_redisService.ListLeftPushAsync<LogInfo>(m_logName, info);
        }
    }
}
