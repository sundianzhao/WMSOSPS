using System.Web.Mvc;

namespace WMSOSPS.Cloud.Web.Areas.YdkManage
{
    public class YdkManageAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "YdkManage";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
              this.AreaName + "_Default",
              this.AreaName + "/{controller}/{action}/{id}",
              new { area = this.AreaName, controller = "Home", action = "Index", id = UrlParameter.Optional },
              new string[] { "WMSOSPS.Cloud.Web.Areas." + this.AreaName + ".Controllers" }
            );
        }
    }
}