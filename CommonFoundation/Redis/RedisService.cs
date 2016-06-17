using CommonFoundation.Redis.Serializer;
using CommonFoundation.Redis.Extensions;

using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFoundation.Redis
{
    public class RedisService
    {
        #region 变量
        /// <summary>
        /// Redis连接
        /// </summary>
        private readonly ConnectionMultiplexer m_connectionMultiplexer = null;
        /// <summary>
        /// 序列化
        /// 支持运行时变更
        /// </summary>
        public ISerializer Serializer { get; set; }
        /// <summary>
        /// Redis 数据库序号
        /// 支持运行时变更
        /// </summary>
        public int DatabaseSerial { get; set; }
        /// <summary>
        /// Redis 数据库
        /// </summary>
        private IDatabase Database
        {
            get
            {
                return m_connectionMultiplexer.GetDatabase(DatabaseSerial);
            }
        }
        /// <summary>
        /// ReidsKey构建方法
        /// 支持运行时变更
        /// </summary>
        public Func<string,string> RedisKeyFunc { get; set; }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="database"></param>
        public RedisService(string connStr, Func<string, string> keyFunc = null, int database = 0)
        {
            Serializer = new NewtonsoftSerializer();
            DatabaseSerial = database;
            RedisKeyFunc = keyFunc;
            m_connectionMultiplexer = RedisClientProvider.GetConnection(connStr);
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="connStr"></param>
        /// <param name="serializer"></param>
        /// <param name="database"></param>
        public RedisService(string connStr, ISerializer serializer, Func<string, string> keyFunc = null, int database = 0)
        {
            Serializer = serializer;
            DatabaseSerial = database;
            RedisKeyFunc = keyFunc;
            m_connectionMultiplexer = RedisClientProvider.GetConnection(connStr);
        }
        #endregion

        #region RedisKey
        /// <summary>
        /// RedisKey构造方法
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private RedisKey GetRedisKey(string key)
        {
            return RedisKeyFunc == null ? key : RedisKeyFunc(key);
        }
        #endregion

        #region Exists
        /// <summary>
        ///     Verify that the specified cache key exists
        /// </summary>
        /// <param name="key">RedisKey.</param>
        /// <returns>
        ///     True if the key is present into Redis. Othwerwise False
        /// </returns>
        public bool Exists(string key)
        {
            string redisKey = GetRedisKey(key);
            return Database.KeyExists(redisKey);
        }

        /// <summary>
        ///     Verify that the specified cache key exists
        /// </summary>
        /// <param name="key">RedisKey.</param>
        /// <returns>
        ///     True if the key is present into Redis. Othwerwise False
        /// </returns>
        public Task<bool> ExistsAsync(string key)
        {
            string redisKey = GetRedisKey(key);
            return Database.KeyExistsAsync(redisKey);
        }
        #endregion

        #region Remove
        /// <summary>
        ///     Removes the specified key from Redis Database
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     True if the key has removed. Othwerwise False
        /// </returns>
        public bool Remove(string key)
        {
            string redisKey = GetRedisKey(key);
            return Database.KeyDelete(redisKey);
        }

        /// <summary>
        ///     Removes the specified key from Redis Database
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        ///     True if the key has removed. Othwerwise False
        /// </returns>
        public Task<bool> RemoveAsync(string key)
        {
            string redisKey = GetRedisKey(key);
            return Database.KeyDeleteAsync(redisKey);
        }

        /// <summary>
        ///     Removes all specified keys from Redis Database
        /// </summary>
        /// <param name="keys">The key.</param>
        public void RemoveAll(IEnumerable<string> keys)
        {
            keys.ForEach(x => Remove(x));
        }

        /// <summary>
        ///     Removes all specified keys from Redis Database
        /// </summary>
        /// <param name="keys">The key.</param>
        /// <returns></returns>
        public Task RemoveAllAsync(IEnumerable<string> keys)
        {
            return keys.ForEachAsync(RemoveAsync);
        }
        /// <summary>
        /// Removes with Prefix from Redis Database
        /// </summary>
        /// <param name="prefix"></param>
        public void RemovePrefix(string prefix)
        {
            Database.KeyDeleteWithPrefix(prefix);
        }
        /// <summary>
        ///     Clear All 
        /// </summary>
        public void Clear()
        {
            Database.KeyDeleteWithPrefix(("*"));
        }
        #endregion

        #region Base
        /// <summary>
        ///     Rewrite IDatabase.StringGet,add GetRedisKey Function
        /// </summary>
        /// <param name="key">RedisKey</param>
        /// <returns>
        ///     the value of key, or nil when key does not exist.
        /// </returns>
        public RedisValue StringGet(string key, CommandFlags flags = CommandFlags.None)
        {
            string redisKey = GetRedisKey(key);
            return Database.StringGet(redisKey, flags);
        }
        /// <summary>
        ///     Rewrite IDatabase.StringGetAsync,add GetRedisKey Function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Task<RedisValue> StringGetAsync(string key, CommandFlags flags = CommandFlags.None)
        {
            string redisKey = GetRedisKey(key);
            return Database.StringGetAsync(redisKey, flags);
        }
        
        /// <summary>
        ///     Rewrite IDatabase.StringSet,add GetRedisKey Function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool StringSet(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            string redisKey = GetRedisKey(key);
            return Database.StringSet(redisKey, value, expiry, when, flags);
        }
        /// <summary>
        ///     Rewrite IDatabase.StringSetAsync,add GetRedisKey Function
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <param name="when"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public Task<bool> StringSetAsync(RedisKey key, RedisValue value, TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            string redisKey = GetRedisKey(key);
            return Database.StringSetAsync(redisKey, value, expiry, when, flags);
        }
        #endregion

        #region Model T
        /// <summary>
        ///     Get the object with the specified key from Redis database
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="key">RedisKey.</param>
        /// <returns>
        ///     Null if not present, otherwise the instance of T.
        /// </returns>
        public T Get<T>(string key)
        {
            var valueBytes = StringGet(key);

            if (!valueBytes.HasValue)
            {
                return default(T);
            }

            return Serializer.Deserialize<T>(valueBytes);
        }

        /// <summary>
        ///     Get the object with the specified key from Redis database
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="key">RedisKey.</param>
        /// <returns>
        ///     Null if not present, otherwise the instance of T.
        /// </returns>
        public async Task<T> GetAsync<T>(string key)
        {
            var valueBytes = await StringGetAsync(key);

            if (!valueBytes.HasValue)
            {
                return default(T);
            }

            return await Serializer.DeserializeAsync<T>(valueBytes);
        }

        /// <summary>
        ///     Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">RedisKey.</param>
        /// <param name="value">The instance of T.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public bool Add<T>(string key, T value)
        {
            var entryBytes = Serializer.Serialize(value);
            return StringSet(key, entryBytes);
        }

        /// <summary>
        ///     Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">RedisKey.</param>
        /// <param name="value">The instance of T.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public async Task<bool> AddAsync<T>(string key, T value)
        {
            var entryBytes = await Serializer.SerializeAsync(value);
            return await StringSetAsync(key, entryBytes);
        }

        /// <summary>
        ///     Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public bool Replace<T>(string key, T value)
        {
            return Add(key, value);
        }

        /// <summary>
        ///     Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public Task<bool> ReplaceAsync<T>(string key, T value)
        {
            return AddAsync(key, value);
        }

        /// <summary>
        ///     Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">RedisKey.</param>
        /// <param name="value">The instance of T.</param>
        /// <param name="expiresAt">Expiration time.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public bool Add<T>(string key, T value, DateTimeOffset expiresAt)
        {
            var entryBytes = Serializer.Serialize(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);
            return StringSet(key, entryBytes, expiration);
        }

        /// <summary>
        ///     Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">RedisKey.</param>
        /// <param name="value">The instance of T.</param>
        /// <param name="expiresAt">Expiration time.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public async Task<bool> AddAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            var entryBytes = await Serializer.SerializeAsync(value);
            var expiration = expiresAt.Subtract(DateTimeOffset.Now);
            return await StringSetAsync(key, entryBytes, expiration);
        }

        /// <summary>
        ///     Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <param name="expiresAt">Expiration time.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public bool Replace<T>(string key, T value, DateTimeOffset expiresAt)
        {
            return Add(key, value, expiresAt);
        }

        /// <summary>
        ///     Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <param name="expiresAt">Expiration time.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public Task<bool> ReplaceAsync<T>(string key, T value, DateTimeOffset expiresAt)
        {
            return AddAsync(key, value, expiresAt);
        }

        /// <summary>
        ///     Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">RedisKey.</param>
        /// <param name="value">The instance of T.</param>
        /// <param name="expiresIn">The duration of the cache using Timespan.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public bool Add<T>(string key, T value, TimeSpan expiresIn)
        {
            var entryBytes = Serializer.Serialize(value);
            return StringSet(key, entryBytes, expiresIn);
        }

        /// <summary>
        ///     Adds the specified instance to the Redis database.
        /// </summary>
        /// <typeparam name="T">The type of the class to add to Redis</typeparam>
        /// <param name="key">RedisKey.</param>
        /// <param name="value">The instance of T.</param>
        /// <param name="expiresIn">The duration of the cache using Timespan.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public async Task<bool> AddAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            var entryBytes = await Serializer.SerializeAsync(value);
            return await StringSetAsync(key, entryBytes, expiresIn);
        }

        /// <summary>
        ///     Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <param name="expiresIn">The duration of the cache using Timespan.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public bool Replace<T>(string key, T value, TimeSpan expiresIn)
        {
            return Add(key, value, expiresIn);
        }

        /// <summary>
        ///     Replaces the object with specified key into Redis database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The instance of T</param>
        /// <param name="expiresIn">The duration of the cache using Timespan.</param>
        /// <returns>
        ///     True if the object has been added. Otherwise false
        /// </returns>
        public Task<bool> ReplaceAsync<T>(string key, T value, TimeSpan expiresIn)
        {
            return AddAsync(key, value, expiresIn);
        }

        /// <summary>
        ///     Get the objects with the specified keys from Redis database with one roundtrip
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="keys">The keys.</param>
        /// <returns>
        ///     Empty list if there are no results, otherwise the instance of T.
        ///     If a cache key is not present on Redis the specified object into the returned Dictionary will be null
        /// </returns>
        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => GetRedisKey(x)).ToArray();
            var result = Database.StringGet(redisKeys);

            var dict = new Dictionary<string, T>(StringComparer.Ordinal);
            for (var index = 0; index < redisKeys.Length; index++)
            {
                var value = result[index];
                dict.Add(redisKeys[index], value == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(value));
            }

            return dict;
        }

        /// <summary>
        ///     Get the objects with the specified keys from Redis database with one roundtrip
        /// </summary>
        /// <typeparam name="T">The type of the expected object</typeparam>
        /// <param name="keys">The keys.</param>
        /// <returns>
        ///     Empty list if there are no results, otherwise the instance of T.
        ///     If a cache key is not present on Redis the specified object into the returned Dictionary will be null
        /// </returns>
        public async Task<IDictionary<string, T>> GetAllAsync<T>(IEnumerable<string> keys)
        {
            var redisKeys = keys.Select(x => GetRedisKey(x)).ToArray();
            var result = await Database.StringGetAsync(redisKeys);
            var dict = new Dictionary<string, T>(StringComparer.Ordinal);
            for (var index = 0; index < redisKeys.Length; index++)
            {
                var value = result[index];
                dict.Add(redisKeys[index], value == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(value));
            }
            return dict;
        }

        /// <summary>
        ///     Adds all.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        public bool AddAll<T>(IList<Tuple<string, T>> items)
        {
            var values = items
                .Select(item => new KeyValuePair<RedisKey, RedisValue>(GetRedisKey(item.Item1), Serializer.Serialize(item.Item2)))
                .ToArray();

            return Database.StringSet(values);
        }

        /// <summary>
        ///     Adds all asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        public async Task<bool> AddAllAsync<T>(IList<Tuple<string, T>> items)
        {
            var values = items
                .Select(item => new KeyValuePair<RedisKey, RedisValue>(GetRedisKey(item.Item1), Serializer.Serialize(item.Item2)))
                .ToArray();

            return await Database.StringSetAsync(values);
        }
        #endregion

        #region Set
        /// <summary>
        ///     Run SADD command http://redis.io/commands/sadd
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool SetAdd<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("key cannot be empty.", "" + key + "");
            if (item == null) throw new ArgumentNullException("" + item + "", "item cannot be null.");

            var redisKey = GetRedisKey(key);
            var serializedObject = Serializer.Serialize(item);
            return Database.SetAdd(redisKey, serializedObject);
        }

        /// <summary>
        ///     Run SADD command http://redis.io/commands/sadd"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> SetAddAsync<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("key cannot be empty.", "" + key + "");
            if (item == null) throw new ArgumentNullException("" + item + "", "item cannot be null.");

            var redisKey = GetRedisKey(key);
            var serializedObject = await Serializer.SerializeAsync(item);
            return await Database.SetAddAsync(redisKey, serializedObject);
        }

        /// <summary>
        ///     Run SMEMBERS command http://redis.io/commands/SMEMBERS
        /// </summary>
        /// <param name="key">Name of the member.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string[] SetMembers(string key)
        {
            var redisKey = GetRedisKey(key);
            return Database.SetMembers(redisKey).Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        ///     Run SMEMBERS command see http://redis.io/commands/SMEMBERS
        /// </summary>
        /// <param name="key">Name of the member.</param>
        /// <returns></returns>
        public async Task<string[]> SetMembersAsync(string key)
        {
            var redisKey = GetRedisKey(key);
            return (await Database.SetMembersAsync(redisKey)).Select(x => x.ToString()).ToArray();
        }

        /// <summary>
        ///     Run SMEMBERS command see http://redis.io/commands/SMEMBERS
        ///     Deserializes the results to T
        /// </summary>
        /// <typeparam name="T">The type of the expected objects</typeparam>
        /// <param name="key">The key</param>
        /// <returns>An array of objects in the set</returns>
        public IEnumerable<T> SetMembers<T>(string key)
        {
            var redisKey = GetRedisKey(key);
            var members = Database.SetMembers(redisKey);

            return members.Select(m => m == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(m));
        }

        /// <summary>
        ///     Run SMEMBERS command see http://redis.io/commands/SMEMBERS
        ///     Deserializes the results to T
        /// </summary>
        /// <typeparam name="T">The type of the expected objects</typeparam>
        /// <param name="key">The key</param>
        /// <returns>An array of objects in the set</returns>
        public async Task<IEnumerable<T>> SetMembersAsync<T>(string key)
        {
            var redisKey = GetRedisKey(key);
            var members = await Database.SetMembersAsync(redisKey);

            return members.Select(m => m == RedisValue.Null ? default(T) : Serializer.Deserialize<T>(m));
        }
        #endregion

        #region List
        /// <summary>
        ///     Insert the specified value at the head of the list stored at key. If key does not exist, it is created as empty
        ///     list before performing the push operations.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     the length of the list after the push operations.
        /// </returns>
        /// <exception cref="System.ArgumentException">key cannot be empty.;key</exception>
        /// <exception cref="System.ArgumentNullException">item;item cannot be null.</exception>
        /// <remarks>
        ///     http://redis.io/commands/lpush
        /// </remarks>
        public long ListLeftPush<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("key cannot be empty.", "" + key + "");
            if (item == null) throw new ArgumentNullException("" + item + "", "item cannot be null.");

            string redisKey = GetRedisKey(key);
            var serializedItem = Serializer.Serialize(item);

            return Database.ListLeftPush(redisKey, serializedItem);
        }
        /// <summary>
        ///  Insert the specified value at the head of the list stored at key. If key
        //   does not exist, it is created as empty list before performing the push operations.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public long ListLeftPush(string key, string item)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("key cannot be empty.", "" + key + "");
            if (item == null) throw new ArgumentNullException("" + item + "", "item cannot be null.");

            string redisKey = GetRedisKey(key);

            return Database.ListLeftPush(redisKey, item);
        }
        /// <summary>
        ///     Lists the add to left asynchronous.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">key cannot be empty.;key</exception>
        /// <exception cref="System.ArgumentNullException">item;item cannot be null.</exception>
        public async Task<long> ListLeftPushAsync<T>(string key, T item) where T : class
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("key cannot be empty.", "" + key + "");
            if (item == null) throw new ArgumentNullException("" + item + "", "item cannot be null.");

            var redisKey = GetRedisKey(key);
            var serializedItem = await Serializer.SerializeAsync(item);

            return await Database.ListLeftPushAsync(redisKey, serializedItem);
        }

        /// <summary>
        ///     Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">key cannot be empty.;key</exception>
        /// <remarks>
        ///     http://redis.io/commands/rpop
        /// </remarks>
        public T ListRightPop<T>(string key) where T : class
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "" + key + "");
            }

            var redisKey = GetRedisKey(key);
            var item = Database.ListRightPop(redisKey);

            return item == RedisValue.Null ? null : Serializer.Deserialize<T>(item);
        }
        /// <summary>
        ///     Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ListRightPop(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key cannot be empty.", "" + key + "");
            }

            var redisKey = GetRedisKey(key);
            var item = Database.ListRightPop(redisKey);

            return item == RedisValue.Null ? null : item.ToString();
        }

        /// <summary>
        ///     Removes and returns the last element of the list stored at key.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">key cannot be empty.;key</exception>
        /// <remarks>
        ///     http://redis.io/commands/rpop
        /// </remarks>
        public async Task<T> ListRightPopAsync<T>(string key) where T : class
        {
            
            if (string.IsNullOrEmpty(key)) throw new ArgumentException("key cannot be empty.", "" + key + "");

            string redisKey = GetRedisKey(key);
            var item = await Database.ListRightPopAsync(redisKey);
            if (item == RedisValue.Null) return null;
            return item == RedisValue.Null ? null : await Serializer.DeserializeAsync<T>(item);
        }

        /// <summary>
        /// Returns the length of the list stored at key. If key does not exist, it is
        //     interpreted as an empty list and 0 is returned.
        /// </summary>
        /// <param name="key">Key of the list</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns></returns>
        public long ListLength(string key, CommandFlags commandFlags = CommandFlags.None)
        {
            string redisKey = GetRedisKey(key);
            return Database.ListLength(redisKey);
        }

        /// <summary>
        ///     Returns the number of fields contained in the list at key.
        /// </summary>
        /// <param name="hashKey">Key of the list</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>number of fields in the hash, or 0 when key does not exist.</returns>
        public async Task<long> ListLengthAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return await Database.ListLengthAsync(redisKey, commandFlags);
        }
        #endregion

        #region Hash

        #region Hash Delete
        /// <summary>
        ///     Removes the specified fields from the hash stored at key. 
        ///     Specified fields that do not exist within this hash are ignored. 
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="key">Key of the entry</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>
        ///     If key is deleted returns true.
        ///     If key does not exist, it is treated as an empty hash and this command returns false.
        /// </returns>
        public bool HashDelete(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database.HashDelete(redisKey, key, commandFlags);
        }

        /// <summary>
        ///     Removes the specified fields from the hash stored at key. 
        ///     Specified fields that do not exist within this hash are ignored. 
        ///     If key does not exist, it is treated as an empty hash and this command returns 0.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the number of fields to be removed.
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="keys"></param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>Tthe number of fields that were removed from the hash, not including specified but non existing fields.</returns>
        public long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database.HashDelete(redisKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        /// <param name="hashKey">Key of the hash</param>
        /// <param name="key">Key of the entry</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>
        ///     If key is deleted returns true.
        ///     If key does not exist, it is treated as an empty hash and this command returns false.
        /// </returns>
        public async Task<bool> HashDeleteAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return await Database.HashDeleteAsync(redisKey, key, commandFlags);
        }

        /// <summary>
        ///     Removes the specified fields from the hash stored at key. 
        ///     Specified fields that do not exist within this hash are ignored. 
        ///     If key does not exist, it is treated as an empty hash and this command returns 0.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the number of fields to be removed.
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="keys">Keys to retrieve from the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>Tthe number of fields that were removed from the hash, not including specified but non existing fields.</returns>
        public async Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return await Database.HashDeleteAsync(redisKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }
        #endregion

        #region Hash Exists
        /// <summary>
        ///     Returns if field is an existing field in the hash stored at key.
        /// </summary>
        /// 
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <param name="hashKey">The key of the hash in redis</param>
        /// <param name="key">The key of the field in the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>Returns if field is an existing field in the hash stored at key.</returns>
        public bool HashExists(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database.HashExists(redisKey, key, commandFlags);
        }

        /// <summary>
        ///     Returns if field is an existing field in the hash stored at key.
        /// </summary>
        /// 
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <param name="hashKey">The key of the hash in redis</param>
        /// <param name="key">The key of the field in the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>Returns if field is an existing field in the hash stored at key.</returns>
        public async Task<bool> HashExistsAsync(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return await Database.HashExistsAsync(redisKey, key, commandFlags);
        }
        #endregion

        #region Hash Get
        /// <summary>
        ///     Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="key">Key of the entry</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>the value associated with field, or nil when field is not present in the hash or key does not exist.</returns>
        public T HashGet<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            var redisValue = Database.HashGet(redisKey, key, commandFlags);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default(T);
        }

        /// <summary>
        ///     Returns the values associated with the specified fields in the hash stored at key.
        ///     For every field that does not exist in the hash, a nil value is returned. 
        ///     Because a non-existing keys are treated as empty hashes, running HMGET against a non-existing key will return a list of nil values.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the number of fields being requested.
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="keys"></param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>list of values associated with the given fields, in the same order as they are requested.</returns>
        public Dictionary<string, T> HashGet<T>(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return keys.Select(x => new { key = x, value = HashGet<T>(redisKey, x, commandFlags) })
                        .ToDictionary(kv => kv.key, kv => kv.value, StringComparer.Ordinal);
        }

        /// <summary>
        ///     Returns all fields and values of the hash stored at key. In the returned value, every field name is followed by its value, so the length of the reply is twice the size of the hash.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the size of the hash.
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>list of fields and their values stored in the hash, or an empty list when key does not exist.</returns>
        public Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database
                        .HashGetAll(redisKey, commandFlags)
                        .ToDictionary(
                            x => x.Name.ToString(),
                            x => Serializer.Deserialize<T>(x.Value),
                            StringComparer.Ordinal);
        }

        /// <summary>
        ///     Returns the value associated with field in the hash stored at key.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="key">Key of the entry</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>the value associated with field, or nil when field is not present in the hash or key does not exist.</returns>
        public async Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            var redisValue = await Database.HashGetAsync(redisKey, key, commandFlags);
            return redisValue.HasValue ? Serializer.Deserialize<T>(redisValue) : default(T);
        }

        /// <summary>
        ///     Returns the values associated with the specified fields in the hash stored at key.
        ///     For every field that does not exist in the hash, a nil value is returned. 
        ///     Because a non-existing keys are treated as empty hashes, running HMGET against a non-existing key will return a list of nil values.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the number of fields being requested.
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="keys">Keys to retrieve from the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>list of values associated with the given fields, in the same order as they are requested.</returns>
        public async Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            var result = new Dictionary<string, T>();
            var redisKey = GetRedisKey(hashKey);
            foreach (var key in keys)
            {
                var value = await HashGetAsync<T>(redisKey, key, commandFlags);
                result.Add(key, value);
            }
            return result;
        }

        /// <summary>
        ///     Returns all fields and values of the hash stored at key. In the returned value, 
        ///     every field name is followed by its value, so the length of the reply is twice the size of the hash.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the size of the hash.
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>list of fields and their values stored in the hash, or an empty list when key does not exist.</returns>
        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return (await Database
                        .HashGetAllAsync(redisKey, commandFlags))
                        .ToDictionary(
                            x => x.Name.ToString(),
                            x => Serializer.Deserialize<T>(x.Value),
                            StringComparer.Ordinal);
        }
        #endregion

        #region Hash Increments
        /// <summary>
        ///     Increments the number stored at field in the hash stored at key by increment. If key does not exist, a new key holding a hash is created. 
        ///     If field does not exist the value is set to 0 before the operation is performed.
        ///     The range of values supported by HINCRBY is limited to 64 bit signed integers.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="key">Key of the entry</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <param name="value">the value at field after the increment operation</param>
        public long HashIncrement(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database.HashIncrement(redisKey, key, value, commandFlags);
        }

        /// <summary>
        ///     Increment the specified field of an hash stored at key, and representing a floating point number, by the specified increment. 
        ///     If the field does not exist, it is set to 0 before performing the operation.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         An error is returned if one of the following conditions occur:
        ///         * The field contains a value of the wrong type (not a string).
        ///         * The current field content or the specified increment are not parsable as a double precision floating point number.
        ///     </para>
        ///     <para>
        ///         Time complexity: O(1)
        ///     </para>
        ///     
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="key">Key of the entry</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <param name="value">the value at field after the increment operation</param>
        public double HashIncrement(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database.HashIncrement(redisKey, key, value, commandFlags);
        }

        /// <summary>
        ///     Increments the number stored at field in the hash stored at key by increment. If key does not exist, a new key holding a hash is created. 
        ///     If field does not exist the value is set to 0 before the operation is performed.
        ///     The range of values supported by HINCRBY is limited to 64 bit signed integers.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="key">Key of the entry</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <param name="value">the value at field after the increment operation</param>
        public async Task<long> HashIncerementByAsync(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return await Database.HashIncrementAsync(redisKey, key, value, commandFlags);
        }

        /// <summary>
        ///     Increment the specified field of an hash stored at key, and representing a floating point number, by the specified increment. 
        ///     If the field does not exist, it is set to 0 before performing the operation.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         An error is returned if one of the following conditions occur:
        ///         * The field contains a value of the wrong type (not a string).
        ///         * The current field content or the specified increment are not parsable as a double precision floating point number.
        ///     </para>
        ///     <para>
        ///         Time complexity: O(1)
        ///     </para>
        ///     
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="key">Key of the entry</param>
        /// <param name="value">the value at field after the increment operation</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>the value at field after the increment operation.</returns>
        public async Task<double> HashIncrementAsync(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return await Database.HashIncrementAsync(redisKey, key, value, commandFlags);
        }
        #endregion

        #region Hash Other
        /// <summary>
        ///     Returns all field names in the hash stored at key.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the size of the hash.
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>list of fields in the hash, or an empty list when key does not exist.</returns>
        public IEnumerable<string> HashKeys(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database.HashKeys(redisKey, commandFlags).Select(x => x.ToString());
        }

        /// <summary>
        ///     Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>number of fields in the hash, or 0 when key does not exist.</returns>
        public long HashLength(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database.HashLength(redisKey, commandFlags);
        }

        /// <summary>
        ///     Returns all values in the hash stored at key.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the size of the hash.
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>list of values in the hash, or an empty list when key does not exist.</returns>
        public IEnumerable<T> HashValues<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database.HashValues(redisKey, commandFlags).Select(x => Serializer.Deserialize<T>(x));
        }

        /// <summary>
        ///     iterates fields of Hash types and their associated values.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1) for every call. O(N) for a complete iteration, including enough command calls for the cursor to return back to 0. 
        ///     N is the number of elements inside the collection.
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="pattern">GLOB search pattern</param>
        /// <param name="pageSize">Number of elements to retrieve from the redis server in the cursor</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns></returns>
        public Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database.HashScan(redisKey, pattern, pageSize, commandFlags)
                        .ToDictionary(x => x.Name.ToString(),
                                      x => Serializer.Deserialize<T>(x.Value),
                                      StringComparer.Ordinal);
        }

        /// <summary>
        ///     Returns all field names in the hash stored at key.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the size of the hash.
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>list of fields in the hash, or an empty list when key does not exist.</returns>
        public async Task<IEnumerable<string>> HashKeysAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return (await Database.HashKeysAsync(redisKey, commandFlags)).Select(x => x.ToString());
        }

        /// <summary>
        ///     Returns the number of fields contained in the hash stored at key.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1)
        /// </remarks>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>number of fields in the hash, or 0 when key does not exist.</returns>
        public async Task<long> HashLengthAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return await Database.HashLengthAsync(redisKey, commandFlags);
        }

        /// <summary>
        ///     Returns all values in the hash stored at key.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the size of the hash.
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>list of values in the hash, or an empty list when key does not exist.</returns>
        public async Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return (await Database.HashValuesAsync(redisKey, commandFlags)).Select(x => Serializer.Deserialize<T>(x));
        }

        /// <summary>
        ///     iterates fields of Hash types and their associated values.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(1) for every call. O(N) for a complete iteration, including enough command calls for the cursor to return back to 0. 
        ///     N is the number of elements inside the collection.
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="pattern">GLOB search pattern</param>
        /// <param name="pageSize"></param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns></returns>
        public async Task<Dictionary<string, T>> HashScanAsync<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return (await Task.Run(() => Database.HashScan(redisKey, pattern, pageSize, commandFlags)))
                .ToDictionary(x => x.Name.ToString(), x => Serializer.Deserialize<T>(x.Value), StringComparer.Ordinal);
        }
        #endregion

        #region Hash Set
        /// <summary>
        ///     Sets field in the hash stored at key to value. If key does not exist, a new key holding a hash is created. If field already exists in the hash, it is overwritten.
        /// </summary>
        /// 
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">The key of the hash in redis</param>
        /// <param name="key">The key of the field in the hash</param>
        /// <param name="nx">Behave like hsetnx - set only if not exists</param>
        /// <param name="value">The value to be inserted</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>
        ///     <c>true</c> if field is a new field in the hash and value was set.
        ///     <c>false</c> if field already exists in the hash and no operation was performed.
        /// </returns>
        public bool HashSet<T>(string hashKey, string key, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return Database.HashSet(redisKey, key, Serializer.Serialize(value), nx ? When.NotExists : When.Always, commandFlags);
        }

        /// <summary>
        ///     Sets the specified fields to their respective values in the hash stored at key. This command overwrites any existing fields in the hash. If key does not exist, a new key holding a hash is created.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the number of fields being set.
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="values"></param>
        /// <param name="commandFlags">Command execution flags</param>
        public void HashSet<T>(string hashKey, Dictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));
            Database.HashSet(redisKey, entries.ToArray(), commandFlags);
        }
        /// <summary>
        ///     Sets field in the hash stored at key to value. If key does not exist, a new key holding a hash is created. If field already exists in the hash, it is overwritten.
        /// </summary>
        /// 
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">The key of the hash in redis</param>
        /// <param name="key">The key of the field in the hash</param>
        /// <param name="nx">Behave like hsetnx - set only if not exists</param>
        /// <param name="value">The value to be inserted</param>
        /// <param name="commandFlags">Command execution flags</param>
        /// <returns>
        ///     <c>true</c> if field is a new field in the hash and value was set.
        ///     <c>false</c> if field already exists in the hash and no operation was performed.
        /// </returns>
        public async Task<bool> HashSetAsync<T>(string hashKey, string key, T value, bool nx = false, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            return await Database.HashSetAsync(redisKey, key, Serializer.Serialize(value), nx ? When.NotExists : When.Always, commandFlags);
        }

        /// <summary>
        ///     Sets the specified fields to their respective values in the hash stored at key. 
        ///     This command overwrites any existing fields in the hash. 
        ///     If key does not exist, a new key holding a hash is created.
        /// </summary>
        /// <remarks>
        ///     Time complexity: O(N) where N is the number of fields being set.
        /// </remarks>
        /// <typeparam name="T">Type of the returned value</typeparam>
        /// <param name="hashKey">Key of the hash</param>
        /// <param name="commandFlags">Command executions flags</param>
        /// <param name="values"></param>
        public async Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None)
        {
            var redisKey = GetRedisKey(hashKey);
            var entries = values.Select(kv => new HashEntry(kv.Key, Serializer.Serialize(kv.Value)));
            await Database.HashSetAsync(redisKey, entries.ToArray(), commandFlags);
        }
        #endregion
        
        #endregion

        #region Subscriber
        /// <summary>
        ///     Publishes a message to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public long Publish<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = m_connectionMultiplexer.GetSubscriber();
            return sub.Publish(channel, Serializer.Serialize(message), flags);
        }

        /// <summary>
        ///     Publishes a message to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task<long> PublishAsync<T>(RedisChannel channel, T message, CommandFlags flags = CommandFlags.None)
        {
            var sub = m_connectionMultiplexer.GetSubscriber();
            return await sub.PublishAsync(channel, await Serializer.SerializeAsync(message), flags);
        }

        /// <summary>
        ///     Registers a callback handler to process messages published to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Subscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null) throw new ArgumentNullException("" + handler + "");

            var sub = m_connectionMultiplexer.GetSubscriber();
            sub.Subscribe(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        ///     Registers a callback handler to process messages published to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async Task SubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null) throw new ArgumentNullException("" + handler + "");

            var sub = m_connectionMultiplexer.GetSubscriber();
            await sub.SubscribeAsync(channel, async (redisChannel, value) => await handler(Serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        ///     Unregisters a callback handler to process messages published to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void Unsubscribe<T>(RedisChannel channel, Action<T> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null) throw new ArgumentNullException("" + handler + "");

            var sub = m_connectionMultiplexer.GetSubscriber();
            sub.Unsubscribe(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        ///     Unregisters a callback handler to process messages published to a channel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="handler"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public async Task UnsubscribeAsync<T>(RedisChannel channel, Func<T, Task> handler, CommandFlags flags = CommandFlags.None)
        {
            if (handler == null) throw new ArgumentNullException("" + handler + "");

            var sub = m_connectionMultiplexer.GetSubscriber();
            await sub.UnsubscribeAsync(channel, (redisChannel, value) => handler(Serializer.Deserialize<T>(value)), flags);
        }

        /// <summary>
        ///     Unregisters all callback handlers on a channel.
        /// </summary>
        /// <param name="flags"></param>
        public void UnsubscribeAll(CommandFlags flags = CommandFlags.None)
        {
            var sub = m_connectionMultiplexer.GetSubscriber();
            sub.UnsubscribeAll(flags);
        }

        /// <summary>
        ///     Unregisters all callback handlers on a channel.
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public async Task UnsubscribeAllAsync(CommandFlags flags = CommandFlags.None)
        {
            var sub = m_connectionMultiplexer.GetSubscriber();
            await sub.UnsubscribeAllAsync(flags);
        }
        #endregion

        #region Redis System
        /// <summary>
        ///     Flushes the database.
        /// </summary>
        public void FlushDb()
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
            {
                Database.Multiplexer.GetServer(endpoint).FlushDatabase(Database.Database);
            }
        }

        /// <summary>
        ///     Flushes the database asynchronous.
        /// </summary>
        /// <returns></returns>
        public async Task FlushDbAsync()
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
            {
                await Database.Multiplexer.GetServer(endpoint).FlushDatabaseAsync(Database.Database);
            }
        }

        /// <summary>
        ///     Save the DB in background.
        /// </summary>
        /// <param name="saveType"></param>
        public void Save(SaveType saveType)
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
            {
                Database.Multiplexer.GetServer(endpoint).Save(saveType);
            }
        }

        /// <summary>
        ///     Save the DB in background asynchronous.
        /// </summary>
        /// <param name="saveType"></param>
        public async void SaveAsync(SaveType saveType)
        {
            var endPoints = Database.Multiplexer.GetEndPoints();

            foreach (var endpoint in endPoints)
            {
                await Database.Multiplexer.GetServer(endpoint).SaveAsync(saveType);
            }
        }

        /// <summary>
        ///     Gets the information about redis.
        ///     More info see http://redis.io/commands/INFO
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetInfo()
        {
            var info = Database.ScriptEvaluate("return redis.call('INFO')").ToString();

            return ParseInfo(info);
        }

        /// <summary>
        ///     Gets the information about redis.
        ///     More info see http://redis.io/commands/INFO
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetInfoAsync()
        {
            var info = (await Database.ScriptEvaluateAsync("return redis.call('INFO')")).ToString();

            return ParseInfo(info);
        }

        private Dictionary<string, string> ParseInfo(string info)
        {
            var lines = info.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            var data = new Dictionary<string, string>();
            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line[0] == '#')
                {
                    // 2.6+ can have empty lines, and comment lines
                    continue;
                }

                var idx = line.IndexOf(':');
                if (idx > 0) // double check this line looks about right
                {
                    var key = line.Substring(0, idx);
                    var infoValue = line.Substring(idx + 1).Trim();

                    data.Add(key, infoValue);
                }
            }

            return data;
        }
        #endregion
    }
}
