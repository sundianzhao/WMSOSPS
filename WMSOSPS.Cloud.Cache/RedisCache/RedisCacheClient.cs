using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Redis;
using ServiceStack.Text;


namespace WMSOSPS.Cloud.Cache.RedisCache
{
    /// <summary>
    /// RedisCacheClient类，是redis操作的类，继承自IDisposable接口，主要用于释放内存
    /// </summary>
    public class RedisCacheClient : CacheClient, IDisposable
    {
        protected RedisHelper RedisHelper { get; private set; }
        protected DBEnum db { get; private set; }
        private bool _disposed = false;
        private const string EXCEPTION_MSG = "Redis操作失败";
        /// <summary>
        /// redis客户端构造方法
        /// </summary>
        /// <param name="_writeServer">redis写服务器</param>
        /// <param name="_readServer">redis读服务器</param>
        /// <param name="_db">指定redis库</param>
        public RedisCacheClient(DBEnum _db)
        {
            RedisHelper = new RedisHelper(WriteServer, ReadServer);
            db = _db;
        }

        public RedisCacheClient()
        {
            RedisHelper = new RedisHelper(WriteServer, ReadServer);
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ExpireDateTime"></param>
        public override void Set(string key, object value, DateTime? ExpireDateTime)
        {
            try
            {
                using (IRedisClient IRedisClient = RedisHelper.GetClient(db))
                {
                    //没有设置过期时间，则过期时间为配置的默认过期时间
                    if (ExpireDateTime == null)
                    {
                        ExpireDateTime = DateTime.Now.AddMinutes(RedisExpireTime);
                    }

                    bool result = IRedisClient.Set<object>(key, value, Convert.ToDateTime(ExpireDateTime));
                    cacheLog.Info(string.Format("Key:{0}, Operate:{1}, Success:{2}", key, "Add", result));
                }

            }
            catch (Exception ex)
            {
                cacheLog.Info(string.Format("Key:{0}, Operate:{1}, Error", key, "Add"));
                throw new Exception(EXCEPTION_MSG, ex);
            }
        }

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ExpireDateTime"></param>
        public override void Set<T>(string key, object value, DateTime? ExpireDateTime)
        {
            try
            {
                using (IRedisClient IRedisClient = RedisHelper.GetClient(db))
                {
                    //没有设置过期时间，则过期时间为配置的默认过期时间
                    if (ExpireDateTime == null)
                    {
                        ExpireDateTime = DateTime.Now.AddMinutes(RedisExpireTime);
                    }

                    bool result = IRedisClient.Set<object>(key, value, Convert.ToDateTime(ExpireDateTime));
                    cacheLog.Info(string.Format("Key:{0}, Operate:{1}, Success:{2}", key, "Add", result));
                }
            }
            catch (Exception ex)
            {
                cacheLog.Info(string.Format("Key:{0}, Operate:{1}, Error", key, "Add"));
                throw new Exception(EXCEPTION_MSG, ex);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object Get(string key)
        {
            try
            {
                using (IRedisClient IRedisClient = RedisHelper.GetClient(db))
                {
                    object value = IRedisClient.Get<object>(key);
                    if (value != null)
                    {
                        string ServerInfo = "<div style=\"display:none;\">redis</div>";
                        ServerInfo += "</body>";
                        value = value.ToString().Replace("</body>", ServerInfo);
                        cacheLog.Info(string.Format("Key:{0}, Operate:{1}, Success:{2}", key, "GET", value));
                    }

                    return value;
                }
            }
            catch (Exception ex)
            {
                cacheLog.Info(string.Format("Key:{0}, Operate:{1}, Error", key, "GET"));
                throw new Exception(EXCEPTION_MSG, ex);
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object Get<T>(string key)
        {
            try
            {
                using (IRedisClient IRedisClient = RedisHelper.GetClient(db))
                {
                    object value = IRedisClient.Get<object>(key);
                    if (value != null)
                    {
                        cacheLog.Info(string.Format("Key:{0}, Operate:{1}, Success:{2}", key, "GET", value));
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>((string)value);
                    }
                    return value;
                }
            }
            catch (Exception ex)
            {
                cacheLog.Info(string.Format("Key:{0}, Operate:{1}, Error", key, "GET"));
                throw new Exception(EXCEPTION_MSG, ex);
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object Remove(string key)
        {
            try
            {
                using (IRedisClient IRedisClient = RedisHelper.GetClient(db))
                {
                    bool result = IRedisClient.Remove(key);
                    cacheLog.Info(string.Format("Key:{0}, Operate:{1}, Success:{2}", key, "Remove", result));
                    return result;
                }
            }
            catch (Exception ex)
            {
                cacheLog.Info(string.Format("Key:{0}, Operate:{1}, Error", key, "REMOVE"));
                throw new Exception(EXCEPTION_MSG, ex);
            }
        }

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        public bool HExist<T>(string hashId, string key)
        {
            return RedisHelper.GetClient(db).HashContainsEntry(hashId, key);
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        public override void HSet<T>(string hashId, string key, T t)
        {
            try
            {
                using (IRedisClient IRedisClient=RedisHelper.GetClient(db))
                {
                    var value = JsonSerializer.SerializeToString<T>(t);
                    IRedisClient.SetEntryInHash(hashId, key, value);
                }
            }
            catch (Exception ex)
            {
                cacheLog.Error(string.Format("hashId:{0},Key:{1},Operate:{2},Error:{3}",hashId,key,"Add",ex));
                throw new Exception(EXCEPTION_MSG,ex);
            }
           
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        public bool HRemove(string hashId, string key)
        {
            return RedisHelper.GetClient(db).RemoveEntryFromHash(hashId, key);
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        public override object HGet<T>(string hashId, string key)
        {
            string value = RedisHelper.GetClient(db).GetValueFromHash(hashId, key);
            return JsonSerializer.DeserializeFromString<T>(value);
        }

        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        public List<T> HGetAll<T>(string hashId)
        {
            var result = new List<T>();
            var list = RedisHelper.GetClient(db).GetHashValues(hashId);
            if (list != null && list.Count() > 0)
            {
                list.ForEach(x =>
                {
                    var value = JsonSerializer.DeserializeFromString<T>(x);
                    result.Add(value);
                });
            }
            return result;
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        public void SetExpire(string key, DateTime datetime)
        {
            RedisHelper.GetClient(db).ExpireEntryAt(key, datetime);
        }
        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    RedisHelper.GetClient(db).Dispose();
                }
            }
            this._disposed = true;
        }

        /// <summary>
        /// 销毁redis对象释放内存
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 保存数据DB文件到硬盘
        /// </summary>
        public void Save()
        {
            RedisHelper.GetClient(db).Save();
        }

        /// <summary>
        /// 异步保存数据DB文件到硬盘
        /// </summary>
        public void SaveAsync()
        {
            RedisHelper.GetClient(db).SaveAsync();
        }

    }
}
