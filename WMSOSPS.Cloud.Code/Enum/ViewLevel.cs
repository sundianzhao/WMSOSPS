using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code.Enum
{
    public enum ViewLevel
    {
        /// <summary>
        /// 所有人可见
        /// </summary>
        AllVisible = 0,
        /// <summary>
        /// 只允许管理员可见
        /// </summary>
        Admin = 1, 
        /// <summary>
        /// 只允许技术可见
        /// </summary>
        Technicist = 2
    }
}
