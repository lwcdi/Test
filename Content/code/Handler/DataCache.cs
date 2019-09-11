using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Web
{
    /// <summary>
    /// 缓存处理类
    /// author liwu 2018-01-08
    /// </summary>
    public sealed class DataCache
    {
        /// <summary>
        /// 传入缓存键和缓存对象，设置缓存（相对过期时间 ）
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="cacheValue">缓存对象</param>
        /// <param name="seconds">过期时间(秒)</param>
        public static void SetCache(string cacheKey, object cacheValue, int seconds)
        {

            if (string.IsNullOrEmpty(cacheKey) || cacheValue == null)
            {
                return;
            }
            if (seconds <= 0)
            {
                seconds = 1;
            }
            HttpRuntime.Cache.Add(
                cacheKey,
                cacheValue,
                null,
                DateTime.Now.AddSeconds(seconds),
                System.Web.Caching.Cache.NoSlidingExpiration,
                System.Web.Caching.CacheItemPriority.Normal,
                null
                );
        }
        /// <summary>
        /// 传入缓存键和缓存对象，设置缓存（过期时间设置缓存后的10秒 ）
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <param name="cacheValue">缓存对象</param>
        public static void SetCache(string cacheKey, object cacheValue)
        {
            //TimeSpan ts = new TimeSpan(0, 0, 10);
            HttpRuntime.Cache.Add(
                cacheKey,
                cacheValue,
                null,
                DateTime.Now.AddSeconds(10),
                System.Web.Caching.Cache.NoSlidingExpiration,
                System.Web.Caching.CacheItemPriority.Normal,
                null
                );
        }
        /// <summary>
        /// 传入缓存键，得到缓存对象
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        /// <returns></returns>
        public static object GetCache(string cacheKey)
        {
            //得到缓存
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            //返回缓存的内容
            return objCache[cacheKey];
        }
        /// <summary>
        /// 传入缓存键，异常缓存对象
        /// </summary>
        /// <param name="cacheKey">缓存键</param>
        public static void ReMoveCatch(string cacheKey)
        {
            HttpRuntime.Cache.Remove(cacheKey);
        }
    }
}