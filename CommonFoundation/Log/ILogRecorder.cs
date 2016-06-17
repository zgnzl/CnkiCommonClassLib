using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Log
{
    public interface ILogRecorder
    {
        /// <summary>
        /// 日志名称
        /// </summary>
        string LogName { get; }
        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="msg"></param>
        void RecordMsg(string msg);
        /// <summary>
        /// 记录日志信息
        /// </summary>
        /// <param name="info"></param>
        void RecordLogInfo(LogInfo info);
    }
}
