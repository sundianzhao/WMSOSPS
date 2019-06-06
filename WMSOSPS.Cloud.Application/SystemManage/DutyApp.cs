 

using System;
using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;
using WMSOSPS.Cloud.Repository.SystemManage;

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class DutyApp
    {
        private IRoleRepository service = new RoleRepository();

        public List<Sys_Role> GetList_Page(Pagination pagination, string keyword = "")
        {
            var expression = ExtLinq.True<Sys_Role>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_FullName.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode.Contains(keyword));
            }
            expression = expression.And(t => t.F_Category == 2);
            //return service.IQueryable(expression).OrderBy(t => t.F_SortCode).ToList();
            return service.FindList(expression, pagination);
        }

        public List<Sys_Role> GetList(string keyword = "")
        {
            var expression = ExtLinq.True<Sys_Role>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_FullName.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode.Contains(keyword));
            }
            expression = expression.And(t => t.F_Category == 2);
            return service.IQueryable(expression).OrderBy(t => t.F_SortCode).ToList();
        }
        public Sys_Role GetForm(string keyValue)
        {
            return service.FindEntity<Sys_Role>(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            var F_FullName = service.FindEntity<Sys_Role>(t => t.F_Id == keyValue).F_FullName;
            service.Delete<Sys_Role>(t => t.F_Id == keyValue);
            LogHelper.Info("岗位：【" + F_FullName + "】删除！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + 
                ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), WMSOSPS.Cloud.Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
        }
        public void SubmitForm(Sys_Role roleEntity, string keyValue)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            if (!string.IsNullOrEmpty(keyValue))
            {
                //roleEntity.Modify(keyValue);
                roleEntity.F_Id = keyValue;
                roleEntity.F_LastModifyTime = DateTime.Now;
                roleEntity.F_LastModifyUserId = OperatorProvider.Provider.GetCurrent().UserId;
                service.Update(roleEntity);
                LogHelper.Info("岗位：【" + roleEntity. F_FullName + "】编辑！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), WMSOSPS.Cloud.Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            }
            else
            {
                //roleEntity.Create();
                roleEntity.F_Id = Common.GuId();
                roleEntity.F_CreatorUserId = OperatorProvider.Provider.GetCurrent().UserId;
                roleEntity.F_CreatorTime = DateTime.Now;
                roleEntity.F_Category = 2;
                service.Insert(roleEntity);
                LogHelper.Info("岗位：【" + roleEntity.F_FullName + "】新增！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),  OpType.System, null, "", "", ViewLevel.Admin);
            }
        }
        public List<Sys_Role> GetListJson()
        {
            var list = service.IQueryable<Sys_Role>().ToList();
            return list;
        }
    }
}
