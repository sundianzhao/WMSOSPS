using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WMSOSPS.Cloud.Application.SystemSecurity;
using WMSOSPS.Cloud.Code;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Extend;
using WMSOSPS.Cloud.Code.Json;
using WMSOSPS.Cloud.Code.Logger;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Security;
using WMSOSPS.Cloud.Domain.ViewModel;
using WMSOSPS.Cloud.Web.Handler;

namespace WMSOSPS.Cloud.Web.Controllers.Mvc
{
    public class LoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IndexT()
        {
            return View();
        }
        [HttpGet]
        public ActionResult OutLogin()
        {
            Session.Abandon();
            Session.Clear();
            OperatorProvider.Provider.RemoveCurrent();
            return RedirectToAction("Index", "Login");
        }
        /// <summary>
        /// 管理员登陆
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult CheckLogin(string username, string password, string code)
        { 
            LoginContent result = new LoginContent()
            {
                IsSmsLogin = "0",
                Msg = "用户名密码错误",
                Success = false,
                MoPhone = "",
                LoginType = (int)LoginTypeEnum.ManageMember
            }; 
            try
            {
                if (string.IsNullOrEmpty(username))
                { 
                    result.Msg = "用户名不能为空";
                    return Content(result.ToJson());
                }
                if (string.IsNullOrEmpty(password))
                {
                    result.Msg = "请输入密码";
                    return Content(result.ToJson());
                }
                if (Session["nfine_session_verifycode"].IsEmpty() || Md5.md5(code.ToLower(), 16) != Session["nfine_session_verifycode"].ToString())
                {
                    result.Msg = "验证码错误，请重新输入"; 
                    return Content(result.ToJson());
                }
                using (var bll = new LoginApp())
                { 
                    result = bll.CheckLogin(username, password); 
                }
                if (!result.Success)
                {
                    result.Msg = "用户名密码错误"; 
                }
                return Content(result.ToJson());
            }
            catch (Exception ex)
            { 
                result.Msg = ex.Message;
                result.Success = false;
                return Content(result.ToJson());
            }
        }
 
        [HttpGet]
        public ActionResult GetAuthCode()
        {
            return File(new VerifyCode().GetVerifyCode(), @"image/Gif");
        }
    }
}