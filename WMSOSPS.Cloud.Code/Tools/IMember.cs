using WMSOSPS.Cloud.Code.Enum;

namespace WMSOSPS.Cloud.Code.Tools
{
    /// <summary>
    /// 用户接口
    /// </summary>
    public interface IMember
    {
        /// <summary>
        /// 用户名
        /// </summary>
        string WorkerID
        {
            get;
            set;
        }
        /// <summary>
        /// 用户密码
        /// </summary>
        string WorkerPwd
        {
            get;
            set;
        }
        /// <summary>
        /// 助理id
        /// </summary>
        string AssistantID
        {
            get;
            set;
        }
        /// <summary>
        /// 角色编码
        /// </summary>
        string Role_Code
        {
            get;
            set;
        }
        /// <summary>
        /// 角色名称
        /// </summary>
        string Role_Name
        {
            get;
            set;
        }
        /// <summary>
        /// 菜单权限编码
        /// </summary>
        string[] MenuArray
        {
            get;
            set;
        }
        /// <summary>
        /// 登录用户类型
        /// </summary>
        LoginTypeEnum LoginType
        {
            get;
            set;
        }

        string WorkerCode
        {
            get;
            set;
        }
        string Parent_kf
        {
            get;
            set;
        }
        string Parent_WorkerID
        {
            get;
            set;
        }
        string LoginFlag
        {
            get;
            set;
        }
        string LastIp { get; set; }
        string CurrentIp { get; set; }
        /// <summary>
        /// 获取当前登录用户菜单权限
        /// </summary>
        /// <returns></returns>
        //string[] GetCurrentMenuPermission();

        /// <summary>
        /// 判断当前用户是否具有某方法权限
        /// </summary>
        /// <param name="fn"></param>
        /// <returns></returns>
        //bool IsMethodPermission(FnCode fn);

        //手机号码，获取短信验证码 新加 2014-09-18
        string MoPhone { get; set; }

        // 登陆方式，1:需要短信验证码，0：不需短信验证码 新加 2014-09-18
        byte IsSMSLogin { get; set; }
        string QQ { get; set; }//新加 2014-09-18 wolf

        string Address { get; set; }
    }
}
