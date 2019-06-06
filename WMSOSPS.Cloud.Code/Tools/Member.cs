using System;
using WMSOSPS.Cloud.Code.Enum;

namespace WMSOSPS.Cloud.Code.Tools
{
    [Serializable]
    public class Member : IMember
    {
        /// <summary>
        /// 用户构造
        /// </summary>
        /// <param name="workerId">用户名</param>
        /// <param name="roleCode">角色编码</param>
        /// <param name="roleName">角色名称</param>
        /// <param name="loginType">登录类型</param>
        /// <param name="menusArray">菜单权限编码数组</param>
        public Member(string workerId, string workerPwd, string roleCode, string roleName,
            LoginTypeEnum loginType, string[] rolePermission, string parent_WorkerID, string parent_kf, string workerCode, string workerName, string assistantID, string lastIp, string currentIp)
        {
            WorkerID = workerId;
            WorkerPwd = workerPwd;
            Role_Code = roleCode;
            Role_Name = roleName;
            LoginType = loginType;
            MenuArray = rolePermission;
            Parent_kf = parent_kf;
            WorkerCode = workerCode;
            Parent_WorkerID = parent_WorkerID;
            WorkerName = workerName;
            AssistantID = assistantID;
            LastIp = lastIp;
            CurrentIp = currentIp;

        }



        public Member(string workerId, string workerPwd, string roleCode, string roleName,
     LoginTypeEnum loginType, string[] rolePermission, string workerName, string assistantID, string lastIp, string currentIp)
        {
            WorkerID = workerId;
            WorkerPwd = workerPwd;
            Role_Code = roleCode;
            Role_Name = roleName;
            LoginType = loginType;
            MenuArray = rolePermission;
            WorkerName = workerName;
            AssistantID = assistantID;
            LastIp = lastIp;
            CurrentIp = currentIp;
        }


        //新加 2014-09-19，使用现在的方法，原来的留着
        public Member(string workerId, string workerPwd, string roleCode, string roleName,
LoginTypeEnum loginType, string[] rolePermission, string workerName, string assistantID, string lastIp, string currentIp, string moPhone, byte isSMSLogin)
        {
            WorkerID = workerId;
            WorkerPwd = workerPwd;
            Role_Code = roleCode;
            Role_Name = roleName;
            LoginType = loginType;
            MenuArray = rolePermission;
            WorkerName = workerName;
            AssistantID = assistantID;
            LastIp = lastIp;
            CurrentIp = currentIp;
            MoPhone = moPhone;
            IsSMSLogin = isSMSLogin;
            
        }

        //新加 2014-09-19，使用现在的方法，原来的留着
        public Member(string workerId, string workerPwd, string roleCode, string roleName,
            LoginTypeEnum loginType, string[] rolePermission, string parent_WorkerID, string parent_kf, string workerCode, string workerName, string assistantID, string lastIp, string currentIp, string moPhone, byte isSMSLogin)
        {
            WorkerID = workerId;
            WorkerPwd = workerPwd;
            Role_Code = roleCode;
            Role_Name = roleName;
            LoginType = loginType;
            MenuArray = rolePermission;
            Parent_kf = parent_kf;
            WorkerCode = workerCode;
            Parent_WorkerID = parent_WorkerID;
            WorkerName = workerName;
            AssistantID = assistantID;
            LastIp = lastIp;
            CurrentIp = currentIp;
            MoPhone = moPhone;
            IsSMSLogin = isSMSLogin;
        }

        #region IMember 成员

        public string WorkerID
        {
            get;
            set;
        }
        public string WorkerPwd
        {
            get;
            set;
        }
        public string AssistantID
        {
            get;
            set;
        }
        public string Role_Code
        {
            get;
            set;
        }

        public string Role_Name
        {
            get;
            set;
        }

        public string[] MenuArray
        {
            get;
            set;
        }

        public LoginTypeEnum LoginType
        {
            get;
            set;
        }

        public string WorkerCode
        {
            get;
            set;
        }
        public string Parent_kf
        {
            get;
            set;
        }
        public string Parent_WorkerID
        {
            get;
            set;
        }
        public string WorkerName
        {
            get;
            set;
        }

        //public bool IsMethodPermission(FnCode fn)
        //{
        //    throw new NotImplementedException();
        //}

        public string LoginFlag
        {
            get;
            set;
        }
        public string LastIp { get; set; }
        public string CurrentIp { get; set; }
        #endregion

        #region IMember 成员

        //手机号码，获取短信验证码 新加 2014-09-18
        public string MoPhone
        {
            get;
            set;
        }

        // 登陆方式，1:需要短信验证码，0：不需短信验证码 新加 2014-09-18
        public byte IsSMSLogin
        {
            get;
            set;
        }

        #endregion

        #region IMember 成员

        //新加 2014-09-18 wolf
        public string QQ
        {
            get;
            set;
        }

        #endregion

        public string Address { get; set; }
    }
}
