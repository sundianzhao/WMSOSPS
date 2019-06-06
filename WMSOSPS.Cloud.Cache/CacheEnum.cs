using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Cache
{
    /// <summary>
    /// 缓存类型
    /// </summary>
    public enum CacheEnum
    {
        MemCache = 0,
        RedisCache = 1,
        LocalCache = 2,
    }

    /// <summary>
    /// 各站点对应的redis库
    /// </summary>
    public enum DBEnum
    { 
         用户信息 =1 
    }

    /// <summary>
    /// 过期时间
    /// </summary>
    public enum CacheDuration
    {
        None = 0,
        Hour = 60,
        TwoHour = 120,
        FourHour = 240,
        Day = 1,
        Week = 7,
        Month = 30
    };
}
