 
using System.Web.Mvc;
using WMSOSPS.Cloud.Code.Operator;
using WMSOSPS.Cloud.Code.Web;


namespace WMSOSPS.Cloud.Web.Handler
{
    public class HandlerLoginAttribute : AuthorizeAttribute
    {
        public bool Ignore = true;
        public HandlerLoginAttribute(bool ignore = true)
        {
            Ignore = ignore;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            OperatorModel op = OperatorProvider.Provider.GetCurrent();
            if (Ignore == false)
            {
                return;
            }
            if (string.IsNullOrEmpty(op?.UserCode))
            {
                WebHelper.WriteCookie("nfine_login_error", "overdue");
                filterContext.HttpContext.Response.Write("<script>alert('登录超时，请重新登录！');top.location.href = '/Login/Index';</script>");
                return;
            }
            var code = OperatorProvider.CheckRight(op);
            if (code != Code.Enum.EError.NoError)
            {
                filterContext.HttpContext.Response.Write("<script>alert('当前帐号在别处登录！');top.location.href = '/Login/Index';</script>");
                return;
            }
        }
    }
}