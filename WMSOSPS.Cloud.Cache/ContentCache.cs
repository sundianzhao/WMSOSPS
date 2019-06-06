using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Cache
{
    public class ContentCache  // : IEnumerable
    {
        /// <summary>
        /// 单例模式下的默认缓存框架
        /// </summary>
        private static ContentCache _defaultCache;//创建缓存对象cache、memcache或redis

        private CacheClient client;
        public static ContentCache defaultCache()
        {
            if (_defaultCache == null || _defaultCache.client == null)
            {
                _defaultCache = new ContentCache();
            }
            return _defaultCache;
        }

        /// <summary>
        /// 默认缓存类型， 默认区
        /// </summary>
        public ContentCache()
        {
            client = new CacheClient().CreateInstance();//创建缓存对象cache、memcache或redis
        }
        /// <summary>
        /// 默认缓存类型，指定区
        /// </summary>
        /// <param name="_redisdb"></param>
        public ContentCache(DBEnum _redisdb)
        {
            client = new CacheClient().CreateInstance(_redisdb);//创建缓存对象cache、memcache或redis
        }
        /// <summary>
        /// 默认缓存类型，默认区
        /// </summary>
        /// <param name="_redisdb"></param>
        public ContentCache(CacheEnum CacheName)
        {
            client = new CacheClient(CacheName).CreateInstance();//创建缓存对象cache、memcache或redis
        }
        /// <summary>
        /// 指定缓存类型，指定区
        /// </summary>
        /// <param name="_redisdb"></param>
        public ContentCache(CacheEnum CacheName, DBEnum _redisdb)
        {
            client = new CacheClient(CacheName).CreateInstance(_redisdb);//创建缓存对象cache、memcache或redis
        }

        /// <summary>
        /// 获取统一格式（小写）KEY
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string GetCacheKey(string key)
        {
            return key.ToLower();
        }
        /// <summary>
        /// 添加缓存 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Set(string key, object value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot cache a null object.");
            client.Set(GetCacheKey(key), value, null);
        }

        public void Set<T>(string key, object value)
        {
            string serstr = JsonConvert.SerializeObject(value);
            if (value == null)
                throw new ArgumentNullException("value", "Cannot cache a null object.");
            client.Set<T>(GetCacheKey(key), serstr, null);
        }


        /// <summary>
        /// 添加缓存 指定缓存时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheDuration"></param>
        public void Set(string key, object value, CacheDuration cacheDuration)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Cannot cache a null object.");
            switch (cacheDuration)
            {
                case CacheDuration.None:
                    client.Set(GetCacheKey(key), value, DateTime.Now.AddMinutes((int)cacheDuration));
                    break;
                case CacheDuration.Hour:
                    client.Set(GetCacheKey(key), value, DateTime.Now.AddMinutes((int)cacheDuration));
                    break;
                case CacheDuration.Day:
                    client.Set(GetCacheKey(key), value, DateTime.Now.AddDays((int)cacheDuration));
                    break;
                case CacheDuration.Week:
                    client.Set(GetCacheKey(key), value, DateTime.Now.AddDays((int)cacheDuration));
                    break;
                case CacheDuration.Month:
                    client.Set(GetCacheKey(key), value, DateTime.Now.AddDays((int)cacheDuration));
                    break;
                default:
                    client.Set(GetCacheKey(key), value, DateTime.Now.AddMinutes((int)cacheDuration));
                    break;
            }
        }


        /// <summary>
        /// 添加缓存 指定缓存时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="cacheDuration"></param>
        public void Set<T>(string key, object value, CacheDuration cacheDuration)
        {
            string serstr = JsonConvert.SerializeObject(value);
            if (value == null)
                throw new ArgumentNullException("value", "Cannot cache a null object.");
            switch (cacheDuration)
            {
                case CacheDuration.None:
                    client.Set<T>(GetCacheKey(key), serstr, DateTime.Now.AddMinutes((int)cacheDuration));
                    break;
                case CacheDuration.Hour:
                    client.Set<T>(GetCacheKey(key), serstr, DateTime.Now.AddMinutes((int)cacheDuration));
                    break;
                case CacheDuration.Day:
                    client.Set<T>(GetCacheKey(key), serstr, DateTime.Now.AddDays((int)cacheDuration));
                    break;
                case CacheDuration.Week:
                    client.Set<T>(GetCacheKey(key), serstr, DateTime.Now.AddDays((int)cacheDuration));
                    break;
                case CacheDuration.Month:
                    client.Set(GetCacheKey(key), value, DateTime.Now.AddDays((int)cacheDuration));
                    break;
                default:
                    client.Set<T>(GetCacheKey(key), serstr, DateTime.Now.AddMinutes((int)cacheDuration));
                    break;
            }
        }


        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            return client.Get(GetCacheKey(key));
        }
        /// <summary>
        /// 获取指定类型的缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get<T>(string key)
        {
            return client.Get<T>(GetCacheKey(key));
        }
        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Remove(string key)
        {
            return client.Remove(GetCacheKey(key));
        }
        /// <summary>
        /// 哈希存储
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <param name="t"></param>
        public void HSet<T>(string hashId,string key,T t)
        {
            client.HSet<T>(hashId, key, t);
        }
        /// <summary>
        /// 哈希获取
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hashId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual object HGet<T>(string hashId,string key)
        {
            return client.HGet<T>(hashId, key);
        }
    }
}
