using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code.Enum
{
    public enum OpType
    {
        /// <summary>
        /// 添加
        /// </summary>
        Add = 1,
        /// <summary>
        /// 编辑
        /// </summary>
        Edit = 2,
        /// <summary>
        /// 删除
        /// </summary>
        Del = 3,
        /// <summary>
        /// 登录
        /// </summary>
        Login = 4,
        /// <summary>
        /// 异常
        /// </summary>
        Error = 5,
        /// <summary>
        /// 提醒
        /// </summary>
        Warn = 6,
        /// <summary>
        /// 系统日志
        /// </summary>
        System = 7
        
    };
}
