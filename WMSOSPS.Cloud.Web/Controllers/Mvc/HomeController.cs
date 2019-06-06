using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WMSOSPS.Cloud.Web.Controllers
{
    public class HomeController : Handler.ControllerBase
    {
      
        public ActionResult Default()
        {
            return View();
        }
        
    }
}