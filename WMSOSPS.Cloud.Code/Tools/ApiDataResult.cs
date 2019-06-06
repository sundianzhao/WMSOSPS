using System;

namespace WMSOSPS.Cloud.Code.Tools
{
    /// <summary>
    /// API 通用返回对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiDataResult
    {
        public ApiDataResult(int errorCode, string jsonResult, string errMsg = "", Exception ex = null)
        {
            this.ErrorCode = errorCode;
            this.ErrorMsg = errMsg;
            this.JsonResult = jsonResult;
            this.Message = ex == null ? "" : ex.Message;
        }
        public int ErrorCode;
        public string ErrorMsg;
        public string JsonResult;
        public string Message;
    }
}
