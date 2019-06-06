using Enyim.Caching;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace WMSOSPS.Cloud.Cache.LocalCache
{
    public class LocalClient : CacheClient
    {
        private static System.Web.Caching.Cache _Cache;
        MemcachedClient mc = new MemcachedClient();
        // protected static readonly log4net.ILog cacheLog = log4net.LogManager.GetLogger("CacheLog");

        public LocalClient()
        {
            _Cache = HttpContext.Current.Cache;

        }

        protected LocalClient(System.Web.Caching.Cache cache)
        {
            _Cache = cache;
        }

        /// <summary>
        /// Retrieves the specified item from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public override object Get(string key)
        {
            object result = _Cache.Get(key);
            if (result == null)
            {
                cacheLog.Info("索引" + key + "在本地内存中没有值");
                return null;
            }
            string ServerInfo = "<div style=\"display:none;\">LOCAL</div>";
            ServerInfo += "</body>";
            result = result.ToString().Replace("</body>", ServerInfo);
            return result;
        }

        public override object Get<T>(string key)
        {
            object result = _Cache.Get(key);
            if (result == null)
            {
                cacheLog.Info("索引" + key + "在本地内存中没有值");
                return null;
            }
            result = JsonConvert.DeserializeObject<T>((string)result);
            return result;
        }

        /// <summary>
        /// Store the data, overwrite if already exist
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override void Set(string key, object value, DateTime? ExpireDateTime)
        {
            //只存入本地内存
            if (ExpireDateTime != null)
            {
                _Cache.Insert(key, value, null, Convert.ToDateTime(ExpireDateTime), TimeSpan.Zero, CacheItemPriority.Normal, null);
            }
            else
            {
                _Cache.Insert(key, value);
            }
        }
        public override void Set(string key, object value)
        {
            Set(key, value, null);
        }

        public override void Set<T>(string key, object value)
        {
            Set(key, value, null);
        }


        public override void Set<T>(string key, object value, DateTime? ExpireDateTime)
        {
            //只存入本地内存
            if (ExpireDateTime != null)
            {
                _Cache.Insert(key, value, null, Convert.ToDateTime(ExpireDateTime), TimeSpan.Zero, CacheItemPriority.Normal, null);
            }
            else
            {
                _Cache.Insert(key, value);
            }
        }





        /// <summary>
        /// Removes the specified item from the cache.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual object Remove(string key)
        {
            return _Cache.Remove(key);
        }
    }
}
