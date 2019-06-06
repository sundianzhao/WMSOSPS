/*******************************************************************************
*********************************************************************************/

using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions; 
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Security;
using WMSOSPS.Cloud.Code.SqlHelper;
using WMSOSPS.Cloud.Code.Tools;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.SystemManage; 
using WMSOSPS.Cloud.Domain.IRepository.SystemManage; 
using WMSOSPS.Cloud.Repository.Login;
using WMSOSPS.Cloud.Repository.SystemManage; 

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class UserApp : IDisposable
    {
        private IUserRepository service = new UserRepository();

        private UserLogOnApp userLogOnApp = new UserLogOnApp();
        private readonly LoginRepository _repository = new LoginRepository();

        public void Dispose()
        {
            if (service != null)
            {
                service.Dispose();
            }
            if (_repository != null)
                _repository.Dispose();
        }


        public List<Sys_User> GetList(Pagination pagination, string keyword, string roleid)
        {
            var op = OperatorProvider.Provider.GetCurrent();
            var expression = ExtLinq.True<Sys_User>();
            if (!op.IsSystem)
            {
                expression = expression.And(p => p.F_EntrCode == op.F_BelongTo);
            } 
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_Account.Contains(keyword));
                expression = expression.Or(t => t.F_RealName.Contains(keyword));
                expression = expression.Or(t => t.F_MobilePhone.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(roleid))
            {
                expression = expression.And(t => t.F_RoleId == roleid);
            }
            expression = expression.And(t => t.F_Account != "admin");
            var userlst = service.FindList(expression, pagination);
            //foreach (var item in userlst)
            //{
            //    item.F_MobilePhone = WMSOSPS.Cloud.Code.Tools.Utility.DecodeStr(item.F_MobilePhone);
            //}
            return userlst;
        }

         

        //public wx_PhoneNumber  GetWxPhoneNumber(string enterpriseId, string workerNumber)
        //{
        //    var data =
        //        _repository.FindEntity<wx_PhoneNumber>(
        //            a => a.EnterpriseID == enterpriseId && a.WorkerNumber == workerNumber);
        //    return data;
        //}

        public Sys_User GetForm(string keyValue)
        {
            var user = service.FindEntity<Sys_User>(keyValue);
            if (user != null)
            {
               /// user.F_MobilePhone = WMSOSPS.Cloud.Code.Tools.Utility.DecodeStr(user.F_MobilePhone);
            }
            return user;
        }


        public void DeleteForm(string keyValue)
        {
            service.DeleteForm(keyValue);
        }
        public void SubmitForm(Sys_User userEntity, Sys_UserLogOn userLogOnEntity, string keyValue)
        {
            var role = _repository.FindEntity<Sys_Role>(p => p.F_Id == userEntity.F_RoleId);
              
            if (!string.IsNullOrEmpty(keyValue))
            {
                //userEntity.Modify(keyValue);
                userEntity.F_Id = keyValue;
                userEntity.F_LastModifyTime = DateTime.Now;
                userEntity.F_LastModifyUserId = OperatorProvider.Provider.GetCurrent().UserId; 
                var op = OperatorProvider.Provider.GetCurrent();
                LogHelper.Info("用户信息：【" + keyValue + "】修改！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
              
            }
            else
            {
                #region Sys_User表
                //userEntity.Create();
                userEntity.F_Id = Common.GuId();
                userEntity.F_CreatorUserId = OperatorProvider.Provider.GetCurrent().UserId;
                userEntity.F_CreatorTime = DateTime.Now;
                userEntity.F_LastModifyTime = DateTime.Now;  
                var op = OperatorProvider.Provider.GetCurrent();
                LogHelper.Info("用户信息：【" + keyValue + "】新增！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
                #endregion

            }
            service.SubmitForm(userEntity, userLogOnEntity, keyValue);
        }

       

        public int DeleteNumber(int nId)
        {
            return service.DeleteNumber(nId);
        }

        public void UpdateForm(Sys_User userEntity)
        {
            service.Update(userEntity);
        }
        public Sys_User CheckLogin(string username, string password)
        {
            Sys_User userEntity = service.FindEntity<Sys_User>(t => t.F_Account == username);
            if (userEntity != null)
            {
                if (userEntity.F_EnabledMark == true)
                {
                    Sys_UserLogOn userLogOnEntity = userLogOnApp.GetForm(userEntity.F_Id);
                    string dbPassword = Md5.md5(DESEncrypt.Encrypt(password.ToLower(), userLogOnEntity.F_UserSecretkey).ToLower(), 32).ToLower();
                    if (dbPassword == userLogOnEntity.F_UserPassword)
                    {
                        DateTime lastVisitTime = DateTime.Now;
                        int LogOnCount = (userLogOnEntity.F_LogOnCount).ToInt() + 1;
                        if (userLogOnEntity.F_LastVisitTime != null)
                        {
                            userLogOnEntity.F_PreviousVisitTime = userLogOnEntity.F_LastVisitTime.ToDate();
                        }
                        userLogOnEntity.F_LastVisitTime = lastVisitTime;
                        userLogOnEntity.F_LogOnCount = LogOnCount;
                        userLogOnApp.UpdateForm(userLogOnEntity);
                        return userEntity;
                    }
                    else
                    {
                        throw new Exception("密码不正确，请重新输入");
                    }
                }
                else
                {
                    throw new Exception("账户被系统锁定,请联系管理员");
                }
            }
            else
            {
                throw new Exception("账户不存在，请重新输入");
            }
        }

         

        public bool IsUserAllByPhone(string keyvalue, string phonenumber)
        {
            Sys_User sysuser = _repository.FindEntity<Sys_User>(p => p.F_Id == keyvalue);
            if (sysuser.F_MobilePhone == phonenumber)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void DisabledAccount(Sys_User sys_user)
        {
            service.DisabledAccount(sys_user);
        }

      

        /// <summary>
        /// 检查帐号是否可用
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public object CheckUser(string userCode)
        {
            var cacheInfo = OperatorProvider.Provider.GetCurrent();
            var count = _repository.IQueryableAsNoTracking<Sys_User>().Count(p => p.F_Account != cacheInfo.UserCode);
            if (count > 0)
            {
                return new { state = false, message = "已存在该帐号！" };
            }
            else
            {
                return new { state = true };
            }
        }
       
        public object UpdadteUserPwd(string pwd)
        {
            var Password = Md5.md5(pwd, 32); 
            var cacheInfo = OperatorProvider.Provider.GetCurrent();//获取当前登陆用户信息
            if (string.IsNullOrEmpty(cacheInfo.UserCode))
            {
                return new { state = "error", message = "当前登录过期，请刷新当前页面" };
            }
            if (string.IsNullOrEmpty(pwd))
            {
                return new { state = "error", message = "参数不正确" };
            }
            try
            {
               
                if (cacheInfo.LoginType == LoginTypeEnum.ManageMember)
                {
                    var userloginOn = _repository.FindEntity<Sys_UserLogOn>(a => a.F_UserId == cacheInfo.UserId);
                    userloginOn.F_UserSecretkey = Md5.md5(Common.CreateNo(), 16).ToLower();
                    userloginOn.F_UserPassword = Md5.md5(DESEncrypt.Encrypt(Password.ToLower(), userloginOn.F_UserSecretkey).ToLower(), 32).ToLower();
                    userloginOn.F_ChangePasswordDate = DateTime.Now;
   
                    if (_repository.Update(userloginOn) < 0)
                    {
                        LogHelper.Info("用户【" + cacheInfo.UserCode + "】修改登录密码失败！", OpType.Edit, null,
                            "UserApp--UpdadteUserPwd");
                        return new { state = "error", message = "操作失败！" };
                    }
                } 

                LogHelper.Info("用户【" + cacheInfo.UserCode + "】修改登录密码成功！", OpType.Edit, null,
                "UserApp--UpdadteUserPwd");

                return new { state = "success"};
            }
            catch (Exception ex)
            {
                LogHelper.Info("用户【" + cacheInfo.UserCode + "】修改登录密码发生异常！，异常信息为" + ex.Message + "", OpType.Edit, ex,
                    "UserApp--UpdadteUserPwd", "", ViewLevel.Technicist);
                return new { state = "error", message = "操作失败！" };
            }
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="sex"></param>
        /// <returns></returns>
        public Tuple<bool, string> UpdateInfo(Sys_User entity, int sex)
        {
            var cacheInfo = OperatorProvider.Provider.GetCurrent();//获取当前登陆用户信息
            try
            {
                #region 手机号查重 
               var phone = entity.F_MobilePhone;
                if (!CheckPhNumber(phone, entity.F_Account))
                {
                    return new Tuple<bool, string>(false, "已存在当前手机号");
                }
                //var account = _repository.FindEntity<Sys_Account>(m => m.UserCode == entity.F_Account);
                #endregion
                TransactionOptions to = new TransactionOptions();
                to.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, to))//事物
                {
                    var model = _repository.FindEntity<Sys_User>(m => m.F_Id == entity.F_Id);
                    if (model == null)
                    {
                        LogHelper.Info("用户【" + cacheInfo.UserCode + "】修改个人信息操作失败！", OpType.Edit, null,
                    "UserApp--UpdateInfo");
                        return new Tuple<bool, string>(false, "操作失败");
                    }
                    model.F_Gender = sex > 0 ? true : false;
                    model.F_RealName = entity.F_NickName;
                    model.F_Birthday = entity.F_Birthday;
                    // model.F_MobilePhone = entity.F_MobilePhone;
                    model.F_MobilePhone = phone;
                    model.F_Email = entity.F_Email;
                    model.F_WeChat = entity.F_WeChat;
                    
                    if (_repository.Update(model) < 0)
                    {
                        LogHelper.Info("用户【" + cacheInfo.UserCode + "】修改个人信息操作失败！", OpType.Edit, null,
                    "UserApp--UpdateInfo");
                        return new Tuple<bool, string>(false, "操作失败");
                    }
                   
                    scope.Complete();
                    LogHelper.Info("用户【" + cacheInfo.UserCode + "】修改个人信息操作成功！", OpType.Edit, null,
                    "UserApp--UpdateInfo");
                    return new Tuple<bool, string>(true, "操作成功");
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info("用户【" + cacheInfo.UserCode + "】修改个人信息操作异常！异常信息为" + ex.Message + "", OpType.Edit, ex,
                    "UserApp--UpdateInfo", "", ViewLevel.Technicist);
                return new Tuple<bool, string>(false, "操作失败");
            }
        }

        

        public bool CheckPhNumber(string phone, string usercode)
        { 
            var account = _repository.FindEntity<Sys_User>(p => p.F_MobilePhone == phone && p.F_Account != usercode);
            if (account != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
         
    }
}
