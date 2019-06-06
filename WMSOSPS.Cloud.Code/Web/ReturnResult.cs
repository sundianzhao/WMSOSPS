using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WMSOSPS.Cloud.Code.Web
{
    public class ReturnResult
    {
        public bool ActionResult { get; set; }

        public string Msg { get; set; }
        public bool IsShowFail { get; set; }
        public int Total { get; set; }
        public int SuccessCount { get; set; }
        public int FailCount { get; set; }
        public JArray Rows { get; set; }

        public JObject Result { get; set; }
        public JArray GetResult()
        {
            JArray jr = new JArray();
            JObject jobj = new JObject(
                           new JProperty("ActionResult", ActionResult),
                           new JProperty("Msg", Msg),
                           new JProperty("IsShowFail", IsShowFail),
                           new JProperty("Total", Total),
                           new JProperty("Rows", Rows),
                           new JProperty("Result", Result)
                          );
            jr.Add(jobj);
            return jr;
        }
    }
}
