 

using System;
using System.Collections.Generic;
using WMSOSPS.Cloud.Code.Enum;

namespace WMSOSPS.Cloud.Code.Operator
{
    public class OperatorModel
    {
        /// <summary>
        /// 登录人Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 登录人账号
        /// </summary>
        public string UserCode { get; set; }
        /// <summary>
        /// 登录人realName
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 登录人绑定的电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 用户密码
        /// </summary>
        public string UserPwd { get; set; }
        /// <summary>
        /// 公司
        /// </summary>
        public string CompanyId { get; set; }
        /// <summary>
        /// 部门
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public string RoleId { get; set; }
        /// <summary>
        /// 登录地址IP
        /// </summary>
        public string LoginIPAddress { get; set; }
        /// <summary>
        /// 登录IP对应的地址名
        /// </summary>
        public string LoginIPAddressName { get; set; }
        public string LoginToken { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// 是否是超级管理员
        /// </summary>
        public bool IsSystem { get; set; }
        /// <summary>
        /// 登录类型
        /// </summary>
        public LoginTypeEnum LoginType { get; set; }
        /// <summary>
        /// 上一次登录Ip
        /// </summary>
        public string LastIp { get; set; }
        /// <summary>
        /// 当前登录Ip
        /// </summary>
        public string CurrentIp { get; set; }
        /// <summary>
        /// 登录人qq
        /// </summary>
        public string QQ { get; set; }
        /// <summary>
        /// 角色对应的编号（主要为了区分上下级关系）（对应的Sys_user）
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// 渠道客服上下级关系
        /// </summary>
        public string WorkerLevelCode { get; set; }
        /// <summary>
        /// 确定代理商的上下级关系（唯一性，只有代理商才有）
        /// </summary>
        public string WorkerCode { get; set; }

        public string AssistantID { get; set; }

        /// <summary>
        /// 终端企业下包含的所有的400号码
        /// </summary>
        public List<string> EnterpriseList { get; set; }

        public Organize OrganizeModel { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public bool? Sex { get; set; }
        /// <summary>
        /// 岗位
        /// </summary>
        public string DutyId { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? F_Birthday { get; set; }
        /// <summary>
        /// 微信
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 微信头像
        /// </summary>
        public string Avator { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 登录标签
        /// </summary>
        public string LoginFlag { get; set; }

        public string IsCheck { get; set; }

        /// <summary>
        /// T_Enterprise表F_Code
        /// </summary>
        public string F_BelongTo { get; set; }
    }
}
