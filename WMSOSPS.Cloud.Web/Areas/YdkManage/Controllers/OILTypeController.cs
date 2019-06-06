using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMSOSPS.Cloud.Application.YkdManage;
using WMSOSPS.Cloud.Code.Json;
using WMSOSPS.Cloud.Code.Web;

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
    }
}