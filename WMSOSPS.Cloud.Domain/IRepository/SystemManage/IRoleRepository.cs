 

using System.Collections.Generic;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Data.PulbicRepository;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;

namespace WMSOSPS.Cloud.Domain.IRepository.SystemManage
{
    public interface IRoleRepository : IRepositoryBase 
    {
        void DeleteForm(string keyValue);
        void SubmitForm(Sys_Role roleEntity, List<Sys_RoleAuthorize> roleAuthorizeEntitys, string keyValue);
        void SubmitFormRole( List<Sys_RoleAuthorize> roleAuthorizeEntitys, string keyValue);
        Sys_Role GetRole(string roleId);
    }
}
