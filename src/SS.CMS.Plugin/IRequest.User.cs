namespace SS.CMS.Plugin
{
    public partial interface IRequest
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

        IUserInfo UserInfo { get; }

        /// <summary>
        /// 当前登录前台用户的权限。
        /// </summary>
        IPermissions UserPermissions { get; }
    }
}
