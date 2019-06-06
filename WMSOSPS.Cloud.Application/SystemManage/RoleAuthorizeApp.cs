 

using System;
using System.Collections.Generic;
using System.Linq;
using WMSOSPS.Cloud.Code.Cache;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Domain.Entity.SystemManage;
using WMSOSPS.Cloud.Domain.IRepository.SystemManage;
using WMSOSPS.Cloud.Domain.ViewModel;
using WMSOSPS.Cloud.Repository.SystemManage;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Enum;

namespace WMSOSPS.Cloud.Application.SystemManage
{
    public class RoleAuthorizeApp
    {
        private IRoleRepository _repository = new RoleRepository();
        private IRoleAuthorizeRepository service = new RoleAuthorizeRepository();
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();

        public List<Sys_RoleAuthorize> GetList(string ObjectId)
        {
            return service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectId == ObjectId).ToList();
        }

        //通过roleid查出所有的模块菜单
        public List<Sys_Module> GetMenuList(string roleId)
        {
            var data = new List<Sys_Module>();
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                //data = moduleApp.GetList().Where(a => a.F_IsMenu ?? false).Where(a=>a.F_EnabledMark==true).ToList();//只查框架页
                data = moduleApp.GetList().Where(a => a.F_EnabledMark == true).ToList();
            }
            else
            {
                var moduledata = moduleApp.GetList().Where(a => a.F_IsMenu ?? false).Where(a => a.F_EnabledMark == true).ToList();//只查框架页
                //t.F_ObjectType == 1是角色对应的权限
                var authorizedata = service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectId == roleId && t.F_ItemType == 1 && t.F_ObjectType == 1).ToList();

