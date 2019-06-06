using System;
using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Repository.Role;

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class RoleBll : IDisposable
    {
        private RoleRepository service = new RoleRepository();
        public void Dispose()
        {
            if (service != null)
            {
                service.Dispose();
            }
        }
       
        
        
        
    }
}
