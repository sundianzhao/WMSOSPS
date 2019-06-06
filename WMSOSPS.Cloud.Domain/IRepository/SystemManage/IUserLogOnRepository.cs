 

using System.Linq;
using WMSOSPS.Cloud.Data.CloudContext;

using WMSOSPS.Cloud.Data.PulbicRepository;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;

namespace WMSOSPS.Cloud.Domain.IRepository.SystemManage
{
    public interface IUserLogOnRepository : IRepositoryBase 
    {
        void RevisePassword(Sys_UserLogOn userLogOnEntity, string account);
      
    }
}
