using WMSOSPS.Cloud.Application.SystemManage;
using WMSOSPS.Cloud.Code.Json;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Data.CloudContext;
using WMSOSPS.Cloud.Web.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMSOSPS.Cloud.Web.Areas.SystemManage.Controllers
{
    public class UserController : Handler.ControllerBase
    {
        private UserApp userApp = new UserApp();
        private UserLogOnApp userLogOnApp = new UserLogOnApp();
         
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetGridJson(Pagination pagination, string keyword, string roleid)
        {

            var data = new
            {
                rows = userApp.GetList(pagination, keyword, roleid),
                total = pagination.total,
                page = pagination.page,
                records = pagination.records
            };
            return Content(data.ToJson());
        }
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetFormJson(string keyValue)
        {
            var data = userApp.GetForm(keyValue);
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(Sys_User userEntity, Sys_UserLogOn userLogOnEntity, string keyValue)
        {
            userApp.SubmitForm(userEntity, userLogOnEntity, keyValue);
            return Success("操作成功。");
        }
        [HttpPost]
        [HandlerAuthorize]
        [HandlerAjaxOnly]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteForm(string keyValue)
        {
            userApp.DeleteForm(keyValue);
            return Success("删除成功。");
        }
        [HttpGet]
        public ActionResult RevisePassword()
        {
            return View();
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitRevisePassword(string userPassword, string keyValue, string account)
        {
            userLogOnApp.RevisePassword(userPassword, keyValue, account);
            return Success("重置密码成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult DisabledAccount(string keyValue, string F_Account)
        {
            Sys_User userEntity = new Sys_User();
            userEntity.F_Id = keyValue;
            userEntity.F_EnabledMark = false;
            userEntity.F_Account = F_Account;
            //userApp.UpdateForm(userEntity);
            userApp.DisabledAccount(userEntity);
            return Success("账户禁用成功。");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        [HandlerAuthorize]
        [ValidateAntiForgeryToken]
        public ActionResult EnabledAccount(string keyValue, string F_Account)
        {
            Sys_User userEntity = new Sys_User();
            userEntity.F_Id = keyValue;
            userEntity.F_EnabledMark = true;
            userEntity.F_Account = F_Account;
            userApp.DisabledAccount(userEntity);
            return Success("账户启用成功。");
        }

        [HttpGet]
        public ActionResult Info()
        {
            return View();
        }

        public ActionResult ShowAffiliation()
        { 
            return View();
        }

        

        [HttpGet]
        [HandlerAuthorize]
        public virtual ActionResult Role()
        {
            return View();
        } 
       
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public ActionResult UpdadteUserPwd(string pwd)
        {
            using (var app = new UserApp())
            {
                var data = app.UpdadteUserPwd(pwd);
                return Content(data.ToJson());
            }
        }

    }
}