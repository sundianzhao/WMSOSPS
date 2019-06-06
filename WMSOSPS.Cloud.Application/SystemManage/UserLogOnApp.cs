/******************************************************************************* 
*********************************************************************************/

using System; 
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Security;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;
using WMSOSPS.Cloud.Repository.SystemManage;

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class UserLogOnApp
    {
        private IUserLogOnRepository service = new UserLogOnRepository();

        public Sys_UserLogOn GetForm(string keyValue)
        {
            return service.FindEntity<Sys_UserLogOn>(keyValue);
        }
        public void UpdateForm(Sys_UserLogOn userLogOnEntity)
        {
            service.Update(userLogOnEntity);
        }
        public void RevisePassword(string userPassword,string keyValue,string account)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            Sys_UserLogOn userLogOnEntity = new Sys_UserLogOn();
            userLogOnEntity.F_Id = keyValue;
            userLogOnEntity.F_UserSecretkey = Md5.md5(Common.CreateNo(), 16).ToLower();
            userLogOnEntity.F_UserPassword = Md5.md5(DESEncrypt.Encrypt(Md5.md5(userPassword, 32).ToLower(), userLogOnEntity.F_UserSecretkey).ToLower(), 32).ToLower();
            service.Update(userLogOnEntity);
            LogHelper.Info("用户ID:【" + keyValue + "】密码重置！编辑人账号：" + op.UserCode + ",编辑人名称:" + op.UserName + ",编辑时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            //service.RevisePassword(userLogOnEntity, account);
        }

    }
}
