using WMSOSPS.Cloud.Application.SystemManage;
using WMSOSPS.Cloud.Code.Json;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Web.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc; 
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Code;
using System.Text;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Application.YkdManage;

namespace WMSOSPS.Cloud.Web.Controllers.Mvc
{
    // GET: ClientsData
    [HandlerLogin]
    public class ClientsDataController : Controller
    {
        [HttpGet]
        [HandlerAjaxOnly]
        //[AllowAnonymous]
        public ActionResult GetClientsDataJson()
        {
            var data = new
            {
                dataItems = this.GetDataItemList(),//字典表
                organize = this.GetOrganizeList(),//组织架构
                role = this.GetRoleList(),//字典型角色《key：f_id,value:匿名对象，code,name》
                duty = this.GetDutyList(),//跟角色是一样的
                user = OperatorProvider.Provider.GetCurrent(),//当前用户
                authorizeMenu = this.GetMenuList(),//菜单
                authorizeButton = this.GetMenuButtonList(),//按钮(按照每个页面进行分组的按钮)
                buttons = this.GetButtonList(),//所有的按钮，没有进行页面分组的 
                province = GetPrivinceList(),//获取省份
                city = GetCityList(),//获取城市  
                company=GetCompany()
            };
            return Content(data.ToJson());
        }
        private object GetCompany()
        {
            List<T_Enterprise> data = (List<T_Enterprise>)new YdkApp().GetEnterprise();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (T_Enterprise item in data)
            {
                var fieldItem = new
                {
                    encode = item.F_Code,
                    fullname = item.F_Name
                };
                dictionary.Add(item.F_Code, fieldItem);
            } 
            return dictionary;
        }
        private object GetDataItemList()
        {
            var itemdata = new ItemsDetailApp().GetList();
            Dictionary<string, object> dictionaryItem = new Dictionary<string, object>();
            foreach (var item in new ItemsApp().GetList())
            {
                var dataItemList = itemdata.FindAll(t => t.F_ItemId.Equals(item.F_Id));
                Dictionary<string, string> dictionaryItemList = new Dictionary<string, string>();
                foreach (var itemList in dataItemList)
                {
                    dictionaryItemList.Add(itemList.F_ItemCode, itemList.F_ItemName);
                }
                dictionaryItem.Add(item.F_EnCode, dictionaryItemList);
            }
            //Common.BIBasicList.Add("dataItems", dictionaryItem);
            // Common.BIBasicList["dataItems"] = dictionaryItem;
            return dictionaryItem;
        }

        public object GetSubstarctReason(string code)
        {
            return new ItemsDetailApp().GetItemList(code);
        }
        private object GetOrganizeList()
        {
            OrganizeApp organizeApp = new OrganizeApp();
            var data = organizeApp.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (Sys_Organize item in data)
            {
                var fieldItem = new
                {
                    encode = item.F_EnCode,
                    fullname = item.F_FullName
                };
                dictionary.Add(item.F_Id, fieldItem);
            }
            Common.BIBasicList["organize"] = dictionary;
            return dictionary;
        }
        private object GetRoleList()
        {
            RoleApp roleApp = new RoleApp();
            var data = roleApp.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (Sys_Role item in data)
            {
                var fieldItem = new
                {
                    encode = item.F_EnCode,
                    fullname = item.F_FullName
                };
                dictionary.Add(item.F_Id, fieldItem);
            }
            Common.BIBasicList["role"] = dictionary;
            return dictionary;
        }
     
        private object GetDutyList()
        {
            DutyApp dutyApp = new DutyApp();
            var data = dutyApp.GetList();
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (Sys_Role item in data)
            {
                var fieldItem = new
                {
                    encode = item.F_EnCode,
                    fullname = item.F_FullName
                };
                dictionary.Add(item.F_Id, fieldItem);
            }
            Common.BIBasicList["duty"] = dictionary;
            return dictionary;
        }
        private object GetMenuList()
        {
            string loginUserCode = string.Empty;
            List<Sys_Module> roleList = new List<Sys_Module>();
            var cache = OperatorProvider.Provider.GetCurrent();

            var roleId = cache.RoleId;
            var userCode = cache.UserCode;


            roleList = new RoleAuthorizeApp().GetMenuList(roleId);
            if (cache.IsSystem) return ToMenuJson(roleList, "0");


