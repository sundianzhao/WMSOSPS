 

using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Data.PulbicRepository; 
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;

namespace WMSOSPS.Cloud.Repository.SystemManage
{
    public class RoleRepository : RepositoryBase, IRoleRepository
    {
        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Delete<Sys_Role>(t => t.F_Id == keyValue);
                db.Delete<Sys_RoleAuthorize>(t => t.F_ObjectId == keyValue);
                db.Commit();
            }
        }
        public void SubmitForm(Sys_Role roleEntity, List<Sys_RoleAuthorize> roleAuthorizeEntitys, string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    db.Update(roleEntity);
                }
                else
                {
                    roleEntity.F_Category = 1;
                    db.Insert(roleEntity);
                }
                db.Delete<Sys_RoleAuthorize>(t => t.F_ObjectId == roleEntity.F_Id);
                db.Insert(roleAuthorizeEntitys);
                db.Commit();
            }
        }

        public void SubmitFormRole(List<Sys_RoleAuthorize> roleAuthorizeEntitys, string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Delete<Sys_RoleAuthorize>(t => t.F_ObjectId == keyValue);
                db.Insert(roleAuthorizeEntitys);
                db.Commit();
            }
        }

        public Sys_Role GetRole(string roleId)
        {
            var data = dbcontext.Sys_Role.FirstOrDefault(a => a.F_Id == roleId);
            return data;
        }
    }
}
