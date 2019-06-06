using WMSOSPS.Cloud.Cache.LocalCache;
using WMSOSPS.Cloud.Cache.MemCache;
using WMSOSPS.Cloud.Cache.RedisCache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WMSOSPS.Cloud.Cache
{
    public class CacheClient
    {
        /// <summary>
        /// 写日志对象
        /// </summary>
        protected static readonly log4net.ILog cacheLog = log4net.LogManager.GetLogger("CacheLog");
        public static readonly string XmlPath = "Configs/CacheConfig.xml"; //缓存配置文件
        protected bool IsTrue = false;//缓存配置文件
        protected string ReadServer = ""; //redis只读服务器，多个服务器之间使用逗号隔开
        protected string WriteServer = ""; //redis只写服务器，多个服务器之间使用逗号隔开
        protected CacheEnum IsCacheName = CacheEnum.RedisCache;//默认缓存类型
        protected DBEnum RedisDB = 0; //默认redis 库
        protected int RedisExpireTime;//redis默认过期时间(分钟)

        /// <summary>
        /// 指定缓存类型
        /// </summary>
        /// <param name="CacheName"></param>
        public CacheClient(CacheEnum CacheName = CacheEnum.RedisCache)
        {
            GetCacheConfig(CacheName);
        }
        /// <summary>
        /// 创建redis对象
        /// </summary>
        /// <returns></returns>
        public CacheClient CreateInstance()
        {
            CacheClient client = null;//获取Redis操作接口
            if (IsTrue)
            {
                switch (IsCacheName)
                {
                    case CacheEnum.RedisCache:
                        client = new RedisCacheClient((DBEnum)RedisDB);
                        break;
                    case CacheEnum.MemCache:
                        client = new MemCacheClient();
                        break;
                    case CacheEnum.LocalCache:
                        client = new LocalClient();
                        break;

                }
            }
            else
            {
                cacheLog.Info(IsCacheName.ToString() + "缓存未启用");
            }
            return client;
        }

        /// <summary>
        /// 创建指定库的redis对象
        /// </summary>
        /// <param name="_redisdb"></param>
        /// <returns></returns>
        public CacheClient CreateInstance(DBEnum _redisdb)
        {
            RedisDB = _redisdb;
            return CreateInstance();
        }

        /// <summary>
        /// 查询缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual object Get(string key)
        {
            cacheLog.Info($"Key:{key}, Operate:{"Get"}, Success:{"成功"}");
            return null;
        }

        /// <summary>
        /// 查询缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual object Get<T>(string key)
        {
            cacheLog.Info($"Key:{key}, Operate:{"Get"}, Success:{"成功"}");
            return null;
        }

        /// <summary>
        /// 设定指定过期时间的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual void Set(string key, object value, DateTime? ExpireDateTime)
        {
            cacheLog.Info($"Key:{key}, Operate:{"Add"}, Success:{"成功"}");
        }

        /// <summary>
        /// 设定指定过期时间的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual void Set<T>(string key, object value, DateTime? ExpireDateTime)
        {
            cacheLog.Info($"Key:{key}, Operate:{"Add"}, Success:{"成功"}");
        }

        /// <summary>
        /// 设定指定过期时间的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual void Set<T>(string key, object value)
        {
            cacheLog.Info($"Key:{key}, Operate:{"Add"}, Success:{"成功"}");
        }
        /// <summary>
        /// 设置默认过期时间的缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual void Set(string key, object value)
        {
            cacheLog.Info($"Key:{key}, Operate:{"Add"}, Success:{"成功"}");
        }

        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual object Remove(string key)
        {
            cacheLog.Info($"Key:{key}, Operate:{"Delete"}, Success:{"成功"}");
            return null;
        }


        /// <summary>
        /// 从xml文件获取redis配置信息
        /// </summary>
        /// <param name="path"></param>
        /// <param name="CacheName"></param>
        private void GetCacheConfig(CacheEnum CacheName)
        {
            XmlDocument xml = new XmlDocument();//声明xml
            xml.Load(AppDomain.CurrentDomain.BaseDirectory + XmlPath);
            XmlNodeList topM = xml.DocumentElement.ChildNodes;
            if (topM.Count < 0) return;
            foreach (XmlElement element in topM)
            {
                if (element.HasAttribute("Name") && element.Attributes["Name"].Value.ToLower() == CacheName.ToString().ToLower())
                {
                    IsTrue = element.SelectSingleNode("IsTrue").InnerText.Trim().ToLower() == "true" ? true : false;
                    ReadServer = element.SelectSingleNode("ReadServer").InnerText.Trim();
                    WriteServer = element.SelectSingleNode("WriteServer").InnerText.Trim(); ;
                    RedisDB = (DBEnum)Enum.Parse(typeof(DBEnum), element.SelectSingleNode("RedisDB").InnerText.Trim(), false);
                    RedisExpireTime = Convert.ToDateTime(element.SelectSingleNode("RedisExpireTime").InnerText.Trim()).Hour * 60 + Convert.ToDateTime(element.SelectSingleNode("RedisExpireTime").InnerText.Trim()).Minute;
                    IsCacheName = CacheName;
                }
            }
        }

        public virtual void HSet<T>(string hashId,string key, T t)
        {
            cacheLog.Info($"hashId:{hashId},Key:{key},Operate:{"Add"},Success:{"成功"}");
        }

        public virtual object HGet<T>(string hashId,string key)
        {
            cacheLog.Info($"hashId:{hashId},Key:{key},Operate:{"Get"},Success:{"成功"}");
            return null;
        }
    }
}
