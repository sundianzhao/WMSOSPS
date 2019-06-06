using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using WMSOSPS.Cloud.Cache;
using WMSOSPS.Cloud.Code.Enum;
using WMSOSPS.Cloud.Code.Model;
using WMSOSPS.Cloud.Code.Operator;
using System.Configuration;
using WMSOSPS.Cloud.Code.Logger;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WMSOSPS.Cloud.Code.Tools
{
    public class Tool
    {
      

        public static string Address;

        public static string GetAppSetting(string key)
        {
            string Appsetting = string.Empty;

            try
            {
                Appsetting = System.Configuration.ConfigurationManager.AppSettings[key];
            }
            catch (Exception)
            {
                Appsetting = null;
            }

            return Appsetting;
        }
        /// <summary>
        /// 获取当前登录ip地址
        /// </summary>
        /// <param name="Ip"></param>
        /// <returns></returns>
        public static string GetAddressBYIP(string Ip)
        {
            string address = "";
            if (Ip.IndexOf(",") != -1)
                Ip = Ip.Substring(0, Ip.IndexOf(","));

            WebClient client = new WebClient();
            string uri = Utility.DecodeStr(GetAppSetting("IPApi")) + Ip;
            string jsonData = client.DownloadString(uri);

            IPCheckSina result = JsonConvert.DeserializeObject<IPCheckSina>(jsonData);
            if (result.ret == 1)
            {
                address = result.country + "," + result.province + "," + result.city;
            }
            return address;

        }

        public static object InsertLogLogin(string workid, string vToOject, string vOjbect, OpType vType, int modelType, int viewLevel, string fromMethod, string address)
        {
            //记录异地登录
            StringBuilder sql2 = new StringBuilder();
            sql2.AppendFormat("INSERT INTO Sys_Log(ServerID,WorkerID,Object,OperationType,WorkerKind" +
                             ",ToObjct,ModelType,ViewLevel,FromMethod,FromIP，LoginAddress,JoinTime) " +
                             "VALUES(0,'{0}','{1}','{2}',0,'{3}','{4}','{5}','{6}','{7}','{8}',getdate())", workid, vOjbect, (int)vType, vToOject, modelType, viewLevel, fromMethod, Net.Net.Ip, address);
            int res = SqlHelper.SqlHelper.InsertAndReturnID(SqlHelper.SqlHelper.connstr, CommandType.Text, sql2.ToString());
            if (res > 0)
                return true;
            return false;
        }

        public static object InsertLogSMS(string workid, string vOjbect, OpType vType, int modelType, int viewLevel, int workerkind, string fromMethod)
        {

            StringBuilder sql2 = new StringBuilder();
            sql2.AppendFormat("INSERT INTO Sys_Log(ServerID,WorkerID,Object,OperationType,WorkerKind" +
                             ",ModelType,ViewLevel,FromMethod,FromIP,JoinTime) " +
                             "VALUES(0,'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',getdate())", workid, vOjbect, (int)vType, workerkind, modelType, viewLevel, fromMethod, Net.Net.Ip);
            int res = SqlHelper.SqlHelper.InsertAndReturnID(SqlHelper.SqlHelper.connstr, CommandType.Text, sql2.ToString());
            if (res > 0)
                return true;
            return false;
        }
        /// <summary>
        /// 匿名类传递使用
        /// </summary>
        /// <typeparam name="T">泛型实体</typeparam>
        /// <param name="anonymous">Objct泛型数据体</param>
        /// <param name="anonymousType">泛型类型结构</param>
        /// <returns></returns>
        public static T CastAnonymous<T>(object anonymous, T anonymousType)
        {
            return (T)anonymous;
        }

        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="vToOject">被操作对象</param>
        /// <param name="vOjbect">操作过程描述</param>
        /// <param name="vType">操作类型</param>
        /// <param name="modelType">模块类型（0：基础模块，1：代理模块，2：企业模块）</param>
        /// <param name="viewLevel">查看等级（0：所有人可查看，1：只允许管理员，2：允许代理、管理员，3：允许企业、管理员，4：只允许技术查看）</param>
        /// <param name="fromMethod">触发记录的方法</param>
        public static object InsertLog(string vToOject, string vOjbect, OpType vType, int modelType, int viewLevel, string fromMethod)
        {
            IMember curuser = new UIPremission().CurrentUser;
            int nWorkerKind = 0;
            string strRoleCode = "";
            string workerid = "";
            string address = "";
            if (curuser != null)
            {
                address = curuser.Address;
                strRoleCode = curuser.Role_Code;
                workerid = curuser.WorkerID;

                switch (curuser.LoginType)
                {
                    case LoginTypeEnum.ManageMember:
                        if (strRoleCode.Length == 1)
                            nWorkerKind = 0;//超级管理员
                        else if (strRoleCode.Length < 4)
                        {
                            string code = strRoleCode.Substring(0, 2);
                            if (code == "03")
                                nWorkerKind = 6;//财务
                            else
                                nWorkerKind = 1;//管理员 

                        }
                        else
                        {
                            string rcode = strRoleCode.Substring(0, 2);
                            if (rcode == "04")
                                nWorkerKind = 4;//渠道
                            else
                                nWorkerKind = 5;//客服
                        }
                        break;
                    case LoginTypeEnum.ExamMember:
                        nWorkerKind = 2;//考生 
                        break;
                    
                }
            }

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("INSERT INTO Sys_Log(ServerID,WorkerID,Object,OperationType" +
                             ",WorkerKind,ToObjct,ModelType,ViewLevel,FromMethod,FromIP,JoinTime) " +
                             "VALUES(0,'{0}','{1}','{2}','{3},'{4}','{5}','{6}','{7}','{8}',getdate())", workerid, vOjbect, (int)vType, nWorkerKind, vToOject, modelType, viewLevel, fromMethod, Net.Net.Ip);

            int res = SqlHelper.SqlHelper.InsertAndReturnID(SqlHelper.SqlHelper.connstr, CommandType.Text, sql.ToString());
            return res;
        }

        private static void IPSourceMethod(string Id, string Ip)
        {
            try
            {
                if (string.IsNullOrEmpty(Ip))
                    return;

                if (Ip.IndexOf(",") != -1)
                    Ip = Ip.Substring(0, Ip.IndexOf(","));

                WebClient client = new WebClient();
                string uri = Utility.DecodeStr(Tool.GetAppSetting("IPApi")) + Ip;
                string jsonData = client.DownloadString(uri);

                IpCheckResult result = JsonConvert.DeserializeObject<IpCheckResult>(jsonData);
                if (result.code == 0 && result.data.city != "")
                {
                    string sql = "UPDATE Sys_Log SET IPCity = '" + result.data.city + "' WHERE nID=" + Id;
                    SqlHelper.SqlHelper.ExecuteUpdate(SqlHelper.SqlHelper.connstr, CommandType.Text, sql);
                }
            }
            catch (Exception)
            {

            }
        }

        delegate void IPSourceDelegate(string Id, string Ip);

        public static void IPSourceAction(string Id, string Ip)
        {
            IPSourceDelegate method = new IPSourceDelegate(IPSourceMethod);
            IAsyncResult ar = method.BeginInvoke(Id, Ip, null, null);
        }

        #region 异步发送短信
        
        /// <summary>
        /// 判断两个IP地址是否在同一个区域，如果可以获取到城市，比较城市是否一致，否则比较省份
        /// </summary>
        /// <param name="oldIP"></param>
        /// <param name="newIP"></param>
        /// <returns></returns>
        public static bool Ip_Addr_IsSameCity(string oldIP, string newIP)
        {
            //如果IP为空或者Ip格式不正确，都返回true，防止误发短信
            if (oldIP.Trim() == string.Empty || newIP == string.Empty)
            {
                return true;
            }
            Regex reg = new Regex(@"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$");
            if (!reg.IsMatch(oldIP) || !reg.IsMatch(newIP))
            {
                return true;
            }
            if (oldIP.Trim() == newIP.Trim())
            {
                return true;
            }
            long l_oldIp = Ip_Addr_IpToInt(oldIP);
            long l_newIp = Ip_Addr_IpToInt(newIP);
            string firstSection_old = oldIP.Substring(0, oldIP.IndexOf("."));
            string firstSection_new = newIP.Substring(0, newIP.IndexOf("."));
            string sql = @"select ProvinceCode,CityCode from Sys_ip_addr where FirstSection={0} and StartIP_Num<={1} and EndIP_Num>={1}";

            string new_pro = string.Empty;
            string new_city = string.Empty;
            string old_pro = string.Empty;
            string old_city = string.Empty;

            //获取之前IP地址的区域
            string sql_old = string.Format(sql, firstSection_old, l_oldIp);
            DataTable dt_old = SqlHelper.SqlHelper.ExecuteDataTable(SqlHelper.SqlHelper.connstr, CommandType.Text, sql_old);
            if (dt_old != null && dt_old.Rows.Count > 0)
            {
                old_pro = dt_old.Rows[0]["ProvinceCode"].ToString().Trim();
                old_city = dt_old.Rows[0]["CityCode"].ToString().Trim();
            }

            //获取新IP所在区域
            string sql_new = string.Format(sql, firstSection_new, l_newIp);
            DataTable dt_new = SqlHelper.SqlHelper.ExecuteDataTable(SqlHelper.SqlHelper.connstr, CommandType.Text, sql_new); ;
            if (dt_new != null && dt_new.Rows.Count > 0)
            {
                new_pro = dt_new.Rows[0]["ProvinceCode"].ToString().Trim();
                new_city = dt_new.Rows[0]["CityCode"].ToString().Trim();
            }

            //如果城市都不为空，比较城市;否则比较省份
            if (new_city != string.Empty && old_city != string.Empty)
            {
                if (new_city != old_city)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (new_pro != old_pro)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        public static long Ip_Addr_IpToInt(string ip)
        {
            char[] separator = new char[] { '.' };
            string[] items = ip.Split(separator);
            return long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
        }
        /// <summary>
        /// 根据IP地址获取城市，省份
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static DataTable Ip_Addr_GetCity(string ip)
        {
            long l_Ip = Ip_Addr_IpToInt(ip);
            string firstSection = ip.Substring(0, ip.IndexOf("."));

            string sql = @"select Province,City from Sys_ip_addr where FirstSection={0} and StartIP_Num<={1} and EndIP_Num>={1}";
            sql = string.Format(sql, firstSection, l_Ip);
            DataTable dt = SqlHelper.SqlHelper.ExecuteDataTable(SqlHelper.SqlHelper.connstr, CommandType.Text, sql);
            return dt;

        }
        #endregion

        public static bool SMS_AddLog(string userid, string num, string content, string categoryID, string status, string result)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat(
                "INSERT INTO	Sys_SMS_Log(CategoryID,Result,SendContent,SendNumber,SendUserID,Status) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')",
                categoryID, result, content, num, userid, Convert.ToInt16(status));
            int count = SqlHelper.SqlHelper.InsertAndReturnID(SqlHelper.SqlHelper.connstr, CommandType.Text,
                sql.ToString());
            if (count <= 0)
            {
                LogHelper.Info("系统短信--插入日志失败", OpType.Add, ModelType.BasicModule, null, "SMS_AddLog", "",
                    ViewLevel.AllVisible);
                return false;
            }
            return true;
        }

        public static string SMSParameters_GetValueByCode(string code)
        {
            string sql = " select Value from Sys_SMS_Search_Condition where Code=" + code;
            object o = SqlHelper.SqlHelper.ExecuteScalar(SqlHelper.SqlHelper.connstr, CommandType.Text, sql);
            return ConvertVal.GetValStr(o);
        }

       
        public static string GetIpLocation(string ipStr)
        {
            // var ipSearch = new IpSearch(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QQWry.dat"));
            //var location= ipSearch.GetIPLocation(ipStr);
            // return location.Country + "-" + location.Area;
            var location = IPLocation.IPLocation.IPLocate(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "QQWry.dat"), ipStr);
            return location?.Trim() ?? "";
        }



       

        public static void InsertExIp(string loginId, string Ip, string date)
        {
            string sql = @" insert into Sys_ExceptionLoginIP (LoginId,LoginIP,JoinTime,ExAddress) values
                           ('" + loginId + "','" + Ip + "','" + date + "','" + Address + "')";
            SqlHelper.SqlHelper.executeNonQuery(SqlHelper.SqlHelper.connstr, sql, CommandType.Text);
        }
       

        private static string GetData(string url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            myRequest.Method = "GET";
            HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
            StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string content = reader.ReadToEnd();
            reader.Close();
            return content;
        }
        /// <summary>
        ///  获取ID 地址信息 Json 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static string GetIPAddress(string ip)
        {
            String method = "GET";
            String bodys = "";
            String url = string.Empty;
            HttpWebRequest httpRequest = null;
            HttpWebResponse httpResponse = null;
            if (0 < ip.Length)
            {
                url = Utility.DecodeStr(GetAppSetting("IPApi"))+"?ip="+ip;
            }

            if (url.Contains("https://"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            }
            else
            {
                httpRequest = (HttpWebRequest)WebRequest.Create(url);
            }
            httpRequest.Method = method;
            httpRequest.Headers.Add("Authorization", "APPCODE " + Utility.DecodeStr(GetAppSetting("ALiBaBaIpCode")));
            if (0 < bodys.Length)
            {
                byte[] data = Encoding.UTF8.GetBytes(bodys);
                using (Stream stream = httpRequest.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            try
            {
                httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            }
            catch (WebException ex)
            {
                httpResponse = (HttpWebResponse)ex.Response;
            }
            Stream st = httpResponse.GetResponseStream();
            StreamReader reader = new StreamReader(st, Encoding.GetEncoding("utf-8"));
            return reader.ReadToEnd();
        }
        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
