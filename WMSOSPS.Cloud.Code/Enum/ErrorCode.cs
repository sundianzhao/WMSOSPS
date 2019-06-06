using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code.Enum
{
    public enum ErrorCode
    {
        成功 = 200,
        系统错误 = -1,
        自定义错误 = -2
    }

    /// <summary>
    /// 错误类型
    /// </summary>
    public enum EError
    {
        /// <summary>
        /// 没有错误
        /// </summary>
        NoError = 0,
        /// <summary>
        /// 一般错误
        /// </summary>
        Normal = 1,
        /// <summary>
        /// 未登录
        /// </summary>
        NoLogin = 2,
        /// <summary>
        /// 没有权限
        /// </summary>
        NoRight = 3,
        /// <summary>
        /// 请求fn错误
        /// </summary>
        Request = 4,
        /// <summary>
        /// 异地登录
        /// </summary>
        RemoteLogin = 5,
        /// <summary>
        /// 用户信息被替换
        /// </summary>
        ResetUser = 6
    }
}
