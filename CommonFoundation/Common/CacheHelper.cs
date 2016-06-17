using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Text.RegularExpressions;

namespace CommonFoundation.Common
{
    public class CacheHelper
    {
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        public static void RemoveCache(string Key)
        {
            Cache Cache = HttpRuntime.Cache;
            if (Cache[Key] != null)
            {
                Cache.Remove(Key);
            }
        }

        public static void RemoveCacheByPattern(string pattern)
        {
            Cache Cache = HttpRuntime.Cache;
            IDictionaryEnumerator enumerator = Cache.GetEnumerator();
            Regex rgx = new Regex(pattern, (RegexOptions.Singleline | (RegexOptions.Compiled | RegexOptions.IgnoreCase)));
            while (enumerator.MoveNext())
            {
                if (rgx.IsMatch(enumerator.Key.ToString()))
                {
                    Cache.Remove(enumerator.Key.ToString());
                }
            }
        }
    }
}
