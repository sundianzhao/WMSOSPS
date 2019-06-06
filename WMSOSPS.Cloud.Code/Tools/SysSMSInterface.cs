using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json.Linq;
using WMSOSPS.Cloud.Code.Security;
using System.Threading.Tasks;

namespace WMSOSPS.Cloud.Code.Tools
{
    public partial class Sms_CategoryID
    {
        /// <summary>
        /// 1.	客服审核通过，提醒代理商充值
        /// </summary>
        public static string open_audit = "open_audit";
        /// <summary>
        /// 2.	客服启用号码，提醒代理商号码已开通
        /// </summary>
        public static string open_enable = "open_enable";
        /// <summary>
        /// 客服启用号码，提醒400用户号码已开通
        /// </summary>
        public static string open_enable_enterprise = "open_enable_enterprise";
        /// <summary>
        /// 3.	申请开通增值功能，客服审核通过后，提醒代理商已开通
        /// </summary>
        public static string open_extras = "open_extras";
        /// <summary>
        /// 申请开通增值功能，客服审核通过后，提醒400客户已开通
        /// </summary>
        public static string open_extras_enterprise = "open_extras_enterprise";
        /// <summary>
        /// 1.	财务给代理商充值后，提醒代理商已充值
        /// </summary>
        public static string Agent_pay = "fee_agent_pay";
        /// <summary>
        /// 2.	400用户增值功能充值后，提醒400用户已充值
        /// </summary>
        public static string Agent_Enterprise_extras = "fee_enterprise_extras_pay";
        /// <summary>
        /// 3.	财务给代理商回收后，提醒代理商已回收
        /// </summary>
        public static string Agent_back = "fee_agent_back";
        /// <summary>
        /// 4.	财务给代理商返款后，提醒代理商已返款
        /// </summary>
        public static string Agent_rebate = "fee_agent_rebate";
        /// <summary>
        /// 1.	修改座席电话，通知400用户座席发生改变
        /// </summary>
        public static string Warn_worker_change = "warn_worker_change";
        /// <summary>
        /// 2.	座席停用，通知400用户座席发生改变
        /// </summary>
        public static string Warn_worker_stop = "warn_worker_stop";
        /// <summary>
        /// 3.	400用户密码修改后，通知400客户密码发生改变
        /// </summary>
        public static string Warn_pwd_change = "warn_pwd_change";
        /// <summary>
        /// 4.	400用户登录异常，通知400用户有异常登录
        /// </summary>
        public static string Warn_enterprise_login = "warn_enterprise_login";
        /// <summary>
        /// 5.	代理商登录异常，通知代理商有异常登录
        /// </summary>
        public static string Warn_agent_login = "warn_agent_login";
        /// <summary>
        /// 1.	400用户话费不足时，提醒用户续费
        /// </summary>
        public static string urge_callinbalance = "urge_callinbalance";
        /// <summary>
        /// 2.	400用户增值功能到期，提醒用户续费
        /// </summary>
        public static string urge_extras = "urge_extras";
        /// <summary>
        /// 3.	400用户套餐到期，提醒用户续费
        /// </summary>
        public static string urge_enddate = "urge_enddate";
    }
    public class Sms_ReplaceWord
    {
        public Sms_ReplaceWord()
        {
            this.AgentID = string.Empty;
            this.Balance = string.Empty;
            this.EnterpriseID = string.Empty;
            this.Extras = string.Empty;
            this.Fee = string.Empty;
            this.FormatTime = string.Empty;
            this.LoginAddress = string.Empty;
            this.LoginIP = string.Empty;
            this.Sign = string.Empty;
            this.WorkerNumber = string.Empty;
            this.WorkerPhone = string.Empty;
            this.FeeType = string.Empty;
            this.NumFeeType = string.Empty;
        }
        /// <summary>
        /// 400号码
        /// </summary>
        public string EnterpriseID { get; set; }
        /// <summary>
        /// 短信最后签名
        /// </summary>
        public string Sign { get; set; }
        /// <summary>
        /// 格式化之后的时间
        /// </summary>
        public string FormatTime { get; set; }
        /// <summary>
        /// 增值功能名称
        /// </summary>
        public string Extras { get; set; }
        /// <summary>
        /// 代理商账号
        /// </summary>
        public string AgentID { get; set; }
        /// <summary>
        /// 余额，可以是正常账号，返款账号等等
        /// </summary>
        public string Balance { get; set; }
        /// <summary>
        /// 操作金额，可以是充值，回收，返款等等
        /// </summary>
        public string Fee { get; set; }
        /// <summary>
        /// 坐席工号
        /// </summary>
        public string WorkerNumber { get; set; }
        /// <summary>
        /// 坐席电话
        /// </summary>
        public string WorkerPhone { get; set; }
        /// <summary>
        /// 登录IP
        /// </summary>
        public string LoginIP { get; set; }
        /// <summary>
        /// 登录地址
        /// </summary>
        public string LoginAddress { get; set; }
        /// <summary>
        /// 费用类型：功能费还是呼入费
        /// </summary>
        public string FeeType { get; set; }
        /// <summary>
        /// 号段
        /// </summary>
        public string NumFeeType { get; set; }

    }

    public class SysSMSInterface
    {
        public static JObject GetJsonObject(string jsonString)
        {
            return JObject.Parse(jsonString);
        }
        public static string GetJsonObject(JObject jsnValues, string filedName)
        {
            JToken token = jsnValues[filedName];
            if (token != null && token.Type != JTokenType.Null)
                return Convert.ToString(token).Replace("\"", string.Empty);
            return string.Empty;
        }
        public static Dictionary<string, string> JObj2Dict(string result)
        {
            JObject jo = SysSMSInterface.GetJsonObject(result);
            return jo.Children()
                .Cast<JProperty>()
                .ToDictionary(item => item.Name, item => SysSMSInterface.GetJsonObject(jo, item.Name));
        }
      

       
    }
}
