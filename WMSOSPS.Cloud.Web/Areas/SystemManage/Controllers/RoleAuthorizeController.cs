using WMSOSPS.Cloud.Application.SystemManage;
using WMSOSPS.Cloud.Code.Web.TreeView;
using WMSOSPS.Cloud.Data.CloudContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMSOSPS.Cloud.Web.Areas.SystemManage.Controllers
{
    public class RoleAuthorizeController : Handler.ControllerBase
    {
        private RoleAuthorizeApp roleAuthorizeApp = new RoleAuthorizeApp();
        private ModuleApp moduleApp = new ModuleApp();
        private ModuleButtonApp moduleButtonApp = new ModuleButtonApp();

        public ActionResult GetPermissionTree(string roleId, string type = "role")
        {
            List<Sys_Module> moduledata = moduleApp.GetList();
            List<Sys_ModuleButton> buttondata = moduleButtonApp.GetList();
            if (type == "role")
            {
                moduledata = moduledata.Where(a => a.F_Type == 1 || a.F_Type == 0).ToList();
                buttondata = buttondata.Where(a => a.F_Type == 1 || a.F_Type == 0).ToList();
            }
            else
            {
                moduledata = moduledata.Where(a => a.F_Type == 2 || a.F_Type == 0).ToList();
                buttondata = buttondata.Where(a => a.F_Type == 2 || a.F_Type == 0).ToList();
            }

            var authorizedata = new List<Sys_RoleAuthorize>();
            if (!string.IsNullOrEmpty(roleId))
            {
                authorizedata = roleAuthorizeApp.GetList(roleId);
            }
            var treeList = new List<TreeViewModel>();
            foreach (Sys_Module item in moduledata)
            {
                TreeViewModel tree = new TreeViewModel();
                bool hasChildren = moduledata.Count(t => t.F_ParentId == item.F_Id) != 0;
                tree.id = item.F_Id;
                tree.text = item.F_FullName;
                tree.value = item.F_EnCode;
                tree.parentId = item.F_ParentId;
                tree.isexpand = false;
                tree.complete = true;
                tree.showcheck = true;
                tree.checkstate = authorizedata.Count(t => t.F_ItemId == item.F_Id);
                tree.hasChildren = true;
                //tree.img = item.F_Icon == "" ? "" : item.F_Icon;
                tree.img = "";
                treeList.Add(tree);
            }
            foreach (Sys_ModuleButton item in buttondata)
            {
                TreeViewModel tree = new TreeViewModel();
                bool hasChildren = buttondata.Count(t => t.F_ParentId == item.F_Id) != 0;
                tree.id = item.F_Id;
                tree.text = item.F_FullName;
                tree.value = item.F_EnCode;
                tree.parentId = item.F_ParentId == "0" ? item.F_ModuleId : item.F_ParentId;
                tree.isexpand = true;
                tree.complete = true;
                tree.showcheck = true;
                tree.checkstate = authorizedata.Count(t => t.F_ItemId == item.F_Id);
                tree.hasChildren = hasChildren;
                tree.img = item.F_Icon == "" ? "" : item.F_Icon;
                treeList.Add(tree);
            }
            return Content(treeList.TreeViewJson());
        }
    }
}