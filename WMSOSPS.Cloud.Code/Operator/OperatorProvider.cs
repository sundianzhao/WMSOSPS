 
using System.Collections.Generic;
using WMSOSPS.Cloud.Cache;
using WMSOSPS.Cloud.Code.Json;
using WMSOSPS.Cloud.Code.Security;
using WMSOSPS.Cloud.Code.Web;

namespace WMSOSPS.Cloud.Code.Operator
{
    public class OperatorProvider
    {
        public static OperatorProvider Provider
        {
            get { return new OperatorProvider(); }
        }
        private string LoginUserKey = "CurrentUser";
        private string LoginProvider = Configs.Configs.GetValue("LoginProvider");
        //用于存放登陆过的用户和标示
        public static Dictionary<string, string> UserList = new Dictionary<string, string>();
        public OperatorModel GetCurrent()
        {
            OperatorModel operatorModel = new OperatorModel();
            if (LoginProvider == "Cookie")
            {
                var cookieModel = WebHelper.GetCookie(LoginUserKey);
                if (string.IsNullOrEmpty(cookieModel))
                    return operatorModel;
                operatorModel = DESEncrypt.Decrypt(WebHelper.GetCookie(LoginUserKey).ToString()).ToObject<OperatorModel>();
            }
            else
            {
                var sessionModel = WebHelper.GetSession(LoginUserKey);
                if (string.IsNullOrEmpty(sessionModel))
                    return operatorModel;
                operatorModel = DESEncrypt.Decrypt(WebHelper.GetSession(LoginUserKey).ToString()).ToObject<OperatorModel>();
            }
            return operatorModel;
        }
        public void AddCurrent(OperatorModel operatorModel)
        {
            operatorModel.LoginFlag = System.DateTime.Now.ToString("yyyyMMddHHmmss");
            string mid = operatorModel.UserCode;
            if (!string.IsNullOrEmpty(operatorModel.AssistantID))
                mid = operatorModel.AssistantID;
            if (UserList.ContainsKey(mid))
                UserList[mid] = operatorModel.LoginFlag;
            else
                UserList.Add(mid, operatorModel.LoginFlag);


            if (LoginProvider == "Cookie")
            {
                //WebHelper.WriteCookie(LoginUserKey, DESEncrypt.Encrypt(operatorModel.ToJson()), 2);
                WebHelper.WriteCookie("token", operatorModel.LoginToken, 30);
                WebHelper.WriteCookie("LoginFlag", operatorModel.LoginFlag, 30);
            }
            else
            {
                WebHelper.WriteSession(LoginUserKey, DESEncrypt.Encrypt(operatorModel.ToJson()));
            }
            WebHelper.WriteCookie("nfine_mac", Md5.md5(Net.Net.GetMacByNetworkInterface().ToJson(), 32));
            WebHelper.WriteCookie("nfine_licence", Licence.GetLicence());
        }
        public void RemoveCurrent()
        {
            if (LoginProvider == "Cookie")
            {
                WebHelper.RemoveCookie(LoginUserKey.Trim());
            }
            else
            {
                WebHelper.RemoveSession(LoginUserKey.Trim());
            }
        }

        /// <summary>
        /// 验证用户当前状态、以及权限
        /// </summary>
        /// <returns></returns>
        public static Enum.EError CheckRight(OperatorModel op)
        {
            if (!string.IsNullOrEmpty(op.UserCode))
            {
                string mid = op.UserCode;
                if (!string.IsNullOrEmpty(op.AssistantID))
                    mid = op.AssistantID;
                if (UserList.ContainsKey(mid) && UserList[mid] != op.LoginFlag)
                {
                    if (Configs.Configs.GetValue("LoginProvider") == "Cookie")
                    {
                        WebHelper.RemoveCookie(Configs.Configs.GetValue("LoginProvider").Trim());
                    }
                    else
                    {
                        WebHelper.RemoveSession(Configs.Configs.GetValue("LoginProvider").Trim());
                    }
                    return Enum.EError.RemoteLogin;
                }
                #region 若无问题 则重新延长cookie、session 
                if (Configs.Configs.GetValue("LoginProvider") == "Cookie")
                {
                    WebHelper.WriteCookie("token", op.LoginToken, 30);
                    WebHelper.WriteCookie("LoginFlag", op.LoginFlag, 30);
                }
                else
                {
                    WebHelper.WriteSession("CurrentUser", DESEncrypt.Encrypt(op.ToJson()));
                }
                #endregion
                return Enum.EError.NoError;
            }
            else
            {
                return Enum.EError.NoLogin;
            } 
        }

        /// <summary>
        /// 获取redis缓存数据
        /// </summary>
        /// <returns></returns>
        public OperatorModel GetCurrent_bf()
        {
            OperatorModel operatorModel = new OperatorModel();
            string token = WebHelper.GetCookie("token");
            if (string.IsNullOrEmpty(token))
            {
                return operatorModel;
            }
            var redis = new ContentCache(DBEnum.用户信息);
            operatorModel = (OperatorModel)redis.HGet<OperatorModel>("Login_ManagerUserInfo", token);
            if (operatorModel== null)
            {
                operatorModel = new OperatorModel();
            }
            return operatorModel;
        }
    }
}
