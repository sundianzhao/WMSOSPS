using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Domain.ViewModel
{
    public class LoginContent
    {
        /// <summary>
        /// 是否需要通过手机短信验证
        /// </summary>
        public string IsSmsLogin { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 是否验证成功
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// 需要验证的手机号
        /// </summary>
        public string MoPhone { get; set; }
        /// <summary>
        /// 登录方式（1:平台管理员 2:代理商 3:企业）
        /// </summary>
        public int LoginType { get; set; }
    }
}
