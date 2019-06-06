 

using System;
using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Cache;
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
    public class RoleApp
    {
        private IRoleRepository service = new RoleRepository();
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();

        public List<Sys_Role> GetList( string keyword = "")
        {
            var expression = ExtLinq.True<Sys_Role>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_FullName.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode.Contains(keyword));
            }
            expression = expression.And(t => t.F_Category == 1);
            return service.IQueryable(expression).OrderBy(t => t.F_SortCode).ToList(); 
        }

        public List<Sys_Role> GetList_Page(Pagination pagination, string keyword = "")
        {
            var expression = ExtLinq.True<Sys_Role>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_FullName.Contains(keyword));
                expression = expression.Or(t => t.F_EnCode.Contains(keyword));
            }
            expression = expression.And(t => t.F_Category == 1);
            //return service.IQueryable(expression).OrderBy(t => t.F_SortCode).ToList();
            return service.FindList(expression, pagination);
        }
        public Sys_Role GetForm(string keyValue)
        {
            return service.FindEntity<Sys_Role>(keyValue);
        }
        public void DeleteForm(string keyValue)
        {
            service.DeleteForm(keyValue);
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            LogHelper.Info("角色ID：【" + keyValue + "】删除！操作人账号：" + op.UserCode + ",操作人名称:" + op.UserName + ",操作时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
        }
        public void SubmitForm(Sys_Role roleEntity, string[] permissionIds, string keyValue)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                roleEntity.F_Id = keyValue;
            }
            else
            {
                roleEntity.F_Id = Common.GuId();
            }
            var moduledata = moduleApp.GetList();
            var buttondata = moduleButtonApp.GetList();
            List<Sys_RoleAuthorize> roleAuthorizeEntitys = new List<Sys_RoleAuthorize>();
            foreach (var itemId in permissionIds)
            {
                Sys_RoleAuthorize roleAuthorizeEntity = new Sys_RoleAuthorize();
                roleAuthorizeEntity.F_Id = Common.GuId();
                roleAuthorizeEntity.F_ObjectType = 1;//角色的权限
                roleAuthorizeEntity.F_ObjectId = roleEntity.F_Id;
                roleAuthorizeEntity.F_ItemId = itemId;
                if (moduledata.Find(t => t.F_Id == itemId) != null)
                {
                    roleAuthorizeEntity.F_ItemType = 1;
                }
                if (buttondata.Find(t => t.F_Id == itemId) != null)
                {
                    roleAuthorizeEntity.F_ItemType = 2;
                }
                roleAuthorizeEntitys.Add(roleAuthorizeEntity);
            }
            service.SubmitForm(roleEntity, roleAuthorizeEntitys, keyValue);
            CacheFactory.Cache().RemoveCache();
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            LogHelper.Info("角色：【" + roleEntity.F_FullName + "】编辑！编辑人账号：" + op.UserCode+ ",编辑人名称:" + op.UserName+ ",编辑时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),Code.Enum.OpType.System,null,"","",ViewLevel.Admin);
        }


        public void SubmitFormRole( string[] permissionIds, string keyValue)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            var moduledata = moduleApp.GetList();
            var buttondata = moduleButtonApp.GetList();
            List<Sys_RoleAuthorize> roleAuthorizeEntitys = new List<Sys_RoleAuthorize>();
            foreach (var itemId in permissionIds)
            {
                Sys_RoleAuthorize roleAuthorizeEntity = new Sys_RoleAuthorize();
                roleAuthorizeEntity.F_Id = Common.GuId();
                roleAuthorizeEntity.F_ObjectType = 3;//用户权限
                roleAuthorizeEntity.F_ObjectId = keyValue;
                roleAuthorizeEntity.F_ItemId = itemId;
                if (moduledata.Any(t => t.F_Id == itemId))
                {
                    roleAuthorizeEntity.F_ItemType = 1;//菜单
                }
                if (buttondata.Any(t => t.F_Id == itemId))
                {
                    roleAuthorizeEntity.F_ItemType = 2;//按钮
                }
                roleAuthorizeEntitys.Add(roleAuthorizeEntity);
            }
            service.SubmitFormRole(roleAuthorizeEntitys, keyValue);
            LogHelper.Info("用户ID:【" + keyValue + "】权限单账号编辑！编辑人账号：" + op.UserCode + ",编辑人名称:" + op.UserName + ",编辑时间" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), Code.Enum.OpType.System, null, "", "", ViewLevel.Admin);
            CacheFactory.Cache().RemoveCache();

        }
    }
}
