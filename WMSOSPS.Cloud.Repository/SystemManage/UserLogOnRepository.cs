 

using System;
using System.Data.Entity;
using System.Linq;
using WMSOSPS.Cloud.Data.CloudContext; 
using WMSOSPS.Cloud.Data.PulbicRepository;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;

namespace WMSOSPS.Cloud.Repository.SystemManage
{
    public class UserLogOnRepository : RepositoryBase, IUserLogOnRepository
    {
      

        public void RevisePassword(Sys_UserLogOn userLogOnEntity, string account)
        {
            using (var db = new RepositoryBase().BeginTrans())
            { 
                db.Update(userLogOnEntity);  
                db.Commit();
            }
        }
    }
}
