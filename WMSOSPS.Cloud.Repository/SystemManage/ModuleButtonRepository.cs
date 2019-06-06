 
using System.Collections.Generic;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Data.PulbicRepository;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;

namespace WMSOSPS.Cloud.Repository.SystemManage
{
    public class ModuleButtonRepository : RepositoryBase, IModuleButtonRepository
    {
        public void SubmitCloneButton(List<Sys_ModuleButton> entitys)
        {
            using (var db = new RepositoryBase().BeginTrans())
            {
                foreach (var item in entitys)
                {
                    db.Insert(item);
                }
                db.Commit();
            }
        }
    }
}
