using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMSOSPS.Cloud.Application.YkdManage;
using WMSOSPS.Cloud.Code.Json;
using WMSOSPS.Cloud.Code.Web;
using WMSOSPS.Cloud.Data.CloudContext;

namespace WMSOSPS.Cloud.Web.Areas.YdkManage.Controllers
{
    public class YdkController : Handler.ControllerBase
    {
        // GET: YdkManage/Ydk
        public ActionResult YdkIndex()
        {
            return View();
        }
        public ActionResult GetGridJson(Pagination pagination, string keyword)
        {
            using (var app = new YdkApp())
            {
                var data = new
                {
                    rows = app.GetGridJson(pagination, keyword),
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records
                };
                return Content(data.ToJson());
            }
        }
        public ActionResult YdkSetIndex()
        {
            return View();
        }
        public ActionResult YdkForm()
        {
            return View();
        }
        public ActionResult Sumbit(T_WMS entity)
        {
            using (var app = new YdkApp())
            {
                var data = app.Sumbit(entity);
                return Content(data.ToJson());
            }
        }

        public ActionResult DeleteInfo(string keyValue)
        {
            using (var app = new YdkApp())
            {
                var data = app.DeleteInfo(keyValue);
                return Content(data.ToJson());
            }
        }

        public ActionResult GetEnterprise()
        {
            using (var app = new YdkApp())
            {
                var data = app.GetEnterprise();
                return Content(data.ToJson());
            }
        }
        public ActionResult GetYdkAll(string UserCode)
        {
            using (var app = new YdkApp())
            {
                var data = app.GetYdkAll(UserCode);
                return Content(data.ToJson());
            }
        }
        public ActionResult GetBillMethod()
        {
            using (var app = new YdkApp())
            {
                var data = app.GetBillMethod();
                return Content(data.ToJson());
            }
        }

        public ActionResult SetYdk(string UserCode, string F_WMSCodes)
        {
            using (var app = new YdkApp())
            {
                var data = app.SetYdk(UserCode, F_WMSCodes);
                return Content(data.ToJson());
            }
        }
    }
}