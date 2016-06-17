using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.MongoDB
{
    public static class MongoDBServiceFactory
    {
        /// <summary>
        /// Mongo默认连接
        /// </summary>
        private static string m_default_mongoDBConnection = ConfigurationManager.ConnectionStrings["MongoDBConnection"]!=null?
            ConfigurationManager.ConnectionStrings["MongoDBConnection"].ToString():null;
        /// <summary>
        /// 默认数据库
        /// </summary>
        private static string m_default_databaseName = ConfigurationManager.AppSettings["MongoDB_DatabaseName"]!=null?
            ConfigurationManager.AppSettings["MongoDB_DatabaseName"].ToString():null;

        /// <summary>
        /// 默认MongoDB服务，连接字符串“MongoDBConnection”
        /// </summary>
        /// <returns></returns>
        public static MongoDBService CreateMongoDBService()
        {
            return new MongoDBService(m_default_mongoDBConnection, m_default_databaseName);
        }

        /// <summary>
        /// MongoDB服务
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public static MongoDBService CreateMongoDBService(string connStr, string databaseName)
        {
            return new MongoDBService(connStr, databaseName);
        }
    }
}
