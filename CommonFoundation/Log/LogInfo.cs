using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Log
{
    /// <summary>
    /// 日志
    /// </summary>
    public class LogInfo
    {
        /// <summary>
        /// 应用名
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string FuncName { get; set; }
        /// <summary>
        /// 日志代码
        /// </summary>
        public string LogCode { get; set; }
        /// <summary>
        /// 日志头
        /// </summary>
        public string LogHead { get; set; }
        /// <summary>
        /// 日志体
        /// </summary>
        public string LogBody { get; set; }
        /// <summary>
        /// 其他描述
        /// </summary>
        public string OtherDesc { get; set; }
        /// <summary>
        /// 错误时间
        /// </summary>
        public string LogTime { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public LogType LogType { get; set; }
    }


    /// <summary>
    /// 日志消息类型的枚举
    /// </summary>
    public enum LogType
    {
        /// <summary>
        /// 指示未知信息类型的日志记录
        /// </summary>
        Unknown,

        /// <summary>
        /// 指示普通信息类型的日志记录
        /// </summary>
        Information,

        /// <summary>
        /// 指示调试类型的日志记录
        /// </summary>
        Debug,

        /// <summary>
        /// 指示警告信息类型的日志记录
        /// </summary>
        Warning,

        /// <summary>
        /// 指示错误信息类型的日志记录
        /// </summary>
        Error,

        /// <summary>
        /// 指示成功信息类型的日志记录
        /// </summary>
        Success
    }
}
