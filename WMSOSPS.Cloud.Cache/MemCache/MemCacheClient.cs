using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.MemCaching;
using Newtonsoft.Json;
using System;

namespace WMSOSPS.Cloud.Cache.MemCache
{
    public class MemCacheClient : CacheClient
    {
        MemcachedClient mc;
        public MemCacheClient()
        {
            mc = new MemcachedClient();
        }
        public override object Get<T>(string key)
        {
            //从memcache中读取数据
            object result = mc.Get(key);
            //memcache中没有值！！！
            if (result == null)
                return null;
            result = JsonConvert.DeserializeObject<T>((string)result);
            return result;
        }

        public override object Get(string key)
        {
            //从memcache中读取数据
            object result = mc.Get(key);
            //memcache中没有值！！！
            if (result == null)
                return null;
            #region 注释
            #endregion
            string ServerInfo = "<div style=\"display:none;\">MEM</div>";
            ServerInfo += "</body>";
            result = result.ToString().Replace("</body>", ServerInfo);
            return result;
        }

        public override void Set(string key, object value, DateTime? ExpireDateTime)
        {
            //只存储memcache
            bool success = mc.Store(Enyim.MemCaching.Memcached.StoreMode.Set, key, value.ToString());
        }

        public override void Set<T>(string key, object value, DateTime? ExpireDateTime)
        {
            //只存储memcache
            bool success = mc.Store(Enyim.MemCaching.Memcached.StoreMode.Set, key, value.ToString());
        }

        public override void Set<T>(string key, object value)
        {
            //只存储memcache
            bool success = mc.Store(Enyim.MemCaching.Memcached.StoreMode.Set, key, value.ToString());
        }

        public override void Set(string key, object value)
        {
            //只存储memcache
            bool success = mc.Store(Enyim.MemCaching.Memcached.StoreMode.Set, key, value.ToString());
        }

        public override object Remove(string key)
        {
            return mc.Remove(key);
        }


        [Serializable]
        protected class MemData
        {
            public Type Type;
            public string Data;
        }


    }
}
