namespace SS.CMS.Abstractions
{
    public partial interface IRequest
    {
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

        IAdministratorInfo AdminInfo { get; }

        /// <summary>
        /// 当前登录后台管理员的权限。
        /// </summary>
        IPermissions AdminPermissions { get; }
    }
}
