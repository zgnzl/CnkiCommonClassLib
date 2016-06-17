using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Log.Recorder
{
    public class FileLogRecorder:ILogRecorder
    {
        /// <summary>
        /// 加锁
        /// </summary>
        private static object objLock = new object();
        /// <summary>
        /// 路径
        /// </summary>
        private string m_logPath;
        /// <summary>
        /// 文件名
        /// </summary>
        private string m_logName;

        /// <summary>
        /// 日志文件一级目录
        /// </summary>
        public string LogDir
        {
            get { return m_logPath; }
        }
        /// <summary>
        /// 日志名称
        /// </summary>
        public string LogName
        {
            get { return m_logName; }
        }
        /// <summary>
        /// 日志全路径
        /// </summary>
        public string LogFullPath
        {
            get
            {
                return string.Format(@"{0}\{1}.txt", LogDir, LogName);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FileLogRecorder(string logName = "")
        {
            string value = System.Configuration.ConfigurationManager.AppSettings["LogDirPath"] ?? "Log";
            m_logPath = string.Format(@"{0}{1}", System.AppDomain.CurrentDomain.BaseDirectory, value);
            m_logName = string.IsNullOrWhiteSpace(logName) ? string.Format("log_{0}", DateTime.Now.ToString("yyyyMMdd")) : logName;
        }

        /// <summary>
        /// 记录信息
        /// </summary>
        /// <param name="msg"></param>
        public void RecordMsg(string msg)
        {
            if (string.IsNullOrWhiteSpace(msg)) return;
            lock (objLock)
            {
                //创建文件目录
                System.IO.Directory.CreateDirectory(LogDir);
                using (StreamWriter sw = File.AppendText(LogFullPath))
                {
                    string text = string.Format("Time:{0},Msg:{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg);
                    sw.WriteLine(text);
                }
            }
        }
        /// <summary>
        /// 记录日志信息
        /// </summary>
        /// <param name="info"></param>
        public void RecordLogInfo(LogInfo info)
        {
            if (info == null) return;
            lock (objLock)
            {
                //创建文件目录
                System.IO.Directory.CreateDirectory(LogDir);
                using (StreamWriter sw = File.AppendText(LogFullPath))
                {
                    foreach (var property in typeof(LogInfo).GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    {
                        object val = property.GetValue(info);
                        if (val!=null)
                        {
                            string text = string.Format("{0}:{1}", property.Name, val);
                            sw.WriteLine(text);
                        }
                    }
                }
            }
        }
    }
}
