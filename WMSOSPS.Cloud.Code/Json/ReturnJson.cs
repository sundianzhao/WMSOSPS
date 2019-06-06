using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code.Json
{
    /// <summary>
    /// 返回json
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ReturnJson<T>
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public ReturnJson()
        {

        }
        /// <summary>
        /// 返回json
        /// </summary>
        /// <param name="res">操作是否成功状态</param>
        /// <param name="data">返回内容</param>
        public ReturnJson(string res, T data)
        {
            result = res;
            msg = data;
        }

        /// <summary>
        /// 返回内容
        /// </summary>
        public T msg { get; set; }
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public string result { get; set; }
    }
}
