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
    public class SAPOILTypeController : Handler.ControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Form()
        {
            return View();
        }

        public ActionResult GetGridJson(Pagination pagination, string keyword)
        {
            using (var app = new OILTypeApp())
            {
                var data = new
                {
                    rows = app.GetSAPGridJson(pagination, keyword),
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records
                };
                return Content(data.ToJson());
            }
        }

        public ActionResult GetYdkItem()
        {
            using (var app = new YdkApp())
            {
                var data = app.GetYdkItem(); 
                return Content(data.ToJson());
            }
        }

        public ActionResult GetOILTypes()
        {
            using (var app = new OILTypeApp())
            {
                var data = app.GetOILTypes();
                return Content(data.ToJson());
            }
        }

        public ActionResult Sumbit(T_SAPOIL entity,string keyValue)
        {
            using (var app=new OILTypeApp())
            {
                var data = app.Sumbit(entity, keyValue);
                return Content(data.ToJson());
            }
        }

        public ActionResult GetFormJson(string keyValue)
        {
            using (var app=new OILTypeApp())
            {
                var data = app.GetFormJson(keyValue);
                return Content(data.ToJson());
            }
        }
    }
}