namespace SS.CMS.Plugin
{
    public partial interface IResponse
    {
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
    }
}
