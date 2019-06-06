using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Data.PulbicRepository;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using RepositoryBase = WMSOSPS.Cloud.Data.PulbicRepository.RepositoryBase;

namespace WMSOSPS.Cloud.Repository.Role
{
    public class RoleRepository : RepositoryBase 
    {
        public void DeleteForm(string keyValue)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                db.Delete<RoleEntity>(t => t.F_Id == keyValue);
                db.Delete<RoleAuthorizeEntity>(t => t.F_ObjectId == keyValue);
                db.Commit();
            }
        }

        //public List<User_RoleData> GetUser_RoleData(string userCode)
        //{
        //    return dbcontext.User_RoleData.Where(a => a.WorkerID == userCode).ToList();
        //}

        //public List<User_SubPower> GetUserPower()
        //{
        //    var data = dbcontext.User_Power.Where(a => a.IsValid == 1).ToList();
        //    List<User_SubPower> list = data.Select(item => new User_SubPower()
        //    {
        //        P_Code = item.P_Code,
        //        P_Name = item.P_Name,
        //        Parent_Code = "0",
        //        Sort = item.Sort,
        //        IsValid = 1,
        //        Src = "",
        //    }).ToList();
        //    var subData = dbcontext.User_SubPower.Where(a => a.IsValid == 1).ToList();
        //    list.AddRange(subData);
        //    return list;
        //}
        

        public void SubmitForm(RoleEntity roleEntity, List<RoleAuthorizeEntity> roleAuthorizeEntitys, string keyValue)
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
                db.Delete<RoleAuthorizeEntity>(t => t.F_ObjectId == roleEntity.F_Id);
                db.Insert(roleAuthorizeEntitys);
                db.Commit();
            }
        }
    }
}
