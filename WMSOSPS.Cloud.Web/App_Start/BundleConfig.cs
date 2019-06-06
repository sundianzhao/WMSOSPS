using System.Web;
using System.Web.Optimization;

namespace WMSOSPS.Cloud.Web
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/linq/js").Include(
                "~/Content/js/linq/linq.js"
            ));
            bundles.Add(new ScriptBundle("~/base/js").Include(
                "~/Content/js/jquery/jquery-2.1.1.min.js",
                "~/Content/js/bootstrap/bootstrap.js",
                "~/Content/js/icheck/icheck.js"
            ));
        }
    }
}
