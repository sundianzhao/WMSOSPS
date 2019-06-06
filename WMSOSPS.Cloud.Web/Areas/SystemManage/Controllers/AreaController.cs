using WMSOSPS.Cloud.Application.SystemManage;
using WMSOSPS.Cloud.Code.Json;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Code.Web.Tree;
using WMSOSPS.Cloud.Code.Web.TreeGrid;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Web.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMSOSPS.Cloud.Web.Areas.SystemManage.Controllers
{
    public class AreaController : Handler.ControllerBase
    { 

        private AreaApp areaApp = new AreaApp();

        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJson()
        {
            var data = areaApp.GetList();
            var treeList = new List<TreeSelectModel>();
            foreach (Sys_Area item in data)
            {
                TreeSelectModel treeModel = new TreeSelectModel();
                treeModel.id = item.F_Id;
                treeModel.text = item.F_FullName;
                treeModel.parentId = item.F_ParentId;
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeSelectJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeGridJson(string keyword)
        {
            var data = areaApp.GetList();
            var treeList = new List<TreeGridModel>();
            foreach (Sys_Area item in data)
            {
                TreeGridModel treeModel = new TreeGridModel();
                bool hasChildren = data.Count(t => t.F_ParentId == item.F_Id) == 0 ? false : true;
                treeModel.id = item.F_Id;
                treeModel.text = item.F_FullName;
                treeModel.isLeaf = hasChildren;
                treeModel.parentId = item.F_ParentId;
                treeModel.expanded = true;
                treeModel.entityJson = item.ToJson();
                treeList.Add(treeModel);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                treeList = treeList.TreeWhere(t => t.text.Contains(keyword), "id", "parentId");
            }
            return Content(treeList.TreeGridJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = areaApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(Sys_Area areaEntity, string keyValue)
        {
            areaApp.SubmitForm(areaEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            areaApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }

        //public ActionResult GetCountryTreeJson(string eId, string callId)
        //{
        //    var data = areaApp.GetList();
        //    data.Add(new Sys_Area()
        //    {
        //        F_Id = "0",
        //        F_EnCode = "0",
        //        F_FullName = "中国",
        //        F_ParentId = "-1",
        //    });
        //    var treeList = new List<TreeViewModel>();
        //    //查询一下哪些城市是需要选中状态的
        //    var app = new EnterpriseManagerApp();
        //    var callInList = app.GetCallInListByEid(eId, callId);
        //    var cityCode = callInList?.CityCode ?? "";
        //    var list = cityCode.Split(',').ToList();
        //    var dataList = list.Where(a => a.Length == 6).Select(a => a.Substring(0, 2)).Distinct().ToList();


        //    var codeList = dataList.Select(a => a + "0000").ToList();


        //    foreach (Sys_Area item in data)
        //    {
        //        TreeViewModel tree = new TreeViewModel();
        //        bool hasChildren = data.Count(t => t.F_ParentId == item.F_Id) != 0;

        //        tree.id = item.F_Id;
        //        tree.text = item.F_FullName;
        //        tree.value = item.F_EnCode;
        //        tree.parentId = item.F_ParentId;
        //        tree.isexpand = item.F_ParentId != "0";
        //        tree.complete = true;
        //        tree.showcheck = true;
        //        tree.checkstate = list.Count(t => t == item.F_EnCode);
        //        tree.hasChildren = hasChildren;
        //        if (codeList.Contains(item.F_EnCode))
        //        {
        //            if (!list.Contains(item.F_EnCode))
        //            {
        //                tree.checkstate = 2;
        //            }
        //        }
        //        if (item.F_EnCode == "0" && codeList.Count > 0)
        //            tree.checkstate = 2;
        //        if (item.F_EnCode == "0" && codeList.Count == 0)
        //            tree.checkstate = 0;
        //        //tree.img = item.F_Icon == "" ? "" : item.F_Icon;
        //        treeList.Add(tree);
        //    }

        //    return Content(treeList.TreeViewJson("-1"));
        //}

        public ActionResult GetCitys()
        {
            var data = areaApp.GetList();
            var result = data.Select(a => new
            {
                code = a.F_Id,
                name = a.F_FullName
            }).ToList();
            return Content(result.ToJson());
        }
    }
}