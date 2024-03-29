﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code.Security
{

    /// <summary>
    /// MD5加密
    /// </summary>
    public class Md5
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">加密字符</param>
        /// <param name="code">加密位数16/32</param>
        /// <returns></returns>
        public static string md5(string str, int code)
        {
            string strEncrypt = string.Empty;
            if (code == 16)
            {
                strEncrypt = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").Substring(8, 16);
            }

            if (code == 32)
            {
                strEncrypt = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            }

            return strEncrypt;
        }

        /// <summary>
        /// MD5加密方法
        /// </summary>
        /// <param name="olddata">传入要加密的串</param>
        /// <returns>返回加密后的串</returns>
        public static string EncryptMD5(string olddata)
        {
            return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(olddata, "MD5");
        }
    }
}
