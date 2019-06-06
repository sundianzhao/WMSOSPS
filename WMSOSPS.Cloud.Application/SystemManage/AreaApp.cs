 

using System;
using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;
using WMSOSPS.Cloud.Repository.SystemManage;

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class AreaApp
    {
        private IAreaRepository service = new AreaRepository();

        public List<Sys_Area> GetList()
        {
            return service.IQueryable<Sys_Area>().ToList();
        }
        public Sys_Area GetForm(string keyValue)
        {
            return service.FindEntity<Sys_Area>(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            if (service.IQueryable<Sys_Area>().Count(t => t.F_ParentId.Equals(keyValue)) > 0)
            {
                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            else
            {
                service.Delete<Sys_Area>(t => t.F_Id == keyValue);
            }
        }
        public void SubmitForm(Sys_Area areaEntity, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                //areaEntity.Modify(keyValue);
                areaEntity.F_Id = keyValue;
                areaEntity.F_LastModifyTime = DateTime.Now;
                areaEntity.F_LastModifyUserId = OperatorProvider.Provider.GetCurrent().UserId;
                service.Update(areaEntity);
            }
            else
            {
                //areaEntity.Create();
                areaEntity.F_Id = Common.GuId();
                areaEntity.F_CreatorUserId = OperatorProvider.Provider.GetCurrent().UserId;
                areaEntity.F_CreatorTime = DateTime.Now;
                service.Insert(areaEntity);
            }
        }

        public object GetPrivince()
        {
            return service.FindList<Sys_Area>(p=>p.F_ParentId=="0"&&p.F_EnabledMark==true);
        }
        public object GetCity()
        {
            return service.FindList<Sys_Area>(p => p.F_ParentId != "0" && p.F_EnabledMark == true);
        }
    }
}
