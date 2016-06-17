using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.MySql
{
    public static class MySqlServiceFactory
    {
        /// <summary>
        /// Mysql默认连接，连接中间件
        /// </summary>
        private static string m_default_mysqlConnection = ConfigurationManager.ConnectionStrings["MySQLMiddleware"].ToString();
        /// <summary>
        /// 默认Mysql服务，连接字符串“MySQLConnection”
        /// </summary>
        /// <returns></returns>
        public static MySqlService CreateMySqlDBService()
        {
            return new MySqlService(m_default_mysqlConnection);
        }

        /// <summary>
        /// 创建Mysql服务
        /// </summary>
        /// <param name="mysqlConnStr"></param>
        /// <returns></returns>
        public static MySqlService CreateMySqlDBService(string mysqlConnStr)
        {
            return new MySqlService(mysqlConnStr);
        }
    }
}
