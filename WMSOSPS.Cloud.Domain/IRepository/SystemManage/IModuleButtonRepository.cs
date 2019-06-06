 

using System.Collections.Generic;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Data.PulbicRepository;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;

namespace WMSOSPS.Cloud.Domain.IRepository.SystemManage
{
    public interface IModuleButtonRepository : IRepositoryBase 
    {
        void SubmitCloneButton(List<Sys_ModuleButton> entitys);
    }
}