            //如果是代理商助理登录，取得助理个人的权限；否则，取得角色的权限
            if (string.IsNullOrEmpty(cache.AssistantID))
            {
                loginUserCode = userCode;
                var userroleList = new RoleAuthorizeApp().GetSpecialMenuList(loginUserCode);
                roleList.AddRange(userroleList);
            }
            else
            {
                loginUserCode = cache.AssistantID;
                var userroleList = new RoleAuthorizeApp().GetSpecialMenuList(loginUserCode);
                if (userroleList.Count > 1) { roleList = userroleList; }
            }


            var newNameList = roleList.Distinct(new Compare<Sys_Module>(
                delegate (Sys_Module x, Sys_Module y)
                {
                    if (null == x || null == y) return false;
                    return x.F_Id == y.F_Id;
                })).ToList();
            string mu = ToMenuJson(newNameList, "0");
            Common.BIBasicList["authorizeMenu"] = mu;
            return mu;
        }

        /// <summary>
        /// 获取特殊的权限(根据某些账号去设置权限)
        /// </summary>
        private object GetSpecialMenuList()
        {
            var userCode = OperatorProvider.Provider.GetCurrent().UserCode;
            return ToMenuJson(new RoleAuthorizeApp().GetSpecialMenuList(userCode), "0");
        }
        //private object GetMenuList()
        //{
        //    var roleId = OperatorProvider.Provider.GetCurrent().RoleId;
        //    using (var bll = new RoleBll())
        //    {
        //        var data = bll.GetMenuList(roleId);
        //        return ToMenuJson(data, "0");
        //    }

        //}
        private string ToMenuJson(List<Sys_Module> data, string parentId)
        {
            StringBuilder sbJson = new StringBuilder();
            sbJson.Append("[");
            List<Sys_Module> entitys = data.FindAll(t => t.F_ParentId == parentId);
            if (entitys.Count > 0)
            {
                foreach (var item in entitys)
                {
                    string strJson = item.ToJson();
                    strJson = strJson.Insert(strJson.Length - 1, ",\"ChildNodes\":" + ToMenuJson(data, item.F_Id) + "");
                    sbJson.Append(strJson + ",");
                }
                sbJson = sbJson.Remove(sbJson.Length - 1, 1);
            }
            sbJson.Append("]");
            return sbJson.ToString();
        }
        private object GetMenuButtonList()
        {
            var cache = OperatorProvider.Provider.GetCurrent();

            string userCode = "";

            //如果是代理商助理登录，取得助理个人的权限；否则，取得角色的权限
            var data = new RoleAuthorizeApp().GetButtonList(cache.RoleId);
            if (string.IsNullOrEmpty(cache.AssistantID))
            {
                userCode = OperatorProvider.Provider.GetCurrent().UserCode;
                var userroleList = new RoleAuthorizeApp().GetSpecialButtonList(userCode);
                data.AddRange(userroleList);
            }
            else
            {
                userCode = OperatorProvider.Provider.GetCurrent().AssistantID;
                var userroleList = new RoleAuthorizeApp().GetSpecialButtonList(userCode);
                if (userroleList.Count > 1) { data = userroleList; }
            }

            var dataModuleId = data.Distinct(new ExtList<Sys_ModuleButton>("F_ModuleId"));
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            foreach (Sys_ModuleButton item in dataModuleId)
            {
                var buttonList = data.Where(t => t.F_ModuleId.Equals(item.F_ModuleId));
                dictionary.Add(item.F_ModuleId, buttonList);
            }
            //Common.BIBasicList["authorizeButton"] = data;
            return dictionary;
        }

        private object GetButtonList()
        {
            var roleId = OperatorProvider.Provider.GetCurrent().RoleId;
            var userCode = OperatorProvider.Provider.GetCurrent().UserCode;
            var data = new RoleAuthorizeApp().GetButtonList(roleId);
            if (!OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                var data1 = new RoleAuthorizeApp().GetSpecialButtonList(userCode);
                data.AddRange(data1);
            }
            return data;
        }

        private object GetPrivinceList()
        {
            return new AreaApp().GetPrivince();
        }
        private object GetCityList()
        {
            return new AreaApp().GetCity();
        }








    }
}