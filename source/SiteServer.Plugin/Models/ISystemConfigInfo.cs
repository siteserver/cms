namespace SiteServer.Plugin.Models
{
    public interface ISystemConfigInfo
    {
        string WebUrl { get; }

        string ApiUrl { get; }

        /****************管理员设置********************/

        int AdminUserNameMinLength { get; }

        int AdminPasswordMinLength { get; }

        string AdminPasswordRestriction { get; }

        bool IsAdminLockLogin { get; }

        int AdminLockLoginCount { get; }

        string AdminLockLoginType { get; }

        int AdminLockLoginHours { get; }

        bool IsAdminFindPassword { get; }

        string AdminFindPasswordSmsTplId { get; }

        /****************用户设置********************/

        bool IsUserRegistrationAllowed { get; }

        int UserPasswordMinLength { get; }

        string UserPasswordRestriction { get; }

        string UserRegistrationVerifyType { get; }

        string UserRegistrationSmsTplId { get; }

        int UserRegistrationMinMinutes { get; }

        bool IsUserFindPassword { get; }

        string UserFindPasswordSmsTplId { get; }

        bool IsUserLockLogin { get; }

        int UserLockLoginCount { get; }

        string UserLockLoginType { get; }

        int UserLockLoginHours { get; }
    }
}
