using System.Text;
using System.Web;
using System.Web.Mvc;
using WMSOSPS.Cloud.Application.SystemManage;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Web;

namespace WMSOSPS.Cloud.Web.Handler
{
    public class HandlerAuthorizeAttribute : ActionFilterAttribute
    {
        private bool Ignore { get; set; }
        private string AuthorizeAction { get; set; }

        public HandlerAuthorizeAttribute(bool ignore = true, string authorizeAction = "")
        {
            Ignore = ignore;
            AuthorizeAction = authorizeAction;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (OperatorProvider.Provider.GetCurrent().IsSystem)
            {
                return;
            }
            if (Ignore == false)
            {
                return;
            }
            if (!this.ActionAuthorize(filterContext))
            {
                StringBuilder sbScript = new StringBuilder();
                sbScript.Append("<script type='text/javascript'>alert('很抱歉！您的权限不足，访问被拒绝！');</script>");
                filterContext.Result = new ContentResult() { Content = sbScript.ToString() };
                return;
            }
        }
        private bool ActionAuthorize(ActionExecutingContext filterContext)
        {
            var operatorProvider = OperatorProvider.Provider.GetCurrent();
            var roleId = operatorProvider.RoleId;
            var userCode = operatorProvider.UserCode;
            //var moduleId = WebHelper.GetCookie("nfine_currentmoduleid");
            var moduleId = "";
            var action = HttpContext.Current.Request.ServerVariables["SCRIPT_NAME"].ToString();
            if (!string.IsNullOrEmpty(AuthorizeAction))//指定需要某Action权限
            {
                action = AuthorizeAction;
            }
            return new RoleAuthorizeApp().ActionValidate(roleId, userCode, moduleId, action);
        }
    }
}