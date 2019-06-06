using WMSOSPS.Cloud.Application.SystemManage;
using WMSOSPS.Cloud.Code.Json;
using WMSOSPS.Cloud.Code.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMSOSPS.Cloud.Web.Areas.SystemManage.Controllers
{
    public class UploadContentController : Handler.ControllerBase
    {
       

        public ActionResult GetGridJson(Pagination pagination, string d1, string d2, int type=0)
        {
            using (var app = new UploadContentApp())
            {
                var data = new
                {
                    rows = app.GetGridJson(pagination, d1, d2, type),
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records
                };
                return Content(data.ToJson());
            }
        }

        public ActionResult AddUploadContent(string type, string FileName, string FilePath, string F_Description,string f_date)
        {
            using (var app = new UploadContentApp())
            {
                var data = app.AddUploadContent(type, FileName, FilePath, F_Description, f_date);
                return Content(data.ToJson());
            }
        }

        public ActionResult DeleteInfo(string keyValue)
        {
            using (var app = new UploadContentApp())
            {
                var data = app.DeleteInfo(keyValue);
                return Content(data.ToJson());
            }
        }
    }
}