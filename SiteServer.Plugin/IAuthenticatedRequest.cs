namespace SiteServer.Plugin
{
    /// <summary>
    /// 表示与请求关联的上下文。
    /// </summary>
    public interface IAuthenticatedRequest
    {
        /// <summary>
        /// 判断用户是否登录。
        /// </summary>
        bool IsUserLoggin { get; }

        /// <summary>
        /// 如果用户已登录，则返回登录用户的Id；否则返回 0。
        /// </summary>
        int UserId { get; }

        /// <summary>
        /// 如果用户已登录，则返回登录用户的用户名；否则返回空。
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 当前登录前台用户的权限。
        /// </summary>
        IPermissions UserPermissions { get; }

        /// <summary>
        /// 用户登录，调用此方法后系统将计算此用户的Token并存储在cookie中，同时此方法将返回用户Token，用于REST Api以及其他场景中。
        /// </summary>
        /// <param name="userName">登录用户的用户名。</param>
        /// <param name="isAutoLogin">
        /// 是否下次自动登录。
        /// 如果设置为 true，则登录cookie将保留7天；否则当浏览器关闭时系统将清除登录cookie。
        /// </param>
        /// <returns>返回此用户的Token。</returns>
        string UserLogin(string userName, bool isAutoLogin);

        /// <summary>
        /// 用户退出登录，调用此方法后系统将清除登录cookie。
        /// </summary>
        void UserLogout();

        /// <summary>
        /// 判断管理员是否登录。
        /// </summary>
        bool IsAdminLoggin { get; }

        /// <summary>
        /// 如果管理员已登录，则返回登录管理员的Id；否则返回 0。
        /// </summary>
        int AdminId { get; }

        /// <summary>
        /// 如果管理员已登录，则返回登录管理员的用户名；否则返回空。
        /// </summary>
        string AdminName { get; }

        /// <summary>
        /// 当前登录后台管理员的权限。
        /// </summary>
        IPermissions AdminPermissions { get; }

        /// <summary>
        /// 管理员登录，调用此方法后系统将计算此管理员的Token并存储在cookie中，同时此方法将返回管理员Token，用于REST Api以及其他场景中。
        /// </summary>
        /// <param name="userName">登录管理员的用户名。</param>
        /// <param name="isAutoLogin">
        /// 是否下次自动登录。
        /// 如果设置为 true，则登录cookie将保留7天；否则当浏览器关闭时系统将清除登录cookie。
        /// </param>
        /// <returns>返回此管理员的Token。</returns>
        string AdminLogin(string userName, bool isAutoLogin);

        /// <summary>
        /// 管理员退出登录，调用此方法后系统将清除登录cookie。
        /// </summary>
        void AdminLogout();

        bool IsQueryExists(string name);

        string GetQueryString(string name);

        int GetQueryInt(string name, int defaultValue = 0);

        decimal GetQueryDecimal(string name, decimal defaultValue = 0);

        bool GetQueryBool(string name, bool defaultValue = false);

        bool IsPostExists(string name);

        T GetPostObject<T>(string name = "");

        string GetPostString(string name);

        int GetPostInt(string name, int defaultValue = 0);

        decimal GetPostDecimal(string name, decimal defaultValue = 0);

        bool GetPostBool(string name, bool defaultValue = false);
    }
}