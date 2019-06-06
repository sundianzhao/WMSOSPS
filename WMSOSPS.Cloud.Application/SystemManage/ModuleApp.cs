 

using System;
using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;
using WMSOSPS.Cloud.Repository.SystemManage;

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class ModuleApp
    {
        private IModuleRepository service = new ModuleRepository();

        public List<Sys_Module> GetList()
        {
            return service.IQueryable<Sys_Module>().OrderBy(t => t.F_SortCode).ToList();
        }
        public Sys_Module GetForm(string keyValue)
        {
            return service.FindEntity<Sys_Module>(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            if (service.IQueryable<Sys_Module>().Count(t => t.F_ParentId.Equals(keyValue)) > 0)
            {
                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            else
            {
                OperatorModel op = OperatorProvider.Provider.GetCurrent();
                var F_FullName= service.FindEntity<Sys_Module>(t => t.F_Id == keyValue).F_FullName;
                service.Delete<Sys_Module>(t => t.F_Id == keyValue);
                LogHelper.Info("菜单：【" + F_FullName + "】删除！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName +
               ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            }
        }
        public void SubmitForm(Sys_Module moduleEntity, string keyValue)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            if (!string.IsNullOrEmpty(keyValue))
            {
                //moduleEntity.Modify(keyValue);
                moduleEntity.F_Id = keyValue;
                moduleEntity.F_LastModifyTime = DateTime.Now;
                moduleEntity.F_LastModifyUserId = OperatorProvider.Provider.GetCurrent().UserId;
                service.Update(moduleEntity);
                LogHelper.Info("菜单：【" + moduleEntity.F_FullName + "】编辑！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName +
           ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            }
            else
            {
                //moduleEntity.Create();
                moduleEntity.F_Id = Common.GuId();
                moduleEntity.F_CreatorUserId = OperatorProvider.Provider.GetCurrent().UserId;
                moduleEntity.F_CreatorTime = DateTime.Now;
                service.Insert(moduleEntity);
                LogHelper.Info("菜单：【" + moduleEntity.F_FullName + "】新增！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName +
           ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            }
        }
    }
}
