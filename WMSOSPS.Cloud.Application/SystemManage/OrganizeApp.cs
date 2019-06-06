 

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
    public class OrganizeApp
    {
        private IOrganizeRepository service = new OrganizeRepository();

        public List<Sys_Organize> GetList()
        {
            return service.IQueryable<Sys_Organize>().OrderBy(t => t.F_CreatorTime).ToList();
        }
        public List<Sys_Organize> GetList(int ParentId)
        {
            var list = service.IQueryable<Sys_Organize>().Where(m => m.F_ParentId == "0").ToList();
            return list;
        }
        public Sys_Organize GetForm(string keyValue)
        {
            return service.FindEntity<Sys_Organize>(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            if (service.IQueryable<Sys_Organize>().Count(t => t.F_ParentId.Equals(keyValue)) > 0)
            {
                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            else
            {
                var F_FullName = service.FindEntity<Sys_Organize>(m => m.F_Id == keyValue).F_FullName;
                LogHelper.Info("机构：【" + F_FullName + "】删除！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
                service.Delete<Sys_Organize>(t => t.F_Id == keyValue);
            }
        }
        public void SubmitForm(Sys_Organize organizeEntity, string keyValue)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            if (!string.IsNullOrEmpty(keyValue))
            {
                //organizeEntity.Modify(keyValue);
                organizeEntity.F_Id = keyValue;
                organizeEntity.F_LastModifyTime = DateTime.Now;
                organizeEntity.F_LastModifyUserId = OperatorProvider.Provider.GetCurrent().UserId;
                int flag= service.Update(organizeEntity);
                LogHelper.Info("机构：【" + organizeEntity.F_FullName + "】编辑！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            }
            else
            {
                //organizeEntity.Create();
                organizeEntity.F_Id = Common.GuId();
                organizeEntity.F_CreatorUserId = OperatorProvider.Provider.GetCurrent().UserId;
                organizeEntity.F_CreatorTime = DateTime.Now;
                service.Insert(organizeEntity);
                LogHelper.Info("机构：【" + organizeEntity.F_FullName + "】新增！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            }
        }
    }
}
