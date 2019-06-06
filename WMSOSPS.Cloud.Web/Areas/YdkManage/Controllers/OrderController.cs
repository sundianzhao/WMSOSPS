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
    public class OrderController : Handler.ControllerBase
    {
        public ActionResult GetGridJson(Pagination pagination, string location,string d1,string d2,string strNo)
        {
            using (var app = new OrderApp())
            {
                var data = new
                {
                    rows = app.GetGridJson(pagination, location,d1,d2,strNo),
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records
                };
                return Content(data.ToJson());
            }
        }
    }
}