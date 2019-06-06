using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Cache.RedisCache
{
    /// <summary>
    /// RedisHelper类主要是创建链接池管理对象的
    /// </summary>
    public class RedisHelper
    {
        /// <summary>
        /// redis配置文件信息
        /// </summary>
        private string writeServer = string.Empty;
        private string readServer = string.Empty;
        private PooledRedisClientManager _prcm;

        /// <summary>
        /// 静态构造方法，初始化链接池管理对象
        /// </summary>
        public RedisHelper(string _writeServer, string _readServer)
        {
            writeServer = _writeServer;
            readServer = _readServer;
            CreateManager(writeServer, readServer);
        }

        /// <summary>
        /// 创建链接池管理对象
        /// </summary>
        private void CreateManager(string _writeServer, string _readServer)
        {
            _prcm = CreateManager(new string[] { _writeServer }, new string[] { _readServer });
        }

        private PooledRedisClientManager CreateManager(string[] readWriteHosts, string[] readOnlyHosts)
        {
            PooledRedisClientManager redisClientManager = null;

            //WriteServerList：可写的Redis链接地址。
            //ReadServerList：可读的Redis链接地址。
            //MaxWritePoolSize：最大写链接数。
            //MaxReadPoolSize：最大读链接数。
            //AutoStart：自动重启。
            //LocalCacheTime：本地缓存到期时间，单位:秒。
            //RecordeLog：是否记录日志,该设置仅用于排查redis运行时出现的问题,如redis工作正常,请关闭该项。
            //RedisConfigInfo类是记录redis连接信息，此信息和配置文件中的RedisConfig相呼应
            // 支持读写分离，均衡负载 
            using (redisClientManager = new PooledRedisClientManager(readWriteHosts, readOnlyHosts, new RedisClientManagerConfig
            {
                MaxWritePoolSize = 1000, // “写”链接池链接数 
                MaxReadPoolSize = 1000, // “读”链接池链接数 
                AutoStart = true,
            }))

                return redisClientManager;
        }

        private static IEnumerable<string> SplitString(string strSource, string split)
        {
            return strSource.Split(split.ToArray());
        }

        /// <summary>
        /// 客户端缓存操作对象
        /// </summary>
        public RedisClient GetClient(DBEnum _db)
        {
            if (_prcm == null)
            {
                CreateManager(writeServer, readServer);
            }
            RedisClient redisclient = _prcm.GetClient() as RedisClient;
            if (redisclient.Db != (int)_db)
                redisclient.ChangeDb((int)_db);
            return redisclient;
        }
    }
}
