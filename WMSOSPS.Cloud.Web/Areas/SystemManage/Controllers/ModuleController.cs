using WMSOSPS.Cloud.Application.SystemManage;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Json;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Code.Web.Tree;
using WMSOSPS.Cloud.Code.Web.TreeGrid;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Web.Handler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WMSOSPS.Cloud.Web.Areas.SystemManage.Controllers
{
    public class ModuleController : Handler.ControllerBase
    {
        private ModuleApp moduleApp = new ModuleApp();
         
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetTreeSelectJson()
        {
            var data = moduleApp.GetList();
            var treeList = new List<TreeSelectModel>();
            foreach (Sys_Module item in data)
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
            var data = moduleApp.GetList();
            if (!string.IsNullOrEmpty(keyword))
            {
                data = data.TreeWhere(t => t.F_FullName.Contains(keyword));
            }
            var treeList = new List<TreeGridModel>();
            foreach (Sys_Module item in data)
            {
                TreeGridModel treeModel = new TreeGridModel();
                bool hasChildren = data.Count(t => t.F_ParentId == item.F_Id) != 0;
                treeModel.id = item.F_Id;
                treeModel.isLeaf = hasChildren;
                treeModel.parentId = item.F_ParentId;
                treeModel.expanded = false;
                treeModel.entityJson = item.ToJson();
                treeList.Add(treeModel);
            }
            return Content(treeList.TreeGridJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = moduleApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(Sys_Module moduleEntity, string keyValue)
        {
            moduleApp.SubmitForm(moduleEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            moduleApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }

        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult GetIconsList(int pageIndex = 1)
        {
            string regex = "icon-([a-z|-]+):before";//"^\\.*:before*";
            var configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\css\\iconfont\\iconfont.css"));
            string content = System.IO.File.ReadAllText(configFile.ToString());
            List<string> arrayList = RegexUtils.RegexMatches(regex, content, RegexOptions.None);
            int pageSize = 35;
            var totalRecords = arrayList.Count;
            Dictionary<string, object> dicObject = new Dictionary<string, object>();
            dicObject.Add("pno", pageIndex);
            int total = totalRecords % pageSize == 0 ? totalRecords / pageSize : totalRecords / pageSize + 1;
            var list = arrayList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            List<string> arrList = list.Select(item => item.Split(':')[0]).Select(mark => "icon iconfont " + mark).ToList();
            dicObject.Add("total", total);
            dicObject.Add("totalRecords", totalRecords);
            dicObject.Add("list", arrList);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("result", "success");
            dic.Add("msg", dicObject);
            return Json((new JavaScriptSerializer()).Serialize(dic), JsonRequestBehavior.AllowGet);

        }


        //[HttpPost]
        //[HandlerAjaxOnly]
        //public ActionResult GetIconsList(int pageIndex = 1)
        //{
        //    string regex = "fa-([a-z|-]+):before";//"^\\.*:before*";
        //    var configFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\css\\framework-font.css"));
        //    string content = System.IO.File.ReadAllText(configFile.ToString());
        //    List<string> arrayList = RegexUtils.RegexMatches(regex, content, RegexOptions.None);
        //    //arrayList.RemoveRange(0, 5);D:\SVN\400云平台\400Management\ZH.Cloud.Web\Content\css\iconfont\iconfont.css
        //    configFile= new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\css\\iconfont\\iconfont.css"));
        //    content = System.IO.File.ReadAllText(configFile.ToString());
        //    regex= "icon-([a-z|-]+):before";//"^\\.*:before*";

        //    List<string> arrayList1 = RegexUtils.RegexMatches(regex, content, RegexOptions.None);
        //    arrayList.AddRange(arrayList1);
        //    int pageSize = 35;
        //    var totalRecords = arrayList.Count;
        //    Dictionary<string, object> dicObject = new Dictionary<string, object>();
        //    dicObject.Add("pno", pageIndex);
        //    int total = totalRecords % pageSize == 0 ? totalRecords / pageSize : totalRecords / pageSize + 1;
        //    var list = arrayList.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

        //    List<string> arrList = list.Select(item => item.Split(':')[0]).Select(mark =>
        //    {
        //        if (mark.Contains("fa"))
        //            return "fa " + mark + " fa-lg";
        //        else
        //            return mark + " icon-large";

        //    }).ToList();
        //    dicObject.Add("total", total);
        //    dicObject.Add("totalRecords", totalRecords);
        //    dicObject.Add("list", arrList);

        //    Dictionary<string, object> dic = new Dictionary<string, object>();
        //    dic.Add("result", "success");
        //    dic.Add("msg", dicObject);
        //    return Json((new JavaScriptSerializer()).Serialize(dic), JsonRequestBehavior.AllowGet);

        //}
    }
}