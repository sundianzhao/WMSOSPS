using System;
using System.Net;
using Newtonsoft.Json;

namespace WMSOSPS.Cloud.Code.Tools
{
    /// <summary>
    /// API接口
    /// </summary>
    public static class Api
    {
        public static readonly String FlowAPIURL = System.Configuration.ConfigurationManager.AppSettings["WorkFlowAPIURL"];


        /// <summary>
        /// Get 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static ReturnResult<T> GetApi<T>(string url)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Timeout = (int)TimeSpan.FromMinutes(20).TotalMilliseconds;
            req.Headers.Add("contentType", "application/json");
            req.Method = "GET";
            var resp = req.GetResponse();
            var str = string.Empty;
            using (var stream = resp.GetResponseStream())
            {
                var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);
                str = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }
            var obj = JsonConvert.DeserializeObject<ReturnResult<string>>(str);
            req = null;
            GC.Collect();
            //return obj;
            return new Tools.ReturnResult<T>()
            {
                ErrorCode = obj.ErrorCode
               ,
                ErrorMsg = obj.ErrorMsg
               ,
                Message = obj.Message
               ,
                JsonResult = JsonConvert.DeserializeObject<T>(obj.JsonResult)
            };
        }
        /// <summary>
        /// POST
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <param name="url"></param>
        /// <param name="m"></param>
        /// <returns></returns>
        public static ReturnResult<T> PostApi<T, M>(string url, M m) where T : new()
        {

            var obj = PostApi(url, typeof(T), m);
            var result = new Tools.ReturnResult<T>()
            {
                ErrorCode = obj.ErrorCode
                ,
                ErrorMsg = obj.ErrorMsg
                ,
                Message = obj.Message
                ,
                JsonResult = ((T)obj.JsonResult)
            };
            if (result.JsonResult == null)
            {
                result.JsonResult = new T() ;
            }
            return result;
        }

        public static ReturnResult<object> PostApi(string url, Type t, object m)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Timeout = (int)TimeSpan.FromMinutes(20).TotalMilliseconds;
            // req.Headers.Add("Content-Type", "application/json");
            req.ContentType = "application/json";
            req.Method = "POST";
            var str = string.Empty;
            byte[] bs = null;

            str = JsonConvert.SerializeObject(m);
            var strtmp = str;
            bs = System.Text.Encoding.UTF8.GetBytes(str);
            using (var stream = req.GetRequestStream())
            {
                stream.Write(bs, 0, bs.Length);
            }

            bs = null;
            //   str = string.Empty;
            var resp = req.GetResponse();
            using (var stream = resp.GetResponseStream())
            {
                var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);
                str = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();
            }
            var obj = JsonConvert.DeserializeObject<ReturnResult<string>>(str);
            resp.Close();
            resp = null;
            req = null;
            GC.Collect();
            return new Tools.ReturnResult<object>()
            {
                ErrorCode = obj.ErrorCode
                ,
                ErrorMsg = obj.ErrorMsg
                ,
                Message = obj.Message
                ,
                JsonResult = JsonConvert.DeserializeObject(obj.JsonResult ?? "", t)
            };
        }
    }
}
