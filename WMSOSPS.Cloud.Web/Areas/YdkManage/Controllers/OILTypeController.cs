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
    public class OILTypeController : Handler.ControllerBase
    {
        // GET: YdkManage/OILType
        public ActionResult GetGridJson(Pagination pagination, string keyword)
        {
            using (var app = new OILTypeApp())
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

        public ActionResult Sumbit(T_OILType entity, string keyValue)
        {
            using (var app = new OILTypeApp())
            {
                var data = app.SumbitOILType(entity, keyValue);
                return Content(data.ToJson());
            }
        }
        public ActionResult GetFormJson(string keyValue)
        {
            using (var app = new OILTypeApp())
            {
                var data = app.GetFormJsonOILType(keyValue);
                return Content(data.ToJson());
            }
        }
        public ActionResult DeleteOILType(string keyValue)
        {
            using (var app=new OILTypeApp())
            {
                var data = app.DeleteOILType(keyValue);
                return Content(data.ToJson());
            }
        }
    }
}