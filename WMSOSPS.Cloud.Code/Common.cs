using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Collections;
using WMSOSPS.Cloud.Code.Operator;
using System.Reflection;


namespace WMSOSPS.Cloud.Code
{
    /// <summary>
    /// 常用公共类
    /// </summary>
    public class Common
    {
        #region Stopwatch计时器
        /// <summary>
        /// 计时器开始
        /// </summary>
        /// <returns></returns>
        public static Stopwatch TimerStart()
        {
            Stopwatch watch = new Stopwatch();
            watch.Reset();
            watch.Start();
            return watch;
        }
        /// <summary>
        /// 计时器结束
        /// </summary>
        /// <param name="watch"></param>
        /// <returns></returns>
        public static string TimerEnd(Stopwatch watch)
        {
            watch.Stop();
            double costtime = watch.ElapsedMilliseconds;
            return costtime.ToString();
        }
        #endregion

        #region 删除数组中的重复项
        /// <summary>
        /// 删除数组中的重复项
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string[] RemoveDup(string[] values)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < values.Length; i++)//遍历数组成员
            {
                if (!list.Contains(values[i]))
                {
                    list.Add(values[i]);
                };
            }
            return list.ToArray();
        }
        #endregion

        #region 自动生成编号
        /// <summary>
        /// 表示全局唯一标识符 (GUID)。
        /// </summary>
        /// <returns></returns>
        public static string GuId()
        {
            return Guid.NewGuid().ToString("N");
        }
        /// <summary>
        /// 自动生成编号  201008251145409865
        /// </summary>
        /// <returns></returns>
        public static string CreateNo()
        {
            Random random = new Random();
            string strRandom = random.Next(1000, 10000).ToString(); //生成编号 
            string code = DateTime.Now.ToString("yyyyMMddHHmmss") + strRandom;//形如
            return code;
        }
        #endregion

        #region 生成0-9随机数
        /// <summary>
        /// 生成0-9随机数
        /// </summary>
        /// <param name="codeNum">生成长度</param>
        /// <returns></returns>
        public static string RndNum(int codeNum)
        {
            StringBuilder sb = new StringBuilder(codeNum);
            Random rand = new Random();
            for (int i = 1; i < codeNum + 1; i++)
            {
                int t = rand.Next(9);
                sb.AppendFormat("{0}", t);
            }
            return sb.ToString();

        }
        #endregion

        #region 删除最后一个字符之后的字符
        /// <summary>
        /// 删除最后结尾的一个逗号
        /// </summary>
        public static string DelLastComma(string str)
        {
            return str.Substring(0, str.LastIndexOf(","));
        }
        /// <summary>
        /// 删除最后结尾的指定字符后的字符
        /// </summary>
        public static string DelLastChar(string str, string strchar)
        {
            return str.Substring(0, str.LastIndexOf(strchar));
        }
        /// <summary>
        /// 删除最后结尾的长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string DelLastLength(string str, int Length)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            str = str.Substring(0, str.Length - Length);
            return str;
        }
        #endregion

        
        /// <summary>
        /// 新系统的落地信息
        /// </summary>
        public static Dictionary<string, string> PBXs = new Dictionary<string, string>();

        /// <summary>
        /// 将字符串转换为键值对
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        public static Dictionary<string, string> ConvertPostList(string items)
        {
            items = items.Replace("\"", "'");
            JObject jo = (JObject)JsonConvert.DeserializeObject(items);
            Dictionary<string, string> parList = new Dictionary<string, string>();
            foreach (var item in jo)
            {
                parList.Add(item.Key, item.Value.ToString());
            }
            return parList;
        }


        #region APP通知
        /// <summary>
        /// 业务大类编号
        /// </summary>
        public enum NoticeCategory
        {
            号码预占 = 101,
            号码提单 = 102,
            预警提醒 = 103
        }
        /// <summary>
        /// 业务类型编号
        /// </summary>
        public enum NoticeType
        {
            号码预查询结果 = 10101,
            号码预占结果 = 10102,
            提单审核结果 = 10201,
            号码启用结果 = 10202,
            到期预警 = 10301,
            余额不足提醒 = 10302
        }
        /// <summary>
        /// 通知对象类型
        /// </summary>
        public enum NoticeTarget
        {
            代理商助理客服终端用户 = 0,
            仅代理商 = 1,
            仅终端用户 = 2
        }

        /// <summary>
        /// app通知
        /// </summary>
        public class APPNotice
        {
            /// <summary>
            /// 业务大类编号
            /// </summary>
            public NoticeCategory NoticeCategory { get; set; }
            /// <summary>
            /// 业务类型编号
            /// </summary>
            public NoticeType NoticeSonCategory { get; set; }
            /// <summary>
            /// 通知对象类型
            /// </summary>
            public NoticeTarget NoticeTargetType { get; set; }
            /// <summary>
            /// 通知代理商
            /// </summary>
            public string NoticeAgent { get; set; }
            /// <summary>
            /// 通知助理/客服
            /// </summary>
            public string NoticeAssistant { get; set; }
            /// <summary>
            /// 通知终端用户
            /// </summary>
            public string NoticeEnterprise { get; set; }
            /// <summary>
            /// 处理对象
            /// </summary>
            public string HandleTarget { get; set; }
            /// <summary>
            /// 处理结果
            /// </summary>
            public string HandleResult { get; set; }
            /// <summary>
            /// 处理消息
            /// </summary>
            public string HandleMessage { get; set; }
            /// <summary>
            /// 完成时间
            /// </summary>
            public string ActionTime { get; set; }
            /// <summary>
            /// 时间戳
            /// </summary>
            public string TimeStamp { get; set; }

        }
        #endregion


        #region 内存数据
        //基础信息列表
        public static Dictionary<string, object> BIBasicList = new Dictionary<string, object>();
        #endregion

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetStatus(int status, int IsAudit)
        {
            string name;
            switch (status)
            {
                case 0:
                    name = "已启用"; break;
                case 1:
                    {
                        if (IsAudit == 1)
                            name = "预查询";
                        else
                            name = "未预占";
                    }
                    break;
                case 2:
                    name = "停用"; break;
                case 3:
                    name = "已使用"; break;
                case 4:
                    name = "预占"; break;
                case 5:
                    name = "提交申请"; break;
                case 6:
                    name = "审核通过"; break;
                case 7:
                    name = "预占申请"; break;
                case 8:
                    name = "不可用"; break;
                case 9:
                    name = "暂未审批"; break;
                case 15:
                    name = "预占1"; break;
                case 16:
                    name = "已用"; break;
                case 17:
                    name = "预查询"; break;
                case 18:
                    name = "推广预占"; break;
                case 19:
                    name = "已拆机"; break;
                default:
                    name = ""; break;
            }
            return name;
        }




 
        /// <summary>
        /// 验证字符是否为数字
        /// </summary>
        /// <param name="str">要验证的字符</param>
        /// <returns>验证通过返回true 否则返回false</returns>
        public static bool IsInt(string str)
        {
            int a;
            try
            {
                return int.TryParse(str, out a);
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region 取得某月的第一天0分0秒
        /// <summary>
        /// 取得某月的第一天0分0秒
        /// </summary>
        /// <param name="datetime">要取得月份的某一天</param>
        /// <returns></returns>
        public static DateTime FirstDayOfMonth(DateTime datetime)
        {
            return datetime.AddMonths(-1).Date.AddDays(1 - datetime.Day).AddMonths(1);
        }
        #endregion

        #region 取得某月的最后一天59分59秒
        /// <summary>
        /// 取得某月的最后一天59分59秒
        /// </summary>
        /// <param name="datetime">要取得月份的某一天</param>
        /// <returns></returns>
        public static DateTime LastDayOfMonth(DateTime datetime)
        {
            return DateTime.Parse(datetime.AddDays(1 - datetime.Day).AddMonths(1).ToShortDateString()).AddSeconds(-1);
        }
        #endregion

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool CheckWorkID(string userName)
        {
            var xDoc = new XmlHelper.XmlHelper("~/xml/AgentWorkId.xml");
            string agentworkids = xDoc.GetValue("WorkerID");//排除的代理商id
            agentworkids = Tools.Utility.DecodeStr(agentworkids);
            return agentworkids.Contains(userName);
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool CheckLocalAgentWorkID(string userName)
        {
            var xDoc = new XmlHelper.XmlHelper("~/xml/AgentWorkId.xml");
            string agentworkids = xDoc.GetValue("OnlinePayWorkerID");//直营代理商id
            agentworkids = Tools.Utility.DecodeStr(agentworkids);
            if (agentworkids.Contains(userName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证是否是业务中心一
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool CheckWorkIdOne(string userName)
        {
            var xDoc = new XmlHelper.XmlHelper("~/xml/AgentWorkId.xml");
            string agentworkids = xDoc.GetValue("OnlinePayWorkerIDForOne");//直营代理商id
            agentworkids = Tools.Utility.DecodeStr(agentworkids);
            return agentworkids.Contains(userName);
        }

        /// <summary>
        /// 验证是否是业务中心二
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool CheckWorkIdTwo(string userName)
        {
            var xDoc = new XmlHelper.XmlHelper("~/xml/AgentWorkId.xml");
            string agentworkids = xDoc.GetValue("OnlinePayWorkerIDForTwoGetPhone");//直营代理商id
            agentworkids = Tools.Utility.DecodeStr(agentworkids);
            return agentworkids.Contains(userName);
        }
        /// <summary>
        /// 三
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool CheckWorkIdThree(string userName)
        {
            var xDoc = new XmlHelper.XmlHelper("~/xml/AgentWorkId.xml");
            string agentworkids = xDoc.GetValue("OnlinePayWorkerIDForThree");//直营代理商id
            agentworkids = Tools.Utility.DecodeStr(agentworkids);
            return agentworkids.Contains(userName);
        }
        #region List转DataTable
        public static DataTable ToDataTable<T>(List<T> items)
        {
            var tb = new DataTable(typeof(T).Name);

            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                Type t = GetCoreType(prop.PropertyType);
                tb.Columns.Add(prop.Name, t);
            }

            foreach (T item in items)
            {
                var values = new object[props.Length];

                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                tb.Rows.Add(values);
            }

            return tb;
        }

        /// <summary>
        /// Determine of specified type is nullable
        /// </summary>
        public static bool IsNullable(Type t)
        {
            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        /// <summary>
        /// Return underlying type if type is Nullable otherwise return the type
        /// </summary>
        public static Type GetCoreType(Type t)
        {
            if (t != null && IsNullable(t))
            {
                if (!t.IsValueType)
                {
                    return t;
                }
                else
                {
                    return Nullable.GetUnderlyingType(t);
                }
            }
            else
            {
                return t;
            }
        }
        #endregion
    }
}
