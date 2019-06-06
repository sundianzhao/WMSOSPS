using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code.Tools
{
    /// <summary>
    ///API 通用返回对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReturnResult<T>
    {
        public string Message { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMsg { get; set; }

        public T JsonResult { get; set; }

        /// <summary>
        /// 判断是否有异常
        /// </summary>
        public bool IsOK
        {
            get
            {
                return string.IsNullOrEmpty(Message) && ErrorCode == 200;
            }
        }
    }
}
