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
    public class CarController : Handler.ControllerBase
    {
        #region 已进场车辆
        /// <summary>
        /// 已进场车辆
        /// </summary>
        /// <returns></returns>
        public ActionResult LoginTruckIndex()
        {
            return View();
        }
        public ActionResult LoginTruckForm()
        {
            return View();
        }
        public ActionResult GetLoginGridJson(Pagination pagination, string location)
        {
            using (var app = new CarApp())
            {
                var data = new
                {
                    rows = app.GetLoginGridJson(pagination, location),
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records
                };
                return Content(data.ToJson());
            }
        }

        public ActionResult GetLoginFormJson(string F_OrderNo)
        {
            using (var app=new CarApp())
            {
                var data = app.GetLoginFormJson(F_OrderNo);
                return Content(data.ToJson());
            }
        }

        public ActionResult AuditForm(string F_OrderNo, string TruckWeight, string GrossWeight, string NetWeight)
        {
            using (var app=new CarApp())
            {
                var data = app.AuditForm(F_OrderNo, TruckWeight, GrossWeight, NetWeight);
                return Content(data.ToJson());
            }
        }
        #endregion

        #region 待审批车辆
        /// <summary>
        /// 待审批车辆
        /// </summary>
        /// <returns></returns>
        public ActionResult GrossTruckIndex()
        {
            return View();
        } 
        public ActionResult GrossTruckForm()
        {
            return View();
        }
        public ActionResult GetGrossGridJson(Pagination pagination, string location)
        {
            using (var app = new CarApp())
            {
                var data = new
                {
                    rows = app.GetGrossGridJson(pagination, location),
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records
                };
                return Content(data.ToJson());
            }
        }
        public ActionResult GetGrossFormJson(string F_OrderNo)
        {
            using (var app = new CarApp())
            {
                var data = app.GetGrossFormJson(F_OrderNo);
                return Content(data.ToJson());
            }
        }
        #endregion

        #region 待结算车辆
        /// <summary>
        /// 待结算车辆
        /// </summary>
        /// <returns></returns>
        public ActionResult UnFinishedTruckIndex()
        {
            return View();
        }
        
        public ActionResult UnFinishedTruckForm()
        {
            return View();
        }
        public ActionResult GetUnFinishedGridJson(Pagination pagination, string location)
        {
            using (var app = new CarApp())
            {
                var data = new
                {
                    rows = app.GetUnFinishedGridJson(pagination, location),
                    total = pagination.total,
                    page = pagination.page,
                    records = pagination.records
                };
                return Content(data.ToJson());
            }
        }
        public ActionResult GetUnFinishedFormJson(string F_OrderNo)
        {
            using (var app = new CarApp())
            {
                var data = app.GetUnFinishedFormJson(F_OrderNo);
                return Content(data.ToJson());
            }
        }
        public ActionResult Accounts(string F_OrderNo)
        {
            using (var app=new CarApp())
            {
                var data = app.Accounts(F_OrderNo);
                return Content(data.ToJson());
            }
        }

        #endregion
    }
}