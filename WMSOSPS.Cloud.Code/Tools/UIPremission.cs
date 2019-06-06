using System;
using System.Collections.Generic;
using System.Web;
using WMSOSPS.Cloud.Code.Enum;

namespace WMSOSPS.Cloud.Code.Tools
{
    public class UIPremission
    {
        public const string CostSessionName = "CurrentUser";

        private IMember _currentUser;
        public IMember CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    if (HttpContext.Current.Session != null)
                        _currentUser = HttpContext.Current.Session[CostSessionName] as IMember;
                }
                return _currentUser;
            }
        }
        public bool IsLogin
        {
            get
            {
                return CurrentUser != null;
            }
        }
        //用于存放登陆过的用户和标示
        public static Dictionary<string, string> UserList = new Dictionary<string, string>();
        /// <summary>
        /// 保存当前Seesion
        /// </summary>
        /// <param name="member"></param>
        public static void SetCurrentUser(IMember member)
        {
            member.LoginFlag = DateTime.Now.ToString("yyyyMMddHHmmss");
            string mid = member.WorkerID;
            if (!string.IsNullOrEmpty(member.AssistantID))
                mid = member.AssistantID;
            if (UserList.ContainsKey(mid))
                UserList[mid] = member.LoginFlag;
            else
                UserList.Add(mid, member.LoginFlag);
            HttpContext.Current.Session[CostSessionName] = member;

            string strKind = "";
            string strRoleCode = member.Role_Code;
            switch (member.LoginType)
            {
                case LoginTypeEnum.ManageMember:
                    if (strRoleCode.Length == 1)
                        strKind = "0";//超级管理员
                    else if (strRoleCode.Length < 4)
                        strKind = "1";//管理员（包括平台客服和财务人员）
                    else
                    {
                        string rcode = strRoleCode.Substring(0, 2);
                        if (rcode == "04")
                            strKind = "4";//渠道
                        else
                            strKind = "5";//客服
                    }
                    break;
                case LoginTypeEnum.ExamMember:
                    strKind = "2";//代理商
                    break; 
            }
            HttpContext.Current.Session["NbearCurUser"] = strKind + "|" + member.WorkerID;
        }
    }
}
