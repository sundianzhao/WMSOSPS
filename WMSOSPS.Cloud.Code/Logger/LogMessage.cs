using System;
using System.Reflection;
using log4net.Layout;
using log4net.Layout.Pattern;

namespace WMSOSPS.Cloud.Code.Logger
{
    /// <summary>
    /// 日志内容
    /// </summary>
    public class LogMessage
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public Exception Exception { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户ip
        /// </summary>
        public string UserIP { get; set; }

        /// <summary>
        /// 当前url
        /// </summary>
        public string CurrentUrl { get; set; }

        /// <summary>
        /// 上一个url
        /// </summary>
        public string PrevUrl { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public int OperationType { get; set; }

        /// <summary>
        /// 被操作对象
        /// </summary>
        public string ToObjct { get; set; }

        /// <summary>
        /// 查看等级
        /// </summary>
        public int ViewLevel { get; set; }

        /// <summary>
        /// 模块类型
        /// </summary>
        public int ModelType { get; set; }

        /// <summary>
        /// 触发的方法
        /// </summary>
        public string FromMethod { get; set; }

        /// <summary>
        /// ip对应城市
        /// </summary>
        public string IPCity { get; set; }

        
    }
}