                foreach (var item in authorizedata)
                {
                    Sys_Module moduleEntity = moduledata.Find(t => t.F_Id == item.F_ItemId);
                    if (moduleEntity != null)
                    {
                        data.Add(moduleEntity);
                    }
                }
            }
            return data.OrderBy(t => t.F_SortCode).ToList();
        }

        //通过userCode查出所有的模块菜单
        public List<Sys_Module> GetSpecialMenuList(string userCode)
        {
            var data = new List<Sys_Module>();
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                data = moduleApp.GetList().Where(a => a.F_IsMenu ?? false).ToList();//只查框架页
            }
            else
            {
                var moduledata = moduleApp.GetList().Where(a => a.F_IsMenu ?? false).ToList();//只查框架页
                //F_ObjectType==3是个人对应的权限
                var authorizedata = service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectType == 3 && t.F_ObjectId == userCode && t.F_ItemType == 1).ToList();
                foreach (var item in authorizedata)
                {
                    Sys_Module moduleEntity = moduledata.Find(t => t.F_Id == item.F_ItemId);
                    if (moduleEntity != null)
                    {
                        data.Add(moduleEntity);
                    }
                }
            }
            return data.OrderBy(t => t.F_SortCode).ToList();
        }

        public List<Sys_ModuleButton> GetButtonList(string roleId)
        {
            var data = new List<Sys_ModuleButton>();
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                data = moduleButtonApp.GetList();
            }
            else
            {
                var buttondata = moduleButtonApp.GetList();
                var authorizedata = service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectId == roleId && t.F_ItemType == 2 && t.F_ObjectType == 1).ToList();
                foreach (var item in authorizedata)
                {
                    Sys_ModuleButton moduleButtonEntity = buttondata.Find(t => t.F_Id == item.F_ItemId);
                    if (moduleButtonEntity != null)
                    {
                        data.Add(moduleButtonEntity);
                    }
                }
            }
            return data.OrderBy(t => t.F_SortCode).ToList();
        }

        public List<Sys_ModuleButton> GetSpecialButtonList(string userCode)
        {
            var data = new List<Sys_ModuleButton>();
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                data = moduleButtonApp.GetList();
            }
            else
            {
                var buttondata = moduleButtonApp.GetList();
                var authorizedata = service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectId == userCode && t.F_ItemType == 2 && t.F_ObjectType == 3).ToList();
                foreach (var item in authorizedata)
                {
                    Sys_ModuleButton moduleButtonEntity = buttondata.Find(t => t.F_Id == item.F_ItemId);
                    if (moduleButtonEntity != null)
                    {
                        data.Add(moduleButtonEntity);
                    }
                }
            }
            return data.OrderBy(t => t.F_SortCode).ToList();
        }

        public bool ActionValidate(string roleId, string userCode, string moduleId, string action)
        {
            var authorizeurldata = new List<AuthorizeActionModel>();
            var cachedata = CacheFactory.Cache().GetCache<List<AuthorizeActionModel>>("authorizeurldata_" + roleId); 
            if (cachedata == null)
            {
                var moduledata = moduleApp.GetList();
                var buttondata = moduleButtonApp.GetList();

                var isAssistant = _repository.GetRole(roleId).F_EnCode;


                //用户对应的菜单和按钮的权限（F_ObjectType == 3）
                if (isAssistant == "agentAssistant")
                    userCode = OperatorProvider.Provider.GetCurrent().AssistantID;

                var authorizedata = service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectType == 3 && t.F_ObjectId == userCode).ToList();
                foreach (var item in authorizedata)
                {
                    //菜单
                    if (item.F_ItemType == 1)
                    {
                        Sys_Module moduleEntity = moduledata.FirstOrDefault(t => t.F_Id == item.F_ItemId);
                        if (moduleEntity != null)
                            authorizeurldata.Add(new AuthorizeActionModel { F_Id = moduleEntity.F_Id, F_UrlAddress = moduleEntity.F_UrlAddress });
                    }
                    //按钮
                    else if (item.F_ItemType == 2)
                    {
                        Sys_ModuleButton moduleButtonEntity = buttondata.FirstOrDefault(t => t.F_Id == item.F_ItemId);
                        if (moduleButtonEntity != null)
                            authorizeurldata.Add(new AuthorizeActionModel { F_Id = moduleButtonEntity.F_ModuleId, F_UrlAddress = moduleButtonEntity.F_UrlAddress });
                    }
                }

                if (isAssistant != "agentAssistant")
                {
                    //角色对应的菜单和按钮权限（F_ObjectType == 1）
                    authorizedata = service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectType == 1 && t.F_ObjectId == roleId).ToList();
                    foreach (var item in authorizedata)
                    {
                        if (item.F_ItemType == 1)
                        {
                            Sys_Module moduleEntity = moduledata.FirstOrDefault(t => t.F_Id == item.F_ItemId);
                            if (moduleEntity != null)
                                authorizeurldata.Add(new AuthorizeActionModel { F_Id = moduleEntity.F_Id, F_UrlAddress = moduleEntity.F_UrlAddress });
                        }
                        else if (item.F_ItemType == 2)
                        {
                            Sys_ModuleButton moduleButtonEntity = buttondata.FirstOrDefault(t => t.F_Id == item.F_ItemId);
                            if (moduleButtonEntity != null)
                                authorizeurldata.Add(new AuthorizeActionModel { F_Id = moduleButtonEntity.F_ModuleId, F_UrlAddress = moduleButtonEntity.F_UrlAddress });
                        }
                    }
                }
                else
                {
                    if (authorizedata.Count<=1)
                    {
                        //角色对应的菜单和按钮权限（F_ObjectType == 1）
                        authorizedata = service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectType == 1 && t.F_ObjectId == roleId).ToList();
                        foreach (var item in authorizedata)
                        {
                            if (item.F_ItemType == 1)
                            {
                                Sys_Module moduleEntity = moduledata.FirstOrDefault(t => t.F_Id == item.F_ItemId);
                                if (moduleEntity != null)
                                    authorizeurldata.Add(new AuthorizeActionModel { F_Id = moduleEntity.F_Id, F_UrlAddress = moduleEntity.F_UrlAddress });
                            }
                            else if (item.F_ItemType == 2)
                            {
                                Sys_ModuleButton moduleButtonEntity = buttondata.FirstOrDefault(t => t.F_Id == item.F_ItemId);
                                if (moduleButtonEntity != null)
                                    authorizeurldata.Add(new AuthorizeActionModel { F_Id = moduleButtonEntity.F_ModuleId, F_UrlAddress = moduleButtonEntity.F_UrlAddress });
                            }
                        }
                    }
                }
                
                

                CacheFactory.Cache().WriteCache(authorizeurldata, "authorizeurldata_" + roleId, DateTime.Now.AddMinutes(5));
            }
            else
            {
                authorizeurldata = cachedata;
            }
           // var sss = authorizeurldata.Where(p => p.F_UrlAddress == "/EnterpriseUserManage/EnterpriseAccountAudit/AuditEnterperiseAccount").ToList();
            //authorizeurldata = authorizeurldata.FindAll(t => t.F_Id.Equals(moduleId));
            foreach (var item in authorizeurldata)
            {
                if (!string.IsNullOrEmpty(item.F_UrlAddress))
                {
                    string[] url = item.F_UrlAddress.Split('?');
                    //if (item.F_Id == moduleId && url[0] == action)
                    //{
                    //    return true;
                    //}
                    if (url[0] == action)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否具有某个权限（查询某一个按钮是否具有操作的权限）
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="userCode"></param>
        /// <param name="enCode"></param>
        /// <returns></returns>
        public bool ActionValidate(string roleId, string userCode, string enCode)
        {
            var authorizeurldata = new List<Sys_ModuleButton>();
            var cachedata = CacheFactory.Cache().GetCache<List<Sys_ModuleButton>>("authorizebuttondata_" + userCode);
            if (cachedata == null)
            {
                var buttondata = moduleButtonApp.GetList();

                var isAssistant = _repository.GetRole(roleId).F_EnCode;

                //用户对应的菜单和按钮的权限（F_ObjectType == 3）
                if (isAssistant =="agentAssistant")
                    userCode = OperatorProvider.Provider.GetCurrent().AssistantID;

                var authorizedata = service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectType == 3 && t.F_ObjectId == userCode && t.F_ItemType == 2).ToList();
                foreach (var item in authorizedata)
                {
                    Sys_ModuleButton moduleButtonEntity = buttondata.FirstOrDefault(t => t.F_Id == item.F_ItemId);
                    if (moduleButtonEntity != null)
                        authorizeurldata.Add(moduleButtonEntity);
                }
                if (isAssistant != "agentAssistant")
                {
                    //角色对应的菜单和按钮权限（F_ObjectType == 1）
                    //只查按钮
                    authorizedata = service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectType == 1 && t.F_ObjectId == roleId && t.F_ItemType == 2).ToList();
                    foreach (var item in authorizedata)
                    {
                        Sys_ModuleButton moduleButtonEntity = buttondata.FirstOrDefault(t => t.F_Id == item.F_ItemId);
                        if (moduleButtonEntity != null)
                            authorizeurldata.Add(moduleButtonEntity);
                    }
                }
                else
                {
                    
                    if (authorizeurldata.Count <=1)
                    {
                        //角色对应的菜单和按钮权限（F_ObjectType == 1）
                        //只查按钮
                        authorizeurldata = new List<Sys_ModuleButton>();
                        authorizedata = service.IQueryable<Sys_RoleAuthorize>(t => t.F_ObjectType == 1 && t.F_ObjectId == roleId && t.F_ItemType == 2).ToList();
                        foreach (var item in authorizedata)
                        {
                            Sys_ModuleButton moduleButtonEntity = buttondata.FirstOrDefault(t => t.F_Id == item.F_ItemId);
                            if (moduleButtonEntity != null)
                                authorizeurldata.Add(moduleButtonEntity);
                        }
                    }
                        
                }
                CacheFactory.Cache().WriteCache(authorizeurldata, "authorizebuttondata_" + userCode, DateTime.Now.AddMinutes(5));
            }
            else
            {
                authorizeurldata = cachedata;
            }
            //LogHelper.Debug("authorizebuttondata_" + userCode+"：【"+ authorizeurldata.Count+ "】", OpType.System);

            if (authorizeurldata.Any(a => a.F_EnCode == enCode))
                return true;
            return false;
        }
    }
}
