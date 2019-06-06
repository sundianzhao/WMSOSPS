 

using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Security;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Data.PulbicRepository;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;

namespace WMSOSPS.Cloud.Repository.SystemManage
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                Sys_User user = db.FindEntity<Sys_User>(p => p.F_Id == keyValue);
                db.Delete<Sys_User>(t => t.F_Id == keyValue);
                db.Delete<Sys_UserLogOn>(t => t.F_UserId == keyValue); 
                db.Commit();
            }
        }
        public void SubmitForm(Sys_User userEntity, Sys_UserLogOn userLogOnEntity, string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            { 
                if (!string.IsNullOrEmpty(keyValue))
                {
                    db.Update(userEntity); 
                }
                else
                {
                    userLogOnEntity.F_Id = userEntity.F_Id;
                    userLogOnEntity.F_UserId = userEntity.F_Id;
                    userLogOnEntity.F_UserSecretkey = Md5.md5(Common.CreateNo(), 16).ToLower();
                    userLogOnEntity.F_UserPassword = Md5.md5(DESEncrypt.Encrypt(Md5.md5(userLogOnEntity.F_UserPassword, 32).ToLower(), userLogOnEntity.F_UserSecretkey).ToLower(), 32).ToLower();
                    db.Insert(userEntity);
                    db.Insert(userLogOnEntity); 
                }
                db.Commit();
            }
        }

        /// <summary>
        /// 禁用/启用
        /// </summary>
        /// <param name="userEntity"></param>
        public void DisabledAccount(Sys_User userEntity)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Update(userEntity); 
                db.Commit();
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        public int DeleteNumber(int keyValue)
        {
            int i = 0;
            using (var db = new RepositoryBase())
            {
               // i= db.Delete<wx_PhoneNumber>(t => t.nId == keyValue);
            }
            return i;
        }
    }
}
