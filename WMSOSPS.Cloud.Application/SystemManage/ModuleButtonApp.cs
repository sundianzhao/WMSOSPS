 

using System;
using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;
using WMSOSPS.Cloud.Repository.SystemManage;

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class ModuleButtonApp
    {
        private IModuleButtonRepository service = new ModuleButtonRepository();

        public List<Sys_ModuleButton> GetList(string moduleId = "")
        {
            var expression = ExtLinq.True<Sys_ModuleButton>();
            if (!string.IsNullOrEmpty(moduleId))
            {
                expression = expression.And(t => t.F_ModuleId == moduleId);
            }
            return service.IQueryable(expression).OrderBy(t => t.F_SortCode).ToList();
        }
        public Sys_ModuleButton GetForm(string keyValue)
        {
            return service.FindEntity<Sys_ModuleButton>(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            if (service.IQueryable<Sys_ModuleButton>().Count(t => t.F_ParentId.Equals(keyValue)) > 0)
            {
                throw new Exception("删除失败！操作的对象包含了下级数据。");
            }
            else
            {
                OperatorModel op = OperatorProvider.Provider.GetCurrent();
                var F_FullName = service.FindEntity<Sys_ModuleButton>(t => t.F_Id == keyValue).F_FullName;
                service.Delete<Sys_ModuleButton>(t => t.F_Id == keyValue);
                LogHelper.Info("按钮：【" + F_FullName + "】删除！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName +
               ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            }
        }
        public void SubmitForm(Sys_ModuleButton moduleButtonEntity, string keyValue)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            if (!string.IsNullOrEmpty(keyValue))
            {
                //moduleButtonEntity.Modify(keyValue);
                moduleButtonEntity.F_Id = keyValue;
                moduleButtonEntity.F_LastModifyTime = DateTime.Now;
                moduleButtonEntity.F_LastModifyUserId = OperatorProvider.Provider.GetCurrent().UserId;
                if (moduleButtonEntity.F_UrlAddress ==null)
                    moduleButtonEntity.F_UrlAddress = "";

                if (moduleButtonEntity.F_JsEvent == null)
                    moduleButtonEntity.F_JsEvent = "";
                service.Update(moduleButtonEntity);
                LogHelper.Info("按钮：【" + moduleButtonEntity.F_FullName + "】编辑！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName +
              ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            }
            else
            {
                //moduleButtonEntity.Create();
                moduleButtonEntity.F_Id = Common.GuId();
                moduleButtonEntity.F_CreatorUserId = OperatorProvider.Provider.GetCurrent().UserId;
                moduleButtonEntity.F_CreatorTime = DateTime.Now;
                service.Insert(moduleButtonEntity);
                LogHelper.Info("按钮：【" + moduleButtonEntity.F_FullName + "】新增！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName +
              ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            }
        }
        public void SubmitCloneButton(string moduleId, string Ids)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            string[] ArrayId = Ids.Split(',');
            var data = this.GetList();
            List<Sys_ModuleButton> entitys = new List<Sys_ModuleButton>();
            foreach (string item in ArrayId)
            {
                Sys_ModuleButton moduleButtonEntity = data.Find(t => t.F_Id == item);
                moduleButtonEntity.F_Id = Common.GuId();
                moduleButtonEntity.F_ModuleId = moduleId;
                entitys.Add(moduleButtonEntity);
            }
            service.SubmitCloneButton(entitys);
            LogHelper.Info("菜单ID：【" + moduleId + "】克隆按钮！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName +
            ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
        }
    }
}
