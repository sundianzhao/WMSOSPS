 

using System.Linq;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Data.PulbicRepository;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;

namespace WMSOSPS.Cloud.Domain.IRepository.SystemManage
{
    public interface IUserRepository : IRepositoryBase 
    {
        void DeleteForm(string keyValue);
        void SubmitForm(Sys_User userEntity, Sys_UserLogOn userLogOnEntity, string keyValue);

        void DisabledAccount(Sys_User userEntity);
        int DeleteNumber(int nId);
    }
}
