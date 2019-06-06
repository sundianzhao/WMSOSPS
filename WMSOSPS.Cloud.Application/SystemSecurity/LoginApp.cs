using WMSOSPS.Cloud.Cache;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Net;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Security;
using WMSOSPS.Cloud.Code.Tools;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.ViewModel;
using WMSOSPS.Cloud.Repository.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Application.SystemSecurity
{
    public class LoginApp : IDisposable
    {
        private readonly LoginRepository _repository = new LoginRepository();

        public void Dispose()
        {
            if (_repository != null)
            {
                _repository.Dispose();
            }
        }
        /// <summary>
        /// 管理员登陆
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public LoginContent CheckLogin(string UserName, string pwd)
        {
            OperatorProvider.Provider.RemoveCurrent();
            OperatorModel operatorModel = new OperatorModel();
            LoginContent content = new LoginContent()
            {
                IsSmsLogin = "0",
                Msg = "用户名密码错误",
                Success = false,
                MoPhone = "",
                LoginType = (int)LoginTypeEnum.ManageMember
            };

            var encodeUserName = Utility.EncodeStr(UserName);

            string thisIp = Net.Ip;
            //var location = Tool.GetIpLocation(thisIp);
            //if (location.Trim() == "局域网" || thisIp == "127.0.0.1")
            //{
            //    thisIp = "112.253.8.147";//山东省潍坊市联通
            //}
            thisIp = "112.253.8.147";//山东省潍坊市联通
            operatorModel.CurrentIp = thisIp;
            //获取平台管理员
            string passWord = Md5.md5(pwd, 32);//将密码加密 
           
            #region 平台管理员
            Sys_User userInfo = _repository.FindEntity<Sys_User>(a => a.F_Account == UserName);
            if (userInfo != null)
            {
                if (userInfo.F_EnabledMark == false)
                {
                    content.Success = false;
                    content.Msg = "该账号已被禁用，请联系管理员！";
                    return content;
                }
                var userloginOn = _repository.FindEntity<Sys_UserLogOn>(a => a.F_UserId == userInfo.F_Id);
                string dbPassword = Md5.md5(DESEncrypt.Encrypt(passWord.ToLower(), userloginOn.F_UserSecretkey).ToLower(), 32).ToLower();

                if (dbPassword != userloginOn.F_UserPassword)
                {
                    content.Success = false;
                    return content;
                }
                content.LoginType = (int)LoginTypeEnum.ManageMember;
                content.Success = true;
                content.IsSmsLogin = "0";
                content.Msg = "登录成功";
                content.MoPhone = userInfo.F_MobilePhone;
               
                Sys_Role roleInfo = _repository.FindEntity<Sys_Role>(p => p.F_Id == userInfo.F_RoleId);

                operatorModel.LoginTime = DateTime.Now;
                operatorModel.LastIp = thisIp;
                int count = userloginOn.F_LogOnCount ?? 0 + 1;
                userloginOn.F_LogOnCount = count;  
                userloginOn.F_LastVisitTime = DateTime.Now;

                int flag = _repository.Update(userloginOn);

                #region 存储session
                operatorModel.UserId = userInfo.F_Id.ToString();
                operatorModel.UserCode = userInfo.F_Account;
                operatorModel.UserName = userInfo.F_RealName;
                operatorModel.CompanyId = userInfo.F_OrganizeId;
                operatorModel.DepartmentId = userInfo.F_DepartmentId;
                operatorModel.RoleId = ConvertVal.GetValStr(userInfo.F_RoleId);
                operatorModel.LoginIPAddress = Net.Ip;
                operatorModel.LoginIPAddressName = Net.GetLocation(operatorModel.LoginIPAddress);
                operatorModel.LoginTime = DateTime.Now;
                operatorModel.LoginToken = userInfo.F_Account;
                operatorModel.LoginFlag = Common.GuId();
                operatorModel.OrganizeModel = GetOrganize(userInfo.F_OrganizeId);
                operatorModel.Sex = userInfo.F_Gender;
                operatorModel.DutyId = userInfo.F_DutyId;
                operatorModel.F_Birthday = userInfo.F_Birthday;
                operatorModel.F_BelongTo = userInfo.F_EntrCode;

                operatorModel.Phone = Utility.DecodeStr(userInfo.F_MobilePhone);
                operatorModel.Email = userInfo.F_Email;
                operatorModel.IsSystem = false;

                if (userInfo.F_Account == "admin")
                {
                    operatorModel.IsSystem = true;
                } 
                operatorModel.LoginType = LoginTypeEnum.ManageMember; 
                OperatorProvider.Provider.AddCurrent(operatorModel);
                //var redis = new ContentCache(DBEnum.用户信息);
               // redis.HSet<OperatorModel>("Login_ManagerUserInfo", operatorModel.UserCode, operatorModel);

               // LogHelper.Info("帐号：" + UserName + ",登录系统IP:" + thisIp, OpType.Login, null, "LoginApp-CheckLogin", "", ViewLevel.Admin);
                #endregion 
                content.Success = true;
                content.Msg = "登录成功";
                return content;
            }
            #endregion

            #region 用户名密码错误，登录失败
            LogHelper.Warn("帐号：" + UserName + ",登录系统失败IP:" + thisIp, OpType.Login, null, "LoginApp-CheckLogin", "", ViewLevel.Admin);
            // Tool.InsertLogLogin(userName, userName, "帐号：" + userName + ",登录系统IP:" + thisIp + ",密码：" + pwd, OpType.Login, 0, 4, "SYS_Service--LoginPass", Tool.GetAddressBYIP(thisIp));
            #endregion
            return content;

        }

        public Organize GetOrganize(string f_OrganizeId)
        {
            Sys_Organize sys_organize = _repository.FindEntity<Sys_Organize>(p => p.F_Id == f_OrganizeId);
            var organize = new Organize();
            if (sys_organize != null)
            {
                if (!string.IsNullOrEmpty(sys_organize.F_Address))
                    organize.F_Address = sys_organize.F_Address;
                if (!string.IsNullOrEmpty(sys_organize.F_AreaId))
                    organize.F_AreaId = sys_organize.F_AreaId;
                if (!string.IsNullOrEmpty(sys_organize.F_CategoryId))
                    organize.F_CategoryId = sys_organize.F_CategoryId;
                organize.F_CreatorTime = sys_organize.F_CreatorTime;
                if (!string.IsNullOrEmpty(sys_organize.F_CreatorUserId))
                    organize.F_CreatorUserId = sys_organize.F_CreatorUserId;
                if (!string.IsNullOrEmpty(sys_organize.F_DeleteUserId))
                    organize.F_DeleteUserId = sys_organize.F_DeleteUserId;
                if (!string.IsNullOrEmpty(sys_organize.F_Description))
                    organize.F_Description = sys_organize.F_Description;
                if (!string.IsNullOrEmpty(sys_organize.F_Email))
                    organize.F_Email = sys_organize.F_Email;
                //organize.F_EnabledMark = sys_organize.F_EnabledMark;
                if (!string.IsNullOrEmpty(sys_organize.F_EnCode))
                    organize.F_EnCode = sys_organize.F_EnCode;
                if (!string.IsNullOrEmpty(sys_organize.F_Fax))
                    organize.F_Fax = sys_organize.F_Fax;
                if (!string.IsNullOrEmpty(sys_organize.F_FullName))
                    organize.F_FullName = sys_organize.F_FullName;
                organize.F_Id = sys_organize.F_Id;
                if (sys_organize.F_LastModifyTime != null)
                    organize.F_LastModifyTime = sys_organize.F_LastModifyTime;
                if (!string.IsNullOrEmpty(sys_organize.F_LastModifyUserId))
                    organize.F_LastModifyUserId = sys_organize.F_LastModifyUserId;
                if (sys_organize.F_Layers != null)
                    organize.F_Layers = sys_organize.F_Layers;
                if (!string.IsNullOrEmpty(sys_organize.F_ManagerId))
                    organize.F_ManagerId = sys_organize.F_ManagerId;
                if (!string.IsNullOrEmpty(sys_organize.F_MobilePhone))
                    organize.F_MobilePhone = sys_organize.F_MobilePhone;
                if (!string.IsNullOrEmpty(sys_organize.F_ParentId))
                    organize.F_ParentId = sys_organize.F_ParentId;
                if (!string.IsNullOrEmpty(sys_organize.F_ShortName))
                    organize.F_ShortName = sys_organize.F_ShortName;
                if (sys_organize.F_SortCode != null)
                    organize.F_SortCode = sys_organize.F_SortCode;
                if (!string.IsNullOrEmpty(sys_organize.F_TelePhone))
                    organize.F_TelePhone = sys_organize.F_TelePhone;
                if (!string.IsNullOrEmpty(sys_organize.F_WeChat))
                    organize.F_WeChat = sys_organize.F_WeChat;
            }
            return organize;
        }
    }
}
