using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Linq.Expressions;

using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace CommonFoundation.MongoDB
{
    /// <summary>
    /// MongoDB操作
    /// </summary>
    public class MongoDBService
    {
        #region 变量
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string m_connectionStr = string.Empty;
        /// <summary>
        /// 数据库名称
        /// 支持运行时更改
        /// </summary>
        public string DatabaseName { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化操作
        /// </summary>
        public MongoDBService(string connStr, string database)
        {
            m_connectionStr = connStr;
            DatabaseName = database;
        }
        #endregion

        #region 插入操作
        /// <summary>
        /// 插入操作
        /// </summary>
        /// <param name="collectionName">集合名</param>
        /// <param name="t">插入的对象</param>
        /// <returns>返回是否插入成功true or false</returns>
        public bool Insert<T>(string collectionName, T t)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);
                Task task = collection.InsertOneAsync(t);
                task.Wait();
                return !task.IsFaulted;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 插入多个实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public bool InsertMany<T>(string collectionName, List<T> list)
        {
            if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentException("collectionName是null、空或由空白字符组成");
            if (list == null) throw new ArgumentException("list是null");
            if (list.Count == 0) throw new ArgumentException("list大小为0");

            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);
                Task task = collection.InsertManyAsync(list);
                task.Wait();
                return !task.IsFaulted;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 插入多个Json
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="jsonList">json链表</param>
        /// <returns></returns>
        public bool InsertMany(string collectionName, List<string> jsonList)
        {
            if (string.IsNullOrWhiteSpace(collectionName)) throw new ArgumentException("collectionName是null、空或由空白字符组成");
            if (jsonList == null) throw new ArgumentException("list是null");
            if (jsonList.Count == 0) throw new ArgumentException("list大小为0");

            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            var db = client.GetDatabase(DatabaseName);
            var collection = db.GetCollection<BsonDocument>(collectionName);
            List<BsonDocument> bsonList = new List<BsonDocument>();
            foreach (string jsonStr in jsonList)
            {
                using (var jsonReader = new JsonReader(jsonStr))
                {
                    var context = BsonDeserializationContext.CreateRoot(jsonReader);
                    var document = collection.DocumentSerializer.Deserialize(context);
                    bsonList.Add(document);
                }
            }
            Task task = collection.InsertManyAsync(bsonList);
            task.Wait();
            return !task.IsFaulted;
        }
        #endregion

        #region 更新操作
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="t">更新对象</param>
        /// <param name="filter">条件</param>
        /// <returns>更新是否成功true or false</returns>
        public bool UpdateOne<T>(string collectionName, Expression<Func<T, bool>> filter, T t)
        {
            if (t == null) return false;

            var itemBson = t.ToBsonDocument();
            var updateDef = new BsonDocumentUpdateDefinition<T>(new BsonDocument("$set", itemBson));

            return UpdateOne<T>(collectionName, filter, updateDef);
        }
        /// <summary>
        /// 更新操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">条件</param>
        /// <param name="updateDef"></param>
        /// <returns></returns>
        public bool UpdateOne<T>(string collectionName, Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDef)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);

                UpdateResult updateResult = collection.UpdateOne(filter, updateDef);
                return updateResult.ModifiedCount > 0 && updateResult.ModifiedCount == updateResult.MatchedCount ? true : false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 更新操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">条件</param>
        /// <param name="updateDef"></param>
        /// <returns></returns>
        public bool UpdateMany<T>(string collectionName, Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDef)
        {
            UpdateResult updateResult = UpdateManyWithResult(collectionName, filter, updateDef);
            return updateResult != null && updateResult.ModifiedCount > 0 && updateResult.ModifiedCount == updateResult.MatchedCount ? true : false;
        }
        /// <summary>
        /// 更新操作,返回更新数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">条件</param>
        /// <param name="updateDef">更新信息</param>
        /// <returns></returns>
        public UpdateResult UpdateManyWithResult<T>(string collectionName, Expression<Func<T, bool>> filter, UpdateDefinition<T> updateDef)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);

                return collection.UpdateMany(filter, updateDef);
            }
            catch (Exception)
            {
                throw;
            }
        }

        #region 更新文档
        /// <summary>
        /// 更新子文档
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filterDefinition">条件</param>
        /// <param name="updateDef">更新信息</param>
        /// <returns></returns>
        public UpdateResult DocumentUpdate<T>(string collectionName, FilterDefinition<T> filterDefinition, UpdateDefinition<T> updateDef)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);
                
                return collection.UpdateMany(filterDefinition, updateDef);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        #endregion

        #region 获取集合

        #region 分页列表
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="filter">条件</param>
        /// <param name="sort">排序信息</param>       
        /// <param name="total">总信息量</param>
        /// <returns>返回集合</returns>
        public List<T> PageList<T>(string collectionName, int pageIndex, int pageSize, Expression<Func<T, bool>> filter, SortDefinition<T> sort, out int total, ProjectionDefinition<T> projection = null)
        {
            return FindPageList<T>(collectionName, pageIndex, pageSize, filter, sort, out total, projection);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="filter">条件</param>
        /// <param name="total">总信息量</param>
        /// <returns>返回集合</returns>
        public List<T> PageList<T>(string collectionName, int pageIndex, int pageSize, Expression<Func<T, bool>> filter, out int total, ProjectionDefinition<T> projection = null)
        {
            return FindPageList<T>(collectionName, pageIndex, pageSize, filter, null, out total, projection);
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="filter">条件</param>
        /// <param name="sort">排序信息</param>       
        /// <param name="total">总信息量</param>
        /// <param name="projection">输出的字段信息</param>
        /// <returns>返回集合</returns>
        private List<T> FindPageList<T>(string collectionName, int pageIndex, int pageSize, Expression<Func<T, bool>> filter, SortDefinition<T> sort, out int total, ProjectionDefinition<T> projection)
        {
            total = 0;
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {

                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);
                total = Convert.ToInt32(collection.Count(filter));

                if (projection == null)
                {
                    var list = sort == null ?
                        collection.Find<T>(filter).Skip(pageSize * (pageIndex - 1)).Limit(pageSize).ToList() :
                        collection.Find<T>(filter).Sort(sort).Skip(pageSize * (pageIndex - 1)).Limit(pageSize).ToList();
                    return list;
                }
                else
                {
                    var list = sort == null ?
                        collection.Find<T>(filter).Skip(pageSize * (pageIndex - 1)).Limit(pageSize).Project<T>(projection).ToList() :
                        collection.Find<T>(filter).Sort(sort).Skip(pageSize * (pageIndex - 1)).Limit(pageSize).Project<T>(projection).ToList();
                    return list;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }     

        #endregion

        /// <summary>
        /// 获得全部信息列表
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="sort">排序信息</param> 
        /// <param name="projection">输出字段信息</param>
        /// <returns>全部信息列表</returns>
        public List<T> AllList<T>(string collectionName, SortDefinition<T> sort, ProjectionDefinition<T> projection = null)
        {
            return FindAllList<T>(collectionName, sort, projection);
        }
        /// <summary>
        /// 获得全部信息列表
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="projection">输出字段信息</param>
        /// <returns>全部信息列表</returns>
        public List<T> AllList<T>(string collectionName, ProjectionDefinition<T> projection = null)
        {
            return FindAllList<T>(collectionName, null, projection);
        }
        private List<T> FindAllList<T>(string collectionName, SortDefinition<T> sort, ProjectionDefinition<T> projection = null)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);
                if (projection == null)
                {
                    var list = sort == null ?
                        collection.Find<T>(x => true).ToList() :
                        collection.Find<T>(x => true).Sort(sort).ToList();
                    return list;
                }
                else
                {
                    var list = sort == null ?
                         collection.Find<T>(x => true).Project<T>(projection).ToList() :
                         collection.Find<T>(x => true).Sort(sort).Project<T>(projection).ToList();
                    return list;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获得信息列表
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">条件</param>
        /// <param name="projection">输出字段信息</param>
        /// <returns>全部信息列表</returns>
        public List<T> List<T>(string collectionName, Expression<Func<T, bool>> filter, ProjectionDefinition<T> projection = null)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);
                if (projection == null)
                {
                    var list = collection.Find<T>(filter == null ? x => true : filter).ToList();
                    return list;
                }
                else
                {
                    var list = collection.Find<T>(filter == null ? x => true : filter).Project<T>(projection).ToList();
                    return list;
                }


            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取Top列表
        /// </summary>
        /// <param name="collectionName">集合名称</param>     
        /// <param name="size">选取数量</param>
        /// <param name="filter">条件</param>
        /// <param name="sort">排序信息</param>    
        /// <param name="projection">输出的字段信息</param>
        /// <returns>返回集合</returns>
        public List<T> TopList<T>(string collectionName, int size, Expression<Func<T, bool>> filter, SortDefinition<T> sort, ProjectionDefinition<T> projection = null)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);
                if (projection == null)
                {
                    var list = sort == null ?
                        collection.Find<T>(filter).Limit(size).ToList() :
                        collection.Find<T>(filter).Sort(sort).Limit(size).ToList();
                    return list;
                }
                else
                {
                    var list = sort == null ?
                        collection.Find<T>(filter).Limit(size).Project<T>(projection).ToList() :
                        collection.Find<T>(filter).Sort(sort).Limit(size).Project<T>(projection).ToList();
                    return list;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 读取单条记录
        /// <summary>
        /// 读取单条记录
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">条件</param>
        /// <param name="projection">需要输出的字段信息</param>
        /// <returns>单条记录</returns>
        public T Single<T>(string collectionName, Expression<Func<T, bool>> filter, ProjectionDefinition<T> projection=null)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);
                var single = projection == null ? collection.Find(filter) : collection.Find(filter).Project<T>(projection);
                var list = single.ToList();
                if (list.Count > 0)
                {
                    return list.First();
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion

        #region 删除操作
        /// <summary>
        /// 删除操作
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">条件</param>
        /// <returns>删除的文档数</returns>
        public long Delete<T>(string collectionName, Expression<Func<T, bool>> filter)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);
                DeleteResult result = collection.DeleteMany(filter);
                return result.DeletedCount;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 统计
        /// <summary>
        /// 统计集合文档数
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter">条件</param>
        /// <returns>统计结果</returns>
        public long Count<T>(string collectionName, Expression<Func<T, bool>> filter)
        {
            long total = 0;
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);
                total = collection.Count(filter);

                return total;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 统计集合文档数
        /// </summary>
        /// <param name="collectionName">集合名称</param>
        /// <returns>统计结果</returns>
        public long Count(string collectionName)
        {
            long total = 0;
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<BsonDocument>(collectionName);
                total = collection.Count(new BsonDocument());
                return total;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 内嵌文档分页
        /// <summary>
        /// 内嵌文档分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filter"></param>
        /// <param name="projection"></param>
        /// <returns></returns>
        public T SubdocumentPageList<T>(string collectionName, Expression<Func<T, bool>> filter, ProjectionDefinition<T> projection)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);

                var kk = collection.Find(filter);//.Project(projection);
                var item = collection.Find<T>(filter).Project<T>(projection);

                return item.First();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 文档是否存在
        /// <summary>
        /// 文档是否存在
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collectionName">集合名称</param>
        /// <param name="filterDefinition">条件</param>
        /// <returns></returns>
        public bool ExistDocument<T>(string collectionName, FilterDefinition<T> filterDefinition)
        {
            MongoClient client = MongoClientProvider.GetClient(m_connectionStr);
            try
            {
                var db = client.GetDatabase(DatabaseName);
                var collection = db.GetCollection<T>(collectionName);

                return collection.Find(filterDefinition).Count() > 0;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}